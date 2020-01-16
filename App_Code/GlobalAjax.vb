Imports System.Data
Imports System.Data.Common
Imports Microsoft.Practices.EnterpriseLibrary.Data

Public Class GlobalAjax

    Private LogFolder As String = ConfigurationManager.AppSettings("LogFolder")
    Public Shared factory As DatabaseProviderFactory = New DatabaseProviderFactory()
    Public Shared db As Database = factory.CreateDefault()

    ''' <summary>
    '''     The generic ILog logger
    ''' </summary>
    Public Shared Logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger()

    Public Shared Function GetApplicationSetting(ByVal SettingKey As String) As String
        Dim sqlCommand As String = "SELECT MySetting FROM ApplicationSettings WHERE SettingType = '" & Common.SQLEncode(SettingKey) & "'"
        Dim dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)
        Dim str As String = ""
        Dim i As Integer = 0

        Try
            Using rs As IDataReader = db.ExecuteReader(dbCommand)
                While rs.Read()
                    str = rs(0).ToString
                    i += 1
                End While
            End Using

            If i = 0 Then
                str = ""
            End If

        Catch ex As Exception
            Logger.Error(ex, "Global Functions: " & ex.Message.ToString())
            Return "Setting Error: " & ex.Message.ToString
        End Try

        Return str
    End Function

    Public Shared Function GetRatingStars(ByVal AvgRating As Integer) As String
        Dim RatingText As New StringBuilder

        'Rating Stars **********************************************************************
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
        Return RatingText.ToString()
    End Function

    Public Class ProductAddClass
        Public CartID As Integer = 0
        Public SessionID As String = ""
        Public CustomerID As Integer = 0
        Public ProductID As Integer = 0
        Public Quantity As Integer = 1
        Public SubProduct As Integer = 0
    End Class

    <AjaxPro.AjaxMethod()> _
    Function GetProductAddClass() As ProductAddClass
        Dim cd As ProductAddClass = New ProductAddClass
        Return cd
    End Function

    <AjaxPro.AjaxMethod()> _
    Function AddProduct(ByVal pc As ProductAddClass) As String
        Dim sqlCommand As String = "process_cart_temp"
        Dim i As Integer = 0

        If Not IsNumeric(pc.ProductID) Then
            Return "1|Error: Please select a valid product first!"
        Else
            Try
                Using dbCommand As DbCommand = db.GetStoredProcCommand(sqlCommand)
                    db.AddInParameter(dbCommand, "cartID", DbType.Int32, pc.CartID)
                    db.AddInParameter(dbCommand, "sessionID ", DbType.String, pc.SessionID)
                    db.AddInParameter(dbCommand, "custID", DbType.Int32, pc.CustomerID)
                    db.AddInParameter(dbCommand, "productID", DbType.Int32, pc.ProductID)
                    db.AddInParameter(dbCommand, "pricingID ", DbType.Int32, pc.SubProduct)
                    db.AddInParameter(dbCommand, "qty", DbType.Int32, pc.Quantity)
                    db.AddOutParameter(dbCommand, "retval", DbType.Int32, 4)

                    i = db.ExecuteNonQuery(dbCommand)
                    Dim retval As Integer = db.GetParameterValue(dbCommand, "retval")

                    Return "0|" & retval
                End Using
            Catch ex As Exception
                Logger.Error(ex, "Global Functions: " & Reflection.MethodBase.GetCurrentMethod.Name & " - " & ex.Message.ToString())
                Return "1|Add Product Error: " & ex.Message.ToString
            End Try
        End If

    End Function

    <AjaxPro.AjaxMethod()> _
    Function ShowCart(ByVal SessionID As String) As String
        Dim sqlCommand As String = "SELECT * FROM [temp_cart_view] WHERE sessionID = '" & Left(SessionID, 50) & "'"
        Dim html As New StringBuilder
        Dim dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)
        Dim i As Integer = 0
        Dim TotalPrice As Decimal = 0

        Try
            Using rs As IDataReader = db.ExecuteReader(dbCommand)

                While rs.Read()
                    html.Append("<div class=""header-cart__product  clearfix  js--cart-remove-target"">")
                    html.Append("<div class=""header-cart__product-image"">")
                    html.Append("<img alt=""Product in the cart"" src=""images/products/" & rs("ProductImage").ToString & """ width=""40"" height=""50"">")
                    html.Append("</div>")
                    html.Append("<div class=""header-cart__product-image--hover"">")
                    html.Append(" <a href=""javascript:void(0)"" onclick=""RemoveCartItem(" & rs("cart_tempID") & ")"" class=""js--remove-item"" data-target="".js--cart-remove-target""><span class=""glyphicon  glyphicon-circle  glyphicon-remove""></span></a>")
                    html.Append("</div>")
                    html.Append("<div class=""header-cart__product-title"">")
                    html.Append(" <a class=""header-cart__link"" href=""ShopDetail.aspx?q=" & rs("productID").ToString() & """>" & rs("ProductName").ToString & " [" & rs("ProductSize").ToString & " inch " & rs("TypeCategory").ToString & "]</a>")
                    html.Append("<span class=""header-cart__qty"">Qty: " & rs("qty").ToString & "</span>")
                    html.Append("</div>")
                    html.Append("<div class=""header-cart__price"">")
                    html.Append(Common.MyNumber(rs("ExtendPrice")))
                    html.Append("</div>")
                    html.Append("</div>")

                    TotalPrice += rs("ExtendPrice")
                    i += 1
                End While

                If i = 0 Then
                    html.Append("<div class=""header-cart__product  clearfix  js--cart-remove-target""><b>Your cart is empty.</b></div>")
                Else
                    html.Append("<hr class=""header-cart__divider"">")
                    html.Append("<div class=""header-cart__subtotal-box"">")
                    html.Append("<span class=""header-cart__subtotal"">CART SUBTOTAL:</span>")
                    html.Append("<span class=""header-cart__subtotal-price"">" & Common.MyNumber(TotalPrice) & "</span>")
                    html.Append("</div>")
                    html.Append(" <a class=""btn btn-darker"" href=""Checkout.aspx"">Procced to checkout</a>")
                End If
            End Using
        Catch ex As Exception
            Logger.Error(ex, "Global Functions: " & Reflection.MethodBase.GetCurrentMethod.Name & " - " & ex.Message.ToString())
            Return "Error: " & ex.Message.ToString
        End Try

        Return html.ToString & "|" & i & "|" & Common.MyNumber(TotalPrice)
    End Function

    <AjaxPro.AjaxMethod()> _
    Function RemoveProduct(ByVal TempCartID As Integer) As String
        Dim sqlCommand As String = "DeleteTempCartItem"
        Dim i As Integer = 0

        Try
            Using dbCommand As DbCommand = db.GetStoredProcCommand(sqlCommand)
                db.AddInParameter(dbCommand, "cart_tempID", DbType.Int32, TempCartID)

                i = db.ExecuteNonQuery(dbCommand)

                Return "0|" & i
            End Using
        Catch ex As Exception
            Logger.Error(ex, "Global Functions: " & Reflection.MethodBase.GetCurrentMethod.Name & " - " & ex.Message.ToString())
            Return "1|Remove Product Error: " & ex.Message.ToString
        End Try

    End Function

    <AjaxPro.AjaxMethod()> _
    Function GetProductPricing(ByVal ProductID As Integer, ByVal PricingGroup As Integer) As DataSet
        Dim sqlCommand As String = "SELECT * FROM [products_view] WHERE productID = " & ProductID & " AND pricingID = " & PricingGroup
        Dim dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)
        Dim ds As New DataSet
        Try
            ds = db.ExecuteDataSet(dbCommand)
            Return ds
        Catch ex As Exception
            Logger.Error(ex, "Global Functions: " & Reflection.MethodBase.GetCurrentMethod.Name & " - " & ex.Message.ToString())
            Throw ex
        End Try
    End Function

#Region "Customer Data Class ***********************************************"

    Public Class customers_Class
        Public customerID As Integer = 0
        Public cust_shipID As Integer = 0
        Public company As String = ""
        Public title As String = ""
        Public first_name As String = ""
        Public last_name As String = ""
        Public address1 As String = ""
        Public address2 As String = ""
        Public city As String = ""
        Public state As String = ""
        Public zip As String = ""
        Public email As String = ""
        Public home_phone As String = ""
        Public work_phone As String = ""
        Public mobile_phone As String = ""
        Public fax As String = ""
        Public cust_type As String = ""
        Public username As String = ""
        Public password As String = ""
        Public notes As String = ""
        Public cc_Number As String = ""
        Public cc_ExpMonth As Integer = 0
        Public cc_ExpYear As Integer = 0
        Public cc_Type As String = ""
        Public cc_Name As String = ""
        Public bulk_email As Integer = 0
        Public bulk_sent As Integer = 0
        Public mail_list As Integer = 0
        Public create_user As String = ""
        Public sessionID As String = ""
        Public active As Integer = 1
        Public TestKey As String = ""
        Public DeliveryMethod As String = ""
    End Class

    <AjaxPro.AjaxMethod()> _
    Function GetcustomersClass() As customers_Class
        Dim cd As customers_Class = New customers_Class
        Return cd
    End Function

    <AjaxPro.AjaxMethod()> _
    Function Getcustomers(ByVal customerID As Integer) As DataSet
        Dim sqlCommand As String = "SELECT * FROM customers WHERE customerID = " & customerID
        Dim dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)
        Dim ds As New DataSet
        Try
            ds = db.ExecuteDataSet(dbCommand)
            Return ds
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    <AjaxPro.AjaxMethod()> _
    Function Process_customers(ByVal mc As customers_Class) As String
        Dim sqlCommand As String = "process_customers"
        Dim i As Integer = 0
        Try
            Using dbCommand As DbCommand = db.GetStoredProcCommand(sqlCommand)
                db.AddInParameter(dbCommand, "customerID", DbType.Int32, mc.customerID)
                db.AddInParameter(dbCommand, "company", DbType.String, mc.company)
                db.AddInParameter(dbCommand, "title", DbType.String, mc.title)
                db.AddInParameter(dbCommand, "first_name", DbType.String, mc.first_name)
                db.AddInParameter(dbCommand, "last_name", DbType.String, mc.last_name)
                db.AddInParameter(dbCommand, "address1", DbType.String, mc.address1)
                db.AddInParameter(dbCommand, "address2", DbType.String, mc.address2)
                db.AddInParameter(dbCommand, "city", DbType.String, mc.city)
                db.AddInParameter(dbCommand, "state", DbType.String, mc.state)
                db.AddInParameter(dbCommand, "zip", DbType.String, mc.zip)
                db.AddInParameter(dbCommand, "email", DbType.String, mc.email)
                db.AddInParameter(dbCommand, "home_phone", DbType.String, mc.home_phone)
                db.AddInParameter(dbCommand, "work_phone", DbType.String, mc.work_phone)
                db.AddInParameter(dbCommand, "mobile_phone", DbType.String, mc.mobile_phone)
                db.AddInParameter(dbCommand, "fax", DbType.String, mc.fax)
                db.AddInParameter(dbCommand, "cust_type", DbType.String, mc.cust_type)
                db.AddInParameter(dbCommand, "username", DbType.String, mc.username)
                db.AddInParameter(dbCommand, "password", DbType.String, mc.password)
                db.AddInParameter(dbCommand, "notes", DbType.String, mc.notes)
                db.AddInParameter(dbCommand, "cc_Number", DbType.String, mc.cc_Number)
                db.AddInParameter(dbCommand, "cc_ExpMonth", DbType.Int32, mc.cc_ExpMonth)
                db.AddInParameter(dbCommand, "cc_ExpYear", DbType.Int32, mc.cc_ExpYear)
                db.AddInParameter(dbCommand, "cc_Type", DbType.String, mc.cc_Type)
                db.AddInParameter(dbCommand, "cc_Name", DbType.String, mc.cc_Name)
                db.AddInParameter(dbCommand, "bulk_email", DbType.Boolean, mc.bulk_email)
                db.AddInParameter(dbCommand, "bulk_sent", DbType.Boolean, mc.bulk_sent)
                db.AddInParameter(dbCommand, "mail_list", DbType.Boolean, mc.mail_list)
                db.AddInParameter(dbCommand, "create_user", DbType.String, mc.create_user)
                db.AddOutParameter(dbCommand, "retval", DbType.Int32, 4)
                i = db.ExecuteNonQuery(dbCommand)
                Dim retval As Integer = db.GetParameterValue(dbCommand, "retval")
                Return retval.ToString
            End Using
        Catch ex As Exception
            Logger.Error(ex, "Global Functions: " & Reflection.MethodBase.GetCurrentMethod.Name & " - " & ex.Message.ToString())
            Return ex.Message.ToString
        End Try
    End Function

    <AjaxPro.AjaxMethod()> _
    Function ProcessTab1(ByVal mc As customers_Class) As String
        Dim sqlCommand As String = "process_customer_chkout"
        Dim i As Integer = 0
        Try
            Using dbCommand As DbCommand = db.GetStoredProcCommand(sqlCommand)
                db.AddInParameter(dbCommand, "customerID", DbType.Int32, mc.customerID)
                db.AddInParameter(dbCommand, "session", DbType.String, mc.sessionID)
                db.AddInParameter(dbCommand, "company", DbType.String, mc.company)
                db.AddInParameter(dbCommand, "title", DbType.String, mc.title)
                db.AddInParameter(dbCommand, "first_name", DbType.String, mc.first_name)
                db.AddInParameter(dbCommand, "last_name", DbType.String, mc.last_name)
                db.AddInParameter(dbCommand, "address1", DbType.String, mc.address1)
                db.AddInParameter(dbCommand, "address2", DbType.String, mc.address2)
                db.AddInParameter(dbCommand, "city", DbType.String, mc.city)
                db.AddInParameter(dbCommand, "state", DbType.String, mc.state)
                db.AddInParameter(dbCommand, "zip", DbType.String, mc.zip)
                db.AddInParameter(dbCommand, "email", DbType.String, mc.email)
                db.AddInParameter(dbCommand, "home_phone", DbType.String, mc.home_phone)
                db.AddInParameter(dbCommand, "order_notes", DbType.String, mc.notes)
                db.AddInParameter(dbCommand, "cust_shipID", DbType.Int32, mc.cust_shipID)
                db.AddInParameter(dbCommand, "user", DbType.String, mc.create_user)
                db.AddInParameter(dbCommand, "DeliveryMethod", DbType.String, mc.DeliveryMethod)
                db.AddOutParameter(dbCommand, "retval", DbType.Int32, 4)
                i = db.ExecuteNonQuery(dbCommand)
                Dim retval As Integer = db.GetParameterValue(dbCommand, "retval")
                Return "0|" & retval.ToString
            End Using
        Catch ex As Exception
            Logger.Error(ex, "Global Functions: " & Reflection.MethodBase.GetCurrentMethod.Name & " - " & ex.Message.ToString())
            Return "1|Process Error: " & ex.Message.ToString
        End Try
    End Function

    Public Shared Function RandomImageShow(ByVal ImageCount As Integer, Optional ByVal ImgSrc As String = "images") As String
        Dim html As New StringBuilder
        Dim collect As New StringBuilder
        Dim rnd As New Random
        Dim tWidth As Integer = 20 * ImageCount
        Dim ImgRespone As String = ""
        Dim i As Integer

        Try
            html.Append("<table width='" & tWidth & "px' cellspacing=1 cellpadding=1>")
            html.Append("<tr>")
            For i = 1 To ImageCount
                ImgRespone = rnd.Next(9).ToString
                collect.Append(ImgRespone)
                html.Append("<td><img src='" & ImgSrc & "/dummy/d" & ImgRespone & ".png' border='0' width='20px' /></td>")
            Next
            html.Append("</tr></table>")
            HttpContext.Current.Session("ImageBlock") = collect.ToString()
        Catch ex As Exception
            Logger.Error(ex, "Global Functions: " & Reflection.MethodBase.GetCurrentMethod.Name & " - " & ex.Message.ToString())
            Return ""
        End Try
        Return html.ToString()
    End Function

    <AjaxPro.AjaxMethod()> _
    Function ProcessCustomersOnline(ByVal mc As customers_Class) As String
        Dim sqlCommand As String = "process_customer_online"
        Dim i As Integer = 0

        'Test Submit Key On New Account
        If mc.customerID = 0 Then
            If mc.TestKey <> "" Then
                Dim ServerKey As String = HttpContext.Current.Session("ImageBlock")
                If ServerKey <> mc.TestKey Then
                    Return "1|Your human security key does not match our key: " & ServerKey
                End If
            Else
                Return "1|Your security key must not be blank when adding a new account."
            End If
        End If

        Try
            Using dbCommand As DbCommand = db.GetStoredProcCommand(sqlCommand)
                db.AddInParameter(dbCommand, "customerID", DbType.Int32, mc.customerID)
                db.AddInParameter(dbCommand, "company", DbType.String, mc.company)
                db.AddInParameter(dbCommand, "title", DbType.String, mc.title)
                db.AddInParameter(dbCommand, "first_name", DbType.String, mc.first_name)
                db.AddInParameter(dbCommand, "last_name", DbType.String, mc.last_name)
                db.AddInParameter(dbCommand, "address1", DbType.String, mc.address1)
                db.AddInParameter(dbCommand, "address2", DbType.String, mc.address2)
                db.AddInParameter(dbCommand, "city", DbType.String, mc.city)
                db.AddInParameter(dbCommand, "state", DbType.String, mc.state)
                db.AddInParameter(dbCommand, "zip", DbType.String, mc.zip)
                db.AddInParameter(dbCommand, "email", DbType.String, mc.email)
                db.AddInParameter(dbCommand, "home_phone", DbType.String, mc.home_phone)
                db.AddInParameter(dbCommand, "mobile_phone", DbType.String, mc.mobile_phone)
                db.AddInParameter(dbCommand, "password", DbType.String, mc.password)
                db.AddInParameter(dbCommand, "create_user", DbType.String, mc.create_user)
                db.AddOutParameter(dbCommand, "retval", DbType.Int32, 4)
                i = db.ExecuteNonQuery(dbCommand)
                Dim retval As Integer = db.GetParameterValue(dbCommand, "retval")
                Return "0|" & retval.ToString
            End Using
        Catch ex As Exception
            Logger.Error(ex, "Global Functions: " & Reflection.MethodBase.GetCurrentMethod.Name & " - " & ex.Message.ToString())
            Return "1|" & "Error: " & ex.Message.ToString
        End Try
    End Function


    <AjaxPro.AjaxMethod()> _
    Function Deletecustomers(ByVal customerID As Integer) As String
        Dim sqlCommand As String = "DELETE FROM customers WHERE customerID = " & customerID
        Dim i As Integer = 0
        Try
            Using dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)
                i = db.ExecuteNonQuery(dbCommand)
                Return i.ToString
            End Using
        Catch ex As Exception
            Logger.Error(ex, "Global Functions: " & Reflection.MethodBase.GetCurrentMethod.Name & " - " & ex.Message.ToString())
            Return ex.Message.ToString
        End Try
    End Function

    Public Class site_comments_Class
        Public commentID As Integer = 0
        Public productID As Integer = 0
        Public Name As String = ""
        Public City As String = ""
        Public State As String = ""
        Public comment As String = ""
        Public show As Integer = 0
        Public feature As Integer = 0
        Public rating As Integer = 0
    End Class

    <AjaxPro.AjaxMethod()> _
    Function Getsite_commentsClass() As site_comments_Class
        Dim cd As site_comments_Class = New site_comments_Class
        Return cd
    End Function

    <AjaxPro.AjaxMethod()> _
    Function ProcessComment(ByVal mc As site_comments_Class) As String
        Dim sqlCommand As String = "process_comment"
        Dim i As Integer = 0
        Try
            Using dbCommand As DbCommand = db.GetStoredProcCommand(sqlCommand)
                db.AddInParameter(dbCommand, "commentID", DbType.Int32, mc.commentID)
                db.AddInParameter(dbCommand, "productID", DbType.Int32, mc.productID)
                db.AddInParameter(dbCommand, "Name", DbType.String, mc.Name)
                db.AddInParameter(dbCommand, "City", DbType.String, mc.City)
                db.AddInParameter(dbCommand, "State", DbType.String, mc.State)
                db.AddInParameter(dbCommand, "comment", DbType.String, mc.comment)
                db.AddInParameter(dbCommand, "show", DbType.Boolean, mc.show)
                db.AddInParameter(dbCommand, "feature", DbType.Boolean, mc.feature)
                db.AddInParameter(dbCommand, "rating", DbType.Int32, mc.rating)
                db.AddOutParameter(dbCommand, "retval", DbType.Int32, 4)
                i = db.ExecuteNonQuery(dbCommand)
                Dim retval As Integer = db.GetParameterValue(dbCommand, "retval")
                Return "0|" & retval.ToString
            End Using
        Catch ex As Exception
            Logger.Error(ex, "Global Functions: " & Reflection.MethodBase.GetCurrentMethod.Name & " - " & ex.Message.ToString())
            Return "1|" & ex.Message.ToString
        End Try
    End Function

    <AjaxPro.AjaxMethod()> _
    Function ShowComments(ByVal ProductID As Integer, ByVal ShowAll As Integer) As String
        'Fresh Product List //////////////////////////////////
        Dim sqlCommand As String = ""

        If ShowAll = 1 Then
            sqlCommand = "SELECT * FROM product_comments WHERE productID = '" & ProductID & "' ORDER BY create_date DESC"
        Else
            sqlCommand = "SELECT * FROM product_comments WHERE show = 1 AND productID = '" & ProductID & "' ORDER BY create_date DESC"
        End If

        Dim html As New StringBuilder
        Dim dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)
        Dim i As Integer = 0

        Try
            Using rs As IDataReader = db.ExecuteReader(dbCommand)
                While rs.Read()

                    'Comment List *******************

                    html.Append("<p class=""tab-text"">")
                    html.Append("<h5>" & rs("City").ToString() & ", " & rs("State").ToString() & " <span class=''>(" & rs("create_date").ToString() & ")</span></h5>")

                    html.Append("<div>" & rs("comment").ToString & "</div>")

                    html.Append("</p>")
                    i += 1
                End While

                If i = 0 Then
                    html.Append("No Reviews Yet...")
                End If
            End Using

        Catch ex As Exception
            Logger.Error(ex, "Global Functions: " & Reflection.MethodBase.GetCurrentMethod.Name & " - " & ex.Message.ToString())
            Return "Error: " & ex.Message.ToString()
        End Try

        Return html.ToString()
        '////////////////////////////////////////////////////////////
    End Function

    <AjaxPro.AjaxMethod()> _
    Function ShowCommentsAdmin(ByVal ProductID As Integer) As String
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
                    If rs("show") Then
                        html.Append("<span class=""in-stock"">&bull;</span>")
                    Else
                        html.Append("<span class=""out-of-stock"">&bull;</span>")
                    End If
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

            End Using

            Return html.ToString()
        Catch ex As Exception
            Logger.Error(ex, "Global Functions: " & Reflection.MethodBase.GetCurrentMethod.Name & " - " & ex.Message.ToString())
            Return "Error: " & ex.Message.ToString()
        End Try

        '////////////////////////////////////////////////////////////
    End Function

    <AjaxPro.AjaxMethod()> _
    Function DeleteComment(ByVal commentID As Integer) As String
        Dim sqlCommand As String = "DELETE FROM site_comments WHERE commentID = " & commentID
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

#End Region

#Region "Ingredients Maintenance"
    Public Class Ingredients_Class
        Public IngredientID As Integer = 0
        Public IngredientName As String = ""
        Public Description As String = ""
        Public create_user As String = ""
    End Class

    <AjaxPro.AjaxMethod()> _
    Function GetIngredientsClass() As Ingredients_Class
        Dim cd As Ingredients_Class = New Ingredients_Class
        Return cd
    End Function

    <AjaxPro.AjaxMethod()> _
    Function GetIngredients(ByVal IngredientID As Integer) As DataSet
        Dim sqlCommand As String = "SELECT * FROM Ingredients WHERE IngredientID = " & IngredientID
        Dim dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)
        Dim ds As New DataSet
        Try
            ds = db.ExecuteDataSet(dbCommand)
            Return ds
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    <AjaxPro.AjaxMethod()> _
    Function Process_Ingredients(ByVal mc As Ingredients_Class) As String
        Dim sqlCommand As String = "process_Ingredients"
        Dim i As Integer = 0
        Try
            Using dbCommand As DbCommand = db.GetStoredProcCommand(sqlCommand)
                db.AddInParameter(dbCommand, "IngredientID", DbType.Int32, mc.IngredientID)
                db.AddInParameter(dbCommand, "IngredientName", DbType.String, mc.IngredientName)
                db.AddInParameter(dbCommand, "Description", DbType.String, mc.Description)
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

    <AjaxPro.AjaxMethod()> _
    Function DeleteIngredients(ByVal IngredientID As Integer) As String
        Dim sqlCommand As String = "DELETE FROM Ingredients WHERE IngredientID = " & IngredientID
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

    <AjaxPro.AjaxMethod()> _
    Function ListIngredients() As String
        Dim sqlCommand As String = "SELECT * FROM Ingredients ORDER BY create_date DESC"
        Dim dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)
        Dim i As Integer = 0
        Dim html As New StringBuilder
        Try
            Using rs As IDataReader = db.ExecuteReader(dbCommand)

                While rs.Read()

                    'Comment List *******************
                    html.Append("<p class=""tab-text"">")
                    html.Append("<h5>")
                    html.Append(rs("IngredientName").ToString() & " <span class='small'>(" & rs("create_date").ToString() & ")</span>")
                    html.Append("&nbsp;&nbsp;<a href='javascript:void(0)' onclick=""Get_Ingredients(" & rs("IngredientID") & ")"">Edit</a>")
                    html.Append("&nbsp;|&nbsp;<a href='javascript:void(0)' onclick=""Delete_Ingredients(" & rs("IngredientID") & ")"">Remove</a>")
                    html.Append("</h5>")
                    html.Append("<div>" & rs("Description").ToString & "</div>")
                    html.Append("</p>")

                    i += 1
                End While

                If i = 0 Then
                    html.Append("<div>No Ingredients Defined Yet...</div>")
                End If

            End Using

        Catch ex As Exception
            html.Append("Error: " & ex.Message.ToString())
        End Try

        Return html.ToString()
    End Function


#End Region

#Region "Shipping List Maintenance"
    <AjaxPro.AjaxMethod()> _
    Public Shared Function ShippingListGet(ByVal customerID As Integer) As String

        Dim sqlCommand As String = "SELECT * FROM [customers_shipping] WHERE [customerID] = " & customerID & " AND active = 1"
        Dim dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)
        Dim html As New StringBuilder
        Dim i As Integer = 0

        Try
            Using rs As IDataReader = db.ExecuteReader(dbCommand)
                html.Append("<div class=""clearfix  visible-xs""></div>")
                While rs.Read()

                    If i = 0 Then
                        html.Append("<table class='table table-theme table-striped'>")
                        html.Append("<thead>")
                        html.Append("<tr><th>Edit</th><th>Remove</th><th>Name</th><th>Address</th></tr>")
                        html.Append("</thead>")
                    End If

                    html.Append("<td><button type=""button"" class=""btn small btn-primary--transition"" onclick=""EditShipping(" & rs("cust_shipID") & ")"">Edit</button></td>")
                    html.Append("<td><button type=""button"" class=""btn small btn-danger--transition"" onclick=""RemoveShipping(" & rs("cust_shipID") & ")"">Remove</button></td>")
                    html.Append("<td>" & rs("ship_name").ToString() & "</td>")
                    html.Append("<td>" & rs("ship_address1").ToString() & "</td>")
                    html.Append("</tr>")
                    i += 1
                End While

                If i >= 1 Then
                    html.Append("</table>")
                Else
                    html.Append("<div class='alert  alert-warning  uppercase'>No separate shipping addresses added yet, click Add above to create a new shipping address.</div>")
                End If


            End Using
        Catch ex As Exception
            Logger.Error(ex, "Global Functions: " & Reflection.MethodBase.GetCurrentMethod.Name & " - " & ex.Message.ToString())
            Return "Error: " & ex.Message.ToString
        End Try

        Return html.ToString()
    End Function

    Public Shared Function CustomerOrderHistory(ByVal customerID As Integer) As String

        Dim sqlCommand As String = "SELECT TOP 25 * FROM [order_summary] WHERE [customerID] = " & customerID & " ORDER BY create_date DESC"
        Dim dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)
        Dim html As New StringBuilder
        Dim i As Integer = 0

        Try
            Using rs As IDataReader = db.ExecuteReader(dbCommand)
                html.Append("<div class=""clearfix  visible-xs""></div>")
                While rs.Read()

                    If i = 0 Then
                        html.Append("<table class='table table-theme table-striped'>")
                        html.Append("<thead>")
                        html.Append("<tr><th>Date</th><th>Amount</th><th>Product(s)</th><th>Status</th></tr>")
                        html.Append("</thead>")
                    End If

                    html.Append("<td>" & rs("create_date") & "</td>")
                    html.Append("<td>" & Common.MyNumber(rs("order_total")) & "</td>")
                    html.Append("<td>" & rs("Products").ToString() & "</td>")
                    html.Append("<td>" & rs("cartStatus").ToString() & "</td>")
                    html.Append("</tr>")
                    i += 1
                End While

                If i >= 1 Then
                    html.Append("</table>")
                Else
                    html.Append("<div class='alert  alert-warning  uppercase'>No Order History Available.</div>")
                End If


            End Using
        Catch ex As Exception
            Logger.Error(ex, "Global Functions: " & Reflection.MethodBase.GetCurrentMethod.Name & " - " & ex.Message.ToString())
            Return "Error: " & ex.Message.ToString
        End Try

        Return html.ToString()
    End Function

    <AjaxPro.AjaxMethod()> _
    Public Shared Function ShippingListGetCheckout(ByVal customerID As Integer) As String

        Dim sqlCommand As String = "SELECT * FROM [customers_shipping] WHERE [customerID] = " & customerID & " AND active = 1"
        Dim dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)
        Dim html As New StringBuilder
        Dim i As Integer = 0

        Try
            Using rs As IDataReader = db.ExecuteReader(dbCommand)
                html.Append("<div class=""clearfix  visible-xs""></div>")
                While rs.Read()

                    If i = 0 Then
                        html.Append("<table class='table table-theme table-striped' id='ShippingTable'>")
                        html.Append("<thead>")
                        html.Append("<tr><th>Select</th><th>Edit</th><th>Remove</th><th>Address</th></tr>")
                        html.Append("</thead>")
                    End If

                    html.Append("<td><input type='radio' class='CustomShipListClass' name='ShipToSelect' id='sts" & i & "' value='" & rs("cust_shipID").ToString() & "' onclick=""SetCustomShipping('" & rs("cust_shipID").ToString() & "')"" /></td>")
                    html.Append("<td><button type=""button"" class=""btn small btn-primary--transition"" onclick=""EditShipping(" & rs("cust_shipID") & ")"">Edit</button></td>")
                    html.Append("<td><button type=""button"" class=""btn small btn-danger--transition"" onclick=""RemoveShipping(" & rs("cust_shipID") & ")"">Remove</button></td>")
                    html.Append("<td>" & rs("ship_name").ToString() & "<br>" & rs("ship_address1").ToString() & "</td>")
                    html.Append("</tr>")
                    i += 1
                End While

                If i >= 1 Then
                    html.Append("</table>")
                Else
                    html.Append("<div class='alert  alert-warning  uppercase'>No separate shipping addresses added yet, click Add above to create a new shipping address.</div>")
                End If


            End Using
        Catch ex As Exception
            Logger.Error(ex, "Global Functions: " & Reflection.MethodBase.GetCurrentMethod.Name & " - " & ex.Message.ToString())
            Return "Error: " & ex.Message.ToString
        End Try

        Return html.ToString()
    End Function

    Public Class customers_shipping_Class
        Public cust_shipID As Integer = 0
        Public customerID As Integer = 0
        Public ship_name As String = ""
        Public ship_address1 As String = ""
        Public ship_address2 As String = ""
        Public ship_city As String = ""
        Public ship_state As String = ""
        Public ship_zip As String = ""
        Public ship_phone As String = ""
        Public email As String = ""
        Public active As Integer = 0
        Public create_user As String = ""
    End Class

    <AjaxPro.AjaxMethod()> _
    Function Getcustomers_shippingClass() As customers_shipping_Class
        Dim cd As customers_shipping_Class = New customers_shipping_Class
        Return cd
    End Function

    <AjaxPro.AjaxMethod()> _
    Function Getcustomers_shipping(ByVal cust_shipID As Integer) As DataSet
        Dim sqlCommand As String = "SELECT * FROM customers_shipping WHERE cust_shipID = " & cust_shipID
        Dim dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)
        Dim ds As New DataSet
        Try
            ds = db.ExecuteDataSet(dbCommand)
            Return ds
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    <AjaxPro.AjaxMethod()> _
    Function Process_customers_shipping(ByVal mc As customers_shipping_Class) As String
        Dim sqlCommand As String = "process_customer_shipping"
        Dim i As Integer = 0
        Try
            Using dbCommand As DbCommand = db.GetStoredProcCommand(sqlCommand)
                db.AddInParameter(dbCommand, "cust_shipID", DbType.Int32, mc.cust_shipID)
                db.AddInParameter(dbCommand, "customerID", DbType.Int32, mc.customerID)
                db.AddInParameter(dbCommand, "ship_name", DbType.String, mc.ship_name)
                db.AddInParameter(dbCommand, "ship_address1", DbType.String, mc.ship_address1)
                db.AddInParameter(dbCommand, "ship_address2", DbType.String, mc.ship_address2)
                db.AddInParameter(dbCommand, "ship_city", DbType.String, mc.ship_city)
                db.AddInParameter(dbCommand, "ship_state", DbType.String, mc.ship_state)
                db.AddInParameter(dbCommand, "ship_zip", DbType.String, mc.ship_zip)
                db.AddInParameter(dbCommand, "ship_phone", DbType.String, mc.ship_phone)
                db.AddInParameter(dbCommand, "email", DbType.String, mc.email)
                db.AddInParameter(dbCommand, "user", DbType.String, mc.create_user)
                db.AddOutParameter(dbCommand, "retval", DbType.Int32, 4)
                i = db.ExecuteNonQuery(dbCommand)
                Dim retval As Integer = db.GetParameterValue(dbCommand, "retval")
                Return "0|" & retval.ToString
            End Using
        Catch ex As Exception
            Logger.Error(ex, "Global Functions: " & Reflection.MethodBase.GetCurrentMethod.Name & " - " & ex.Message.ToString())
            Return "1|" & ex.Message.ToString
        End Try
    End Function

    <AjaxPro.AjaxMethod()> _
    Function Deletecustomers_shipping(ByVal cust_shipID As Integer) As String
        Dim sqlCommand As String = "DELETE FROM customers_shipping WHERE cust_shipID = " & cust_shipID
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

    <AjaxPro.AjaxMethod()> _
    Function deactivate_shipping(ByVal cust_shipID As Integer) As String
        Dim sqlCommand As String = "UPDATE customers_shipping SET active = 0 WHERE cust_shipID = " & cust_shipID
        Dim i As Integer = 0
        Try
            Using dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)
                i = db.ExecuteNonQuery(dbCommand)
                Return "0|" & i.ToString
            End Using
        Catch ex As Exception
            Return "1|" & ex.Message.ToString
        End Try
    End Function

    <AjaxPro.AjaxMethod()> _
    Function LocalDeliveryCheck(ByVal zip As String) As String
        Dim sqlCommand As String = "getFedExRates"
        Dim i As Integer = 0
        Dim RetStr As String = ""
        Try
            Using dbCommand As DbCommand = db.GetStoredProcCommand(sqlCommand)
                db.AddInParameter(dbCommand, "zip", DbType.String, zip)
                db.AddInParameter(dbCommand, "weight", DbType.Int32, 1)

                Using rs As IDataReader = db.ExecuteReader(dbCommand)
                    While rs.Read()
                        If Not String.IsNullOrEmpty(rs("Local_Delivery").ToString()) Then
                            RetStr = "0|Local delivery IS AVAILABLE for this zip code: " & zip
                        Else
                            RetStr = "99|Local delivery not found for this zip code: " & zip
                        End If
                        i += 1
                    End While
                End Using
            End Using

            If i = 0 Then
                RetStr = "99|Local delivery not found for this zip code: " & zip
            End If
        Catch ex As Exception
            Logger.Error(ex, "Global Functions: " & Reflection.MethodBase.GetCurrentMethod.Name & " - " & ex.Message.ToString())
            Return "1|" & ex.Message.ToString
        End Try

        Return RetStr
    End Function

    <AjaxPro.AjaxMethod()> _
    Function ClearProductPhoto(ByVal productID as Integer) As String
        Dim sqlCommand As String = "UPDATE products SET ProductImage = 'no_photo.png' WHERE productID = " & productID
        Dim i As Integer = 0
        Dim RetStr As String = ""
        Try
            Using dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)

                i = db.ExecuteNonQuery(dbCommand)

            End Using

            If i = 0 Then
                RetStr = "1|Product photo could not be updated..."
            Else
                RetStr = "0|Photo cleared successfully!"
            End If
        Catch ex As Exception
            Logger.Error(ex, "Global Functions: " & Reflection.MethodBase.GetCurrentMethod.Name & " - " & ex.Message.ToString())
            Return "1|" & ex.Message.ToString
        End Try

        Return RetStr
    End Function
#End Region
End Class

