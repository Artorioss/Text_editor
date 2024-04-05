using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Linq;
using WpfApp.Models.MicrosoftOffice;

namespace WpfApp.Models
{
    internal class WordReport : Report
    {
        private Application _WinWord = new Application();
        private Document _document;

        public WordReport(string fileName, int invoiceNumber, string supplier, string buyer, IEnumerable<Supply> objects)
            : base(fileName, invoiceNumber, supplier, buyer, objects)
        {
            _WinWord = new Application();
            _document = _WinWord.Documents.Add();
        }

        public override void GenerateReport()
        {
            AddText($"Расходная накладная №{_invoiceId} от {DateTime.Now.ToString("dd.MM.yyyy")}");
            AddText($"Поставщик: {_supplierName}");
            AddText($"Покупатель: {_buyerName}");
            AddTable(_objects, _headers);
            AddText($"Общая сумма: {_totalSum}", WdParagraphAlignment.wdAlignParagraphRight);
            _document.SaveAs(_fileName);
            _document.Close();
            _WinWord.Quit();
        }

        void AddText(string text)
        {
            Paragraph paragraph = createParagraph();
            paragraph.Range.Text = text;
            paragraph.Range.InsertParagraphAfter();
        }

        void AddText(string text, WdParagraphAlignment alignParagraph)
        {
            Paragraph paragraph = createParagraph(alignParagraph);
            paragraph.Range.Text = text;
            paragraph.Range.InsertParagraphAfter();
        }

        Paragraph createParagraph()
        {
            Paragraph paragraph = _document.Content.Paragraphs.Add();
            paragraph.Range.Font.Name = _fontNames[FontName];
            paragraph.Range.Font.Size = FontSize;
            return paragraph;
        }

        Paragraph createParagraph(WdParagraphAlignment alignParagraph)
        {
            Paragraph paragraph = createParagraph();
            paragraph.Range.ParagraphFormat.Alignment = alignParagraph;
            return paragraph;
        }

        void AddTable(IEnumerable<Supply> objects, string[] headers)
        {
            List<Supply> items = objects.ToList();
            Table table = createTable(headers.Length, items.Count + 1);
            insertHeadersInTable(table, headers);
            insertDataInTable(table, items);
        }

        Table createTable(int countColumns, int countRows)
        {
            Paragraph paragraph = createParagraph();
            paragraph.Range.InsertParagraphAfter();
            Table table = _document.Tables.Add(paragraph.Range, countRows, countColumns);
            table.Borders.Enable = 1;
            foreach (Row row in table.Rows)
            {
                foreach (Cell cell in row.Cells)
                {
                    cell.Range.Font.Name = _fontNames[FontName];
                    cell.Range.Font.Size = FontSize;
                }
            }
            return table;
        }

        void insertHeadersInTable(Table table, string[] headers)
        {
            table.ApplyStyleFirstColumn = true;
            Cells dataRow = table.Rows[1].Cells;
            for (int i = 0; i < headers.Length; i++)
            {
                dataRow[i + 1].Range.Text = headers[i];
            }
        }

        void insertDataInTable(Table table, List<Supply> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                Cells dataRow = table.Rows[i + 2].Cells;
                dataRow[1].Range.Text = items[i].Id.ToString();
                dataRow[2].Range.Text = items[i].Name;
                dataRow[3].Range.Text = items[i].Count.ToString();
                dataRow[4].Range.Text = items[i].Cost.ToString();
                dataRow[5].Range.Text = items[i].Sum.ToString();
            }
        }
    }
}
