Imports System.Data
Imports System.Data.Common
Imports Microsoft.Practices.EnterpriseLibrary.Data

Partial Class Dashboard
    Inherits System.Web.UI.Page

    Public Shared factory As DatabaseProviderFactory = New DatabaseProviderFactory()
    Public Shared db As Database = factory.CreateDefault()
    Public PageFolder As String = ConfigurationManager.AppSettings("PageFolder")

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        AjaxPro.Utility.RegisterTypeForAjax(GetType(Dashboard))
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

        If Not Session("AdminLoggedIn") Then
            Response.Redirect("Admin.aspx")
        Else
            OrderHistory.InnerHtml = OrderSummaryReport()
            OrderTable.InnerHtml = PendingOrdersList()
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

    Function OrderSummaryReport() As String
        'Product Order Summary List //////////////////////////////////
        Dim sqlCommand As String = "EXEC orderSummary_report 2,'ProductName', ''," & Common.SQLEncode(Year(Date.Today))
        Dim html As New StringBuilder
        Dim dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)
        Dim i As Integer = 0

        Try
            Using rs As IDataReader = db.ExecuteReader(dbCommand)

                While rs.Read()
                    If i = 0 Then
                        html.Append("<div class=""products-navigation__title""><h3>" & Year(Date.Today) & " <span class=""light"">Order Summary</span> </h3></div>")
                        html.Append("<table class='table table-theme table-striped'>")
                        html.Append("<thead><tr><th>Orders</th><th>Product</th></tr></thead>")

                    End If

                    html.Append("<tr>")
                    html.Append("<td><b>" & rs("cnt").ToString() & "</b></td>")
                    html.Append("<td>" & rs("sgroup").ToString() & "</td>")
                    html.Append("</tr>")
                    i += 1
                End While

                If i >= 1 Then
                    html.Append("</table>")
                End If

            End Using

        Catch ex As Exception
            Return ex.Message.ToString
        End Try

        Return html.ToString
        '////////////////////////////////////////////////////////////
    End Function

    <AjaxPro.AjaxMethod()>
    Function PendingOrdersList() As String
        Dim uCheck As UserFactory.MyUser = UserFactory.GetCurrentUser()

        'Pending Order Detail List //////////////////////////////////
        Dim sqlCommand As String = "SELECT * FROM order_summary WHERE cartStatus = 'Pending' ORDER BY create_date ASC"
        Dim html As New StringBuilder
        Dim dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)
        Dim i As Integer = 0

        Try
            Using rs As IDataReader = db.ExecuteReader(dbCommand)

                While rs.Read()
                    If i = 0 Then
                        html.Append("<div class=""products-navigation__title""><h3>Pending <span class=""light"">Order List</span> <span style='margin-left: 10px' class='error' id='ProdSelectCnt'></span></h3></div>")
                        html.Append("<table class='table table-small table-striped'>")
                        html.Append("<thead><tr>")
                        html.Append("<th><input type='checkbox' name='remopen' id='remopen' value='1' onclick=""checkOpen(this)"" title='Click to select all...'></th>")
                        html.Append("<th><b>ID</b></th><th><b>Order</b></th><th><b>Cust</b></th><th><b>Status</b></th><th><b>Customer</b></th><th><b>City/St</b></th><th><b>Method</b></th><th><b>Paid</b></th><th><b>Total</b></th><th><b>Notes</b></th><th><b>Ordered</b></th>")
                        html.Append("</tr></thead>")

                    End If

                    html.Append("<tr>")
                    html.Append("<td><input type='checkbox' name='remchecked' id='c" & rs("cartID") & "' value='" & rs("cartID") & "' onclick=""SelectItem(this)"" /></td>")
                    html.Append("<td style='text-align:center'><b>" & rs("cartID").ToString() & "</b></td>")
                    html.Append("<td><a href='javascript:void(0)' onclick=""PrintCartItem(" & rs("cartID") & ")"" data-toggle=""tooltip"" title='" & rs("Products").ToString() & "'><i class='glyphicon glyphicon-list-alt'></i></a></td>")
                    html.Append("<td><a href='javascript:void(0)' onclick=""EditCustomer(" & rs("customerID") & ")"" ' title='click to view customer details...'><i class='glyphicon glyphicon-user'></i></a></td>")

                    html.Append("<td><span class='ticket ticket-info'>" & rs("cartStatus") & "</span></td>")
                    html.Append("<td><b>" & rs("cust_name").ToString() & "</b></td>")
                    html.Append("<td>" & rs("ship_city").ToString() & ", " & rs("ship_state").ToString() & "</td>")
                    If rs("deliver_type").ToString() = "PICKUP" Then
                        html.Append("<td><span class='ticket ticket-warning'>" & rs("deliver_type").ToString() & "</span></td>")
                    Else
                        html.Append("<td><span class='ticket ticket-default'>" & rs("deliver_type").ToString() & "</span></td>")
                    End If

                    If String.IsNullOrEmpty(rs("AuthCode").ToString()) Then
                        html.Append("<td><span class='ticket ticket-danger'>UN-PAID</span></td>")
                    Else
                        Dim AuthStr As String = "Auth Code: " & rs("AuthCode").ToString() & " - " & rs("PaymentNote").ToString()
                        html.Append("<td><span class='ticket ticket-success' title='" & AuthStr & "'>PAID</span></td>")
                    End If

                    html.Append("<td align=right>" & Common.MyNumber(rs("order_total")) & "</td>")
                    html.Append("<td>" & Common.TrimString(rs("notes").ToString(), 50) & "</td>")
                    html.Append("<td>" & rs("create_date") & "</td>")
                    html.Append("</tr>")
                    i += 1
                End While

                If i >= 1 Then
                    html.Append("<tfoot><tr><th colspan='11'>Total Pending Orders: " & i & "</th></tr></tfoot>")
                    html.Append("</table>")
                Else
                    html.Append("<div class='error'>Currently, no Pending orders...</div>")
                End If

            End Using

        Catch ex As Exception
            Return ex.Message.ToString
        End Try

        Return html.ToString
        '////////////////////////////////////////////////////////////
    End Function

    <AjaxPro.AjaxMethod()>
    Function BuildCartItem(ByVal CartID As Integer) As String
        Dim uCheck As UserFactory.MyUser = UserFactory.GetCurrentUser()
        'Pending Order Detail List //////////////////////////////////
        Dim sqlCommand As String = "SELECT * FROM cart_detail WHERE cartID = " & CartID
        Dim html As New StringBuilder
        Dim dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)
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
        Dim pack As Decimal = 0
        Dim tax As Decimal = 0
        Dim HeaderText As New StringBuilder
        Dim AuthStr As String = ""

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

                    If rs("ship_type") <> "" Then
                        shipMeth = " (" & rs("ship_type") & ")"
                    End If

                    If i = 0 Then
                        'Build Header Shipping/Customer ***************************
                        HeaderText.Append("<div class='col-md-6'>")
                        HeaderText.Append("<h4 style='margin-bottom: 3px'>Bill To: <br>" & rs("first_name").ToString() & " " & rs("last_name").ToString() & "</h4>")
                        HeaderText.Append("<div><b>" & rs("address1").ToString() & "</b></div>")
                        HeaderText.Append("<div><b>" & rs("city").ToString() & ", " & rs("state").ToString() & " " & rs("zip").ToString() & "</b></div>")
                        HeaderText.Append("<div><b>Phone:</b> " & Common.parse_phone(rs("home_phone").ToString()) & "</div>")
                        HeaderText.Append("<div><a href='mailto: " & omailTo & "'>" & omailTo & "</a></div>")
                        HeaderText.Append("</div>")

                        HeaderText.Append("<div class='col-md-6'>")
                        HeaderText.Append("<h4 style='margin-bottom: 3px'>Ship To: <br>" & rs("ship_name").ToString() & "</h4>")
                        HeaderText.Append("<div>Phone: " & rs("ship_phone").ToString() & "</div>")
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

                        AuthStr = "Auth Code: " & rs("AuthCode").ToString() & " - " & rs("PaymentNote").ToString()
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
                    html.Append("<tr><td colspan=5 style='padding:7px'><b>Date Requested:</b> " & Common.CaptureNull(dtWanted, "", "date") & "</td></tr>")
                    html.Append("<tr><td colspan=5 style='padding:7px'><b>Notes/Instructions:</b> " & oNotes & "</td></tr>")
                    html.Append("<tr><td colspan=5 style='padding:7px'><b>Payment Info:</b> " & AuthStr & "</td></tr>")
                    html.Append("<tr class=pHeader><td colspan=5 bgcolor=#cccccc align=left><b>Order ID: " & CartID & " | Delivery Type: " & dMeth & "</b></td></tr>")
                    html.Append("</table>")
                Else
                    html.Append("<div class='error'>Currently, no Pending orders...</div>")
                End If

            End Using

        Catch ex As Exception
            Return ex.Message.ToString
        End Try

        Return html.ToString
        '////////////////////////////////////////////////////////////
    End Function

    <AjaxPro.AjaxMethod()>
    Function BatchProcess(ByVal BatchList As String, ByVal username As String) As String
        Dim uCheck As UserFactory.MyUser = UserFactory.GetCurrentUser()
        Dim sqlCommand As String = "DeliverBatchCheck"
        Dim i As Integer = 0
        Try
            Using dbCommand As DbCommand = db.GetStoredProcCommand(sqlCommand)
                db.AddInParameter(dbCommand, "BatchList", DbType.String, BatchList)
                db.AddInParameter(dbCommand, "username", DbType.String, username)

                i = db.ExecuteNonQuery(dbCommand)
                Return "0|" & i & " Records succssfully processed..."
            End Using
        Catch ex As Exception
            Return "1|" & ex.Message.ToString
        End Try
    End Function

    <AjaxPro.AjaxMethod()>
    Function BatchProcessCancel(ByVal BatchList As String, ByVal username As String) As String
        Dim uCheck As UserFactory.MyUser = UserFactory.GetCurrentUser()
        Dim sqlCommand As String = "CancelBatchCheck"
        Dim i As Integer = 0
        Try
            Using dbCommand As DbCommand = db.GetStoredProcCommand(sqlCommand)
                db.AddInParameter(dbCommand, "BatchList", DbType.String, BatchList)
                db.AddInParameter(dbCommand, "username", DbType.String, username)

                i = db.ExecuteNonQuery(dbCommand)
                Return "0|" & i & " Records succssfully cancelled..."
            End Using
        Catch ex As Exception
            Return "1|" & ex.Message.ToString
        End Try
    End Function
End Class
