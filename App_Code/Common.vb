Imports System.Configuration
Imports System.Web
Imports System.Text
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Convert
Imports Microsoft.Practices.EnterpriseLibrary.Data
Imports System.Data.Common
Imports System.Data
Imports System.Data.SqlClient
Imports System.Net

' stores shared settings (like the database connection string)
Public Class Common

    Public Shared factory As DatabaseProviderFactory = New DatabaseProviderFactory()
    Public Shared db As Database = factory.CreateDefault()

#Region " Misc Helper Functions "
    'Parse Phone
    Public Shared Function parse_phone(ByVal sString As Object) As String
        Dim cHP1, cHP2, cHP3, gHP1 As String

        If String.IsNullOrEmpty(sString) Then
            Return ""
        Else
            'Check Formatting Type
            If InStr(sString, "-") >= 1 Then
                Return sString
            Else
                'Phone Number (Old Format)
                gHP1 = Trim(sString)
                cHP1 = Left(gHP1, 3)
                cHP2 = Mid(gHP1, 4, 3)
                cHP3 = Right(gHP1, 4)
                Return "(" + cHP1 + ") " + cHP2 + "-" + cHP3
            End If
        End If
    End Function

    Public Shared Function CatchInnerException(ByVal iException As Exception) As String
        Dim iErrStr As String = ""
        If Not iException Is Nothing Then
            iErrStr = "Inner Exception: " & iException.Message.ToString & " Source: " & iException.Source.ToString & " Stack: " & iException.StackTrace.ToString
        End If
        Return iErrStr
    End Function

    Public Shared Function MyNumber(ByVal numval As Object) As String
        If IsDBNull(numval) Then
            Return FormatCurrency(0, 2).ToString
        Else
            Return FormatCurrency(numval, 2).ToString
        End If
    End Function

    Public Shared Function MyNumberNoComma(ByVal numval As Object) As String
        If IsDBNull(numval) Then
            Return FormatNumber(0, 2).ToString
        Else
            Return FormatNumber(numval, 2).ToString.Replace(",", "")
        End If
    End Function

    Public Shared Function MyNumberNoCurr(ByVal numval As Object) As String
        If IsDBNull(numval) Then
            Return FormatNumber(0, 2).ToString
        Else
            Return FormatNumber(numval, 2).ToString
        End If
    End Function

    Public Shared Function MyNumberComma(ByVal numval As Object) As String
        If IsDBNull(numval) Or CStr(numval) = "" Then
            Return FormatNumber(0, 0).ToString
        Else
            Return FormatNumber(numval, 0).ToString
        End If
    End Function

    Public Shared Function FormatFileSize(ByVal value As Integer) As String
        If value < 1024 Then
            Return value.ToString & " b"
        ElseIf value < 1048576 Then
            Return Math.Round(value / 1024) & " kb"
        Else
            Return Math.Round(value / 10485.76) / 100 & " mb"
        End If
    End Function

    Public Shared Function ShowDateDiff(ByVal date1 As Object) As String
        Dim retstr As String = ""

        If IsDBNull(date1) Or date1 = "" Then
            retstr = " <span class='blue'><b>[No Dates]</b></span>"
        Else
            retstr = " <span class='blue'><b>[" & DateDiff(DateInterval.Day, Date.Today(), date1) & " days]</b></span>"
        End If

        Return retstr
    End Function

    Public Shared Function PadDate(ByVal date1 As Object) As String
        Dim retstr As String = ""

        If Not IsDate(date1) Then
            retstr = ""
        Else
            'Get Month
            Dim mVal As String = ""
            If Month(date1) <= 9 Then
                mVal = "0" & Month(date1).ToString
            Else
                mVal = Month(date1).ToString
            End If

            'Get Day
            Dim dVal As String = ""
            If Day(date1) <= 9 Then
                dVal = "0" & Day(date1).ToString
            Else
                dVal = Day(date1).ToString
            End If

            retstr = mVal & "/" & dVal & "/" & Year(date1).ToString
        End If

        Return retstr
    End Function

    'Get Last Day of Month
    Public Shared Function GetCurrentMonthEnd(ByVal Mnth As Integer, ByVal Year As Integer) As String

        Return String.Format("{0}/{1}/{2}", Mnth.ToString(), DateTime.DaysInMonth(Year, Mnth).ToString(), Year.ToString())
    End Function

    'Get Time Diff in H:M
    Public Shared Function GetCurrentTimeDiff(ByVal Time1 As String) As String
        Dim RetStr As String = "0:0"
        Dim minutes As Integer = 0
        Dim word As String = ""

        If Time1 <> "" Then
            minutes = DateDiff(DateInterval.Minute, CDate(Time1), DateTime.Now)
            If minutes <= 0 Then
                word = "0 Min(s)"
            Else
                word = ""
                If minutes >= 24 * 60 Then
                    word = word & minutes \ (24 * 60) & " Day(s) "
                    Return word
                End If

                minutes = minutes Mod (24 * 60)
                If minutes >= 60 Then
                    word = word & minutes \ (60) & " Hour(s), "
                    Return word
                End If

                minutes = minutes Mod 60
                word = word & minutes & " Min(s)"
            End If
        End If

        Return word
    End Function

    Public Shared Function GetCurrentTimeDiffDueDate(ByVal Time1 As String) As String
        Dim RetStr As String = "0:0"
        Dim minutes As Integer = 0
        Dim word As String = ""

        If Time1 <> "" Then
            minutes = DateDiff(DateInterval.Minute, DateTime.Now, CDate(Time1))
            If minutes <= 0 Then
                word = "<span class='error'>Overdue - "
                minutes = DateDiff(DateInterval.Minute, CDate(Time1), DateTime.Now)
            Else
                word = "<span>"
            End If

            If minutes >= 24 * 60 Then
                word = word & minutes \ (24 * 60) & " Day(s) "
                Return word & "</span>"
            End If

            minutes = minutes Mod (24 * 60)
            If minutes >= 60 Then
                word = word & minutes \ (60) & " Hour(s), "
                Return word & "</span>"
            End If

            minutes = minutes Mod 60
            word = word & minutes & " Min(s)</span>"
        End If

        Return word & "</span>"
    End Function

    'OrderStatus Formatting
    Public Shared Function EventStatusFormat(ByVal orderStatus As String, Optional ByVal IsTicket As Boolean = True) As String
        Dim status As String = ""

        If IsTicket Then
            Select Case orderStatus
                Case "Active"
                    status = "<small class=""tag"">Active</small>"
                Case "Cancel", "NoShow"
                    status = "<small class=""tag red-bg"">" & orderStatus & "</small>"
                Case "Pending"
                    status = "<small class=""tag grey-bg"">" & orderStatus & "</small>"
                Case "Confirmed"
                    status = "<small class=""tag green-bg"">" & orderStatus & "</small>"
                Case "InActive"
                    status = "<small class=""tag orange-bg"">" & orderStatus & "</small>"
                Case Else
                    status = "<small class=""tag anthracite-bg"">" & orderStatus & "</small>"
            End Select
        Else
            status = "<span class=""tag anthracite-bg"">" & orderStatus & "</span>"
        End If

        Return status
    End Function

    'Return Random Number
    Public Shared Function GetRandomNumber(Optional ByVal Max As Integer = 100000) As Integer
        Dim rnd As New Random
        Return rnd.Next(1000, Max)
    End Function

    'Is valid date?
    Public Shared Function IsDate(ByVal sDate As String) As Boolean

        Dim rEx As Regex = New Regex("^\d{1,2}\/\d{1,2}\/\d{2,4}$")

        ' Create regular expression:
        If rEx.IsMatch(sDate) Then
            Return True
        Else
            Return False
        End If
    End Function

    '----------------------------------------------------------------------
    '	Function: LogDate
    '   Converts date to log file date
    '----------------------------------------------------------------------
    Public Shared Function LogDate(ByVal stringToCheck As String, ByVal defaultOut As String) As String
        Dim dayPart As Integer
        Dim monthPart As String
        Dim yearPart As Integer

        If IsDate(stringToCheck) Then
            dayPart = Day(stringToCheck)
            monthPart = MonthName(Month(stringToCheck), True)
            yearPart = Year(stringToCheck)
            Return monthPart & "-" & dayPart & "-" & yearPart
        Else
            Return defaultOut
        End If
    End Function

    'Is valid number?
    Public Shared Function IsNumber(ByVal sNum As String) As Boolean
        Dim n As Integer
        IsNumber = True
        Try
            n = Int32.Parse(sNum)
        Catch
            IsNumber = False
        End Try
        Return IsNumber
    End Function

    Public Shared Function MailDeCode(ByVal mText As String) As String
        Dim rText As String = ""
        Try
            rText = mText.Replace("{", "<")
            rText = rText.Replace("}", ">")
        Catch ex As Exception
            Return mText
        End Try

        Return rText
    End Function

    Public Shared Function XMLUrlEncode(ByVal myText As String) As String
        Return HttpUtility.UrlEncode(myText)
    End Function

    Public Shared Function EncodeXMLText(ByVal gText As String) As String
        Dim rText As String = ""
        Try
            rText = gText.Replace("'", "&apos;")
            rText = rText.Replace("""", "&quot;")
            rText = rText.Replace("&", "&amp;")
            rText = rText.Replace("<", "&lt;")
            rText = rText.Replace(">", "&gt;")

        Catch ex As Exception
            Return gText
        End Try

        Return rText
    End Function

    Public Shared Function JSEncodeApos(ByVal eText As String) As String
        Dim rText As String = ""
        Try
            rText = eText.Replace("'", "")
        Catch
            rText = ""
        End Try
        Return rText
    End Function

    Public Shared Function SQLEncode(ByVal eText As String) As String
        Dim rText As String = ""
        Try
            rText = eText.Replace("'", "''")
            rText = rText.Replace(">", "")
            rText = rText.Replace("<", "")
            rText = rText.Replace(";", "")
        Catch
            rText = ""
        End Try
        Return rText
    End Function

    'Trim String Length
    Public Shared Function TrimString(ByVal str As String, ByVal len As Integer) As String
        If str <> "" And IsDBNull(str) = False Then
            If str.Length > len Then
                Return Left(str, len) & "..."
            Else
                Return str
            End If
        Else
            Return ""
        End If
    End Function

    Public Shared Function gMapEncode(ByVal strText2 As String) As String
        Dim repText As String = ""
        If Not String.IsNullOrEmpty(strText2) Then
            repText = Replace(strText2, "'", "\'")
            repText = Replace(repText, """", "\""")
            repText = Replace(repText, ",", " ")
            repText = Replace(repText, vbCrLf, "\n")
        End If
        Return repText
    End Function

    Public Shared Function StripHTML(ByVal html As String, Optional ByVal ReplaceBreak As Object = "\n") As String
        Dim text As String = html
        Dim i As Integer = 0
        Dim options As RegexOptions = RegexOptions.IgnoreCase Or RegexOptions.Multiline

        If ReplaceBreak = "vbCrLf" Then
            ReplaceBreak = vbCrLf
        End If

        ' Remove line breaks
        Dim lb1_regex As String = "(?:\n|\r\n|\r)"
        Dim r1 As Regex = New Regex(lb1_regex, options)
        text = r1.Replace(text, " ")

        ' Remove content in style tags.
        Dim style_regex As String = "<\s*style[^>]*>[\s\S]*?<\/style>"
        Dim r2 As Regex = New Regex(style_regex, options)
        text = r2.Replace(text, "")

        'Define HTML line breaking tags
        Dim doubleNewlineTags() As String = {"p", "h[1-6]", "dl", "dt", "dd", "ol", "ul", "dir", "address", "blockquote", "center", "div", "hr", "pre", "form", "textarea", "table"}
        Dim singleNewlineTags() As String = {"li", "del", "ins", "fieldset", "legend", "tr", "th", "caption", "thead", "tbody", "tfoot"}

        For i = 0 To doubleNewlineTags.Length - 1
            Dim DoubleLineBreak_regex As String = "</?\s*" & doubleNewlineTags(i) & "[^>]*>"
            Dim r3 As Regex = New Regex(DoubleLineBreak_regex, options)
            text = r3.Replace(text, ReplaceBreak)
        Next i

        For i = 0 To singleNewlineTags.Length - 1
            Dim SingleLineBreak_regex As String = "<\s*" & singleNewlineTags(i) & "[^>]*>"
            Dim r4 As New Regex(SingleLineBreak_regex, options)
            text = r4.Replace(text, ReplaceBreak)
        Next i

        'Replace <br> and <br/> with a single newline
        Dim break_regex As String = "<\s*br[^>]*\/?\s*>"
        Dim r5 As Regex = New Regex(break_regex, options)
        text = r5.Replace(text, ReplaceBreak)

        ' Remove all remaining tags.
        Dim AllTags_regex As String = "(<([^>]+)>)"
        Dim r6 As Regex = New Regex(AllTags_regex, options)
        text = r6.Replace(text, ReplaceBreak)

        ' Trim rightmost whitespaces for all lines
        Dim ws_regex As String = "\s+"
        Dim r7 As Regex = New Regex(ws_regex, options)
        text = r7.Replace(text, " ")

        ' Make sure there are never more than two
        ' consecutive linebreaks.
        Dim lb2_regex As String = "\n{2,}"
        Dim r8 As Regex = New Regex(lb2_regex, options)
        text = r8.Replace(text, "")

        ' Remove newlines at the beginning of the text.
        Dim nl1_regex As String = "^\n+/"
        Dim r9 As Regex = New Regex(nl1_regex, options)
        text = r9.Replace(text, "")

        ' Remove newlines at the end of the text.
        'Dim nl2_regex As String = "^\n+/"
        'Dim r10 As Regex = New Regex(nl2_regex, options)
        'text = r10.Replace(text, "")

        text = text.Replace("\n", vbCrLf)
        Return text
    End Function

    'Checkbox Is Checked?
    Public Shared Function IsChecked(ByVal ob As Object) As String
        If ob Then
            Return "checked"
        Else
            Return ""
        End If
    End Function


    'Replace Date Value in note
    '#sdate# = schedule date, #hdate# = hold date
    Public Shared Function DateReplace(ByVal strText As String, ByVal hDate As String, ByVal sDate As String) As String

        If strText <> "" And IsDBNull(strText) = False Then
            strText = Trim(strText)
            strText = Replace(strText, "#hdate#", hDate)
            strText = Replace(strText, "#sdate#", sDate)
        End If
        Return strText
    End Function

    'Loop Through Email String and Validate each address
    Public Shared Function EmailSendTest(ByVal email_str As String) As String
        Dim str_parse() As String
        Dim TestStr As String = email_str.Replace(",", ";")
        Dim email_new As New StringBuilder
        Dim NewStr As String = ""
        Dim FinalStr As String = ""
        Dim MatchTable As New ArrayList
        Dim i As Integer = 0

        If Not String.IsNullOrEmpty(email_str) Then
            str_parse = TestStr.Split(";")

            For i = 0 To UBound(str_parse)
                NewStr = Trim(str_parse(i))
                If ValidEmailSingle(NewStr) Then
                    If Not MatchTable.Contains(NewStr) Then
                        MatchTable.Add(NewStr)
                        email_new.Append(NewStr & "; ")
                    End If
                End If
            Next
        Else
            Return ""
        End If

        FinalStr = StrRemoveLast(email_new.ToString())

        While Right(FinalStr, 1) = ";"
            FinalStr = StrRemoveLast(FinalStr)
        End While

        Return FinalStr
    End Function

    '----------------------------------------------------------------------
    '	Function: ValidEmail
    '   This function returns true if the email passed is valid.
    '----------------------------------------------------------------------
    Public Shared Function ValidEmail(ByVal email_str As String) As Boolean
        Dim rEx As Regex = New Regex("^[\w-\.]{1,}\@([\da-zA-Z-]{1,}\.){1,}[\da-zA-Z-]{2,4}$")
        Dim str_parse() As String
        Dim TestStr As String = email_str.Replace(",", ";")
        Dim i As Integer = 0
        Dim EmailPass As Boolean = False

        'Check for email list else, check single string
        If Not String.IsNullOrEmpty(email_str) Then
            str_parse = TestStr.Split(";")

            For i = 0 To UBound(str_parse)
                If rEx.IsMatch(Trim(str_parse(i))) Then
                    EmailPass = True
                Else
                    Return False
                End If
            Next
        Else
            Return False
        End If

        Return EmailPass
    End Function

    '----------------------------------------------------------------------
    '	Function: ValidEmailSingle
    '   This function returns true if the email passed is valid.
    '----------------------------------------------------------------------
    Public Shared Function ValidEmailSingle(ByVal email_str As String) As Boolean
        Dim rEx As Regex = New Regex("^[\w-\.]{1,}\@([\da-zA-Z-]{1,}\.){1,}[\da-zA-Z-]{2,4}$")

        If rEx.IsMatch(Trim(email_str)) Then
            Return True
        Else
            Return False
        End If

    End Function

    '----------------------------------------------------------------------
    '	Function: CaptureNull
    '   Capture null values...
    '----------------------------------------------------------------------
    Public Shared Function CaptureNull(ByVal Value As Object, ByVal Replaced As String) As String

        If Convert.IsDBNull(Value) Or Value.ToString = "" Then
            Return Replaced
        Else
            Return Value.ToString()
        End If
    End Function

    Public Shared Function CaptureNull(ByVal Value As Object, ByVal Replaced As String, ByVal rType As String) As String
        Dim Returned As Object = ""

        If Convert.IsDBNull(Value) Or Value.ToString = "" Then
            Returned = Replaced
        Else
            Select Case rType
                Case "date"
                    Returned = PadDate(CDate(Value).ToShortDateString())
                Case "sdate"
                    Returned = SplitDateShort(CDate(Value).ToShortDateString())
                Case "time"
                    Returned = CDate(Value).ToShortTimeString()
                Case "number"
                    Returned = Value.Integer
                Case "currency"
                    Returned = FormatNumber(Value, 2)
                Case "datetime"
                    Returned = String.Format("{0:MM/dd/yyyy hh:mm tt}", CDate(Value))
                Case "decimal"
                    Returned = Convert.ToDecimal(Value)
            End Select
        End If

        Return Returned
    End Function

    'encode escape characters in pop-up notes
    Public Shared Function SplitDateShort(ByVal strDate As String) As String
        Dim RetStr As String = ""

        If Not String.IsNullOrEmpty(strDate) Then
            Dim strTextShort As String() = strDate.Split("/")
            RetStr = strTextShort(0) & "/" & strTextShort(1)
        End If

        Return RetStr
    End Function

    'encode escape characters in pop-up notes
    Public Shared Function HTMLBreak(ByVal strText2 As String) As String

        If Not String.IsNullOrEmpty(strText2) Then
            strText2 = Trim(strText2)
            strText2 = Replace(strText2, vbCrLf, "<br>")
        End If

        Return strText2
    End Function

    'encode escape characters in pop-up notes
    Public Shared Function popNoteEscape(ByVal strText2 As String) As String

        If Not String.IsNullOrEmpty(strText2) Then
            strText2 = Trim(strText2)
            strText2 = Replace(strText2, "'", "&quot;")
            strText2 = Replace(strText2, """", "&quot;")
            strText2 = Replace(strText2, "&", "&amp;")
            strText2 = Replace(strText2, vbCrLf, "<br>")
            strText2 = Replace(strText2, "(", "&#40;")
            strText2 = Replace(strText2, ")", "&#41;")
        End If
        Return strText2
    End Function

    'encode escape characters in pop-up notes
    Public Shared Function qNoteEscape(ByVal strText2 As String) As String
        If Not String.IsNullOrEmpty(strText2) Then
            strText2 = Trim(strText2)
            strText2 = Replace(strText2, "'", "\'")
            strText2 = Replace(strText2, """", "&quot;")
            strText2 = Replace(strText2, "-", "_")
        End If
        Return strText2
    End Function

    'encode escape characters in javascript code
    Public Shared Function JScriptEscape(ByVal strText2 As Object) As String

        If Not IsDBNull(strText2) Then
            strText2 = Trim(strText2)
            strText2 = Replace(strText2, "\", "\\")
            strText2 = Replace(strText2, "'", "\'")
            strText2 = Replace(strText2, """", "&quot;")
            strText2 = Replace(strText2, "<br>", "\n")
            strText2 = Replace(strText2, vbCrLf, "\n")
        Else
            strText2 = ""
        End If

        Return strText2
    End Function

    'Remove last character of string
    Public Shared Function StrRemoveLast(ByVal str As String) As String
        Try
            Dim strlen As Integer = Len(str)
            Return Left(str, strlen - 1)
        Catch ex As Exception
            Return ""
        End Try

    End Function

    'parse min and hr and make min total
    Public Shared Function parse_time(ByVal min As Integer, ByVal hr As Integer) As Integer
        Dim t_min, t_hr As Integer
        t_hr = hr * 60
        t_min = min + t_hr
        Return t_min
    End Function

    'parse min and hr and make min total
    Public Shared Function split_time(ByVal time As Integer, ByVal type As String) As Integer
        Dim t_min As Integer
        Dim t_hr As Decimal
        Dim tsplit As Decimal

        Select Case type
            Case "min"
                If time > 0 Then
                    t_min = (time Mod 60)
                    If t_min = 0 Then
                        Return 0
                    Else
                        Return t_min
                    End If
                End If
            Case "hr"
                If time >= 60 Then
                    t_hr = (time / 60)
                    tsplit = Decimal.Truncate(t_hr)
                    Return tsplit
                Else
                    Return 0
                End If
        End Select

        Return 0
    End Function

    Public Shared Function GetMonthFromInt(ByVal MonthInt As Integer) As String
        Dim MonthAbbr As String = ""

        Select Case MonthInt
            Case 1
                MonthAbbr = "Jan"
            Case 2
                MonthAbbr = "Feb"
            Case 3
                MonthAbbr = "Mar"
            Case 4
                MonthAbbr = "Apr"
            Case 5
                MonthAbbr = "May"
            Case 6
                MonthAbbr = "Jun"
            Case 7
                MonthAbbr = "Jul"
            Case 8
                MonthAbbr = "Aug"
            Case 9
                MonthAbbr = "Sep"
            Case 10
                MonthAbbr = "Oct"
            Case 11
                MonthAbbr = "Nov"
            Case 12
                MonthAbbr = "Dec"
            Case Else
                MonthAbbr = "N/A"
        End Select

        Return MonthAbbr

    End Function

    Public Shared Function GetIP4Address() As String
        Dim IP4Address As String = String.Empty

        For Each IPA As IPAddress In Dns.GetHostAddresses(HttpContext.Current.Request.UserHostAddress)
            If IPA.AddressFamily.ToString() = "InterNetwork" Then
                IP4Address = IPA.ToString()
                Exit For
            End If
        Next

        If IP4Address <> String.Empty Then
            Return IP4Address
        End If

        For Each IPA As IPAddress In Dns.GetHostAddresses(Dns.GetHostName())
            If IPA.AddressFamily.ToString() = "InterNetwork" Then
                IP4Address = IPA.ToString()
                Exit For
            End If
        Next

        Return IP4Address
    End Function

    Public Shared Function MyBase64Encode(ByVal StringToEncode As String) As String
        Try
            Return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(StringToEncode))
        Catch ex As Exception
            Return "Encode Error: " & ex.Message.ToString
        End Try
    End Function

    Public Shared Function MyBase64Decode(ByVal StringToDecode As String) As String
        Try
            Dim DBytes() As Byte = Convert.FromBase64String(StringToDecode)
            Return System.Text.Encoding.UTF8.GetString(DBytes)
        Catch ex As Exception
            Return "Decode Error: " & ex.Message.ToString
        End Try
    End Function
    '//////////////////////////////////// Alpha Array Function  ///////////////////////////
    ' Convert numeric values to Alpha
    Public Shared Function LetterArray(ByVal num As Integer) As String

        Try
            Dim alphArray(450) As String
            'Create Array of Alpha to Numeric
            alphArray(0) = "A"
            alphArray(1) = "B"
            alphArray(2) = "C"
            alphArray(3) = "D"
            alphArray(4) = "E"
            alphArray(5) = "F"
            alphArray(6) = "G"
            alphArray(7) = "H"
            alphArray(8) = "I"
            alphArray(9) = "J"
            alphArray(10) = "K"
            alphArray(11) = "L"
            alphArray(12) = "M"
            alphArray(13) = "N"
            alphArray(14) = "O"
            alphArray(15) = "P"
            alphArray(16) = "Q"
            alphArray(17) = "R"
            alphArray(18) = "S"
            alphArray(19) = "T"
            alphArray(20) = "U"
            alphArray(21) = "V"
            alphArray(22) = "W"
            alphArray(23) = "X"
            alphArray(24) = "Y"
            alphArray(25) = "Z"
            alphArray(26) = "AA"
            alphArray(27) = "AB"
            alphArray(28) = "AC"
            alphArray(29) = "AD"
            alphArray(30) = "AE"
            alphArray(31) = "AF"
            alphArray(32) = "AG"
            alphArray(33) = "AH"
            alphArray(34) = "AI"
            alphArray(35) = "AJ"
            alphArray(36) = "AK"
            alphArray(37) = "AL"
            alphArray(38) = "AM"
            alphArray(39) = "AN"
            alphArray(40) = "AO"
            alphArray(41) = "AP"
            alphArray(42) = "AQ"
            alphArray(43) = "AR"
            alphArray(44) = "AS"
            alphArray(45) = "AT"
            alphArray(46) = "AU"
            alphArray(47) = "AV"
            alphArray(48) = "AW"
            alphArray(49) = "AX"
            alphArray(50) = "AY"
            alphArray(51) = "AZ"
            alphArray(52) = "BA"
            alphArray(53) = "BB"
            alphArray(54) = "BC"
            alphArray(55) = "BD"
            alphArray(56) = "BE"
            alphArray(57) = "BF"
            alphArray(58) = "BG"
            alphArray(59) = "BH"
            alphArray(60) = "BI"
            alphArray(61) = "BJ"
            alphArray(62) = "BK"
            alphArray(63) = "BL"
            alphArray(64) = "BM"
            alphArray(65) = "BN"
            alphArray(66) = "BO"
            alphArray(67) = "BP"
            alphArray(68) = "BQ"
            alphArray(69) = "BR"
            alphArray(70) = "BS"
            alphArray(71) = "BT"
            alphArray(72) = "BU"
            alphArray(73) = "BV"
            alphArray(74) = "BW"
            alphArray(75) = "BX"
            alphArray(76) = "BY"
            alphArray(77) = "BZ"
            alphArray(78) = "CA"
            alphArray(79) = "CB"
            alphArray(80) = "CC"
            alphArray(81) = "CD"
            alphArray(82) = "CE"
            alphArray(83) = "CF"
            alphArray(84) = "CG"
            alphArray(85) = "CH"
            alphArray(86) = "CI"
            alphArray(87) = "CJ"
            alphArray(88) = "CK"
            alphArray(89) = "CL"
            alphArray(90) = "CM"
            alphArray(91) = "CN"
            alphArray(92) = "CO"
            alphArray(93) = "CP"
            alphArray(94) = "CQ"
            alphArray(95) = "CR"
            alphArray(96) = "CS"
            alphArray(97) = "CT"
            alphArray(98) = "CU"
            alphArray(99) = "CV"
            alphArray(100) = "CW"
            alphArray(101) = "CY"
            alphArray(102) = "CZ"
            alphArray(103) = "DA"
            alphArray(104) = "DB"
            alphArray(105) = "DC"
            alphArray(106) = "DD"
            alphArray(107) = "DE"
            alphArray(108) = "DF"
            alphArray(109) = "DG"
            alphArray(110) = "DH"
            alphArray(111) = "DI"
            alphArray(112) = "DJ"
            alphArray(113) = "DK"
            alphArray(114) = "DL"
            alphArray(115) = "DM"
            alphArray(116) = "DN"
            alphArray(117) = "DO"
            alphArray(118) = "DP"
            alphArray(119) = "DQ"
            alphArray(120) = "DR"
            alphArray(121) = "DS"
            alphArray(122) = "DT"
            alphArray(123) = "DU"
            alphArray(124) = "DV"
            alphArray(125) = "DW"
            alphArray(126) = "DX"
            alphArray(127) = "DY"
            alphArray(128) = "DZ"
            alphArray(129) = "EA"
            alphArray(130) = "EB"
            alphArray(131) = "EC"
            alphArray(132) = "ED"
            alphArray(133) = "EE"
            alphArray(134) = "EF"
            alphArray(135) = "EG"
            alphArray(136) = "EH"
            alphArray(137) = "EI"
            alphArray(138) = "EJ"
            alphArray(139) = "EK"
            alphArray(140) = "EL"
            alphArray(141) = "EM"
            alphArray(142) = "EN"
            alphArray(143) = "EO"
            alphArray(144) = "EP"
            alphArray(145) = "EQ"
            alphArray(146) = "ER"
            alphArray(147) = "ES"
            alphArray(148) = "ET"
            alphArray(149) = "EU"
            alphArray(150) = "EV"
            alphArray(151) = "EW"
            alphArray(152) = "EX"
            alphArray(153) = "EY"
            alphArray(154) = "EZ"
            alphArray(155) = "FA"
            alphArray(156) = "FB"
            alphArray(157) = "FC"
            alphArray(158) = "FD"
            alphArray(159) = "FE"
            alphArray(160) = "FF"
            alphArray(161) = "FG"
            alphArray(162) = "FH"
            alphArray(163) = "FI"
            alphArray(164) = "FJ"
            alphArray(165) = "FK"
            alphArray(166) = "FL"
            alphArray(167) = "FM"
            alphArray(168) = "FN"
            alphArray(169) = "FO"
            alphArray(170) = "FP"
            alphArray(171) = "FQ"
            alphArray(172) = "FR"
            alphArray(173) = "FS"
            alphArray(174) = "FT"
            alphArray(175) = "FU"
            alphArray(176) = "FV"
            alphArray(177) = "FW"
            alphArray(178) = "FX"
            alphArray(179) = "FY"
            alphArray(180) = "FZ"
            alphArray(181) = "GA"
            alphArray(182) = "GB"
            alphArray(183) = "GC"
            alphArray(184) = "GD"
            alphArray(185) = "GE"
            alphArray(186) = "GF"
            alphArray(187) = "GG"
            alphArray(188) = "GH"
            alphArray(189) = "GI"
            alphArray(190) = "GJ"
            alphArray(191) = "GK"
            alphArray(192) = "GL"
            alphArray(193) = "GM"
            alphArray(194) = "GN"
            alphArray(195) = "GO"
            alphArray(196) = "GP"
            alphArray(197) = "GQ"
            alphArray(198) = "GR"
            alphArray(199) = "GS"
            alphArray(200) = "GT"
            alphArray(201) = "GU"
            alphArray(202) = "GV"
            alphArray(203) = "GW"
            alphArray(204) = "GX"
            alphArray(205) = "GY"
            alphArray(206) = "GZ"
            alphArray(207) = "HA"
            alphArray(208) = "HB"
            alphArray(209) = "HC"
            alphArray(210) = "HD"
            alphArray(211) = "HE"
            alphArray(212) = "HF"
            alphArray(213) = "HG"
            alphArray(214) = "HH"
            alphArray(215) = "HI"
            alphArray(216) = "HJ"
            alphArray(217) = "HK"
            alphArray(218) = "HL"
            alphArray(219) = "HM"
            alphArray(220) = "HN"
            alphArray(221) = "HO"
            alphArray(222) = "HP"
            alphArray(223) = "HQ"
            alphArray(224) = "HR"
            alphArray(225) = "HS"
            alphArray(226) = "HT"
            alphArray(227) = "HU"
            alphArray(228) = "HV"
            alphArray(229) = "HW"
            alphArray(230) = "HX"
            alphArray(231) = "HY"
            alphArray(232) = "HZ"
            alphArray(233) = "IA"
            alphArray(234) = "IB"
            alphArray(235) = "IC"
            alphArray(236) = "ID"
            alphArray(237) = "IE"
            alphArray(238) = "IF"
            alphArray(239) = "IG"
            alphArray(240) = "IH"
            alphArray(241) = "II"
            alphArray(242) = "IJ"
            alphArray(243) = "IK"
            alphArray(244) = "IL"
            alphArray(245) = "IM"
            alphArray(246) = "IN"
            alphArray(247) = "IO"
            alphArray(248) = "IP"
            alphArray(249) = "IQ"
            alphArray(250) = "IR"
            alphArray(251) = "IS"
            alphArray(252) = "IT"
            alphArray(253) = "IU"
            alphArray(254) = "IV"
            alphArray(255) = "IW"
            alphArray(256) = "IX"
            alphArray(257) = "IY"
            alphArray(258) = "IZ"
            alphArray(259) = "JA"
            alphArray(260) = "JB"
            alphArray(261) = "JC"
            alphArray(262) = "JD"
            alphArray(263) = "JE"
            alphArray(264) = "JF"
            alphArray(265) = "JG"
            alphArray(266) = "JH"
            alphArray(267) = "JI"
            alphArray(268) = "JJ"
            alphArray(269) = "JK"
            alphArray(270) = "JL"
            alphArray(271) = "JM"
            alphArray(272) = "JN"
            alphArray(273) = "JO"
            alphArray(274) = "JP"
            alphArray(275) = "JQ"
            alphArray(276) = "JR"
            alphArray(277) = "JS"
            alphArray(278) = "JT"
            alphArray(279) = "JU"
            alphArray(280) = "JV"
            alphArray(281) = "JW"
            alphArray(282) = "JX"
            alphArray(283) = "JY"
            alphArray(284) = "JZ"
            alphArray(285) = "KA"
            alphArray(286) = "KB"
            alphArray(287) = "KC"
            alphArray(288) = "KD"
            alphArray(289) = "KE"
            alphArray(290) = "KF"
            alphArray(291) = "KG"
            alphArray(292) = "KH"
            alphArray(293) = "KI"
            alphArray(294) = "KJ"
            alphArray(295) = "KK"
            alphArray(296) = "KL"
            alphArray(297) = "KM"
            alphArray(298) = "KN"
            alphArray(299) = "KO"
            alphArray(300) = "KP"
            alphArray(301) = "KQ"
            alphArray(301) = "KR"
            alphArray(302) = "KS"
            alphArray(303) = "KT"
            alphArray(304) = "KU"
            alphArray(305) = "KV"
            alphArray(306) = "KW"
            alphArray(307) = "KX"
            alphArray(308) = "KY"
            alphArray(309) = "KZ"
            alphArray(310) = "LA"
            alphArray(311) = "LB"
            alphArray(312) = "LC"
            alphArray(313) = "LD"
            alphArray(314) = "LE"
            alphArray(315) = "LF"
            alphArray(316) = "LG"
            alphArray(317) = "LH"
            alphArray(318) = "LI"
            alphArray(319) = "LJ"
            alphArray(320) = "LK"
            alphArray(321) = "LL"
            alphArray(322) = "LM"
            alphArray(323) = "LN"
            alphArray(324) = "LO"
            alphArray(325) = "LP"
            alphArray(326) = "LQ"
            alphArray(327) = "LR"
            alphArray(328) = "LS"
            alphArray(329) = "LT"
            alphArray(330) = "LU"
            alphArray(331) = "LV"
            alphArray(332) = "LW"
            alphArray(333) = "LX"
            alphArray(334) = "LY"
            alphArray(335) = "LZ"
            alphArray(336) = "MA"
            alphArray(337) = "MB"
            alphArray(338) = "MC"
            alphArray(339) = "MD"
            alphArray(340) = "ME"
            alphArray(341) = "MF"
            alphArray(342) = "MG"
            alphArray(343) = "MH"
            alphArray(344) = "MI"
            alphArray(345) = "MJ"
            alphArray(346) = "MK"
            alphArray(347) = "ML"
            alphArray(348) = "MM"
            alphArray(349) = "MN"
            alphArray(350) = "MO"
            alphArray(351) = "MP"
            alphArray(352) = "MQ"
            alphArray(353) = "MR"
            alphArray(354) = "MS"
            alphArray(355) = "MT"
            alphArray(356) = "MU"
            alphArray(357) = "MV"
            alphArray(358) = "MW"
            alphArray(359) = "MX"
            alphArray(360) = "MY"
            alphArray(361) = "MZ"
            alphArray(362) = "NA"
            alphArray(363) = "NB"
            alphArray(364) = "NC"
            alphArray(365) = "ND"
            alphArray(366) = "NE"
            alphArray(367) = "NF"
            alphArray(368) = "NG"
            alphArray(369) = "NH"
            alphArray(370) = "NI"
            alphArray(371) = "NJ"
            alphArray(372) = "NK"
            alphArray(373) = "NL"
            alphArray(374) = "NM"
            alphArray(375) = "NN"
            alphArray(376) = "NO"
            alphArray(377) = "NP"
            alphArray(378) = "NQ"
            alphArray(379) = "NR"
            alphArray(380) = "NS"
            alphArray(381) = "NT"
            alphArray(382) = "NU"
            alphArray(383) = "NV"
            alphArray(384) = "NW"
            alphArray(385) = "NX"
            alphArray(386) = "NY"
            alphArray(387) = "NZ"
            alphArray(388) = "OA"
            alphArray(389) = "OB"
            alphArray(390) = "OC"
            alphArray(391) = "OD"
            alphArray(392) = "OE"
            alphArray(393) = "OF"
            alphArray(394) = "OG"
            alphArray(395) = "OH"
            alphArray(396) = "OI"
            alphArray(397) = "OJ"
            alphArray(398) = "OK"
            alphArray(399) = "OL"
            alphArray(400) = "OM"
            alphArray(401) = "ON"
            alphArray(402) = "OO"
            alphArray(403) = "OP"
            alphArray(404) = "OQ"
            alphArray(405) = "OR"
            alphArray(406) = "OS"
            alphArray(407) = "OT"
            alphArray(408) = "OU"
            alphArray(409) = "OV"
            alphArray(410) = "OW"
            alphArray(411) = "OX"
            alphArray(412) = "OY"
            alphArray(413) = "OZ"
            alphArray(414) = "PA"
            alphArray(415) = "PB"
            alphArray(416) = "PC"
            alphArray(417) = "PD"
            alphArray(418) = "PE"
            alphArray(419) = "PF"
            alphArray(420) = "PG"
            alphArray(421) = "PH"
            alphArray(422) = "PI"
            alphArray(423) = "PJ"
            alphArray(424) = "PK"
            alphArray(425) = "PL"
            alphArray(426) = "PM"
            alphArray(427) = "PN"
            alphArray(428) = "PO"
            alphArray(429) = "PP"
            alphArray(430) = "PQ"
            alphArray(431) = "PR"
            alphArray(432) = "PS"
            alphArray(433) = "PT"
            alphArray(434) = "PU"
            alphArray(435) = "PV"
            alphArray(436) = "PW"
            alphArray(437) = "PX"
            alphArray(438) = "PY"
            alphArray(439) = "PZ"
            alphArray(440) = "QA"
            alphArray(441) = "QB"
            alphArray(442) = "QC"
            alphArray(443) = "QD"
            alphArray(444) = "QE"
            alphArray(445) = "QF"
            alphArray(446) = "QG"
            alphArray(447) = "QH"
            alphArray(448) = "QI"
            alphArray(449) = "QJ"
            alphArray(450) = "QK"

            Return alphArray(num)
        Catch ex As Exception
            Return "NA"
        End Try

    End Function

    Public Shared Function GetColors(ByVal a As Integer) As String
        Dim Colors(100) As String
        Colors(0) = "3B9079"
        Colors(1) = "0099FF"
        Colors(2) = "66CC66"
        Colors(3) = "CD6AC0"
        Colors(4) = "A0B3DE"
        Colors(5) = "CCDD99"
        Colors(6) = "FFCC33"
        Colors(7) = "6699CC"
        Colors(8) = "CC3366"
        Colors(9) = "663399"
        Colors(10) = "A1A079"
        Colors(11) = "0033FF"
        Colors(12) = "0099ff"
        Colors(13) = "FFCC00"
        Colors(14) = "FF0000"
        Colors(15) = "9900FF"
        Colors(16) = "993333"
        Colors(17) = "66FFCC"
        Colors(18) = "CC99FF"
        Colors(19) = "CCCC00"
        Colors(20) = "33CCCC"
        Colors(21) = "336699"
        Colors(22) = "CC6666"
        Colors(23) = "663399"
        Colors(24) = "00CC33"
        Colors(25) = "999999"
        Colors(26) = "FF33FF"
        Colors(27) = "9966FF"
        Colors(28) = "FF9966"
        Colors(29) = "0099FF"
        Colors(30) = "66CC66"
        Colors(31) = "CD6AC0"
        Colors(32) = "FF5904"
        Colors(33) = "A0B3DE"
        Colors(34) = "C6D699"
        Colors(35) = "C4AF79"
        Colors(36) = "77DC79"
        Colors(37) = "AC7F79"
        Colors(38) = "C3F679"
        Colors(39) = "A1A079"
        Colors(40) = "611D79"
        Colors(41) = "A31179"
        Colors(42) = "4F9679"
        Colors(43) = "3B9079"
        Colors(44) = "4B5F79"
        Colors(45) = "DFA479"
        Colors(46) = "977979"
        Colors(47) = "D4C579"
        Colors(48) = "35A179"
        Colors(49) = "1A3779"
        Colors(50) = "245D79"
        Colors(51) = "6DB679"
        Colors(52) = "1F2679"
        Colors(53) = "558579"
        Colors(54) = "6CFD79"
        Colors(55) = "4AA679"
        Colors(56) = "096879"
        Colors(57) = "4C1879"
        Colors(58) = "B4E079"
        Colors(59) = "E3DB79"
        Colors(60) = "F3AA79"
        Colors(61) = "87AB79"
        Colors(62) = "408079"
        Colors(63) = "7CCB79"
        Colors(64) = "DEEB79"
        Colors(65) = "C23D79"
        Colors(66) = "886479"
        Colors(67) = "15BC79"
        Colors(68) = "C82D79"
        Colors(69) = "B9D079"
        Colors(70) = "144779"
        Colors(71) = "AEF179"
        Colors(72) = "B26E79"
        Colors(73) = "F46279"
        Colors(74) = "5CE779"
        Colors(75) = "8CE179"
        Colors(76) = "9CB079"
        Colors(77) = "30B179"
        Colors(78) = "E8CA79"
        Colors(79) = "251679"
        Colors(80) = "86F279"
        Colors(81) = "6B4479"
        Colors(82) = "316A79"
        Colors(83) = "BE0779"
        Colors(84) = "703379"
        Colors(85) = "62D679"
        Return Colors(a)
    End Function

#End Region

#Region " DropDown Command Functions "
    Public Shared Function DDCreateDataSource(ByVal sqlString As String) As ICollection

        ' Create a table to store data for the DropDownList control.
        Dim dt As DataTable = New DataTable

        Dim db As Database = DatabaseFactory.CreateDatabase()
        Dim sqlCommand As String = sqlString
        Dim dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)

        Try
            Using rs As IDataReader = db.ExecuteReader(dbCommand)
                ' Define the columns of the table.
                dt.Columns.Add(New DataColumn("TextField", GetType(String)))
                dt.Columns.Add(New DataColumn("ValueField", GetType(String)))

                'DD Header
                dt.Rows.Add(DDCreateRow("Select...", "", dt))

                While rs.Read
                    ' Populate the table with values.
                    dt.Rows.Add(DDCreateRow(rs(1), rs(0), dt))
                End While

                ' Create a DataView from the DataTable to act as the data source
                ' for the DropDownList control.
                Dim dv As DataView = New DataView(dt)
                Return dv
            End Using
        Catch ex As Exception
            'Catch Error
            Throw ex
        End Try
    End Function

    Public Shared Function DDCreateRow(ByVal Text As String, ByVal Value As String, ByVal dt As DataTable) As DataRow

        ' Create a DataRow using the DataTable defined in the 
        ' CreateDataSource method.
        Dim dr As DataRow = dt.NewRow()

        ' This DataRow contains the TextField and ValueField 
        ' fields, as defined in the CreateDataSource method. Set the 
        ' fields with the appropriate value. Remember that column 0 
        ' is defined as TextField, and column 1 is defined as 
        ' ValueField.
        dr(0) = Text
        dr(1) = Value

        Return dr

    End Function

    Public Shared Function MenuXMLItems() As ICollection
        Dim dt As DataTable = New DataTable

        ' Define the columns of the table.
        dt.Columns.Add(New DataColumn("TextField", GetType(String)))
        dt.Columns.Add(New DataColumn("ValueField", GetType(String)))

        'DD Header
        dt.Rows.Add(DDCreateRow("Select Menu Item...", "", dt))

        Try
            'you can get an array of files (their path) by using:
            Dim menuFolder As String = ConfigurationManager.AppSettings("MenuFolder")

            Dim myFile As String

            For Each myFile In Directory.GetFiles(menuFolder, "*.xml")
                dt.Rows.Add(DDCreateRow(Path.GetFileName(myFile), Path.GetFileName(myFile), dt))
            Next

            Dim dv As DataView = New DataView(dt)
            Return dv

        Catch ex As Exception
            Throw ex
        Finally

        End Try
    End Function

    Public Shared Function TemplateFiles(ByVal TempFolder As String) As ICollection
        Dim dt As DataTable = New DataTable

        ' Define the columns of the table.
        dt.Columns.Add(New DataColumn("TextField", GetType(String)))
        dt.Columns.Add(New DataColumn("ValueField", GetType(String)))

        'DD Header
        dt.Rows.Add(DDCreateRow("Select File...", "", dt))

        Try
            'you can get an array of files (their path) by using:
            Dim menuFolder As String = TempFolder

            Dim myFile As String
            For Each myFile In Directory.GetFiles(menuFolder, "*.*")
                'myFile = Path.GetFileName(myFile)
                dt.Rows.Add(DDCreateRow(Path.GetFileNameWithoutExtension(myFile), Path.GetFileName(myFile), dt))
            Next

            Dim dv As DataView = New DataView(dt)
            Return dv

        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Shared Function FieldSheetFiles(ByVal TempFolder As String) As ICollection
        Dim dt As DataTable = New DataTable

        ' Define the columns of the table.
        dt.Columns.Add(New DataColumn("TextField", GetType(String)))
        dt.Columns.Add(New DataColumn("ValueField", GetType(String)))

        'DD Header
        dt.Rows.Add(DDCreateRow("Select Fieldsheet File...", "", dt))

        Try
            'you can get an array of files (their path) by using:
            Dim menuFolder As String = TempFolder

            Dim myFile As String

            For Each myFile In Directory.GetFiles(menuFolder, "*.*")
                ' myFile = Left(Path.GetFileName(myFile), Len(Path.GetFileName(myFile)) - 4)
                dt.Rows.Add(DDCreateRow(Path.GetFileName(myFile), Path.GetFileName(myFile), dt))
            Next

            Dim dv As DataView = New DataView(dt)
            Return dv

        Catch ex As Exception
            Throw ex
        End Try
    End Function
