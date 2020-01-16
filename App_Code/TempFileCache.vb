Imports Microsoft.VisualBasic
Imports System.IO

Public Class TempFileCache

    'Create Functions To Cache Data for 24 hours
    Public Shared Function getLastModified(ByVal filespec As String) As Integer
        Dim s As Date
        Dim hdiff As Integer

        If Not (File.Exists(filespec)) Then
            Return 99
        Else
            s = File.GetLastWriteTime(filespec)
            'Get hour diff of last modified
            hdiff = DateDiff(DateInterval.Hour, s, Now())
            Return Math.Round(hdiff, 0)
        End If
    End Function

    Public Shared Function getLastModifiedDay(ByVal filespec As String) As Integer
        Dim s As Date
        Dim hdiff As Integer

        If Not (File.Exists(filespec)) Then
            Return 99
        Else
            s = File.GetLastWriteTime(filespec)
            'Get hour diff of last modified
            hdiff = DateDiff(DateInterval.Day, s, Now())
            Return Math.Round(hdiff, 0)
        End If
    End Function

    Public Shared Function getLastModifiedDirectoryDay(ByVal filespec As String) As Integer
        Dim s As Date
        Dim hdiff As Integer

        If Not (Directory.Exists(filespec)) Then
            Return 99
        Else
            s = Directory.GetLastWriteTime(filespec)
            'Get hour diff of last modified
            hdiff = DateDiff(DateInterval.Day, s, Now())
            Return Math.Round(hdiff, 0)
        End If
    End Function

    Public Shared Function getLastModifiedMin(ByVal filespec As String) As Integer
        Dim s As Date
        Dim hdiff As Integer

        If Not (File.Exists(filespec)) Then
            Return 99
        Else
            s = File.GetLastWriteTime(filespec)
            'Get hour diff of last modified
            hdiff = DateDiff(DateInterval.Minute, s, Now())
            Return Math.Round(hdiff, 0)
        End If
    End Function

    Public Shared Function getLastModifiedMonth(ByVal filespec As String) As Integer
        Dim s As Date
        Dim hdiff As Integer

        If Not (File.Exists(filespec)) Then
            Return 99
        Else
            s = File.GetLastWriteTime(filespec)
            'Get month diff of last modified
            'hdiff = DateDiff(DateInterval.Month, s, Date.Today())
            hdiff = Month(s)
            Return Math.Round(hdiff, 0)
        End If
    End Function

    Public Shared Function fileCached(ByVal fileget As String, ByVal ele As String) As String
        Dim CacheFolder As String = ConfigurationManager.AppSettings("TempFolder")
        Dim filespec As String
        Dim rText As String

        filespec = CacheFolder & "\" & fileget

        If Not (File.Exists(filespec)) Then
            Return "<i>Data Current As Of: " & Now() & " (New File)</i>"
        Else
            Dim t As String = File.GetLastWriteTime(filespec)
            rText = "<i>Data Current As Of: " & t & "</i>"
            Return rText
        End If

    End Function

    Public Shared Function GetFile(ByVal filespec As String) As String
        Dim getFull As String

        If File.Exists(filespec) Then
            getFull = File.ReadAllText(filespec)
            Return getFull
        Else
            WriteFile(filespec, 0)
            Return ""
        End If
    End Function

    Public Shared Function DeleteFile(ByVal fileget As String) As String
        Dim CacheFolder As String = ConfigurationManager.AppSettings("TempFolder")
        Dim filespec As String
        filespec = CacheFolder & "\" & fileget

        If File.Exists(filespec) Then
            File.Delete(filespec)
            Return "File: <b>" & fileget & "</b> refreshed successfully..."
        Else
            Return "File <b>" & fileget & "</b> was not found..."
        End If
    End Function

    Public Shared Function DeleteTempFolderFiles(ByVal folder As String) As String
        Dim CacheFolder As String = ConfigurationManager.AppSettings("TempFolder")
        Dim filespec As String
        Dim FileAge As Integer = 5
        Dim test As New StringBuilder
        filespec = CacheFolder & "\" & folder

        If Directory.Exists(filespec) Then
            Dim allFiles() As String = Directory.GetFiles(filespec)
            For Each f As String In allFiles
                'test.Append(f & "<br>")
                If getLastModifiedMin(f) > FileAge Then
                    File.Delete(f)
                    'test.Append("Deleted: " & f & "<br>")
                End If
            Next
            test.Append(allFiles.Length & " Files Removed Successfully...")
            Return test.ToString()
        Else
            Return "Folder Does Not Exist... " & filespec
        End If
    End Function

    'Delete Files By Extension in given temp folder that are older than 5 min old.
    Public Shared Function DeleteTempFilesByType(ByVal folder As String, ByVal Extension As String) As String
        Dim CacheFolder As String = ConfigurationManager.AppSettings("TempFolder")
        Dim filespec As String
        Dim FileAge As Integer = 5
        Dim test As New StringBuilder
        filespec = CacheFolder & "\" & folder

        If Directory.Exists(filespec) Then
            Dim allFiles() As String = Directory.GetFiles(filespec)
            For Each f As String In allFiles
                If Right(f, 3).ToLower = Extension.ToLower Then
                    If getLastModifiedMin(f) > FileAge Then
                        File.Delete(f)
                        test.Append("Deleted: " & f & "<br>")
                    End If
                End If
            Next
            test.Append(allFiles.Length & " Files Removed Successfully...")
            Return test.ToString()
        Else
            Return "Folder Does Not Exist... " & filespec
        End If
    End Function

    '<summary>
    '(By Day) Delete all files in given folder - Older then given age (Default All Files - 0 Days)
    '</summary>
    Public Shared Function DeleteFilesByAge(ByVal folder As String, Optional ByVal FileAge As Integer = 0, Optional ByVal RemoveSubFolders As Boolean = False) As String
        Dim filespec As String
        Dim i As Integer = 0
        Dim x As Integer = 0
        Dim test As New StringBuilder
        filespec = folder

        If Directory.Exists(filespec) Then
            Dim allFiles() As String = Directory.GetFiles(filespec)
            Dim allFolders() As String = Directory.GetDirectories(filespec)

            If RemoveSubFolders Then
                If FileAge = 0 Then
                    For Each d As String In allFolders
                        test.Append("Directory: " & d & " Last Modified: " & getLastModifiedDirectoryDay(d) & "<br>")
                        Directory.Delete(d, True)
                        x += 1
                    Next
                Else
                    For Each d As String In allFolders
                        If getLastModifiedDirectoryDay(d) >= FileAge Then
                            test.Append("Directory: " & d & " Last Modified: " & getLastModifiedDirectoryDay(d) & "<br>")
                            Directory.Delete(d, True)
                            x += 1
                        End If
                    Next
                End If
            End If

            'Delete All Files If 0
            If FileAge = 0 Then
                For Each f As String In allFiles
                    test.Append("Files Deleted: " & f & " Last Modified: " & getLastModifiedDay(f) & "<br>")
                    File.Delete(f)
                    i += 1
                Next
            Else
                For Each f As String In allFiles
                    If getLastModifiedDay(f) >= FileAge Then
                        test.Append("Files Deleted: " & f & " Last Modified: " & getLastModifiedDay(f) & "<br>")
                        File.Delete(f)
                        i += 1
                    End If
                Next
            End If

            test.Append(x & " Directories Removed Successfully... <br>")
            test.Append(i & " Files Removed Successfully...")
            Return test.ToString()
        Else
            Return "Folder Does Not Exist... " & filespec
        End If
    End Function

    'Delete all files in given folder - Older then given age (Default All Files - 0 Min)
    Public Shared Function DeleteFilesByAgeMin(ByVal folder As String, Optional ByVal FileAge As Integer = 0) As String
        Dim filespec As String
        Dim i As Integer = 0
        Dim test As New StringBuilder
        filespec = folder

        If Directory.Exists(filespec) Then
            Dim allFiles() As String = Directory.GetFiles(filespec)
            For Each f As String In allFiles

                If getLastModifiedMin(f) > FileAge Then
                    File.Delete(f)
                    test.Append("Deleted: " & f & "<br>")
                    i += 1
                End If

            Next
            test.Append(i & " Files Removed Successfully...")
            Return test.ToString()
        Else
            Return "Folder Does Not Exist... " & filespec
        End If
    End Function

    Public Shared Sub WriteFile(ByVal filespec As String, ByVal sValue As String)
        File.WriteAllText(filespec, sValue)
    End Sub

    Public Shared Function WriteFileNoOverwrite(ByVal FileFolder As String, ByVal filespec As String, ByVal sValue As String) As String
        Dim fileNum As Integer = 0
        Dim ext As String = Right(filespec, 3)
        Dim FileNoExt As String = Left(filespec, Len(filespec) - 4)

        Try
            While File.Exists(FileFolder & filespec)
                If File.Exists(FileFolder & filespec) Then
                    fileNum += 1
                    filespec = FileNoExt & "(" & fileNum & ")." & ext
                End If
            End While

            File.WriteAllText(FileFolder & filespec, sValue)
        Catch ex As Exception
            Throw ex
        End Try

        Return filespec
    End Function

    Public Shared Sub WriteLog(ByVal filespec As String, ByVal sValue As String)
        File.AppendAllText(filespec, sValue)
    End Sub

    Public Shared Function GetFileContents(ByVal filespec As String) As String
        Try
            Dim Contents As String = ""
            Dim sr As StreamReader
            sr = File.OpenText(filespec)
            Contents = sr.ReadToEnd()
            sr.Close()
            Return Contents
        Catch ex As Exception
            Return ex.Message.ToString
        End Try
    End Function

    '----------------------------------------------------------------------
    '	Function: writeXML (Text)
    '   This Sub writes a cache XML file for the chart to process
    '----------------------------------------------------------------------
    Public Shared Sub WriteXMLFile(ByVal filespec As String, ByVal sValue As String, ByVal head As Boolean)
        If head Then
            Dim XMLHead As String = "<?xml version=""1.0"" encoding=""iso-8859-1""?>" & vbCrLf
            File.WriteAllText(filespec, XMLHead & sValue)
        Else
            File.WriteAllText(filespec, sValue)
        End If
    End Sub
End Class
