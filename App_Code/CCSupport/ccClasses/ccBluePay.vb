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
    ' This class provides Credit Card processing against the BluePay
    ' Gateway.
    ' </summary>
    Public Class ccBluePay
        Inherits ccProcessing

        Public Sub New()
            Me.HttpLink = "https://secure.bluepay.com/bp10emu"
        End Sub

        ' <summary>
        ' Validates the actual card against Authorize.Net Gateway using the HTTP 
        ' interface.
        ' <seealso>Class ccBluePay</seealso>
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

            Me.Http.AddPostKey("MERCHANT", Me.MerchantId)

            Dim TransactionMode As String = "LIVE"
            If Me.UseTestTransaction Then
                TransactionMode = "TEST"
            End If
            Me.Http.AddPostKey("MODE", TransactionMode)

            Dim cOrderAmount As String = "0.00"

            Dim AuthType As String = "SALE"

            ' *** Order Amount
            If Me.OrderAmount >= 0 Then
                If Me.ProcessType = ccProcessTypes.Sale Then
                    AuthType = "SALE"
                ElseIf Me.ProcessType = ccProcessTypes.PreAuth Then
                    AuthType = "AUTH"
                ElseIf Me.ProcessType = ccProcessTypes.Credit Then
                    AuthType = "REFUND"
                End If

                cOrderAmount = Me.OrderAmount.ToString(CultureInfo.InvariantCulture.NumberFormat)
            Else
                AuthType = "REFUND"
                cOrderAmount = (Me.OrderAmount * -1).ToString(CultureInfo.InvariantCulture.NumberFormat)
            End If
            Me.Http.AddPostKey("TRANSACTION_TYPE", AuthType)
            Me.Http.AddPostKey("AMOUNT", cOrderAmount)

            ' *** Optional Tax Amount 
            If Me.TaxAmount > 0D Then
                Me.Http.AddPostKey("AMOUNT_TAX", Me.TaxAmount.ToString(CultureInfo.InvariantCulture.NumberFormat))
            End If

            ' *** Credit Card Info
            Dim CardNo As String = Regex.Replace(Me.CreditCardNumber, "[ -/._#]", "")
            Me.Http.AddPostKey("CC_NUM", CardNo)
            Me.Http.AddPostKey("CC_EXPIRES", Me.CreditCardExpirationMonth & Me.CreditCardExpirationYear)

            If Not Me.SecurityCode Is Nothing AndAlso Me.SecurityCode <> "" Then
                Me.Http.AddPostKey("CVCCVV2", Me.SecurityCode)
            End If


            Me.Http.AddPostKey("NAME", Me.Firstname.TrimEnd() & " " & Me.Lastname.Trim())
            Me.Http.AddPostKey("ADDR1", Me.Address)
            Me.Http.AddPostKey("CITY", Me.City)
            Me.Http.AddPostKey("STATE", Me.State)
            Me.Http.AddPostKey("ZIPCODE", Me.Zip)


            If Not Me.Email Is Nothing AndAlso Me.Email <> "" Then
                Me.Http.AddPostKey("EMAIL", Me.Email)
            End If
            If Not Me.Phone Is Nothing AndAlso Me.Phone <> "" Then
                Me.Http.AddPostKey("PHONE", Me.Phone)
            End If

            Me.Http.AddPostKey("ORDER_ID", Me.OrderId)
            Me.Http.AddPostKey("COMMENT", Me.Comment)


            Dim SealText As String = Me.MerchantPassword & Me.MerchantId & AuthType & cOrderAmount & TransactionMode

            Dim md5 As MD5 = New MD5CryptoServiceProvider()
            Dim hash As Byte() = md5.ComputeHash(System.Text.Encoding.ASCII.GetBytes(SealText))
            Dim EncodedSealText As String = ByteArrayToBinHex(hash).ToLower()

            Me.Http.AddPostKey("TAMPER_PROOF_SEAL", EncodedSealText)

            Me.Http.CreateWebRequestObject(Me.HttpLink)

            ' *** Blue pay returns 302 Moved header - with result in querystring
            Me.Http.WebRequest.AllowAutoRedirect = False

            Me.RawProcessorResult = Me.Http.GetUrl(Me.HttpLink)

            ' *** Http Error of some sort
            If Me.Http.Error Then
                Me.ValidatedResult = "FAILED"
                Me.ValidatedMessage = Me.Http.ErrorMessage
                Me.SetError(Me.Http.ErrorMessage)
                Me.LogTransaction()
                Return False
            End If

            Dim AuthResult As String = Me.Http.WebResponse.Headers("Location")
            AuthResult = wwUtils.ExtractString(AuthResult, "?", Constants.vbCr, False, True)

            ' *** can't parse result - error
            If AuthResult = "" Then
                Me.ValidatedResult = "FAILED"
                Me.ValidatedMessage = "Invalid Result returned"
                Me.SetError("Invalid Result returned.")
                Return False
            End If

            ' *** Re-Assign just the raw query string instead of the HTTP content that contains it
            Me.RawProcessorRequest = AuthResult

            Me.ValidatedResult = wwHttpUtils.GetUrlEncodedKey(AuthResult, "RESULT")
            Me.ValidatedMessage = wwHttpUtils.GetUrlEncodedKey(AuthResult, "MESSAGE")
            Dim AVS As String = wwHttpUtils.GetUrlEncodedKey(AuthResult, "AVS")
            Me.TransactionId = wwHttpUtils.GetUrlEncodedKey(AuthResult, "RRNO")


            If Me.ValidatedResult = "APPROVED" Then
                Me.ValidatedMessage = "Transaction has been approved"
                Me.SetError(Nothing) ' Clear any errors
                Me.AuthorizationCode = wwHttpUtils.GetUrlEncodedKey(AuthResult, "AUTH_CODE")
            ElseIf Me.ValidatedResult = "ERROR" Then
                Me.ValidatedResult = "FAILED"
                Me.SetError(Me.ValidatedMessage)
            ElseIf Me.ValidatedResult = "DECLINED" Then
                Me.SetError(Me.ValidatedMessage)
            Else
                Me.SetError(Me.ValidatedMessage)
            End If

            Me.LogTransaction()

            Return Not Me.Error
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
