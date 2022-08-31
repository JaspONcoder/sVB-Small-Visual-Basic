﻿Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Microsoft.Nautilus.Text
Imports Microsoft.Nautilus.Text.Editor
Imports Microsoft.SmallBasic.Completion

Namespace Microsoft.SmallBasic.LanguageService
    Public Module CompilerService
        Private _compiler As Compiler

        Public ReadOnly Property DummyCompiler As Compiler
            Get

                If _compiler Is Nothing Then
                    _compiler = New Compiler()
                End If

                Return _compiler
            End Get
        End Property

        Public Function Compile(programText As String, outputFilePath As String, errors As ICollection(Of String)) As Boolean
            Try
                Dim compiler As New Compiler()
                Dim fileNameWithoutExtension = Path.GetFileNameWithoutExtension(outputFilePath)
                Dim directoryName = Path.GetDirectoryName(outputFilePath)
                Dim list As List(Of [Error]) = compiler.Build(New StringReader(programText), fileNameWithoutExtension, directoryName)

                For Each item In list
                    errors.Add($"{item.Line + 1},{item.Column + 1}: {item.Description}")
                Next

                Return errors.Count = 0
            Catch ex As Exception
                errors.Add(ex.Message)
                Return False
            End Try
        End Function

        Public Function Compile(programText As String, errors As ICollection(Of String)) As Compiler
            Try
                Dim compiler As New Compiler()
                Dim list As List(Of [Error]) = compiler.Compile(New StringReader(programText))

                For Each item In list
                    errors.Add($"{item.Line + 1},{item.Column + 1}: {item.Description}")
                Next

                Return compiler
            Catch ex As Exception
                errors.Add(ex.Message)
                Return Nothing
            End Try
        End Function

        Public Sub FormatDocument(textBuffer As ITextBuffer, Optional lineNumber As Integer = -1, Optional prettyListing As Boolean = True)
            Dim snapshot = textBuffer.CurrentSnapshot
            Dim source As New TextBufferReader(snapshot)

            Dim indentationLevel = 0
            Dim start, [end] As Integer
            If lineNumber = -1 Then
                start = 0
                [end] = snapshot.LineCount - 1
            Else
                start = FindCurrentSubStart(snapshot, lineNumber)
                [end] = FindCurrentSubEnd(snapshot, lineNumber)
            End If

            Dim lines As New List(Of String)

            For i = start To [end]
                lines.Add(snapshot.GetLineFromLineNumber(i).GetText())
            Next

            Using textEdit = textBuffer.CreateEdit()
                For lineNum = 0 To lines.Count - 1
                    Dim line = snapshot.GetLineFromLineNumber(lineNum + start)
                    ' (lineNum) to send it not to be changed ByRef
                    Dim tokens = LineScanner.GetTokens(lines(lineNum), (lineNum), lines)

                    If tokens.Count = 0 Then
                        AdjustIndentation(textEdit, line, indentationLevel, lines(lineNum).Length)
                        Continue For
                    End If

                    If prettyListing OrElse lineNum <> lineNumber Then FormatLine(textEdit, line, tokens, 0)

                    Dim firstCharPos = tokens(0).Column

                    Select Case tokens(0).Type
                        Case TokenType.EndIf, TokenType.Next, TokenType.EndFor, TokenType.Wend, TokenType.EndWhile
                            indentationLevel -= 1
                            AdjustIndentation(textEdit, line, indentationLevel, firstCharPos)

                        Case TokenType.EndSub, TokenType.EndFunction
                            indentationLevel = 0
                            AdjustIndentation(textEdit, line, indentationLevel, firstCharPos)

                        Case TokenType.If, TokenType.For, TokenType.While
                            AdjustIndentation(textEdit, line, indentationLevel, firstCharPos)
                            indentationLevel += 1

                        Case TokenType.Sub, TokenType.Function
                            AdjustIndentation(textEdit, line, 0, firstCharPos)
                            indentationLevel = 1

                        Case TokenType.Else, TokenType.ElseIf
                            indentationLevel -= 1
                            AdjustIndentation(textEdit, line, indentationLevel, firstCharPos)
                            indentationLevel += 1

                        Case Else
                            If tokens.Count > 1 AndAlso tokens(1).Type = TokenType.Colon Then
                                AdjustIndentation(textEdit, line, Integer.MaxValue, firstCharPos)
                            Else
                                AdjustIndentation(textEdit, line, indentationLevel, firstCharPos)
                            End If
                    End Select

                    ' format sub lines
                    Dim lineStart = False, lineEnd = False, subLine = 0

                    Dim subLineOffset = 0
                    Dim indentStack As New Stack(Of Integer)
                    Dim firstSubLine = True
                    Dim n = tokens.Count - 1

                    For i = 1 To n
                        Dim t = tokens(i)
                        If t.subLine = 0 Then Continue For

                        If firstSubLine Then
                            firstSubLine = False
                            ' check the last token in the first line
                            i = i - 1
                            GoTo CheckLineEnd
                        End If

                        If t.subLine > subLine Then
                            lineStart = True
                            lineNum += 1
                            line = snapshot.GetLineFromLineNumber(lineNum + start)
                            If prettyListing OrElse lineNum <> lineNumber Then FormatLine(textEdit, line, tokens, i)
                        Else
                            lineStart = False
                        End If

                        subLine = t.subLine
                        lineEnd = (i = n OrElse tokens(i + 1).subLine > subLine)

                        Select Case t.Type
                            Case TokenType.LeftParens, TokenType.LeftBracket, TokenType.LeftCurlyBracket
                                If lineStart Then AdjustIndentation(textEdit, line, indentationLevel + subLineOffset, t.Column)
                                If Not lineEnd Then
                                    subLineOffset += 1
                                    indentStack.Push(subLineOffset)
                                End If

                            Case TokenType.RightParens, TokenType.RightBracket, TokenType.RightCurlyBracket
                                If Not lineEnd Then
                                    If indentStack.Count = 0 Then
                                        subLineOffset = Math.Max(0, subLineOffset - 1)
                                    Else
                                        indentStack.Pop()
                                        subLineOffset = If(indentStack.Count = 0, Math.Max(0, subLineOffset - 1), indentStack.Peek())
                                    End If

                                ElseIf lineStart Then
                                    subLineOffset = If(indentStack.Count = 0, Math.Max(0, subLineOffset - 1), Math.Max(0, indentStack.Peek() - 1))
                                End If

                                If lineStart Then AdjustIndentation(textEdit, line, indentationLevel + subLineOffset, t.Column)

                            Case TokenType.Addition, TokenType.Subtraction, TokenType.Multiplication, TokenType.Division, TokenType.Or, TokenType.And
                                If lineStart Then
                                    subLineOffset = Math.Max(1, subLineOffset)
                                    AdjustIndentation(textEdit, line, indentationLevel + subLineOffset, t.Column)
                                End If

                            Case Else
                                If lineStart Then AdjustIndentation(textEdit, line, indentationLevel + subLineOffset, t.Column)
                        End Select

                        If Not lineEnd Then Continue For

