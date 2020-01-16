Imports System.Data
Imports System.Data.Common
Imports System.Data.SqlClient
Imports Microsoft.Practices.EnterpriseLibrary.Data

Partial Class ProductDetail
    Inherits System.Web.UI.Page

    Public Shared factory As DatabaseProviderFactory = New DatabaseProviderFactory()
    Public Shared db As Database = factory.CreateDefault()

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        AjaxPro.Utility.RegisterTypeForAjax(GetType(ProductDetail))
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

        Dim q As String = Request.QueryString("q")

        If Not Session("AdminLoggedIn") Then
            Response.Redirect("Default.aspx")
        Else
            ProductListEdit.InnerHtml = GetProductList("")

            'Dropdowns
            DropDowns.LookupsDropdown("", epricing_group, "price_group")
            DropDowns.LookupsDropdown("", pSchemeFilter, "price_group")
            pSchemeFilter.Attributes.Add("onchange", "List_pricing()")

            DropDowns.LookupsDropdown("", eCategory, "prod_category")
            DropDowns.IngredientList(0, eIngredientsID)
            DropDowns.NumberRangeSelect(4, eRating, 5, 1)
        End If

        If q <> "" Then
            mySession.AddElement("LastProduct", q)
            LoadProduct(q)
        Else
            If LastProduct >= 1 Then
                LoadProduct(LastProduct)
            End If
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
    Function GetProductList(ByVal NameFilter As String) As String
        Dim sqlCommand As String = "SELECT * FROM products"

        If NameFilter <> "" Then
            sqlCommand &= " WHERE ProductName LIKE '" & Common.SQLEncode(NameFilter) & "%'"
        End If

        sqlCommand &= " ORDER BY ProductName"

        Dim dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)
        Dim html As New StringBuilder
        Dim i As Integer = 0

        Try
            Using rs As IDataReader = db.ExecuteReader(dbCommand)

                While rs.Read()

                    If i = 0 Then
                        html.Append("<table class='table table-theme table-striped'>")
                        html.Append("<thead>")
                        html.Append("<tr><th>Product</th><th>Active</th></tr>")
                        html.Append("</thead>")
                    End If


                    html.Append("<td><a href='ProductDetail.aspx?q=" & rs("productID").ToString() & "'>" & rs("ProductName").ToString() & "</a></td>")
                    If rs("active") Then
                        html.Append("<td><span class=""in-stock"">&bull;</span></td>")
                    Else
                        html.Append("<td><span class=""out-of-stock"">&bull;</span></td>")
                    End If

                    html.Append("</tr>")
                    i += 1
                End While

                If i >= 1 Then
                    html.Append("</table>")
                Else
                    html.Append("<div class='alert  alert-warning  uppercase'>No Products... Better Start Baking!</div>")
                End If


            End Using
        Catch ex As Exception
            Return "Error: " & ex.Message.ToString
        End Try

        Return html.ToString()
    End Function

    <AjaxPro.AjaxMethod()>
    Function GetProductListQuick(ByVal NameFilter As String) As String
        Dim sqlCommand As String = "SELECT * FROM products"

        Select Case NameFilter
            Case "Seasonal"
                sqlCommand &= " WHERE Holiday = 1"
            Case "Active"
                sqlCommand &= " WHERE Active = 1"
        End Select

        sqlCommand &= " ORDER BY ProductName"

        Dim dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)
        Dim html As New StringBuilder
        Dim i As Integer = 0

        Try
            Using rs As IDataReader = db.ExecuteReader(dbCommand)

                While rs.Read()

                    If i = 0 Then
                        html.Append("<table class='table table-theme table-striped'>")
                        html.Append("<thead>")
                        html.Append("<tr><th>Product</th><th>Active</th></tr>")
                        html.Append("</thead>")
                    End If


                    html.Append("<td><a href='ProductDetail.aspx?q=" & rs("productID").ToString() & "'>" & rs("ProductName").ToString() & "</a></td>")
                    If rs("active") Then
                        html.Append("<td><span class=""in-stock"">&bull;</span></td>")
                    Else
                        html.Append("<td><span class=""out-of-stock"">&bull;</span></td>")
                    End If

                    html.Append("</tr>")
                    i += 1
                End While

                If i >= 1 Then
                    html.Append("</table>")
                Else
                    html.Append("<div class='alert  alert-warning  uppercase'>No Products... Better Start Baking!</div>")
                End If


            End Using
        Catch ex As Exception
            Return "Error: " & ex.Message.ToString
        End Try

        Return html.ToString()
    End Function

    Sub LoadProduct(ByVal ProductID As Integer)
        'Fresh Product List //////////////////////////////////
        Dim sqlCommand As String = "SELECT * FROM [products_view] WHERE productID = " & ProductID & " ORDER BY ProductSize DESC"
        Dim html As New StringBuilder
        Dim RatingText As New StringBuilder
        Dim dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)
        Dim i As Integer = 0
        Dim Category As String = ""

        Try
            Using rs As IDataReader = db.ExecuteReader(dbCommand)

                While rs.Read()
                    If i = 0 Then
                        Category = rs("Category").ToString
                        MyProductID.Value = rs("ProductID")
                        ProductName.InnerHtml = rs("ProductName").ToString
                        ProductCategory.InnerHtml = rs("Category").ToString
                        ProductDescription.InnerHtml = rs("ProductDescription").ToString
                        ProductPrice.InnerHtml = Common.MyNumber(rs("ProductPrice"))
                        ProductImage.Src = "images/products/" & rs("ProductImage").ToString
                        ProductDescription2.InnerHtml = rs("ProductDescription").ToString

                        If String.IsNullOrEmpty(rs("Ingredients").ToString()) Then
                            Indredients.InnerHtml = "No ingredients assigned..."
                        Else
                            Indredients.InnerHtml = rs("Ingredients").ToString()
                        End If

                        'Rating Stars **********************************************************************
                        Dim AvgRating As Integer = rs("AvgRating")
                        If AvgRating = 0 Then
                            For x = 1 To 5
                                RatingText.Append("<span class=""glyphicon glyphicon-star star-off""></span>&nbsp;")
                            Next
                        Else
                            For x = 1 To AvgRating
                                RatingText.Append("<span class=""glyphicon glyphicon-star star-on""></span>&nbsp;")
                            Next

                            For y = 1 To 5 - AvgRating
                                RatingText.Append("<span class=""glyphicon glyphicon-star star-off""></span>&nbsp;")
                            Next
                        End If

                        ProductRating.InnerHtml = RatingText.ToString()


                        html.Append("<option value='" & rs("pricingID").ToString & "'>" & rs("ProductSize").ToString & " inch " & rs("TypeCategory").ToString & " - " & Common.MyNumber(rs("ProductPrice")) & "</option>")

                        If rs("Active") Then
                            Active.InnerHtml = "<span class=""in-stock"">&bull;</span> <span class=""in-stock--text"">Active</span>"
                        Else
                            Active.InnerHtml = "<span class=""out-of-stock"">&bull;</span> <span class=""in-stock--text"">In-Active</span>"
                        End If

                        If rs("InStock") Then
                            InStock.InnerHtml = "<span class=""in-stock"">&bull;</span> <span class=""in-stock--text"">In stock</span>"
                        Else
                            InStock.InnerHtml = "<span class=""out-of-stock"">&bull;</span> <span class=""in-stock--text"">Out of stock</span>"
                        End If

                        If rs("Holiday") Then
                            Holiday.InnerHtml = "<span class=""in-stock"">&bull;</span> <span class=""in-stock--text"">Seasonal</span>"
                        Else
                            Holiday.InnerHtml = "<span class=""out-of-stock"">&bull;</span> <span class=""in-stock--text"">Not Seasonal</span>"
                        End If

                        If rs("Featured") Then
                            Featured.InnerHtml = "<span class=""in-stock"">&bull;</span> <span class=""in-stock--text"">Featured</span>"
                        Else
                            Featured.InnerHtml = "<span class=""out-of-stock"">&bull;</span> <span class=""in-stock--text"">Not Featured</span>"
                        End If

                    Else
                        html.Append("<option value='" & rs("pricingID").ToString & "'>" & rs("ProductSize").ToString & " inch " & rs("TypeCategory").ToString & " - " & Common.MyNumber(rs("ProductPrice")) & "</option>")
                    End If

                    i += 1

                    ShowComments(ProductID)
                End While

                ProductTypeSelect.Text = "<select class=""btn btn-shop"" id=""ProductPricingID"" onchange=""ChangeProductPrice(this)"">" & html.ToString & "</select>"


            End Using

        Catch ex As Exception
            Dim ValArray As New StringDictionary
            ValArray.Add("ErrStr", ex.Message.ToString)
            WriteClientVals(ValArray)
        End Try
    End Sub

    <AjaxPro.AjaxMethod()>
    Function GetOrderHistory(ByVal ProductID As Integer, ByVal GetYear As Integer) As String
        Dim sqlCommand As String = "GetProductOrderHistory"
        Dim html As New StringBuilder
        Dim dbCommand As DbCommand = db.GetStoredProcCommand(sqlCommand)
        db.AddInParameter(dbCommand, "productID", DbType.Int32, ProductID)
        db.AddInParameter(dbCommand, "GetYear", DbType.Int32, GetYear)

        Dim i As Integer = 0

        Try
            Using rs As IDataReader = db.ExecuteReader(dbCommand)
                While rs.Read()
                    html.Append(rs(0).ToString())
                End While
            End Using
        Catch ex As Exception
            Return "Error: " & ex.Message.ToString()
        End Try

        Return html.ToString()
    End Function

    Public Class products_Class
        Public productID As Integer = 0
        Public weight As Integer = 0
        Public pricing_group As Integer = 0
        Public IngredientsID As Integer = 0
        Public Category As String = ""
        Public ProductName As String = ""
        Public ProductDescription As String = ""
        Public ProductImage As String = ""
        Public active As Integer = 1
        Public InStock As Integer = 0
        Public Fresh As Integer = 0
        Public Featured As Integer = 0
        Public Rating As Integer = 0
        Public create_user As String = ""
    End Class

    <AjaxPro.AjaxMethod()>
    Function GetproductsClass() As products_Class
        Dim cd As products_Class = New products_Class
        Return cd
    End Function

    <AjaxPro.AjaxMethod()>
    Function Getproducts(ByVal productID As Integer) As DataSet
        Dim sqlCommand As String = "SELECT * FROM products WHERE productID = " & productID
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
    Function Process_products(ByVal mc As products_Class) As String
        Dim sqlCommand As String = "process_products"
        Dim i As Integer = 0
        Try
            Using dbCommand As DbCommand = db.GetStoredProcCommand(sqlCommand)
                db.AddInParameter(dbCommand, "productID", DbType.Int32, mc.productID)
                db.AddInParameter(dbCommand, "weight", DbType.Int32, mc.weight)
                db.AddInParameter(dbCommand, "pricing_group", DbType.Int32, mc.pricing_group)
                db.AddInParameter(dbCommand, "IngredientsID", DbType.Int32, mc.IngredientsID)
                db.AddInParameter(dbCommand, "Category", DbType.String, mc.Category)
                db.AddInParameter(dbCommand, "ProductName", DbType.String, mc.ProductName)
                db.AddInParameter(dbCommand, "ProductDescription", DbType.String, mc.ProductDescription)
                db.AddInParameter(dbCommand, "active", DbType.Boolean, mc.active)
                db.AddInParameter(dbCommand, "InStock", DbType.Boolean, mc.InStock)
                db.AddInParameter(dbCommand, "Fresh", DbType.Boolean, mc.Fresh)
                db.AddInParameter(dbCommand, "Featured", DbType.Boolean, mc.Featured)
                db.AddInParameter(dbCommand, "Rating", DbType.Int32, mc.Rating)
                db.AddInParameter(dbCommand, "create_user", DbType.String, mc.create_user)
                db.AddOutParameter(dbCommand, "retval", DbType.Int32, 4)
                i = db.ExecuteNonQuery(dbCommand)
                Dim retval As Integer = db.GetParameterValue(dbCommand, "retval")
                Return "0|" & retval.ToString
            End Using
        Catch ex As Exception
            Return "1|" & ex.Message.ToString
        End Try
    End Function

    <AjaxPro.AjaxMethod()>
    Function Deleteproducts(ByVal productID As Integer) As String
        Dim sqlCommand As String = "DELETE FROM products WHERE productID = " & productID
        Dim i As Integer = 0
        Try
            Using dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)
                i = db.ExecuteNonQuery(dbCommand)
                Return i.ToString
            End Using
        Catch ex As Exception
            Return ex.Message.ToString
        End Try
    End Function


    Sub ShowComments(ByVal ProductID As Integer)
        'Fresh Product List //////////////////////////////////
        Dim sqlCommand As String = "SELECT * FROM product_comments WHERE productID = '" & ProductID & "' ORDER BY create_date DESC"
        Dim html As New StringBuilder
        Dim dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)
        Dim i As Integer = 0

        Try
            Using rs As IDataReader = db.ExecuteReader(dbCommand)

                While rs.Read()

                    'Comment List *******************

                    html.Append("<p class=""tab-text"">")
                    html.Append("<h5>")

                    html.Append("<div style='margin-righ: 6px'>" & GlobalAjax.GetRatingStars(rs("rating")) & "</div>")

                    html.Append(rs("City").ToString() & ", " & rs("State").ToString() & " <span class=''>(" & rs("create_date").ToString() & ")</span>")
                    html.Append("&nbsp;&nbsp;<a href='javascript:void(0)' onclick=""RemoveComment(" & rs("commentID") & ")"">Remove</a>")
                    html.Append("</h5>")
                    html.Append("<div>" & rs("comment").ToString & "</div>")

                    html.Append("</p>")
                    i += 1
                End While

                If i = 0 Then
                    html.Append("No Reviews Yet...")
                End If

                ReviewCount.InnerHtml = i.ToString()
            End Using

            ReviewText.InnerHtml = html.ToString()
        Catch ex As Exception
            ReviewText.InnerHtml = "Error: " & ex.Message.ToString()
        End Try

        '////////////////////////////////////////////////////////////
    End Sub