#End Region

#Region " Global Application Functions "

    Public Shared Function GetApplicationSetting(ByVal SettingKey As String) As String
        Dim sqlCommand As String = "SELECT MySetting FROM ApplicationSettings WHERE SettingType = '" & SettingKey & "'"
        Dim dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)
        Dim str As String = ""

        Try
            Using rs As IDataReader = db.ExecuteReader(dbCommand)
                If rs.Read() Then
                    str = rs(0).ToString
                Else
                    str = ""
                End If
            End Using
        Catch ex As Exception
            Return "Setting Error: " & ex.Message.ToString
        End Try
        Return str
    End Function

    Public Shared Function SetApplicationSettings(ByVal AppKey As String, ByVal AppValue As String) As String
        Dim sqlCommand As String = "SetApplicationSettings"
        Dim i As Integer = 0

        Try
            Using dbCommand As DbCommand = db.GetStoredProcCommand(sqlCommand)
                db.AddInParameter(dbCommand, "AppKey", DbType.String, AppKey)
                db.AddInParameter(dbCommand, "AppValue", DbType.String, AppValue)
                db.AddOutParameter(dbCommand, "retval", DbType.Int32, 4)
                i = db.ExecuteNonQuery(dbCommand)
                Dim retval As Integer = db.GetParameterValue(dbCommand, "retval")
                Return retval.ToString
            End Using
        Catch ex As Exception
            Return ex.Message.ToString
        End Try
    End Function

    Public Shared Function GetClientImage(ByVal clientID As Integer, ByVal CompanyType As String) As String
        Dim db As Database = DatabaseFactory.CreateDatabase()
        Dim sqlCommand As String = "SELECT LogoFile FROM Clients WHERE ClientID = " & clientID.ToString
        Dim dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)
        Dim LogoRet As String = ""
        Dim DefaultLogo As String = "nvms_logo.jpg"

        Select Case CompanyType
            Case "FIELDREP"
                LogoRet = DefaultLogo
            Case "NVMS"
                LogoRet = DefaultLogo
            Case "CLIENT"
                Try
                    Using rs As SqlDataReader = db.ExecuteReader(dbCommand)
                        If rs.HasRows Then
                            rs.Read()
                            LogoRet = rs("LogoFile")
                        Else
                            LogoRet = DefaultLogo
                        End If
                    End Using
                Catch ex As Exception
                    ' LogoRet = DefaultLogo
                    Throw ex
                End Try
        End Select

        Return LogoRet
    End Function

    '-----------------------------------------------------------------------------------------
    '	Function: UserClientVendor
    '   This function returns Client or Vendor name according to values passed.
    '-----------------------------------------------------------------------------------------
    Public Shared Function UserClientVendor(ByVal CompanyType As String, ByVal userID As Integer, Optional ByVal Farm As String = "") As String

        Dim CompName As New StringBuilder
        Dim db As Database = DatabaseFactory.CreateDatabase()

        If CompanyType <> "" And userID >= 1 Then
            Select Case CompanyType
                Case "FIELDREP"
                    Dim sqlCommand As String = "SELECT VendorName, VendorCode FROM Vendors WHERE VendorID IN (SELECT companyID FROM user_groups WHERE userID= " & userID.ToString & ") ORDER BY VendorName"
                    Dim dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)
                    Using rs As SqlDataReader = db.ExecuteReader(dbCommand)
                        If rs.HasRows Then
                            While rs.Read()
                                CompName.Append(rs("VendorName") & " [" & rs("VendorCode") & "]<br>")
                            End While
                        Else
                            CompName.Append("<table width='100%'><tr><td class=error>No Default Company Loaded >> Please contact your representative before using the system...</td></tr></table>")
                        End If
                    End Using
                Case "CLIENT"
                    Dim sqlCommand As String = "SELECT ClientName, ClientCode, LogoFile, active FROM Clients WHERE ClientID IN (SELECT companyID FROM user_groups WHERE userID= " & userID.ToString & ") ORDER BY ClientName"
                    Dim dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)
                    Using rs As SqlDataReader = db.ExecuteReader(dbCommand)
                        If rs.HasRows Then
                            While rs.Read()
                                If rs("active") Then
                                    CompName.Append(rs("ClientName") & " [" & rs("ClientCode") & "]<br>")
                                Else
                                    CompName.Append("<span class=grayText><i>" & rs("ClientName") & " [" & rs("ClientCode") & "] (Inactive)</i></span><br>")
                                End If
                            End While
                        Else
                            CompName.Append("<span class=error>No Default Company Loaded >> Please contact your representative before using the system...</span>")
                        End If
                    End Using
                Case Else
                    CompName.Append("NVMS [" & Farm & "]")
            End Select
        Else
            CompName.Append("No Client/Vendor Selected...")
        End If
        Return CompName.ToString
    End Function

    Public Shared Function DynamicQueryResults(ByVal sql As String) As String
        Dim db As Database = DatabaseFactory.CreateDatabase()
        Dim html As New StringBuilder
        Dim sqlCommand As String = sql
        Dim dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)
        Dim i As Integer = 0
        Dim x As Integer = 0
        Dim fc As Integer = 0

        Try
            Using rs As SqlDataReader = db.ExecuteReader(dbCommand)
                If rs.HasRows Then
                    'Build Header
                    fc = rs.FieldCount
                    html.Append("<table width='100%' cellspacing='1' cellpadding='1' border='1'>")
                    html.Append("<thead class='tHeader'>")
                    html.Append("<tr>")
                    For x = 0 To rs.FieldCount - 1
                        html.Append("<td><b>" & rs.GetName(x) & "</b></td>")
                    Next
                    html.Append("</tr>")
                    html.Append("<thead>")

                    Dim TotalRow(fc) As String

                    While rs.Read()
                        If i Mod 2 = 0 Then
                            html.Append("<tr style='background-color:#ffffff'>")
                        Else
                            html.Append("<tr>")
                        End If

                        For x = 0 To rs.FieldCount - 1
                            'Get Data Type
                            Dim CountMe As Boolean = False
                            Dim colValue As String = ""
                            Dim colType As String = rs(x).GetType.Name

                            Select Case colType
                                Case "String"
                                    colValue = rs(x).ToString
                                    TotalRow(x) = "" 'colType
                                    html.Append("<td>" & colValue & "</td>")
                                Case "Decimal"
                                    colValue = Common.MyNumberNoCurr(rs(x))
                                    CountMe = True
                                    If CDec(colValue) Then
                                        If x = 0 Then
                                            TotalRow(x) = CDec(colValue)
                                        Else
                                            TotalRow(x) = CDec(TotalRow(x)) + CDec(colValue)
                                        End If
                                    Else
                                        TotalRow(x) = 0
                                    End If

                                    html.Append("<td align='right'>" & colValue & "</td>")
                                Case "Int32"
                                    colValue = rs(x).ToString
                                    CountMe = True

                                    If IsNumber(colValue) Then
                                        If x = 0 Then
                                            TotalRow(x) = CInt(colValue)
                                        Else
                                            TotalRow(x) = CInt(TotalRow(x)) + CInt(colValue)
                                        End If
                                    Else
                                        TotalRow(x) = 0
                                    End If

                                    html.Append("<td align='center'>" & colValue & "</td>")
                                Case "Byte"
                                    colValue = rs(x).ToString
                                    CountMe = True
                                    If x = 0 Then
                                        TotalRow(x) = CInt(colValue)
                                    Else
                                        TotalRow(x) = CInt(TotalRow(x)) + CInt(colValue)
                                    End If
                                    html.Append("<td align='center'>" & colValue & "</td>")
                                Case Else
                                    colValue = rs(x).ToString
                                    TotalRow(x) = "" 'colType
                                    html.Append("<td>" & colValue & "</td>")
                            End Select
                            'html.Append("<td>" & rs(x) & "</td>")

                        Next
                        html.Append("</tr>")
                        i += 1
                    End While

                    'Print Total Row
                    Dim u As Integer = 0
                    html.Append("<tr style='background-color:#e0e0e0; font-weight:bold'>")
                    For u = 0 To TotalRow.Length - 2
                        If u = 0 Then
                            html.Append("<td align='center'>Totals:</td>")
                        Else
                            html.Append("<td align='center'>" & TotalRow(u) & "</td>")
                        End If
                    Next
                    html.Append("</tr>")

                    html.Append("<tr><td bgcolor='#e0e0e0' colspan=" & fc.ToString & "><b>Total Records: " & i.ToString & "</b></td></tr>")
                    html.Append("</table>")
                Else
                    html.Append("<div class='error'>RecordSet 1 - No records found for this query...</div>")
                End If

                If rs.NextResult() Then
                    If rs.HasRows Then
                        'Build Header
                        fc = rs.FieldCount
                        html.Append("<table width='100%' cellspacing='1' cellpadding='1' border='1'>")
                        html.Append("<thead class='tHeader'>")
                        html.Append("<tr>")
                        For x = 0 To rs.FieldCount - 1
                            html.Append("<td><b>" & rs.GetName(x) & "</b></td>")
                        Next
                        html.Append("</tr>")
                        html.Append("<thead>")

                        Dim TotalRow(fc) As String
                        i = 0

                        While rs.Read()
                            If i Mod 2 = 0 Then
                                html.Append("<tr style='background-color:#ffffff'>")
                            Else
                                html.Append("<tr>")
                            End If

                            For x = 0 To rs.FieldCount - 1
                                'Get Data Type
                                Dim CountMe As Boolean = False
                                Dim colValue As String = ""
                                Dim colType As String = rs(x).GetType.Name

                                Select Case colType
                                    Case "String"
                                        colValue = rs(x).ToString
                                        TotalRow(x) = "" 'colType
                                        html.Append("<td>" & colValue & "</td>")
                                    Case "Decimal"
                                        colValue = Common.MyNumberNoCurr(rs(x))
                                        CountMe = True
                                        If x = 0 Then
                                            TotalRow(x) = CDec(colValue)
                                        Else
                                            TotalRow(x) = CDec(TotalRow(x)) + CDec(colValue)
                                        End If
                                        html.Append("<td align='right'>" & colValue & "</td>")
                                    Case "Int32"
                                        colValue = rs(x).ToString
                                        CountMe = True
                                        If x = 0 Then
                                            TotalRow(x) = CInt(colValue)
                                        Else
                                            TotalRow(x) = CInt(TotalRow(x)) + CInt(colValue)
                                        End If
                                        html.Append("<td align='center'>" & colValue & "</td>")
                                    Case "Byte"
                                        colValue = rs(x).ToString
                                        CountMe = True
                                        If x = 0 Then
                                            TotalRow(x) = CInt(colValue)
                                        Else
                                            TotalRow(x) = CInt(TotalRow(x)) + CInt(colValue)
                                        End If
                                        html.Append("<td align='center'>" & colValue & "</td>")
                                    Case Else
                                        colValue = rs(x).ToString
                                        TotalRow(x) = "" 'colType
                                        html.Append("<td>" & colValue & "</td>")
                                End Select
                                'html.Append("<td>" & rs(x) & "</td>")
                            Next
                            html.Append("</tr>")
                            i += 1
                        End While

                        'Print Total Row
                        Dim u As Integer = 0
                        html.Append("<tr style='background-color:#e0e0e0; font-weight:bold'>")
                        For u = 0 To TotalRow.Length - 2
                            If u = 0 Then
                                html.Append("<td align='center'>Totals:</td>")
                            Else
                                html.Append("<td align='center'>" & TotalRow(u) & "</td>")
                            End If
                        Next
                        html.Append("</tr>")

                        html.Append("<tr><td bgcolor='#e0e0e0' colspan=" & fc.ToString & "><b>Total Records: " & i.ToString & "</b></td></tr>")
                        html.Append("</table>")

                    End If
                End If

            End Using
        Catch ex As Exception
            Return ex.Message.ToString
        End Try

        Return html.ToString()
    End Function

#End Region
End Class
