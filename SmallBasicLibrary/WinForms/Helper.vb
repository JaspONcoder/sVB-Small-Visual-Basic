﻿Imports System.Windows.Threading
Imports Microsoft.SmallVisualBasic.Library
Imports App = Microsoft.SmallVisualBasic.Library.Internal.SmallBasicApplication

Module Helper

    Private exeFile As String = Process.GetCurrentProcess().MainModule.FileName
    Private exeDir As String = IO.Path.GetDirectoryName(exeFile)
    Private errorFile As New Primitive(IO.Path.Combine(exeDir, "RuntimeErrors.txt"))
    Private ClearErrors As Boolean = True

    Public Sub ReportError(msg As String, ex As Exception)
        If App.IsDebugging Then
            Program.Exception = New Exception(msg, ex)
            Return
        End If

        Try
            If ClearErrors Then
                ClearErrors = False
                File.DeleteFile(errorFile)
            End If
            File.AppendContents(errorFile, New Primitive(Now & vbCrLf & msg & vbCrLf))
            App.ShowErrorWindow(msg, ex.StackTrace)
        Catch
        End Try
    End Sub

    Public Sub RunLater(DisObj As DispatcherObject, action As Action, Optional milliseconds As Integer = 20)
        If DisObj Is Nothing Then Return
        Dim dt As New DispatcherTimer(
            TimeSpan.FromMilliseconds(milliseconds),
            DispatcherPriority.Background,
            Sub()
                action()
                dt.Stop()
            End Sub, DisObj.Dispatcher)
        dt.Start()
    End Sub
End Module
