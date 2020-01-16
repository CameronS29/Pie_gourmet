Imports Microsoft.VisualBasic
Imports System
Imports System.Reflection
Imports System.Text.RegularExpressions
Imports System.Globalization

Imports NVMS.InternetTools
Imports NVMS.Tools

Namespace NVMS.WebStore
    ' <summary>
    ' 
    ' </summary>
    Public Class ccPayFlowPro
        Inherits ccProcessing
        ' <summary>
        ' Port on the server (Verisign)
        ' </summary>
        Public Property HostPort() As Integer
            Get
                Return _HostPort
            End Get
            Set(ByVal value As Integer)
                _HostPort = value
            End Set
        End Property
        Private _HostPort As Integer = 443


        Public Property ProxyAddress() As String
            Get
                Return _ProxyAddress
            End Get
            Set(ByVal value As String)
                _ProxyAddress = value
            End Set
        End Property
        Private _ProxyAddress As String = ""


        Public Property ProxyPort() As Integer
            Get
                Return _ProxyPort
            End Get
            Set(ByVal value As Integer)
                _ProxyPort = value
            End Set
        End Property
        Private _ProxyPort As Integer = 0


        Public Property ProxyUsername() As String
            Get
                Return _ProxyUsername
            End Get
            Set(ByVal value As String)
                _ProxyUsername = value
            End Set
        End Property
        Private _ProxyUsername As String = ""

        Public Property ProxyPassword() As String
            Get
                Return _ProxyPassword
            End Get
            Set(ByVal value As String)
                _ProxyPassword = value
            End Set
        End Property
        Private _ProxyPassword As String = ""

        ' <summary>
        ' Sign up partner ID. Required only if you signed up through 
        ' a third party.
        ' </summary>
        Public Property SignupPartner() As String
            Get
                Return _SignupPartner
            End Get
            Set(ByVal value As String)
                _SignupPartner = value
            End Set
        End Property
        Private _SignupPartner As String = "Verisign"

        ' <summary>
        ' Overridden consistency with API names.
        ' maps to MerchantId
        ' </summary>
        Public Property UserId() As String
            Get
                Return _UserId
            End Get
            Set(ByVal value As String)
                _UserId = value
            End Set
        End Property
        Private _UserId As String = ""

        ' <summary>
        ' Internal string value used to hold the values sent to the server
        ' </summary>
        Private Parameters As String = ""

        ' <summary>
        ' Validates the credit card. Supported transactions include only Sale or Credit.
        ' Credits should have a negative sales amount.
        ' </summary>
        ' <returns></returns>
        Public Overrides Function ValidateCard() As Boolean
            If (Not MyBase.ValidateCard()) Then
                Return False
            End If

            Me.Error = False
            Me.ErrorMessage = ""

            ' *** Counter that holds our parameter string to send
            Me.Parameters = ""

            ' *** Sale and Credit Supported
            Dim TOrderAmount As Decimal = Me.OrderAmount
            If Me.OrderAmount < 0D Then
                Me.Parameters = "TRXTYPE=C"
                TOrderAmount = Me.OrderAmount * -1D
            Else
                Me.Parameters = "TRXTYPE=S"
            End If

            Dim CardNo As String = Regex.Replace(Me.CreditCardNumber, "[ -/._#]", "")

            Dim TUserId As String = Me.UserId

            If TUserId = "" Then
                TUserId = Me.MerchantId
            End If

            Dim ExpDate As String = String.Format("{0:##}", Me.CreditCardExpirationMonth)
            ExpDate &= Me.CreditCardExpirationYear

            Me.Parameters &= "&TENDER=C" & "&ACCT=" & CardNo & "&VENDOR=" & Me.MerchantId & "&USER=" & TUserId & "&PWD=" & Me.MerchantPassword & "&PARTNER=" & Me.SignupPartner & "&AMT=" & TOrderAmount.ToString(CultureInfo.InvariantCulture.NumberFormat) & "&EXPDATE=" & ExpDate & "&STREET=" & Me.Address & "&CITY=" & Me.City & "&STATE=" & Me.State & "&ZIP=" & Me.Zip & "&EMAIL=" & Me.Email & "&COMMENT1=" & Me.Comment


            If Me.TaxAmount > 0D Then
                Me.Parameters &= "&TAXAMT=" & Me.TaxAmount.ToString(CultureInfo.InvariantCulture.NumberFormat)
            End If

            If Me.SecurityCode <> "" Then
                Me.Parameters &= "&CVV2=" & Me.SecurityCode
            End If


            ' ***  Save our raw input string for debugging
            Me.RawProcessorRequest = Me.Parameters

            ' *** connects to Verisign COM object to handle transaction
            Dim typPayFlow As System.Type = System.Type.GetTypeFromProgID("PFProCOMControl.PFProCOMControl.1")
            Dim PayFlow As Object = Activator.CreateInstance(typPayFlow)

            Me.RawProcessorResult = ""

            Try
                ' *** Use Reflection and Late binding to call COM object
                ' *** to avoid creating Interop assembly
                Dim context As Integer = CInt(Fix(wwUtils.CallMethod(PayFlow, "CreateContext", Me.HttpLink, Me.HostPort, Me.Timeout, Me.ProxyAddress, Me.ProxyPort, Me.ProxyUsername, Me.ProxyPassword)))

                Me.RawProcessorResult = CStr(wwUtils.CallMethod(PayFlow, "SubmitTransaction", context, Parameters, Parameters.Length))

                wwUtils.CallMethod(PayFlow, "DestroyContext", context)
            Catch ex As Exception
                Me.ValidatedResult = "FAILED"
                Me.ValidatedMessage = ex.Message
                Me.LogTransaction()
                Return False
            End Try

            Dim ResultValue As String = wwHttpUtils.GetUrlEncodedKey(Me.RawProcessorResult, "RESULT")

            Me.TransactionId = wwHttpUtils.GetUrlEncodedKey(Me.RawProcessorResult, "PNREF")


            ' *** 0 means success 
            If ResultValue = "0" Then
                Me.ValidatedResult = "APPROVED"
                Me.AuthorizationCode = wwHttpUtils.GetUrlEncodedKey(Me.RawProcessorResult, "AUTHCODE")
                Me.LogTransaction()
                Return True
                ' *** Empty response means we have an unknown failure
            ElseIf ResultValue = "" Then
                Me.ValidatedMessage = "Unknown Error"
                Me.ValidatedResult = "FAILED"
                ' *** Negative number means communication failure
            ElseIf ResultValue.StartsWith("-") Then
                Me.ValidatedResult = "FAILED"
                Me.ValidatedMessage = wwHttpUtils.GetUrlEncodedKey(Me.RawProcessorResult, "RESPMSG")

                ' *** POSITIVE number means we have a DECLINE 
            Else
                Me.ValidatedResult = "DECLINED"
                Me.ValidatedMessage = wwHttpUtils.GetUrlEncodedKey(Me.RawProcessorResult, "RESPMSG")
            End If

            Me.Error = True
            Me.LogTransaction()
            Return False
        End Function

    End Class
End Namespace
