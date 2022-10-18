﻿Imports System.ComponentModel.Composition
Imports Microsoft.Nautilus.Core.Undo
Imports Microsoft.Nautilus.Text.Editor
Imports Microsoft.Nautilus.Text.Operations

Namespace Microsoft.SmallVisualBasic.LanguageService
    Public Class CompletionKeyboardFilter
        Inherits KeyboardFilter

        <Import>
        Public Property EditorOperationsProvider As IEditorOperationsProvider
        <Import>
        Public Property UndoHistoryRegistry As IUndoHistoryRegistry

        Public Overrides Sub KeyDown(textView As IAvalonTextView, args As KeyEventArgs)
            Dim completionSurface = textView.Properties.GetProperty(Of CompletionSurface)()

            If completionSurface IsNot Nothing AndAlso completionSurface.IsAdornmentVisible Then
                Dim completionListBox = completionSurface.CompletionListBox
                Dim __ = completionListBox.SelectedIndex

                Select Case args.Key
                    Case Key.Escape
                        completionSurface.Adornment.Dismiss(force:=True)
                        textView.VisualElement.Focus()
                        args.Handled = True

                    Case Key.Up
                        completionListBox.MoveUp()
                        args.Handled = True

                    Case Key.Down
                        completionListBox.MoveDown()
                        args.Handled = True

                    Case Key.Return
                        args.Handled = CommitConditionally(textView, completionSurface, If(Keyboard.Modifiers And ModifierKeys.Control > 0, "+nl", ""))

                    Case Key.Space, Key.Tab
                        args.Handled = CommitConditionally(textView, completionSurface, " ")

                    Case Key.OemPeriod
                        If Keyboard.Modifiers = 0 Then CommitConditionally(textView, completionSurface)

                End Select

            Else
                Dim provider = textView.Properties.GetProperty(Of CompletionProvider)()
                If provider Is Nothing Then Return

                If args.Key = Key.Space AndAlso args.KeyboardDevice.Modifiers = ModifierKeys.Control Then
                    provider.ShowHelp("*")

                    If provider.IsSpectialListVisible Then
                        provider.IsSpectialListVisible = False
                    Else
                        provider.ShowCompletionAdornment(
                            textView.TextSnapshot,
                            textView.Caret.Position.TextInsertionIndex,
                            True
                        )
                    End If
                    args.Handled = True

                    ElseIf args.Key = Key.F1 Then
                        provider.ShowHelp(True)
                End If
            End If
        End Sub

        Public Overrides Sub KeyUp(textView As IAvalonTextView, args As KeyEventArgs)
            Dim adornmentSurface As CompletionSurface = Nothing

            If (args.Key = Key.LeftCtrl OrElse args.Key = Key.RightCtrl) AndAlso
                        textView.Properties.TryGetProperty(Of CompletionSurface)(adornmentSurface) AndAlso
                        adornmentSurface.IsAdornmentVisible Then
                adornmentSurface.UnfadeCompletionList()
            End If

            MyBase.KeyUp(textView, args)
        End Sub

        Public Overrides Sub TextInput(textView As IAvalonTextView, args As TextCompositionEventArgs)
            Dim completionSurface As CompletionSurface = Nothing

            If textView.Properties.TryGetProperty(GetType(CompletionSurface), completionSurface) Then
                If completionSurface.IsAdornmentVisible Then
                    Select Case args.Text
                        Case "+", "-", "*", "/"
                            args.Handled = CommitConditionally(textView, completionSurface, " " & args.Text & " ")

                        Case "="
                            Dim pos = textView.Caret.Position.CharacterIndex
                            Select Case textView.TextSnapshot.GetText(pos - 1, 1)
                                Case ">", "<"

                                Case Else
                                    args.Handled = CommitConditionally(textView, completionSurface, " = ", True)
                            End Select

                        Case "<", ">"
                            args.Handled = CommitConditionally(textView, completionSurface, " " & args.Text & " ", True)

                        Case "!", ")", "[", "]", "{", "}"
                            CommitConditionally(textView, completionSurface)

                        Case ",", "("
                            args.Handled = CommitConditionally(textView, completionSurface, args.Text & " ")

                    End Select
                End If
            End If
            MyBase.TextInputUpdate(textView, args)
        End Sub

        Private Function CommitConditionally(
                        textView As IAvalonTextView,
                        completionSurface As CompletionSurface,
                        Optional extraText As String = "",
                        Optional showCompletionAdornmentAgain As Boolean = False
                     ) As Boolean

            If Not completionSurface.IsFaded AndAlso completionSurface.CompletionListBox.SelectedItem IsNot Nothing Then
                Dim editorOperations = EditorOperationsProvider.GetEditorOperations(textView)
                Dim compList = completionSurface.CompletionListBox
                Dim itemWrapper = CType(compList.SelectedItem, CompletionItemWrapper)
                Dim item = itemWrapper.CompletionItem
                Dim repWith = itemWrapper.ReplacementText
                Dim provider = textView.Properties.GetProperty(Of CompletionProvider)()
                Dim replaceSpan = provider.GetReplacementSpane()

                Dim key = item.HistoryKey
                If key <> "" Then CompletionProvider.CompHistory(key) = item.DisplayName

                If repWith.EndsWith("(") OrElse repWith.EndsWith(")") Then
                    If extraText.EndsWith("( ") Then extraText = ""
                    Dim pos = textView.Caret.Position.TextInsertionIndex
                    Dim line = textView.TextSnapshot.GetLineFromPosition(pos)
                    Dim txt = line.GetText().Substring(pos - line.Start)
                    If LineScanner.GetFirstToken(txt, 0).Type = TokenType.LeftParens Then
                        repWith = repWith.TrimEnd("("c, ")"c)
                    End If
                End If

                Dim leadingSpace = ""
                If replaceSpan.Length = 0 And replaceSpan.Start > 0 Then
                    Select Case textView.TextSnapshot(replaceSpan.Start - 1)
                        Case "=", "<", ">", "+", "-", "*", "/"
                            leadingSpace = " "
                    End Select
                End If

                If extraText = "+nl" Then
                    leadingSpace = vbCrLf & leadingSpace
                    extraText = ""
                End If

                editorOperations.ReplaceText(
                    replaceSpan,
                    leadingSpace & repWith & extraText,
                    UndoHistoryRegistry.GetHistory(textView.TextBuffer)
                )

                If completionSurface.Adornment IsNot Nothing Then
                    completionSurface.Adornment.Dismiss(force:=False)
                End If

                textView.VisualElement.Focus()

                If extraText = ", " Then
                    provider.ShowHelp(", ")
                ElseIf repWith.EndsWith("(") Then
                    provider.ShowHelp(True)
                ElseIf showCompletionAdornmentAgain Then
                    provider.ShowCompletionAdornment(textView.TextSnapshot, textView.Caret.Position.TextInsertionIndex, True)
                End If
                Return True

            Else
                If completionSurface.Adornment IsNot Nothing Then
                    completionSurface.Adornment.Dismiss(force:=False)
                End If
                textView.VisualElement.Focus()
                Return False
            End If

        End Function

        Private Function CommitItem(textView As IAvalonTextView, adornmentSurface As CompletionSurface) As Boolean
            Dim compList = adornmentSurface.CompletionListBox
            Dim itemWrapper = TryCast(compList.SelectedItem, CompletionItemWrapper)
            Dim provider As CompletionProvider = Nothing

            If itemWrapper IsNot Nothing Then
                If textView.Properties.TryGetProperty(GetType(CompletionProvider), provider) Then
                    provider.CommitItem(itemWrapper)
                End If
                Return True

            ElseIf adornmentSurface.Adornment IsNot Nothing Then
                adornmentSurface.Adornment.Dismiss(force:=False)
            End If

            Return False
        End Function
    End Class
End Namespace
