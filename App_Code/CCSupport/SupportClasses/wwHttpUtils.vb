Imports Microsoft.VisualBasic
Imports System
Imports System.Text
Imports System.IO

Namespace NVMS.InternetTools
    ' <summary>
    ' wwHttp Utility class to provide UrlEncoding without the need to use
    ' the System.Web libraries (too much overhead)
    ' </summary>
    Public Class wwHttpUtils
        ' <summary>
        ' UrlEncodes a string without the requirement for System.Web
        ' </summary>
        ' <param name="String"></param>
        ' <returns></returns>
        Public Shared Function UrlEncode(ByVal InputString As String) As String
            Dim sr As StringReader = New StringReader(InputString)
            Dim sb As StringBuilder = New StringBuilder(InputString.Length)

            Do
                Dim lnVal As Integer = sr.Read()
                If lnVal = -1 Then
                    Exit Do
                End If
                Dim lcChar As Char = ChrW(lnVal)

                If lcChar >= "a"c AndAlso lcChar < "z"c OrElse lcChar >= "A"c AndAlso lcChar < "Z"c OrElse lcChar >= "0"c AndAlso lcChar < "9"c Then
                    sb.Append(lcChar)
                ElseIf lcChar = " "c Then
                    sb.Append("+")
                Else
                    sb.AppendFormat("%{0:X2}", lnVal)
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

            Dim Result As String = wwHttpUtils.UrlDecode(UrlEncodedString.Substring(lnStart, Index2 - lnStart))

            Return Result
        End Function

    End Class
End Namespace
