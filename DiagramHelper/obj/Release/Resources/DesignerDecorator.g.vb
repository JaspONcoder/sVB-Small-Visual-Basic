﻿#ExternalChecksum("..\..\..\Resources\DesignerDecorator.xaml","{8829d00f-11b8-4213-878b-770e8597ac16}","7A6122977EF3AE56EB265000FB42718E4360FB97DC83195E4EECF1DAECD7ECFD")
'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.42000
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict Off
Option Explicit On

Imports DiagramHelper
Imports System
Imports System.Diagnostics
Imports System.Windows
Imports System.Windows.Automation
Imports System.Windows.Controls
Imports System.Windows.Controls.Primitives
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Ink
Imports System.Windows.Input
Imports System.Windows.Markup
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Media.Effects
Imports System.Windows.Media.Imaging
Imports System.Windows.Media.Media3D
Imports System.Windows.Media.TextFormatting
Imports System.Windows.Navigation
Imports System.Windows.Shapes
Imports System.Windows.Shell


'''<summary>
'''DesignerDecorator
'''</summary>
<Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>  _
Partial Public Class DesignerDecorator
    Inherits System.Windows.ResourceDictionary
    Implements System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector
    
    Private _contentLoaded As Boolean
    
    '''<summary>
    '''InitializeComponent
    '''</summary>
    <System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")>  _
    Public Sub InitializeComponent() Implements System.Windows.Markup.IComponentConnector.InitializeComponent
        If _contentLoaded Then
            Return
        End If
        _contentLoaded = true
        Dim resourceLocater As System.Uri = New System.Uri("/DiagramHelper;component/resources/designerdecorator.xaml", System.UriKind.Relative)
        
        #ExternalSource("..\..\..\Resources\DesignerDecorator.xaml",1)
        System.Windows.Application.LoadComponent(Me, resourceLocater)
        
        #End ExternalSource
    End Sub
    
    <System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never),  _
     System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes"),  _
     System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity"),  _
     System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")>  _
    Sub System_Windows_Markup_IComponentConnector_Connect(ByVal connectionId As Integer, ByVal target As Object) Implements System.Windows.Markup.IComponentConnector.Connect
        Me._contentLoaded = true
    End Sub
    
    <System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never),  _
     System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes"),  _
     System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily"),  _
     System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")>  _
    Sub System_Windows_Markup_IStyleConnector_Connect(ByVal connectionId As Integer, ByVal target As Object) Implements System.Windows.Markup.IStyleConnector.Connect
        If (connectionId = 1) Then
            
            #ExternalSource("..\..\..\Resources\DesignerDecorator.xaml",30)
            AddHandler CType(target,System.Windows.Controls.ContextMenu).Opened, New System.Windows.RoutedEventHandler(AddressOf Me.ContextMenu_Opened)
            
            #End ExternalSource
        End If
        If (connectionId = 2) Then
            
            #ExternalSource("..\..\..\Resources\DesignerDecorator.xaml",31)
            AddHandler CType(target,System.Windows.Controls.MenuItem).Click, New System.Windows.RoutedEventHandler(AddressOf Me.NewMenuItem_Click)
            
            #End ExternalSource
        End If
        If (connectionId = 3) Then
            
            #ExternalSource("..\..\..\Resources\DesignerDecorator.xaml",32)
            AddHandler CType(target,System.Windows.Controls.MenuItem).Click, New System.Windows.RoutedEventHandler(AddressOf Me.OpenMenuItem_Click)
            
            #End ExternalSource
        End If
        If (connectionId = 4) Then
            
            #ExternalSource("..\..\..\Resources\DesignerDecorator.xaml",33)
            AddHandler CType(target,System.Windows.Controls.MenuItem).Click, New System.Windows.RoutedEventHandler(AddressOf Me.SaveMenuItem_Click)
            
            #End ExternalSource
        End If
        If (connectionId = 5) Then
            
            #ExternalSource("..\..\..\Resources\DesignerDecorator.xaml",34)
            AddHandler CType(target,System.Windows.Controls.MenuItem).Click, New System.Windows.RoutedEventHandler(AddressOf Me.SaveAsMenuItem_Click)
            
            #End ExternalSource
        End If
        If (connectionId = 6) Then
            
            #ExternalSource("..\..\..\Resources\DesignerDecorator.xaml",35)
            AddHandler CType(target,System.Windows.Controls.MenuItem).Click, New System.Windows.RoutedEventHandler(AddressOf Me.CloseMenuItem_Click)
            
            #End ExternalSource
        End If
        If (connectionId = 7) Then
            
            #ExternalSource("..\..\..\Resources\DesignerDecorator.xaml",37)
            AddHandler CType(target,System.Windows.Controls.MenuItem).Click, New System.Windows.RoutedEventHandler(AddressOf Me.SaveImageMenuItem_Click)
            
            #End ExternalSource
        End If
        If (connectionId = 8) Then
            
            #ExternalSource("..\..\..\Resources\DesignerDecorator.xaml",38)
            AddHandler CType(target,System.Windows.Controls.MenuItem).Click, New System.Windows.RoutedEventHandler(AddressOf Me.PrintMenuItem_Click)
            
            #End ExternalSource
        End If
        If (connectionId = 9) Then
            
            #ExternalSource("..\..\..\Resources\DesignerDecorator.xaml",40)
            AddHandler CType(target,System.Windows.Controls.MenuItem).Click, New System.Windows.RoutedEventHandler(AddressOf Me.SellectAllMenuItem_Click)
            
            #End ExternalSource
        End If
        If (connectionId = 10) Then
            
            #ExternalSource("..\..\..\Resources\DesignerDecorator.xaml",41)
            AddHandler CType(target,System.Windows.Controls.MenuItem).Click, New System.Windows.RoutedEventHandler(AddressOf Me.PasteMenuItem_Click)
            
            #End ExternalSource
        End If
        If (connectionId = 11) Then
            
            #ExternalSource("..\..\..\Resources\DesignerDecorator.xaml",42)
            AddHandler CType(target,System.Windows.Controls.MenuItem).Click, New System.Windows.RoutedEventHandler(AddressOf Me.UndoMenuItem_Click)
            
            #End ExternalSource
        End If
        If (connectionId = 12) Then
            
            #ExternalSource("..\..\..\Resources\DesignerDecorator.xaml",43)
            AddHandler CType(target,System.Windows.Controls.MenuItem).Click, New System.Windows.RoutedEventHandler(AddressOf Me.RedoMenuItem_Click)
            
            #End ExternalSource
        End If
        If (connectionId = 13) Then
            
            #ExternalSource("..\..\..\Resources\DesignerDecorator.xaml",46)
            AddHandler CType(target,System.Windows.Controls.MenuItem).Checked, New System.Windows.RoutedEventHandler(AddressOf Me.ShowGridMenuItem_Checked)
            
            #End ExternalSource
            
            #ExternalSource("..\..\..\Resources\DesignerDecorator.xaml",46)
            AddHandler CType(target,System.Windows.Controls.MenuItem).Unchecked, New System.Windows.RoutedEventHandler(AddressOf Me.ShowGridMenuItem_Unchecked)
            
            #End ExternalSource
        End If
        If (connectionId = 14) Then
            
            #ExternalSource("..\..\..\Resources\DesignerDecorator.xaml",47)
            AddHandler CType(target,System.Windows.Controls.MenuItem).Click, New System.Windows.RoutedEventHandler(AddressOf Me.DecreaseThicknessMenuItem_Click)
            
            #End ExternalSource
        End If
        If (connectionId = 15) Then
            
            #ExternalSource("..\..\..\Resources\DesignerDecorator.xaml",48)
            AddHandler CType(target,System.Windows.Controls.MenuItem).Click, New System.Windows.RoutedEventHandler(AddressOf Me.IncreaseThicknessMenuItem_Click)
            
            #End ExternalSource
        End If
        If (connectionId = 16) Then
            
            #ExternalSource("..\..\..\Resources\DesignerDecorator.xaml",49)
            AddHandler CType(target,System.Windows.Controls.MenuItem).Click, New System.Windows.RoutedEventHandler(AddressOf Me.GridBrushMenuItem_Click)
            
            #End ExternalSource
        End If
        If (connectionId = 17) Then
            
            #ExternalSource("..\..\..\Resources\DesignerDecorator.xaml",51)
            AddHandler CType(target,System.Windows.Controls.MenuItem).Click, New System.Windows.RoutedEventHandler(AddressOf Me.PageBackgroundMenuItem_Click)
            
            #End ExternalSource
        End If
        If (connectionId = 18) Then
            
            #ExternalSource("..\..\..\Resources\DesignerDecorator.xaml",52)
            AddHandler CType(target,System.Windows.Controls.MenuItem).Checked, New System.Windows.RoutedEventHandler(AddressOf Me.AllowTransparencyMenuItem_Checked)
            
            #End ExternalSource
            
            #ExternalSource("..\..\..\Resources\DesignerDecorator.xaml",52)
            AddHandler CType(target,System.Windows.Controls.MenuItem).Unchecked, New System.Windows.RoutedEventHandler(AddressOf Me.AllowTransparencyMenuItem_Checked)
            
            #End ExternalSource
        End If
        If (connectionId = 19) Then
            
            #ExternalSource("..\..\..\Resources\DesignerDecorator.xaml",53)
            AddHandler CType(target,System.Windows.Controls.MenuItem).Click, New System.Windows.RoutedEventHandler(AddressOf Me.PropertiesMenuItem_Click)
            
            #End ExternalSource
        End If
        If (connectionId = 20) Then
            
            #ExternalSource("..\..\..\Resources\DesignerDecorator.xaml",66)
            AddHandler CType(target,System.Windows.Controls.Menu).MouseDoubleClick, New System.Windows.Input.MouseButtonEventHandler(AddressOf Me.MainMenuBar_MouseDoubleClick)
            
            #End ExternalSource
        End If
    End Sub
End Class

