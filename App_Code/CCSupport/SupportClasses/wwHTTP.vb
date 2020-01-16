'
'West Wind HTTP Access class

'This class is a full featured wrapper around the native .NET WebRequest
'class to provide a simpler front end. It supports easier POST mechanisms
'(UrlEncoded, Multi-Part Form, XML and raw) and the ability to directly
'retrieve HTTP content into strings, files with minimal lines of code.

'The class also provides automated cookie and state handling and simplified
'proxy and authentication mechanisms to provide a simple single level class
'interface. The underlying WebRequest is also exposed so you will not loose
'any functionality in that existing .NET BCL class.

'Copyright 2002-2004, West Wind Technologies
'Author: Rick Strahl
'West Wind Technologies
'http://www.west-wind.com/

'

Imports Microsoft.VisualBasic
Imports System
Imports System.Net
Imports System.IO
Imports System.Text

Imports System.Threading

Namespace NVMS.InternetTools
    ' <summary>
    ' An HTTP wrapper class that abstracts away the common needs for adding post keys
    ' and firing update events as data is received. This class is real easy to use
    ' with many operations requiring single method calls.
    ' </summary>
    Public Class wwHttp
        ' <summary>
        ' Determines how data is POSTed when when using AddPostKey() and other methods
        ' of posting data to the server. Support UrlEncoded, Multi-Part, XML and Raw modes.
        ' </summary>
        Public Property PostMode() As HttpPostMode
            Get
                Return Me.nPostMode
            End Get
            Set(ByVal value As HttpPostMode)
                Me.nPostMode = value
            End Set
        End Property

        ' <summary>
        '  User name used for Authentication. 
        '  To use the currently logged in user when accessing an NTLM resource you can use "AUTOLOGIN".
        ' </summary>
        Public Property Username() As String
            Get
                Return Me.cUsername
            End Get
            Set(ByVal value As String)
                cUsername = value
            End Set
        End Property

        ' <summary>
        ' Password for Authentication.
        ' </summary>
        Public Property Password() As String
            Get
                Return Me.cPassword
            End Get
            Set(ByVal value As String)
                Me.cPassword = value
            End Set
        End Property

        ' <summary>
        ' Address of the Proxy Server to be used.
        ' Use optional DEFAULTPROXY value to specify that you want to IE's Proxy Settings
        ' </summary>
        Public Property ProxyAddress() As String
            Get
                Return Me.cProxyAddress
            End Get
            Set(ByVal value As String)
                Me.cProxyAddress = value
            End Set
        End Property

        ' <summary>
        ' Semicolon separated Address list of the servers the proxy is not used for.
        ' </summary>
        Public Property ProxyBypass() As String
            Get
                Return Me.cProxyBypass
            End Get
            Set(ByVal value As String)
                Me.cProxyBypass = value
            End Set
        End Property

        ' <summary>
        ' Username for a password validating Proxy. Only used if the proxy info is set.
        ' </summary>
        Public Property ProxyUsername() As String
            Get
                Return Me.cProxyUsername
            End Get
            Set(ByVal value As String)
                Me.cProxyUsername = value
            End Set
        End Property
        ' <summary>
        ' Password for a password validating Proxy. Only used if the proxy info is set.
        ' </summary>
        Public Property ProxyPassword() As String
            Get
                Return Me.cProxyPassword
            End Get
            Set(ByVal value As String)
                Me.cProxyPassword = value
            End Set
        End Property

        ' <summary>
        ' Timeout for the Web request in seconds. Times out on connection, read and send operations.
        ' Default is 30 seconds.
        ' </summary>
        Public Property Timeout() As Integer
            Get
                Return Me.nConnectTimeout
            End Get
            Set(ByVal value As Integer)
                Me.nConnectTimeout = value
            End Set
        End Property

        ' <summary>
        ' Returns whether the last request was cancelled through one of the
        ' events.
        ' </summary>
        Public Property Cancelled() As Boolean
            Get
                Return Me.bCancelled
            End Get
            Set(ByVal value As Boolean)
                Me.bCancelled = value
            End Set
        End Property
        Private bCancelled As Boolean

        ' <summary>
        ' Error Message if the Error Flag is set or an error value is returned from a method.
        ' </summary>
        Public Property ErrorMessage() As String
            Get
                Return Me.cErrorMessage
            End Get
            Set(ByVal value As String)
                Me.cErrorMessage = value
            End Set
        End Property

        ' <summary>
        ' Error flag if an error occurred.
        ' </summary>
        Public Property [Error]() As Boolean
            Get
                Return Me.bError
            End Get
            Set(ByVal value As Boolean)
                Me.bError = value
            End Set
        End Property

        ' <summary>
        ' Determines whether errors cause exceptions to be thrown. By default errors 
        ' are handled in the class and the Error property is set for error conditions.
        ' (not implemented at this time).
        ' </summary>
        Public Property ThrowExceptions() As Boolean
            Get
                Return bThrowExceptions
            End Get
            Set(ByVal value As Boolean)
                Me.bThrowExceptions = value
            End Set
        End Property

        ' <summary>
        ' If set to a non-zero value will automatically track cookies.
        ' </summary>
        Public Property HandleCookies() As Boolean
            Get
                Return Me.bHandleCookies
            End Get
            Set(ByVal value As Boolean)
                Me.bHandleCookies = value
            End Set
        End Property

        ' <summary>
        ' Holds the internal Cookie collection before or after a request. This 
        ' collection is used only if HandleCookies is set to .t. which also causes it
        '  to capture cookies and repost them on the next request.
        ' </summary>
        Public Property Cookies() As CookieCollection
            Get
                If Me.oCookies Is Nothing Then
                    Me.Cookies = New CookieCollection()
                End If

                Return Me.oCookies
            End Get
            Set(ByVal value As CookieCollection)
                Me.oCookies = value
            End Set
        End Property

        ' <summary>
        ' WebResponse object that is accessible after the request is complete and 
        ' allows you to retrieve additional information about the completed request.
        '
        '   The Response Stream is already closed after the GetUrl methods complete 
        ' (except GetUrlResponse()) but you can access the Response object members 
        ' and collections to retrieve more detailed information about the current 
        ' request that completed.

        ' </summary>
        Public Property WebResponse() As HttpWebResponse
            Get
                Return Me.oWebResponse
            End Get
            Set(ByVal value As HttpWebResponse)
                Me.oWebResponse = value
            End Set
        End Property

        ' <summary>
        ' WebRequest object that can be manipulated and set up for the request if you
        '  called .
        ' 
        ' Note: This object must be recreated and reset for each request, since a 
        ' request's life time is tied to a single request. This object is not used if
        '  you specify a URL on any of the GetUrl methods since this causes a default
        '  WebRequest to be created.
        ' </summary>
        Public Property WebRequest() As HttpWebRequest
            Get
                Return Me.oWebRequest
            End Get
            Set(ByVal value As HttpWebRequest)
                Me.oWebRequest = value
            End Set
        End Property

        ' <summary>
        ' The buffersize used for the Send and Receive operations
        ' </summary>
        Public Property BufferSize() As Integer
            Get
                Return Me.nBufferSize
            End Get
            Set(ByVal value As Integer)
                Me.nBufferSize = value
            End Set
        End Property
        Private nBufferSize As Integer = 100

        ' <summary>
        ' Lets you specify the User Agent  browser string that is sent to the server.
        '  This allows you to simulate a specific browser if necessary.
        ' </summary>
        Public Property UserAgent() As String
            Get
                Return cUserAgent
            End Get
            Set(ByVal value As String)
                cUserAgent = value
            End Set
        End Property
        Private cUserAgent As String = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/29.0.1547.66 Safari/537.36"


        ' *** member properties
        'string cPostBuffer = "";
        Private oPostStream As MemoryStream
        Private oPostData As BinaryWriter

        Private nPostMode As HttpPostMode = HttpPostMode.UrlEncoded

        Private nConnectTimeout As Integer = 30

        Private cUsername As String = ""
        Private cPassword As String = ""

        Private cProxyAddress As String = ""
        Private cProxyBypass As String = ""
        Private cProxyUsername As String = ""
        Private cProxyPassword As String = ""

        Private bThrowExceptions As Boolean = False
        Private bHandleCookies As Boolean = False

        Private cErrorMessage As String = ""
        Private bError As Boolean = False

        Private oWebResponse As HttpWebResponse
        Private oWebRequest As HttpWebRequest
        Private oCookies As CookieCollection

        Private cMultiPartBoundary As String = "-----------------------------7cf2a327f01ae"

        ' <summary>
        ' The wwHttp Default Constructor
        ' </summary>
        Public Sub wwHTTP()
        End Sub

        ' <summary>
        ' Creates a new WebRequest instance that can be set prior to calling the 
        ' various Get methods. You can then manipulate the WebRequest property, to 
        ' custom configure the request.
        ' 
        ' Instead of passing a URL you  can then pass null.
        ' 
        ' Note - You need a new Web Request for each and every request so you need to
        '  set this object for every call if you manually customize it.
        ' </summary>
        ' <param name="String Url">
        ' The Url to access with this WebRequest
        ' </param>
        ' <returns>Boolean</returns>
        Public Function CreateWebRequestObject(ByVal Url As String) As Boolean
            Try
                Me.WebRequest = CType(System.Net.WebRequest.Create(Url), HttpWebRequest)
            Catch ex As Exception
                Me.ErrorMessage = ex.Message.ToString & Common.CatchInnerException(ex.InnerException)
                Return False
            End Try

            Return True
        End Function

        Public Function ConnectionTest(ByVal Url As String) As Boolean
            Dim req As System.Net.HttpWebRequest
            Dim res As System.Net.HttpWebResponse
            ConnectionTest = False
            Try
                req = CType(System.Net.HttpWebRequest.Create(Url), System.Net.HttpWebRequest)
                res = CType(req.GetResponse(), System.Net.HttpWebResponse)
                req.Abort()
                If res.StatusCode = System.Net.HttpStatusCode.OK Then
                    Me.ErrorMessage = res.StatusCode.ToString
                    ConnectionTest = True
                End If
            Catch weberrt As System.Net.WebException
                ConnectionTest = False
            Catch except As Exception
                ConnectionTest = False
            End Try
        End Function

        ' <summary>
        ' Adds POST form variables to the request buffer.
        ' PostMode determines how parms are handled.
        ' </summary>
        ' <param name="Key">Key value or raw buffer depending on post type</param>
        ' <param name="Value">Value to store. Used only in key/value pair modes</param>
        Public Sub AddPostKey(ByVal Key As String, ByVal Value As Byte())

            If Me.oPostData Is Nothing Then
                Me.oPostStream = New MemoryStream()
                Me.oPostData = New BinaryWriter(Me.oPostStream)
            End If

            If Key = "RESET" Then
                Me.oPostStream = New MemoryStream()
                Me.oPostData = New BinaryWriter(Me.oPostStream)
            End If

            Select Case Me.nPostMode
                Case HttpPostMode.UrlEncoded
                    Me.oPostData.Write(Encoding.GetEncoding(1252).GetBytes(Key & "=" & wwHttpUtils.UrlEncode(Encoding.GetEncoding(1252).GetString(Value)) & "&"))
                Case HttpPostMode.MultiPart
                    Me.oPostData.Write(Encoding.GetEncoding(1252).GetBytes("--" & Me.cMultiPartBoundary & Constants.vbCrLf & "Content-Disposition: form-data; name=""" & Key & """" & Constants.vbCrLf & Constants.vbCrLf))

                    Me.oPostData.Write(Value)

                    Me.oPostData.Write(Encoding.GetEncoding(1252).GetBytes(Constants.vbCrLf))
                Case Else ' Raw or Xml modes
                    Me.oPostData.Write(Value)
            End Select
        End Sub

        ' <summary>
        ' Adds POST form variables to the request buffer.
        ' PostMode determines how parms are handled.
        ' </summary>
        ' <param name="Key">Key value or raw buffer depending on post type</param>
        ' <param name="Value">Value to store. Used only in key/value pair modes</param>
        Public Sub AddPostKey(ByVal Key As String, ByVal Value As String)
            Me.AddPostKey(Key, Encoding.GetEncoding(1252).GetBytes(Value))
        End Sub

        ' <summary>
        ' Adds a fully self contained POST buffer to the request.
        ' Works for XML or previously encoded content.
        ' </summary>
        ' <param name="FullPostBuffer">String based full POST buffer</param>
        Public Sub AddPostKey(ByVal FullPostBuffer As String)
            Me.AddPostKey(Nothing, FullPostBuffer)
        End Sub

        ' <summary>
        ' Adds a fully self contained POST buffer to the request.
        ' Works for XML or previously encoded content.
        ' </summary>
        ' <param name="PostBuffer">Byte array of a full POST buffer</param>
        Public Sub AddPostKey(ByVal FullPostBuffer As Byte())
            Me.AddPostKey(Nothing, FullPostBuffer)
        End Sub

        ' <summary>
        ' Allows posting a file to the Web Server. Make sure that you 
        ' set PostMode
        ' </summary>
        ' <param name="Key"></param>
        ' <param name="FileName"></param>
        ' <returns></returns>
        Public Function AddPostFile(ByVal Key As String, ByVal FileName As String) As Boolean
            Dim lcFile As Byte()

            If Me.nPostMode <> HttpPostMode.MultiPart Then
                Me.cErrorMessage = "File upload allowed only with Multi-part forms"
                Me.bError = True
                Return False
            End If

            Try
                Dim loFile As FileStream = New FileStream(FileName, System.IO.FileMode.Open, System.IO.FileAccess.Read)

                lcFile = New Byte(loFile.Length - 1) {}
                loFile.Read(lcFile, 0, CInt(Fix(loFile.Length)))
                loFile.Close()
            Catch e As Exception
                Me.cErrorMessage = e.Message.ToString
                Me.bError = True
                Return False
            End Try
            If Me.oPostData Is Nothing Then
                Me.oPostStream = New MemoryStream()
                Me.oPostData = New BinaryWriter(Me.oPostStream)
            End If

            Me.oPostData.Write(Encoding.GetEncoding(1252).GetBytes("--" & Me.cMultiPartBoundary & Constants.vbCrLf & "Content-Disposition: form-data; name=""" & Key & """ filename=""" & New FileInfo(FileName).Name & """" & Constants.vbCrLf & Constants.vbCrLf))

            Me.oPostData.Write(lcFile)

            Me.oPostData.Write(Encoding.GetEncoding(1252).GetBytes(Constants.vbCrLf))

            Return True
        End Function

        ' <summary>
        ' Allows posting a file to the Web Server. Make sure that you 
        ' set PostMode Using Memory Stream instead of file
        ' </summary>
        ' <param name="Key"></param>
        ' <param name="FileName"></param>
        ' <returns></returns>
        Public Function AddPostFile(ByVal Key As String, ByVal FileContents As MemoryStream) As Boolean
            Dim lcFile As Byte()

            If Me.nPostMode <> HttpPostMode.MultiPart Then
                Me.cErrorMessage = "File upload allowed only with Multi-part forms"
                Me.bError = True
                Return False
            End If

            Try
                Dim loFile As MemoryStream = FileContents

                lcFile = New Byte(loFile.Length - 1) {}
                loFile.Read(lcFile, 0, CInt(Fix(loFile.Length)))
                loFile.Close()
            Catch e As Exception
                Me.cErrorMessage = e.Message.ToString
                Me.bError = True
                Return False
            End Try
            If Me.oPostData Is Nothing Then
                Me.oPostStream = New MemoryStream()
                Me.oPostData = New BinaryWriter(Me.oPostStream)
            End If

            Me.oPostData.Write(Encoding.GetEncoding(1252).GetBytes("--" & Me.cMultiPartBoundary & Constants.vbCrLf & "Content-Disposition: form-data; name=""" & Key & """ filename=""bpoxml.xml""" & Constants.vbCrLf & Constants.vbCrLf))

            Me.oPostData.Write(lcFile)

            Me.oPostData.Write(Encoding.GetEncoding(1252).GetBytes(Constants.vbCrLf))

            Return True
        End Function

        ' <summary>
        ' Return a the result from an HTTP Url into a StreamReader.
        ' Client code should call Close() on the returned object when done reading.
        ' </summary>
        ' <param name="Url">Url to retrieve.</param>
        ' <param name="WebRequest">An HttpWebRequest object that can be passed in with properties preset.</param>
        ' <returns></returns>
        Private Function GetUrlStream(ByVal Url As String) As StreamReader
            Dim enc As Encoding

            Dim Response As HttpWebResponse = Me.GetUrlResponse(Url)
            If Response Is Nothing Then
                Return Nothing
            End If

            Try
                If Response.ContentEncoding.Length > 0 Then
                    enc = Encoding.GetEncoding(Response.ContentEncoding)
                Else
                    enc = Encoding.GetEncoding(1252)
                End If
            Catch
                ' *** Invalid encoding passed
                enc = Encoding.GetEncoding(1252)
            End Try

            ' *** drag to a stream
            Dim strResponse As StreamReader = New StreamReader(Response.GetResponseStream(), enc)
            Return strResponse
        End Function

        ' <summary>
        ' Return an HttpWebResponse object for a request. You can use the Response to
        ' read the result as needed. This is a low level method. Most of the other 'Get'
        ' methods call this method and process the results further.
        ' </summary>
        ' <remarks>Important: The Response object's Close() method must be called when you are done with the object.</remarks>
        ' <param name="Url">Url to retrieve.</param>
        ' <returns>An HttpWebResponse Object</returns>
        Public Function GetUrlResponse(ByVal Url As String) As HttpWebResponse
            Me.Cancelled = False

            Try
                Me.bError = False
                Me.cErrorMessage = ""
                Me.bCancelled = False

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

                If Me.WebRequest Is Nothing Then
                    Me.WebRequest = CType(System.Net.WebRequest.Create(Url), HttpWebRequest)
                    Me.WebRequest.Headers.Add("Cache", "no-cache")
                End If

                Me.WebRequest.UserAgent = Me.cUserAgent
                Me.WebRequest.Timeout = Me.nConnectTimeout * 1000

                ' *** Handle Security for the request
                If Me.cUsername.Length > 0 Then
                    If Me.cUsername = "AUTOLOGIN" Then
                        Me.WebRequest.Credentials = CredentialCache.DefaultCredentials
                    Else
                        Me.WebRequest.Credentials = New NetworkCredential(Me.cUsername, Me.cPassword)
                    End If
                End If

                ' *** Handle Proxy Server configuration
                If Me.cProxyAddress.Length > 0 Then
                    If Me.cProxyAddress = "DEFAULTPROXY" Then
                        Me.WebRequest.Proxy = Net.WebRequest.DefaultWebProxy()

                    Else
                        Dim loProxy As WebProxy = New WebProxy(Me.cProxyAddress, True)
                        If Me.cProxyBypass.Length > 0 Then
                            loProxy.BypassList = Me.cProxyBypass.Split(";"c)
                        End If

                        If Me.cProxyUsername.Length > 0 Then
                            loProxy.Credentials = New NetworkCredential(Me.cProxyUsername, Me.cProxyPassword)
                        End If

                        Me.WebRequest.Proxy = loProxy
                    End If
                End If

                ' *** Handle cookies - automatically re-assign 
                If Me.bHandleCookies OrElse (Not Me.oCookies Is Nothing AndAlso Me.oCookies.Count > 0) Then
                    Me.WebRequest.CookieContainer = New CookieContainer()
                    If Not Me.oCookies Is Nothing AndAlso Me.oCookies.Count > 0 Then
                        Me.WebRequest.CookieContainer.Add(Me.oCookies)
                    End If
                End If

                ' *** Deal with the POST buffer if any
                If Not Me.oPostData Is Nothing Then
                    Me.WebRequest.Method = "POST"
                    Select Case Me.nPostMode
                        Case HttpPostMode.UrlEncoded
