Imports Microsoft.VisualBasic
Imports System.Xml
Imports System.IO
Imports NVMS.InternetTools
Imports System.Net
Imports Newtonsoft.Json.Linq
Imports System.Data
Imports System.Data.Common
Imports System.Data.SqlClient
Imports Microsoft.Practices.EnterpriseLibrary.Data

Public Class IPCountryTest
    Private CacheFolder As String = ConfigurationManager.AppSettings("LogFolder")
    Public factory As DatabaseProviderFactory = New DatabaseProviderFactory()
    Public db As Database = factory.CreateDefault()

    Private _FindKey As String = "countryCode"
    Public Property FindKey() As String
        Get
            Return (_FindKey)
        End Get
        Set(ByVal Value As String)
            _FindKey = Value
        End Set
    End Property

    Private _KeyFound As String = ""
    Public Property KeyFound() As String
        Get
            Return (_KeyFound)
        End Get
        Set(ByVal Value As String)
            _KeyFound = Value
        End Set
    End Property

    Private _KeyPass As Boolean = True
    Public Property KeyPass() As Boolean
        Get
            Return (_KeyPass)
        End Get
        Set(ByVal Value As Boolean)
            _KeyPass = Value
        End Set
    End Property

    Private _ExcludeList As String = Common.GetApplicationSetting("ExcludeCountryList")
    Public Property ExcludeList() As String
        Get
            Return (_ExcludeList)
        End Get
        Set(ByVal Value As String)
            _ExcludeList = Value
        End Set
    End Property

    Function GetGeoLocation() As String
        'Build Request
        Dim IPAddress As String = GetIP4Address() '"202.112.28.100" 'China
        Dim MyRequest As System.Net.HttpWebResponse = Nothing

        Dim ErrResp As New StringBuilder
        Dim i As Integer = 0
        Dim x As Integer = 0
        Dim Resp As String = ""
        Dim ReturnCode As String = ""
        Dim DBaseCheckValid As Boolean = False
        Dim RequestURL As String = "http://api.ipinfodb.com/v3/ip-country/?key=4171ea422e5f0db17e115ff983233c52cc7122bb345006e0d2ab680d0e812388&format=json&ip=" & IPAddress
        Dim sqlCommand As String = "process_GeoIPLookup"

        'Check For Existing IP Entry In Database //////////////////////////////////////
        Try
            Using dbCommand As DbCommand = db.GetStoredProcCommand(sqlCommand)
                ' Add Parameters to SPROC
                db.AddInParameter(dbCommand, "Action", DbType.String, "CHECK")
                db.AddInParameter(dbCommand, "IPAddress", DbType.String, IPAddress)
                db.AddInParameter(dbCommand, "CountryCode", DbType.String, "")
                db.AddOutParameter(dbCommand, "retval", DbType.Int32, 4)

                x = db.ExecuteNonQuery(dbCommand)
                Dim CheckVal As Integer = db.GetParameterValue(dbCommand, "retval")
                If CheckVal = 1 Then
                    ErrResp.Append("IP Exists & Blocked: " & IPAddress)
                    DBaseCheckValid = True
                    Me.KeyPass = False
                End If
            End Using

        Catch ex As Exception
            Me.KeyPass = False
            Resp = "Get GeoLocation DB Check Error: " & ex.Message.ToString
            WriteLog("Get GeoLocation DB Check Error: " & ex.Message.ToString)
        End Try

        'Get IP Info ////////////////////////////////////////////////////////////////
        If Not DBaseCheckValid Then
            Try
                Dim rSend As New wwHttp
                rSend.PostMode = HttpPostMode.UrlEncoded
                rSend.Timeout = 60

                If Me.ExcludeList = "None" Then
                    ErrResp.Append("IP Response: No blocking enabled.. Setting: " & Me.ExcludeList)
                Else
                    If Not String.IsNullOrEmpty(RequestURL) Then

                        If Not rSend.CreateWebRequestObject(RequestURL) Then
                            ErrResp.Append("Connection Error: " & rSend.ErrorMessage)
                        Else
                            MyRequest = rSend.GetUrlResponse(RequestURL)
                            ReturnCode = MyRequest.StatusCode.ToString

                            If ReturnCode <> "OK" Then
                                ErrResp.Append("Error: " & ReturnCode & " - " & MyRequest.StatusDescription)
                            Else
                                Resp = GetJSONResult(MyRequest)

                                'Log response ////////////////////////////
                                ErrResp.Append("IP Response: " & IPAddress & " (" & Me.FindKey & ") = " & Resp)

                                'Check Excluded List
                                If Me.ExcludeList = "BlockAllNonUS" Then
                                    If Trim(Resp) <> "-" Or Trim(Resp) <> "US" Then
                                        Me.KeyPass = False
                                        ErrResp.Append(" >> IP Excluded: " & Me.ExcludeList)
                                    End If
                                Else
                                    Dim StrArray() As String = Me.ExcludeList.Split(",")
                                    For i = 0 To UBound(StrArray)
                                        If Trim(Resp) = Trim(StrArray(i)) Then
                                            'Key Match
                                            Me.KeyPass = False
                                            ErrResp.Append(" >> IP Excluded: " & StrArray(i))
                                        End If
                                    Next
                                End If

                                'Record Excluded IP Address //////////////////////////////////////////////
                                If Not KeyPass Then
                                    Using dbCommand As DbCommand = db.GetStoredProcCommand(sqlCommand)
                                        ' Add Parameters to SPROC
                                        db.AddInParameter(dbCommand, "Action", DbType.String, "UPDATE")
                                        db.AddInParameter(dbCommand, "IPAddress", DbType.String, IPAddress)
                                        db.AddInParameter(dbCommand, "CountryCode", DbType.String, Trim(Resp))
                                        db.AddOutParameter(dbCommand, "retval", DbType.Int32, 4)

                                        x = db.ExecuteNonQuery(dbCommand)
                                        Dim CheckVal As Integer = db.GetParameterValue(dbCommand, "retval")
                                        If CheckVal = 2 Then
                                            ErrResp.Append(" ** IP Found/Updated")
                                        ElseIf CheckVal = 3 Then
                                            ErrResp.Append(" ** IP Added")
                                        End If
                                    End Using
                                End If

                                ErrResp.Append(" >>> IP Pass: " & Me.KeyPass & " [" & Resp & "]")
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                Me.KeyPass = True
                Resp = "Get GeoLocation Error: " & ex.Message.ToString
                WriteLog("Get GeoLocation Error: " & ex.Message.ToString)
            End Try
        End If

        WriteLog(ErrResp.ToString)
        Return Resp
    End Function

    Private Function GetJSONResult(ByVal servResponse As HttpWebResponse) As String
        Dim KeyReturn As String = ""
        Dim serviceResponseStream As StreamReader = Nothing
        Dim js As New JObject

        Try
            Dim serviceResponse As HttpWebResponse = CType(servResponse, HttpWebResponse)
            serviceResponseStream = New StreamReader(serviceResponse.GetResponseStream, System.Text.Encoding.ASCII)
        Catch e As WebException
            serviceResponseStream = New StreamReader(e.Response.GetResponseStream, System.Text.Encoding.ASCII)
        Catch e As Exception
            Return "Web Response Error: " & e.Message.ToString
        End Try

        Try
            Dim serviceResponseBody As String
            serviceResponseBody = serviceResponseStream.ReadToEnd
            serviceResponseStream.Close()

            'JSON Parse
            js = JObject.Parse(serviceResponseBody)
            KeyReturn = js.Item(Me.FindKey).ToString
        Catch ex As Exception
            KeyReturn = "Get JSON Result Error: " & ex.Message.ToString
        End Try

        Return KeyReturn
    End Function


    Private Function GetIP4Address() As String
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

    '----------------------------------------------------------------------
    '	Function: WriteLog
    '   This Sub writes a log file when sending email
    '	use this when sending emails.
    '----------------------------------------------------------------------
    Sub WriteLog(ByVal log_text As String)
        Dim LogFile As String = CacheFolder & "\ip_test\" & LogDate(Date.Today.ToShortDateString, "") & ".txt"
        Dim Server As String = ConfigurationManager.AppSettings("ServerName")
        Dim gLogFile As StreamWriter = Nothing

        SyncLock Me
            Try
                If File.Exists(LogFile) Then
                    gLogFile = File.AppendText(LogFile)
                Else
                    gLogFile = File.CreateText(LogFile)
                End If

                gLogFile.WriteLine(vbLf & Date.Now & " [" & Server & "] ---- " & log_text)
                gLogFile.Close()
            Catch e As Exception
                Throw New Exception("Cannot open log file: " & LogFile & e.Message)
                gLogFile.Close()
                gLogFile.Dispose()
            Finally
                gLogFile.Dispose()
            End Try
        End SyncLock
    End Sub

    '----------------------------------------------------------------------
    '	Function: LogDate
    '   Converts date to log file date
    '----------------------------------------------------------------------
    Function LogDate(ByVal stringToCheck As String, ByVal defaultOut As String) As String
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
End Class
