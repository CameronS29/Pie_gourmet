Imports System
Imports System.Web
Imports NVPAPICaller

Partial Class ReceivePayPal
    Inherits System.Web.UI.Page
    Private LogFolder As String = ConfigurationManager.AppSettings("LogFolder")

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
       
        Dim test As New NVPAPICaller()

        Dim retMsg As String = ""
        Dim token As String = ""

        If Session("payment_amt") IsNot Nothing Then
            Dim amt As String = Session("payment_amt").ToString()

            Dim ret As Boolean = test.ShortcutExpressCheckout(amt, token, retMsg)

            TempFileCache.WriteLog(LogFolder & "\PayPalLog.txt", Date.Now() & vbCrLf & "Session Token: " & Session("token") & " Return: " & ret.ToString & vbCrLf)

            If ret Then
                Session("token") = token
                Response.Redirect(retMsg)
            Else
                Response.Redirect("APIError.aspx?" & retMsg)
            End If
        Else
            Response.Redirect("APIError.aspx?ErrorCode=AmtMissing")
        End If
    End Sub
End Class
