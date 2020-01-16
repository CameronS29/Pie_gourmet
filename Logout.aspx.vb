
Partial Class Logout
    Inherits System.Web.UI.Page

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'System Logout
        Session.Abandon()
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1))
        Response.Cache.SetNoStore()
        Response.AppendHeader("Cache-Control", "must-revalidate")
        Response.Redirect("Default.aspx")
    End Sub
End Class
