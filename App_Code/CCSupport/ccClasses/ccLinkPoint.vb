'

'Set the EnableLinkPoint Flag either here or via compiler Constant switch to enable this interface.

'By default this class is disabled (not included) in order to avoid pulling in the required
'References to referenced Assemblies for the LinkPoint API. When disabled this class won't
'instantiate - it will throw and Exception in the constructor.

'To Enable this class:

'*  Make sure to copy LinkPointTransaction.dll and lpSSL.dll from the LinkPoint SDK
   'into your /bin or executable directory
'*  Make sure you installed the OpenSSL DLL per LinkPoint (see Web Store ccLinkPoint help for details)
'*  Add a reference to the project for LinkPointTransaction.dll
 
'
'#define EnableLinkPoint  


Imports Microsoft.VisualBasic
Imports System
Imports System.Reflection
Imports System.Text.RegularExpressions
Imports System.Globalization

Imports NVMS.InternetTools
Imports NVMS.Tools

#If EnableLinkPoint Then
Imports LinkPointTransaction
#End If

Namespace NVMS.WebStore
    ' <summary>
    ' The ccLinkPoint class provides an ccProcessing interface to the 
    ' LinkPoint 6.0 interface.
    ' </summary>
    Public Class ccLinkPoint
        Inherits ccProcessing
        ' <summary>
        ' The Port used by the LinkPoint API to communicate with the server.
        ' This port will be provided to you by LinkPoint. Note that you
        ' can also provide the port as part of the HTTPLink domainname.
        ' </summary>
        Public HostPort As Integer = 1129

        Public Sub New()
            Me.HttpLink = "secure.linkpt.net:1129"
        End Sub

#If EnableLinkPoint Then

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

		' *** Parse the HttpLink for domain + port number
		Dim At As Integer = Me.HttpLink.IndexOf(":")
		If At > 0 Then
			Dim t As String = Me.HttpLink.Substring(At+1)
			Me.HostPort = Integer.Parse(t)
			Me.HttpLink = Me.HttpLink.Substring(0,At)
		End If

		' create order
		Dim order As LPOrderPart = LPOrderFactory.createOrderPart("order")

		' create a part we will use to build the order
		Dim op As LPOrderPart = LPOrderFactory.createOrderPart()

		' Figure out what type of order we are processing
		If Me.OrderAmount < 0 Then
			op.put("ordertype","CREDIT")
			Me.OrderAmount = Me.OrderAmount * -1
		ElseIf Me.ProcessType = ccProcessTypes.Credit Then
			op.put("ordertype","CREDIT")
		ElseIf Me.ProcessType = ccProcessTypes.Sale Then
			op.put("ordertype","SALE")
		ElseIf Me.ProcessType = ccProcessTypes.PreAuth Then
			op.put("ordertype","PREAUTH")
		End If

		' add 'orderoptions to order
		order.addPart("orderoptions", op)

		' Build 'merchantinfo'
		op.clear()
		op.put("configfile",Me.MerchantId)

		' add 'merchantinfo to order
		order.addPart("merchantinfo", op)

		' Build 'billing'
		' Required for AVS. If not provided, 
		' transactions will downgrade.
		op.clear()
		op.put("name",Me.Name)
		op.put("address1",Me.Address)
		op.put("city",Me.City)
		op.put("state",Me.State)
		op.put("zip",Me.Zip)
		op.put("country",Me.Country)
		op.put("email",Me.Email)
		op.put("phone",Me.Phone)

		Dim AddrNum As Integer = 0
		Try
			Dim T As String = Me.Address.Substring(0, Me.Address.IndexOf(" ")).Trim()
			AddrNum = Integer.Parse(T)
		Catch

		End Try

		If AddrNum <> 0 Then
			op.put("addrnum", AddrNum.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat))
		End If
		order.addPart("billing", op)

		' Build 'creditcard'
		op.clear()
		op.put("cardnumber",Me.CreditCardNumber)
		op.put("cardexpmonth",Me.CreditCardExpirationMonth)
		op.put("cardexpyear",Me.CreditCardExpirationYear)

		order.addPart("creditcard", op)

		' Build 'payment'
		op.clear()
		op.put("chargetotal",Me.OrderAmount.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat))

		order.addPart("payment", op)

		' *** Trasnaction Details
		op.clear()
		If Not Me.OrderId Is Nothing AndAlso Me.OrderId <> "" Then
			op.put("oid",Me.OrderId)
		End If

		order.addPart("transactiondetails",op)

		If Me.Comment <> "" Then
			' *** Notes
			op.clear()
			op.put("comments",Me.Comment)
			order.addPart("notes",op)
		End If

		' create transaction object	
		Dim LPTxn As LinkPointTxn = New LinkPointTxn()

		' get outgoing XML from the 'order' object
		Me.RawProcessorRequest = order.toXML()

		' Call LPTxn
		Try
			Me.RawProcessorResult = LPTxn.send(Me.CertificatePath,Me.HttpLink,Me.HostPort, Me.RawProcessorRequest)
		Catch ex As Exception
			If Me.RawProcessorResult <> "" Then
				Dim Msg As String = wwUtils.ExtractString(Me.RawProcessorResult,"<r_error>","</r_error>")
				If Msg = "" Then
					Me.SetError(ex.Message)
				Else
					Me.SetError(Msg)
				End If
			Else
				Me.SetError(ex.Message)
			End If

			Me.ValidatedMessage = Me.ErrorMessage
			Me.ValidatedResult = "FAILED"
			Return False
		End Try

		Me.ValidatedResult = wwUtils.ExtractString(Me.RawProcessorResult,"<r_approved>","</r_approved>")
		If Me.ValidatedResult = "" Then
			Me.ValidatedResult = "FAILED"
		End If
		Me.ValidatedMessage = wwUtils.ExtractString(Me.RawProcessorResult,"<r_message>","</r_message>")


		If Me.ValidatedResult = "APPROVED" Then
			Me.SetError(Nothing)
		Else
			Me.SetError(wwUtils.ExtractString(Me.RawProcessorResult,"<r_error>","</r_error>"))
			Me.ValidatedMessage = Me.ErrorMessage
		End If

		Me.LogTransaction()

		Return Not Me.Error
	End Function
#Else
        Public Overrides Function ValidateCard() As Boolean
            Throw New Exception("ccLinkPoint Class is not enabled. Set the EnableLinkPoint #define in the source file.")
        End Function
#End If
    End Class
End Namespace