#Region "Pricing Form"


    Public Class pricing_Class
        Public pricingID As Integer = 0
        Public pricing_group As Integer = 0
        Public GroupName As String = ""
        Public ProductPrice As String = ""
        Public Packaging As String = ""
        Public ShipWeight As String = ""
        Public ProductSize As String = ""
        Public TypeCategory As String = ""
        Public TypeQuantity As Integer = 0
        Public create_user As String = ""
    End Class

    <AjaxPro.AjaxMethod()>
    Function GetpricingClass() As pricing_Class
        Dim cd As pricing_Class = New pricing_Class
        Return cd
    End Function

    <AjaxPro.AjaxMethod()>
    Function Getpricing(ByVal pricingID As Integer) As DataSet
        Dim sqlCommand As String = "SELECT * FROM pricing WHERE pricingID = " & pricingID
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
    Function Process_pricing(ByVal mc As pricing_Class) As String
        Dim sqlCommand As String = "process_pricing"
        Dim i As Integer = 0
        Try
            Using dbCommand As DbCommand = db.GetStoredProcCommand(sqlCommand)
                db.AddInParameter(dbCommand, "pricingID", DbType.Int32, mc.pricingID)
                db.AddInParameter(dbCommand, "pricing_group", DbType.Int32, mc.pricing_group)
                db.AddInParameter(dbCommand, "GroupName", DbType.String, mc.GroupName)
                db.AddInParameter(dbCommand, "ProductPrice", DbType.String, mc.ProductPrice)
                db.AddInParameter(dbCommand, "Packaging", DbType.String, mc.Packaging)
                db.AddInParameter(dbCommand, "ShipWeight", DbType.String, mc.ShipWeight)
                db.AddInParameter(dbCommand, "ProductSize", DbType.String, mc.ProductSize)
                db.AddInParameter(dbCommand, "TypeCategory", DbType.String, mc.TypeCategory)
                db.AddInParameter(dbCommand, "TypeQuantity", DbType.Int32, mc.TypeQuantity)
                db.AddInParameter(dbCommand, "create_user", DbType.String, mc.create_user)
                db.AddOutParameter(dbCommand, "retval", DbType.Int32, 4)
                i = db.ExecuteNonQuery(dbCommand)
                Dim retval As Integer = db.GetParameterValue(dbCommand, "retval")
                Return "0|" & retval.ToString
            End Using
        Catch ex As Exception
            Return "1|" & ex.Message.ToString
        End Try
    End Function

    <AjaxPro.AjaxMethod()>
    Function Deletepricing(ByVal pricingID As Integer) As String
        Dim sqlCommand As String = "DELETE FROM pricing WHERE pricingID = " & pricingID
        Dim i As Integer = 0
        Try
            Using dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)
                i = db.ExecuteNonQuery(dbCommand)
                Return i.ToString
            End Using
        Catch ex As Exception
            Return ex.Message.ToString
        End Try
    End Function

    <AjaxPro.AjaxMethod()>
    Function Listpricing(ByVal pGroup As Integer) As String
        Dim sqlCommand As String = "SELECT * FROM pricing WHERE pricing_group = " & pGroup
        Dim i As Integer = 0
        Dim html As New StringBuilder

        Try
            Dim dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)
            Using rs As IDataReader = db.ExecuteReader(dbCommand)

                While rs.Read()
                    If i = 0 Then
                        html.Append("<table class='table table-striped table-sm' id='pTable1'>")
                        html.Append("<thead class='header_tbl'>")
                        html.Append("<tr>")
                        html.Append("<th>Action</th>")
                        html.Append("<th>GRP</th>")
                        html.Append("<th>PRICE</th>")
                        html.Append("<th>PKG</th>")
                        html.Append("<th>WEIGHT</th>")
                        html.Append("<th>SIZE</th>")

                        html.Append("</tr></thead><tbody>")
                    End If

                    html.Append("<tr>")
                    html.Append("<td><button type='button' onclick=""Get_pricing(" & rs("pricingID").ToString & ")"" class='btn btn-info'><i class='glyphicon glyphicon-check'></i></button></td>")
                    html.Append("<td>" & rs("pricing_group").ToString & "</td>")
                    html.Append("<td>" & Common.MyNumber(rs("ProductPrice")) & "</td>")
                    html.Append("<td>" & rs("Packaging").ToString & "</td>")
                    html.Append("<td>" & rs("ShipWeight").ToString & "</td>")
                    html.Append("<td>" & rs("ProductSize").ToString & "</td>")

                    html.Append("</tr>")
                    i = i + 1
                End While
                html.Append("</tbody>")
                html.Append("</tfoot><tr><td colspan='6' class='ButtonRow'><b>Total Records: " & i.ToString & "</b></td></tr></tfoot>")
                html.Append("</table>")

                If i = 0 Then
                    html.Append("No records found for this query... Please try again.")
                End If
            End Using
        Catch ex As Exception
            Return "List Error: " & ex.Message.ToString()
        End Try
        Return html.ToString()
    End Function

#End Region
End Class
