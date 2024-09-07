Imports Microsoft.SmallVisualBasic.Library

Namespace WinForms

    ''' <summary>
    ''' This class provides methods to deal with date and time
    ''' </summary>
    <SmallVisualBasicType>
    <HideFromIntellisense>
    Public NotInheritable Class DateEx

        ''' <summary>
        ''' Gets the full time part in the user local culture (including seconds, AM/PM and milliseconds) of the current date.
        ''' </summary>
        <ReturnValueType(VariableType.Date)>
        <ExProperty>
        Public Shared Function GetLongTime([date] As Primitive) As Primitive
            Return WinForms.Date.GetLongTime([date])
        End Function

        ''' <summary>
        ''' Gets the short time part of the current date in the user local culture. The time will incude hours and minutes and AM or PM, but not seconds and milliseconds.
        ''' </summary>
        <ReturnValueType(VariableType.Date)>
        <ExProperty>
        Public Shared Function GetShortTime([date] As Primitive) As Primitive
            Return WinForms.Date.GetShortTime([date])
        End Function

        ''' <summary>
        ''' Gets the long form of the current date. The long date contains the month name in the user local culture instead of its number.
        ''' </summary>
        ''' <returns>a string representing the long date in the user local culture</returns>
        <ReturnValueType(VariableType.Date)>
        <ExProperty>
        Public Shared Function GetLongDate([date] As Primitive) As Primitive
            Return WinForms.Date.GetLongDate([date])
        End Function

        ''' <summary>
        ''' Gets the short form of the current date in the user local culture, like 1/1/2020
        ''' </summary>
        ''' <returns>a string representing the short date in the user local culture.</returns>
        <ReturnValueType(VariableType.Date)>
        <ExProperty>
        Public Shared Function GetShortDate([date] As Primitive) As Primitive
            Return WinForms.Date.GetShortDate([date])
        End Function

        ''' <summary>
        ''' Gets the short date and long time representaion of the current date in the user local culture.
        ''' </summary>
        ''' <returns>a string representing the short date and long time in the user local culture</returns>
        <ReturnValueType(VariableType.String)>
        <ExProperty>
        Public Shared Function GetDateAndTime([date] As Primitive) As Primitive
            Return WinForms.Date.GetDateAndTime([date])
        End Function

        ''' <summary>
        ''' Gets the year of the current date.
        ''' </summary>
        ''' <returns>a number representing the year</returns>
        <ReturnValueType(VariableType.Double)>
        <ExProperty>
        Public Shared Function GetYear([date] As Primitive) As Primitive
            Return WinForms.Date.GetYear([date])
        End Function

        ''' <summary>
        ''' Gets the month of the current date.
        ''' </summary>
        ''' <returns>a number between 1 and 12 that represents the month</returns>
        <ReturnValueType(VariableType.Double)>
        <ExProperty>
        Public Shared Function GetMonth([date] As Primitive) As Primitive
            Return WinForms.Date.GetMonth([date])
        End Function

        ''' <summary>
        ''' Gets the English name of the month of the current date.
        ''' </summary>
        ''' <returns>the name of the month in English</returns>
        <ReturnValueType(VariableType.String)>
        <ExProperty> Public Shared Function GetEnglishMonthName([date] As Primitive) As Primitive
            Return WinForms.Date.GetEnglishMonthName([date])
        End Function

        ''' <summary>
        ''' Gets the local name of the month of the current date.
        ''' </summary>
        ''' <returns>the name of the month in the local language defined on the user system</returns>
        <ReturnValueType(VariableType.String)>
        <ExProperty>
        Public Shared Function GetMonthName([date] As Primitive) As Primitive
            Return WinForms.Date.GetMonthName([date])
        End Function

        ''' <summary>
        ''' Gets the day of the current date.
        ''' </summary>
        ''' <returns>a number between 1 and 31 that represents the day</returns>
        <ReturnValueType(VariableType.Double)>
        <ExProperty>
        Public Shared Function GetDay([date] As Primitive) As Primitive
            Return WinForms.Date.GetDay([date])
        End Function

        ''' <summary>
        ''' Gets the day number in the year of the current date. 
        ''' </summary>
        ''' <returns>a number between 1 and 366 that Represents the date in the week</returns>
        <ReturnValueType(VariableType.Double)>
        <ExProperty>
        Public Shared Function GetDayOfYear([date] As Primitive) As Primitive
            Return WinForms.Date.GetDayOfYear([date])
        End Function


        ''' <summary>
        ''' Gets the day of the week of the current date. Note that Sunday is the first day of the week. 
        ''' </summary>
        ''' <returns>a number between 1 and 7 that Represents the date in the week</returns>
        <ReturnValueType(VariableType.Double)>
        <ExProperty>
        Public Shared Function GetDayOfWeek([date] As Primitive) As Primitive
            Return WinForms.Date.GetDayOfWeek([date])
        End Function


        ''' <summary>
        ''' Gets the English name of the week day of the current date.
        ''' </summary>
        ''' <returns>the name of the week day in English</returns>
        <ReturnValueType(VariableType.String)>
        <ExProperty>
        Public Shared Function GetEnglishDayName([date] As Primitive) As Primitive
            Return WinForms.Date.GetEnglishDayName([date])
        End Function

        ''' <summary>
        ''' Gets the local name of the week day of the current date.
        ''' </summary>
        ''' <returns>the name of the week day in the local language defined on the user system</returns>
        <ReturnValueType(VariableType.String)>
        <ExProperty>
        Public Shared Function GetDayName([date] As Primitive) As Primitive
            Return WinForms.Date.GetDayName([date])
        End Function

        ''' <summary>
        ''' Gets the hour of the current date.
        ''' </summary>
        ''' <returns>a number between 0 and 23 that represents the hour</returns>
        <ReturnValueType(VariableType.Double)>
        <ExProperty>
        Public Shared Function GetHour([date] As Primitive) As Primitive
            Return WinForms.Date.GetHour([date])
        End Function


        ''' <summary>
        ''' Gets the minute of the current date.
        ''' </summary>
        ''' <returns>a number between 0 and 59 that represents the minute</returns>
        <ReturnValueType(VariableType.Double)>
        <ExProperty>
        Public Shared Function GetMinute([date] As Primitive) As Primitive
            Return WinForms.Date.GetMinute([date])
        End Function


        ''' <summary>
        ''' Gets the second of the current date.
        ''' </summary>
        ''' <returns>a number between 0 and 59 that represents the second</returns>
        <ReturnValueType(VariableType.Double)>
        <ExProperty>
        Public Shared Function GetSecond([date] As Primitive) As Primitive
            Return WinForms.Date.GetSecond([date])
        End Function

        ''' <summary>
        ''' Gets the millisecond of the current date.
        ''' </summary>
        ''' <returns>a number between 0 and 999 that represents the millisecond</returns>
        <ReturnValueType(VariableType.Double)>
        <ExProperty>
        Public Shared Function GetMillisecond([date] As Primitive) As Primitive
            Return WinForms.Date.GetMillisecond([date])
        End Function

        ''' <summary>
        ''' Gets the total ticks in the current date. Note that one second contains 10 million ticks!
        ''' </summary>
        ''' <returns>the total ticks</returns>
        <ReturnValueType(VariableType.Double)>
        <ExProperty>
        Public Shared Function GetTicks([date] As Primitive) As Primitive
            Return WinForms.Date.GetTicks([date])
        End Function

        ''' <summary>
        ''' Calculates the difference in milliseconds between the start of the year 1900 and the current date.
        ''' </summary>
        ''' <returns>the number of milliseconds that have elapsed since 1900 until the current date</returns>
        <ReturnValueType(VariableType.Double)>
        <ExProperty>
        Public Shared Function GetElapsedMilliseconds([date] As Primitive) As Primitive
            Return WinForms.Date.GetElapsedMilliseconds([date])
        End Function

        ''' <summary>
        ''' Creates a new date based on the current date with the year set to the given value.
        ''' </summary>
        ''' <param name="value">the new year</param>
        ''' <returns>a new date with the new year value. The input date will not change</returns>
        <ReturnValueType(VariableType.Date)>
        <ExMethod>
        Public Shared Function ChangeYear([date] As Primitive, value As Primitive) As Primitive
            Return WinForms.Date.ChangeYear([date], value)
        End Function

        ''' <summary>
        ''' Creates a new date based on the current date with the month set to the given value.
        ''' </summary>
        ''' <param name="value">a number between 1 and 12 that represents the month</param>
        ''' <returns>a new date with the new month value. The input date will not change</returns>
        <ReturnValueType(VariableType.Date)>
        <ExMethod>
        Public Shared Function ChangeMonth([date] As Primitive, value As Primitive) As Primitive
            Return WinForms.Date.ChangeMonth([date], value)
        End Function

        ''' <summary>
        ''' Creates a new date based on the current date with the day set to the given value.
        ''' </summary>
        ''' <param name="value">a number between 1 and 31 that represents the day</param>
        ''' <returns>a new date with the new day value. The input date will not change</returns>
        <ReturnValueType(VariableType.Date)>
        <ExMethod>
        Public Shared Function ChangeDay([date] As Primitive, value As Primitive) As Primitive
            Return WinForms.Date.ChangeDay([date], value)
        End Function

        ''' <summary>
        ''' Creates a new date based on the current date with the hour set to the given value.
        ''' </summary>
        ''' <param name="value">a number between 0 and 23 that represents the new hour</param>
        ''' <returns>a new date with the new hour value. The input date will not change</returns>
        <ReturnValueType(VariableType.Date)>
        <ExMethod>
        Public Shared Function ChangeHour([date] As Primitive, value As Primitive) As Primitive
            Return WinForms.Date.ChangeHour([date], value)
        End Function

        ''' <summary>
        ''' Creates a new date based on the current date with the minute set to the given value.
        ''' </summary>
        ''' <param name="value">a number between 0 and 59 that represents the new minute</param>
        ''' <returns>a new date with the new minute value. The input date will not change</returns>
        <ReturnValueType(VariableType.Date)>
        <ExMethod>
        Public Shared Function ChangeMinute([date] As Primitive, value As Primitive) As Primitive
            Return WinForms.Date.ChangeMinute([date], value)
        End Function

        ''' <summary>
        ''' Creates a new date based on the current date with the second set to the given value.
        ''' </summary>
        ''' <param name="value">a number between 0 and 59 that represents the new second</param>
        ''' <returns>a new date with the new second value. The input date will not change</returns>
        <ReturnValueType(VariableType.Date)>
        <ExMethod>
        Public Shared Function ChangeSecond([date] As Primitive, value As Primitive) As Primitive
            Return WinForms.Date.ChangeSecond([date], value)
        End Function

        ''' <summary>
        ''' Creates a new date based on the current date with the millisecond set to the given value.
        ''' </summary>
        ''' <param name="value">a number between 0 and 999 that represents the new millisecond</param>
        ''' <returns>a new date with the new millisecond value. The input date will not change</returns>
        <ReturnValueType(VariableType.Date)>
        <ExMethod>
        Public Shared Function ChangeMillisecond([date] As Primitive, value As Primitive) As Primitive
            Return WinForms.Date.ChangeMillisecond([date], value)
        End Function


        ''' <summary>
        ''' Creates a new date by adding the given years to the current date.
        ''' </summary>
        ''' <param name="value">the number of years you want to add. If the given date is a duration, the total days of the years will be calculated assuming that a year = 365.2425 days.</param>
        ''' <returns>a new date with the added years. The input date will not be affected</returns>
        <ReturnValueType(VariableType.Date)>
        <ExMethod>
        Public Shared Function AddYears([date] As Primitive, value As Primitive) As Primitive
            Return WinForms.Date.AddYears([date], value)
        End Function

        ''' <summary>
        ''' Creates a new date by adding the given months to the current date.
        ''' </summary>
        ''' <param name="value">the number of months you want to add. If the given date is a duration, the total days of the months will be calculated assuming that a month = 30.4375 days in avarage.</param>
        ''' <returns>a new date with the added months. The input date will not be affected</returns>
        <ReturnValueType(VariableType.Date)>
        <ExMethod>
        Public Shared Function AddMonths([date] As Primitive, value As Primitive) As Primitive
            Return WinForms.Date.AddMonths([date], value)
        End Function

        ''' <summary>
        ''' Creates a new date by adding the given days to the current date.
        ''' </summary>
        ''' <param name="value">the number of days you want to add</param>
        ''' <returns>a new date with the added days. The input date will not be affected</returns>
        <ReturnValueType(VariableType.Date)>
        <ExMethod>
        Public Shared Function AddDays([date] As Primitive, value As Primitive) As Primitive
            Return WinForms.Date.AddDays([date], value)
        End Function

        ''' <summary>
        ''' Creates a new date by adding the given hours to the current date.
        ''' </summary>
        ''' <param name="value">the number of hours you want to add</param>
        ''' <returns>a new date with the added hours. The input date will not be affected</returns>
        <ReturnValueType(VariableType.Date)>
        <ExMethod>
        Public Shared Function AddHours([date] As Primitive, value As Primitive) As Primitive
            Return WinForms.Date.AddHours([date], value)
        End Function

        ''' <summary>
        ''' Creates a new date by adding the given minutes to the current date.
        ''' </summary>
        ''' <param name="value">the number of minutes you want to add</param>
        ''' <returns>a new date with the added minutes. The input date will not be affected</returns>
        <ReturnValueType(VariableType.Date)>
        <ExMethod>
        Public Shared Function AddMinutes([date] As Primitive, value As Primitive) As Primitive
            Return WinForms.Date.AddMinutes([date], value)
        End Function

        ''' <summary>
        ''' Creates a new date by adding the given seconds to the current date.
        ''' </summary>
        ''' <param name="value">the number of seconds you want to add</param>
        ''' <returns>a new date with the added seconds. The input date will not be affected</returns>
        <ReturnValueType(VariableType.Date)>
        <ExMethod>
        Public Shared Function AddSeconds([date] As Primitive, value As Primitive) As Primitive
            Return WinForms.Date.AddSeconds([date], value)
        End Function

        ''' <summary>
        ''' Creates a new date by adding the given milliseconds to the current date.
        ''' </summary>
        ''' <param name="value">the number of milliseconds to add</param>
        ''' <returns>a new date with the added milliseconds. The input date will not be affected</returns>
        <ReturnValueType(VariableType.Date)>
        <ExMethod>
        Public Shared Function AddMilliseconds([date] As Primitive, value As Primitive) As Primitive
            Return WinForms.Date.AddMilliseconds([date], value)
        End Function

        ''' <summary>
        ''' Creates a new date by adding the current duration to the current date.
        ''' </summary>
        ''' <param name="duration">a date representing the duration you want to add</param>
        ''' <returns>a new date with the added duration. The input date will not be affected</returns>
        <ReturnValueType(VariableType.Date)>
        <ExMethod>
        Public Shared Function Add([date] As Primitive, duration As Primitive) As Primitive
            Return WinForms.Date.Add([date], duration)
        End Function

        ''' <summary>
        ''' Creates a new date by subtracting the given duration from the given date.
        ''' </summary>
        ''' <param name="value">the date\time or duration you want to subtract</param>
        ''' <returns>a duration representing the difference between the two dates. The input date will not be affected</returns>
        <ReturnValueType(VariableType.Date)>
        <ExMethod>
        Public Shared Function Subtract(date1 As Primitive, value As Primitive) As Primitive
            Return WinForms.Date.Subtract(date1, value)
        End Function

        ''' <summary>
        ''' Gets the total days in the current duration.
        ''' </summary>
        ''' <returns>the total days</returns>
        <ReturnValueType(VariableType.Double)>
        <ExProperty>
        Public Shared Function GetTotalDays(duration As Primitive) As Primitive
            Return WinForms.Date.GetTotalDays(duration)
        End Function


        ''' <summary>
        ''' Gets the total hours in the current duration.
        ''' </summary>
        ''' <returns>the total hours</returns>
        <ReturnValueType(VariableType.Double)>
        <ExProperty>
        Public Shared Function GetTotalHours(duration As Primitive) As Primitive
            Return WinForms.Date.GetTotalHours(duration)
        End Function

        ''' <summary>
        ''' Gets the total minutes in the current duration.
        ''' </summary>
        ''' <returns>the total minutes</returns>
        <ReturnValueType(VariableType.Double)>
        <ExProperty>
        Public Shared Function GetTotalMinutes(duration As Primitive) As Primitive
            Return WinForms.Date.GetTotalMinutes(duration)
        End Function


        ''' <summary>
        ''' Gets the total seconds in the current duration.
        ''' </summary>
        ''' <returns>the total seconds</returns>
        <ReturnValueType(VariableType.Double)>
        <ExProperty>
        Public Shared Function GetTotalSeconds(duration As Primitive) As Primitive
            Return WinForms.Date.GetTotalSeconds(duration)
        End Function


        ''' <summary>
        ''' Gets the approximate total months in the current duration, assuming that there are 12 months in the year, and each year contains 365.2425 days.
        ''' </summary>
        <ReturnValueType(VariableType.Double)>
        <ExProperty>
        Public Shared Function GetTotalMonths(duration As Primitive) As Primitive
            Return [Date].GetTotalMonths(duration)
        End Function

        ''' <summary>
        ''' Gets the approximate total years in the given duration, assuming that each year contains 365.2425 days.
        ''' </summary>
        <ReturnValueType(VariableType.Double)>
        <ExProperty>
        Public Shared Function GetTotalYears(duration As Primitive) As Primitive
            Return [Date].GetTotalYears(duration)
        End Function


        ''' <summary>
        ''' Gets the total milliseconds in the current duration.
        ''' </summary>
        ''' <returns>the total milliseconds</returns>
        <ReturnValueType(VariableType.Double)>
        <ExProperty>
        Public Shared Function GetTotalMilliseconds(duration As Primitive) As Primitive
            Return WinForms.Date.GetTotalMilliseconds(duration)
        End Function

        ''' <summary>
        ''' Gets the short date and short time string representaion of the given date formatted with the given culture.
        ''' </summary>
        ''' <param name="cultureName">the culture name used to format the date, like "en-US" for English (United States) culture, "ar-EG" for Arabic (Egypt) culture, and "ar-SA" for Arabic (Saudi Arabia) culture</param>
        ''' <returns>a string represent the date in the given culture</returns>
        <ReturnValueType(VariableType.String)>
        <ExMethod>
        Public Shared Function ToCulture([date] As Primitive, cultureName As Primitive) As Primitive
            Return WinForms.Date.ToCulture([date], cultureName)
        End Function


        ''' <summary>
        ''' Negatives the current duration.
        ''' </summary>
        ''' <returns>
        ''' If the duration is positive, returns the negative value of it.
        ''' If the duration is negative, returns the positive value of it.
        ''' If this is a date, it returns it as it can't be negated.
        ''' </returns>
        <ExMethod>
        <ReturnValueType(VariableType.Date)>
        Public Shared Function Negate(duration As Primitive) As Primitive
            Return [Date].Negate(duration)
        End Function
    End Class
End Namespace
