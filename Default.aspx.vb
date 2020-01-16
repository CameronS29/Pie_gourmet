Imports System.Data
Imports System.Data.Common
Imports Microsoft.Practices.EnterpriseLibrary.Data
Imports System.IO

Partial Class _Default
    Inherits System.Web.UI.Page

    Public Shared factory As DatabaseProviderFactory = New DatabaseProviderFactory()
    Public Shared db As Database = factory.CreateDefault()
    Public PageFolder As String = ConfigurationManager.AppSettings("PageFolder")

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        AjaxPro.Utility.RegisterTypeForAjax(GetType(_Default))
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
        WriteClientVals(ValArray)

        Dim q As String = Left(Request.QueryString("q"), 25)

        'If Not mySession.AccessCheck(Session("LoggedIn"), "ALL", "EXEC,ADMIN") Then
        '    Response.Redirect("Default.aspx?e=Page Access Not Allowed")
        '    Exit Sub
        'End If

        'Load Fresh List
        'ProductCarosel.InnerHtml = ShowFreshProducts()
        'FeaturedProductList.Text = ShowFeaturedProducts()
        'SpecialsText.InnerHtml = LoadPageContent("Specials")
        'AnnouncementsText.InnerHtml = LoadPageContent("Announcements")
        'MenuText.InnerHtml = LoadPageContent("Menu")
        'ProductComments.Text = ShowProductComments()
    End Sub

    Private Sub WriteClientVals(ByVal ValueArray As StringDictionary)
        Dim sb As New StringBuilder
        Dim i As DictionaryEntry

        For Each i In ValueArray
            sb.Append("var " & i.Key & " = '" & i.Value & "';" & vbCrLf)
        Next

        Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "DashClientVals", sb.ToString, True)
    End Sub


    Function LoadPageContent(ByVal ServerPageLoad As String) As String
        Dim Content As String = ""
        If File.Exists(PageFolder & "\" & ServerPageLoad & ".htm") Then
            Dim sr As StreamReader
            sr = File.OpenText(PageFolder & "\" & ServerPageLoad & ".htm")
            Content = sr.ReadToEnd()
            sr.Close()
        Else
            Content = "<h3s>Currently no content available... Please check back later.</h3>"
        End If

        Return Content
    End Function

    Function ShowFreshProducts() As String
        'Fresh Product List //////////////////////////////////
        Dim sqlCommand As String = "SELECT TOP 20 * FROM [products_view] WHERE active = 1 AND Fresh = 1 AND ProductSize = 10"
        Dim html As New StringBuilder
        Dim dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)
        Dim i As Integer = 0

        Try
            Using rs As IDataReader = db.ExecuteReader(dbCommand)

                While rs.Read()
                    If i = 0 Then
                        html.Append("<div class=""item active"">")
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
                    html.Append("<a href=""ShopDetail.aspx?q=" & rs("productID") & """>")
                    html.Append("<img alt=""#"" class=""product__image"" width=""263"" height=""334"" src=""images/products/" & rs("ProductImage").ToString & """ />")
                    html.Append("</a>")
                    html.Append("<div class=""product-overlay"">")
                    html.Append("<a class=""product-overlay__more"" href=""ShopDetail.aspx?q=" & rs("productID") & """>")
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
                    html.Append("<div class=""products__category"">" & rs("ProductSize").ToString & " inch " & rs("Category").ToString & "</div>")
                    html.Append("</div>")
                    html.Append("</div>")
                    i += 1
                End While

                html.Append("</div></div>")
            End Using

        Catch ex As Exception
            Return "Error: " & ex.Message.ToString
        End Try

        Return html.ToString
        '////////////////////////////////////////////////////////////
    End Function

    Function ShowFeaturedProducts() As String
        'Fresh Product List //////////////////////////////////
        Dim sqlCommand As String = "SELECT * FROM [products_view] WHERE active = 1 and Featured = 1 AND ProductSize = 10"
        Dim html As New StringBuilder
        Dim dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)
        Dim i As Integer = 0

        Try
            Using rs As IDataReader = db.ExecuteReader(dbCommand)

                While rs.Read()
                    If i = 0 Then
                        html.Append("<div class=""products-navigation__title""><h3>Featured <span class=""light"">Items</span> </h3></div>")
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
                    html.Append("<a href=""single-product.html"">")
                    html.Append("<img alt=""#"" class=""product__image"" width=""263"" height=""334"" src=""images/products/" & rs("ProductImage").ToString & """>")
                    html.Append("</a>")
                    html.Append("<div class=""product-overlay"">")
                    html.Append("<a class=""product-overlay__more"" href=""ShopDetail.aspx?q=" & rs("productID") & """>")
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
                    html.Append("<div class=""products__category"">" & rs("ProductSize").ToString & " inch " & rs("Category").ToString & "</div>")
                    html.Append("</div>")
                    html.Append("</div>")
                    i += 1
                End While

                html.Append("</div></div>")
            End Using

            If i = 0 Then
                'Capture No Records
                html.Clear()
            End If

        Catch ex As Exception
            Return ex.Message.ToString
        End Try

        Return html.ToString
        '////////////////////////////////////////////////////////////
    End Function

    Function ShowProductComments() As String
        'Fresh Product List //////////////////////////////////
        Dim sqlCommand As String = "SELECT * FROM [product_comments] WHERE [rating] >= 4 AND show = 1"
        Dim html As New StringBuilder
        Dim dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)
        Dim i As Integer = 0

        Try
            Using rs As IDataReader = db.ExecuteReader(dbCommand)

                While rs.Read()
                    If i = 0 Then
                        html.Append("<div class=""carousel-inner"">")
                        html.Append("<div class=""item  active"">")
                    Else
                        html.Append("<div class=""item"">")
                    End If

                    html.Append("<q class=""testimonials__text"">" & rs("comment").ToString & "</q><br>")
                    html.Append("<cite><b>" & rs("ProductName").ToString() & "</b></cite> - " & rs("City").ToString() & ", " & rs("State").ToString())
                    html.Append("</div>")
                    i += 1
                End While
            End Using

            If i = 0 Then
                'Capture No Records
                html.Append("<div class=""carousel-inner"">")
                html.Append("<div class=""item  active"">")
                html.Append("<q class=""testimonials__text"">Please comment on our products/service</q><br>")
                html.Append("</div></div>")
            Else
                html.Append("</div>")
            End If

        Catch ex As Exception
            Return ex.Message.ToString
        End Try

        Return html.ToString
        '////////////////////////////////////////////////////////////
    End Function
End Class
