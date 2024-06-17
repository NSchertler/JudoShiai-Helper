using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using Shiai_Helper.Calculations.WeighingList;
using Shiai_Helper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Shiai_Helper.PDF
{
    internal class WeighingListPdf
    {
        public static void Create(Tournament tournament, string path, PageFormat pageSize, Orientation orientation)
        {
            var groupingStrategy = new SimpleWeighingListGroupingStrategy();

            var pdf = PdfUtils.SetupDocument();

            // Table of contents
            var tocSection = pdf.AddSection();
            tocSection.PageSetup.PageFormat = PageFormat.A4;
            tocSection.AddParagraph().AddFormattedText("Wiegelisten", new Font("Arial", 14));
            foreach (var sheet in groupingStrategy.EnumerateSheets(tournament))
            {
                tocSection.AddParagraph($"☐ {sheet.Title}");
            }

            // Actual content
            foreach (var sheet in groupingStrategy.EnumerateSheets(tournament))
            {
                var section = pdf.AddSection();
                section.PageSetup.PageFormat = pageSize;
                section.PageSetup.Orientation = orientation;

                PageSetup.GetPageSize(pageSize, out Unit pageWidth, out Unit pageHeight);
                if(orientation == Orientation.Landscape)
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
                var widthForWeight = new Unit(1.5, UnitType.Centimeter);
                var availablePageWidth = pageWidth - pdf.DefaultPageSetup.LeftMargin - pdf.DefaultPageSetup.RightMargin;
                var widthForName = new Unit(availablePageWidth - widthForNo - widthForId - widthForPassCheck - widthForWeight);


                table.AddColumn(widthForNo).Format.Alignment = ParagraphAlignment.Right;
                table.AddColumn(widthForId).Format.Alignment = ParagraphAlignment.Right;
                table.AddColumn(widthForName);
                table.AddColumn(widthForPassCheck);
                table.AddColumn(widthForWeight);

                var headerRow = table.AddRow();
                headerRow.Format.Font.Bold = true;
                headerRow.HeadingFormat = true;
                headerRow.Cells[1].AddParagraph("ID");
                headerRow.Cells[2].AddParagraph("Name");
                headerRow.Cells[3].AddParagraph("Pass");
                headerRow.Cells[4].AddParagraph("Gewicht");

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
