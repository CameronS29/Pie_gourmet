Imports System.Data
Imports System.Data.Common
Imports System.Data.SqlClient
Imports Microsoft.Practices.EnterpriseLibrary.Data

Partial Class ShopDetail
    Inherits System.Web.UI.Page

    Public Shared factory As DatabaseProviderFactory = New DatabaseProviderFactory()
    Public Shared db As Database = factory.CreateDefault()

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        AjaxPro.Utility.RegisterTypeForAjax(GetType(ShopDetail))
        AjaxPro.Utility.RegisterTypeForAjax(GetType(GlobalAjax))

        Dim mySession As New SessionManager
        Dim CustomerID As String = mySession.GetElement("CustomerID", "number")
        Dim Username As String = mySession.GetElement("username")
        Dim myName As String = mySession.GetElement("FullName")
        Dim MyEmail As String = mySession.GetElement("Email")

        'Write Client Values For AJAX Methods *******************************************
        Dim ValArray As New StringDictionary
        ValArray.Add("CustomerID", CustomerID)
        ValArray.Add("Username", Username)
        ValArray.Add("myName", myName)
        ValArray.Add("MyEmail", MyEmail)
        ValArray.Add("SessionID", Session.SessionID)
        ValArray.Add("ErrStr", "")
        WriteClientVals(ValArray)

        Dim q As String = Left(Request.QueryString("q"), 10)

        If IsNumeric(q) Then
            LoadProduct(q)
        Else
            Response.Redirect("Shop.aspx")
        End If

        'Load Related Products
        'ProductCarosel.InnerHtml = ShowFreshProducts()
        'FeaturedProductList.Text = ShowFeaturedProducts()
    End Sub

    Private Sub WriteClientVals(ByVal ValueArray As StringDictionary)
        Dim sb As New StringBuilder
        Dim i As DictionaryEntry

        For Each i In ValueArray
            sb.Append("var " & i.Key & " = '" & i.Value & "';" & vbCrLf)
        Next

        Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "DashClientVals", sb.ToString, True)
    End Sub

    Sub LoadProduct(ByVal ProductID As Integer)
        'Fresh Product List //////////////////////////////////
        Dim sqlCommand As String = "SELECT * FROM [products_view] WHERE productID = " & ProductID & " ORDER BY ProductSize DESC"
        Dim html As New StringBuilder
        Dim RatingText As New StringBuilder
        Dim dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)
        Dim i As Integer = 0
        Dim Category As String = ""
        Dim ProdInStock as Boolean = True

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
                        ProdImgHref.HRef = "images/products/" & rs("ProductImage").ToString
                        ProductDescription2.InnerHtml = rs("ProductDescription").ToString

                        If String.IsNullOrEmpty(rs("Ingredients").ToString()) Then
                            Indredients.InnerHtml = "All good stuff..."
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

                        If rs("InStock") Then
                            ProdInStock = True
                            InStock.InnerHtml = "<span class=""in-stock"">&bull;</span> <span class=""in-stock--text"">In stock</span>"
                        Else
                            InStock.InnerHtml = "<span class=""out-of-stock"">&bull;</span> <span class=""in-stock--text"">Out of stock</span>"
                            btnAddToCart.Visible = False
                        End If
                    Else
                        html.Append("<option value='" & rs("pricingID").ToString & "'>" & rs("ProductSize").ToString & " inch " & rs("TypeCategory").ToString & " - " & Common.MyNumber(rs("ProductPrice")) & "</option>")
                    End If

                    i += 1

                    ShowComments(ProductID)
                End While

                ProductTypeSelect.Text = "<select class=""btn btn-shop"" id=""ProductPricingID"" onchange=""ChangeProductPrice(this)"">" & html.ToString & "</select>"

                'Show Related Product List
                'RelatedProductList.Text = ShowRelatedProducts(Category)
            End Using

        Catch ex As Exception
            Dim ValArray As New StringDictionary
            ValArray.Add("ErrStr", ex.Message.ToString)
            WriteClientVals(ValArray)
        End Try
    End Sub

    Sub ShowComments(ByVal ProductID As Integer)
        'Fresh Product List //////////////////////////////////
        Dim sqlCommand As String = "SELECT * FROM product_comments WHERE show = 1 AND productID = '" & ProductID & "' ORDER BY create_date DESC"
        Dim html As New StringBuilder
        Dim dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)
        Dim i As Integer = 0

        Try
            Using rs As IDataReader = db.ExecuteReader(dbCommand)
                html.Append("<div id='ReviewList'>")
                While rs.Read()

                    'Comment List *******************

                    html.Append("<p class=""tab-text"">")
                    html.Append("<h5><div style='margin-righ: 6px'>" & GlobalAjax.GetRatingStars(rs("rating")) & "</div>" & rs("City").ToString() & ", " & rs("State").ToString() & " <span class=''>(" & rs("create_date").ToString() & ")</span></h5>")

                    html.Append("<div>" & rs("comment").ToString & "</div>")

                    html.Append("</p>")
                    i += 1
                End While

                If i = 0 Then
                    html.Append("No Reviews Yet...</div>")
                Else
                    html.Append("</div>")
                End If

                ReviewCount.innerhtml = i.ToString()
            End Using

            ReviewText.Text = html.ToString()
        Catch ex As Exception
            ReviewText.Text = "Error: " & ex.Message.ToString()
        End Try

        '////////////////////////////////////////////////////////////
    End Sub

    Function ShowRelatedProducts(ByVal Category As String) As String
        'Fresh Product List //////////////////////////////////
        Dim sqlCommand As String = "SELECT * FROM [products_view] WHERE active = 1 AND ProductSize = 8 AND Category = '" & Category & "' ORDER BY ProductName"
        Dim html As New StringBuilder
        Dim dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)
        Dim i As Integer = 0

        Try
            Using rs As IDataReader = db.ExecuteReader(dbCommand)

                While rs.Read()
                    If i = 0 Then
                        html.Append("<div class=""item"">")
                        html.Append("<div class=""row"">")
                    ElseIf i Mod 4 = 0 Then
                        html.Append("</div></div>")
                        html.Append("<div class=""item"">")
                        html.Append("<div class=""row"">")
                    End If

                    'Main Product Single Item *******************
                    html.Append("<div class=""col-xs-6 col-sm-3  js--isotope-target"" data-price=""" & rs("ProductPrice").ToString & """ data-rating=""5"">")
                    html.Append("<div class=""products__single"">")
                    html.Append("<figure class=""products__image"">")
                    html.Append("<a href=""ShopDetail.aspx?q=" & rs("productID").ToString & """>")
                    html.Append("<img alt=""#"" class=""product__image fancybox"" width=""263"" height=""334"" src=""images/products/" & rs("ProductImage").ToString & """>")
                    html.Append("</a>")
                    html.Append("<div class=""product-overlay"">")
                    html.Append("<a class=""product-overlay__more"" href=""ShopDetail.aspx?q=" & rs("productID").ToString & """>")
                    html.Append("<span class=""glyphicon glyphicon-search""></span>")
                    html.Append("</a>")
                    html.Append("<a class=""product-overlay__cart"" href=""javascript:void(0)"" onclick=""AddToCart(" & rs("productID") & "," & rs("pricingID") & ")"">+<span class=""glyphicon glyphicon-shopping-cart""></span>")
                    html.Append(" </a>")
                    html.Append("<div class=""product-overlay__stock"">")

                    If rs("InStock") Then
                        html.Append("<span class=""in-stock"">&bull;</span> <span class=""in-stock--text"">In stock</span>")
                    Else
                        html.Append("<span class=""out-of-stock"">&bull;</span> <span class=""in-stock--text"">Out of stock</span>")
                    End If

                    html.Append("</div>")
                    html.Append("</div>")
                    html.Append("</figure>")
                    html.Append("<div class=""row"">")
                    html.Append("<div class=""col-xs-9"">")
                    html.Append("<h5 class=""products__title"">")
                    html.Append("<a class=""products__link  js--isotope-title"" href=""ShopDetail.aspx?q=" & rs("productID").ToString & """>" & rs("ProductName").ToString & "</a>")
                    html.Append("</h5>")
                    html.Append("</div>")
                    html.Append("<div class=""col-xs-3"">")
                    html.Append("<div class=""products__price"">" & Common.MyNumber(rs("ProductPrice")) & "</div>")
                    html.Append("</div>")
                    html.Append("</div>")
                    html.Append("<div class=""products__category"">8 or 10 inch " & rs("Category").ToString & "</div>")
                    html.Append("</div>")
                    html.Append("</div>")
                    i += 1
                End While

                html.Append("</div></div>")
            End Using

        Catch ex As Exception
            Return ex.Message.ToString
        End Try

        Return html.ToString
        '////////////////////////////////////////////////////////////
    End Function
End Class
