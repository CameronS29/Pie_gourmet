Imports System.IO
Imports Mandrill
Imports Mandrill.MandrillApi
Imports MailBee
Imports MailBee.SmtpMail
Imports MailBee.Mime
Imports MailBee.Security

'Send Mail class using Mandrill
Public Class Sendmail

    'Set Variables
    Private _mailTo As String = ""
    Private _mailToName As String
    Private _mailFrom As String = "shop@piegourmet.com"
    Private _mailCC As String = ""
    Private _mailBCC As String = ""
    Private _mailSubject As String = ""
    Private _mailBody As String = ""
    Private _HTMLBodyText As String = ""
    Private _mailHTML As Integer = 1
    Private _mailAttachments As String = ""
    Private _MailError As String = ""
    Private _MailStyle As String = "assets/css/main.css"
    Private _PrintLegal As Boolean = False
    Private _useQueue As Boolean = False
    Private _useHeader As Boolean = True
    Private _ReturnCode As Integer = 0
    Private _MailLogo As String = "assets/images/email_logo.jpg"
    Private _FooterFile As String = "legal.htm"
    Private _Urgent As Boolean = False
    Private _MyMessageObject As Mandrill.Models.EmailMessage

    'Server Settings
    Private _mailServer As String = ConfigurationManager.AppSettings("MailServer")
    Private _mailUser As String = ConfigurationManager.AppSettings("MailUser")
    Private _mailPassword As String = ConfigurationManager.AppSettings("MailPassword")
    Private msgQueue As String = ConfigurationManager.AppSettings("MessageQ")
    Private MailBeeLicense As String = ConfigurationManager.AppSettings("MailBeeLicenseKey")
    Private MandrillAPIKey As String = ConfigurationManager.AppSettings("MandrillAPIKey")
    Private webURL As String = ConfigurationManager.AppSettings("AppURL")
    Private MailLog As String = ConfigurationManager.AppSettings("LogFolder")

#Region " Define Mail Properties"
    'Set Properties
    Public Property MailServer() As String
        Get
            Return (_mailServer)
        End Get
        Set(ByVal Value As String)
            _mailServer = Value
        End Set
    End Property

    Public Property MailUser() As String
        Get
            Return (_mailUser)
        End Get
        Set(ByVal Value As String)
            _mailUser = Value
        End Set
    End Property

    Public Property MailPassword() As String
        Get
            Return (_mailPassword)
        End Get
        Set(ByVal Value As String)
            _mailPassword = Value
        End Set
    End Property

    Public Property MailStyle() As String
        Get
            Return (_MailStyle)
        End Get
        Set(ByVal Value As String)
            _MailStyle = Value
        End Set
    End Property

    Public Property MailLogo() As String
        Get
            Return (_MailLogo)
        End Get
        Set(ByVal Value As String)
            _MailLogo = Value
        End Set
    End Property

    Public Property MailError() As String
        Get
            Return (_MailError)
        End Get
        Set(ByVal Value As String)
            _MailError = Value
        End Set
    End Property

    Public Property mailTo() As String
        Get
            Return (_mailTo)
        End Get
        Set(ByVal Value As String)
            _mailTo = Value
        End Set
    End Property

    Public Property mailToName() As String
        Get
            Return (_mailToName)
        End Get
        Set(ByVal Value As String)
            _mailToName = Value
        End Set
    End Property

    Public Property mailFrom() As String
        Get
            Return (_mailFrom)
        End Get
        Set(ByVal Value As String)
            _mailFrom = Value
        End Set
    End Property

    Public Property mailCC() As String
        Get
            Return (_mailCC)
        End Get
        Set(ByVal Value As String)
            _mailCC = Value
        End Set
    End Property

    Public Property mailBCC() As String
        Get
            Return (_mailBCC)
        End Get
        Set(ByVal Value As String)
            _mailBCC = Value
        End Set
    End Property

    Public Property mailSubject() As String
        Get
            Return (_mailSubject)
        End Get
        Set(ByVal Value As String)
            _mailSubject = Value
        End Set
    End Property

    Public Property mailBody() As String
        Get
            Return (_mailBody)
        End Get
        Set(ByVal Value As String)
            _mailBody = Value
        End Set
    End Property

    Public Property mailHTML() As Integer
        Get
            Return (_mailHTML)
        End Get
        Set(ByVal Value As Integer)
            _mailHTML = Value
        End Set
    End Property

    Public Property mailAttachments() As String
        Get
            Return (_mailAttachments)
        End Get
        Set(ByVal Value As String)
            _mailAttachments = Value
        End Set
    End Property

    Public Property ErrorCode() As Integer
        Get
            Return (_ReturnCode)
        End Get
        Set(ByVal Value As Integer)
            _ReturnCode = Value
        End Set
    End Property

    Public Property useQueue() As Boolean
        Get
            Return (_useQueue)
        End Get
        Set(ByVal Value As Boolean)
            _useQueue = Value
        End Set
    End Property

    Public Property UrgentMessage() As Boolean
        Get
            Return (_Urgent)
        End Get
        Set(ByVal Value As Boolean)
            _Urgent = Value
        End Set
    End Property

    Public Property PrintHeader() As Boolean
        Get
            Return (_useHeader)
        End Get
        Set(ByVal Value As Boolean)
            _useHeader = Value
        End Set
    End Property

    Public Property PrintLegal() As Boolean
        Get
            Return (_PrintLegal)
        End Get
        Set(ByVal Value As Boolean)
            _PrintLegal = Value
        End Set
    End Property

    Public Property HTMLBodyText() As String
        Get
            Return (_HTMLBodyText)
        End Get
        Set(ByVal Value As String)
            _HTMLBodyText = Value
        End Set
    End Property

    Public Property FooterFile() As String
        Get
            Return (_FooterFile)
        End Get
        Set(ByVal Value As String)
            _FooterFile = Value
        End Set
    End Property

    Public Property MyMessageObject() As Mandrill.Models.EmailMessage
        Get
            Return (_MyMessageObject)
        End Get
        Set(ByVal Value As Mandrill.Models.EmailMessage)
            _MyMessageObject = Value
        End Set
    End Property
