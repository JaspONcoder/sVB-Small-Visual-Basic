﻿Public Class DesignerDecorator

    Function GetDesigner(Mi As Control) As Designer
        Dim Cntx As ContextMenu
        Dim p = TryCast(Mi.Parent, Control)

        Do
            If p Is Nothing Then Return Nothing
            If TypeOf p Is ContextMenu Then
                Cntx = p
                Exit Do
            End If
            p = TryCast(p.Parent, Control)
        Loop
        Return Helper.GetDesigner(Cntx.PlacementTarget)
    End Function

    Friend Sub ContextMenu_Opened(sender As Object, e As RoutedEventArgs)
        Dim Cntx As ContextMenu = sender
        Dim Dsn = Helper.GetDesigner(Cntx.PlacementTarget)
        For Each m In Cntx.Items
            Dim Mi = TryCast(m, MenuItem)
            If Mi IsNot Nothing Then
                Select Case Mi.Header
                    Case "New"
                        Mi.IsEnabled = Dsn.HasChanges OrElse Not (Dsn.IsNew AndAlso Designer.PageCount = 1)
                    Case "Save"
                        Mi.IsEnabled = Dsn.HasChanges
                    Case "Save As..."
                        Mi.IsEnabled = Dsn.XamlFile <> ""
                    Case "Save To Image", "Print"
                        Mi.IsEnabled = Dsn.Items.Count > 0
                    Case "Paste"
                        Mi.IsEnabled = Dsn.CanPaste
                    Case "Undo"
                        Mi.IsEnabled = Dsn.CanUndo
                    Case "Redo"
                        Mi.IsEnabled = Dsn.CanRedo
                    Case "Close"
                        Mi.IsEnabled = Not Dsn.IsNew OrElse Designer.PageCount > 1
                End Select
            End If
        Next

    End Sub


    Friend Sub SaveMenuItem_Click(sender As Object, e As RoutedEventArgs)
        GetDesigner(sender).Save()
    End Sub

    Friend Sub SaveAsMenuItem_Click(sender As Object, e As RoutedEventArgs)
        GetDesigner(sender).SaveAs()
    End Sub

    Private Sub NewMenuItem_Click(sender As Object, e As RoutedEventArgs)
        Designer.OpenNewPage()
    End Sub

    Private Sub CloseMenuItem_Click(sender As Object, e As RoutedEventArgs)
        Designer.ClosePage()
    End Sub

    Private Sub OpenMenuItem_Click(sender As Object, e As RoutedEventArgs)
        Designer.Open()
    End Sub

    Private Sub SaveImageMenuItem_Click(sender As Object, e As RoutedEventArgs)
        GetDesigner(sender).SaveToImage()
    End Sub

    Private Sub PrintMenuItem_Click(sender As Object, e As RoutedEventArgs)
        GetDesigner(sender).Print()
    End Sub

    Private Sub SellectAllMenuItem_Click(sender As Object, e As RoutedEventArgs)
        GetDesigner(sender).SelectAll()
    End Sub

    Private Sub PasteMenuItem_Click(sender As Object, e As RoutedEventArgs)
        GetDesigner(sender).Paste()
    End Sub

    Private Sub RedoMenuItem_Click(sender As Object, e As RoutedEventArgs)
        GetDesigner(sender).Redo()
    End Sub

    Private Sub UndoMenuItem_Click(sender As Object, e As RoutedEventArgs)
        GetDesigner(sender).Undo()
    End Sub

    Private Sub ShowGridMenuItem_Checked(sender As Object, e As RoutedEventArgs)
        GetDesigner(sender).ShowGrid = True
    End Sub

    Private Sub ShowGridMenuItem_Unchecked(sender As Object, e As RoutedEventArgs)
        GetDesigner(sender).ShowGrid = False
    End Sub

    Private Sub GridBrushMenuItem_Click(sender As Object, e As RoutedEventArgs)
        Commands.ChangeBrush(GetDesigner(sender).GridPen, Pen.BrushProperty)
    End Sub

    Private Sub PageBackgroundMenuItem_Click(sender As Object, e As RoutedEventArgs)
        Dim dsn = GetDesigner(sender)
        Commands.ChangeBrush(dsn.DesignerCanvas, Canvas.BackgroundProperty)
    End Sub

    Private Sub DecreaseThicknessMenuItem_Click(sender As Object, e As RoutedEventArgs)
        GetDesigner(sender).IncreaseGridThickness(-0.1)
    End Sub

    Private Sub IncreaseThicknessMenuItem_Click(sender As Object, e As RoutedEventArgs)
        GetDesigner(sender).IncreaseGridThickness(0.1)
    End Sub

    Private Sub PropertiesMenuItem_Click(sender As Object, e As RoutedEventArgs)
        Dim Dsn = GetDesigner(sender)
        Dim canvas = Dsn.DesignerCanvas
        Dim WndProps As New WndProperties

        With WndProps
            .Show() ' Show the form to apply its templates
            .Hide()
            .LeftValue = Dsn.PageLeft
            .NumLeft.IsCheckable = True
            .TopValue = Dsn.PageTop
            .NumTop.IsCheckable = True
            .WidthValue = Dsn.PageWidth
            .HeightValue = Dsn.PageHeight
            .MinWidthValue = canvas.MinWidth
            .MinHeightValue = canvas.MinHeight
            .MaxWidthValue = canvas.MaxWidth
            .MaxHeightValue = canvas.MaxHeight
            .RightToLeftValue = (canvas.FlowDirection = FlowDirection.RightToLeft)
            .cmbWordWrap.IsEnabled = False
            .TagValue = If(Dsn.Tag, "")
            .ToolTipValue = If(Dsn.PageToolTip, "")

            If .ShowDialog = True Then
                Dim unit As New UndoRedoUnit()
                Dim OldState As New PropertyState(Dsn)

                If Dsn.PageLeft <> .LeftValue Then
                    OldState.Add(Designer.PageLeftProperty)
                    Dsn.PageLeft = .LeftValue
                End If

                If Dsn.PageTop <> .TopValue Then
                    OldState.Add(Designer.PageTopProperty)
                    Dsn.PageTop = .TopValue
                End If

                If Dsn.PageWidth <> .WidthValue Then
                    OldState.Add(Designer.PageWidthProperty)
                    Dsn.PageWidth = .WidthValue.Value
                End If

                If Dsn.PageHeight <> .HeightValue Then
                    OldState.Add(Designer.PageHeightProperty)
                    Dsn.PageHeight = .HeightValue.Value
                End If

                Dim txt = If(.TagValue = "", Nothing, .TagValue)
                If Dsn.Tag <> txt Then
                    OldState.Add(Designer.TagProperty)
                    Dsn.Tag = txt
                End If

                txt = If(.ToolTipValue = "", Nothing, .ToolTipValue)
                If Dsn.PageToolTip <> txt Then
                    OldState.Add(Designer.PageToolTipProperty)
                    Dsn.PageToolTip = txt
                End If

                If OldState.HasChanges Then unit.Add(OldState)

                OldState = New PropertyState(canvas)
                If canvas.MinWidth <> .MinWidthValue Then
                    OldState.Add(Canvas.MinWidthProperty)
                    canvas.MinWidth = .MinWidthValue
                    Dsn.PageWidth = Math.Max(canvas.MinWidth, Dsn.PageWidth)
                End If

                If canvas.MinHeight <> .MinHeightValue Then
                    OldState.Add(Canvas.MinHeightProperty)
                    canvas.MinHeight = .MinHeightValue
                    Dsn.PageHeight = Math.Max(canvas.MinHeight, Dsn.PageHeight)
                End If

                If canvas.MaxWidth <> .MaxWidthValue Then
                    OldState.Add(Canvas.MaxWidthProperty)
                    canvas.MaxWidth = .MaxWidthValue
                    Dsn.PageWidth = Math.Min(canvas.MaxWidth, Dsn.PageWidth)
                End If

                If canvas.MaxHeight <> .MaxHeightValue Then
                    OldState.Add(Canvas.MaxHeightProperty)
                    canvas.MaxHeight = .MaxHeightValue
                    Dsn.PageHeight = Math.Min(canvas.MaxHeight, Dsn.PageHeight)
                End If

                Dim rtl = If(.RightToLeftValue, FlowDirection.RightToLeft, FlowDirection.LeftToRight)
                If canvas.FlowDirection <> rtl Then
                    OldState.Add(Canvas.FlowDirectionProperty)
                    canvas.FlowDirection = rtl
                End If

                If OldState.HasChanges Then unit.Add(OldState)

                If unit.Count > 0 Then Dsn.UndoStack.ReportChanges(unit)
            End If
        End With
    End Sub

    Private Sub AllowTransparencyMenuItem_Checked(sender As Object, e As RoutedEventArgs)
        Dim dsn = GetDesigner(sender)
        If dsn Is Nothing Then Return

        Dim m = CType(sender, MenuItem)
        Dim propertyState As New PropertyState(m)
        propertyState.Add(MenuItem.IsCheckedProperty, New ValuePair(Of Object)(Not m.IsChecked, m.IsChecked))
        Dim Unit As New UndoRedoUnit(propertyState)
        dsn.UndoStack.ReportChanges(Unit)
    End Sub

End Class
