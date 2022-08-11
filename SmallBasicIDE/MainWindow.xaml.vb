﻿Imports Microsoft.Nautilus.Text
Imports Microsoft.SmallBasic.com.smallbasic
Imports Microsoft.SmallBasic.Documents
Imports Microsoft.SmallBasic.LanguageService
Imports Microsoft.SmallBasic.Shell
Imports Microsoft.SmallBasic.Utility
Imports Microsoft.Win32
Imports Microsoft.Windows.Controls
Imports System
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Globalization
Imports System.IO
Imports Nf = Microsoft.SmallBasic.Utility.NotificationButtons
Imports System.Reflection
Imports System.Threading
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Threading
Imports System.Linq
Imports System.ComponentModel.Composition
Imports sb = Microsoft.SmallBasic.WinForms
Imports Microsoft.VisualBasic

Namespace Microsoft.SmallBasic
    <Export("MainWindow")>
    Public Class MainWindow
        Private documentTrackerField As New DocumentTracker()
        Private currentCompletionItem As CompletionItemWrapper
        Private helpUpdateTimer As DispatcherTimer
        Private currentProgramProcess As Process
        Private mdiViews As ObservableCollection(Of MdiView)
        Private lastSearchedText As String = ""

        Public Shared NewCommand As New RoutedUICommand(
            ResourceHelper.GetString("NewCommand"),
            ResourceHelper.GetString("NewCommand"),
            GetType(MainWindow)
        )
        Public Shared OpenCommand As New RoutedUICommand(
            ResourceHelper.GetString("OpenCommand"),
            ResourceHelper.GetString("OpenCommand"),
            GetType(MainWindow)
        )
        Public Shared SaveCommand As New RoutedUICommand(
            ResourceHelper.GetString("SaveCommand"),
            ResourceHelper.GetString("SaveCommand"),
            GetType(MainWindow)
        )
        Public Shared SaveAsCommand As New RoutedUICommand(
            ResourceHelper.GetString("SaveAsCommand"),
            ResourceHelper.GetString("SaveAsCommand"),
            GetType(MainWindow)
        )
        Public Shared CutCommand As New RoutedUICommand(
            ResourceHelper.GetString("CutCommand"),
            ResourceHelper.GetString("CutCommand"),
            GetType(MainWindow)
        )
        Public Shared CopyCommand As New RoutedUICommand(
            ResourceHelper.GetString("CopyCommand"),
            ResourceHelper.GetString("CopyCommand"),
            GetType(MainWindow)
         )
        Public Shared PasteCommand As New RoutedUICommand(
            ResourceHelper.GetString("PasteCommand"),
            ResourceHelper.GetString("PasteCommand"),
            GetType(MainWindow)
        )
        Public Shared FindCommand As New RoutedUICommand(
            ResourceHelper.GetString("FindCommand"),
            ResourceHelper.GetString("FindCommand"),
            GetType(MainWindow)
        )
        Public Shared FindNextCommand As New RoutedUICommand(
            ResourceHelper.GetString("FindNextCommand"),
            ResourceHelper.GetString("FindNextCommand"),
            GetType(MainWindow)
        )
        Public Shared SelectNextMatchCommand As New RoutedUICommand(
            "Find Next matching pair",
            "Find Next matching pair",
            GetType(MainWindow)
        )
        Public Shared SelectPrevMatchCommand As New RoutedUICommand(
            "Find Previous matching pair",
            "Find Previous matching pair",
            GetType(MainWindow)
        )
        Public Shared UndoCommand As New RoutedUICommand(
            ResourceHelper.GetString("UndoCommand"),
            ResourceHelper.GetString("UndoCommand"),
            GetType(MainWindow)
        )
        Public Shared RedoCommand As New RoutedUICommand(
            ResourceHelper.GetString("RedoCommand"),
            ResourceHelper.GetString("RedoCommand"),
            GetType(MainWindow)
        )
        Public Shared FormatCommand As New RoutedUICommand(
            ResourceHelper.GetString("FormatProgramCommand"),
            ResourceHelper.GetString("FormatProgramCommand"),
            GetType(MainWindow)
        )
        Public Shared RunCommand As New RoutedUICommand(
            ResourceHelper.GetString("RunProgramCommand"),
            ResourceHelper.GetString("RunProgramCommand"),
            GetType(MainWindow)
        )
        Public Shared EndProgramCommand As New RoutedUICommand(
            ResourceHelper.GetString("EndProgramCommand"),
            ResourceHelper.GetString("EndProgramCommand"),
            GetType(MainWindow)
        )
        Public Shared StepOverCommand As New RoutedUICommand(
            ResourceHelper.GetString("StepOverCommand"),
            ResourceHelper.GetString("StepOverCommand"),
            GetType(MainWindow)
        )
        Public Shared BreakpointCommand As New RoutedUICommand(
            ResourceHelper.GetString("BreakpointCommand"),
            ResourceHelper.GetString("BreakpointCommand"),
            GetType(MainWindow)
         )
        Public Shared DebugCommand As New RoutedUICommand(
            ResourceHelper.GetString("DebugCommand"),
            ResourceHelper.GetString("DebugCommand"),
            GetType(MainWindow)
         )
        Public Shared WebSaveCommand As New RoutedUICommand(
            ResourceHelper.GetString("PublishProgramCommand"),
            ResourceHelper.GetString("PublishProgramCommand"),
            GetType(MainWindow)
        )
        Public Shared WebLoadCommand As New RoutedUICommand(
            ResourceHelper.GetString("ImportProgramCommand"),
            ResourceHelper.GetString("ImportProgramCommand"),
            GetType(MainWindow)
        )
        Public Shared ExportToVisualBasicCommand As New RoutedUICommand(
            ResourceHelper.GetString("ExportToVisualBasicCommand"),
            ResourceHelper.GetString("ExportToVisualBasicCommand"),
            GetType(MainWindow)
        )

        ''' <summary>
        ''' DocumentTracker
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property DocumentTracker As DocumentTracker
            Get
                Return documentTrackerField
            End Get
        End Property

        Public ReadOnly Property ActiveDocument As TextDocument
            Get
                Dim result As TextDocument = Nothing
                Dim selectedItem = Me.viewsControl.SelectedItem

                If selectedItem IsNot Nothing Then
                    result = selectedItem.Document
                End If

                Return result
            End Get
        End Property

        Public Sub New()
            Me.InitializeComponent()

            mdiViews = New ObservableCollection(Of MdiView)()
            Me.viewsControl.ItemsSource = mdiViews
            AddHandler CompilerService.CurrentCompletionItemChanged, AddressOf OnCurrentCompletionItemChanged
            'OnFileNew(Me, Nothing)
            helpUpdateTimer = New DispatcherTimer(TimeSpan.FromMilliseconds(200.0), DispatcherPriority.ApplicationIdle, AddressOf OnHelpUpdate, Dispatcher)
            'ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf OnCheckVersion))

        End Sub

        Private Sub OnFileNew(sender As Object, e As RoutedEventArgs)
            Dim textDocument As New TextDocument(Nothing)
            textDocument.ContentType = "text.smallbasic"
            DocumentTracker.TrackDocument(textDocument)
            Dim mdiView As New MdiView()
            mdiView.Document = textDocument
            mdiViews.Add(mdiView)
            textDocument.Focus()
        End Sub

        Private Sub OnFileOpen(ByVal sender As Object, ByVal e As RoutedEventArgs)
            Dim openFileDialog As New OpenFileDialog()
            openFileDialog.Filter = ResourceHelper.GetString("SmallBasicFileFilter") & "|*.sb;*.smallbasic"

            If openFileDialog.ShowDialog() = True Then
                OpenDocIfNot(openFileDialog.FileName).Focus()
            End If
        End Sub

        Private Sub CanFileSave(sender As Object, e As CanExecuteRoutedEventArgs)
            e.CanExecute = ActiveDocument IsNot Nothing
        End Sub

        Private Sub OnFileSave(ByVal sender As Object, ByVal e As RoutedEventArgs)
            SaveDocument(ActiveDocument)
        End Sub

        Private Sub OnFileSaveAs(ByVal sender As Object, ByVal e As RoutedEventArgs)
            SaveDocumentAs(ActiveDocument)
        End Sub

        Private Sub CanEditCut(ByVal sender As Object, ByVal e As CanExecuteRoutedEventArgs)
            e.CanExecute = ActiveDocument IsNot Nothing AndAlso Not ActiveDocument.EditorControl.TextView.Selection.IsEmpty
        End Sub

        Private Sub OnEditCut(sender As Object, e As RoutedEventArgs)
            Dim doc = ActiveDocument
            doc.EditorControl.EditorOperations.CutSelection(doc.UndoHistory)
        End Sub

        Private Sub CanEditCopy(ByVal sender As Object, ByVal e As CanExecuteRoutedEventArgs)
            e.CanExecute = ActiveDocument IsNot Nothing AndAlso Not ActiveDocument.EditorControl.TextView.Selection.IsEmpty
        End Sub

        Private Sub OnEditCopy(ByVal sender As Object, ByVal e As RoutedEventArgs)
            ActiveDocument.EditorControl.EditorOperations.CopySelection()
        End Sub

        Private Sub CanEditPaste(ByVal sender As Object, ByVal e As CanExecuteRoutedEventArgs)
            e.CanExecute = ActiveDocument IsNot Nothing AndAlso ActiveDocument.EditorControl.EditorOperations.CanPaste
        End Sub

        Private Sub OnEditPaste(ByVal sender As Object, ByVal e As RoutedEventArgs)
            Dim doc = ActiveDocument
            doc.EditorControl.EditorOperations.Paste(doc.UndoHistory)
        End Sub

        Private Sub CanEditUndo(ByVal sender As Object, ByVal e As CanExecuteRoutedEventArgs)
            e.CanExecute = ActiveDocument IsNot Nothing AndAlso ActiveDocument.UndoHistory.CanUndo
        End Sub

        Private Sub OnEditUndo(ByVal sender As Object, ByVal e As RoutedEventArgs)
            ActiveDocument.UndoHistory.Undo(1)
        End Sub

        Private Sub CanEditRedo(ByVal sender As Object, ByVal e As CanExecuteRoutedEventArgs)
            e.CanExecute = ActiveDocument IsNot Nothing AndAlso ActiveDocument.UndoHistory.CanRedo
        End Sub

        Private Sub OnEditRedo(ByVal sender As Object, ByVal e As RoutedEventArgs)
            ActiveDocument.UndoHistory.Redo(1)
        End Sub

        Private Sub OnCloseItem(sender As Object, e As RequestCloseEventArgs)
            Dim mdiView = TryCast(e.Item, MdiView)
            DoCloseDoc(mdiView)
        End Sub

        Sub DoCloseDoc(mdiView As MdiView)
            If mdiView IsNot Nothing AndAlso CloseDocument(mdiView.Document) Then
                mdiViews.Remove(mdiView)
                If mdiViews.Count > 0 Then
                    If viewsControl.SelectedItem Is Nothing Then
                        viewsControl.ChangeSelection(mdiViews(mdiViews.Count - 1))
                    End If
                End If
            End If

        End Sub

        Private Sub CanExportToVisualBasic(ByVal sender As Object, ByVal e As CanExecuteRoutedEventArgs)
            If ActiveDocument IsNot Nothing AndAlso ActiveDocument.Text.Trim().Length > 0 Then
                e.CanExecute = True
            Else
                e.CanExecute = False
            End If
        End Sub

        Private Sub OnExportToVisualBasic(ByVal sender As Object, ByVal e As RoutedEventArgs)
            Dim exportToVBDialog As ExportToVBDialog = New ExportToVBDialog(ActiveDocument)
            exportToVBDialog.Owner = Me
            exportToVBDialog.ShowDialog()
        End Sub

        Private Sub OnFind(sender As Object, e As RoutedEventArgs)
            If ActiveDocument Is Nothing Then
                Return
            End If

            Dim messageBox As Utility.MessageBox = New Utility.MessageBox()
            messageBox.Description = ResourceHelper.GetString("TextToSearch")
            messageBox.Title = ResourceHelper.GetString("FindCommand")
            Dim text = lastSearchedText

            If Not ActiveDocument.EditorControl.TextView.Selection.IsEmpty Then
                Dim text2 As String = ActiveDocument.EditorControl.TextView.Selection.ActiveSnapshotSpan.GetText()

                If Not text2.Contains(VisualBasic.Constants.vbCr) AndAlso Not text2.Contains(VisualBasic.Constants.vbLf) Then
                    text = text2
                End If
            End If

            Dim textBox As New TextBox() With {
                .Text = text,
                .FontSize = 20.0,
                .FontFamily = New FontFamily("Consolas"),
                .Foreground = Brushes.DimGray,
                .Margin = New Thickness(0.0, 4.0, 4.0, 4.0),
                .MinWidth = 300.0
            }
            messageBox.OptionalContent = textBox
            messageBox.NotificationButtons = Nf.Close Or Nf.OK
            messageBox.okButton.Content = ResourceHelper.GetString("FindCommand")
            messageBox.NotificationIcon = NotificationIcon.Custom
            messageBox.iconImageControl.Source = New BitmapImage(New Uri("pack://application:,,/SB;component/Resources/Search.png"))
            textBox.SelectAll()
            textBox.Focus()
            Dim notificationButtons As NotificationButtons = messageBox.Display()

            If notificationButtons = NotificationButtons.OK Then
                lastSearchedText = textBox.Text

                If Not ActiveDocument.EditorControl.HighlightNextMatch(lastSearchedText, ignoreCase:=True) Then
                    Console.Beep()
                End If
            End If
        End Sub

        Private Sub OnFindNext(sender As Object, e As RoutedEventArgs)
            If ActiveDocument IsNot Nothing Then
                Dim editor = ActiveDocument.EditorControl
                If Not editor.ContainsHighlights AndAlso lastSearchedText = "" Then
                    OnFind(sender, e)
                Else
                    Dim srch = If(lastSearchedText = "",
                         editor.TextView.Selection?.ActiveSnapshotSpan.GetText(),
                         lastSearchedText
                    )

                    If Not editor.HighlightNextMatch(srch, ignoreCase:=True) Then Console.Beep()
                End If
            End If
        End Sub

        Private Sub OnSelectNextMatch(sender As Object, e As RoutedEventArgs)
            SelectAnotherMatch(True)
        End Sub

        Private Sub OnSelectPrevMatch(sender As Object, e As RoutedEventArgs)
            SelectAnotherMatch(False)
        End Sub

        Private Sub SelectAnotherMatch(moveDown As Boolean)
            Dim doc = ActiveDocument
            If doc IsNot Nothing Then
                Dim editor = doc.EditorControl
                If Not editor.ContainsWordHighlights Then
                    doc.HighlightEnclosingBlockKeywords()
                End If
                editor.SelectAnotherHighlightedWord(moveDown)
            End If
        End Sub

        Private Sub OnFormatProgram(ByVal sender As Object, ByVal e As RoutedEventArgs)
            If ActiveDocument IsNot Nothing Then
                CompilerService.FormatDocument(ActiveDocument.TextBuffer)
            End If
        End Sub

        Private Sub CanRunProgram(ByVal sender As Object, ByVal e As CanExecuteRoutedEventArgs)
            e.CanExecute = ActiveDocument IsNot Nothing
        End Sub

        Private Sub OnProgramRun(sender As Object, e As RoutedEventArgs)
            RunProgram()
        End Sub

        Private Sub OnProgramEnd(ByVal sender As Object, ByVal e As RoutedEventArgs)
            If currentProgramProcess IsNot Nothing AndAlso Not currentProgramProcess.HasExited Then
                currentProgramProcess.Kill()
                currentProgramProcess = Nothing
                Me.programRunningOverlay.Visibility = Visibility.Hidden
            End If
        End Sub

        Private Sub OnStepOver(ByVal sender As Object, ByVal e As RoutedEventArgs)
            ActiveDocument.Errors.Clear()
            CompilerService.Compile(ActiveDocument.Text, ActiveDocument.Errors)

            If ActiveDocument.Errors.Count = 0 Then
                Dim debugger = ProgramDebugger.GetDebugger(ActiveDocument)
                debugger.StepOver()
            End If
        End Sub

        Private Sub OnToggleBreakpoint(ByVal sender As Object, ByVal e As RoutedEventArgs)
            Dim debugger = ProgramDebugger.GetDebugger(ActiveDocument)
            Dim stMarkerProvider = StatementMarkerProvider.GetStatementMarkerProvider(ActiveDocument.EditorControl.TextView)
            Dim lineFromPosition = ActiveDocument.TextBuffer.CurrentSnapshot.GetLineFromPosition(ActiveDocument.EditorControl.TextView.Caret.Position.CharacterIndex)

            If debugger.Breakpoints.Contains(lineFromPosition.LineNumber) Then
                debugger.Breakpoints.Remove(lineFromPosition.LineNumber)
                Dim array As StatementMarker() = stMarkerProvider.Markers.ToArray()

                For Each statementMarker In array
                    Dim containingLine As ITextSnapshotLine = statementMarker.Span.GetStartPoint(ActiveDocument.TextBuffer.CurrentSnapshot).GetContainingLine()

                    If containingLine.LineNumber = lineFromPosition.LineNumber Then
                        stMarkerProvider.RemoveMarker(statementMarker)
                    End If
                Next
            Else
                debugger.Breakpoints.Add(lineFromPosition.LineNumber)
                stMarkerProvider.AddStatementMarker(New StatementMarker(New TextSpan(ActiveDocument.TextBuffer.CurrentSnapshot, lineFromPosition.Start, lineFromPosition.Length, SpanTrackingMode.EdgeInclusive), Colors.Red))
            End If
        End Sub

        Private Sub CanWebSave(ByVal sender As Object, ByVal e As CanExecuteRoutedEventArgs)
            e.CanExecute = ActiveDocument IsNot Nothing
        End Sub

        Private Sub OnWebSave(ByVal sender As Object, ByVal e As RoutedEventArgs)
            PublishDocument(ActiveDocument)
        End Sub

        Private Sub OnWebLoad(ByVal sender As Object, ByVal e As RoutedEventArgs)
            ImportDocument()
        End Sub

        Protected Overrides Sub OnClosing(ByVal e As CancelEventArgs)
            For Each item In New List(Of MdiView)(mdiViews)
                If Not CloseDocument(item.Document) Then
                    e.Cancel = True
                    Return
                End If

                mdiViews.Remove(item)
            Next

            tabDesigner.IsSelected = True
            For i = 0 To DiagramHelper.Designer.Pages.Count - 1
                If Not DiagramHelper.Designer.ClosePage(False) Then
                    e.Cancel = True
                    Exit For
                End If
            Next

            MyBase.OnClosing(e)
        End Sub

        Private Function OpenCodeFile(ByVal filePath As String) As TextDocument
            tabCode.IsSelected = True
            Dim doc As New TextDocument(filePath)
            DocumentTracker.TrackDocument(doc)
            Dim mdiView As New MdiView()
            mdiView.Document = doc
            mdiViews.Add(mdiView)
            doc.Focus(True)
            Return doc
        End Function

        Private Function SaveDocument(ByVal document As TextDocument) As Boolean
            If document.IsNew OrElse (document.Form <> "" AndAlso formDesigner.FileName = "") Then
                Return SaveDocumentAs(document)
            End If

            Try
                Dim designIsDirty = document.Form <> "" AndAlso formDesigner.HasChanges
                If designIsDirty Then SaveDesignInfo(document)
                If document.IsDirty Then document.Save()
                Return True
            Catch ex As Exception
                Dim notificationButtons = Utility.MessageBox.Show(ResourceHelper.GetString("SaveFailed"), "Small Basic", String.Format(CultureInfo.CurrentCulture, ResourceHelper.GetString("SaveFailedReason"), New Object(0) {ex.Message}), Nf.No Or Nf.Yes, NotificationIcon.Information)

                If notificationButtons = Nf.Yes Then
                    Return SaveDocumentAs(document)
                End If

                Return False
            End Try
        End Function

        Private Function SaveDocumentAs(document As TextDocument) As Boolean
            Dim saveFileDialog As SaveFileDialog = New SaveFileDialog()
            saveFileDialog.Filter = ResourceHelper.GetString("SmallBasicFileFilter") & "|*.sb"
            saveFileDialog.FileName = document.Form

            If saveFileDialog.ShowDialog() Then
                Try

                    Dim FileName = saveFileDialog.FileName
                    If File.Exists(FileName) Then File.Delete(FileName)
                    document.SaveAs(FileName)

                    If document.PageKey <> "" Then
                        Dim newFormName = Path.GetFileNameWithoutExtension(saveFileDialog.FileName)
                        Dim newDir = Path.GetDirectoryName(saveFileDialog.FileName)
                        Dim newFilePath = Path.Combine(newDir, newFormName)

                        FileName = newFilePath & ".sb.gen"
                        If File.Exists(FileName) Then File.Delete(FileName)

                        FileName = newFilePath & ".xaml"
                        If File.Exists(FileName) Then File.Delete(FileName)

                        formDesigner.FileName = FileName
                        formDesigner.CodeFilePath = saveFileDialog.FileName
                        formDesigner.HasChanges = True ' Force saving
                        SaveDesignInfo(document)
                    End If

                    Return True
                Catch ex As Exception
                    Dim notificationButtons = Utility.MessageBox.Show(ResourceHelper.GetString("SaveFailed"), ResourceHelper.GetString("Title"), String.Format(CultureInfo.CurrentUICulture, ResourceHelper.GetString("SaveFailedReason"), New Object(0) {ex.Message}), Nf.No Or Nf.Yes, NotificationIcon.Information)

                    If notificationButtons = Nf.Yes Then
                        Return SaveDocumentAs(document)
                    End If

                    Return False
                End Try
            End If

            Return False
        End Function

        Private Function CloseDocument(document As TextDocument) As Boolean
            If document.IsDirty Then
                tabCode.IsSelected = True
                Select Case Utility.MessageBox.Show(ResourceHelper.GetString("SaveDocumentBeforeClosing"), ResourceHelper.GetString("Title"), document.Title & ResourceHelper.GetString("DocumentModified"), Nf.Cancel Or Nf.No Or Nf.Yes, NotificationIcon.Information)
                    Case Nf.Yes
                        Return SaveDocument(document)
                    Case Nf.No
                        Return True
                    Case Else
                        Return False
                End Select
            End If

            Return True
        End Function


        Private Sub RunProgram()
            Dim ReSelectDesigner As Boolean = False

            ' Try
            If tabDesigner.IsSelected Then
                    ' User can hit F5 on the designer.
                    ' We need to save changes and generate the code behind
                    ' The safe way to do this is to shwitch to the code tab
                    ' then come back after the program ends.
                    tabCode.IsSelected = True
                    ReSelectDesigner = True
                End If

                Dim doc = ActiveDocument
                Dim code As String
                Dim outputFileName = GetOutputFileName(doc)
                Dim offset = 0
                Dim gen As String

                If doc.PageKey <> "" Then
                    If doc.Text.Contains("'@Form Hints:") Then
                        doc.ParseFormHints()
                        gen = doc.GetCodeBehind(True)
                    Else
                        gen = doc.GenerateCodeBehind(formDesigner, False)
                    End If
                Else
                    gen = doc.GetCodeBehind(True)
                End If

                If gen <> "" Then
                    offset = CountLines(gen) + 1
                    code = gen & VisualBasic.vbLf & doc.Text
                Else
                    code = doc.Text
                End If

                code = code.Replace(VisualBasic.vbCrLf, VisualBasic.vbLf)

                doc.Errors.Clear()
                Dim errors As List(Of [Error])

                If Not RunProgram(code, errors, outputFileName) Then
                    For Each err As [Error] In errors
                        If err.Line = -1 Then
                            doc.Errors.Add(err.Description)
                        Else
                            doc.Errors.Add($"{err.Line - offset + 1},{err.Column + 1}: {err.Description}")
                        End If
                    Next
                    doc.ErrorListControl.SelectError(0)
                End If

            ' Catch ex As Exception
            'Utility.MessageBox.Show(ResourceHelper.GetString("FailedToCreateOutputFile"), ResourceHelper.GetString("Title"), String.Format(CultureInfo.CurrentUICulture, ResourceHelper.GetString("FailedToCreateOutputFileReason"), New Object(0) {ex.Message}), NotificationButtons.Close, NotificationIcon.Error)
            'Finally
            If ReSelectDesigner Then tabDesigner.IsSelected = True
            'End Try
        End Sub

        Private Function RunProgram(code As String, ByRef errors As List(Of [Error]), outputFileName As String) As Boolean
            Dim doc = ActiveDocument
            errors = Compile(code, outputFileName)
            If errors.Count = 0 Then
                Thread.Sleep(500)
                currentProgramProcess = Process.Start(outputFileName)
                currentProgramProcess.EnableRaisingEvents = True

                AddHandler currentProgramProcess.Exited,
                    Sub()
                        Dispatcher.BeginInvoke(DispatcherPriority.Normal, CType(
                             Function()
                                 Me.programRunningOverlay.Visibility = Visibility.Hidden
                                 currentProgramProcess = Nothing

                                 If doc.IsNew Then
                                     Try
                                         File.Delete(outputFileName)
                                     Catch
                                     End Try
                                 End If

                                 doc.Focus()
                                 Return True
                             End Function,
                             DispatcherOperationCallback), Nothing)
                    End Sub

                Me.processRunningMessage.Text = String.Format(ResourceHelper.GetString("ProgramRunning"), doc.Title)
                Me.programRunningOverlay.Visibility = Visibility.Visible
                Me.endProgramButton.Focus()

            ElseIf doc.Form <> "" Then
                Dim controlNamesList As New Dictionary(Of String, String)
                Dim normalErrors As New List(Of [Error])

                For Each [error] In errors
                    Dim errMsg = [error].Description
                    If Not (errMsg = "!" OrElse errMsg.StartsWith("Cannot find object", StringComparison.InvariantCultureIgnoreCase)) Then
                        normalErrors.Add([error])
                    ElseIf normalErrors.Count = 0 Then
                        Dim pos = errMsg.LastIndexOf("'", errMsg.Length - 3) + 1
                        Dim obj As String = errMsg.Substring(pos, errMsg.Length - pos - 2).ToLower()

                        If Not controlNamesList.ContainsKey(obj) Then
                            Dim controlName As String
                            If doc.ControlsInfo.ContainsKey(obj) Then
                                controlName = doc.ControlsInfo(obj)
                            Else
                                controlName = sb.PreCompiler.GetModuleFromVarName(obj)
                                If controlName = "" Then normalErrors.Add([error])
                            End If
                            controlNamesList.Add(obj, controlName)
                        End If
                    End If
                Next

                If normalErrors.Count > 0 Then
                    ' Fix normal errors first
                    errors = normalErrors
                    Return False
                End If

                Return PreCompile(code, errors, outputFileName, controlNamesList)

            Else
                Return False
            End If

            Return True
        End Function


        Private Sub PublishDocument(ByVal document As TextDocument)
            Try
                Cursor = Cursors.Wait
                Dim service As Service = New Service()
                Dim text = service.SaveProgram("", document.Text, document.BaseId)

                If Equals(text, "error") Then
                    Utility.MessageBox.Show(ResourceHelper.GetString("FailedToPublishToWeb"), ResourceHelper.GetString("Title"), ResourceHelper.GetString("PublishToWebFailedReason"), NotificationButtons.Close, NotificationIcon.Error)
                Else
                    Dim publishProgramDialog As PublishProgramDialog = New PublishProgramDialog(text)
                    publishProgramDialog.Owner = Me
                    publishProgramDialog.ShowDialog()
                End If

            Catch ex As Exception
                Utility.MessageBox.Show(ResourceHelper.GetString("FailedToPublishToWeb"), ResourceHelper.GetString("Title"), String.Format(CultureInfo.CurrentUICulture, ResourceHelper.GetString("ReasonForFailure"), New Object(0) {ex.Message}), NotificationButtons.Close, NotificationIcon.Error)
            Finally
                Cursor = Cursors.Arrow
            End Try
        End Sub

        Private Sub ImportDocument()
            Try
                Dim messageBox As New Utility.MessageBox()
                messageBox.Description = ResourceHelper.GetString("ImportFromWeb")
                messageBox.Title = ResourceHelper.GetString("Title")
                Dim stackPanel As StackPanel = New StackPanel()
                stackPanel.Orientation = Orientation.Vertical
                Dim stackPanel2 = stackPanel
                Dim textBlock As New TextBlock()
                textBlock.Text = ResourceHelper.GetString("ImportLocationOfProgramOnWeb")
                textBlock.Margin = New Thickness(0.0, 0.0, 4.0, 4.0)
                Dim element = textBlock
                Dim textBox As New TextBox() With {
                    .FontSize = 32.0,
                    .FontWeight = FontWeights.Bold,
                    .FontFamily = New FontFamily("Courier New"),
                    .Foreground = Brushes.DimGray,
                    .Margin = New Thickness(0.0, 4.0, 4.0, 4.0),
                    .MinWidth = 300.0
                }
                stackPanel2.Children.Add(element)
                stackPanel2.Children.Add(textBox)
                messageBox.OptionalContent = stackPanel2
                messageBox.NotificationButtons = NotificationButtons.Cancel Or NotificationButtons.OK
                messageBox.NotificationIcon = NotificationIcon.Information
                textBox.Focus()

                If messageBox.Display() = NotificationButtons.OK Then
                    Dim service As Service = New Service()
                    Dim baseId As String = textBox.Text.Trim()
                    Dim code = service.LoadProgram(baseId)

                    If Equals(code, "error") Then
                        Utility.MessageBox.Show(ResourceHelper.GetString("FailedToImportFromWeb"), ResourceHelper.GetString("Title"), ResourceHelper.GetString("ImportFromWebFailedReason"), NotificationButtons.Close, NotificationIcon.Error)
                    Else
                        code = code.Replace(VisualBasic.Constants.vbLf, VisualBasic.Constants.vbCrLf)
                        Dim newDocument As New TextDocument(Nothing)
                        newDocument.ContentType = "text.smallbasic"
                        newDocument.BaseId = baseId
                        newDocument.TextBuffer.Insert(0, code)
                        Dim service2 As Service = New Service()
                        AddHandler service2.GetProgramDetailsCompleted, Sub(ByVal o, ByVal e)
                                                                            Dim result = e.Result
                                                                            result.Category = ResourceHelper.GetString("Category" & result.Category)
                                                                            newDocument.ProgramDetails = result
                                                                        End Sub

                        service2.GetProgramDetailsAsync(baseId)
                        DocumentTracker.TrackDocument(newDocument)
                        Dim mdiView As New MdiView()
                        mdiView.Document = newDocument
                        mdiViews.Add(mdiView)
                        newDocument.Focus(True)
                    End If
                End If

            Catch ex As Exception
                Utility.MessageBox.Show(ResourceHelper.GetString("FailedToImportFromWeb"), ResourceHelper.GetString("Title"), String.Format(CultureInfo.CurrentUICulture, ResourceHelper.GetString("ReasonForFailure"), New Object(0) {ex.Message}), NotificationButtons.Close, NotificationIcon.Error)
            Finally
                Cursor = Cursors.Arrow
            End Try
        End Sub

        Private Function GetOutputFileName(document As TextDocument) As String
            If document.IsNew Then
                Dim tempFileName = Path.GetTempFileName()
                File.Move(tempFileName, tempFileName & ".exe")
                Return tempFileName & ".exe"
            End If

            Dim fileName = Path.GetFileNameWithoutExtension(document.FilePath)
            Dim docDirectory = Path.GetDirectoryName(document.FilePath)
            Dim binDirectory = docDirectory & "\bin"
            If Not Directory.Exists(binDirectory) Then Directory.CreateDirectory(binDirectory)
            Dim newFile = Path.Combine(binDirectory, fileName)
            Dim xamlFile = Path.Combine(docDirectory, fileName + ".xaml")

            If File.Exists(xamlFile) Then
                File.Copy(xamlFile, newFile & ".xaml", True)
            End If

            For Each f In Directory.EnumerateFiles(docDirectory)
                Select Case Path.GetExtension(f).ToLower().TrimStart(".")
                    Case "bmp", "jpg", "jpeg", "png", "gif", "txt"
                        Dim f2 = Path.Combine(binDirectory, Path.GetFileName(f))
                        Try
                            File.Copy(f, f2, True)
                        Catch
                        End Try
                End Select
            Next
            Return newFile & ".exe"
        End Function

        Private Sub OnCurrentCompletionItemChanged(ByVal sender As Object, ByVal e As CurrentCompletionItemChangedEventArgs)
            currentCompletionItem = e.CurrentCompletionItem

            If currentCompletionItem IsNot Nothing Then
                helpUpdateTimer.Start()
            End If
        End Sub

        Private Sub OnHelpUpdate(ByVal sender As Object, ByVal e As EventArgs)
            helpUpdateTimer.Stop()

            If currentCompletionItem IsNot Nothing Then
                Me.helpPanel.CompletionItemWrapper = currentCompletionItem
            End If
        End Sub

        Private Sub OnCheckVersion(state As Object)
            Thread.Sleep(20000)

            Try
                Dim service As Service = New Service()
                Dim currentVersion As String = service.GetCurrentVersion()
                Dim version As Version = New Version(currentVersion)
                Dim version2 As Version = Assembly.GetExecutingAssembly().GetName().Version

                'If version.CompareTo(version2) > 0 Then
                '    Dispatcher.BeginInvoke(CType(Sub() Me.updateAvailable.Visibility = Visibility.Visible, Action))
                'End If

            Catch
            End Try
        End Sub

        Private Sub OnClickNewVersionAvailable(ByVal sender As Object, ByVal e As RoutedEventArgs)
            Process.Start("http://smallbasic.com/download.aspx")
        End Sub

        Function OpenDocIfNot(FilePath As String) As TextDocument
            Dim docPath = Path.GetFullPath(FilePath)
            For Each view As MdiView In Me.viewsControl.Items
                If view.Document.FilePath = docPath Then
                    viewsControl.ChangeSelection(view)
                    Return view.Document
                End If
            Next

            Return OpenCodeFile(FilePath)
        End Function

        Function GetDocIfOpened() As TextDocument
            For Each view As MdiView In Me.viewsControl.Items
                If view.Document.PageKey = formDesigner.PageKey Then
                    Return view.Document
                End If
            Next
            Return Nothing
        End Function

        Private Sub tabCode_Selected(sender As Object, e As RoutedEventArgs)

            If DiagramHelper.Designer.CurrentPage IsNot Nothing Then
                SaveDesignInfo()
            End If

            ' Note this prop isn't changed yet
            tabDesigner.IsSelected = False
            UpdateTitle()
        End Sub

        Private Sub tabDesigner_Selected(sender As Object, e As RoutedEventArgs)
            UpdateTitle()
        End Sub

        Dim _projectPath As String

        Private Function GetProjectPath() As String
            If _projectPath <> "" Then Return _projectPath

            Dim appDir = Path.GetDirectoryName(Environment.GetCommandLineArgs()(0))
            Dim tmpPath = Path.Combine(appDir, "UnSaved")
            If Not IO.Directory.Exists(tmpPath) Then IO.Directory.CreateDirectory(tmpPath)

            Dim projectName = DateTime.Now.ToString("yy-MM-dd-HH-mm-ss")
            _projectPath = Path.Combine(tmpPath, projectName)

            Return _projectPath
        End Function

        Private Sub SaveDesignInfo(Optional doc As TextDocument = Nothing, Optional openDoc As Boolean = True)
            Dim formName As String
            Dim xamlPath As String
            Dim formPath As String

            If CStr(txtControlName.Tag) <> "" Then
                txtControlName_LostFocus(Nothing, Nothing)
            ElseIf CStr(txtControlText.Tag) <> "" Then
                txtControlText_LostFocus(Nothing, Nothing)
            End If

            OpeningDoc = True

            If openDoc AndAlso formDesigner.CodeFilePath <> "" AndAlso Not formDesigner.HasChanges Then
                doc = OpenDocIfNot(formDesigner.CodeFilePath)

                ' User may change the form name
                doc.Form = formDesigner.Name

                If doc.PageKey = "" Then doc.PageKey = formDesigner.PageKey
                OpeningDoc = False
                Return
            End If

            If formDesigner.FileName = "" Then
                If formDesigner.CodeFilePath = "" Then
                    Dim projectPath = GetProjectPath()
                    xamlPath = ""
                    formName = formDesigner.Name
                    Do
                        xamlPath = Path.Combine(projectPath, formName)
                        If Not Directory.Exists(xamlPath) Then
                            Directory.CreateDirectory(xamlPath)
                            Exit Do
                        End If
                        formName = DiagramHelper.Designer.GetTempFormName().Replace("KEY", "Form")
                    Loop
                    formPath = Path.Combine(xamlPath, formName)

                Else
                    If doc Is Nothing Then doc = GetDoc(formDesigner.CodeFilePath, openDoc)
                    formName = formDesigner.Name
                    doc.Form = formName

                    xamlPath = Path.GetDirectoryName(formDesigner.CodeFilePath)
                    formPath = formDesigner.CodeFilePath.Substring(0, formDesigner.CodeFilePath.Length - 3)
                End If

                formDesigner.Name = formName
                formDesigner.DoSave(formPath & ".xaml")
                IO.File.Create(formPath & ".sb").Close()

            Else
                xamlPath = Path.GetDirectoryName(formDesigner.FileName)
                formPath = formDesigner.FileName.Substring(0, formDesigner.FileName.Length - 5)

                If doc Is Nothing Then doc = GetDoc(formPath & ".sb", openDoc)
                formName = formDesigner.Name
                doc.Form = formName

                If formDesigner.HasChanges Then formDesigner.DoSave()

            End If

            If doc Is Nothing Then doc = GetDoc(formPath & ".sb", openDoc)
            doc.Form = formName
            doc.PageKey = formDesigner.PageKey
            formDesigner.CodeFilePath = doc.FilePath

            IO.File.WriteAllText(formPath & ".sb.gen", doc.GenerateCodeBehind(formDesigner, True))
            OpeningDoc = False
        End Sub

        Function GetDoc(codeFilePath As String, openDoc As Boolean) As TextDocument
            If openDoc Then
                Return OpenDocIfNot(codeFilePath)
            Else
                Dim doc = GetDocIfOpened()
                If doc IsNot Nothing Then Return doc
                If Not File.Exists(codeFilePath) Then File.Create(codeFilePath).Close()
                Return New TextDocument(codeFilePath)
            End If


        End Function

        Private Sub MainWindow_Closed(sender As Object, e As EventArgs) Handles Me.Closed
            Dim appDir = Path.GetDirectoryName(Environment.GetCommandLineArgs()(0))
            Dim UnSaved = Path.Combine(appDir, "UnSaved")
            For Each directory In IO.Directory.GetDirectories(UnSaved)
                Try
                    Dim d = Date.ParseExact(
                            IO.Path.GetFileNameWithoutExtension(directory),
                            "yy-MM-dd-HH-mm-ss",
                            New Globalization.CultureInfo("Ar-eg"))
                    If (Date.Now - d).TotalDays > 10 Then
                        Global.My.Computer.FileSystem.DeleteDirectory(directory, FileIO.DeleteDirectoryOption.DeleteAllContents)
                    End If
                Catch
                End Try
            Next
        End Sub

        Private Function PreCompile(code As String, ByRef errors As List(Of [Error]), outputFileName As String, controlNamesList As Dictionary(Of String, String)) As Boolean
            Dim ReRun = False
            Dim lines = New List(Of String)(code.Split(New String(0) {VisualBasic.vbLf}, StringSplitOptions.None))
            Dim doc = ActiveDocument

            Dim lineNum As Integer
            For i = errors.Count - 1 To 0 Step -1
                Dim err = errors(i)
                If lineNum = err.Line AndAlso ReRun Then Exit For

                Dim errMsg = err.Description
                lineNum = err.Line
                Dim charNum = err.Column
                Dim line = lines(lineNum)

                If errMsg = "!" Then
                    lines(lineNum) = line.Substring(0, charNum) & "Data." & line.Substring(charNum + 1)
                    errors.RemoveAt(i)
                    ReRun = True
                    Continue For
                End If

                Dim pos = errMsg.LastIndexOf("'", errMsg.Length - 3) + 1
                Dim obj As String = errMsg.Substring(pos, errMsg.Length - pos - 2).ToLower()

                Dim controlName = controlNamesList(obj)
                ' (lineNum) to be passed by value
                Dim tokens = LineScanner.GetTokens(line, (lineNum), lines)

                Dim objId As Integer
                For objId = 0 To tokens.Count - 1
                    If tokens(objId).Column = charNum Then Exit For
                Next

                If objId + 2 >= tokens.Count Then
                    errors(i) = New [Error](lineNum, 0, charNum, "Method name is expected.")
                    Continue For
                End If

                If tokens(objId + 1).Token = Token.Dot Then
                    Dim prevText = If(charNum = 0, "", line.Substring(0, charNum))
                    Dim methodPos = charNum + obj.Length + 1
                    Dim nextText = line.Substring(methodPos)

                    If objId + 3 < tokens.Count AndAlso tokens(objId + 2).Token = Token.Identifier AndAlso tokens(objId + 3).Token = Token.LeftParens Then
                        ' Method Call
                        Dim method = tokens(objId + 2).Text
                        Dim restText = line.Substring(tokens(objId + 3).Column + 1)
                        Dim argsExprList = Parser.ParseArgumentList(restText, lineNum, lines, Token.LeftParens)
                        If argsExprList Is Nothing Then
                            errors(i) = New [Error](err.Line, 0, methodPos, "Wrong brackets pairs")
                            Continue For
                        End If

                        Dim methodInfo = sb.PreCompiler.GetMethodInfo(controlName, method)
                        Dim ModuleName = methodInfo.Module
                        If ModuleName = "" Then
                            errors(i) = New [Error](err.Line, 0, methodPos, $"`{method}` doesn't exist.")
                            Continue For
                        End If

                        Dim argsCount = argsExprList.Count
                        Dim paramsCount = methodInfo.ParamsCount
                        If paramsCount = 0 OrElse paramsCount <= argsCount OrElse
                                  (ModuleName <> "Form" AndAlso argsCount > paramsCount - 2) Then
                            errors(i) = New [Error](err.Line, 0, methodPos, "Wrong number of parameters.")
                            Continue For
                        End If

                        Select Case paramsCount
                            Case 1
                                lines(lineNum) = prevText &
                                        $"{ModuleName}.{method}({obj}" &
                                        restText
                            Case argsCount + 1
                                lines(lineNum) = prevText &
                                            $"{ModuleName}.{method}({obj}, " &
                                            restText
                            Case 2
                                Dim secondParam = If(methodInfo.secondIsControl, obj & ", ", "")
                                lines(lineNum) = prevText &
                                            $"{ModuleName}.{method}({doc.Form}, {secondParam}" &
                                            restText
                            Case Else
                                Dim secondParam = If(methodInfo.secondIsControl, obj & ", ", "")
                                lines(lineNum) = prevText &
                                            $"{ModuleName}.{method}({doc.Form}, {secondParam} " &
                                            restText
                        End Select

                        errors.RemoveAt(i)
                        ReRun = True

                    ElseIf objId = 0 AndAlso err.subLine = 0 Then 'Property Set 

                        If tokens(objId + 2).Token <> Token.Identifier Then
                            errors(i) = New [Error](err.Line, 0, methodPos, $"Expected a property name.")
                            Continue For
                        End If

                        If objId + 3 >= tokens.Count OrElse tokens(objId + 3).Token <> Token.Equals Then
                            errors(i) = New [Error](err.Line, 0, methodPos, $"Expected `=` and a value to set the property")
                            Continue For
                        End If

                        Dim propName = tokens(objId + 2).Text

                        Dim propInfo = sb.PreCompiler.GetMethodInfo(controlName, propName)
                        If propInfo.Module = "" Then
                            Dim method = $"Set{propName}"
                            Dim methodInfo = sb.PreCompiler.GetMethodInfo(controlName, method)
                            Dim ModuleName = methodInfo.Module
                            If ModuleName = "" Then
                                errors(i) = New [Error](err.Line, 0, methodPos, $"`{propName}` doesn't exist.")
                                Continue For
                            End If

                            pos = tokens(objId + 3).Column

                            Select Case methodInfo.ParamsCount
                                Case 2
                                    lines(lineNum) = prevText &
                                            $"{ModuleName}.{method}({obj}, {line.Substring(pos + 1).Trim}"
                                Case 3
                                    lines(lineNum) = prevText &
                                             $"{ModuleName}.{method}({doc.Form}, {obj}, {line.Substring(pos + 1).Trim}"
                                Case Else
                                    errors(i) = New [Error](err.Line, 0, methodPos, $"`{method}` definition is not supported.")
                                    Continue For
                            End Select

                            lines(lineNum + tokens.Last.subLine) += ")"

                            errors.RemoveAt(i)
                            ReRun = True

                        Else ' Event
                            Dim ModuleName = propInfo.Module
                            lines.Insert(lineNum, $"Control.HandleEvents({doc.Form}, {obj})")
                            lines(lineNum + 1) = prevText & $"{ModuleName}.{nextText}"
                            errors.RemoveAt(i)
                            ReRun = True
                        End If

                    Else 'Property Get                     
                        If tokens.Count > objId + 2 AndAlso tokens(objId + 2).Token <> Token.Identifier Then
                            errors(i) = New [Error](err.Line, 0, methodPos, "property name is expected.")
                            Continue For
                        End If

                        Dim propName = tokens(objId + 2).Text
                        Dim method = $"Get{propName}"
                        Dim methodInfo = sb.PreCompiler.GetMethodInfo(controlName, method)
                        Dim ModuleName = methodInfo.Module
                        If ModuleName = "" Then
                            errors(i) = New [Error](err.Line, 0, methodPos, $"`{propName}` doesn't exist.")
                            Continue For
                        End If

                        Select Case methodInfo.ParamsCount
                            Case 1
                                lines(lineNum) =
                                            prevText &
                                            $"{ModuleName}.{method}({obj})" &
                                            nextText.Substring(propName.Length)
                            Case 2
                                lines(lineNum) =
                                            prevText &
                                            $"{ModuleName}.{method}({doc.Form}, {obj})" &
                                            nextText.Substring(propName.Length)

                            Case Else
                                errors(i) = New [Error](err.Line, 0, methodPos, $"`{method}` definition is not supported.")
                                Continue For

                        End Select

                        errors.RemoveAt(i)
                        ReRun = True
                    End If

                End If

            Next

            If ReRun Then Return RunProgram(
                        String.Join(VisualBasic.vbLf, lines),
                        errors,
                        outputFileName)

            Return errors.Count = 0

        End Function



        Private Function CountLines(str As String) As Integer
            If str = "" Then Return 0

            Dim pos As Integer = -1
            Dim count As Integer = 0
            Do
                pos = str.IndexOf(VisualBasic.vbLf, pos + 1)
                If pos = -1 Then Exit Do
                count += 1
            Loop
            Return count
        End Function


        Public Shared Function Compile(programText As String, outputFilePath As String) As List(Of [Error])
            Dim errors As List(Of [Error]) = Nothing
            Try
                Dim compiler1 As New Compiler
                Dim fileNameWithoutExtension As String = Path.GetFileNameWithoutExtension(outputFilePath)
                Dim directoryName As String = Path.GetDirectoryName(outputFilePath)
                errors = compiler1.Build(New StringReader(programText), fileNameWithoutExtension, directoryName)

            Catch ex As Exception
                If errors Is Nothing Then errors = New List(Of [Error])
                errors.Add(New [Error](-1, 0, 0, ex.Message))
            End Try

            Return errors
        End Function

        Private Sub formDesigner_DiagramDoubleClick(control As UIElement)
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait

            tabCode.IsSelected = True

            If formDesigner.CodeFilePath <> "" Then
                Dim doc = OpenDocIfNot(formDesigner.CodeFilePath)
                Dim controlName = formDesigner.GetControlNameOrDefault(control)
                Dim key = controlName.ToLower()
                Dim type = If(doc.ControlsInfo.ContainsKey(key), doc.ControlsInfo(key), "")

                If doc.AddEventHandler(controlName, sb.PreCompiler.GetDefaultEvent(type)) Then
                    doc.PageKey = formDesigner.PageKey
                    ' The code behind is saved before the new Handler is added.
                    ' We must make the designer dirty, to force saving this chamge in 
                    ' a nex call to SaveDesignInfo()
                    formDesigner.HasChanges = True
                End If
            End If

            Mouse.OverrideCursor = Nothing

        End Sub

        Dim FirstTime As Boolean = True
        Dim SelectCodeTab As Boolean

        Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
            If Not FirstTime Then Return
            FirstTime = False

            DiagramHelper.Designer.PagesGrid = DesignerGrid
            Dim HeaderPanelGrid As Grid = CType(VisualTreeHelper.GetChild(sVBTabs, 0), Grid)
            CType(Me.Content, Grid).Children.Remove(stkInfo)
            HeaderPanelGrid.Children.Add(stkInfo)
            UpdateTitle()

            formDesigner.SavePage =
                    Function()
                        Dim newFormName = Path.GetFileNameWithoutExtension(formDesigner.FileName)
                        Dim newDir = Path.GetDirectoryName(formDesigner.FileName)
                        Dim newFilePath = Path.Combine(newDir, newFormName)
                        Dim FileName = newFilePath & ".sb"

                        Dim oldCodeFile = formDesigner.CodeFilePath
                        SaveDesignInfo(Nothing, False)
                        Dim doc = GetDocIfOpened()

                        If oldCodeFile.ToLower() = FileName.ToLower() Then
                            If doc IsNot Nothing Then doc.Save()
                        Else
                            If File.Exists(FileName) Then File.Delete(FileName)
                            If doc IsNot Nothing Then
                                doc.SaveAs(FileName)
                            Else
                                File.Move(oldCodeFile, FileName)
                            End If
                            formDesigner.CodeFilePath = FileName

                            FileName = newFilePath & ".sb.gen"
                            If File.Exists(FileName) Then File.Delete(FileName)
                            File.Move(oldCodeFile & ".gen", FileName)
                        End If

                        Return True
                    End Function

            AddHandler DiagramHelper.Designer.PageShown, AddressOf formDesigner_CurrentPageChanged

            ' Set any defaults you want
            'DiagramHelper.Designer.SetDefaultPropertiesSub =
            '    Sub()
            '        DiagramHelper.Designer.SetDefaultProperties()
            '        Set any defaults you want here
            '    End Sub


            Me.Dispatcher.Invoke(DispatcherPriority.Background,
                      Sub()
                          For Each fileName In FilesToOpen
                              fileName = fileName.ToLower()
                              If fileName.EndsWith(".sb") Then
                                  OpenDocIfNot(fileName)
                                  SelectCodeTab = True
                              ElseIf fileName.EndsWith(".xaml") Then
                                  DiagramHelper.Designer.SwitchTo(fileName)
                                  SelectCodeTab = False
                              End If
                          Next
                      End Sub)

        End Sub

        Private Sub formDesigner_CurrentPageChanged(index As Integer)
            formDesigner = DiagramHelper.Designer.CurrentPage
            ZoomBox.Designer = formDesigner
            ProjExplorer.Designer = formDesigner
            ToolBox.Designer = formDesigner

            ' Remove the handler if exists not to be called twice
            RemoveHandler formDesigner.DiagramDoubleClick, AddressOf formDesigner_DiagramDoubleClick
            AddHandler formDesigner.DiagramDoubleClick, AddressOf formDesigner_DiagramDoubleClick

            RemoveHandler formDesigner.SelectionChanged, AddressOf formDesigner_SelectionChanged
            AddHandler formDesigner.SelectionChanged, AddressOf formDesigner_SelectionChanged
            UpdateTitle()

            If index > -1 AndAlso ProjExplorer.FilesList IsNot Nothing Then
                ProjExplorer.FreezListFiles = True
                ProjExplorer.FilesList.SelectedIndex = index
                ProjExplorer.FreezListFiles = False
            End If

            UpdateTextBoxes()

        End Sub

        Dim FocusTxtName As New DiagramHelper.RunAfter(10, Sub() txtControlName.Focus())
        Dim ExitSelectionChanged As Boolean

        Private Sub formDesigner_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
            If ExitSelectionChanged Then Return
            Dim i = formDesigner.SelectedIndex
            Dim controlIndex As Integer

            If CStr(txtControlName.Tag) <> "" Then
                controlIndex = CInt(txtControlName.Tag)
                If txtControlName.Text <> formDesigner.GetControlName(controlIndex) Then
                    If Not CommitName() Then
                        ' Re-select the control. this event can fire b4 lostfocus event of the textbox
                        ExitSelectionChanged = True
                        If controlIndex = -1 Then
                            formDesigner.SelectedItem = Nothing
                        Else
                            ' Note: setting selectedIndex doesn't work!!
                            formDesigner.SelectedItem = formDesigner.Items(controlIndex)
                        End If
                        ExitSelectionChanged = False

                        ' gobacl to the textbox
                        FocusTxtName.Start()
                        Return
                    End If
                End If

            ElseIf CStr(txtControlText.Tag) <> "" Then
                controlIndex = CInt(txtControlText.Tag)
                formDesigner.SetControlText(controlIndex, txtControlText.Text)
            End If

            UpdateTextBoxes()

        End Sub

        Sub UpdateTextBoxes()
            txtControlName.Tag = ""
            txtControlText.Tag = ""

            Dim controlIndex = formDesigner.SelectedIndex
            If formDesigner.SelectedIndex = -1 Then
                txtControlName.Text = formDesigner.Name
                txtControlText.Text = formDesigner.Text
            Else
                txtControlName.Text = formDesigner.GetControlName()
                txtControlText.Text = formDesigner.GetControlText()
            End If
        End Sub

        Private Sub UpdateTitle()
            If tabDesigner.IsSelected Then
                grdNameText.Visibility = Visibility.Visible
                txtTitle.Text = "Form Designer - "
                txtForm.Text = $"{formDesigner.Name}{If(formDesigner.FileName = "", " *", ".xaml")}"
            Else
                grdNameText.Visibility = Visibility.Collapsed
                txtTitle.Text = "Code Editor - "
                txtForm.Text = $"{formDesigner.Name}.sb"
            End If
        End Sub

        Dim OpeningDoc As Boolean
        Friend Shared FilesToOpen As New List(Of String)

        Private Sub viewsControl_ActiveDocumentChanged()
            If OpeningDoc Then Return

            Dim currentView = viewsControl.SelectedItem
            If currentView Is Nothing Then Return

            Dim doc = currentView.Document

            If doc.PageKey = "" Then
                If doc.FilePath <> "" Then
                    Dim pagePath = doc.FilePath.Substring(0, doc.FilePath.Length - 3) & ".xaml"
                    If File.Exists(pagePath) Then
                        doc.PageKey = DiagramHelper.Designer.SwitchTo(pagePath)
                        formDesigner.CodeFilePath = doc.FilePath
                    Else
                        ' Do nothing to allow opening old sb files without a form
                        ' If you want to attach a form, comment the next line,
                        ' ' and  umcomment the 2 lineslines after.
                        doc.PageKey = ""
                    End If
                End If
            Else
                DiagramHelper.Designer.SwitchTo(doc.PageKey)
                formDesigner.CodeFilePath = doc.FilePath
            End If

        End Sub

        Private Sub ProjExplorer_ItemDoubleClick(sender As Object, e As MouseButtonEventArgs)
            tabCode.IsSelected = True
        End Sub

        Private Sub CloseButton_Click(sender As Object, e As RoutedEventArgs)
            If tabCode.IsSelected Then
                If ActiveDocument IsNot Nothing Then DoCloseDoc(ActiveDocument.MdiView)
            Else
                DiagramHelper.Designer.ClosePage()
            End If

        End Sub

        Function CommitName() As Boolean
            If CStr(txtControlName.Tag) = "" Then Return True

            Dim controlIndex = CInt(txtControlName.Tag)
            If Not formDesigner.SetControlName(controlIndex, txtControlName.Text) Then
                VisualBasic.Beep()
                Return False
            End If

            'Dim doc = GetDocIfOpened()
            'If doc IsNot Nothing Then doc.Form = formDesigner.Name

            Return True
        End Function

        Private Sub txtControlName_PreviewKeyDown(sender As Object, e As KeyEventArgs)
            Select Case e.Key
                Case Key.Enter
                    CommitName()
                    e.Handled = True

                Case Key.Escape
                    UpdateTextBoxes()
                    e.Handled = True

                Case Key.Space
                    VisualBasic.Beep()
                    e.Handled = True
            End Select
        End Sub

        Private Sub txtControlName_LostFocus(sender As Object, e As RoutedEventArgs)
            If FocusTxtName.Started Then Return

            If CommitName() Then
                txtControlName.Tag = ""
            Else
                If e IsNot Nothing Then e.Handled = True
                FocusTxtName.Start()
            End If
        End Sub

        Private Sub txtControlName_PreviewTextInput(sender As Object, e As TextCompositionEventArgs)
            Select Case e.Text.ToLower
                Case "a" To "z", "_", "0" To "9"
                    ' allowed
                Case Else
                    VisualBasic.Beep()
                    e.Handled = True
            End Select
        End Sub

        Private Sub txtControlText_PreviewKeyDown(sender As Object, e As KeyEventArgs)
            Select Case e.Key
                Case Key.Enter
                    If CStr(txtControlText.Tag) <> "" Then
                        Dim controlIndex = CInt(txtControlText.Tag)
                        formDesigner.SetControlText(controlIndex, txtControlText.Text)
                    End If
                    e.Handled = True

                Case Key.Escape
                    UpdateTextBoxes()
                    e.Handled = True
            End Select

        End Sub

        Private Sub txtControlText_LostFocus(sender As Object, e As RoutedEventArgs)
            If CStr(txtControlText.Tag) = "" Then Return

            Dim controlIndex = CInt(txtControlText.Tag)
            formDesigner.SetControlText(controlIndex, txtControlText.Text)
            txtControlText.Tag = ""
        End Sub

        Private Sub txtControl_GotFocus(sender As Object, e As RoutedEventArgs)
            Dim txt As TextBox = sender
            txt.Tag = formDesigner.SelectedIndex
        End Sub

        Private Sub Window_ContentRendered(sender As Object, e As EventArgs)
            ' Load the code edotor
            tabCode.IsSelected = True
            If Not SelectCodeTab Then tabDesigner.IsSelected = True
        End Sub
    End Class


End Namespace