CaseLabel1:
                            Me.WebRequest.ContentType = "application/x-www-form-urlencoded"
                            ' strip off any trailing & which can cause problems with some 
                            ' http servers
                            '							if (this.cPostBuffer.EndsWith("&"))
                            '								this.cPostBuffer = this.cPostBuffer.Substring(0,this.cPostBuffer.Length-1);
                        Case HttpPostMode.MultiPart
                            Me.WebRequest.ContentType = "multipart/form-data; boundary=" & Me.cMultiPartBoundary
                            Me.oPostData.Write(Encoding.GetEncoding(1252).GetBytes("--" & Me.cMultiPartBoundary & Constants.vbCrLf))
                        Case HttpPostMode.Xml
                            Me.WebRequest.ContentType = "text/xml"
                        Case HttpPostMode.Raw
                            Me.WebRequest.ContentType = "application/octet-stream"
                        Case Else
                            GoTo CaseLabel1
                    End Select


                    Dim loPostData As Stream = Me.WebRequest.GetRequestStream()

                    If Me.SendDataEvent Is Nothing Then
                        Me.oPostStream.WriteTo(loPostData) ' Simplest version - no events
                    Else
                        Me.StreamPostBuffer(loPostData) ' Send in chunks and fire events
                    End If

                    '*** Close the memory stream
                    Me.oPostStream.Close()
                    Me.oPostStream = Nothing

                    '*** Close the Binary Writer
                    Me.oPostData.Close()
                    Me.oPostData = Nothing

                    '*** Close Request Stream
                    loPostData.Close()

                    ' *** If user cancelled the 'upload' exit
                    If Me.Cancelled Then
                        Me.ErrorMessage = "HTTP Request was cancelled."
                        Me.Error = True
                        Return Nothing
                    End If
                End If

                ' *** Retrieve the response headers 
                Dim Response As HttpWebResponse = CType(Me.WebRequest.GetResponse(), HttpWebResponse)
                Me.oWebResponse = Response

                ' *** Close out the request - it cannot be reused
                Me.WebRequest = Nothing

                ' ** Save cookies the server sends
                If Me.bHandleCookies Then
                    If Response.Cookies.Count > 0 Then
                        If Me.oCookies Is Nothing Then
                            Me.oCookies = Response.Cookies
                        Else
                            ' ** If we already have cookies update the list
                            For Each oRespCookie As Cookie In Response.Cookies
                                Dim bMatch As Boolean = False
                                For Each oReqCookie As Cookie In Me.oCookies
                                    If oReqCookie.Name = oRespCookie.Name Then
                                        oReqCookie.Value = oRespCookie.Value
                                        bMatch = True
                                        Exit For
                                    End If
                                Next oReqCookie ' for each ReqCookies
                                If (Not bMatch) Then
                                    Me.oCookies.Add(oRespCookie)
                                End If
                            Next oRespCookie ' for each Response.Cookies
                        End If ' this.Cookies == null
                    End If ' if Response.Cookie.Count > 0
                End If ' if this.bHandleCookies = 0


                Return Response
            Catch e As Exception
                If Me.bThrowExceptions Then
                    Throw e
                End If

                Me.cErrorMessage = e.Message.ToString & Common.CatchInnerException(e.InnerException)
                Me.bError = True
                Return Nothing
            End Try
        End Function

        ' <summary>
        ' Sends the Postbuffer to the server
        ' </summary>
        ' <param name="loPostData"></param>
        Protected Sub StreamPostBuffer(ByVal loPostData As Stream)

            If Me.oPostStream.Length < Me.BufferSize Then
                Me.oPostStream.WriteTo(loPostData)

                ' *** Handle Send Data Even
                ' *** Here just let it know we're done
                If Not Me.SendDataEvent Is Nothing Then
                    Dim Args As ReceiveDataEventArgs = New ReceiveDataEventArgs()
                    Args.CurrentByteCount = Me.oPostStream.Length
                    Args.Done = True
                    RaiseEvent SendData(Me, Args)
                End If
            Else
                ' *** Send data up in 8k blocks
                Dim Buffer As Byte() = Me.oPostStream.GetBuffer()
                Dim lnSent As Integer = 0
                Dim lnToSend As Integer = CInt(Fix(Me.oPostStream.Length))
                Dim lnCurrent As Integer = 1
                Do
                    If lnToSend < 1 OrElse lnCurrent < 1 Then
                        If Not Me.SendDataEvent Is Nothing Then
                            Dim Args As ReceiveDataEventArgs = New ReceiveDataEventArgs()
                            Args.CurrentByteCount = lnSent
                            Args.TotalBytes = Buffer.Length
                            Args.Done = True
                            RaiseEvent SendData(Me, Args)
                        End If
                        Exit Do
                    End If

                    lnCurrent = lnToSend

                    If lnCurrent > Me.BufferSize Then
                        lnCurrent = BufferSize
                        lnToSend = lnToSend - lnCurrent
                    Else
                        lnToSend = lnToSend - lnCurrent
                    End If

                    loPostData.Write(Buffer, lnSent, lnCurrent)

                    lnSent = lnSent + lnCurrent

                    If Not Me.SendDataEvent Is Nothing Then
                        Dim Args As ReceiveDataEventArgs = New ReceiveDataEventArgs()
                        Args.CurrentByteCount = lnSent
                        Args.TotalBytes = Buffer.Length
                        If Buffer.Length = lnSent Then
                            Args.Done = True
                            RaiseEvent SendData(Me, Args)
                            Exit Do
                        End If
                        RaiseEvent SendData(Me, Args)

                        If Args.Cancel Then
                            Me.Cancelled = True
                            Exit Do
                        End If
                    End If
                Loop
            End If
        End Sub

        ' <summary>
        ' Retrieves the content of a Url into a string.
        ' </summary>
        ' <remarks>Fires the ReceiveData event</remarks>
        ' <param name="Url">Url to retrieve</param>
        ' <returns></returns>
        Public Function GetUrl(ByVal Url As String) As String
            Return Me.GetUrl(Url, 8192)
        End Function

        ' <summary>
        ' Retrieves the content of a Url into a string.
        ' </summary>
        ' <remarks>Fires the ReceiveData event</remarks>
        ' <param name="Url">Url to retrieve</param>
        ' <param name="BufferSize">Optional ReadBuffer Size</param>
        ' <returns></returns>
        Public Function GetUrl(ByVal Url As String, ByVal BufferSize As Long) As String

            Dim oHttpResponse As StreamReader = Me.GetUrlStream(Url)
            If oHttpResponse Is Nothing Then
                Return ""
            End If

            Dim lnSize As Long = BufferSize
            If Me.oWebResponse.ContentLength > 0 Then
                lnSize = Me.oWebResponse.ContentLength
            Else
                lnSize = 0
            End If

            Dim loWriter As StringBuilder = New StringBuilder(CInt(Fix(lnSize)))

            Dim lcTemp As Char() = New Char(BufferSize - 1) {}

            Dim oArgs As ReceiveDataEventArgs = New ReceiveDataEventArgs()
            oArgs.TotalBytes = lnSize

            lnSize = 1
            Dim lnCount As Integer = 0
            Dim lnTotalBytes As Long = 0

            Do While lnSize > 0
                lnSize = oHttpResponse.Read(lcTemp, 0, CInt(Fix(BufferSize)))
                If lnSize > 0 Then
                    loWriter.Append(lcTemp, 0, CInt(Fix(lnSize)))
                    lnCount += 1
                    lnTotalBytes += lnSize

                    ' *** Raise an event if hooked up
                    If Not Me.ReceiveDataEvent Is Nothing Then
                        ' *** Update the event handler
                        oArgs.CurrentByteCount = lnTotalBytes
                        oArgs.NumberOfReads = lnCount
                        oArgs.CurrentChunk = lcTemp
                        RaiseEvent ReceiveData(Me, oArgs)

                        ' *** Check for cancelled flag
                        If oArgs.Cancel Then
                            Me.bCancelled = True
                            GoTo CloseDown
                        End If
                    End If
                End If
                ' Thread.Sleep(1);  // Give up a timeslice
            Loop ' while