#End Region

    '----------------------------------------------------------------------
    '	Function: SendEmail
    '   This Function sends email using MailBee message queueing or not (whatever...)
    '----------------------------------------------------------------------
    Public Sub SendEmail()
        'Set SMTP License Key:
        Dim mandrill_api As New Mandrill.MandrillApi(MandrillAPIKey)
        Dim msg As New Mandrill.Models.EmailMessage
        Dim toArray(1) As Mandrill.Models.EmailAddress
        If Me.mailHTML = 1 Then
            msg.Html = BuildHTMLEmail()
        Else
            msg.Html = Me.mailBody
        End If
        toArray(0) = New Mandrill.Models.EmailAddress(_mailTo)
        msg.To = toArray
        msg.FromEmail = _mailFrom
        msg.BccAddress = _mailBCC
        msg.Subject = _mailSubject

        'Dim attFiles() As String
        'Dim i As Integer
        'Dim emailAttachment As Mandrill.Models.Attachment
        Dim sendMessageRequest As Mandrill.Requests.Messages.SendMessageRequest
        Dim res As Mandrill.Models.EmailResult

        'If Me.mailAttachments <> "" Then
        '    emailAttachment = New Mandrill.Models.Attachment()
        '    If InStr(1, Me.mailAttachments, "|", 1) >= 1 Then
        '        attFiles = Split(Me.mailAttachments, "|")
        '        For i = 0 To UBound(attFiles)
        '            If Trim(attFiles(i)) <> "" Then
        '                emailAttachment.Add(Trim(attFiles(i)))
        '            End If
        '        Next
        '        msg.Attachments = emailAttachment
        '    Else
        '        emailAttachment.Content = Me.mailAttachments
        '        msg.Attachments = emailAttachment
        '    End If
        'End If

        If Me.UrgentMessage Then
            msg.Important = True
        End If

        Try
            Me.MyMessageObject = msg
            sendMessageRequest = New Mandrill.Requests.Messages.SendMessageRequest(msg)
            Dim resTask As Threading.Tasks.Task(Of List(Of Mandrill.Models.EmailResult)) = mandrill_api.SendMessage(sendMessageRequest)
            resTask.Wait()

            If resTask.Result IsNot Nothing And resTask.Result.Count > 0 Then
                res = resTask.Result.First()
            Else
                WriteLog("Time: " & Now.ToShortTimeString & " | User: " & _mailFrom & " | Subject: " & _mailSubject & " | Email Error: Invalid return result from provider")
                MailError = "Invalid return result from provider"
                Return
            End If

            WriteLog("Time: " & Now.ToShortTimeString & " | User: " & _mailFrom & " | Subject: " & _mailSubject & " | Email sent successfully to: " & _mailTo)
            Me.MailError = "Message:" & _mailSubject & " Logged/Sent"
        Catch md As Mandrill.Utilities.MandrillException
            MailError = md.Message.ToString() & msg.To.ToString()
            WriteLog("Time: " & Now.ToShortTimeString & " | User: " & _mailFrom & " | Subject: " & _mailSubject & " | Email Error: " & md.Message.ToString())
        Catch ex As Exception
            WriteLog("Time: " & Now.ToShortTimeString & " | User: " & _mailFrom & " | Subject: " & _mailSubject & " | Email Error: " & ex.Message.ToString())
            MailError = ex.Message.ToString
            Throw ex
        End Try

        'With oMailer.Message
        '    .To.AsString = _mailTo
        '    .From.AsString = _mailFrom
        '    .Cc.AsString = _mailCC
        '    .Bcc.AsString = _mailBCC
        '    .Subject = _mailSubject

        '    If Me.mailHTML = 1 Then
        '        .BodyHtmlText = SetBody
        '    Else
        '        .BodyPlainText = SetBody
        '    End If

        '    If Me.mailAttachments <> "" Then
        '        If InStr(1, Me.mailAttachments, "|", 1) >= 1 Then
        '            attFiles = Split(Me.mailAttachments, "|")

        '            For i = 0 To UBound(attFiles)
        '                If Trim(attFiles(i)) <> "" Then
        '                    .Attachments.Add(Trim(attFiles(i)))
        '                End If
        '            Next
        '        Else
        '            .Attachments.Add(Me.mailAttachments)
        '        End If
        '    End If

        '    If Me.UrgentMessage Then
        '        .Importance = MailPriority.Highest
        '        .Priority = MailPriority.Highest
        '    End If
        'End With
        'Dim sendmsgreq As New Mandrill.Requests.Messages.SendMessageRequest(msg)
        'mandrill_api.SendMessage(sendmsgreq)






        'MailBee.Global.LicenseKey = MailBeeLicense
        'Dim oMailer As New MailBee.SmtpMail.Smtp
        'Dim mMsg As New MailBee.Mime.MailMessage

        'Dim attFiles() As String
        'Dim SetBody As String = ""
        'Dim dPrivateKey As String = "MIIBNgIBAAJA/u4mFcjXc4b224XAKpB/4w66Npc1JFU6ZggufmCltjSpwGj7Kmr5eIF58IwOt9ryJkIz9Iv7oMN+NkigQZf1rQIDAQABAkBTFG2Y/+EnJz7f/9DGjlz/NFd/XvvUoWnswpHUYc7fd9ev0C7PgBSjNpl0QN3v0shiY9XeA5laZ9(+kUm32EebBAiD / ToduBqeDPxqTj45EkKud)zL2/b+7xLVopcBoYBBVRKQIg/59bqJ2gv5BbyvHBkaqFOD13mlkcMohM8oiMVRPS/OUCIDx8t3tq2i8vOTWysksuV2qYgnAjreG4E/9zTstX1FEZAiC6PdwlRPtqtv36JhwXSnsXl8k9frFJq/8MMiq0jeYKdQIgRROmjDIziV2uUJApVJppGrs0riNjEJAyzLYhhaOJLxc="

        'Dim i As Integer

        ''Build Mail Text
        'If Me.mailHTML = 1 Then
        '    SetBody = BuildHTMLEmail()
        'Else
        '    SetBody = Me.mailBody
        'End If

        ''Live Configuration
        ''Set user credentials for the SMTP authentication
        'oMailer.SmtpServers.Add(MailServer, MailUser, MailPassword)

        'With oMailer.Message
        '    .To.AsString = _mailTo
        '    .From.AsString = _mailFrom
        '    .Cc.AsString = _mailCC
        '    .Bcc.AsString = _mailBCC
        '    .Subject = _mailSubject

        '    If Me.mailHTML = 1 Then
        '        .BodyHtmlText = SetBody
        '    Else
        '        .BodyPlainText = SetBody
        '    End If

        '    If Me.mailAttachments <> "" Then
        '        If InStr(1, Me.mailAttachments, "|", 1) >= 1 Then
        '            attFiles = Split(Me.mailAttachments, "|")

        '            For i = 0 To UBound(attFiles)
        '                If Trim(attFiles(i)) <> "" Then
        '                    .Attachments.Add(Trim(attFiles(i)))
        '                End If
        '            Next
        '        Else
        '            .Attachments.Add(Me.mailAttachments)
        '        End If
        '    End If

        '    If Me.UrgentMessage Then
        '        .Importance = MailPriority.Highest
        '        .Priority = MailPriority.Highest
        '    End If
        'End With

        ''Try
        ''    ' Sign the message and assign the signed message for sending.
        ''    Dim dk As New DomainKeys
        ''    oMailer.Message = dk.Sign(mMsg, Nothing, dPrivateKey, False, "nkey")
        ''Catch ex As MailBeeInvalidArgumentException
        ''    'Do nothing
        ''End Try

        'Try
        '    'Save Message Object
        '    Me.MyMessageObject = oMailer.Message

        '    If _useQueue Then
        '        oMailer.SubmitToPickupFolder(msgQueue, False)
        '    Else
        '        oMailer.Send()
        '    End If

        '    WriteLog("Time: " & Now.ToShortTimeString & " | User: " & _mailFrom & " | Subject: " & _mailSubject & " | Email sent successfully to: " & _mailTo)
        '    Me.ErrorCode = oMailer.LastResult
        '    Me.MailError = "Message:" & _mailSubject & " Logged/Sent"

        'Catch mb As MailBeeException
        '    Me.ErrorCode = oMailer.LastResult
        '    MailError = mb.Message.ToString()
        '    WriteLog("Time: " & Now.ToShortTimeString & " | User: " & _mailFrom & " | Subject: " & _mailSubject & " | Email Error: " & mb.Message.ToString())
        'Catch ex As Exception
        '    WriteLog("Time: " & Now.ToShortTimeString & " | User: " & _mailFrom & " | Subject: " & _mailSubject & " | Email Error: " & ex.Message.ToString())
        '    MailError = ex.Message.ToString
        '    Me.ErrorCode = 99
        '    Throw ex
        'End Try
    End Sub

    Private Function BuildHTMLEmail() As String
        Dim sHTML As New System.Text.StringBuilder
        sHTML.Append("<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01 Transitional//EN"" ""http://www.w3.org/TR/html4/loose.dtd"">")
        sHTML.Append("< Html > ")
        sHTML.Append(" < head > ")
        sHTML.Append(" < meta http-equiv=""Content-Type""")
        sHTML.Append(" content=""text/html; charset=iso-8859-1"">")
        sHTML.Append("<link type='text/css' rel='stylesheet' href='" & webURL & _MailStyle & "'/>")
        sHTML.Append("<style>")
        sHTML.Append(".pbreak { page-break-after:always; } ")
        sHTML.Append("@page {margin-left: 0.25in; margin-right: 0.25in;}")
        sHTML.Append("</style>")
        sHTML.Append("</head>")
        sHTML.Append("<body bgcolor=""#FFFFFF"">")
        sHTML.Append("<table width='100%'>")

        'Print Header
        If Me.PrintHeader Then
            sHTML.Append("<tr><td bgcolor=""#FFFFFF""><img src='" & webURL & Me.MailLogo & "'></td></tr>")
        End If

        sHTML.Append("<tr><td><font size=""5"" face=""Arial""><strong>" & _mailSubject & "</strong></font><hr></td></tr>")
        sHTML.Append("<tr><td valign=top>" & Me.HTMLBodyText & "</td></tr>")
        sHTML.Append("</table>")

        'Add Footer --------------------------------------
        'sHTML.Append("<div style=""padding:4px; text-align:right; background-color:white; width:100%; display:block""><a href='" & webURL & "'><img src='" & webURL & "images/poweredBy.gif' border=0></a><br></div>")

        'Legal ------------------------------------------
        If Me.PrintLegal Then
            sHTML.Append(GetLegal(Me.FooterFile))
        End If
        ' ------------------------------------------------
        sHTML.Append("</body>")
        sHTML.Append("</html>")

        Return sHTML.ToString
    End Function

    '----------------------------------------------------------------------
    '	Function: WriteLog
    '   This Sub writes a log file when sending email
    '	use this when sending emails.
    '----------------------------------------------------------------------
    Sub WriteLog(ByVal log_text As String)
        Dim LogFile As String = MailLog & "\mail\" & LogDate(Date.Today.ToShortDateString, "") & ".txt"
        Dim gLogFile As StreamWriter = Nothing

        SyncLock Me
            Try
                If File.Exists(LogFile) Then
                    gLogFile = File.AppendText(LogFile)
                Else
                    gLogFile = File.CreateText(LogFile)
                End If

                gLogFile.WriteLine(log_text)
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
    '	Function: GetLegal
    '   This function gets the legal text at bottom of email
    '	use this when sending emails.
    '----------------------------------------------------------------------
    Private Function GetLegal(ByVal legal_file As String) As String
        Dim LogFile As String = MailLog & "\legal\" & legal_file
        Dim gLogFile As New StreamReader(LogFile)
        Dim tx As String

        Try
            If File.Exists(LogFile) Then
                tx = gLogFile.ReadToEnd
            Else
                tx = ""
            End If
            gLogFile.Close()
            Return tx

        Catch e As Exception
            Throw New Exception("Cannot open legal file: " & legal_file & " - " & e.Message)
            gLogFile.Close()
        End Try
    End Function

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
