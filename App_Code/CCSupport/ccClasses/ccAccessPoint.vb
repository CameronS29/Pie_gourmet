Imports Microsoft.VisualBasic
Imports System

Imports System.Text.RegularExpressions
Imports System.Globalization

Imports NVMS.InternetTools


Namespace NVMS.WebStore
    Public Class ccAccessPoint
        Inherits ccProcessing

        Public Sub New()
            Me.HttpLink = "https://secure1.merhcantmanager.com/ccgateway.asp"
        End Sub

        Public Overrides Function ValidateCard() As Boolean
            If (Not MyBase.ValidateCard()) Then
                Return False
            End If

            If Me.OrderAmount < 0 Then
                Me.ValidatedResult = "FAILED"
                Me.ValidatedMessage = "Credits are not supported for AccessPoint's API."
                Me.SetError(Me.ValidatedMessage)
                Me.LogTransaction()
                Return False
            End If

            If Me.Http Is Nothing Then
                Me.Http = New wwHttp()
                Me.Http.Timeout = Me.Timeout
            End If

            Dim CardNo As String = Regex.Replace(Me.CreditCardNumber, "[ -/._#]", "")

            Me.Http.AddPostKey("REQUESTTYPE", "APPROVALONLY")

            If Me.ProcessType = ccProcessTypes.Sale Then
                Me.Http.AddPostKey("TRANSTYPE", "SALE")
            ElseIf Me.ProcessType = ccProcessTypes.PreAuth Then
                Me.Http.AddPostKey("TRANSTYPE", "PREAUTH")
            End If

            Me.Http.AddPostKey("MERCHANTID", Me.MerchantId)
            Me.Http.AddPostKey("AMOUNT", Me.OrderAmount.ToString(CultureInfo.InstalledUICulture.NumberFormat))
            Me.Http.AddPostKey("BNAME", Me.Name)
            Me.Http.AddPostKey("BCOMPANY", Me.Company)
            Me.Http.AddPostKey("CCNUMBER", CardNo)
            If Me.SecurityCode.Trim() <> "" Then
                Me.Http.AddPostKey("CVV2", Me.SecurityCode.Trim())
            End If

            Me.Http.AddPostKey("EXPMO", Me.CreditCardExpirationMonth.ToString())
            Me.Http.AddPostKey("EXPYE", Me.CreditCardExpirationYear.ToString())
            Me.Http.AddPostKey("BADDRESS1", Me.Address)
            Me.Http.AddPostKey("BCITY", Me.City)
            Me.Http.AddPostKey("BSTATE", Me.State)
            Me.Http.AddPostKey("BZIPCODE", Me.Zip)
            Me.Http.AddPostKey("BCOUNTRY", Me.Country)
            Me.Http.AddPostKey("BEMAIL", Me.Email)
            Me.Http.AddPostKey("BPHONE", Me.Phone)
            Me.Http.AddPostKey("INVOICENO", Me.OrderId)

            If Me.TaxAmount > 0D Then
                Me.Http.AddPostKey("TAX", Me.TaxAmount.ToString(CultureInfo.InstalledUICulture.NumberFormat))
            End If

            Me.Http.CreateWebRequestObject(Me.HttpLink)
            Me.Http.WebRequest.Referer = Me.ReferringUrl

            Me.RawProcessorResult = Me.Http.GetUrl(Me.HttpLink)
            If Me.Http.Error Then
                Me.ValidatedResult = "FAILED"
                Me.ValidatedMessage = Me.Http.ErrorMessage
                Me.SetError(Me.Http.ErrorMessage)
                Me.LogTransaction()
                Return False
            End If

            Dim Approved As String = wwHttpUtils.GetUrlEncodedKey(Me.RawProcessorResult, "APPROVED")

            If Approved <> "Y" Then
                Me.ValidatedResult = "DECLINED"
                Me.ValidatedMessage = wwHttpUtils.GetUrlEncodedKey(Me.RawProcessorResult, "MSG").Replace("&", "")
                If Me.ValidatedMessage = "" Then
                    Me.ValidatedMessage = Me.RawProcessorResult
                End If
                Me.SetError(Me.ValidatedMessage)
                Me.LogTransaction()
                Return False
            End If

            Me.ValidatedResult = "APPROVED"

            ' *** Make the RawProcessorResult more readable for client application
            Me.ValidatedMessage = wwHttpUtils.UrlDecode(Me.RawProcessorResult)

            Me.LogTransaction()
            Return True
        End Function

    End Class

End Namespace