CheckLineEnd:
                        Dim last = If(tokens(i).ParseType = ParseType.Comment, i - 1, i)

                        If last = -1 Then
                            subLineOffset = 0
                            Continue For
                        End If

                        Select Case tokens(last).NormalizedText
                            Case ","
                                If indentStack.Count = 0 Then
                                    If subLineOffset = 0 Then subLineOffset = 1
                                Else
                                    subLineOffset = indentStack.Peek()
                                End If

                            Case "_", "+", "-", "*", "/", "and", "or"
                                subLineOffset = Math.Max(1, subLineOffset)
                            Case "="
                                subLineOffset += 1
                            Case "(", "{", "["
                                subLineOffset += 1
                                indentStack.Push(subLineOffset)

                            Case ")", "}", "]"
                                If indentStack.Count > 0 Then indentStack.Pop()
                        End Select

                    Next

                Next

                textEdit.Apply()
            End Using

            FixKeywords(textBuffer, start, [end])
            FixIdentifiers(textBuffer, start, [end])
        End Sub

        Private Sub FixKeywords(textBuffer As ITextBuffer, start As Integer, [end] As Integer)
            Dim snapshot = textBuffer.CurrentSnapshot
            Dim lines As New List(Of String)

            For i = start To [end]
                lines.Add(snapshot.GetLineFromLineNumber(i).GetText())
            Next

            Using textEdit = textBuffer.CreateEdit()
                For lineNum = 0 To lines.Count - 1
                    Dim line = snapshot.GetLineFromLineNumber(lineNum + start)
                    Dim tokens = LineScanner.GetTokens(lines(lineNum), lineNum, lines)

                    For Each t In tokens
                        If t.ParseType = ParseType.Keyword OrElse t.Type = TokenType.And OrElse t.Type = TokenType.Or Then
                            Dim keyword = t.Type.ToString()
                            If t.Text <> keyword Then textEdit.Replace(line.Start + t.Column, t.EndColumn - t.Column, keyword)
                        End If
                    Next
                Next
                textEdit.Apply()
            End Using
        End Sub

        Private Sub FixIdentifiers(textBuffer As ITextBuffer, start As Integer, [end] As Integer)
            Dim snapshot = textBuffer.CurrentSnapshot
            Dim symbolTable = GetsymbolTable(textBuffer)

            Using textEdit = textBuffer.CreateEdit()
                ' fix lib types and members
                For Each token In symbolTable.AllLibMembers
                    Dim line = snapshot.GetLineFromLineNumber(token.Line)
                    If line.LineNumber < start OrElse line.LineNumber > [end] Then Continue For

                    ' The exact type/method name is stored in the comment field
                    textEdit.Replace(line.Start + token.Column, token.EndColumn - token.Column, token.Comment)
                Next

                ' fix local vars definitions
                Dim locals = symbolTable.LocalVariables
                For i = 0 To locals.Count - 1
                    Dim expr = locals.Values(i)
                    Dim id = expr.Identifier
                    If Char.IsUpper(id.Text(0)) Then
                        Dim line = snapshot.GetLineFromLineNumber(id.Line)
                        Dim name = id.Text
                        Dim c = name(0).ToString().ToLower()
                        textEdit.Replace(line.Start + id.Column, 1, c)
                        id.Text = c + If(name.Length > 1, name.Substring(1), "")
                        expr.Identifier = id
                        symbolTable.LocalVariables(locals.Keys(i)) = expr
                    End If
                Next

                ' fix global vars definitions
                ToUpper(start, [end], snapshot, symbolTable.GlobalVariables, textEdit)

                ' fix subs definitions
                ToUpper(start, [end], snapshot, symbolTable.Subroutines, textEdit)

                ' fix labels definitions
                ToUpper(start, [end], snapshot, symbolTable.Labels, textEdit)

                ' fix dynamic properties definitions
                For Each dynObj In symbolTable.Dynamics
                    ToUpper(start, [end], snapshot, dynObj.Value, textEdit)
                Next

                ' fix vars usage
                For Each id In symbolTable.AllIdentifiers
                    Dim line = snapshot.GetLineFromLineNumber(id.Line)
                    If line.LineNumber < start OrElse line.LineNumber > [end] Then Continue For

                    ' fix local vars usage
                    If id.SymbolType = CompletionItemType.LocalVariable Then
                        Dim subName = Statements.SubroutineStatement.GetSubroutine(id)?.Name.NormalizedText
                        Dim key = $"{subName}.{id.NormalizedText}"
                        Dim name = symbolTable.LocalVariables(Key).Identifier.Text
                        If id.Text <> name Then textEdit.Replace(line.Start + id.Column, id.EndColumn - id.Column, name)
                        Continue For
                    End If

                    ' fix global vars usage
                    If id.SymbolType = CompletionItemType.GlobalVariable Then
                        If FixToken(id, line.Start, symbolTable.GlobalVariables, textEdit) Then
                            Continue For
                        End If
                    End If

                    ' fix event handlers and sub calls
                    If id.SymbolType = CompletionItemType.SubroutineName Then
                        If FixToken(id, line.Start, symbolTable.Subroutines, textEdit) Then
                            Continue For
                        End If
                    End If

                    ' fix goto labels
                    If id.SymbolType = CompletionItemType.Label Then
                        FixToken(id, line.Start, symbolTable.Labels, textEdit)
                    End If
                Next

                For Each obj In symbolTable.AllDynamicProperties
                    If Not symbolTable.Dynamics.ContainsKey(obj.Key) Then Continue For

                    Dim objName = obj.Key
                    Dim x = CompletionHelper.TrimData(objName)
                    Dim propDictionery = symbolTable.Dynamics(objName)
                    Dim fixed = False

                    For Each prop In obj.Value
                        Dim line = snapshot.GetLineFromLineNumber(prop.Line)
                        If line.LineNumber < start OrElse line.LineNumber > [end] Then Continue For

                        If Not FixToken(prop, line.Start, propDictionery, textEdit) Then
                            For Each type In symbolTable.Dynamics
                                If type.Key = objName Then Continue For ' Add before
                                Dim y = CompletionHelper.TrimData(type.Key)
                                If x.Contains(y) Then
                                    Dim propDictionery2 = symbolTable.Dynamics(type.Key)
                                    If FixToken(prop, line.Start, propDictionery2, textEdit) Then Exit For
                                End If
                            Next
                        End If
                    Next
                Next

                textEdit.Apply()
            End Using
        End Sub

        Private Function FixToken(token As Token, lineStart As Integer, dictionary As Dictionary(Of String, Token), textEdit As ITextEdit)
            Dim key = token.NormalizedText

            If dictionary.ContainsKey(key) Then
                Dim name = dictionary(key).Text
                If token.Text <> name Then textEdit.Replace(lineStart + token.Column, token.EndColumn - token.Column, name)
                Return True
            End If

            Return False
        End Function

        Private Sub ToUpper(start As Integer, [end] As Integer, snapshot As ITextSnapshot, dictionary As Dictionary(Of String, Token), textEdit As ITextEdit)
            For i = 0 To dictionary.Count - 1
                Dim token = dictionary.Values(i)
                If Char.IsLower(token.Text(0)) Then
                    Dim line = snapshot.GetLineFromLineNumber(token.Line)
                    If line.LineNumber >= start AndAlso line.LineNumber <= [end] Then
                        Dim name = token.Text
                        Dim c = token.Text(0).ToString().ToUpper()
                        textEdit.Replace(line.Start + token.Column, 1, c)
                        token.Text = c + If(name.Length > 1, name.Substring(1), "")
                        dictionary(dictionary.Keys(i)) = token
                    End If
                End If
            Next
        End Sub

        Private Function GetsymbolTable(buffer As ITextBuffer) As SymbolTable
            DummyCompiler.Compile(New TextBufferReader(buffer.CurrentSnapshot), True)
            Dim symbolTable = _compiler.Parser.SymbolTable
            symbolTable.ModuleNames = buffer.Properties.GetProperty(Of Dictionary(Of String, String))("ControlsInfo")
            symbolTable.ControlNames = buffer.Properties.GetProperty(Of List(Of String))("ControlNames")
            Return symbolTable
        End Function

        Public Function FindCurrentSubStart(textSnapshot As ITextSnapshot, LineNumber As Integer) As Integer
            For i = LineNumber To 0 Step -1
                Dim line = textSnapshot.GetLineFromLineNumber(i)
                Dim token = LineScanner.GetFirstToken(line.GetText(), i)

                Select Case token.Type
                    Case TokenType.Sub, TokenType.Function
                        Return Math.Max(0, i - 1)  ' i -1 To format prev EndSub in special case

                    Case TokenType.EndSub, TokenType.EndFunction
                        If i < LineNumber Then
                            Return i  'not i + 1 To format prev End Sub
                        End If

                End Select
            Next

            Return 0
        End Function

        Public Function FindCurrentSubEnd(textSnapshot As ITextSnapshot, LineNumber As Integer) As Integer
            For i = LineNumber + 1 To textSnapshot.LineCount - 1
                Dim line = textSnapshot.GetLineFromLineNumber(i)
                Dim token = LineScanner.GetFirstToken(line.GetText(), i)
                Select Case token.Type
                    Case TokenType.Sub, TokenType.Function
                        If i > LineNumber + 1 Then Return i - 1

                    Case TokenType.EndSub, TokenType.EndFunction
                        Return i
                End Select
            Next

            Return textSnapshot.LineCount - 1
        End Function


        Private Sub FormatLine(
                         textEdit As ITextEdit,
                         line As ITextSnapshotLine,
                         tokens As List(Of Token),
                         startAt As Integer
                    )

            Dim text = line.GetText()
            Dim textLen = text.Length
            Dim endAt = tokens.Count - 1

            Dim subLine = tokens(startAt).subLine

            For i = startAt To endAt
                Dim t = tokens(i)
                If t.subLine <> subLine Then Return

                Dim notLastToken = i < endAt AndAlso tokens(i + 1).subLine = subLine

                Select Case t.Type
                    Case TokenType.Equals, TokenType.NotEqualTo, TokenType.GreaterThan, TokenType.GreaterThanEqualTo,
                                   TokenType.LessThan, TokenType.LessThanEqualTo,
                                   TokenType.And, TokenType.Or,
                                   TokenType.Addition, TokenType.Multiplication, TokenType.Division
                        If notLastToken Then FixSpaces(textEdit, line, t, tokens(i + 1), 1)

                    Case TokenType.Subtraction
                        If notLastToken Then
                            If i = 0 OrElse tokens(i - 1).ParseType = ParseType.Operator Then
                                FixSpaces(textEdit, line, t, tokens(i + 1), If(tokens(i + 1).Type = TokenType.Subtraction, 0, 0))
                            Else
                                FixSpaces(textEdit, line, t, tokens(i + 1), If(tokens(i + 1).Type = TokenType.Subtraction, 0, 1))
                            End If
                        End If

                    Case TokenType.LeftBracket, TokenType.LeftCurlyBracket, TokenType.LeftParens
                        If notLastToken Then FixSpaces(textEdit, line, t, tokens(i + 1), 0)

                    Case TokenType.RightBracket, TokenType.RightCurlyBracket, TokenType.RightParens
                        If notLastToken Then
                            Select Case tokens(i + 1).Type
                                Case TokenType.Comma, TokenType.LeftBracket, TokenType.LeftCurlyBracket, TokenType.LeftParens, TokenType.RightBracket, TokenType.RightCurlyBracket, TokenType.RightParens
                                    FixSpaces(textEdit, line, t, tokens(i + 1), 0)
                                Case Else
                                    FixSpaces(textEdit, line, t, tokens(i + 1), 1)
                            End Select
                        End If

                    Case TokenType.Comma
                        If notLastToken Then FixSpaces(textEdit, line, t, tokens(i + 1), 1)

                    Case TokenType.Identifier, TokenType.StringLiteral, TokenType.NumericLiteral
                        If notLastToken Then
                            Select Case tokens(i + 1).Type
                                Case TokenType.Dot, TokenType.Lookup, TokenType.Comma, TokenType.Colon,
                                        TokenType.LeftBracket, TokenType.LeftCurlyBracket, TokenType.LeftParens,
                                        TokenType.RightBracket, TokenType.RightCurlyBracket, TokenType.RightParens
                                    FixSpaces(textEdit, line, t, tokens(i + 1), 0)
                                Case Else
                                    FixSpaces(textEdit, line, t, tokens(i + 1), 1)
                            End Select
                        End If

                    Case TokenType.Dot, TokenType.Lookup
                        If notLastToken Then FixSpaces(textEdit, line, t, tokens(i + 1), 0)

                    Case Else
                        If t.ParseType = ParseType.Keyword Then
                            If notLastToken Then FixSpaces(textEdit, line, t, tokens(i + 1), 1)
                        End If
                End Select

            Next
        End Sub

        Private Sub FixSpaces(
                       textEdit As ITextEdit,
                       line As ITextSnapshotLine,
                       token1 As Token,
                       token2 As Token,
                       requiredSpaces As Integer
                )

            If token2.Type = TokenType.Comment Then Return

            Dim spaces = token2.Column - token1.EndColumn
            If spaces < 0 OrElse spaces = requiredSpaces Then Return

            Dim n = spaces - requiredSpaces
            If n > 0 Then
                textEdit.Replace(line.Start + token2.Column - n, n, "")
            Else
                textEdit.Insert(line.Start + token2.Column, Space(n * -1))
            End If
        End Sub

        Private Sub AdjustIndentation(
                    textEdit As ITextEdit,
                    line As ITextSnapshotLine,
                    indentationLevel As Integer,
                    firstCharPos As Integer
                )

            Dim indent As Integer
            If indentationLevel < 0 Then
                indent = 0
            ElseIf indentationLevel = Integer.MaxValue Then ' Label
                indent = 1
            Else
                indent = indentationLevel * 3
            End If

            If firstCharPos <> indent Then
                textEdit.Replace(
                    line.Start, firstCharPos,
                    New String(" "c, indent)
                )
            End If

            ' Trim line end
            Dim x = line.GetText()
            Dim L = x.Length - x.TrimEnd().Length
            If L > 0 AndAlso L < x.Length Then
                Select Case x.Trim().ToLower()
                    Case "sub", "function", "while"
                    Case Else
                        textEdit.Delete(New Span(line.Start + line.Length - L, L))
                End Select
            End If
        End Sub
    End Module
End Namespace
