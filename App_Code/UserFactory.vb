Public NotInheritable Class UserFactory
    Public Class MyUser
        Public UserID as Integer = 0
        Public Username as String = ""
        Public UserAuth As Boolean = False
    End Class

    Public Shared Function GetCurrentUser() As MyUser
        ' If the HttpContext is missing we cannot access 
        ' the session or get any identity from logged-in users.

        If HttpContext.Current Is Nothing Then
            Throw New NullReferenceException("The HttpContext is missing.")
        End If

        If HttpContext.Current.Session IsNot Nothing AndAlso HttpContext.Current.Session("userID") IsNot Nothing Then
            ' Check if there is a MyUser object still available in session.

            Try
                Dim user As New  MyUser
                user.UserID = HttpContext.Current.Session("userID")

                If user IsNot Nothing Then
                    Return user
                End If
            Catch generatedExceptionName As Exception
            End Try
        End If

        If HttpContext.Current.User.Identity.IsAuthenticated AndAlso Not String.IsNullOrEmpty(HttpContext.Current.User.Identity.Name) Then
            ' Create a new MyUser instance from the authenticated
            ' user name.

            Dim user As New MyUser
            user.Username = HttpContext.Current.User.Identity.Name
            user.UserID = HttpContext.Current.Session("userID")
            user.UserAuth = HttpContext.Current.User.Identity.IsAuthenticated

            ' Add the new MyUser instance to the session for
            ' further requests.
            Return user
        Else
            ' If not authenticated we trow an SecurityException which
            ' can be identified in the AJAX response (res.error.Type).
            ' If this happens we redirect to the login page or ask
            ' for user credentials to get authenticated with the built-in
            ' AjaxPro authentication service.

            Throw New System.Security.SecurityException("Not authenticated.")
        End If
    End Function
End Class