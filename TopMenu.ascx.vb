Imports System.IO

Partial Class TopMenu
    Inherits System.Web.UI.UserControl
    Public PageFolder As String = ConfigurationManager.AppSettings("PageFolder")

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Get Current Page Name
        Dim PageName As String = System.IO.Path.GetFileName(Request.Url.ToString())
        Dim FindEnd As Integer = PageName.IndexOf(".aspx")
        Dim FinalName As String = ""
        If FindEnd >= 1 Then
            FinalName = PageName.Remove(FindEnd).ToLower()
        End If

        Dim mySession As New SessionManager
        Dim CustomerID As String = mySession.GetElement("CustomerID", "number")
        Dim Username As String = mySession.GetElement("Username")
        Dim myName As String = mySession.GetElement("FullName")
        Dim MyEmail As String = mySession.GetElement("Email")
        Dim Access As String = mySession.GetElement("AccessLevel")

        WriteTopMenuList(FinalName, Access)

        'Get Holiday Settings *********************************************
        Dim Holiday as String = GlobalAjax.GetApplicationSetting("Holiday")

        If Holiday <> "" Then
            SpecialMenu.Text = "<li id='lispecial'><a href=""CurrentSpecials.aspx"">" & Holiday & " Menu</a></li>"
        End If

        If Session("LoggedIn") = True Then
            MyAccountLink.InnerHtml = "<a href=""MyAccount.aspx"">Welcome <span class=""glyphicon  glyphicon-heart  tertiary-color""></span> " & myName & "</a>"
            MyAccountLogout.InnerHtml = "<a href=""Logout.aspx"">Logout</a>"
        ElseIf Session("AdminLoggedIn") = True Then
            MyAccountLink.InnerHtml = "<a href=""#"">Admin Login <span class=""glyphicon  glyphicon-heart  tertiary-color""></span> " & myName & "</a>"
            MyAccountLogout.InnerHtml = "<a href=""Logout.aspx"">Logout</a>"
        End If

        CheckSpecailPages("Announcements")
    End Sub

    Sub WriteTopMenuList(ByVal PageName As String, ByVal Access As String)
        Dim html As New StringBuilder


        'Admin Section **********************
        If Access = "ADMIN" Then
            html.Append("<li class=""dropdown"">")
            html.Append("<a href=""#"" class=""dropdown-toggle"" data-toggle=""dropdown"">ADMIN <b class=""caret""></b></a>")
            html.Append("<ul class=""dropdown-menu"">")
            html.Append("<li><a href=""Dashboard.aspx"">Dashboard</a></li>")
            html.Append("<li><a href=""ProductDetail.aspx"">Products</a></li>")
            html.Append(" <li><a href=""Specials.aspx?pt=Announcements"">Announcements</a></li>")
            html.Append("<li><a href=""Maintenance.aspx"">Maintenance</a></li>")
            html.Append("<li><a href=""Reports.aspx"">Reports</a></li>")
            html.Append("</ul>")
            html.Append("</li>")
        ElseIf PageName <> "admin" Then
            'Cart Code
            html.Append("<li>")
            html.Append("<!-- Cart in header -->")
            html.Append("<div class='header-cart'>")
            html.Append("<span class='header-cart__text--price'>")
            html.Append("<span id='CartPrice'>$0.00</span>")
            html.Append("</span>")
            html.Append("<a href='Checkout.aspx' class='header-cart__items'>")
            html.Append("<span class='header-cart__items-num' id='CartCount'>0</span>")
            html.Append("</a>")

            html.Append("<!-- Open cart panel -->")
            html.Append("<div class='header-cart__open-cart' id='CartList'>")
            html.Append("</div>")
            html.Append("</div>")
            html.Append("</li>")
        End If


        TopMenuList.Text = html.ToString()
    End Sub

    Sub CheckSpecailPages(ByVal MyPageType As String) 
        Dim Content As String = ""
        Dim html as New StringBuilder

        If File.Exists(PageFolder & "\" & MyPageType & ".htm") Then
            Dim sr As StreamReader
            sr = File.OpenText(PageFolder & "\" & MyPageType & ".htm")
            Content = sr.ReadToEnd()
            sr.Close()
        End If

        If Content.Length > 10 Then
            html.Append("<div class='alert alert-warning' style='margin-top: 8px'>" & Content & "</div>")
            Announcements.Text = html.ToString()
        End If

    End Sub
End Class
