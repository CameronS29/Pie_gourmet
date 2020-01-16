'***************************************************************
' SessionManagerSQL
' Description: Manages the constants/objects that a user might
'   save in a web session.  These get stored in a xml data file and 
'   then in memory and when the file changes, the file is 
'   removed from memory and re-read back into memory.
'
' NOTE: 
' Author: J Scott King
' Create date: 10/10/2005
' Mod Date: 12/12/2007 (Added SQL Server Table Access)
'***************************************************************

Imports System.Data
Imports System.Data.Common
Imports System.Data.SqlClient
Imports Microsoft.Practices.EnterpriseLibrary.Data
Imports System.IO
Imports System.Web.Caching
Imports System.Web
Imports System.Xml

Public Class SessionManager

    Private _sKey As String
    Private _sValue As String
    Private _userID As String
    Private _errText As String
    Private _access As Boolean = False
    Private _useCache As Boolean = False
    Private _DebugSession As Boolean = False
    Private _isErr As Boolean = False

    Public Shared factory As DatabaseProviderFactory = New DatabaseProviderFactory()
    Public Shared db As Database = factory.CreateDefault()

    Private FullFile As String
    Private xmlFile As New XmlDocument


#Region "Properties For Session State"
    '*********************************************************************
    ' Properties
    Public Property userID() As String
        Get
            Return (_userID)
        End Get
        Set(ByVal Value As String)
            _userID = Value

            If Value = "" Then
                _errText = "NoSession"
            End If
        End Set
    End Property

    Public Property sKey() As String
        Get
            Return (_sKey)
        End Get
        Set(ByVal Value As String)
            _sKey = Value
        End Set
    End Property

    Public Property sValue() As String
        Get
            Return (_sValue)
        End Get
        Set(ByVal Value As String)
            _sValue = Value
        End Set
    End Property

    Public Property errText() As String
        Get
            Return (_errText)
        End Get
        Set(ByVal Value As String)
            _errText = Value
        End Set
    End Property

    Public Property access() As Boolean
        Get
            Return (_access)
        End Get
        Set(ByVal Value As Boolean)
            _access = Value
        End Set
    End Property

    Public Property SessErr() As Boolean
        Get
            Return (_isErr)
        End Get
        Set(ByVal Value As Boolean)
            _isErr = Value
        End Set
    End Property
