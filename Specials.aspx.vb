Imports System.Data
Imports System.Data.Common
Imports System.Data.SqlClient
Imports Microsoft.Practices.EnterpriseLibrary.Data
Imports System.IO
Imports InnovaStudio.WYSIWYGEditor
Imports InnovaStudio

Partial Class Specials
    Inherits System.Web.UI.Page

    Public Shared factory As DatabaseProviderFactory = New DatabaseProviderFactory()
    Public Shared db As Database = factory.CreateDefault()
    Public PageFolder As String = ConfigurationManager.AppSettings("PageFolder")

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        AjaxPro.Utility.RegisterTypeForAjax(GetType(Specials))
        AjaxPro.Utility.RegisterTypeForAjax(GetType(GlobalAjax))

        Dim mySession As New SessionManager
        Dim CustomerID As String = mySession.GetElement("CustomerID", "number")
        Dim UserID As String = mySession.GetElement("userID", "number")
        Dim Username As String = mySession.GetElement("username")
        Dim myName As String = mySession.GetElement("FullName")
        Dim MyEmail As String = mySession.GetElement("Email")
        Dim Access As String = mySession.GetElement("AccessLevel")
        Dim LastProduct As String = mySession.GetElement("LastProduct", "number")

        Dim pType As String = Request.QueryString("pt")
        Dim ServerPageLoad As String = "Specials"

        If pType <> "" Then
            ServerPageLoad = pType
        End If

        PageLoadType.Value = ServerPageLoad

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
            oEdit1.Visible = False
            Dim Content As String = ""
            If File.Exists(PageFolder & "\" & ServerPageLoad & ".htm") Then
                Dim sr As StreamReader
                sr = File.OpenText(PageFolder & "\" & ServerPageLoad & ".htm")
                Content = sr.ReadToEnd()
                sr.Close()
            Else
                Content = "<h3>No current specials, please check back later...</h3>"
            End If

            SpecialPage.InnerHtml = Content

        Else
            'Load Specials Page
            oEdit1.scriptPath = "escripts/"
            oEdit1.EditMode = EditorModeEnum.XHTMLBody
            oEdit1.Css = "stylesheets/main.css"
            oEdit1.btnStyles = True
            oEdit1.UseBR = True
            oEdit1.ToolbarMode = 0
            oEdit1.UseTab = False
            oEdit1.btnClearAll = True
            oEdit1.btnPrint = True

            'Toolbar Buttons Configuration
            Dim tabHome As InnovaStudio.ISTab
            Dim grpEdit1 As InnovaStudio.ISGroup = New InnovaStudio.ISGroup("grpEdit1", "", New String() {"CustomTag", "Bold", "Italic", "Underline", "FontDialog", "FontSize", "ForeColor", "BackColor", "TextDialog", "RemoveFormat", "ClearAll"})
            Dim grpEdit2 As InnovaStudio.ISGroup = New InnovaStudio.ISGroup("grpEdit2", "", New String() {"Bullets", "Numbering", "JustifyLeft", "JustifyCenter", "JustifyRight"})
            Dim grpEdit3 As InnovaStudio.ISGroup = New InnovaStudio.ISGroup("grpEdit3", "", New String() {"LinkDialog", "ImageDialog", "TextDialog", "TableDialog", "Line"})
            Dim grpEdit4 As InnovaStudio.ISGroup = New InnovaStudio.ISGroup("grpEdit4", "", New String() {"InternalLink", "CustomObject", "MyCustomButton"})
            Dim grpEdit5 As InnovaStudio.ISGroup = New InnovaStudio.ISGroup("grpEdit5", "", New String() {"Undo", "Redo", "FullScreen", "SourceDialog", "CustomSave"})
            tabHome = New InnovaStudio.ISTab("tabHome", "Home")

            oEdit1.ToolbarCustomButtons.Add(New CustomButton("CustomSave", "SavePage()", "Save Page", "btnSave.gif")) 'Custom Save Button

            tabHome.Groups.AddRange(New InnovaStudio.ISGroup() {grpEdit1, grpEdit2, grpEdit3, grpEdit5})
            oEdit1.ToolbarTabs.Add(tabHome)
            Dim Content As String = ""

            If File.Exists(PageFolder & "\" & ServerPageLoad & ".htm") Then
                Dim sr As StreamReader
                sr = File.OpenText(PageFolder & "\" & ServerPageLoad & ".htm")
                Content = sr.ReadToEnd()
                sr.Close()
            Else
                Content = "File Does Not Exist..."
            End If

            oEdit1.Text = Content
        End If

        WriteButtonRow(ServerPageLoad)
    End Sub

    Private Sub WriteClientVals(ByVal ValueArray As StringDictionary)
        Dim sb As New StringBuilder
        Dim i As DictionaryEntry

        For Each i In ValueArray
            sb.Append("var " & i.Key & " = '" & i.Value & "';" & vbCrLf)
        Next

        Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "DashClientVals", sb.ToString, True)
    End Sub

    Sub WriteButtonRow(ByVal PageType As String)
        Dim html As New StringBuilder

        Select Case PageType
            Case "Specials"
                html.Append("<a href=""Specials.aspx?pt=Specials"" class=""btn btn-warning"">Specials</a>&nbsp;")
                html.Append("<a href=""Specials.aspx?pt=Menu"" class=""btn btn-dark"">Menu</a>&nbsp;")
                html.Append("<a href=""Specials.aspx?pt=Announcements"" class=""btn btn-dark"">Announcements</a>&nbsp;")
            Case "Menu"
                html.Append("<a href=""Specials.aspx?pt=Specials"" class=""btn btn-dark"">Specials</a>&nbsp;")
                html.Append("<a href=""Specials.aspx?pt=Menu"" class=""btn btn-warning"">Menu</a>&nbsp;")
                html.Append("<a href=""Specials.aspx?pt=Announcements"" class=""btn btn-dark"">Announcements</a>&nbsp;")
            Case "Announcements"
                html.Append("<a href=""Specials.aspx?pt=Specials"" class=""btn btn-dark"">Specials</a>&nbsp;")
                html.Append("<a href=""Specials.aspx?pt=Menu"" class=""btn btn-dark"">Menu</a>&nbsp;")
                html.Append("<a href=""Specials.aspx?pt=Announcements"" class=""btn btn-warning"">Announcements</a>&nbsp;")
        End Select

        MyButtonRow.InnerHtml = html.ToString()

    End Sub

    <AjaxPro.AjaxMethod()> _
    Function SaveFileToServer(ByVal FileContents As String, ByVal PageSave As String) As String
        Dim TempFile As String = PageFolder & "\" & PageSave & ".htm"

        Try
            If File.Exists(TempFile) Then
                Dim sr As StreamWriter = New StreamWriter(TempFile)
                sr.WriteLine(FileContents)
                sr.Close()
            Else
                Dim sr As StreamWriter = File.CreateText(TempFile)
                sr.WriteLine(FileContents)
                sr.Close()
            End If
        Catch ex As Exception
            Return "1|Save Error: " & ex.Message.ToString()
        End Try

        Return "0|Page Saved Successfully!"
    End Function
End Class
