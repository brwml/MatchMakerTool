namespace MatchMaker.Scheduling.Exporters;

using System;
using System.Globalization;
using System.IO;

using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;

using MatchMaker.Models;

/// <summary>
/// Exports the schedule as a PDF file using iText.
/// </summary>
/// <seealso cref="IScheduleExporter" />
public class PdfScheduleExporter : BaseScheduleExporter
{
    private static readonly Color NavyBlue = new DeviceRgb(26, 54, 104);
    private static readonly Color LightBlue = new DeviceRgb(220, 227, 245);
    private static readonly Color AlternateRow = new DeviceRgb(238, 241, 252);
    private static readonly Color BorderColor = new DeviceRgb(160, 170, 200);
    private static readonly Color White = ColorConstants.WHITE;

    /// <summary>
    /// Exports the specified schedule as a PDF file.
    /// </summary>
    /// <param name="schedule">The schedule to export.</param>
    /// <param name="folder">The target folder.</param>
    public override void Export(Schedule schedule, string folder)
    {
        var fileName = Path.Combine(folder, FormattableString.Invariant($"{schedule.Name}.pdf"));
        var rooms = GetRooms(schedule);
        var roomHeaders = GetRoomHeaders(rooms);
        var rows = GetRows(schedule, rooms);
        var teamPairs = GetTeamPairs(schedule);
        var date = GetDate(schedule);

        using var writer = new PdfWriter(fileName);
        using var pdfDoc = new PdfDocument(writer);
        using var document = new Document(pdfDoc);

        AddTitle(document, schedule.Name);

        if (date != null)
        {
            AddDate(document, date);
        }

        document.Add(new Paragraph("\u00a0").SetFontSize(4));

        AddScheduleTable(document, roomHeaders, rows);

        document.Add(new Paragraph("\u00a0").SetFontSize(8));

        AddTeamsSection(document, teamPairs);

        document.Close();
    }

    private static void AddTitle(Document document, string title)
    {
        document.Add(
            new Paragraph(title)
                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                .SetFontSize(28)
                .SetFontColor(NavyBlue)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(2));
    }

    private static void AddDate(Document document, string date)
    {
        document.Add(
            new Paragraph(date)
                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                .SetFontSize(12)
                .SetFontColor(new DeviceRgb(80, 80, 80))
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(0));
    }

    private static void AddScheduleTable(Document document, List<string> roomHeaders, IList<RoundRow> rows)
    {
        // +1 column for the round-label column
        var table = new Table(roomHeaders.Count + 1)
            .UseAllAvailableWidth()
            .SetHorizontalAlignment(HorizontalAlignment.CENTER);

        // Header row
        table.AddHeaderCell(CreateHeaderCell(string.Empty));
        foreach (var room in roomHeaders)
        {
            table.AddHeaderCell(CreateHeaderCell(room));
        }

        // Data rows
        foreach (var row in rows)
        {
            var rowBg = row.IsOdd ? AlternateRow : White;
            table.AddCell(CreateLabelCell(row.Label, rowBg));
            foreach (var cell in row.Cells)
            {
                table.AddCell(CreateDataCell(cell, rowBg));
            }
        }

        document.Add(table);
    }

    private static void AddTeamsSection(Document document, IList<TeamPair> teamPairs)
    {
        document.Add(
            new Paragraph("Teams")
                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                .SetFontSize(14)
                .SetFontColor(NavyBlue)
                .SetMarginBottom(4));

        var table = new Table(2)
            .UseAllAvailableWidth()
            .SetHorizontalAlignment(HorizontalAlignment.LEFT);

        foreach (var pair in teamPairs)
        {
            table.AddCell(CreateTeamCell(pair.Left));
            table.AddCell(CreateTeamCell(pair.Right));
        }

        document.Add(table);
    }

    private static Cell CreateHeaderCell(string content)
    {
        return new Cell()
            .Add(new Paragraph(content)
                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                .SetFontSize(9)
                .SetFontColor(White)
                .SetTextAlignment(TextAlignment.CENTER))
            .SetBackgroundColor(NavyBlue)
            .SetBorder(new SolidBorder(BorderColor, 0.5f))
            .SetPadding(5);
    }

    private static Cell CreateLabelCell(string content, Color background)
    {
        return new Cell()
            .Add(new Paragraph(content)
                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                .SetFontSize(9)
                .SetFontColor(NavyBlue)
                .SetTextAlignment(TextAlignment.LEFT))
            .SetBackgroundColor(LightBlue)
            .SetBorder(new SolidBorder(BorderColor, 0.5f))
            .SetPadding(5);
    }

    private static Cell CreateDataCell(string content, Color background)
    {
        return new Cell()
            .Add(new Paragraph(content)
                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                .SetFontSize(9)
                .SetTextAlignment(TextAlignment.CENTER))
            .SetBackgroundColor(background)
            .SetBorder(new SolidBorder(BorderColor, 0.5f))
            .SetPadding(5);
    }

    private static Cell CreateTeamCell(TeamEntry? team)
    {
        var text = team != null
            ? FormattableString.Invariant($"{team.Abbreviation} - {team.Name}")
            : string.Empty;

        return new Cell()
            .Add(new Paragraph(text)
                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                .SetFontSize(9))
            .SetBorder(new SolidBorder(BorderColor, 0.5f))
            .SetPadding(4);
    }
}
