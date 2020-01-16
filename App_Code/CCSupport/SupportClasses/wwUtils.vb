Imports Microsoft.VisualBasic
Imports System
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Reflection
Imports Microsoft.Win32

Imports System.Xml
Imports System.Xml.Serialization
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Runtime.InteropServices
Imports System.Globalization
Imports System.Web

Namespace NVMS.Tools

    ' <summary>
    ' wwUtils class which contains a set of common utility classes for 
    ' Formatting strings
    ' Reflection Helpers
    ' Object Serialization
    ' </summary>
    Public Class wwUtils

#Region "String Helper Functions"

        ' <summary>
        ' Replaces and  and Quote characters to HTML safe equivalents.
        ' </summary>
        ' <param name="Html">HTML to convert</param>
        ' <returns>Returns an HTML string of the converted text</returns>
        Public Shared Function FixHTMLForDisplay(ByVal Html As String) As String
            Html = Html.Replace("<", "&lt;")
            Html = Html.Replace(">", "&gt;")
            Html = Html.Replace("""", "&quot;")
            Return Html
        End Function

        ' <summary>
        ' Strips HTML tags out of an HTML string and returns just the text.
        ' </summary>
        ' <param name="Html">Html String</param>
        ' <returns></returns>
        Public Shared Function StripHtml(ByVal Html As String) As String
            Html = Regex.Replace(Html, "<(.|\n)*?>", String.Empty)
            Html = Html.Replace(Constants.vbTab, " ")
            Html = Html.Replace(Constants.vbCrLf, "")
            Html = Html.Replace("   ", " ")
            Return Html.Replace("  ", " ")
        End Function

        ' <summary>
        ' Fixes a plain text field for display as HTML by replacing carriage returns 
        ' with the appropriate br and p tags for breaks.
        ' </summary>
        ' <param name="String Text">Input string</param>
        ' <returns>Fixed up string</returns>
        Public Shared Function DisplayMemo(ByVal HtmlText As String) As String
            HtmlText = HtmlText.Replace(Constants.vbCrLf, Constants.vbCr)
            HtmlText = HtmlText.Replace(Constants.vbLf, Constants.vbCr)
            HtmlText = HtmlText.Replace(Constants.vbCr + Constants.vbCr, "<p>")
            HtmlText = HtmlText.Replace(Constants.vbCr, "<br>")
            Return HtmlText
        End Function
        ' <summary>
        ' Method that handles handles display of text by breaking text.
        ' Unlike the non-encoded version it encodes any embedded HTML text
        ' </summary>
        ' <param name="Text"></param>
        ' <returns></returns>
        Public Shared Function DisplayMemoEncoded(ByVal Text As String) As String
            Dim PreTag As Boolean = False
            If Text.IndexOf("<pre>") > -1 Then
                Text = Text.Replace("<pre>", "__pre__")
                Text = Text.Replace("</pre>", "__/pre__")
                PreTag = True
            End If

            ' *** fix up line breaks into <br><p>
            Text = NVMS.Tools.wwUtils.DisplayMemo(HttpUtility.HtmlEncode(Text))

            If PreTag Then
                Text = Text.Replace("__pre__", "<pre>")
                Text = Text.Replace("__/pre__", "</pre>")
            End If

            Return Text
        End Function

        ' <summary>
        ' Expands links into HTML hyperlinks inside of text or HTML.
        ' </summary>
        ' <param name="Text"></param>
        ' <returns></returns>
        Public Shared Function ExpandUrls(ByVal Text As String) As String
            ' *** Expand embedded hyperlinks
            Dim regex As String = "\b(((ftp|https?)://)?[-\w]+(\.\w[-\w]*)+|\w+\@|mailto:|[a-z0-9](?:[-a-z0-9]*[a-z0-9])?\.)+(com\b|edu\b|biz\b|gov\b|in(?:t|fo)\b|mil\b|net\b|org\b|[a-z][a-z]\b)(:\d+)?(/[-a-z0-9_:\@&?=+,.!/~*'%\$]*)*(?<![.,?!])(?!((?!(?:<a )).)*?(?:</a>))(?!((?!(?:<!--)).)*?(?:-->))"
            Dim options As System.Text.RegularExpressions.RegexOptions = ((System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace Or System.Text.RegularExpressions.RegexOptions.Multiline) Or System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            Dim reg As System.Text.RegularExpressions.Regex = New System.Text.RegularExpressions.Regex(regex, options)

            Dim MatchEval As MatchEvaluator = New MatchEvaluator(AddressOf ExpandUrlsRegExEvaluator)
            Return regex.Replace(Text, regex)

        End Function

        ' <summary>
        ' Internal RegExEvaluator callback
        ' </summary>
        ' <param name="M"></param>
        ' <returns></returns>
        Private Shared Function ExpandUrlsRegExEvaluator(ByVal M As System.Text.RegularExpressions.Match) As String
            Dim Href As String = M.Groups(0).Value
            Dim Text As String = Href

            If Href.IndexOf("://") < 0 Then
                If Href.StartsWith("www.") Then
                    Href = "http://" & Href
                ElseIf Href.StartsWith("ftp") Then
                    Href = "ftp://" & Href
                ElseIf Href.IndexOf("@") > -1 Then
                    Href = "mailto://" & Href
                End If
            End If
            Return "<a href='" & Href & "'>" & Text & "</a>"
        End Function

        ' <summary>
        ' Extracts a string from between a pair of delimiters. Only the first 
        ' instance is found.
        ' </summary>
        ' <param name="Source">Input String to work on</param>
        ' <param name="StartDelim">Beginning delimiter</param>
        ' <param name="EndDelim">ending delimiter</param>
        ' <param name="CaseInsensitive">Determines whether the search for delimiters is case sensitive</param>
        ' <returns>Extracted string or ""</returns>
        Public Shared Function ExtractString(ByVal Source As String, ByVal BeginDelim As String, ByVal EndDelim As String, ByVal CaseInSensitive As Boolean, ByVal AllowMissingEndDelimiter As Boolean) As String
            Dim At1, At2 As Integer

            If Source Is Nothing OrElse Source.Length < 1 Then
                Return ""
            End If

            If CaseInSensitive Then
                At1 = Source.IndexOf(BeginDelim)
                At2 = Source.IndexOf(EndDelim, At1 + BeginDelim.Length)
            Else
                Dim Lower As String = Source.ToLower()
                At1 = Lower.IndexOf(BeginDelim.ToLower())
                At2 = Lower.IndexOf(EndDelim.ToLower(), At1 + BeginDelim.Length)
            End If

            If AllowMissingEndDelimiter AndAlso At2 = -1 Then
                Return Source.Substring(At1 + BeginDelim.Length)
            End If

            If At1 > -1 AndAlso At2 > 1 Then
                Return Source.Substring(At1 + BeginDelim.Length, At2 - At1 - BeginDelim.Length)
            End If

            Return ""
        End Function

        ' <summary>
        ' Extracts a string from between a pair of delimiters. Only the first
        ' instance is found.
        ' <seealso>Class wwUtils</seealso>
        ' </summary>
        ' <param name="Source">
        ' Input String to work on
        ' </param>
        ' <param name="BeginDelim"></param>
        ' <param name="EndDelim">
        ' ending delimiter
        ' </param>
        ' <param name="CaseInSensitive"></param>
        ' <returns>String</returns>
        Public Shared Function ExtractString(ByVal Source As String, ByVal BeginDelim As String, ByVal EndDelim As String, ByVal CaseInSensitive As Boolean) As String
            Return ExtractString(Source, BeginDelim, EndDelim, False, False)
        End Function

        ' <summary>
        ' Extracts a string from between a pair of delimiters. Only the first 
        ' instance is found. Search is case insensitive.
        ' </summary>
        ' <param name="Source">
        ' Input String to work on
        ' </param>
        ' <param name="StartDelim">
        ' Beginning delimiter
        ' </param>
        ' <param name="EndDelim">
        ' ending delimiter
        ' </param>
        ' <returns>Extracted string or ""</returns>
        Public Shared Function ExtractString(ByVal Source As String, ByVal BeginDelim As String, ByVal EndDelim As String) As String
            Return wwUtils.ExtractString(Source, BeginDelim, EndDelim, False, False)
        End Function


        ' <summary>
        ' Determines whether a string is empty (null or zero length)
        ' </summary>
        ' <param name="String">Input string</param>
        ' <returns>true or false</returns>
        Public Shared Function Empty(ByVal [String] As [String]) As Boolean
            If [String] Is Nothing OrElse [String].Trim().Length = 0 Then
                Return True
            End If

            Return False
        End Function

        ' <summary>
        ' Determines wheter a string is empty (null or zero length)
        ' </summary>
        ' <param name="StringValue">Input string (in object format)</param>
        ' <returns>true or false/returns>
        Public Shared Function Empty(ByVal StringValue As Object) As Boolean
            Dim [String] As [String] = CStr(StringValue)
            If [String] Is Nothing OrElse [String].Trim().Length = 0 Then
                Return True
            End If

            Return False
        End Function

        ' <summary>
        ' Return a string in proper Case format
        ' </summary>
        ' <param name="Input"></param>
        ' <returns></returns>
        Public Shared Function ProperCase(ByVal Input As String) As String
            Return System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(Input)
        End Function

        ' <summary>
        ' Returns an abstract of the provided text by returning up to Length characters
        ' of a text string. If the text is truncated a ... is appended.
        ' </summary>
        ' <param name="Text">Text to abstract</param>
        ' <param name="Length">Number of characters to abstract to</param>
        ' <returns>string</returns>
        Public Shared Function TextAbstract(ByVal Text As String, ByVal Length As Integer) As String
            If Text.Length <= Length Then
                Return Text
            End If

            Text = Text.Substring(0, Length)

            Text = Text.Substring(0, Text.LastIndexOf(" "))
            Return Text & "..."
        End Function

        ' <summary>
        ' Creates an Abstract from an HTML document. Strips the 
        ' HTML into plain text, then creates an abstract.
        ' </summary>
        ' <param name="Html"></param>
        ' <returns></returns>
        Public Shared Function HtmlAbstract(ByVal Html As String, ByVal Length As Integer) As String
            Return TextAbstract(StripHtml(Html), Length)
        End Function


        ' <summary>
        ' Expands URLs into Href links
        ' </summary>
        ' <param name="Text"></param>
        ' <returns></returns>
        Public Shared Function ExpandUrls(ByVal TextToExpand As String, ByVal Target As String) As String
            If Target Is Nothing Then
                Target = ""
            Else
                Target = "target=""" & Target & """"
            End If

            Dim pattern As String = "(http|ftp|https):\/\/[\w]+(.[\w]+)([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])"
            Dim Matches As MatchCollection

            Matches = Regex.Matches(TextToExpand, pattern, RegexOptions.IgnoreCase Or RegexOptions.Compiled)
            For Each m As Match In Matches
                Dim Url As String = m.ToString()

                TextToExpand = TextToExpand.Replace(Url, "<a " & Target & "href=""" & Url & """>" & Url & "</a>")
            Next m
            Return TextToExpand
        End Function
#End Region

#Region "UrlEncoding and UrlDecoding without System.Web"
        ' <summary>
        ' UrlEncodes a string without the requirement for System.Web
        ' </summary>
        ' <param name="String"></param>
        ' <returns></returns>
        Public Shared Function UrlEncode(ByVal InputString As String) As String
            Dim sr As StringReader = New StringReader(InputString)
            Dim sb As StringBuilder = New StringBuilder(InputString.Length)

            Do
                Dim Value As Integer = sr.Read()
                If Value = -1 Then
                    Exit Do
                End If
                Dim CharValue As Char = ChrW(Value)

                If CharValue >= "a"c AndAlso CharValue <= "z"c OrElse CharValue >= "A"c AndAlso CharValue <= "Z"c OrElse CharValue >= "0"c AndAlso CharValue <= "9"c Then
                    sb.Append(CharValue)
                ElseIf CharValue = " "c Then
                    sb.Append("+")
                Else
                    sb.AppendFormat("%{0:X2}", Value)
                End If
            Loop

            Return sb.ToString()
        End Function

        ' <summary>
        ' UrlDecodes a string without requiring System.Web
        ' </summary>
        ' <param name="InputString">String to decode.</param>
        ' <returns>decoded string</returns>
        Public Shared Function UrlDecode(ByVal InputString As String) As String
            Dim temp As Char = " "c
            Dim sr As StringReader = New StringReader(InputString)
            Dim sb As StringBuilder = New StringBuilder(InputString.Length)

            Do
                Dim lnVal As Integer = sr.Read()
                If lnVal = -1 Then
                    Exit Do
                End If
                Dim TChar As Char = ChrW(lnVal)
                If TChar = "+"c Then
                    sb.Append(" "c)
                ElseIf TChar = "%"c Then
                    ' *** read the next 2 chars and parse into a char
                    temp = CChar(ChrW(Int32.Parse((CChar(ChrW(sr.Read()))).ToString() & (CChar(ChrW(sr.Read()))).ToString(), System.Globalization.NumberStyles.HexNumber)))
                    sb.Append(temp)
                Else
                    sb.Append(TChar)
                End If
            Loop

            Return sb.ToString()
        End Function

        ' <summary>
        ' Retrieves a value by key from a UrlEncoded string.
        ' </summary>
        ' <param name="UrlEncodedString">UrlEncoded String</param>
        ' <param name="Key">Key to retrieve value for</param>
        ' <returns>returns the value or "" if the key is not found or the value is blank</returns>
        Public Shared Function GetUrlEncodedKey(ByVal UrlEncodedString As String, ByVal Key As String) As String
            UrlEncodedString = "&" & UrlEncodedString & "&"

            Dim Index As Integer = UrlEncodedString.ToLower().IndexOf("&" & Key.ToLower() & "=")
            If Index < 0 Then
                Return ""
            End If

            Dim lnStart As Integer = Index + 2 + Key.Length

            Dim Index2 As Integer = UrlEncodedString.IndexOf("&", lnStart)
            If Index2 < 0 Then
                Return ""
            End If

            Return UrlDecode(UrlEncodedString.Substring(lnStart, Index2 - lnStart))
        End Function
#End Region

#Region "Reflection Helper Code"
        ' <summary>
        ' Binding Flags constant to be reused for all Reflection access methods.
        ' </summary>
        Public Const MemberAccess As BindingFlags = BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.Static Or BindingFlags.Instance Or BindingFlags.IgnoreCase



        ' <summary>
        ' Retrieve a dynamic 'non-typelib' property
        ' </summary>
        ' <param name="Object">Object to make the call on</param>
        ' <param name="Property">Property to retrieve</param>
        ' <returns>Object - cast to proper type</returns>
        Public Shared Function GetProperty(ByVal [Object] As [Object], ByVal [Property] As String) As [Object]
            Return [Object].GetType().GetProperty([Property], wwUtils.MemberAccess).GetValue([Object], Nothing)
        End Function

        ' <summary>
        ' Retrieve a dynamic 'non-typelib' field
        ' </summary>
        ' <param name="Object">Object to retreve Field from</param>
        ' <param name="Property">name of the field to retrieve</param>
        ' <returns></returns>
        Public Shared Function GetField(ByVal [Object] As [Object], ByVal [Property] As String) As [Object]
            Return [Object].GetType().GetField([Property], wwUtils.MemberAccess).GetValue([Object])
        End Function

        ' <summary>
        ' Returns a property or field value using a base object and sub members including . syntax.
        ' For example, you can access: this.oCustomer.oData.Company with (this,"oCustomer.oData.Company")
        ' </summary>
        ' <param name="Parent">Parent object to 'start' parsing from.</param>
        ' <param name="Property">The property to retrieve. Example: 'oBus.oData.Company'</param>
        ' <returns></returns>
        Public Shared Function GetPropertyEx(ByVal Parent As Object, ByVal [Property] As String) As Object
            Dim Member As MemberInfo = Nothing

            Dim Type As Type = Parent.GetType()

            Dim lnAt As Integer = [Property].IndexOf(".")
            If lnAt < 0 Then
                If [Property] = "this" OrElse [Property] = "me" Then
                    Return Parent
                End If

                ' *** Get the member
                Member = Type.GetMember([Property], wwUtils.MemberAccess)(0)
                If Member.MemberType = MemberTypes.Property Then
                    Return (CType(Member, PropertyInfo)).GetValue(Parent, Nothing)
                Else
                    Return (CType(Member, FieldInfo)).GetValue(Parent)
                End If
            End If

            ' *** Walk the . syntax - split into current object (Main) and further parsed objects (Subs)
            Dim Main As String = [Property].Substring(0, lnAt)
            Dim Subs As String = [Property].Substring(lnAt + 1)

            ' *** Retrieve the current property
            Member = Type.GetMember(Main, wwUtils.MemberAccess)(0)

            Dim [Sub] As Object
            If Member.MemberType = MemberTypes.Property Then
                ' *** Get its value
                [Sub] = (CType(Member, PropertyInfo)).GetValue(Parent, Nothing)

            Else
                [Sub] = (CType(Member, FieldInfo)).GetValue(Parent)

            End If

            ' *** Recurse further into the sub-properties (Subs)
            Return wwUtils.GetPropertyEx([Sub], Subs)
        End Function

        ' <summary>
        ' Sets the property on an object.
        ' </summary>
        ' <param name="Object">Object to set property on</param>
        ' <param name="Property">Name of the property to set</param>
        ' <param name="Value">value to set it to</param>
        Public Shared Sub SetProperty(ByVal [Object] As [Object], ByVal [Property] As String, ByVal Value As [Object])
            [Object].GetType().GetProperty([Property], wwUtils.MemberAccess).SetValue([Object], Value, Nothing)
        End Sub

        ' <summary>
        ' Sets the field on an object.
        ' </summary>
        ' <param name="Object">Object to set property on</param>
        ' <param name="Property">Name of the field to set</param>
        ' <param name="Value">value to set it to</param>
        Public Shared Sub SetField(ByVal [Object] As [Object], ByVal [Property] As String, ByVal Value As [Object])
            [Object].GetType().GetField([Property], wwUtils.MemberAccess).SetValue([Object], Value)
        End Sub

        ' <summary>
        ' Sets the value of a field or property via Reflection. This method alws 
        ' for using '.' syntax to specify objects multiple levels down.
        ' 
        ' wwUtils.SetPropertyEx(this,"Invoice.LineItemsCount",10)
        ' 
        ' which would be equivalent of:
        ' 
        ' this.Invoice.LineItemsCount = 10;
        ' </summary>
        ' <param name="Object Parent">
        ' Object to set the property on.
        ' </param>
        ' <param name="String Property">
        ' Property to set. Can be an object hierarchy with . syntax.
        ' </param>
        ' <param name="Object Value">
        ' Value to set the property to
        ' </param>
        Public Shared Function SetPropertyEx(ByVal Parent As Object, ByVal [Property] As String, ByVal Value As Object) As Object
            Dim Type As Type = Parent.GetType()
            Dim Member As MemberInfo = Nothing

            ' *** no more .s - we got our final object
            Dim lnAt As Integer = [Property].IndexOf(".")
            If lnAt < 0 Then
                Member = Type.GetMember([Property], wwUtils.MemberAccess)(0)
                If Member.MemberType = MemberTypes.Property Then

                    CType(Member, PropertyInfo).SetValue(Parent, Value, Nothing)
                    Return Nothing
                Else
                    CType(Member, FieldInfo).SetValue(Parent, Value)
                    Return Nothing
                End If
            End If

            ' *** Walk the . syntax
            Dim Main As String = [Property].Substring(0, lnAt)
            Dim Subs As String = [Property].Substring(lnAt + 1)
            Member = Type.GetMember(Main, wwUtils.MemberAccess)(0)

            Dim [Sub] As Object
            If Member.MemberType = MemberTypes.Property Then
                [Sub] = (CType(Member, PropertyInfo)).GetValue(Parent, Nothing)
            Else
                [Sub] = (CType(Member, FieldInfo)).GetValue(Parent)
            End If

            ' *** Recurse until we get the lowest ref
            SetPropertyEx([Sub], Subs, Value)
            Return Nothing
        End Function

        ' <summary>
        ' Wrapper method to call a 'dynamic' (non-typelib) method
        ' on a COM object
        ' </summary>
        ' <param name="Params"></param>
        ' 1st - Method name, 2nd - 1st parameter, 3rd - 2nd parm etc.
        ' <returns></returns>
        Public Shared Function CallMethod(ByVal [Object] As [Object], ByVal Method As String, ByVal ParamArray Params As [Object]()) As [Object]
            Return [Object].GetType().InvokeMember(Method, wwUtils.MemberAccess Or BindingFlags.InvokeMethod, Nothing, [Object], Params)
            'return Object.GetType().GetMethod(Method,wwUtils.MemberAccess | BindingFlags.InvokeMethod).Invoke(Object,Params);
        End Function

        ' <summary>
        ' Creates an instance from a type by calling the parameterless constructor.
        ' 
        ' Note this will not work with COM objects - continue to use the Activator.CreateInstance
        ' for COM objects.
        ' <seealso>Class wwUtils</seealso>
        ' </summary>
        ' <param name="TypeToCreate">
        ' The type from which to create an instance.
        ' </param>
        ' <returns>object</returns>
        Public Function CreateInstanceFromType(ByVal TypeToCreate As Type) As Object
            Dim Parms As Type() = Type.EmptyTypes
            Return TypeToCreate.GetConstructor(Parms).Invoke(Nothing)
        End Function

        ' <summary>
        ' Converts a type to string if possible. This method supports an optional culture generically on any value.
        ' It calls the ToString() method on common types and uses a type converter on all other objects
        ' if available
        ' </summary>
        ' <param name="RawValue">The Value or Object to convert to a string</param>
        ' <param name="Culture">Culture for numeric and DateTime values</param>
        ' <returns>string</returns>
        Public Shared Function TypedValueToString(ByVal RawValue As Object, ByVal Culture As CultureInfo) As String
            Dim ValueType As Type = RawValue.GetType()
            Dim [Return] As String = Nothing

            If ValueType Is GetType(String) Then
                [Return] = RawValue.ToString()
            ElseIf ValueType Is GetType(Integer) OrElse ValueType Is GetType(Decimal) OrElse ValueType Is GetType(Double) OrElse ValueType Is GetType(Single) Then
                [Return] = String.Format(Culture.NumberFormat, "{0}", RawValue)
            ElseIf ValueType Is GetType(DateTime) Then
                [Return] = String.Format(Culture.DateTimeFormat, "{0}", RawValue)
            ElseIf ValueType Is GetType(Boolean) Then
                [Return] = RawValue.ToString()
            ElseIf ValueType Is GetType(Byte) Then
                [Return] = RawValue.ToString()
            ElseIf ValueType.IsEnum Then
                [Return] = RawValue.ToString()
            Else
                ' Any type that supports a type converter
                Dim converter As System.ComponentModel.TypeConverter = System.ComponentModel.TypeDescriptor.GetConverter(ValueType)
                If Not converter Is Nothing AndAlso converter.CanConvertTo(GetType(String)) Then
                    [Return] = converter.ConvertToString(Nothing, Culture, RawValue)
                Else
                    ' Last resort - just call ToString() on unknown type
                    [Return] = RawValue.ToString()
                End If

            End If

            Return [Return]
        End Function

        ' <summary>
        ' Converts a type to string if possible. This method uses the current culture for numeric and DateTime values.
        ' It calls the ToString() method on common types and uses a type converter on all other objects
        ' if available.
        ' </summary>
        ' <param name="RawValue">The Value or Object to convert to a string</param>
        ' <param name="Culture">Culture for numeric and DateTime values</param>
        ' <returns>string</returns>
        Public Shared Function TypedValueToString(ByVal RawValue As Object) As String
            Return TypedValueToString(RawValue, CultureInfo.CurrentCulture)
        End Function

        ' <summary>
        ' Turns a string into a typed value. Useful for auto-conversion routines
        ' like form variable or XML parsers.
        ' <seealso>Class wwUtils</seealso>
        ' </summary>
        ' <param name="SourceString">
        ' The string to convert from
        ' </param>
        ' <param name="TargetType">
        ' The type to convert to
        ' </param>
        ' <param name="Culture">
        ' Culture used for numeric and datetime values.
        ' </param>
        ' <returns>object. Throws exception if it cannot be converted.</returns>
        Public Shared Function StringToTypedValue(ByVal SourceString As String, ByVal TargetType As Type, ByVal Culture As CultureInfo) As Object
            Dim Result As Object = Nothing

            If TargetType Is GetType(String) Then
                Result = SourceString
            ElseIf TargetType Is GetType(Integer) Then
                Result = Integer.Parse(SourceString, NumberStyles.Integer, Culture.NumberFormat)
            ElseIf TargetType Is GetType(Byte) Then
                Result = Convert.ToByte(SourceString)
            ElseIf TargetType Is GetType(Decimal) Then
                Result = Decimal.Parse(SourceString, NumberStyles.Any, Culture.NumberFormat)
            ElseIf TargetType Is GetType(Double) Then
                Result = Double.Parse(SourceString, NumberStyles.Any, Culture.NumberFormat)
            ElseIf TargetType Is GetType(Boolean) Then
                If SourceString.ToLower() = "true" OrElse SourceString.ToLower() = "on" OrElse SourceString = "1" Then
                    Result = True
                Else
                    Result = False
                End If
            ElseIf TargetType Is GetType(DateTime) Then
                Result = Convert.ToDateTime(SourceString, Culture.DateTimeFormat)
            ElseIf TargetType.IsEnum Then
                Result = System.Enum.Parse(TargetType, SourceString)
            Else
                Dim converter As System.ComponentModel.TypeConverter = System.ComponentModel.TypeDescriptor.GetConverter(TargetType)
                If Not converter Is Nothing AndAlso converter.CanConvertFrom(GetType(String)) Then
                    Result = converter.ConvertFromString(Nothing, Culture, SourceString)
                Else
                    System.Diagnostics.Debug.Assert(False, "Type Conversion not handled in StringToTypedValue for " & TargetType.Name & " " & SourceString)
                    Throw (New ApplicationException("Type Conversion not handled in StringToTypedValue"))
                End If
            End If

            Return Result
        End Function

        ' <summary>
        ' Turns a string into a typed value. Useful for auto-conversion routines
        ' like form variable or XML parsers.
        ' </summary>
        ' <param name="SourceString">The input string to convert</param>
        ' <param name="TargetType">The Type to convert it to</param>
        ' <returns>object reference. Throws Exception if type can not be converted</returns>
        Public Shared Function StringToTypedValue(ByVal SourceString As String, ByVal TargetType As Type) As Object
            Return StringToTypedValue(SourceString, TargetType, CultureInfo.CurrentCulture)
        End Function

#End Region

#Region "COM Reflection Helper Code"

        ' <summary>
        ' Retrieve a dynamic 'non-typelib' property
        ' </summary>
        ' <param name="Object">Object to make the call on</param>
        ' <param name="Property">Property to retrieve</param>
        ' <returns></returns>
        Public Shared Function GetPropertyCom(ByVal [Object] As [Object], ByVal [Property] As String) As [Object]
            Return [Object].GetType().InvokeMember([Property], wwUtils.MemberAccess Or BindingFlags.GetProperty Or BindingFlags.GetField, Nothing, [Object], Nothing)
        End Function


        ' <summary>
        ' Returns a property or field value using a base object and sub members including . syntax.
        ' For example, you can access: this.oCustomer.oData.Company with (this,"oCustomer.oData.Company")
        ' </summary>
        ' <param name="Parent">Parent object to 'start' parsing from.</param>
        ' <param name="Property">The property to retrieve. Example: 'oBus.oData.Company'</param>
        ' <returns></returns>
        Public Shared Function GetPropertyExCom(ByVal Parent As Object, ByVal [Property] As String) As Object

            Dim Type As Type = Parent.GetType()

            Dim lnAt As Integer = [Property].IndexOf(".")
            If lnAt < 0 Then
                If [Property] = "this" OrElse [Property] = "me" Then
                    Return Parent
                End If

                ' *** Get the member
                Return Parent.GetType().InvokeMember([Property], wwUtils.MemberAccess Or BindingFlags.GetProperty Or BindingFlags.GetField, Nothing, Parent, Nothing)
            End If

            ' *** Walk the . syntax - split into current object (Main) and further parsed objects (Subs)
            Dim Main As String = [Property].Substring(0, lnAt)
            Dim Subs As String = [Property].Substring(lnAt + 1)

            Dim [Sub] As Object = Parent.GetType().InvokeMember(Main, wwUtils.MemberAccess Or BindingFlags.GetProperty Or BindingFlags.GetField, Nothing, Parent, Nothing)

            ' *** Recurse further into the sub-properties (Subs)
            Return wwUtils.GetPropertyExCom([Sub], Subs)
        End Function

        ' <summary>
        ' Sets the property on an object.
        ' </summary>
        ' <param name="Object">Object to set property on</param>
        ' <param name="Property">Name of the property to set</param>
        ' <param name="Value">value to set it to</param>
        Public Shared Sub SetPropertyCom(ByVal [Object] As [Object], ByVal [Property] As String, ByVal Value As [Object])
            [Object].GetType().InvokeMember([Property], wwUtils.MemberAccess Or BindingFlags.SetProperty Or BindingFlags.SetField, Nothing, [Object], New Object(0) {Value})
            'GetProperty(Property,wwUtils.MemberAccess).SetValue(Object,Value,null);
        End Sub

        ' <summary>
        ' Sets the value of a field or property via Reflection. This method alws 
        ' for using '.' syntax to specify objects multiple levels down.
        ' 
        ' wwUtils.SetPropertyEx(this,"Invoice.LineItemsCount",10)
        ' 
        ' which would be equivalent of:
        ' 
        ' this.Invoice.LineItemsCount = 10;
        ' </summary>
        ' <param name="Object Parent">
        ' Object to set the property on.
        ' </param>
        ' <param name="String Property">
        ' Property to set. Can be an object hierarchy with . syntax.
        ' </param>
        ' <param name="Object Value">
        ' Value to set the property to
        ' </param>
        Public Shared Function SetPropertyExCom(ByVal Parent As Object, ByVal [Property] As String, ByVal Value As Object) As Object
            Dim Type As Type = Parent.GetType()

            Dim lnAt As Integer = [Property].IndexOf(".")
            If lnAt < 0 Then
                ' *** Set the member
                Parent.GetType().InvokeMember([Property], wwUtils.MemberAccess Or BindingFlags.SetProperty Or BindingFlags.SetField, Nothing, Parent, New Object(0) {Value})

                Return Nothing
            End If

            ' *** Walk the . syntax - split into current object (Main) and further parsed objects (Subs)
            Dim Main As String = [Property].Substring(0, lnAt)
            Dim Subs As String = [Property].Substring(lnAt + 1)


            Dim [Sub] As Object = Parent.GetType().InvokeMember(Main, wwUtils.MemberAccess Or BindingFlags.GetProperty Or BindingFlags.GetField, Nothing, Parent, Nothing)

            Return SetPropertyExCom([Sub], Subs, Value)
        End Function


        ' <summary>
        ' Wrapper method to call a 'dynamic' (non-typelib) method
        ' on a COM object
        ' </summary>
        ' <param name="Params"></param>
        ' 1st - Method name, 2nd - 1st parameter, 3rd - 2nd parm etc.
        ' <returns></returns>
        Public Shared Function CallMethodCom(ByVal [Object] As [Object], ByVal Method As String, ByVal ParamArray Params As [Object]()) As [Object]
            Return [Object].GetType().InvokeMember(Method, wwUtils.MemberAccess Or BindingFlags.InvokeMethod, Nothing, [Object], Params)
        End Function

#End Region

#Region "Object Serialization routines"
        ' <summary>
        ' Returns a string of all the field value pairs of a given object.
        ' Works only on non-statics.
        ' </summary>
        ' <param name="Obj"></param>
        ' <param name="Separator"></param>
        ' <returns></returns>
        Public Shared Function ObjectToString(ByVal Obj As Object, ByVal Separator As String, ByVal Type As ObjectToStringTypes) As String
            Dim fi As FieldInfo() = Obj.GetType().GetFields()

            Dim lcOutput As String = ""

            If Type = ObjectToStringTypes.Properties OrElse Type = ObjectToStringTypes.PropertiesAndFields Then
                For Each [Property] As PropertyInfo In Obj.GetType().GetProperties()
                    Try
                        lcOutput = lcOutput & [Property].Name & ":" & [Property].GetValue(Obj, Nothing).ToString() & Separator
                    Catch
                        lcOutput = lcOutput & [Property].Name & ": n/a" & Separator
                    End Try
                Next [Property]
            End If

            If Type = ObjectToStringTypes.Fields OrElse Type = ObjectToStringTypes.PropertiesAndFields Then
                For Each Field As FieldInfo In fi
                    Try
                        lcOutput = lcOutput & Field.Name & ": " & Field.GetValue(Obj).ToString() & Separator
                    Catch
                        lcOutput = lcOutput & Field.Name & ": n/a" & Separator
                    End Try
                Next Field
            End If
            Return lcOutput
        End Function

        Public Enum ObjectToStringTypes
            Properties
            PropertiesAndFields
            Fields
        End Enum

        ' <summary>
        ' Serializes an object instance to a file.
        ' </summary>
        ' <param name="Instance">the object instance to serialize</param>
        ' <param name="Filename"></param>
        ' <param name="BinarySerialization">determines whether XML serialization or binary serialization is used</param>
        ' <returns></returns>
        Public Shared Function SerializeObject(ByVal Instance As Object, ByVal Filename As String, ByVal BinarySerialization As Boolean) As Boolean
            Dim retVal As Boolean = True

            If (Not BinarySerialization) Then
                Dim writer As XmlTextWriter = Nothing
                Try
                    Dim serializer As XmlSerializer = New XmlSerializer(Instance.GetType())

                    ' Create an XmlTextWriter using a FileStream.
                    Dim fs As Stream = New FileStream(Filename, FileMode.Create)
                    writer = New XmlTextWriter(fs, New UTF8Encoding())
                    writer.Formatting = Formatting.Indented
                    writer.IndentChar = " "c
                    writer.Indentation = 3

                    ' Serialize using the XmlTextWriter.
                    serializer.Serialize(writer, Instance)
                Catch e1 As Exception
                    retVal = False
                Finally
                    If Not writer Is Nothing Then
                        writer.Close()
                    End If
                End Try
            Else
                Dim fs As Stream = Nothing
                Try
                    Dim serializer As BinaryFormatter = New BinaryFormatter()
                    fs = New FileStream(Filename, FileMode.Create)
                    serializer.Serialize(fs, Instance)
                Catch
                    retVal = False
                Finally
                    If Not fs Is Nothing Then
                        fs.Close()
                    End If
                End Try
            End If

            Return retVal
        End Function

        ' <summary>
        ' Overload that supports passing in an XML TextWriter. Note the Writer is not closed
        ' </summary>
        ' <param name="Instance"></param>
        ' <param name="writer"></param>
        ' <param name="BinarySerialization"></param>
        ' <returns></returns>
        Public Shared Function SerializeObject(ByVal Instance As Object, ByVal writer As XmlTextWriter) As Boolean
            Dim retVal As Boolean = True

            Try
                Dim serializer As XmlSerializer = New XmlSerializer(Instance.GetType())

                ' Create an XmlTextWriter using a FileStream.
                writer.Formatting = Formatting.Indented
                writer.IndentChar = " "c
                writer.Indentation = 3

                ' Serialize using the XmlTextWriter.
                serializer.Serialize(writer, Instance)
            Catch ex As Exception
                Dim Message As String = ex.Message
                retVal = False
            End Try

            Return retVal
        End Function

        ' <summary>
        ' Serializes an object into a string variable for easy 'manual' serialization
        ' </summary>
        ' <param name="Instance"></param>
        ' <returns></returns>
        Public Shared Function SerializeObject(ByVal Instance As Object, <System.Runtime.InteropServices.Out()> ByRef XmlResultString As String) As Boolean
            XmlResultString = ""
            Dim ms As MemoryStream = New MemoryStream()

            Dim writer As XmlTextWriter = New XmlTextWriter(ms, New UTF8Encoding())

            If (Not SerializeObject(Instance, writer)) Then
                ms.Close()
                Return False
            End If

            Dim Result As Byte() = New Byte(ms.Length - 1) {}
            ms.Position = 0
            ms.Read(Result, 0, CInt(Fix(ms.Length)))

            XmlResultString = Encoding.UTF8.GetString(Result, 0, CInt(Fix(ms.Length)))

            ms.Close()
            writer.Close()

            Return True
        End Function


        ' <summary>
        ' Serializes an object instance to a file.
        ' </summary>
        ' <param name="Instance">the object instance to serialize</param>
        ' <param name="Filename"></param>
        ' <param name="BinarySerialization">determines whether XML serialization or binary serialization is used</param>
        ' <returns></returns>
        Public Shared Function SerializeObject(ByVal Instance As Object, <System.Runtime.InteropServices.Out()> ByRef ResultBuffer As Byte()) As Boolean
            Dim retVal As Boolean = True

            Dim ms As MemoryStream = Nothing
            Try
                Dim serializer As BinaryFormatter = New BinaryFormatter()
                ms = New MemoryStream()
                serializer.Serialize(ms, Instance)
            Catch
                retVal = False
            Finally
                If Not ms Is Nothing Then
                    ms.Close()
                End If
            End Try

            ResultBuffer = ms.ToArray()

            Return retVal
        End Function

        ' <summary>
        ' Deserializes an object from file and returns a reference.
        ' </summary>
        ' <param name="Filename">name of the file to serialize to</param>
        ' <param name="ObjectType">The Type of the object. Use typeof(yourobject class)</param>
        ' <param name="BinarySerialization">determines whether we use Xml or Binary serialization</param>
        ' <returns>Instance of the deserialized object or null. Must be cast to your object type</returns>
        Public Shared Function DeSerializeObject(ByVal Filename As String, ByVal ObjectType As Type, ByVal BinarySerialization As Boolean) As Object
            Dim Instance As Object = Nothing

            If (Not BinarySerialization) Then

                Dim reader As XmlReader = Nothing
                Dim serializer As XmlSerializer = Nothing
                Dim fs As FileStream = Nothing
                Try
                    ' Create an instance of the XmlSerializer specifying type and namespace.
                    serializer = New XmlSerializer(ObjectType)

                    ' A FileStream is needed to read the XML document.
                    fs = New FileStream(Filename, FileMode.Open)
                    reader = New XmlTextReader(fs)

                    Instance = serializer.Deserialize(reader)

                Catch
                    Return Nothing
                Finally
                    If Not fs Is Nothing Then
                        fs.Close()
                    End If

                    If Not reader Is Nothing Then
                        reader.Close()
                    End If
                End Try
            Else

                Dim serializer As BinaryFormatter = Nothing
                Dim fs As FileStream = Nothing

                Try
                    serializer = New BinaryFormatter()
                    fs = New FileStream(Filename, FileMode.Open)
                    Instance = serializer.Deserialize(fs)

                Catch
                    Return Nothing
                Finally
                    If Not fs Is Nothing Then
                        fs.Close()
                    End If
                End Try
            End If

            Return Instance
        End Function

        ' <summary>
        ' Deserialize an object from an XmlReader object.
        ' </summary>
        ' <param name="reader"></param>
        ' <param name="ObjectType"></param>
        ' <returns></returns>
        Public Shared Function DeSerializeObject(ByVal reader As System.Xml.XmlReader, ByVal ObjectType As Type) As Object
            Dim serializer As XmlSerializer = New XmlSerializer(ObjectType)
            Dim Instance As Object = serializer.Deserialize(reader)
            reader.Close()

            Return Instance
        End Function

        Public Shared Function DeSerializeObject(ByVal XML As String, ByVal ObjectType As Type) As Object
            Dim reader As XmlTextReader = New XmlTextReader(XML, XmlNodeType.Document, Nothing)
            Return DeSerializeObject(reader, ObjectType)
        End Function

        Public Shared Function DeSerializeObject(ByVal Buffer As Byte(), ByVal ObjectType As Type) As Object
            Dim serializer As BinaryFormatter = Nothing
            Dim ms As MemoryStream = Nothing
            Dim Instance As Object = Nothing

            Try
                serializer = New BinaryFormatter()
                ms = New MemoryStream(Buffer)
                Instance = serializer.Deserialize(ms)

            Catch
                Return Nothing
            Finally
                If Not ms Is Nothing Then
                    ms.Close()
                End If
            End Try

            Return Instance
        End Function
#End Region

#Region "Miscellaneous Routines "


        ' <summary>
        ' Returns the logon password stored in the registry if Auto-Logon is used.
        ' This function is used privately for demos when I need to specify a login username and password.
        ' </summary>
        ' <param name="GetUserName"></param>
        ' <returns></returns>
        Public Shared Function GetSystemPassword(ByVal GetUserName As Boolean) As String
            Dim RegKey As RegistryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon")
            If RegKey Is Nothing Then
                Return ""
            End If

            Dim Password As String
            If (Not GetUserName) Then
                Password = CStr(RegKey.GetValue("DefaultPassword"))
            Else
                Password = CStr(RegKey.GetValue("DefaultUsername"))
            End If

            If Password Is Nothing Then
                Return ""
            End If

            Return CStr(Password)
        End Function

        ' <summary>
        ' Converts the passed date time value to Mime formatted time string
        ' </summary>
        ' <param name="Time"></param>
        Public Shared Function MimeDateTime(ByVal Time As DateTime) As String
            Dim Offset As TimeSpan = TimeZone.CurrentTimeZone.GetUtcOffset(Time)

            Dim sOffset As String = Offset.Hours.ToString().PadLeft(2, "0"c)
            If Offset.Hours < 0 Then
                sOffset = "-" & (Offset.Hours * -1).ToString().PadLeft(2, "0"c)
            End If

            sOffset &= Offset.Minutes.ToString().PadLeft(2, "0"c)

            Return "Date: " & DateTime.Now.ToString("ddd, dd MMM yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture) & " " & sOffset
        End Function

        ' <summary>
        ' Single method to retrieve HTTP content from the Web quickly
        ' </summary>
        ' <param name="Url"></param>
        ' <param name="ErrorMessage"></param>
        ' <returns></returns>
        Public Shared Function HttpGet(ByVal Url As String, ByRef ErrorMessage As String) As String
            Dim MergedText As String = ""

            Dim Http As System.Net.WebClient = New System.Net.WebClient()

            ' Download the Web resource and save it into a data buffer.
            Try
                Dim Result As Byte() = Http.DownloadData(Url)
                MergedText = Encoding.Default.GetString(Result)
            Catch ex As Exception
                ErrorMessage = ex.Message
                Return Nothing
            End Try

            Return MergedText
        End Function


#End Region

#Region "Path Functions"
        ' <summary>
        ' Returns the full path of a full physical filename
        ' </summary>
        ' <param name="Path"></param>
        ' <returns></returns>
        Public Shared Function JustPath(ByVal Path As String) As String
            Dim fi As FileInfo = New FileInfo(Path)
            Return fi.DirectoryName & "\"
        End Function

        ' <summary>
        ' Returns a relative path string from a full path.
        ' </summary>
        ' <param name="FullPath">The path to convert. Can be either a file or a directory</param>
        ' <param name="BasePath">The base path to truncate to and replace</param>
        ' <returns>
        ' Lower case string of the relative path. If path is a directory it's returned without a backslash at the end.
        ' 
        ' Examples of returned values:
        '  .\test.txt, ..\test.txt, ..\..\..\test.txt, ., ..
        ' </returns>
        Public Shared Function GetRelativePath(ByVal FullPath As String, ByVal BasePath As String) As String
            ' *** Start by normalizing paths
            FullPath = FullPath.ToLower()
            BasePath = BasePath.ToLower()

            If BasePath.EndsWith("\") Then
                BasePath = BasePath.Substring(0, BasePath.Length - 1)
            End If
            If FullPath.EndsWith("\") Then
                FullPath = FullPath.Substring(0, FullPath.Length - 1)
            End If

            ' *** First check for full path
            If (FullPath & "\").IndexOf(BasePath & "\") > -1 Then
                Return FullPath.Replace(BasePath, ".")
            End If

            ' *** Now parse backwards
            Dim BackDirs As String = ""
            Dim PartialPath As String = BasePath
            Dim Index As Integer = PartialPath.LastIndexOf("\")
            Do While Index > 0
                ' *** Strip path step string to last backslash
                PartialPath = PartialPath.Substring(0, Index)

                ' *** Add another step backwards to our pass replacement
                BackDirs = BackDirs & "..\"

                ' *** Check for a matching path
                If FullPath.IndexOf(PartialPath) > -1 Then
                    If FullPath = PartialPath Then
                        ' *** We're dealing with a full Directory match and need to replace it all
                        Return FullPath.Replace(PartialPath, BackDirs.Substring(0, BackDirs.Length - 1))
                    Else
                        ' *** We're dealing with a file or a start path
                        If FullPath = PartialPath Then
                            Return FullPath.Replace(PartialPath & (""), BackDirs)
                        Else
                            Return FullPath.Replace(PartialPath & ("\"), BackDirs)
                        End If
                    End If
                End If
                Index = PartialPath.LastIndexOf("\", PartialPath.Length - 1)
            Loop

            Return FullPath
        End Function
#End Region

#Region "Shell Functions for displaying URL, HTML, Text and XML"
        <DllImport("Shell32.dll")> _
        Private Shared Function ShellExecute(ByVal hwnd As Integer, ByVal lpOperation As String, ByVal lpFile As String, ByVal lpParameters As String, ByVal lpDirectory As String, ByVal nShowCmd As Integer) As Integer
        End Function

        ' <summary>
        ' Uses the Shell Extensions to launch a program based or URL moniker.
        ' </summary>
        ' <param name="lcUrl">Any URL Moniker that the Windows Shell understands (URL, Word Docs, PDF, Email links etc.)</param>
        ' <returns></returns>
        Public Shared Function GoUrl(ByVal Url As String) As Integer
            Dim TPath As String = Path.GetTempPath()

            Dim Result As Integer = ShellExecute(0, "OPEN", Url, "", TPath, 1)
            Return Result
        End Function

        ' <summary>
        ' Displays an HTML string in a browser window
        ' </summary>
        ' <param name="HtmlString"></param>
        ' <returns></returns>
        Public Shared Function ShowString(ByVal HtmlString As String, ByVal extension As String) As Integer
            If extension Is Nothing Then
                extension = "htm"
            End If

            Dim File As String = Path.GetTempPath() & "\__preview." & extension
            Dim sw As StreamWriter = New StreamWriter(File, False, Encoding.Default)
            sw.Write(HtmlString)
            sw.Close()

            Return GoUrl(File)
        End Function

        Public Shared Function ShowHtml(ByVal HtmlString As String) As Integer
            Return ShowString(HtmlString, Nothing)
        End Function

        ' <summary>
        ' Displays a large Text string as a text file.
        ' </summary>
        ' <param name="TextString"></param>
        ' <returns></returns>
        Public Shared Function ShowText(ByVal TextString As String) As Integer
            Dim File As String = Path.GetTempPath() & "\__preview.txt"

            Dim sw As StreamWriter = New StreamWriter(File, False)
            sw.Write(TextString)
            sw.Close()

            Return GoUrl(File)
        End Function
#End Region


#If False Then
		' <summary>
		' Parses the text of a Soap Exception and returns just the error message text
		' Ideally you'll want to have a SoapException fire on the server, otherwise
		' this method will try to parse out the inner exception error message.
		' </summary>
		' <param name="SoapExceptionText"></param>
		' <returns></returns>
		Public Shared Function ParseSoapExceptionText(ByVal SoapExceptionText As String) As String
			Dim Message As String = wwUtils.ExtractString(SoapExceptionText,"SoapException: ",Constants.vbLf)
			If Message <> "" Then
				Return Message
			End If

			Message = wwUtils.ExtractString(SoapExceptionText,"SoapException: "," --->")
			If Message = "Server was unable to process request." Then
				Message = wwUtils.ExtractString(SoapExceptionText,"-->",Constants.vbLf)
				Message = Message.Substring(Message.IndexOf(":")+1)
			End If

			If Message = "" Then
				Return "An error occurred on the server."
			End If

			Return Message
		End Function
#End If
    End Class

End Namespace



