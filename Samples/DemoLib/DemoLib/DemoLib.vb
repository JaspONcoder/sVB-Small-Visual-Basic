﻿Imports Microsoft.SmallVisualBasic
Imports Microsoft.SmallVisualBasic.Library
Imports Microsoft.SmallVisualBasic.WinForms


''' <summary>
''' A demo sample library created by VB.NET for sVB. The source code exists in the DemoLib folder in the samples folder, and the DemoLib sample shows you how to use this libray.
''' Note that you can delete the DemoLib.dll and DempLib.xml from the sVB\bin\Lib folder if you done trying this demo libray.
''' </summary>
<SmallVisualBasicType>
Public Class DemoLib

    ''' <summary>
    ''' Gets or sets the value
    ''' </summary>
    <ReturnValueType(VariableType.Double)>
    Public Shared Property Value As Primitive

    ''' <summary>
    ''' Returns True if the Value is positive
    ''' </summary>
    <ReturnValueType(VariableType.Boolean)>
    Public Shared ReadOnly Property IsPositive As Primitive
        Get
            Return Value >= 0
        End Get
    End Property


    ''' <summary>
    ''' Increases the value by the given delta
    ''' </summary>
    ''' <param name="delata">a number</param>
    ''' <returns>the new value</returns>
    <ReturnValueType(VariableType.Double)>
    Public Shared Function Increase(delata As Primitive) As Primitive
        Value += delata
        Return Value
    End Function

    ''' <summary>
    ''' Decreases the value by the given delta
    ''' </summary>
    ''' <param name="delata">a number</param>
    ''' <returns>the new value</returns>
    <ReturnValueType(VariableType.Double)>
    Public Shared Function Decrease(delata As Primitive) As Primitive
        Value -= delata
        Return Value
    End Function

    ''' <summary>
    ''' retuns the Value as a string.
    ''' </summary>
    <ReturnValueType(VariableType.String)>
    Public Shared Function ToStr() As Primitive
        Return Value
    End Function

End Class
