using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using GrandSmetaReader.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrandSmetaReader.Services
{
    public static class SmetaExporter
    {
        private static Row CreateRow(SheetData sheetData)
        {
            if (sheetData is null)
            {
                throw new ArgumentNullException(nameof(sheetData));
            }

            Row newRow = new Row();
            return newRow;
        }

        private static Cell CreateCell(Row newRow, string cellValue)
        {
            if (newRow is null)
            {
                throw new ArgumentNullException(nameof(newRow));
            }

            Cell cell = new Cell();
            cell.DataType = CellValues.String;
            cell.CellValue = new CellValue(cellValue);
            return cell;
        }
        public static void ExportToExcel(IEnumerable<Chapter> content, string destination)
        {
            using (var workbook = SpreadsheetDocument.Create(destination, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
            {
                var workbookPart = workbook.AddWorkbookPart();

                workbook.WorkbookPart.Workbook = new Workbook();

                workbook.WorkbookPart.Workbook.Sheets = new Sheets();



                var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
                var sheetData = new SheetData();
                sheetPart.Worksheet = new Worksheet(sheetData);

                Sheets sheets = workbook.WorkbookPart.Workbook.GetFirstChild<Sheets>();
                string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);

                uint sheetId = 1;
                if (sheets.Elements<Sheet>().Count() > 0)
                {
                    sheetId =
                        sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                }

                Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = "smeta" };
                sheets.Append(sheet);

                //Заголовок
                var headerRow = CreateRow(sheetData);

                List<string> columns = new List<string>() { "#", "Код", "Позиция", "Ед.изм.", "Кол-во" };
                foreach (var column in columns)
                {
                    var cell = CreateCell(headerRow, column);
                    headerRow.AppendChild(cell);
                }
                sheetData.AppendChild(headerRow);

                //Контент
                foreach (var chapter in content)
                {
                    //глава
                    var newRow = CreateRow(sheetData);
                    var cell = CreateCell(newRow, chapter.Caption);
                    newRow.AppendChild(cell);
                    sheetData.AppendChild(newRow);

                    //Позиции
                    foreach (var pos in chapter.Positions)
                    {
                        newRow = CreateRow(sheetData);

                        cell = CreateCell(newRow, pos.Number.ToString());
                        newRow.AppendChild(cell);

                        cell = CreateCell(newRow, pos.Code);
                        newRow.AppendChild(cell);

                        cell = CreateCell(newRow, pos.Caption);
                        newRow.AppendChild(cell);

                        cell = CreateCell(newRow, pos.Units);
                        newRow.AppendChild(cell);

                        cell = CreateCell(newRow, pos.Qnty.Result.ToString());
                        newRow.AppendChild(cell);

                        sheetData.AppendChild(newRow);

                        //ресурсы
                        foreach (var res in pos.ResourcesList)
                        {
                            newRow = CreateRow(sheetData);

                            cell = CreateCell(newRow, "");
                            newRow.AppendChild(cell);

                            cell = CreateCell(newRow, res.Code);
                            newRow.AppendChild(cell);

                            cell = CreateCell(newRow, res.Caption);
                            newRow.AppendChild(cell);

                            cell = CreateCell(newRow, res.Units);
                            newRow.AppendChild(cell);

                            cell = CreateCell(newRow, res.Quantity.ToString());
                            newRow.AppendChild(cell);
                            sheetData.AppendChild(newRow);

                        }
                    }
                }
            }
        }
    }
}
