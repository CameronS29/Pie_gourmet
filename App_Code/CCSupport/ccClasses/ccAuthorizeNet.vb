Imports Microsoft.VisualBasic
Imports System
Imports System.Text.RegularExpressions
Imports System.Globalization
Imports NVMS.InternetTools

Namespace NVMS.WebStore

    ' <summary>
    ' This class provides Credit Card processing against 
    ' the Authorize.Net Gateway.
    ' </summary>
    Public Class ccAuthorizeNet
        Inherits ccProcessing

        Public Sub New()
            'Me.HttpLink = "https://secure.authorize.net/gateway/transact.dll"
            Me.HttpLink = App.Configuration.CCHostUrl
        End Sub

        ' <summary>
        ' Validates the actual card against Authorize.Net Gateway using the HTTP 
        ' interface.
        ' <seealso>Class ccAuthorizeNet</seealso>
        ' </summary>
        ' <param name=""></param>
        ' <returns>Boolean</returns>
        Public Overrides Function ValidateCard() As Boolean
            If (Not MyBase.ValidateCard()) Then
                Return False
            End If

            If Me.Http Is Nothing Then
                Me.Http = New wwHttp()
                Me.Http.Timeout = Me.Timeout
            End If

            Dim CardNo As String = Regex.Replace(Me.CreditCardNumber, "[ -/._#]", "")

            Me.Http.AddPostKey("x_version", "3.1")
            Me.Http.AddPostKey("x_method", "CC")
            Me.Http.AddPostKey("x_delim_data", "true")
            Me.Http.AddPostKey("x_delim_char", "|")
            Me.Http.AddPostKey("x_tran_key", Me.MerchantPassword)
            Me.Http.AddPostKey("x_login", Me.MerchantId)

            If Me.UseTestTransaction Then
                Me.Http.AddPostKey("x_test_request", "true")
            Else
                Me.Http.AddPostKey("x_test_request", "false")
            End If

            If Me.OrderAmount >= 0 Then
                If Me.ProcessType = ccProcessTypes.Sale Then
                    Me.Http.AddPostKey("x_type", "AUTH_CAPTURE")
                ElseIf Me.ProcessType = ccProcessTypes.PreAuth Then
                    Me.Http.AddPostKey("x_type", "AUTH_ONLY")
                ElseIf Me.ProcessType = ccProcessTypes.AuthCapture Then
                    Me.Http.AddPostKey("x_type", "PRIOR_AUTH_CAPTURE")
                    Me.Http.AddPostKey("x_trans_ID", Me.TransactionId)
                ElseIf Me.ProcessType = ccProcessTypes.Credit Then
                    Me.Http.AddPostKey("x_type", "CREDIT")
                End If

                Me.Http.AddPostKey("x_amount", Me.OrderAmount.ToString(CultureInfo.InvariantCulture.NumberFormat))
            Else
                Me.Http.AddPostKey("x_type", "CREDIT")
                Me.Http.AddPostKey("x_amount", (CInt(Fix(-1 * Me.OrderAmount))).ToString(CultureInfo.InvariantCulture.NumberFormat))
            End If

            If Me.TaxAmount > 0 Then
                Me.Http.AddPostKey("x_tax", Me.TaxAmount.ToString(CultureInfo.InvariantCulture.NumberFormat))
            End If

            Me.Http.AddPostKey("x_card_num", CardNo)
            Me.Http.AddPostKey("x_exp_date", Me.CreditCardExpirationMonth.ToString() & "-" & Me.CreditCardExpirationYear.ToString())

            If Me.SecurityCode.Trim() <> "" Then
                Me.Http.AddPostKey("x_card_code", Me.SecurityCode.Trim())
            End If

            Me.Http.AddPostKey("x_cust_id", Me.CustomerID)
            Me.Http.AddPostKey("x_first_name", Me.Firstname)
            Me.Http.AddPostKey("x_last_name", Me.Lastname)
            Me.Http.AddPostKey("x_company", Me.Company)
            Me.Http.AddPostKey("x_address", Me.Address)
            Me.Http.AddPostKey("x_city", Me.City)
            Me.Http.AddPostKey("x_state", Me.State)
            Me.Http.AddPostKey("x_zip", Me.Zip)
            Me.Http.AddPostKey("x_country", Me.Country)

            If Me.Phone.Trim() <> "" Then
                Me.Http.AddPostKey("x_phone", Me.Phone)
            End If

            Me.Http.AddPostKey("x_email", Me.Email)

            Me.Http.AddPostKey("x_invoice_num", Me.OrderId)
            Me.Http.AddPostKey("x_description", Me.Comment)

            Me.Http.CreateWebRequestObject(Me.HttpLink)
            Me.Http.WebRequest.Referer = Me.ReferringUrl

            Me.RawProcessorResult = Me.Http.GetUrl(Me.HttpLink)

            'Log Result
            LogTransactionRaw()

            If Me.Http.Error Then
                Me.ValidatedResult = "FAILED"
                Me.ValidatedMessage = Me.Http.ErrorMessage
                Me.SetError(Me.Http.ErrorMessage)
                Me.LogTransaction()
                Return False
            End If

            'Get Credit Card Validation Results
            Dim Result As String() = Me.RawProcessorResult.Split("|")
            If Result Is Nothing Then
                Me.ValidatedResult = "FAILED"
                Me.ValidatedMessage = "Invalid response received from Merchant Server"
                Me.SetError(Me.ValidatedMessage)
                Me.LogTransaction()
                Return False
            End If

            ' *** REMEMBER: Result Codes are in 0 based array!
            Me.TransactionId = Result(6)
            Me.AvsResultCode = Result(5)

            If CInt(Result(0)) = 3 Then ' Error - Processor communications usually
                ' *** Consider an invalid Card a DECLINE
                ' *** so we can send back to the customer for display
                If Result(3).IndexOf("credit card number is invalid") > -1 Then
                    Me.ValidatedResult = "DECLINED"
                Else
                    Me.ValidatedResult = "FAILED"
                End If

                Me.ValidatedMessage = Result(3)
                Me.SetError(Me.ValidatedMessage)
                Me.Error = False

            ElseIf CInt(Result(0)) = 2 Or CInt(Result(0)) = 4 Then ' Declined
                Me.ValidatedResult = "DECLINED"
                Me.ValidatedMessage = Result(3)
                If Me.ValidatedMessage = "" Then
                    Me.ValidatedMessage = Me.RawProcessorResult
                End If
                Me.SetError(Me.ValidatedMessage)
                Me.Error = False
            Else
                Me.ValidatedResult = "APPROVED"

                ' *** Make the RawProcessorResult more readable for client application
                Me.ValidatedMessage = Result(3)
                Me.AuthorizationCode = Result(4)
                Me.SetError(Nothing)
                Me.Error = True
            End If

            Me.LogTransaction()

            Return Me.Error
        End Function

    End Class

End Namespace
