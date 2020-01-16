Imports System
Imports System.Web
Imports NVPAPICaller

Partial Class expresscheckout
    Inherits System.Web.UI.Page

    Private Sub expresscheckout_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim test As New NVPAPICaller()
        Dim retMsg As String = ""
        Dim token As String = ""

        If Session("payment_amt") IsNot Nothing Then
            Dim amt As String = Session("payment_amt").ToString()

            Dim ret As Boolean = test.ShortcutExpressCheckout(amt, token, retMsg)
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
