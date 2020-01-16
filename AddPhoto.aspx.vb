Imports System.Data
Imports System.Data.Common
Imports System.Data.SqlClient
Imports Microsoft.Practices.EnterpriseLibrary.Data
Imports System.IO
Imports System.Drawing
Imports System.Drawing.Imaging

Partial Class Account_AddPhoto
    Inherits System.Web.UI.Page

    Private PhotoFolder As String = ConfigurationManager.AppSettings("ProductImages")
    Private AllowedExtensions As String = "jpg,gif,png"
    Public Shared factory As DatabaseProviderFactory = New DatabaseProviderFactory()
    Public Shared db As Database = factory.CreateDefault()

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        AjaxPro.Utility.RegisterTypeForAjax(GetType(GlobalAjax))

        Dim mySession As New SessionManager
        Dim CustomerID As String = mySession.GetElement("CustomerID", "number")
        Dim UserID As String = mySession.GetElement("userID", "number")
        Dim Username As String = mySession.GetElement("username")
        Dim myName As String = mySession.GetElement("FullName")
        Dim MyEmail As String = mySession.GetElement("Email")
        Dim Access As String = mySession.GetElement("AccessLevel")
        Dim LastProduct As String = mySession.GetElement("LastProduct")

        Dim myClientID As String = Request.QueryString("q")

        If Not mySession.AccessCheck(Session("AdminLoggedIn"), "ALL") Then
            Response.Write("<script type=""text/javascript"">window.close()</script>")
            Exit Sub
        End If

        If Not Page.IsPostBack Then
            'Dropdowns
           
            If String.IsNullOrEmpty(myClientID) Then
                If LastProduct <> "None" Then
                    myClientID = LastProduct
                Else
                    myClientID = 0
                End If
            End If

            If myClientID >= 1 Then
                GetUserData(myClientID)
                ProductID.Value = myClientID
            Else
                err_text.Text = "No User Information was found, please close page and try again."
            End If
        End If
    End Sub

    Sub GetUserData(ByVal ProductID As Integer)

        Dim sqlCommand As String = "SELECT * FROM products WHERE productID = " & ProductID.ToString
        Dim dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)

        Try
            Using rs As IDataReader = db.ExecuteReader(dbCommand)
                rs.Read()
                ClientName.Text = rs("ProductName").ToString & " [" & rs("ProductID").ToString & "]"
            End Using
        Catch ex As Exception
            err_text.Text = ex.Message.ToString
        End Try
    End Sub

    Protected Sub btn_save_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btn_save.Click
        Dim myFile As HttpPostedFile = GetDoc.PostedFile
        Dim nFileLen As Integer = myFile.ContentLength
        Dim x As Integer = 1
        Dim myUserID As Integer = ProductID.Value
        Dim ImgHeight As Integer = 360
        Dim ResizeFlag As Boolean = False

        If nFileLen > 0 Then

            'Get Extension and filename
            Dim exten As String = Path.GetExtension(myFile.FileName).Replace(".", "")
            Dim sFilename As String = myUserID & "." & exten 'Path.GetFileName(myFile.FileName)
            Dim FileSave As String = PhotoFolder & "\" & sFilename

            If Not IsAllowed(exten) Then
                err_text.Text = "Error uploading document >> The file being uploaded must be one of the following document types... " & AllowedExtensions
                Exit Sub
            End If

            If myUserID = 0 Then
                err_text.Text = "Please select a product account first..."
                Exit Sub
            End If

            Try
                'Resize/Save Image
                'ResizeFlag = ResizeImageFromStream(myFile.InputStream, ImgHeight, FileSave)

                ' If ResizeFlag Then
                Dim ac As New Attachments_Class
                ac.ProductID = myUserID
                ac.MyImage = sFilename

                Dim RetStr As String = SavePhotoToProduct(ac)
                Dim StrArr() As String = RetStr.Split("|")

                If StrArr(0) = 0 Then
                    myFile.SaveAs(FileSave)
                    err_text.Text = "Photo uploaded and saved successfully!"
                Else
                    err_text.Text = "Upload Error: " & StrArr(1)
                End If

                ' End If
            Catch ex As Exception
                err_text.Text = "Upload Error: " & ex.Message.ToString
            End Try

        Else
            err_text.Text = "No file was selected..."
        End If
    End Sub

    Function IsAllowed(ByVal filename As String) As Boolean
        If InStr(AllowedExtensions, filename, CompareMethod.Text) >= 1 Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Class Attachments_Class
        Public ProductID As Integer = 0
        Public MyImage As String = ""
        Public original_name As String = ""
        Public extension As String = ""
        Public Description As String = ""
        Public create_user As String = ""
    End Class

    Function SavePhotoToProduct(ByVal mc As Attachments_Class) As String
        Dim sqlCommand As String = "AddProductPhoto"
        Dim i As Integer = 0
        Try
            Using dbCommand As DbCommand = db.GetStoredProcCommand(sqlCommand)
                db.AddInParameter(dbCommand, "ProductID", DbType.Int32, mc.ProductID)
                db.AddInParameter(dbCommand, "PhotoName", DbType.String, mc.MyImage)

                i = db.ExecuteNonQuery(dbCommand)

                Return "0|" & mc.ProductID
            End Using
        Catch ex As Exception
            Return "1|" & "Error: " & ex.Message.ToString
        End Try
    End Function

    Function ResizeImageFromStream(ByVal vImg As Stream, ByVal height As Integer, ByVal FilePath As String) As Boolean
        Dim NewStream As New MemoryStream
        Dim iStream As Stream = vImg
        Dim original As Bitmap = New Bitmap(iStream)
        Dim new_image As Bitmap = Nothing

        'Image Resize It.....
        Dim newHeight As Integer = height
        Dim newWidth As Integer = 0
        Dim oWidth As Integer = original.Width
        Dim oHeight As Integer = original.Height

        If oHeight <= newHeight Then
            Try
                original.Save(FilePath, ImageFormat.Jpeg)
            Catch ex As Exception
                Return False
            End Try
            Return True
            Exit Function
        End If

        'Make image proportional to passed width
        If oHeight > newHeight Then
            newWidth = oWidth * newHeight / oHeight
        Else
            newHeight = oHeight
            newWidth = oWidth
        End If

        'Create Newly Sized Image
        new_image = New Bitmap(newWidth, newHeight)
        Dim Pic As Graphics = Graphics.FromImage(new_image)

        Try
            Pic.FillRectangle(Brushes.White, 0, 0, newWidth, newHeight)
            Pic.DrawImage(original, 0, 0, newWidth, newHeight)
            Pic.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
            Pic.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
            Pic.PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality

            'Finally Return Stream of Resized Pic
            new_image.Save(FilePath, ImageFormat.Jpeg)
            new_image.Dispose()
            Return True

        Catch ex As Exception
            Throw ex
        Finally
            Pic.Dispose()
            iStream.Close()
            NewStream.Close()
        End Try
    End Function
End Class
