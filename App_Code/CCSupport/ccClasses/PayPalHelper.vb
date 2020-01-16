Imports Microsoft.VisualBasic
Imports System
Imports System.IO
Imports System.Text
Imports System.Web

Imports NVMS.InternetTools

Namespace PayPalIntegration
    ' <summary>
    ' Summary description for PayPalUrl.
    ' </summary>
    Public Class PayPalHelper
        Public LogoUrl As String = ""
        Public AccountEmail As String = ""
        Public BuyerEmail As String = ""
        Public SuccessUrl As String = ""
        Public CancelUrl As String = ""
        Public ItemName As String = ""
        Public Amount As Decimal = 0D
        Public InvoiceNo As String = ""


        Public PayPalBaseUrl As String = "https://www.paypal.com/cgi-bin/webscr?"
        '"https://www.paypal.com/cgi-bin/webscr?";
        '"https://www.sandbox.paypal.com/us/cgi-bin/webscr?";


        Public LastResponse As String = ""

        Public Sub New()

        End Sub

        Public Function GetSubmitUrl() As String
            Dim url As StringBuilder = New StringBuilder()

            url.Append(Me.PayPalBaseUrl & "cmd=_xclick&business=" & HttpUtility.UrlEncode(AccountEmail))

            If Not BuyerEmail Is Nothing AndAlso BuyerEmail <> "" Then
                url.AppendFormat("&email={0}", HttpUtility.UrlEncode(BuyerEmail))
            End If

            If Amount <> 0D Then
                url.AppendFormat("&amount={0:f2}", Amount)
            End If

            If Not LogoUrl Is Nothing AndAlso LogoUrl <> "" Then
                url.AppendFormat("&image_url={0}", HttpUtility.UrlEncode(LogoUrl))
            End If

            If Not ItemName Is Nothing AndAlso ItemName <> "" Then
                url.AppendFormat("&item_name={0}", HttpUtility.UrlEncode(ItemName))
            End If

            If Not InvoiceNo Is Nothing AndAlso InvoiceNo <> "" Then
                url.AppendFormat("&invoice={0}", HttpUtility.UrlEncode(InvoiceNo))
            End If

            If Not SuccessUrl Is Nothing AndAlso SuccessUrl <> "" Then
                url.AppendFormat("&return={0}", HttpUtility.UrlEncode(SuccessUrl))
            End If

            If Not CancelUrl Is Nothing AndAlso CancelUrl <> "" Then
                url.AppendFormat("&cancel_return={0}", HttpUtility.UrlEncode(CancelUrl))
            End If

            Return url.ToString()
        End Function

        ' <summary>
        ' Posts all form variables received back to PayPal. This method is used on 
        ' is used for Payment verification from the 
        ' </summary>
        ' <returns>Empty string on success otherwise the full message from the server</returns>
        Public Function IPNPostDataToPayPal(ByVal PayPalUrl As String, ByVal PayPalEmail As String) As Boolean
            Dim Request As HttpRequest = HttpContext.Current.Request
            Me.LastResponse = ""

            ' *** Make sure our payment goes back to our own account
            Dim Email As String = Request.Form("receiver_email")
            If Email Is Nothing OrElse Email.Trim().ToLower() <> PayPalEmail.ToLower() Then
                Me.LastResponse = "Invalid receiver email"
                Return False
            End If

            Dim Http As wwHttp = New wwHttp()
            Http.AddPostKey("cmd", "_notify-validate")

            For Each postKey As String In Request.Form
                Http.AddPostKey(postKey, Request.Form(postKey))
            Next postKey

            ' *** Retrieve the HTTP result to a string
            Me.LastResponse = Http.GetUrl(PayPalUrl)

            If Me.LastResponse = "VERIFIED" Then
                Return True
            End If

            Return False
        End Function

    End Class
End Namespace
