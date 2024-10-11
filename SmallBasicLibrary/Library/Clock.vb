Imports System.Globalization

Namespace Library

    ''' <summary>
    ''' This class provides access to the system clock
    ''' </summary>
    <SmallVisualBasicType>
    Public NotInheritable Class Clock

        ''' <summary>
        ''' Gets the current system time.
        ''' </summary>
        <WinForms.ReturnValueType(VariableType.Date)>
        Public Shared ReadOnly Property Time As Primitive
            Get
                Dim longTimePattern = DateTimeFormatInfo.GetInstance(CultureInfo.CurrentCulture).LongTimePattern
                Return New Primitive(Date.Now.ToString(longTimePattern, CultureInfo.CurrentUICulture))
            End Get
        End Property

        ''' <summary>
        ''' Gets the current system date.
        ''' </summary>
        <WinForms.ReturnValueType(VariableType.Date)>
        Public Shared ReadOnly Property [Date] As Primitive
            Get
                Dim shortDatePattern = DateTimeFormatInfo.GetInstance(CultureInfo.CurrentCulture).ShortDatePattern
                Return New Primitive(DateTime.Now.ToString(shortDatePattern, CultureInfo.CurrentUICulture))
            End Get
        End Property

        ''' <summary>
        ''' Gets the current year.
        ''' </summary>
        <WinForms.ReturnValueType(VariableType.Double)>
        Public Shared ReadOnly Property Year As Primitive
            Get
                Return DateTime.Now.Year
            End Get
        End Property

        ''' <summary>
        ''' Gets the current Month.
        ''' </summary>
        <WinForms.ReturnValueType(VariableType.Double)>
        Public Shared ReadOnly Property Month As Primitive
            Get
                Return DateTime.Now.Month
            End Get
        End Property

        ''' <summary>
        ''' Gets the current day of the month.
        ''' </summary>
        <WinForms.ReturnValueType(VariableType.Double)>
        Public Shared ReadOnly Property Day As Primitive
            Get
                Return DateTime.Now.Day
            End Get
        End Property

        ''' <summary>
        ''' Gets the current day of the week.
        ''' </summary>
        <WinForms.ReturnValueType(VariableType.String)>
        Public Shared ReadOnly Property WeekDay As Primitive
            Get
                Return New Primitive(DateTime.Now.ToString("dddd", CultureInfo.CurrentUICulture))
            End Get
        End Property

        ''' <summary>
        ''' Gets the current Hour.
        ''' </summary>
        <WinForms.ReturnValueType(VariableType.Double)>
        Public Shared ReadOnly Property Hour As Primitive
            Get
                Return Date.Now.Hour
            End Get
        End Property

        ''' <summary>
        ''' Gets the current Minute.
        ''' </summary>
        <WinForms.ReturnValueType(VariableType.Double)>
        Public Shared ReadOnly Property Minute As Primitive
            Get
                Return DateTime.Now.Minute
            End Get
        End Property

        ''' <summary>
        ''' Gets the current Second.
        ''' </summary>
        <WinForms.ReturnValueType(VariableType.Double)>
        Public Shared ReadOnly Property Second As Primitive
            Get
                Return DateTime.Now.Second
            End Get
        End Property

        ''' <summary>
        ''' Gets the current Millisecond.
        ''' </summary>
        <WinForms.ReturnValueType(VariableType.Double)>
        Public Shared ReadOnly Property Millisecond As Primitive
            Get
                Return DateTime.Now.Millisecond
            End Get
        End Property

        ''' <summary>
        ''' Gets the number of milliseconds that have elapsed since 1900.
        ''' </summary>
        <WinForms.ReturnValueType(VariableType.Double)>
        Public Shared ReadOnly Property ElapsedMilliseconds As Primitive
            Get
                Return (DateTime.Now - New DateTime(1900, 1, 1)).TotalMilliseconds
            End Get
        End Property
    End Class
End Namespace
