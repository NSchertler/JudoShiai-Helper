using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using PdfSharp.Fonts;
using PdfSharp.Snippets.Font;
using Shiai_Helper.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiai_Helper.PDF
{
    internal class ClubRankingPdf
    {
        public static void Create(ClubRanking ranking, string documentTitle, string path)
        {
            var pdf = PdfUtils.SetupDocument();

            var nonTrivialPointsPerPlace = ranking.Options.PointsPerPlace.Where(ppp => ppp.Value != 0).OrderBy(ppp => ppp.Key).ToList();

            var section = pdf.AddSection();

            var par = section.AddParagraph();
            par.AddFormattedText(documentTitle, new Font("Arial", 14));

            var table = section.AddTable();
            table.Borders = new Borders() { Bottom = new Border() { Width = 0.25 } };

            var widthForRank = new Unit(1, UnitType.Centimeter);
            var widthForPlaces = new Unit(1, UnitType.Centimeter);
            var widthForTotal = new Unit(1.5, UnitType.Centimeter);
            var availablePageWidth = pdf.DefaultPageSetup.PageWidth - pdf.DefaultPageSetup.LeftMargin - pdf.DefaultPageSetup.RightMargin;
            var widthForClub = new Unit(availablePageWidth - widthForRank - widthForPlaces * nonTrivialPointsPerPlace.Count - widthForTotal);

            table.AddColumn(widthForRank).Format.Alignment = ParagraphAlignment.Right;
            table.AddColumn(widthForClub);
            for (int i = 0; i < nonTrivialPointsPerPlace.Count; i++)
                table.AddColumn(widthForPlaces).Format.Alignment = ParagraphAlignment.Right;
            table.AddColumn(widthForTotal).Format.Alignment = ParagraphAlignment.Right;

            var headerRow1 = table.AddRow();
            var headerRow2 = table.AddRow();
            headerRow1.HeadingFormat = true;
            headerRow1.VerticalAlignment = VerticalAlignment.Bottom;
            headerRow1.Format.Font.Bold = true;
            headerRow2.HeadingFormat = true;
            headerRow2.Format.Font.Bold = true;

            headerRow1.Cells[0].AddParagraph("#");
            headerRow1.Cells[0].MergeDown = 1;

            headerRow1.Cells[1].AddParagraph("Verein");
            headerRow1.Cells[1].MergeDown = 1;

            headerRow1.Cells[2].AddParagraph("Platzierungen");
            headerRow1.Cells[2].Borders.Bottom = new Border() { Width = 0 };
            headerRow1.Cells[2].Format.Font.Bold = false;
            headerRow1.Cells[2].MergeRight = nonTrivialPointsPerPlace.Count - 1;
            headerRow1.Cells[2].Format.Alignment = ParagraphAlignment.Right;
            var nextCol = 2;
            var placeToCol = new Dictionary<int, int>();
            foreach (var ppp in nonTrivialPointsPerPlace)
            {
                placeToCol.Add(ppp.Key, nextCol);
                headerRow2.Cells[nextCol++].AddParagraph(ppp.Key.ToString());
            }
            var totalCol = nextCol;
            headerRow1.Cells[nextCol].AddParagraph("Pkt.");
            headerRow1.Cells[nextCol].MergeDown = 1;

            int beforeRank = -1;
            foreach (var club in ranking.ClubsSortedByRank)
            {
                Debug.WriteLine($"{club.Club}: {club.TotalPoints} points");

                var row = table.AddRow();
                if (club.Rank != beforeRank)
                    row.Cells[0].AddParagraph(club.Rank.ToString());
                beforeRank = club.Rank;
                row.Cells[1].AddParagraph(club.Club);
                foreach (var (place, count) in club.GetWinnerCounts())
                {
                    if (placeToCol.TryGetValue(place, out int col))
                    {
                        if (count != 0)
                            row.Cells[col].AddParagraph(count.ToString());
                        else
                            row.Cells[col].AddParagraph("·");
                    }

                }
                row.Cells[totalCol].AddParagraph(club.TotalPoints.ToString());
            }

            table.SetEdge(0, 0, totalCol + 1, 2, Edge.Bottom, BorderStyle.Single, 1.0, Colors.Black);
            PdfUtils.SaveToFile(path, pdf);
        }        
    }
}
