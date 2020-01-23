Imports System.Data
Imports System.Data.Common
Imports Microsoft.Practices.EnterpriseLibrary.Data
Imports NVMS.WebStore
Imports NVMS.Tools
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.IO
Imports Newtonsoft.Json.Serialization

Partial Class Checkout
    Inherits System.Web.UI.Page

    Public Shared factory As DatabaseProviderFactory = New DatabaseProviderFactory()
    Public Shared db As Database = factory.CreateDefault()
    Private SecureApp As String = ConfigurationManager.AppSettings("AppSecure")
    Private TempFolder As String = ConfigurationManager.AppSettings("TempFolder")

    ''' <summary>
    '''     The generic ILog logger
    ''' </summary>
    Public Shared Logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger()

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        AjaxPro.Utility.RegisterTypeForAjax(GetType(Checkout))
        AjaxPro.Utility.RegisterTypeForAjax(GetType(GlobalAjax))

        Dim mySession As New SessionManager
        Dim CustomerID As String = mySession.GetElement("CustomerID", "number")
        Dim UserID As String = mySession.GetElement("userID", "number")
        Dim Username As String = mySession.GetElement("username")
        Dim myName As String = mySession.GetElement("FullName")
        Dim MyEmail As String = mySession.GetElement("Email")
        Dim Access As String = mySession.GetElement("AccessLevel")
        Dim LastProduct As String = mySession.GetElement("LastProduct", "number")

        'Write Client Values For AJAX Methods *******************************************
        Dim ValArray As New StringDictionary
        ValArray.Add("CustomerID", CustomerID)
        ValArray.Add("UserID", UserID)
        ValArray.Add("Username", Username)
        ValArray.Add("myName", myName)
        ValArray.Add("MyEmail", MyEmail)
        ValArray.Add("access", Access)
        ValArray.Add("SessionID", Session.SessionID)
        WriteClientVals(ValArray)

        Dim q As String = Left(Request.QueryString("q"), 25)
        Dim p As String = Left(Request.QueryString("p"), 25)

        Dim BlockOrders As string = GlobalAjax.GetApplicationSetting("BlockOrders")

        'Orders Blocked, Send back to home page
        If BlockOrders =  "True" Then
            Response.Redirect("Default.aspx")
        End If

        If Not Session("LoggedIn") Then
            Response.Redirect("MyAccount.aspx?goto=Checkout.aspx")
        Else
            If (Request.ServerVariables("HTTPS") = "off") Then
                If ConfigurationManager.AppSettings("ForceSecure") = "True" Then
                    Response.Redirect(SecureApp & "Checkout.aspx")
                End If
            End If

            MySessionValue.Value = Session.SessionID
            ShipHistoryList.InnerHtml = GlobalAjax.ShippingListGetCheckout(CustomerID)
            ProductOrderList.InnerHtml = ProductsCheckout(Session.SessionID)

            'Dropdowns
            DropDowns.GetMonths(0, CCExpireMonth)
            DropDowns.NumberRangeSelectYears(0, CCExpireYear, Year(Date.Now), Year(Date.Now) + 10, 1)

            'Final Checkout/Send Email/Show Product Confirmation
            If p = "PayPalDone" Then
                ProdCheckoutForm.InnerHtml = SubmitPayPal()
            End If

            'Test
            'ProdCheckoutForm.InnerHtml = BuildCartItem("2959")
        End If

    End Sub

    Private Sub WriteClientVals(ByVal ValueArray As StringDictionary)
        Dim sb As New StringBuilder
        Dim i As DictionaryEntry

        For Each i In ValueArray
            sb.Append("var " & i.Key & " = '" & i.Value & "';" & vbCrLf)
        Next

        Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "DashClientVals", sb.ToString, True)
    End Sub

    <AjaxPro.AjaxMethod()>
    Function ProductsCheckout(ByVal MySession As String) As String
        Dim sqlCommand As String = "getTempCartDetail"
        Dim dbCommand As DbCommand = db.GetStoredProcCommand(sqlCommand)
        Dim i As Integer = 0
        Dim RetStr As String = ""
        Dim head_html As New StringBuilder
        Dim foot_html As New StringBuilder
        Dim body_html As New StringBuilder
        Dim ShippingZip As String = ""
        Dim DMethod As String = ""
        Dim ProdTotal As Decimal = 0
        Dim TaxRate As Decimal = 0
        Dim ProdTax As Decimal = 0
        Dim TotalWeight As Decimal = 0
        Dim Packaging As Decimal = 0

        ' Add Parameters to SPROC
        db.AddInParameter(dbCommand, "sessionID", DbType.String, MySession)

        Try
            Using rs As IDataReader = db.ExecuteReader(dbCommand)

                While rs.Read()
                    If i = 0 Then
                        head_html.Append("<table class=""shop_table  push-down-30"">")
                        head_html.Append("<thead><tr><th class=""product-name"">Product</th><th class=""product-total"">Total</th></tr></thead>")
                        body_html.Append("<tbody>")
                        ShippingZip = rs("ship_zip").ToString()
                        DMethod = rs("DeliveryMethod").ToString()
                        TaxRate = rs("TaxRate")
                        TotalWeight = rs("total_weight")

                    End If

                    body_html.Append("<tr class=""checkout_table_item"">")
                    body_html.Append("<td class=""product-name"">")
                    body_html.Append("<img src='images/products/" & rs("ProductImage").ToString() & "' style='height: 80px; margin-right: 15px' />")
                    body_html.Append("<a href='javascript:void(0)' onclick=""EditCartCheckItem(" & rs("cart_tempID") & ")""><span class=""glyphicon  glyphicon-circle  glyphicon-edit"" title='Edit this product'></span></a>&nbsp;")
                    body_html.Append("<a href='javascript:void(0)' onclick=""RemoveCartCheckItem(" & rs("cart_tempID") & ")""><span class=""glyphicon  glyphicon-circle  glyphicon-remove"" title='Remove cart item'></span></a>&nbsp;")

                    body_html.Append(rs("ProductName").ToString() & "<strong class=""product-quantity""> Quantity: " & rs("qty").ToString() & "</strong><br> <strong>" & rs("ProductSize").ToString() & " inch </strong> " & rs("TypeCategory").ToString() & "</td>")
                    body_html.Append("<td class=""product-total""><span class=""amount"">" & Common.MyNumber(rs("ExtendPrice")) & "</span></td>")
                    body_html.Append("</tr>")

                    ProdTotal += rs("ExtendPrice")
                    Packaging += rs("Packaging")
                    i += 1
                End While

                ProdTax = (ProdTotal * TaxRate)

                If i >= 1 Then
                    Dim ShippingRate() As String = LocalDeliveryAmount(ShippingZip, TotalWeight, DMethod).Split("|")
                    Dim TotalOrderPrice As Decimal = 0

                    body_html.Append("</tbody></table>")
                    foot_html.Append("<tfoot>")
                    foot_html.Append(" <tr class=""cart-subtotal"">")
                    foot_html.Append("<th>Cart Subtotal</th>")
                    foot_html.Append("<td class=""amount"">" & Common.MyNumber(ProdTotal) & "</td>")
                    foot_html.Append("</tr>")
                    foot_html.Append("<tr class=""shipping"">")
                    foot_html.Append("<th>Shipping (" & ShippingRate(2) & ")</th>")
                    foot_html.Append("<td class=""amount"">" & Common.MyNumber(ShippingRate(1)) & "</td>")
                    foot_html.Append(" </tr>")
                    foot_html.Append("<tr class=""shipping"">")
                    foot_html.Append("<th>Packaging</th>")
                    foot_html.Append("<td class=""amount"">" & Common.MyNumber(Packaging) & "</td>")
                    foot_html.Append(" </tr>")
                    foot_html.Append("<tr class=""shipping"">")
                    foot_html.Append("<th>Sub-Total</th>")
                    foot_html.Append("<td class=""amount""><strong>" & Common.MyNumber(ProdTotal + ShippingRate(1) + Packaging) & "</strong></td>")
                    foot_html.Append(" </tr>")
                    foot_html.Append("<tr class=""shipping"">")
                    foot_html.Append("<th>Taxes</th>")
                    foot_html.Append("<td class=""amount"">" & Common.MyNumber(ProdTax) & "</span></td>")
                    foot_html.Append("</tr>")
                    foot_html.Append("<tr class=""total"">")
                    foot_html.Append("<th><strong>Order Total</strong></th>")
                    foot_html.Append("<td class=""amount"">")
                    foot_html.Append("<strong>" & Common.MyNumber(ProdTotal + ShippingRate(1) + Packaging + ProdTax) & "</strong>")
                    foot_html.Append("</td>")
                    foot_html.Append(" </tr>")
                    foot_html.Append(" </tfoot>")

                    'Set Shipping Total
                    TotalOrderPrice = ProdTotal + ShippingRate(1) + Packaging + ProdTax
                    System.Web.HttpContext.Current.Session("payment_amt") = Common.MyNumber(TotalOrderPrice)
                End If

                If i = 0 Then
                    RetStr = "No products found in your cart... Please try again. <a href='menu.aspx'>Continue Shopping</a>"
                Else
                    RetStr = head_html.ToString() & foot_html.ToString() & body_html.ToString()
                End If
            End Using

        Catch ex As Exception
            RetStr = "Product List Error: " & ex.Message.ToString()
            Logger.Error(ex, "Checkout Functions: " & Reflection.MethodBase.GetCurrentMethod.Name & " - " & ex.Message.ToString())
        End Try

        Return RetStr
    End Function

    Function LocalDeliveryAmount(ByVal zip As String, ByVal weight As Integer, ByVal DeliveryMethod As String) As String
        Dim sqlCommand As String = "getFedExRates"
        Dim i As Integer = 0
        Dim RetStr As String = ""
        Try
            If DeliveryMethod = "pickup" Then
                RetStr = "0|0|Pickup"
            Else
                Using dbCommand As DbCommand = db.GetStoredProcCommand(sqlCommand)
                    db.AddInParameter(dbCommand, "zip", DbType.String, zip)
                    db.AddInParameter(dbCommand, "weight", DbType.Int32, weight)

                    Using rs As IDataReader = db.ExecuteReader(dbCommand)
                        While rs.Read()
                            If Not String.IsNullOrEmpty(rs("Local_Delivery").ToString()) Then
                                RetStr = "0|" & rs("Local_Delivery").ToString() & "|Local Delivery"
                            ElseIf Not String.IsNullOrEmpty(rs("FedEx_2Day").ToString()) Then
                                RetStr = "0|" & rs("FedEx_2Day").ToString() & "|FedEx 2 Day"
                            ElseIf Not String.IsNullOrEmpty(rs("Standard_Overnight").ToString()) Then
                                RetStr = "0|" & rs("Standard_Overnight").ToString() & "|Standard Overnight"
                            Else
                                RetStr = "0|0|No Delivery"
                            End If
                            i += 1
                        End While
                    End Using
                End Using

                If i = 0 Then
                    RetStr = "99|Delivery rates not found for this zip code: " & zip & "|None"
                End If
            End If

        Catch ex As Exception
            Logger.Error(ex, "Checkout Functions: " & Reflection.MethodBase.GetCurrentMethod.Name & " - " & ex.Message.ToString())
            Return "1|" & ex.Message.ToString & "|Error"
        End Try

        Return RetStr
    End Function

    <AjaxPro.AjaxMethod()>
    Function GetProduct(ByVal tempID As Integer) As DataSet
        Dim sqlCommand As String = "SELECT * FROM [temp_cart] WHERE cart_tempID = " & tempID
        Dim dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)
        Dim ds As New DataSet
        Try
            ds = db.ExecuteDataSet(dbCommand)
            Return ds
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    <AjaxPro.AjaxMethod()>
    Function LoadProduct(ByVal ProductID As Integer) As String
        'Fresh Product List //////////////////////////////////
        Dim sqlCommand As String = "SELECT * FROM [products_view] WHERE productID = " & ProductID & " ORDER BY ProductSize DESC"
        Dim html As New StringBuilder
        Dim dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)
        Dim i As Integer = 0
        Dim Category As New StringBuilder
        Dim ProductDrop As String = ""

        Try
            Using rs As IDataReader = db.ExecuteReader(dbCommand)

                While rs.Read()
                    If i = 0 Then

                        Dim ProductName As String = rs("ProductName").ToString
                        Dim ProductDescription As String = rs("ProductDescription").ToString
                        Dim ProductImage As String = "images/products/" & rs("ProductImage").ToString

                        Category.Append("<h3><img src='" & ProductImage & "' style='height: 60px; align: left'> <span id=""ProductCartName"">" & ProductName & "</span></h3>")
                        Category.Append("<div class=""col-xs-12  col-sm-12  push-down-3""><span class='small'>" & ProductDescription & "</span></div>")
                    End If

                    html.Append("<option value='" & rs("pricingID").ToString & "'>" & rs("ProductSize").ToString & " inch " & rs("TypeCategory").ToString & " - " & Common.MyNumber(rs("ProductPrice")) & "</option>")

                    i += 1
                End While

                ProductDrop = "<select class=""btn btn-shop"" id=""ProductPricingID"" onchange=""ChangeProductPrice(this)"">" & html.ToString & "</select>"

            End Using

        Catch ex As Exception
            Logger.Error(ex, "Checkout Functions: " & Reflection.MethodBase.GetCurrentMethod.Name & " - " & ex.Message.ToString())
            Return ex.Message.ToString()
        End Try

        Return Category.ToString() & ProductDrop
    End Function

    <AjaxPro.AjaxMethod()>
    Function SaveCartUpdate(ByVal tempID As Integer, ByVal qty As Integer, ByVal pricingID As Integer) As String
        Dim sqlCommand As String = "update_cart_temp"
        Dim i As Integer = 0
        Try
            Using dbCommand As DbCommand = db.GetStoredProcCommand(sqlCommand)
                db.AddInParameter(dbCommand, "cartID", DbType.Int32, tempID)
                db.AddInParameter(dbCommand, "qty", DbType.Int32, qty)
                db.AddInParameter(dbCommand, "pricingID", DbType.String, pricingID)
                db.AddOutParameter(dbCommand, "retval", DbType.Int32, 4)
                i = db.ExecuteNonQuery(dbCommand)
                Dim retval As Integer = db.GetParameterValue(dbCommand, "retval")
                Return "0|" & retval.ToString
            End Using
        Catch ex As Exception
            Logger.Error(ex, "Checkout Functions: " & Reflection.MethodBase.GetCurrentMethod.Name & " - " & ex.Message.ToString())
            Return "1|" & ex.Message.ToString
        End Try
    End Function

    Public Class EFTClass
        Public TotalBill As String = ""
        Public EFTCCNum As String = ""
        Public EFTExpDate As String = ""
        Public EFTCorpID As String = ""
        Public EFTName As String = ""
        Public EFTAddress As String = ""
        Public EFTCity As String = ""
        Public EFTState As String = ""
        Public EFTZip As String = ""
        Public EFTCSV As String = ""
        Public EFTEmail As String = ""
        Public Date1 As String = ""
        Public Date2 As String = ""
        Public myMonth As String = ""
        Public myYear As String = ""
        Public BillClientID As Integer = 0
        Public InvoiceRef As String = ""
        Public UserSession As String = ""
        Public username As String = ""
        Public PayBatch As String = ""
        Public DeliveryMethod As String = ""
        Public CartID As Integer = 0
        Public TotalTax As String = ""
        Public TotalPackage As String = ""
        Public TotalShipping As String = ""
    End Class

    <AjaxPro.AjaxMethod()>
    Function GetEFTClass() As EFTClass
        Dim cd As EFTClass = New EFTClass
        Return cd
    End Function

    <AjaxPro.AjaxMethod()>
    Function ValidateSubmitCreditCardProcess(ByVal md As EFTClass) As String
        Dim sqlCommand As String = "EXEC [getTempCartDetail] @sessionID = '" & Common.SQLEncode(Left(md.PayBatch, 50)) & "'"
        Dim dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)
        Dim i As Integer = 0

        Dim ShippingZip As String = ""
        Dim DMethod As String = ""
        Dim ProdTotal As Decimal = 0
        Dim Packaging As Decimal = 0
        Dim TaxRate As Decimal = 0
        Dim TotalWeight As Integer = 0
        Dim RetVal As String = ""

        Try
            Using rs As IDataReader = db.ExecuteReader(dbCommand)
                While rs.Read()
                    'Get Header Values (Only One Value)
                    If i = 0 Then
                        With md
                            .EFTName = rs("first_name").ToString() & " " & rs("last_name").ToString()
                            .EFTAddress = rs("address1").ToString()
                            .EFTCity = rs("city").ToString()
                            .EFTState = rs("state").ToString()
                            .EFTZip = rs("zip").ToString()
                            .EFTEmail = rs("email").ToString()
                            .BillClientID = rs("customerID")
                            .DeliveryMethod = rs("DeliveryMethod").ToString()
                        End With

                        ShippingZip = rs("ship_zip").ToString()
                        DMethod = rs("DeliveryMethod").ToString()
                        TaxRate = 0.045
                    End If

                    ProdTotal += rs("ExtendPrice") 'Need To Add Shipping!
                    TotalWeight += rs("total_weight")
                    Packaging += rs("Packaging")

                    i += 1
                End While

                'We have records... Continue
                If i >= 1 Then
                    Dim TotalToTax as Decimal = 0
                    
                    Dim ShippingRate() As String = LocalDeliveryAmount(ShippingZip, TotalWeight, DMethod).Split("|")
                    TotalToTax = ProdTotal
                   
                    Dim ProdTax As Decimal = TotalToTax * TaxRate

                    md.TotalTax = ProdTax
                    md.TotalPackage = Packaging
                    md.TotalShipping = ShippingRate(1)
                    md.TotalBill = Common.MyNumberNoCurr(ProdTotal + ShippingRate(1) + ProdTax + Packaging)
                    md.EFTExpDate = md.myMonth & "/" & md.myYear
                    RetVal = SubmitValidateOrder(md)

                End If
            End Using
        Catch ex As Exception
            Logger.Error(ex, "Checkout Functions: " & Reflection.MethodBase.GetCurrentMethod.Name & " - " & ex.Message.ToString())
            Return "1|" & ex.Message.ToString
        End Try

        Return RetVal
    End Function

    Function SubmitPayPal() As String
        Dim MySession As String = Session.SessionID
        Dim i As Integer = 0
        Dim RetVal As String = ""
        Dim RetHTML As String = ""
        Try
            RetVal = FinalOrderSubmit(MySession, "", 0)

            Dim RetValResp() As String = RetVal.Split("|")
            If RetValResp(0) = 0 Then
                RetHTML = BuildCartItem(Session.SessionID)
            Else
                RetHTML = "Process Error: " & RetValResp(1)
            End If

        Catch ex As Exception
            Logger.Error(ex, "Checkout Functions: " & Reflection.MethodBase.GetCurrentMethod.Name & " - " & ex.Message.ToString())
            Return "1|" & ex.Message.ToString
        End Try

        Return RetHTML
    End Function

    '<summary>
    'Validate credit card and submit transaction order details.
    'Send confirm email to customer.
    '</summary>
    <AjaxPro.AjaxMethod()>
    Function SubmitValidateOrder(ByVal md As EFTClass) As String
        Dim CC As ccProcessing = Nothing
        Dim Result As Boolean = False
        Dim ResultTxt As New StringBuilder
        Dim AuthCode As String = ""
        Dim MyCartID As Integer = 0
        Dim MerchantUsername As String = App.Configuration.CCMerchantId
        Dim MerchantPassword As String = App.Configuration.CCMerchantPassword
        CC = New ccAuthorizeNet()

        Try
            CC.LogFile = TempFolder & "\CCLog.txt"
            CC.UseTestTransaction = True

            ' *** Tell whether we do SALE or Pre-Auth
            CC.ProcessType = App.Configuration.CCProcessType

            ' *** Disable this for testing to get provider response
            CC.UseMod10Check = False

            CC.Timeout = App.Configuration.CCConnectionTimeout ' In Seconds

            CC.MerchantId = App.Configuration.CCMerchantId
            CC.MerchantUserName = App.Configuration.CCMerchantId
            CC.MerchantPassword = App.Configuration.CCMerchantPassword

            CC.LogFile = ConfigurationManager.AppSettings("LogFolder") & "\eft\ECLinxLog.txt"
            CC.ReferringUrl = App.Configuration.CCReferingOrderUrl

            ' *** Name can be provided as a single string or as firstname and lastname
            'Parse Name
            Dim Name() As String = md.EFTName.TrimEnd.Split(" ")

            'Return Name.Length
            If Name.Length = 2 Then
                CC.Firstname = Name(0)
                CC.Lastname = Name(1)
            ElseIf Name.Length = 3 Then
                CC.Firstname = Name(0) & " " & Name(1)
                CC.Lastname = Name(2)
            ElseIf Name.Length = 4 Then
                CC.Firstname = Name(0) & " " & Name(1) & " " & Name(2)
                CC.Lastname = Name(3)
            Else
                CC.Name = md.EFTName
            End If

            CC.Address = md.EFTAddress
            CC.State = md.EFTState
            CC.City = md.EFTCity
            CC.Zip = md.EFTZip
            CC.Country = "US" ' 2 Character Country ID
            CC.Phone = "703-281-7437"
            CC.Email = md.EFTEmail
            CC.CustomerID = md.BillClientID

            CC.OrderAmount = Decimal.Parse(md.TotalBill)

            CC.CreditCardNumber = md.EFTCCNum
            CC.CreditCardExpiration = md.EFTExpDate
            CC.SecurityCode = md.EFTCSV

            ' *** Make this Unique
            CC.OrderId = md.PayBatch
            CC.PONumber = md.EFTCorpID
            CC.Comment = "Pie Gourmet ordering # " & md.InvoiceRef & " Bill Code: " & md.EFTCorpID

            'Debug Write Values
            'Dim DebugCls As String = JsonConvert.SerializeObject(md)
            'TempFileCache.WriteLog(TempFolder & "\PaymentPostValues.txt", DebugCls & vbCrLf & "*********************** " & Date.Now.ToString() & vbCrLf)

            ' **** Validate Card
            Result = CC.ValidateCard()

            If CC.UseTestTransaction Then
                ResultTxt.Append(CC.ValidatedMessage & "<hr>" & CC.ErrorMessage & "<br>")
            ElseIf Result Then
                ' *** Should be APPROVED
                ResultTxt.Append("Approved: " & CC.ValidatedMessage)
                AuthCode = CC.AuthorizationCode
                'ResultTxt.Append(CC.AvsResultCode & " : " & CC.RawProcessorResult)
            Else
                ResultTxt.Append("1|Result: " & CC.ValidatedResult & " Message: " & CC.ValidatedMessage & "... Please try again.<br>")
                'ResultTxt.Append(CC.AvsResultCode & " : " & CC.RawProcessorResult)
            End If

            ' *** Always write out the raw response
            If wwUtils.Empty(CC.RawProcessorResult) Then
                ResultTxt.Append("<hr>" & "Raw Results:<br>" & CC.RawProcessorResult)
            End If

        Catch ex As Exception
            ResultTxt.Append("1|<hr>FAILED: " & "Processing Error: " & ex.Message)
            Result = False
        End Try

        If Result Or CC.UseTestTransaction Then
            'Save Transaction Send Confirm Email

            'Submit Final Order
            Dim RetFinal() As String = FinalOrderSubmit(md.InvoiceRef, md.DeliveryMethod, md.TotalTax).Split("|")

            If CInt(RetFinal(0)) = 0 Then
                MyCartID = RetFinal(1)
                Dim sqlCommand As String = "process_PaymentHistory"
                Dim i As Integer = 0
                Try
                    Using dbCommand As DbCommand = db.GetStoredProcCommand(sqlCommand)
                        db.AddInParameter(dbCommand, "cartID", DbType.Int32, RetFinal(1))
                        db.AddInParameter(dbCommand, "TotalPayment", DbType.String, md.TotalBill)
                        db.AddInParameter(dbCommand, "AuthCode", DbType.String, AuthCode)
                        db.AddInParameter(dbCommand, "IPAddress", DbType.String, "")
                        db.AddInParameter(dbCommand, "PaymentNote", DbType.String, ResultTxt.ToString)
                        db.AddOutParameter(dbCommand, "retval", DbType.Int32, 4)
                        i = db.ExecuteNonQuery(dbCommand)

                        Dim retval As Integer = db.GetParameterValue(dbCommand, "retval")
                        ResultTxt.Append("<br>[" & retval.ToString & "] Payment posted successfully...")

                        'Send Confirmation To Customer
                        'SendOrderDetails(retval)
                    End Using
                Catch ex As Exception
                    Logger.Error(ex, "Checkout Functions: " & Reflection.MethodBase.GetCurrentMethod.Name & " - " & ex.Message.ToString())
                    Return "1|" & ex.Message.ToString
                End Try
            Else
                Return "1|" & RetFinal(1)
            End If
        Else
            'Send Error Results
            SendErrorMessage("scott@jskdesign.net", ResultTxt.ToString())
        End If

        If Result Or CC.UseTestTransaction Then
            Return "0|" & ResultTxt.ToString() & "|" & MyCartID
        Else
            Return ResultTxt.ToString()
        End If

    End Function

    <AjaxPro.AjaxMethod()>
    Function FinalOrderSubmit(ByVal sessionID As String, ByVal DeliveryMethod As String, byval Tax As String) As String
        Dim sqlCommand As String = "process_order_customer"
        Dim i As Integer = 0
        Try
            Using dbCommand As DbCommand = db.GetStoredProcCommand(sqlCommand)
                db.AddInParameter(dbCommand, "session", DbType.String, Common.SQLEncode(sessionID))
                db.AddInParameter(dbCommand, "ship_type", DbType.String, Common.SQLEncode(DeliveryMethod))
                db.AddInParameter(dbCommand, "Tax", DbType.String, Common.SQLEncode(Tax))
                db.AddOutParameter(dbCommand, "retCart", DbType.Int32, 4)

                i = db.ExecuteNonQuery(dbCommand)

                Dim retval As Integer = db.GetParameterValue(dbCommand, "retCart")
                Return "0|" & retval.ToString
            End Using
        Catch ex As Exception
            Logger.Error(ex, "Checkout Functions: " & Reflection.MethodBase.GetCurrentMethod.Name & " - " & ex.Message.ToString())
            Return "1|" & ex.Message.ToString
        End Try
    End Function

    <AjaxPro.AjaxMethod()>
    Function BuildCartItem(ByVal CartID As String) As String
        'Pending Order Detail List //////////////////////////////////
        Dim sqlstr As String = ""
        If IsNumeric(CartID) Then
            sqlstr = "SELECT * FROM cart_detail WHERE cartID = " & Common.SQLEncode(CartID)
        Else
            sqlstr = "SELECT * FROM cart_detail WHERE sessionID = '" & Common.SQLEncode(CartID) & "'"
        End If

        Dim html As New StringBuilder
        Dim dbCommand As DbCommand = db.GetSqlStringCommand(sqlstr)
        Dim i As Integer = 0
        Dim uCnt As Integer = 0
        Dim oTotal As Decimal = 0
        Dim oDue As Decimal = 0
        Dim dMeth As String = ""
        Dim shipMeth As String = ""
        Dim shipping As Decimal = 0
        Dim oNotes As String = ""
        Dim omailTo As String = ""
        Dim dtWanted As String = ""
        Dim Mysession As String = ""
        Dim pack As Decimal = 0
        Dim tax As Decimal = 0
        Dim HeaderText As New StringBuilder

        Try
            Using rs As IDataReader = db.ExecuteReader(dbCommand)

                While rs.Read()
                    shipping = rs("shipping")
                    dMeth = rs("deliver_type").ToString()
                    omailTo = rs("email").ToString()
                    oNotes = rs("notes").ToString()
                    pack = rs("package")
                    dtWanted = rs("date_wanted").ToString()
                    tax = rs("tax")
                    Mysession = rs("cartID").ToString()

                    If rs("ship_type") <> "" Then
                        shipMeth = " (" & rs("ship_type") & ")"
                    End If

                    If i = 0 Then
                        'Build Header Shipping/Customer ***************************
                        HeaderText.Append("<h2>Thank you for your order!</h2>")
                        HeaderText.Append("<div class='col-md-6'>")
                        HeaderText.Append("<h4 style='margin-bottom: 3px'>Bill To: <br>" & rs("first_name").ToString() & " " & rs("last_name").ToString() & "</h4>")
                        HeaderText.Append("<div><b>" & rs("address1").ToString() & "</b></div>")
                        HeaderText.Append("<div><b>" & rs("city").ToString() & ", " & rs("state").ToString() & " " & rs("zip").ToString() & "</b></div>")
                        HeaderText.Append("<div><b>Phone:</b> " & Common.parse_phone(rs("home_phone").ToString()) & "</div>")
                        HeaderText.Append("<div><a href='mailto: " & omailTo & "'>" & omailTo & "</a></div>")
                        HeaderText.Append("</div>")

                        HeaderText.Append("<div class='col-md-6'>")
                        HeaderText.Append("<h4 style='margin-bottom: 3px'>Ship To: <br>" & rs("ship_name").ToString() & "</h4>")
                        HeaderText.Append("<div><b>" & rs("ship_address1").ToString() & "</b></div>")
                        HeaderText.Append("<div><b>" & rs("ship_city").ToString() & ", " & rs("ship_state").ToString() & " " & rs("ship_zip").ToString() & "</b></div>")

                        HeaderText.Append("</div>")

                        html.Append(HeaderText.ToString())
                        html.Append("<table border='0' class='table'>")
                        html.Append("<thead>")
                        html.Append("<tr>")
                        html.Append("<th>Size</th>")
                        html.Append("<th>Product</th>")
                        html.Append("<th>Price</th>")
                        html.Append("<th>Qty</th>")
                        html.Append("<th>Ext. Price</th>")
                        html.Append("</tr></thead>" & vbCrLf)

                    End If

                    html.Append("<tr><td align=center>" & rs("ProductSize") & " in.</td>")
                    html.Append("<td align=left>" & rs("ProductName") & "</td>")
                    html.Append("<td align=right>" & Common.MyNumber(rs("price")) & "</td>")
                    html.Append("<td align=center>" & rs("qty") & "</td>")
                    html.Append("<td align=right><b>" & Common.MyNumber(rs("total")) & "</b></td>")
                    html.Append("</tr>")

                    oTotal = oTotal + rs("total")
                    uCnt = uCnt + rs("qty")

                    i += 1
                End While

                If i >= 1 Then
                    oDue = oTotal + shipping + pack + tax
                    html.Append("<tr><td colspan=5><hr></td></tr>")
                    html.Append("<tr class=pHeader><td colspan=3 align=right><b>Product Total:</b></td><td align=center><b>" & uCnt & "</b></td><td align=right><b>" & Common.MyNumber(oTotal) & "</b></td></tr>")
                    html.Append("<tr class=pHeader><td colspan=3 align=right><b>Shipping:" & shipMeth & "</b></td><td>&nbsp;</td><td align=right><b>" & Common.MyNumber(shipping) & "</b></td></tr>")
                    html.Append("<tr class=pHeader><td colspan=3 align=right><b>Packaging:</b></td><td>&nbsp;</td><td align=right><b>" & Common.MyNumber(pack) & "</b></td></tr>")
                    html.Append("<tr class=pHeader><td colspan=3 align=right><b>Tax:</b></td><td>&nbsp;</td><td align=right><b>" & Common.MyNumber(tax) & "</b></td></tr>")
                    html.Append("<tr class=pHeader><td colspan=3 align=right><b>Order Total:</b></td><td>&nbsp;</td><td align=right><b>" & Common.MyNumber(oDue) & "</b></td></tr>")
                    html.Append("<tr height=3px><td colspan=5>&nbsp;</td></tr>")
                    'html.Append("<tr><td colspan=5 style='padding:7px'><b>Date Requested:</b> " & Common.CaptureNull(dtWanted, "", "date") & "</td></tr>")
                    html.Append("<tr><td colspan='5' style='padding:10px'><hr><b>Notes/Instructions:</b> " & oNotes & "<hr></td></tr>")
                    html.Append("<tr><td colspan='5' style='padding:10px; background-color: #f2f2f2'><h3><b>Online Order ID Reference: " & Mysession & "  Delivery Type: " & dMeth & "</b></h3></td></tr>")
                    html.Append("</table>")
                Else
                    html.Append("<div class='error'>Currently, no active orders matching this session...</div>")
                End If

                Try
                    Dim MailRet As String = SendCheckoutEmail(omailTo, html.ToString())
                Catch mx As Exception
                    Logger.Error(mx, "Checkout Functions: " & Reflection.MethodBase.GetCurrentMethod.Name & " - " & mx.Message.ToString())
                    Return "Mail Error: " & mx.Message.ToString & Common.CatchInnerException(mx.InnerException)
                End Try

            End Using

        Catch ex As Exception
            Logger.Error(ex, "Checkout Functions: " & Reflection.MethodBase.GetCurrentMethod.Name & " - " & ex.Message.ToString())
            Return "Error: " & ex.Message.ToString & Common.CatchInnerException(ex.InnerException)
        End Try

        Return html.ToString
        '////////////////////////////////////////////////////////////
    End Function

    Function SendCheckoutEmail(ByVal Email As String, ByVal EmailText As String) As String
        Dim sMail As New Sendmail
        With sMail
            .mailTo = Email
            .HTMLBodyText = EmailText
            .mailSubject = "Pie Gourmet Order Confirmation"
            .mailBCC = "shop@piegourmet.com"
            .PrintLegal = False
            .useQueue = True
            .SendEmail()
        End With

        Return sMail.ErrorCode.ToString()
    End Function

    Function SendErrorMessage(ByVal Email As String, ByVal EmailText As String) As String
        Dim sMail As New Sendmail
        With sMail
            .mailTo = Email
            .HTMLBodyText = EmailText
            .mailSubject = "Pie Gourmet Order Error"
            .PrintLegal = False
            .useQueue = True
            .SendEmail()
        End With

        Return sMail.ErrorCode.ToString()
    End Function
End Class
