﻿Namespace Microsoft.SmallVisualBasic
    Public Enum TokenType
        Illegal
        Comment
        StringLiteral
        NumericLiteral
        DateLiteral
        [True]
        [False]
        [Nothing]
        Identifier
        [Else]
        [ElseIf]
        EndFor
        [Next]
        [EndIf]
        EndSub
        EndFunction
        [Return]
        ExitLoop
        EndWhile
        [Wend]
        [For]
        ForEach
        [Goto]
        [Stop]
        [If]
        [In]
        [Step]
        [Sub]
        [Function]
        [Then]
        [To]
        [While]
        [And]
        EqualsTo
        [Or]
        Dot
        Lookup
        Concatenation
        Addition
        Subtraction
        Division
        [Mod]
        Multiplication
        LineContinuity
        LeftParens
        RightParens
        LeftBracket
        RightBracket
        LeftBrace
        RightBrace
        LessThan
        LessThanOrEqualsTo
        GreaterThan
        GreaterThanOrEqualsTo
        NotEqualsTo
        Comma
        Colon
        SingleQuote
        DoubleQuote
        ContinueLoop
        Question
        HashQuestion
    End Enum
End Namespace
