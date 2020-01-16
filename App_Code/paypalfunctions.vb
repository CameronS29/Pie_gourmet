Imports System
Imports System.Collections
Imports System.Collections.Specialized
Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Data
Imports System.Configuration
Imports System.Web

Public Class NVPAPICaller
    Private LogFolder As String = ConfigurationManager.AppSettings("LogFolder")

    Private pendpointurl As String = "https://api-3t.paypal.com/nvp"
    Private Const CVV2 As String = "CVV2"

    'Flag that determines the PayPal environment (live or sandbox) 
    Private Const bSandbox As Boolean = False

    Private Const SIGNATURE As String = "SIGNATURE"
    Private Const PWD As String = "PWD"
    Private Const ACCT As String = "ACCT"

    'Demo Info
    'Public APIUsername As String = "dmogolo-facilitator_api1.gmail.com" 'LIVE:"dmogolo_api1.gmail.com"
    'Private APIPassword As String = "3RHNDEL64SWQKWRE" 'LIVE:3ZPHJPRL48EV4B2B"
    'Private APISignature As String = "AFcWxV21C7fd0v3bYYYRCpSSRl31A.oclxUYEOqQoJVh4NLVokmcAWSC"  'LIVE: "AnZ37NxWC9QsZm7Xq7naTfjbPYYhA9g64QfAbJZ.6btvPvGs22e62hoF"

    'Live Info
    Public APIUsername As String = "dmogolo_api1.gmail.com"
    Private APIPassword As String = "3ZPHJPRL48EV4B2B"
    Private APISignature As String = "AnZ37NxWC9QsZm7Xq7naTfjbPYYhA9g64QfAbJZ.6btvPvGs22e62hoF"


    Private Subject As String = ""
    Private BNCode As String = "PP-ECWizard"

    'HttpWebRequest Timeout specified in milliseconds 
    Private Const Timeout As Integer = 5000
    Private Shared ReadOnly SECURED_NVPS As String() = New String() {ACCT, CVV2, SIGNATURE, PWD}

    ''' <summary> 
    ''' Sets the API Credentials 
    ''' </summary> 
    ''' <param name="Userid"></param> 
    ''' <param name="Pwd"></param> 
    ''' <param name="Signature"></param>
    Public Sub SetCredentials(ByVal Userid As String, ByVal Pwd As String, ByVal Signature As String)
        APIUsername = Userid
        APIPassword = Pwd
        APISignature = Signature
    End Sub

    ''' <summary> 
    ''' ShortcutExpressCheckout: The method that calls SetExpressCheckout API 
    ''' </summary> 
    ''' <param name="amt"></param> 
    ''' <param name="token"></param> 
    ''' <param name="retMsg"></param> 
    ''' <returns></returns> 
    Public Function ShortcutExpressCheckout(ByVal amt As String, ByRef token As String, ByRef retMsg As String) As Boolean
        Dim host As String = "www.paypal.com"
        If bSandbox Then
            pendpointurl = "https://api-3t.sandbox.paypal.com/nvp"
            host = "www.sandbox.paypal.com"
        End If

        'Dim returnURL As String = "http://piegourmet.jskdesign.net/Checkout.aspx?p=PayPalDone"
        'Dim cancelURL As String = "http://piegourmet.jskdesign.net/Checkout.aspx"

        Dim returnURL As String = "https://www.piegourmet.com/ReceivePayPal.aspx" 'Live Site
        Dim cancelURL As String = "https://www.piegourmet.com/Checkout.aspx" 'Live Site

        Dim encoder As New NVPCodec()
        encoder("METHOD") = "SetExpressCheckout"
        encoder("RETURNURL") = returnURL
        encoder("CANCELURL") = cancelURL
        encoder("PAYMENTREQUEST_0_AMT") = amt
        encoder("PAYMENTREQUEST_0_PAYMENTACTION") = "Sale"
        encoder("PAYMENTREQUEST_0_CURRENCYCODE") = "USD"

        Dim pStrrequestforNvp As String = encoder.Encode()
        Dim pStresponsenvp As String = HttpCall(pStrrequestforNvp)

        Dim decoder As New NVPCodec()
        decoder.Decode(pStresponsenvp)

        Dim strAck As String = decoder("ACK").ToLower()
        If strAck IsNot Nothing AndAlso (strAck = "success" OrElse strAck = "successwithwarning") Then
            token = decoder("TOKEN")

            Dim ECURL As String = ("https://" & host & "/cgi-bin/webscr?cmd=_express-checkout" & "&token=") + token

            retMsg = ECURL
            TempFileCache.WriteLog(LogFolder & "\PayPalLog.txt", Date.Now() & vbCrLf & retMsg & vbCrLf)

            Return True
        Else
            retMsg = (("ErrorCode=" & decoder("L_ERRORCODE0") & "&" & "Desc=") + decoder("L_SHORTMESSAGE0") & "&" & "Desc2=") + decoder("L_LONGMESSAGE0")
            TempFileCache.WriteLog(LogFolder & "\PayPalLog.txt", Date.Now() & vbCrLf & retMsg & vbCrLf)

            Return False
        End If
    End Function

    ''' <summary> 
    ''' MarkExpressCheckout: The method that calls SetExpressCheckout API, invoked from the 
    ''' Billing Page EC placement 
    ''' </summary> 
    ''' <param name="amt"></param> 
    ''' <param name="token"></param> 
    ''' <param name="retMsg"></param> 
    ''' <returns></returns> 
    Public Function MarkExpressCheckout(ByVal amt As String, ByVal shipToName As String, ByVal shipToStreet As String, ByVal shipToStreet2 As String, ByVal shipToCity As String, ByVal shipToState As String, _
    ByVal shipToZip As String, ByVal shipToCountryCode As String, ByRef token As String, ByRef retMsg As String) As Boolean
        Dim host As String = "www.paypal.com"
        If bSandbox Then
            pendpointurl = "https://api-3t.sandbox.paypal.com/nvp"
            host = "www.sandbox.paypal.com"
        End If

        'Dim returnURL As String = "http://piegourmet.jskdesign.net/ReceivePayPal.aspx" 'Testing Site
        'Dim cancelURL As String = "http://piegourmet.jskdesign.net/Checkout.aspx" 'Testing Site

        Dim returnURL As String = "https://www.piegourmet.com/ReceivePayPal.aspx" 'Live Site
        Dim cancelURL As String = "https://www.piegourmet.com/Checkout.aspx" 'Live Site

        Dim encoder As New NVPCodec()
        encoder("METHOD") = "SetExpressCheckout"
        encoder("RETURNURL") = returnURL
        encoder("CANCELURL") = cancelURL
        encoder("PAYMENTREQUEST_0_AMT") = amt
        encoder("PAYMENTREQUEST_0_PAYMENTACTION") = "Sale"
        encoder("PAYMENTREQUEST_0_CURRENCYCODE") = "USD"

        'Optional Shipping Address entered on the merchant site 
        encoder("PAYMENTREQUEST_0_SHIPTONAME") = shipToName
        encoder("PAYMENTREQUEST_0_SHIPTOSTREET") = shipToStreet
        encoder("PAYMENTREQUEST_0_SHIPTOSTREET2") = shipToStreet2
        encoder("PAYMENTREQUEST_0_SHIPTOCITY") = shipToCity
        encoder("PAYMENTREQUEST_0_SHIPTOSTATE") = shipToState
        encoder("PAYMENTREQUEST_0_SHIPTOZIP") = shipToZip
        encoder("PAYMENTREQUEST_0_SHIPTOCOUNTRYCODE") = shipToCountryCode

        Dim pStrrequestforNvp As String = encoder.Encode()
        Dim pStresponsenvp As String = HttpCall(pStrrequestforNvp)

        Dim decoder As New NVPCodec()
        decoder.Decode(pStresponsenvp)

        Dim strAck As String = decoder("ACK").ToLower()
        If strAck IsNot Nothing AndAlso (strAck = "success" OrElse strAck = "successwithwarning") Then
            token = decoder("TOKEN")

            Dim ECURL As String = ("https://" & host & "/cgi-bin/webscr?cmd=_express-checkout" & "&token=") + token

            retMsg = ECURL
            TempFileCache.WriteLog(LogFolder & "\PayPalLog.txt", Date.Now() & vbCrLf & retMsg & vbCrLf)
            Return True
        Else
            retMsg = (("ErrorCode=" & decoder("L_ERRORCODE0") & "&" & "Desc=") + decoder("L_SHORTMESSAGE0") & "&" & "Desc2=") + decoder("L_LONGMESSAGE0")
            TempFileCache.WriteLog(LogFolder & "\PayPalLog.txt", Date.Now() & vbCrLf & retMsg & vbCrLf)
            Return False
        End If
    End Function

    ''' <summary> 
    ''' GetShippingDetails: The method that calls SetExpressCheckout API, invoked from the 
    ''' Billing Page EC placement 
    ''' </summary> 
    ''' <param name="token"></param> 
    ''' <param name="retMsg"></param> 
    ''' <returns></returns> 
    Public Function GetShippingDetails(ByVal token As String, ByRef PayerId As String, ByRef ShippingAddress As String, ByRef retMsg As String) As Boolean

        If bSandbox Then
            pendpointurl = "https://api-3t.sandbox.paypal.com/nvp"
        End If

        Dim encoder As New NVPCodec()
        encoder("METHOD") = "GetExpressCheckoutDetails"
        encoder("TOKEN") = token

        Dim pStrrequestforNvp As String = encoder.Encode()
        Dim pStresponsenvp As String = HttpCall(pStrrequestforNvp)

        Dim decoder As New NVPCodec()
        decoder.Decode(pStresponsenvp)

        Dim strAck As String = decoder("ACK").ToLower()
        If strAck IsNot Nothing AndAlso (strAck = "success" OrElse strAck = "successwithwarning") Then
            ShippingAddress = "<table><tr>"
            ShippingAddress += "<td> First Name </td><td>" & decoder("FIRSTNAME") & "</td></tr>"
            ShippingAddress += "<td> Last Name </td><td>" & decoder("LASTNAME") & "</td></tr>"
            ShippingAddress += "<td colspan='2'> Shipping Address</td></tr>"
            ShippingAddress += "<td> Name </td><td>" & decoder("PAYMENTREQUEST_0_SHIPTONAME") & "</td></tr>"
            ShippingAddress += "<td> Street1 </td><td>" & decoder("PAYMENTREQUEST_0_SHIPTOSTREET") & "</td></tr>"
            ShippingAddress += "<td> Street2 </td><td>" & decoder("PAYMENTREQUEST_0_SHIPTOSTREET2") & "</td></tr>"
            ShippingAddress += "<td> City </td><td>" & decoder("PAYMENTREQUEST_0_SHIPTOCITY") & "</td></tr>"
            ShippingAddress += "<td> State </td><td>" & decoder("PAYMENTREQUEST_0_SHIPTOSTATE") & "</td></tr>"
            ShippingAddress += "<td> Zip </td><td>" & decoder("PAYMENTREQUEST_0_SHIPTOZIP") & "</td>"
            ShippingAddress += "</tr>"

            Return True
        Else
            retMsg = (("ErrorCode=" & decoder("L_ERRORCODE0") & "&" & "Desc=") + decoder("L_SHORTMESSAGE0") & "&" & "Desc2=") + decoder("L_LONGMESSAGE0")

            Return False
        End If
    End Function

    ''' <summary> 
    ''' ConfirmPayment: The method that calls SetExpressCheckout API, invoked from the 
    ''' Billing Page EC placement 
    ''' </summary> 
    ''' <param name="token"></param> 
    ''' <param name="retMsg"></param> 
    ''' <returns></returns> 
    Public Function ConfirmPayment(ByVal finalPaymentAmount As String, ByVal token As String, ByVal PayerId As String, ByRef decoder As NVPCodec, ByRef retMsg As String) As Boolean
        If bSandbox Then
            pendpointurl = "https://api-3t.sandbox.paypal.com/nvp"
        End If

        Dim encoder As New NVPCodec()
        encoder("METHOD") = "DoExpressCheckoutPayment"
        encoder("TOKEN") = token
        encoder("PAYMENTACTION") = "Sale"
        encoder("PAYERID") = PayerId
        encoder("AMT") = finalPaymentAmount
		encoder("CURRENCYCODE") = "USD"

        Dim pStrrequestforNvp As String = encoder.Encode()
        Dim pStresponsenvp As String = HttpCall(pStrrequestforNvp)

        decoder = New NVPCodec()
        decoder.Decode(pStresponsenvp)

        Dim strAck As String = decoder("ACK").ToLower()
        If strAck IsNot Nothing AndAlso (strAck = "success" OrElse strAck = "successwithwarning") Then
            Return True
        Else
            retMsg = (("ErrorCode=" & decoder("L_ERRORCODE0") & "&" & "Desc=") + decoder("L_SHORTMESSAGE0") & "&" & "Desc2=") + decoder("L_LONGMESSAGE0")

            Return False
        End If
    End Function

    ''' <summary> 
    ''' HttpCall: The main method that is used for all API calls 
    ''' </summary> 
    ''' <param name="NvpRequest"></param> 
    ''' <returns></returns> 
    Public Function HttpCall(ByVal NvpRequest As String) As String
        'CallNvpServer 
        Dim url As String = pendpointurl

        'To Add the credentials from the profile
        Dim codec As New NVPCodec()
        Dim strPost As String = (NvpRequest & "&") + buildCredentialsNVPString()
        strPost = (strPost & "&BUTTONSOURCE=") + HttpUtility.UrlEncode(BNCode)

        Dim objRequest As HttpWebRequest = DirectCast(WebRequest.Create(url), HttpWebRequest)
        objRequest.Timeout = Timeout
        objRequest.Method = "POST"
        objRequest.ContentLength = strPost.Length

        Try
            Using myWriter As New StreamWriter(objRequest.GetRequestStream())
                myWriter.Write(strPost)
            End Using
        Catch e As Exception
            ' 
            ' if (log.IsFatalEnabled) 
            ' { 
            ' log.Fatal(e.Message, this); 
            ' } 

        End Try

        'Retrieve the Response returned from the NVP API call to PayPal 
        Dim objResponse As HttpWebResponse = DirectCast(objRequest.GetResponse(), HttpWebResponse)
        Dim result As String
        Using sr As New StreamReader(objResponse.GetResponseStream())
            result = sr.ReadToEnd()
        End Using

        'Logging the response of the transaction 
        ' if (log.IsInfoEnabled) 
        ' { 
        ' log.Info("Result :" + 
        ' " Elapsed Time : " + (DateTime.Now - startDate).Milliseconds + " ms" + 
        ' result); 
        ' } 
        ' 

        Return result
    End Function

    ''' <summary> 
    ''' Credentials added to the NVP string 
    ''' </summary>
    ''' <returns></returns> 
    Private Function buildCredentialsNVPString() As String
        Dim codec As New NVPCodec()

        If Not IsEmpty(APIUsername) Then
            codec("USER") = APIUsername
        End If

        If Not IsEmpty(APIPassword) Then
            codec(PWD) = APIPassword
        End If

        If Not IsEmpty(APISignature) Then
            codec(SIGNATURE) = APISignature
        End If

        If Not IsEmpty(Subject) Then
            codec("SUBJECT") = Subject
        End If

        codec("VERSION") = "93"

        Return codec.Encode()
    End Function

    ''' <summary> 
    ''' Returns if a string is empty or null 
    ''' </summary> 
    ''' <param name="s">the string</param> 
    ''' <returns>true if the string is not null and is not empty or just whitespace</returns> 
    Public Shared Function IsEmpty(ByVal s As String) As Boolean
        Return s Is Nothing OrElse s.Trim() = String.Empty
    End Function

End Class

Public NotInheritable Class NVPCodec

    Inherits NameValueCollection
    Private Const AMPERSAND As String = "&"
    Private Const EQUALSIGN As String = "="
    Private Shared ReadOnly AMPERSAND_CHAR_ARRAY As Char() = AMPERSAND.ToCharArray()
    Private Shared ReadOnly EQUALS_CHAR_ARRAY As Char() = EQUALSIGN.ToCharArray()

    ''' <summary> 
    ''' Returns the built NVP string of all name/value pairs in the Hashtable 
    ''' </summary> 
    ''' <returns></returns> 
    Public Function Encode() As String
        Dim sb As New StringBuilder()
        Dim firstPair As Boolean = True
        For Each kv As String In AllKeys
            Dim name As String = UrlEncode(kv)
            Dim value As String = UrlEncode(Me(kv))
            If Not firstPair Then
                sb.Append(AMPERSAND)
            End If
            sb.Append(name).Append(EQUALSIGN).Append(value)
            firstPair = False
        Next
        Return sb.ToString()
    End Function

    ''' <summary> 
    ''' Decoding the string 
    ''' </summary> 
    ''' <param name="nvpstring"></param> 
    Public Sub Decode(ByVal nvpstring As String)
        Clear()
        For Each nvp As String In nvpstring.Split(AMPERSAND_CHAR_ARRAY)
            Dim tokens As String() = nvp.Split(EQUALS_CHAR_ARRAY)
            If tokens.Length >= 2 Then
                Dim name As String = UrlDecode(tokens(0))
                Dim value As String = UrlDecode(tokens(1))
                Add(name, value)
            End If
        Next
    End Sub

    Private Shared Function UrlDecode(ByVal s As String) As String
        Return HttpUtility.UrlDecode(s)
    End Function
    Private Shared Function UrlEncode(ByVal s As String) As String
        Return HttpUtility.UrlEncode(s)
    End Function

#Region "Array methods"
    Public Overloads Sub Add(ByVal name As String, ByVal value As String, ByVal index As Integer)
        Me.Add(GetArrayName(index, name), value)
    End Sub

    Public Overloads Sub Remove(ByVal arrayName As String, ByVal index As Integer)
        Me.Remove(GetArrayName(index, arrayName))
    End Sub

    ''' <summary> 
    ''' 
    ''' </summary> 
    Default Public Overloads Property Item(ByVal name As String, ByVal index As Integer) As String
        Get
            Return Me(GetArrayName(index, name))
        End Get
        Set(ByVal value As String)
            Me(GetArrayName(index, name)) = value
        End Set
    End Property

    Private Shared Function GetArrayName(ByVal index As Integer, ByVal name As String) As String
        If index < 0 Then
            Throw New ArgumentOutOfRangeException("index", "index can not be negative : " & index)
        End If
        Return name + index
    End Function
#End Region

End Class
