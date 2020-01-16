﻿Imports System.Data
Imports System.Data.Common
Imports Microsoft.Practices.EnterpriseLibrary.Data
Imports System.IO

Partial Class MyAccount
    Inherits System.Web.UI.Page

    Public Shared factory As DatabaseProviderFactory = New DatabaseProviderFactory()
    Public Shared db As Database = factory.CreateDefault()
    Private SecureApp As String = ConfigurationManager.AppSettings("AppSecure")

    ''' <summary>
    '''     The generic ILog logger
    ''' </summary>
    Public Shared Logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger()

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        AjaxPro.Utility.RegisterTypeForAjax(GetType(MyAccount))
        AjaxPro.Utility.RegisterTypeForAjax(GetType(GlobalAjax))

        Dim mySession As New SessionManager
        Dim CustomerID As String = mySession.GetElement("CustomerID", "number")
        Dim Username As String = mySession.GetElement("username")
        Dim myName As String = mySession.GetElement("FullName")
        Dim MyEmail As String = mySession.GetElement("Email")
        Dim Access As String = mySession.GetElement("AccessLevel")

        'Write Client Values For AJAX Methods *******************************************
        Dim ValArray As New StringDictionary
        ValArray.Add("CustomerID", CustomerID)
        ValArray.Add("Username", Username)
        ValArray.Add("myName", myName)
        ValArray.Add("MyEmail", MyEmail)
        ValArray.Add("access", Access)
        ValArray.Add("SessionID", Session.SessionID)
        WriteClientVals(ValArray)

        Dim q As String = Left(Request.QueryString("q"), 20)
        Dim go As String = Left(Request.QueryString("goto"), 20)

        Dim Referring As String = ""

        If Not Request.UrlReferrer Is Nothing Then
            Referring = Path.GetFileName(Request.UrlReferrer.PathAndQuery.ToString())
        End If

        If go <> "" Then
            Session("GoToPage") = go
        Else
            Session("GoToPage") = Referring
        End If

        If (Request.ServerVariables("HTTPS") = "off") Then
            If ConfigurationManager.AppSettings("ForceSecure") = "True" Then
                Response.Redirect(SecureApp & "MyAccount.aspx")
            End If
        End If

        If Session("LoggedIn") Then
            NewForm.Visible = False
            AccountUpdate.Visible = True
            ShipHistoryList.InnerHtml = GlobalAjax.ShippingListGet(CustomerID)
            OrderHistoryList.InnerHtml = GlobalAjax.CustomerOrderHistory(CustomerID)
        Else
            AccountUpdate.Visible = False
            MyPageKey.Text = GlobalAjax.RandomImageShow(5) '& "<br><b>Session:" & Session("ImageBlock") & "</b>"
        End If

    End Sub

    Private Sub WriteClientVals(ByVal ValueArray As StringDictionary)
        Dim sb As New StringBuilder
        Dim i As DictionaryEntry

        For Each i In ValueArray
            sb.Append("var " & i.Key & " = '" & i.Value & "';" & vbCrLf)
        Next

        Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "DashClientVals", sb.ToString, True)
    End Sub

    <AjaxPro.AjaxMethod()>
    Function ProcessLogin(ByVal username As String, ByVal Password As String) As String

        Dim sqlCommand As String = "VerifyLoginCustomer"
        Dim dbCommand As DbCommand = db.GetStoredProcCommand(sqlCommand)
        Dim i As Integer = 0
        Dim RetStr As String = ""
        Dim pFlag As Boolean = False
        Dim LoadMyPage As String = ""

        ' Add Parameters to SPROC
        db.AddInParameter(dbCommand, "username", DbType.String, Common.SQLEncode(username))
        db.AddInParameter(dbCommand, "password", DbType.String, Common.SQLEncode(Password))
        db.AddInParameter(dbCommand, "ip", DbType.String, Context.Request.UserHostAddress())
        db.AddInParameter(dbCommand, "session", DbType.String, Context.Session.SessionID.ToString())

        Try
            Using rs As IDataReader = db.ExecuteReader(dbCommand)

                While rs.Read()
                    'If active User Record Found ////////////////////////
                    If rs("active") Then

                        HttpContext.Current.Session("LoggedIn") = True
                        HttpContext.Current.Session("userID") = rs("customerID")
                        LoadMyPage = HttpContext.Current.Session("GoToPage")

                        'Create XML Session Manager /////////////////////
                        Dim mySession As New SessionManager
                        mySession.AddElement("userID", rs("customerID"))
                        mySession.AddElement("CustomerID", rs("customerID"))
                        mySession.AddElement("FullName", rs("first_name").ToString & " " & rs("last_name").ToString)
                        mySession.AddElement("username", rs("email").ToString)
                        mySession.AddElement("Email", rs("email").ToString)
                        mySession.AddElement("AccessLevel", "CUSTOMER")

                        RetStr = "0|" & LoadMyPage

                    Else
                        RetStr = "1|Your account was found but is no longer active... Please contact the system administrator for more info."

                    End If
                    i += 1
                End While

                If i = 0 Then
                    RetStr = "1|No user found with those details... Please try again."
                End If
            End Using

        Catch ex As Exception
            Logger.Error(ex, "MyAccount Functions: " & Reflection.MethodBase.GetCurrentMethod.Name & " - " & ex.Message.ToString())
            RetStr = "1|Login Error: " & ex.Message.ToString()
        End Try

        Return RetStr
    End Function

    <AjaxPro.AjaxMethod()>
    Function ForgotLogin(ByVal Email As String) As String
        Dim RetStr As String = ""
        Dim i As Integer = 0

        If Not Common.ValidEmail(Email) Then
            RetStr = "1|This email is not valid... Please check and try again."
        ElseIf Email = "" Then
            RetStr = "1|Please enter your email address first..."
        Else
            'Check email

            Dim sqlCommand As String = "VerifyLoginEmail"
            Dim dbCommand As DbCommand = db.GetStoredProcCommand(sqlCommand)

            Dim mailString As String = ""

            ' Add Parameters to SPROC
            db.AddInParameter(dbCommand, "Email", DbType.String, Email)

            Try
                Using rs As IDataReader = db.ExecuteReader(dbCommand)
                    While rs.Read()
                        'Send Email with details
                        mailString += "Following are your login details for piegourmet.com...<br>"

                        mailString += "<b>Username/Email: <span class='error'>" & rs("email").ToString() & "</span></b><br>"
                        mailString += "<b>Password: <span class='error'>" & rs("password").ToString() & "</span></b><br><br>"
                        mailString += "<b>Please delete this email or put it in a safe place when you have retrieved your login details.</b>"

                        Try
                            Dim myMail As New Sendmail
                            ' myMail.mailFrom = "info@nvms.com"
                            myMail.mailTo = rs("email").ToString()
                            myMail.mailSubject = "Pie Gourmet User Details..."
                            myMail.HTMLBodyText = mailString
                            myMail.useQueue = True
                            myMail.SendEmail()
                            RetStr = "0|Mail Sent Successfully... Please check your inbox for your login details."
                        Catch ex As Exception
                            RetStr = "1|Password Retrieval Error: " & ex.Message.ToString
                        End Try
                        i += 1
                    End While

                    If i = 0 Then
                        RetStr = "1|No user found with that email..."
                    End If
                End Using
            Catch ex As Exception
                Logger.Error(ex, "MyAccount Functions: " & Reflection.MethodBase.GetCurrentMethod.Name & " - " & ex.Message.ToString())
                RetStr = "1|Password Retrieval Error: " & ex.Message.ToString
            End Try
        End If

        Return RetStr
    End Function



End Class