#End Region

    Sub New()
        Try
            Me.userID = HttpContext.Current.Session("userID")
            'Check for blank user session
            If Me.userID = "" Or Me.userID Is Nothing Then
                Me.errText = "NoSession"
                Me.SessErr = True
            End If
        Catch ex As NullReferenceException
            Me.errText = "NoSession"
            Me.SessErr = True
        End Try

    End Sub

    '----------------------------------------------------------------------
    '	Function: AccessCheck
    '   This function checks to see if user is logged in or session is
    '	not expired (Admin Users).
    '   menuGroup:  users allowed to access page.
    '----------------------------------------------------------------------
    Public Function AccessCheck(ByVal sess_flag As Boolean, ByVal menuGroup As String, Optional ByVal menuLevel As String = "ALL") As Boolean
        Dim check As Boolean = sess_flag
        Dim gpass As Integer = 0
        Dim lpass As Integer = 0
        Dim a() As String
        Dim b() As String
        Dim i As Integer = 0

        check = sess_flag

        'Check For Session Elements
        If GetElement("AccessLevel") = "None" Then
            check = False
        End If

        'If session Flag passes check user access to page (Group)
        If menuGroup <> "ALL" Then
            a = Split(menuGroup, ",")
            For i = 0 To UBound(a)
                If Trim(GetElement("AccessLevel")) = Trim(a(i)) Then
                    gpass = 1
                End If
            Next
        Else
            gpass = 1
        End If

        'Check menu level (User Class)
        If menuLevel <> "ALL" Then
            b = Split(menuLevel, ",")
            For i = 0 To UBound(b)
                If Trim(GetElement("UserClass")) = Trim(b(i)) Then
                    lpass = 1
                End If
            Next
        Else
            lpass = 1
        End If

        If Not check Or gpass = 0 Or lpass = 0 Then
            Return False
        Else
            Return True
        End If

    End Function

    Public Function AddElement(ByVal key As String, ByVal value As String) As Object
        Dim tmpObj As Object = "NoSession"
        Dim sqlCommand As String = "MySessionProcess"
        Dim i As Integer = 0

        If Me.userID >= 1 Then
            Try
                Using dbCommand As DbCommand = db.GetStoredProcCommand(sqlCommand)
                    db.AddInParameter(dbCommand, "Action", DbType.String, "POST")
                    db.AddInParameter(dbCommand, "Session", DbType.Int32, Me.userID)
                    db.AddInParameter(dbCommand, "SessionKey", DbType.String, key)
                    db.AddInParameter(dbCommand, "SessionValue", DbType.String, value)

                    i = db.ExecuteNonQuery(dbCommand)
                    tmpObj = True
                    Return tmpObj
                End Using
            Catch ex As Exception
                Return tmpObj
                Me.errText = ex.Message.ToString
            End Try
        Else
            Me.errText = "NoSession"
        End If

        If _DebugSession Then
            Me.errText = "File created - element created ..." & key & " :: " & value
        End If

        Return tmpObj
    End Function

    Public Sub AddElemArray(ByVal element As String, ByVal str As String)
        Dim Store As New SortedList
        Dim tst As New StringBuilder
        Dim ListCount As Integer = 0

        If Not HttpContext.Current.Session(element) Is Nothing Then
            Store = HttpContext.Current.Session(element)
            If Not Store.ContainsValue(str) Then
                If Store.Count = 10 Then
                    ListCount = Store.GetKey(9) + 1
                    Store.RemoveAt(0)
                Else
                    ListCount = Store.Count
                End If

                Store.Add(ListCount, str)
                HttpContext.Current.Session.Add(element, Store)
            End If

            'Return Store.Count.ToString
        Else
            Store.Add(ListCount, str)
            HttpContext.Current.Session.Add(element, Store)
            'Return "New"
        End If
    End Sub

    Public Function GetElement(ByRef key As String, Optional ByVal key_type As String = "string") As Object
        Dim tmpObj As Object = ""
        Dim sqlCommand As String = "MySessionProcess"
        Dim dbCommand As DbCommand = db.GetStoredProcCommand(sqlCommand)
        db.AddInParameter(dbCommand, "Action", DbType.String, "GET")
        db.AddInParameter(dbCommand, "Session", DbType.Int32, Me.userID)
        db.AddInParameter(dbCommand, "SessionKey", DbType.String, key)
        db.AddInParameter(dbCommand, "SessionValue", DbType.String, "")

        Dim i As Integer = 0

        Select Case key_type
            Case "string"
                tmpObj = "None"
            Case "number"
                tmpObj = 0
        End Select

        Try
            Using rs As IDataReader = db.ExecuteReader(dbCommand)
                While rs.Read
                    tmpObj = rs("SessionValue")
                End While
                rs.Close()
            End Using
        Catch ex As Exception
            Return tmpObj
            Me.errText = ex.Message.ToString
        End Try

        Try
            If IsNumeric(tmpObj) Then
                tmpObj = Convert.ToInt32(tmpObj)
            Else
                tmpObj = Convert.ToString(tmpObj)
            End If
        Catch ex As Exception
            tmpObj = Convert.ToString(tmpObj)
        End Try
       
        Return tmpObj
    End Function

    Public Function RemoveElement(ByRef key As String) As Object
        Dim sqlCommand As String = "MySessionProcess"
        Dim i As Integer = 0

        Try
            Using dbCommand As DbCommand = db.GetStoredProcCommand(sqlCommand)
                db.AddInParameter(dbCommand, "Action", DbType.String, "DELETE")
                db.AddInParameter(dbCommand, "Session", DbType.Int32, Me.userID)
                db.AddInParameter(dbCommand, "SessionKey", DbType.String, key)
                db.AddInParameter(dbCommand, "SessionValue", DbType.String, "")

                i = db.ExecuteNonQuery(dbCommand)
            End Using
        Catch ex As Exception
            Return "NoSession"
            Me.errText = ex.Message.ToString
        End Try

        Return i
    End Function

    Public Function PrintSession(ByVal uID As Integer) As String
        Dim obj As Object = "None"
        Dim retStr As New System.Text.StringBuilder
        Dim xFile As New XmlDocument
        Dim sFile As String = ""
        Dim xLoaded As Boolean = False

        Dim sqlCommand As String = "SELECT * FROM Session_Status WHERE userID = " & uID
        Dim dbCommand As DbCommand = db.GetSqlStringCommand(sqlCommand)

        Try
            Using rs As IDataReader = db.ExecuteReader(dbCommand)
                If rs.Read() Then
                    xFile.LoadXml(rs("SessionData"))
                    xLoaded = True
                    rs.Close()
                End If
            End Using
        Catch ex As Exception
            Return "Session File Not Yet Created... (File Not Found For User: " & uID & ") " & ex.Message.ToString
        End Try

        If Not xLoaded Then
            Return "<b>No Session File Found...</b>"
            Exit Function
        End If

        'Attempt to find the element given the "key" for that tag
        Dim doc As XmlNodeList = xFile.GetElementsByTagName("session")
        Dim node As XmlNode = doc.Item(0)
        Dim ele As XmlNode
        Dim snodes As Integer = node.ChildNodes.Count

        retStr.Append("<ul>")
        retStr.Append("<li><b>Session File: " & sFile & "</b></li>")
        If snodes > 1 Then

            For Each ele In node
                retStr.Append("<li>")
                retStr.Append("<b>Object:</b> " & ele.Name & " | ")
                retStr.Append("<b>Value:</b> " & ele.InnerText)
                retStr.Append("</li>")
            Next

            retStr.Append("<li><b>Session On: " & HttpContext.Current.Session("LoggedIn") & "</b></li>")
        End If
        retStr.Append("</ul>")

        Return retStr.ToString
    End Function

End Class
