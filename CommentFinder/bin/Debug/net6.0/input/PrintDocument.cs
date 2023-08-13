graphics.PageUnit = GraphicsUnit.Display;
//graphics.PageUnit = GraphicsUnit.Point;
//graphics.PageUnit = GraphicsUnit.Millimeter;
//graphics.PageUnit = GraphicsUnit.Inch;

// Счетчик строк
int counterLines = 0;
// Строка текста
string? lineText = null;
float leftMargin = e.MarginBounds.Left;
float topMargin = e.MarginBounds.Top;
// Высота шрифта в выбранных единицах объекта graphics.
float fontHeight = _printFont.GetHeight(graphics);
// Количество возможных строк для одной страницы.
float linesPerPage = e.MarginBounds.Height / fontHeight;

Pen rectPen = new(Brushes.Red)
{
    Alignment = System.Drawing.Drawing2D.PenAlignment.Inset
};

switch (graphics.PageUnit)
{
    case GraphicsUnit.Display:
        // Ширина пера 1мм
        rectPen.Width = (1 * 100.0f) / 25.4f;
        graphics.DrawRectangle(
            rectPen, 
            0, 0, 
            8.267716535433071f * 100.0f, 11.69291338582677f * 100.0f);
        break;
    case GraphicsUnit.Point:
        // Ширина пера 1мм
        rectPen.Width = (1 * 72.0f) / 25.4f;
        linesPerPage = 
            (e.MarginBounds.Height * (72.0f / 100.0f)) / fontHeight;
        leftMargin = e.MarginBounds.Left * (72.0f / 100.0f);
        topMargin = e.MarginBounds.Top * (72.0f / 100.0f);
        graphics.DrawRectangle(
            rectPen, 
            0, 0, 
            8.267716535433071f * 72.0f, 11.69291338582677f * 72.0f);
        break;

    case GraphicsUnit.Millimeter:
        // Ширина пера 1мм
        rectPen.Width = 1;
        linesPerPage = 
            (e.MarginBounds.Height * (25.4f / 100.0f)) / fontHeight;
        leftMargin = e.MarginBounds.Left * (25.4f / 100.0f);
        topMargin = e.MarginBounds.Top * (25.4f / 100.0f);
        graphics.DrawRectangle(rectPen, 0, 0, 210, 297);
        break;

    case GraphicsUnit.Inch:
        // Ширина пера 1мм
        rectPen.Width = 1 / 25.4f;
        linesPerPage = 
            (e.MarginBounds.Height / 100.0f) / fontHeight;
        leftMargin = e.MarginBounds.Left / 100.0f;
        topMargin = e.MarginBounds.Top / 100.0f;
        graphics.DrawRectangle(
        rectPen, 
        0, 0, 
        8.267716535433071f, 11.69291338582677f);
        break;
}


// Печать последовательно всех строк файла на одной странице.
while (counterLines < linesPerPage && 
        ((lineText = _printFile.ReadLine()) != null))
{
    // Координата позиции строки по высоте от верхнего края страницы.
    float yPos = topMargin + (counterLines * fontHeight);
    //
    graphics.DrawString(
        lineText,
        _printFont,
        _printColor,
        leftMargin,
        yPos
    );
    //
    counterLines++;
}