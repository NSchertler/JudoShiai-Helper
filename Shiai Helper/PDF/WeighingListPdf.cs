using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using Shiai_Helper.Calculations.WeighingList;
using Shiai_Helper.Models;
using SkiaSharp;
using System;
using System.Linq;

namespace Shiai_Helper.PDF
{
    internal class WeighingListPdf
    {
        public static void Create(Tournament tournament, string path,
            WeighingListOptions options)
        {
            var groupingStrategy = new SimpleWeighingListGroupingStrategy();

            var pdf = PdfUtils.SetupDocument();

            // Table of contents
            var tocSection = pdf.AddSection();
            tocSection.PageSetup.PageFormat = options.PageSizeOverview;
            tocSection.PageSetup.Orientation = options.OrientationOverview;
            tocSection.AddParagraph().AddFormattedText("Wiegelisten", new Font("Arial", 14));
            foreach (var sheet in groupingStrategy.EnumerateSheets(tournament))
            {
                tocSection.AddParagraph($"☐ {sheet.Title}");
            }

            // Actual content
            foreach (var sheet in groupingStrategy.EnumerateSheets(tournament))
            {
                var section = pdf.AddSection();
                section.PageSetup.PageFormat = options.PageSize;
                section.PageSetup.Orientation = options.Orientation;

                PageSetup.GetPageSize(options.PageSize, out Unit pageWidth, out Unit pageHeight);
                if(options.Orientation == Orientation.Landscape)
                {
                    var temp = pageWidth;
                    pageWidth = pageHeight;
                    pageHeight = temp;
                }

                section.AddParagraph().AddFormattedText(sheet.Title, new Font("Arial", 14));

                var table = section.AddTable();
                table.Borders = new Borders() { Width = 1 };

                var widthForNo = new Unit(1, UnitType.Centimeter);
                var widthForId = new Unit(1, UnitType.Centimeter);
                var widthForPassCheck = new Unit(1.5, UnitType.Centimeter);
                var widthForWeight = new Unit(1.6, UnitType.Centimeter);
                var widthForCheck = new Unit(0);
                if (options.AddCheckColumn)
                    widthForCheck = new Unit(1.0, UnitType.Centimeter);
                var availablePageWidth = pageWidth - pdf.DefaultPageSetup.LeftMargin - pdf.DefaultPageSetup.RightMargin;
                var widthForName = availablePageWidth - widthForNo - widthForId - widthForPassCheck - widthForWeight - widthForCheck;

                table.AddColumn(widthForNo).Format.Alignment = ParagraphAlignment.Right;
                table.AddColumn(widthForId).Format.Alignment = ParagraphAlignment.Right;
                table.AddColumn(widthForName);
                table.AddColumn(widthForPassCheck);
                table.AddColumn(widthForWeight);
                if (options.AddCheckColumn)
                    table.AddColumn(widthForCheck);

                var headerRow = table.AddRow();
                headerRow.Format.Font.Bold = true;
                headerRow.HeadingFormat = true;
                headerRow.Cells[1].AddParagraph("ID");
                headerRow.Cells[2].AddParagraph("Name");
                headerRow.Cells[3].AddParagraph("Pass");
                headerRow.Cells[4].AddParagraph("Gewicht");
                if (options.AddCheckColumn)
                    headerRow.Cells[5].AddParagraph("OK?");

                var i = 0;
                foreach (var competitor in sheet.Competitors)
                {
                    ++i;
                    var row = table.AddRow();
                    row.HeightRule = RowHeightRule.AtLeast;
                    row.Height = new Unit(1, UnitType.Centimeter);
                    row.VerticalAlignment = VerticalAlignment.Center;
                    row.Cells[0].AddParagraph(i.ToString());
                    row.Cells[1].AddParagraph(competitor.Id.ToString());
                    row.Cells[2].AddParagraph($"{competitor.LastName}, {competitor.FirstName}" + Environment.NewLine + competitor.Club);
                    
                    if(options.UseExactWeight)
                        row.Cells[4].AddParagraph($"{competitor.WeightKilograms} kg");
                    else
                        row.Cells[4].AddParagraph(competitor.WeightCategory);
                }

                var coachIds = sheet.Competitors.Select(c => c.CoachId).Where(i => !string.IsNullOrEmpty(i)).Distinct().Order().ToList();
                if(coachIds.Any())
                {
                    var plural = coachIds.Count > 1 ? "s" : "";
                    section.AddParagraph($"Trainer ID{plural}: " + string.Join(", ", coachIds))
                        .Format.SpaceBefore = new Unit(0.5, UnitType.Centimeter);
                }
                
            }

            PdfUtils.SaveToFile(path, pdf);
        }
    }
}
