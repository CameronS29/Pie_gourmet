Imports Microsoft.VisualBasic
Imports System

Imports System.Globalization
Imports NVMS.InternetTools
Imports System.Text.RegularExpressions
Imports System.Security.Cryptography
Imports NVMS.Tools
Imports System.Text

Namespace NVMS.WebStore

    ' <summary>
    ' This class provides Credit Card processing against the EC Linx
    ' Gateway.
    ' </summary>
    Public Class ccECLinx
        Inherits ccProcessing

        Public Sub New()
            'Test
            'Me.HttpLink = "https://services.pwsdemo.com/secure/external/ExternalAuthQS.asp"

            'Live
            Me.HttpLink = "https://services.paymentworksuite.com/secure/external/ExternalAuthQS.asp"
        End Sub

        Public DrCr As String = "D"

        ' <summary>
        ' Validates the actual card EC Linx using the HTTP interface.
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

            'Login
            Me.Http.AddPostKey("MerchId", Me.MerchantId)
            Me.Http.AddPostKey("UserId", Me.MerchantUserName)
            Me.Http.AddPostKey("Pwd", Me.MerchantPassword)

            Dim cOrderAmount As String = "0.00"
            Dim AuthType As String = "B"

            ' *** Order Amount
            If Me.OrderAmount >= 0 Then
                If Me.ProcessType = ccProcessTypes.Sale Then
                    AuthType = "B"
                ElseIf Me.ProcessType = ccProcessTypes.PreAuth Then
                    AuthType = "A"
                ElseIf Me.ProcessType = ccProcessTypes.Credit Then
                    AuthType = "C"
                    DrCr = "C"
                End If

                cOrderAmount = Me.OrderAmount.ToString(CultureInfo.InvariantCulture.NumberFormat)
            Else
                AuthType = "C"
                cOrderAmount = (Me.OrderAmount * -1).ToString(CultureInfo.InvariantCulture.NumberFormat)
            End If

            Me.Http.AddPostKey("AuthType", AuthType)
            Me.Http.AddPostKey("TotalAmt", cOrderAmount.Replace(".", ""))
            Me.Http.AddPostKey("NumberOfItems", 1)
            Me.Http.AddPostKey("DbCr", DrCr)
            Me.Http.AddPostKey("TranNum", Me.OrderId)

            ' *** Optional Tax Amount 
            If Me.TaxAmount > 0D Then
                Me.Http.AddPostKey("TaxAmt", Me.TaxAmount.ToString(CultureInfo.InvariantCulture.NumberFormat))
            End If

            ' *** Credit Card Info
            Dim CardNo As String = Regex.Replace(Me.CreditCardNumber, "[ -/._#]", "")
            Me.Http.AddPostKey("CreditCardNo", CardNo)
            Me.Http.AddPostKey("ExpireMM", Me.CreditCardExpirationMonth)
            Me.Http.AddPostKey("ExpireYY", Me.CreditCardExpirationYear)
            Me.Http.AddPostKey("CardPresent", 0)
            Me.Http.AddPostKey("POSEntry", 0)

            If Not Me.SecurityCode Is Nothing AndAlso Me.SecurityCode <> "" Then
                Me.Http.AddPostKey("CVV", Me.SecurityCode)
            End If

            Me.Http.AddPostKey("NameOnCard", Me.Firstname.TrimEnd() & " " & Me.Lastname.Trim())
            Me.Http.AddPostKey("ShipToStreet", Me.Address)
            Me.Http.AddPostKey("ShipToZip", Me.Zip)
            Me.Http.AddPostKey("InvoiceNo", Me.OrderId)
            Me.Http.AddPostKey("CustCode", Me.PONumber)

            ' *** Item Quantity (Required For Level 3)
            Me.Http.AddPostKey("ItemQty1", 1)
            Me.Http.AddPostKey("ItemDesc1", "Weekly NVMS Inspection Billing")
            Me.Http.AddPostKey("ItemAmt1", cOrderAmount.Replace(".", ""))
            Me.Http.AddPostKey("ItemUOM1", "EA")
            Me.Http.AddPostKey("ItemPartNo1", "Inspection Service")
            Me.Http.AddPostKey("ItemCommCode1", "78141600")

            If Not Me.Email Is Nothing AndAlso Me.Email <> "" Then
                Me.Http.AddPostKey("MailTo1", Me.Email)
            End If

            Me.Http.AddPostKey("MailFrom", "accounts@nvms.com")
            Me.Http.AddPostKey("ExtraInfo", Me.Comment)


            Me.Http.CreateWebRequestObject(Me.HttpLink)

            ' *** Blue pay returns 302 Moved header - with result in querystring
            Me.RawProcessorResult = Me.Http.GetUrl(Me.HttpLink)

            ' *** Http Error of some sort
            If Me.Http.Error Then
                Me.ValidatedResult = "FAILED"
                Me.ValidatedMessage = Me.Http.ErrorMessage
                Me.SetError(Me.Http.ErrorMessage)
                Me.LogTransactionRaw()
                Return False
            End If

            Dim AuthResult As String = Me.RawProcessorResult
            AuthResult = wwUtils.ExtractString(AuthResult, "?", Constants.vbCr, False, True)

            ' *** can't parse result - error
            If AuthResult = "" Then
                Me.ValidatedResult = "FAILED"
                Me.ValidatedMessage = "Invalid Result returned"
                Me.SetError("Invalid Result returned.")
                Return False
            End If

            ' *** Re-Assign just the raw query string instead of the HTTP content that contains it
            'Me.RawProcessorRequest = AuthResult
            Me.ValidatedResult = wwHttpUtils.GetUrlEncodedKey(AuthResult, "Status")
            Me.ValidatedMessage = wwHttpUtils.GetUrlEncodedKey(AuthResult, "AuthMsg")
            Me.AvsResultCode = wwHttpUtils.GetUrlEncodedKey(AuthResult, "ReturnCode")
            Me.TransactionId = wwHttpUtils.GetUrlEncodedKey(AuthResult, "TranNum")

            If Me.ValidatedResult = "A" Then
                Me.ValidatedMessage = "Transaction has been approved"
                Me.SetError(Nothing) ' Clear any errors
                Me.AuthorizationCode = wwHttpUtils.GetUrlEncodedKey(AuthResult, "AuthCode")
                Me.Error = True
            ElseIf Me.ValidatedResult = "F" Then
                Me.ValidatedResult = "FAILED"
                Me.SetError(Me.ValidatedMessage)
                Me.Error = False
            ElseIf Me.ValidatedResult = "D" Then
                Me.SetError(Me.ValidatedMessage)
                Me.Error = False
            Else
                Me.SetError(Me.ValidatedMessage)
                Me.Error = False
            End If

            Me.LogTransactionRaw()

            Return Me.Error
        End Function

        'This is used to convert a byte array to a hex string
        Private Function ByteArrayToBinHex(ByVal arrInput As Byte()) As String
            Dim i As Integer
            Dim sOutput As StringBuilder = New StringBuilder(arrInput.Length)
            For i = 0 To arrInput.Length - 1
                sOutput.Append(arrInput(i).ToString("X2"))
            Next i
            Return sOutput.ToString()
        End Function
    End Class

End Namespace
