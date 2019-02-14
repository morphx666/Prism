Public Class CustomCursors
    Public Enum Type
        HandPlus
        HandMinus
    End Enum

    Public Shared Function CreateCursor(type As Type) As Cursor
        Dim bmp As Bitmap = New Bitmap(Cursors.Hand.Size.Width * 2, Cursors.Hand.Size.Height * 2)
        Dim g As Graphics = Graphics.FromImage(bmp)
        Dim r As Rectangle = New Rectangle(bmp.Width - 20, bmp.Height - 12, 8, 8)

        g.PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality
        g.SmoothingMode = Drawing2D.SmoothingMode.HighSpeed

        Select Case type
            Case CustomCursors.Type.HandPlus
                Cursors.Hand.Draw(g, New Rectangle(New Point(bmp.Width / 2 - 5, bmp.Height / 2), bmp.Size))
                g.FillRectangle(Brushes.White, r)
                g.DrawRectangle(Pens.Black, r)
                g.DrawLine(Pens.Black, r.X + 4, r.Y + 1, r.X + 4, r.Y + 1 + 5)
                g.DrawLine(Pens.Black, r.X + 1, r.Y + 4, r.X + 1 + 5, r.Y + 4)
            Case CustomCursors.Type.HandMinus
                Cursors.Hand.Draw(g, New Rectangle(New Point(bmp.Width / 2 - 5, bmp.Height / 2), bmp.Size))
                g.FillRectangle(Brushes.White, r)
                g.DrawRectangle(Pens.Black, r)
                g.DrawLine(Pens.Black, r.X + 1, r.Y + 4, r.X + 1 + 5, r.Y + 4)
        End Select

        'ControlPaint.DrawMenuGlyph(g, New Rectangle(Cursor.Size.Width, Cursor.Size.Height, 8, 8), MenuGlyph.Max)

        g.Dispose()

        Return New Cursor(bmp.GetHicon())
    End Function
End Class
