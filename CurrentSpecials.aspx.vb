Imports System.Data
Imports System.Data.Common
Imports Microsoft.Practices.EnterpriseLibrary.Data
Imports System.IO

Partial Class CurrentSpecials
    Inherits System.Web.UI.Page

    Public Shared factory As DatabaseProviderFactory = New DatabaseProviderFactory()
    Public Shared db As Database = factory.CreateDefault()
    Public PageFolder As String = ConfigurationManager.AppSettings("PageFolder")

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        AjaxPro.Utility.RegisterTypeForAjax(GetType(menu))
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

        Dim q As String = Left(Request.QueryString("q"), 10)

        'If Not mySession.AccessCheck(Session("LoggedIn"), "ALL", "EXEC,ADMIN") Then
        '    Response.Redirect("Default.aspx?e=Page Access Not Allowed")
        '    Exit Sub
        'End If

        'Load Specials List
        SpecialsText.InnerHtml = LoadPageContent("Specials")
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

    Function ShowCateogryList() As String
        Dim sqlCommand As String = "SELECT * FROM [lookups] WHERE [look_type] = 'prod_category' ORDER BY [number]"
        Dim html As New StringBuilder
        Dim dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)
        Dim i As Integer = 0

        Try
            Using rs As IDataReader = db.ExecuteReader(dbCommand)

                While rs.Read()
                    html.Append("<li><a data-target='." & rs("look_value").ToString() & "' class=""js--filter-selectable"" href=""#"">" & rs("valuetext").ToString() & "</a></li>")
                End While

            End Using

        Catch ex As Exception
            Return ex.Message.ToString
        End Try

        Return html.ToString
    End Function

    <AjaxPro.AjaxMethod()>
    Function ShowFullProductList(ByVal Category As String) As String
        'Fresh Product List //////////////////////////////////
        Dim sqlCommand As String = "SELECT * FROM [products_view]"

        If Category = "cheese" Then
            sqlCommand &= " WHERE ProductSize = 8 AND active = 1 AND Fresh = 0"
        Else
            sqlCommand &= " WHERE ProductSize = 10 AND active = 1 AND Fresh = 0"
        End If

        If Category <> "" Then
            sqlCommand &= " AND CategoryClass = '" & Common.SQLEncode(Category) & "'"
        End If

        sqlCommand &= " ORDER BY ProductName"

        Dim html As New StringBuilder
        Dim dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)
        Dim i As Integer = 0

        Try
            Using rs As IDataReader = db.ExecuteReader(dbCommand)

                While rs.Read()
                    If i = 0 Then
                        html.Append("<div class='col-sm-2  col-sm-offset-1'>")

                    ElseIf i Mod 5 = 0 Then
                        html.Append("</div>")
                        html.Append("<div class='row'>")
                        html.Append("<div class='col-sm-2 col-sm-offset-1'>")
                    Else
                        html.Append("<div class='col-sm-2'>")
                    End If

                    'Main Product Single Item *******************
                    html.Append("<center>")
                    html.Append("<a href='images/products/" & rs("ProductImage").ToString & "' class='fancybox'>")
                    html.Append("<img alt=""#"" class=""img-responsive"" src=""images/products/" & rs("ProductImage").ToString & "?width=168"">")
                    html.Append("</a>")
                    html.Append("<div class='menu-label'>" & rs("ProductSize").ToString() & " inch: " & Common.MyNumber(rs("ProductPrice")) & "&nbsp;")

                    If rs("InStock") Then
                        html.Append("<a href=""javascript:void(0)"" title='Add to cart' onclick=""AddToCart(" & rs("productID") & "," & rs("pricingID") & ")""><span class=""glyphicon glyphicon-shopping-cart""></span></a>")
                    End If

                    html.Append("</div>")
                    html.Append("<h5><a href=""ShopDetail.aspx?q=" & rs("productID") & """ title='Click for more detail...'>" & rs("ProductName").ToString & "</a></h5>")
                    html.Append("<div class='menu-detail'>" & rs("ProductDescription").ToString & "</div>")


                    html.Append("</center>")
                    html.Append("</div>")

                    i += 1
                End While

            End Using

        Catch ex As Exception
            Return ex.Message.ToString
        End Try

        Return html.ToString
        '////////////////////////////////////////////////////////////
    End Function

    <AjaxPro.AjaxMethod()>
    Function SeasonalProductList(ByVal Category As String) As String
        'Fresh Product List //////////////////////////////////
        Dim sqlCommand As String = "SELECT * FROM [products_view] WHERE ProductSize = 10 AND active = 1 AND fresh = 1"

        If Category <> "" Then
            sqlCommand &= " AND CategoryClass = '" & Common.SQLEncode(Category) & "'"
        End If

        sqlCommand &= " ORDER BY ProductName"

        Dim html As New StringBuilder
        Dim dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)
        Dim i As Integer = 0

        Try
            Using rs As IDataReader = db.ExecuteReader(dbCommand)

                While rs.Read()
                    If i = 0 Then
                        html.Append("<div class='col-sm-2  col-sm-offset-1'>")

                    ElseIf i Mod 5 = 0 Then
                        html.Append("</div>")
                        html.Append("<div class='row'>")
                        html.Append("<div class='col-sm-2 col-sm-offset-1'>")
                    Else
                        html.Append("<div class='col-sm-2'>")
                    End If

                    'Main Product Single Item *******************
                    html.Append("<center>")
                    html.Append("<a href='images/products/" & rs("ProductImage").ToString & "' class='fancybox'>")
                    html.Append("<img alt=""#"" class=""img-responsive"" src=""images/products/" & rs("ProductImage").ToString & """>")
                    html.Append("</a>")
                    html.Append("<div class='menu-label'>" & rs("ProductSize").ToString() & " inch: " & Common.MyNumber(rs("ProductPrice")) & "&nbsp;")

                    If rs("InStock") Then
                        html.Append("<a href=""javascript:void(0)"" title='Add to cart' onclick=""AddToCart(" & rs("productID") & "," & rs("pricingID") & ")""><span class=""glyphicon glyphicon-shopping-cart""></span></a>")
                    End If

                    html.Append("</div>")
                    html.Append("<h5><a href=""ShopDetail.aspx?q=" & rs("productID") & """ title='Click for more detail...'>" & rs("ProductName").ToString & "</a></h5>")
                    html.Append("<div class='menu-detail'>" & rs("ProductDescription").ToString & "</div>")
                    html.Append("</center>")
                    html.Append("</div>")

                    i += 1
                End While
            End Using

        Catch ex As Exception
            Return ex.Message.ToString
        End Try

        Return html.ToString
        '////////////////////////////////////////////////////////////
    End Function
End Class