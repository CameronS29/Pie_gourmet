Imports Microsoft.Practices.EnterpriseLibrary.Data
Imports System.Data.Common
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO

Public Class DropDowns

    Public Shared factory As DatabaseProviderFactory = New DatabaseProviderFactory()
    Public Shared db As Database = factory.CreateDefault()

    ' Main DropDown Function to create data source
    Shared Function CreateDataSource(ByVal sqlString As String, ByVal FirstRow As String, Optional ByVal FirstRowText As String = "Select...") As ICollection

        ' Create a table to store data for the DropDownList control.
        Dim dt As DataTable = New DataTable
        Dim dv As DataView
        Dim dbCommand As DbCommand = db.GetSqlStringCommand(sqlString)
        Try
            Using rs As IDataReader = db.ExecuteReader(dbCommand)

                ' Define the columns of the table.
                dt.Columns.Add(New DataColumn("TextField", GetType(String)))
                dt.Columns.Add(New DataColumn("ValueField", GetType(String)))

                'DD Header
                If IsNumeric(FirstRow) Or FirstRow = "" Then
                    dt.Rows.Add(CreateRow(FirstRowText, FirstRow, dt))
                Else
                    dt.Rows.Add(CreateRow(FirstRow & "...", FirstRow, dt))
                End If

                While rs.Read
                    ' Populate the table with values.
                    dt.Rows.Add(CreateRow(rs(1), rs(0), dt))
                End While

                ' Create a DataView from the DataTable to act as the data source
                ' for the DropDownList control.
                dv = New DataView(dt)
            End Using

        Catch ex As Exception
            'Error Capture
            Throw
        End Try

        Return dv
    End Function

    Shared Function CreateMenuSource(ByVal sqlString As String, ByVal FirstRow As String) As ICollection
        ' Create a table to store data for the DropDownList control.
        Dim dt As DataTable = New DataTable

        Dim dbCommand As DbCommand = db.GetSqlStringCommand(sqlString)
        Try
            Using rs As IDataReader = db.ExecuteReader(dbCommand)

                ' Define the columns of the table.
                dt.Columns.Add(New DataColumn("TextField", GetType(String)))
                dt.Columns.Add(New DataColumn("ValueField", GetType(String)))

                'DD Header
                If IsNumeric(FirstRow) Or FirstRow = "" Then
                    dt.Rows.Add(CreateRow("Root...", FirstRow, dt))
                Else
                    dt.Rows.Add(CreateRow(FirstRow & "...", FirstRow, dt))
                End If


                While rs.Read
                    ' Populate the table with values.
                    dt.Rows.Add(CreateRow(rs(1), rs(0), dt))
                End While

                ' Create a DataView from the DataTable to act as the data source
                ' for the DropDownList control.
                Dim dv As DataView = New DataView(dt)
                Return dv
            End Using

        Catch ex As Exception
            'Error Capture
            Throw
        End Try

    End Function

    Shared Function CreateRow(ByVal Text As String, ByVal Value As String, ByVal dt As DataTable) As DataRow

        ' Create a DataRow using the DataTable defined in the 
        ' CreateDataSource method.
        Dim dr As DataRow = dt.NewRow()

        ' This DataRow contains the TextField and ValueField 
        ' fields, as defined in the CreateDataSource method. Set the 
        ' fields with the appropriate value. Remember that column 0 
        ' is defined as TextField, and column 1 is defined as 
        ' ValueField.
        dr(0) = Text
        dr(1) = Value

        Return dr
    End Function

    'Lookup Maint - All Lookup Groups
    Public Shared Sub LookupGroupMaint(ByVal selItem As String, ByVal e As Object)
        'DropDown Location //////////////////////////////////
        Dim sqldrpdown As String
        Try
            sqldrpdown = "SELECT look_type as ValueField, look_type as TextField FROM lookups GROUP BY look_type"

            e.DataSource = CreateDataSource(sqldrpdown, "")
            e.DataTextField = "TextField"
            e.DataValueField = "ValueField"
            e.DataBind()
            If selItem <> "" Then
                e.SelectedValue = selItem.ToString
            End If
        Catch nl As ArgumentOutOfRangeException
            ' Do nothing
        Catch ex As Exception
            Throw
        End Try
        '////////////////////////////////////////////////////////////
    End Sub

    Public Shared Sub LookupsDropdown(ByVal selItem As String, ByVal e As Object, ByVal group As String, Optional ByVal OrderByNumber As Integer = 0)
        'DropDown Default Lookups Table //////////////////////////////////
        Dim sqldrpdown As String
        Try
            sqldrpdown = "EXEC MakeDropDown '" & group & "', " & OrderByNumber
            e.DataSource = CreateDataSource(sqldrpdown, "")
            e.DataTextField = "TextField"
            e.DataValueField = "ValueField"
            e.DataBind()
            If selItem <> "" Then
                e.SelectedValue = Trim(selItem.ToString)
            End If
        Catch nl As ArgumentOutOfRangeException
            ' Do nothing
        Catch ex As Exception
            Throw
        End Try

        '////////////////////////////////////////////////////////////
    End Sub

    Public Shared Sub PricingLookup(ByVal selItem As Integer, ByVal e As Object)
        Dim sqldrpdown As String
        Try
            sqldrpdown = "SELECT [pricingID], CAST([ProductPrice] AS VARCHAR(10)) + ' - ' + [ProductSize] + '""' FROM [pricing] ORDER BY [ProductSize]"
            e.DataSource = CreateDataSource(sqldrpdown, 0)
            e.DataTextField = "TextField"
            e.DataValueField = "ValueField"
            e.DataBind()
            If selItem >= 1 Then
                e.SelectedValue = selItem.ToString
            End If
        Catch nl As ArgumentOutOfRangeException
            ' Do nothing
        Catch ex As Exception
            Throw
        End Try
    End Sub


    Public Shared Sub BuildTimeArray(ByVal e As Object, ByVal SelItem As String, ByVal StartTime As Integer, ByVal EndTime As Integer, ByVal MinIncr As Integer)
        Dim tArray As New SortedList
        Dim i As Integer = 0
        Dim x As Integer = 0
        Dim keyCnt As Integer = 0
        Dim ttext As String = ""
        Dim mtext As String = ""

        'Add beginning Key
        tArray.Add(keyCnt, "")
        keyCnt += 1

        For i = StartTime To EndTime
            If i = 12 Then
                For x = 0 To 45 Step MinIncr
                    If x = 0 Then
                        mtext = ":00"
                    Else
                        mtext = ":" & x
                    End If
                    ttext = "12" & mtext & " PM"
                    tArray.Add(keyCnt, ttext)
                    keyCnt += 1
                Next
            ElseIf i >= 13 Then
                If i = EndTime Then
                    mtext = ":00"
                    ttext = (i - 12).ToString & mtext & " PM"
                    tArray.Add(keyCnt, ttext)
                Else
                    For x = 0 To 45 Step MinIncr
                        If x = 0 Then
                            mtext = ":00"
                        Else
                            mtext = ":" & x
                        End If
                        ttext = (i - 12).ToString & mtext & " PM"
                        tArray.Add(keyCnt, ttext)
                        keyCnt += 1
                    Next
                End If
            Else
                For x = 0 To 45 Step MinIncr
                    If x = 0 Then
                        mtext = ":00"
                    Else
                        mtext = ":" & x
                    End If
                    ttext = i.ToString & mtext & " AM"
                    tArray.Add(keyCnt, ttext)
                    keyCnt += 1
                Next
            End If
            keyCnt += 1
        Next

        e.DataSource = tArray
        e.DataTextField = "Value"
        e.DataValueField = "Value"
        e.DataBind()
        If SelItem <> "" Then
            e.SelectedValue = SelItem.ToString
        End If
    End Sub

    Public Shared Sub GetYears(ByVal selItem As String, ByVal e As Object)
        Dim tArray As New SortedList
        Dim i As Integer = 0
        Dim gYear As Integer = Year(Date.Today())
        Dim sYear = gYear - 2

        For i = sYear To gYear
            tArray.Add(i.ToString, i.ToString)
        Next

        e.DataSource = tArray
        e.DataTextField = "Value"
        e.DataValueField = "Key"
        e.DataBind()
        If selItem <> "" Then
            e.SelectedValue = selItem.ToString
        End If
    End Sub

    Public Shared Sub GetMonths(ByVal selItem As Integer, ByVal e As Object)
        Dim tArray As New SortedList
        Dim i As Integer = 0
        Dim gMonth As Integer = Month(Date.Today())

        tArray.Add(0, "Select Month...")

        For i = 1 To 12
            tArray.Add(i, MonthName(i))
        Next

        e.DataSource = tArray
        e.DataTextField = "Value"
        e.DataValueField = "Key"
        e.DataBind()
        If selItem >= 1 Then
            e.SelectedValue = selItem.ToString
        End If
    End Sub

    Public Shared Function SelectBillOrderClient(ByVal selItem As Integer) As String
        Dim tArray As New SortedList
        Dim i As Integer = 0
        Dim stext As String = ""
        Dim ttext As String = ""
        Dim btext As New System.Text.StringBuilder

        For i = 0 To 3
            If selItem = i Then
                stext = " selected"
            Else
                stext = ""
            End If

            If i = 0 Then
                ttext = "None"
            Else
                ttext = "Bill " & i.ToString
            End If

            btext.Append("<option value=" & i.ToString & stext & ">" & ttext & "</option>")
        Next

        Return btext.ToString
    End Function


    Public Shared Sub ScheduleLength(ByVal selItem As String, ByVal e As Object)
        Dim tArray As New SortedList
        Dim i As Integer
        Dim ttext As String

        For i = 15 To 60 Step 15
            ttext = i.ToString & " Min."
            tArray.Add(i, ttext)
        Next

        e.DataSource = tArray
        e.DataTextField = "Value"
        e.DataValueField = "Key"
        e.DataBind()
        If selItem <> "" Then
            e.SelectedValue = selItem.ToString
        End If

    End Sub

    Public Shared Sub PassExpireDays(ByVal selItem As Integer, ByVal e As Object)
        Dim tArray As New SortedList
        Dim i As Integer
        Dim ttext As String

        For i = 0 To 90 Step 10
            ttext = i.ToString & " Days"
            tArray.Add(i, ttext)
        Next

        e.DataSource = tArray
        e.DataTextField = "Value"
        e.DataValueField = "Key"
        e.DataBind()
        If selItem >= 1 Then
            e.SelectedValue = selItem.ToString
        End If

    End Sub

    '//////////////// Menu Items //////////////////////////////
    Public Shared Sub ParentMenuItems(ByVal e As Object)
        'DropDown providers //////////////////////////////////
        Dim sqldrpdown As String
        Try

            sqldrpdown = "SELECT itemID, Text FROM RadMenu WHERE IsParent >= 1"

            e.DataSource = CreateMenuSource(sqldrpdown, 0)
            e.DataTextField = "TextField"
            e.DataValueField = "ValueField"
            e.DataBind()

        Catch ex As Exception
            Throw
        End Try

        '////////////////////////////////////////////////////////////
    End Sub

    Public Shared Sub NumberRangeSelect(ByVal selItem As Integer, ByVal e As Object, ByVal MaxNum As Integer, ByVal StepNum As Integer)
        Dim tArray As New SortedList
        Dim i As Integer

        For i = 0 To MaxNum Step StepNum
            tArray.Add(i, i.ToString)
        Next

        e.DataSource = tArray
        e.DataTextField = "Value"
        e.DataValueField = "Key"
        e.DataBind()
        If selItem >= 1 Then
            e.SelectedValue = selItem
        End If
    End Sub

    Public Shared Sub NumberRangeSelectYears(ByVal selItem As Integer, ByVal e As Object, ByVal StartNum As Integer, ByVal MaxNum As Integer, ByVal StepNum As Integer)
        Dim tArray As New SortedList
        Dim i As Integer

        For i = StartNum To MaxNum Step StepNum
            tArray.Add(i, i.ToString)
        Next

        e.DataSource = tArray
        e.DataTextField = "Value"
        e.DataValueField = "Key"
        e.DataBind()
        If selItem >= 1 Then
            e.SelectedValue = selItem
        End If
    End Sub
    'Lookup for Ingredient List
    Public Shared Sub IngredientList(ByVal selItem As Integer, ByVal e As Object)
        Dim sqldrpdown As String
        Try
            sqldrpdown = "SELECT IngredientID, IngredientName FROM Ingredients ORDER BY IngredientName"

            e.DataSource = CreateDataSource(sqldrpdown, 0)
            e.DataTextField = "TextField"
            e.DataValueField = "ValueField"
            e.DataBind()
            If selItem >= 1 Then
                e.SelectedValue = selItem.ToString
            End If
        Catch nl As ArgumentOutOfRangeException
            ' Do nothing
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    'Lookup for Client Custom Reports
    Public Shared Function cCustomReportsClients(ByVal selItem As Integer, ByVal clientID As Integer) As String
        'DropDown Default Lookups Table (Function Embedded) //////////////////////////////////

        Dim sqlCommand As String = "SELECT cReportID, ReportTitle FROM MyCustomReports WHERE clientID = " & clientID & " ORDER BY ReportTitle"
        Dim html As New System.Text.StringBuilder
        Dim dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)

        Try
            Using rs As IDataReader = db.ExecuteReader(dbCommand)
                html.Append("<option value=0>Select Report...</option>")
                While rs.Read()
                    If selItem = rs("cReportID") Then
                        html.Append("<option value='" & rs("cReportID").ToString & "' selected>" & rs("ReportTitle").ToString & "</option>")
                    Else
                        html.Append("<option value='" & rs("cReportID").ToString & "'>" & rs("ReportTitle").ToString & "</option>")
                    End If
                End While
            End Using

        Catch ex As Exception
            Return ex.Message.ToString
        End Try

        Return html.ToString
        '////////////////////////////////////////////////////////////
    End Function


    Public Shared Function EventLaneAssign(ByVal EventID As Integer, ByVal RoomID As Integer) As String
        'DropDown Default Lookups Table (Function Embedded) //////////////////////////////////

        Dim sqlCommand As String = "EXEC EventLaneList " & EventID & ", " & RoomID
        Dim html As New System.Text.StringBuilder
        Dim dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)
        Dim RoomName As String = ""

            Try
                Using rs As IDataReader = db.ExecuteReader(dbCommand)
                    html.Append("<select id='LaneSelectList' class=""select validate[required] multiple-as-single easy-multiple-selection check-list"" multiple='multiple'>")

                While rs.Read()
                    If rs("RoomType") = "Pool" Then
                        RoomName = "Lane"
                    Else
                        RoomName = "Room"
                    End If

                    If Not String.IsNullOrEmpty(rs("LaneNum").ToString) Then
                        html.Append("<option value='" & rs("LaneCode").ToString & "' selected='selected'>" & RoomName & " " & rs("LaneCode").ToString & "</option>")
                    Else
                        html.Append("<option value='" & rs("LaneCode").ToString & "'>" & RoomName & " " & rs("LaneCode").ToString & "</option>")
                    End If
                End While

                    html.Append("</select>")
                End Using

            Catch ex As Exception
                Return ex.Message.ToString
            End Try

            Return html.ToString
            '////////////////////////////////////////////////////////////
    End Function


    Public Shared Function ShowFamilyList(ByVal MemberID As Integer, ByVal Selected As Integer, Optional ByVal EleID As String = "FamilyID") As String
        'DropDown Default Lookups Table (Function Embedded) //////////////////////////////////

        Dim sqlCommand As String = "SELECT * FROM Family WHERE MemberID = " & MemberID
        Dim html As New System.Text.StringBuilder
        Dim dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)

        Try
            Using rs As IDataReader = db.ExecuteReader(dbCommand)
                html.Append("<select id='" & EleID & "' class=""select"">")
                html.Append("<option value='0'>Member Only</option>")

                While rs.Read()
                    If Selected = rs("FamilyID") Then
                        html.Append("<option value='" & rs("FamilyID").ToString & "' selected='selected'>" & rs("FamilyFirstName").ToString & " " & rs("FamilyLastName").ToString & " [" & rs("Relation").ToString & "]</option>")
                    Else
                        html.Append("<option value='" & rs("FamilyID").ToString & "'>" & rs("FamilyFirstName").ToString & " " & rs("FamilyLastName").ToString & " [" & rs("Relation").ToString & "]</option>")
                    End If
                End While

                html.Append("</select>")
            End Using

        Catch ex As Exception
            Return ex.Message.ToString
        End Try

        Return html.ToString
        '////////////////////////////////////////////////////////////
    End Function

    Public Shared Function LookupsDropdownFunction(ByVal selItem As String, ByVal group As String) As String
        'DropDown Default Lookups Table (Function Embedded) //////////////////////////////////

        Dim sqlCommand As String = "EXEC MakeDropDown '" & group & "'"
        Dim dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)
        Dim html As New System.Text.StringBuilder

        Try
            Using rs As IDataReader = db.ExecuteReader(dbCommand)
                While rs.Read()
                    If selItem = rs(0).ToString Then
                        html.Append("<option value='" & rs(0).ToString & "' selected>" & rs(1).ToString & "</option>")
                    Else
                        html.Append("<option value='" & rs(0).ToString & "'>" & rs(1).ToString & "</option>")
                    End If
                End While
            End Using
        Catch nl As ArgumentOutOfRangeException
            ' Do nothing
        Catch ex As Exception
            Return ex.Message.ToString
        End Try

        Return html.ToString
        '////////////////////////////////////////////////////////////
    End Function
End Class