CloseDown:
            oHttpResponse.Close()

            ' *** Send Done notification
            If Not Me.ReceiveDataEvent Is Nothing AndAlso (Not oArgs.Cancel) Then
                ' *** Update the event handler
                oArgs.Done = True
                RaiseEvent ReceiveData(Me, oArgs)
            End If

            Return loWriter.ToString()
        End Function

        ' <summary>
        ' Retrieves URL into an Byte Array.
        ' </summary>
        ' <remarks>Fires the ReceiveData Event</remarks>
        ' <param name="Url">Url to read</param>
        ' <param name="BufferSize">Size of the buffer for each read. 0 = 8192</param>
        ' <returns></returns>
        Public Function GetUrlBytes(ByVal Url As String, ByVal BufferSize As Long) As Byte()
            Dim Response As HttpWebResponse = Me.GetUrlResponse(Url)
            Dim oHttpResponse As BinaryReader = New BinaryReader(Response.GetResponseStream())

            If oHttpResponse Is Nothing Then
                Return Nothing
            End If

            If BufferSize < 1 Then
                BufferSize = 8192
            End If

            Dim lnSize As Long = BufferSize
            If Response.ContentLength > 0 Then
                lnSize = Me.oWebResponse.ContentLength
            Else
                lnSize = 0
            End If

            Dim Result As Byte() = New Byte(lnSize - 1) {}
            Dim lcTemp As Byte() = New Byte(BufferSize - 1) {}

            Dim oArgs As ReceiveDataEventArgs = New ReceiveDataEventArgs()
            oArgs.TotalBytes = lnSize

            lnSize = 1
            Dim lnCount As Integer = 0
            Dim lnTotalBytes As Long = 0

            Do While lnSize > 0
                If lnTotalBytes + BufferSize > Me.oWebResponse.ContentLength Then
                    BufferSize = Me.oWebResponse.ContentLength - lnTotalBytes
                End If

                lnSize = oHttpResponse.Read(Result, CInt(Fix(lnTotalBytes)), CInt(Fix(BufferSize)))
                If lnSize > 0 Then
                    lnCount += 1
                    lnTotalBytes += lnSize

                    ' *** Raise an event if hooked up
                    If Not Me.ReceiveDataEvent Is Nothing Then
                        ' *** Update the event handler
                        oArgs.CurrentByteCount = lnTotalBytes
                        oArgs.NumberOfReads = lnCount
                        oArgs.CurrentChunk = Nothing ' don't send anything here
                        RaiseEvent ReceiveData(Me, oArgs)

                        ' *** Check for cancelled flag
                        If oArgs.Cancel Then
                            Me.bCancelled = True
                            GoTo CloseDown
                        End If
                    End If
                End If
            Loop ' while


