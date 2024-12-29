﻿Imports System
Imports System.Reflection.Emit
Imports Microsoft.SmallVisualBasic.Library
Imports Microsoft.SmallVisualBasic.Statements

Namespace Microsoft.SmallVisualBasic.Expressions
    <Serializable>
    Public Class IdentifierExpression
        Inherits Expression

        Public Identifier As Token
        Public Subroutine As SubroutineStatement

        Public Property IsParam As Boolean

        Public Overrides Sub AddSymbols(symbolTable As SymbolTable)
            MyBase.AddSymbols(symbolTable)
            Identifier.Parent = Me.Parent
            If Identifier.SymbolType = Completion.CompletionItemType.None Then
                Identifier.SymbolType = If(symbolTable.IsLocalVar(Me), Completion.CompletionItemType.LocalVariable, Completion.CompletionItemType.GlobalVariable)
            End If
            symbolTable.AddIdentifier(Identifier)
        End Sub

        Public Sub AddSymbolInitialization(symbolTable As SymbolTable)
            symbolTable.AddVariableInitialization(Identifier)
        End Sub

        Public Overrides Sub EmitIL(scope As CodeGenScope)
            Dim var = scope.GetLocalBuilder(Subroutine, Identifier)

            If var IsNot Nothing Then
                scope.ILGenerator.Emit(OpCodes.Ldloc, var)

            ElseIf scope.Fields.ContainsKey(Identifier.LCaseText) Then
                Dim field = scope.Fields(Identifier.LCaseText)
                scope.ILGenerator.Emit(OpCodes.Ldsfld, field)

            ElseIf Not CodeGenerator.IgnoreVarErrors Then
                scope.SymbolTable.Errors.Add(New [Error](Identifier, $"The variable `{Identifier.Text}` is used before being initialized."))
            End If
        End Sub

        Public Overrides Function ToString() As String
            Return Identifier.Text
        End Function

        Public Overrides Function InferType(symbolTable As SymbolTable) As VariableType
            Return symbolTable.GetInferedType(Identifier)
        End Function

        Public Overrides Function Evaluate(runner As Engine.ProgramRunner) As Primitive
            Dim value As Primitive = Nothing
            Dim fields = runner.Fields
            If fields.TryGetValue(runner.GetKey(Identifier), value) Then
                Return value
            End If

            Return Nothing
        End Function

        Public Overrides Function ToVB() As String
            Return Identifier.Text
        End Function
    End Class
End Namespace