CloseDown:
            oHttpResponse.Close()

            ' *** Send Done notification
            If Not Me.ReceiveDataEvent Is Nothing AndAlso (Not oArgs.Cancel) Then
                ' *** Update the event handler
                oArgs.Done = True
                RaiseEvent ReceiveData(Me, oArgs)
            End If

            Return Result
        End Function

        ' <summary>
        ' Writes the output from the URL request to a file firing events.
        ' </summary>
        ' <param name="Url">Url to fire</param>
        ' <param name="BufferSize">Buffersize - how often to fire events</param>
        ' <param name="lcOutputFile">File to write response to</param>
        ' <returns>true or false</returns>
        Public Function GetUrlFile(ByVal Url As String, ByVal BufferSize As Long, ByVal lcOutputFile As String) As Boolean
            Dim Result As Byte() = Me.GetUrlBytes(Url, BufferSize)
            If Result Is Nothing Then
                Return False
            End If

            Dim File As FileStream = New FileStream(lcOutputFile, FileMode.OpenOrCreate, FileAccess.Write)
            File.Write(Result, 0, CInt(Fix(Me.WebResponse.ContentLength)))
            File.Close()

            Return True
        End Function

#Region "Events and Event Delegates and Arguments"

        ' <summary>
        ' Fires progress events when receiving data from the server
        ' </summary>
        Public Event ReceiveData As ReceiveDataDelegate
        Public Delegate Sub ReceiveDataDelegate(ByVal sender As Object, ByVal e As ReceiveDataEventArgs)

        ' <summary>
        ' Fires progress events when using GetUrlEvents() to retrieve a URL.
        ' </summary>
        Public Event SendData As ReceiveDataDelegate

        ' <summary>
        ' Event arguments passed to the ReceiveData event handler on each block of data sent
        ' </summary>
        Public Class ReceiveDataEventArgs
            ' <summary>
            ' Size of the cumulative bytes read in this request
            ' </summary>
            Public CurrentByteCount As Long = 0

            ' <summary>
            ' The number of total bytes of this request
            ' </summary>
            Public TotalBytes As Long = 0

            ' <summary>
            ' The number of reads that have occurred - how often has this event been called.
            ' </summary>
            Public NumberOfReads As Integer = 0

            ' <summary>
            ' The current chunk of data being read
            ' </summary>
            Public CurrentChunk As Char()

            ' <summary>
            ' Flag set if the request is currently done.
            ' </summary>
            Public Done As Boolean = False

            ' <summary>
            ' Flag to specify that you want the current request to cancel. This is a write-only flag
            ' </summary>
            Public Cancel As Boolean = False
        End Class
#End Region

    End Class

    ' <summary>
    ' Enumeration of the various HTTP POST modes supported by wwHttp
    ' </summary>

    Public Enum HttpPostMode
        UrlEncoded
        MultiPart
        Xml
        Raw
    End Enum


End Namespace
