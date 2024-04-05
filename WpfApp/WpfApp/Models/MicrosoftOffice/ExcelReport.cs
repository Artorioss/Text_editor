using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WpfApp.Models.MicrosoftOffice;

namespace WpfApp.Models
{
    internal class ExcelReport : Report
    {
        Application _excelApplication;
        Workbook _workbook;
        Worksheet _worksheet;

        public ExcelReport(string fileName, int invoiceNumber, string supplier, string buyer, IEnumerable<Supply> objects)
            : base(fileName, invoiceNumber, supplier, buyer, objects)
        {
            _excelApplication = new Application();
            _workbook = _excelApplication.Workbooks.Add();
            _worksheet = _workbook.ActiveSheet;
        }

        public override void GenerateReport()
        {
            AddTable(_objects, _headers, 5, 2);
            InsertText($"Расходная накладная №{_invoiceId} от {DateTime.Now}", 2, 7);
            InsertText($"Поставщик: {_supplierName}", 2, 2);
            InsertText($"Покупатель: {_buyerName}", 3, 2);
            InsertText($"Общая сумма: {_totalSum}", _objects.Count() + 6, _headers.Length);
            if (File.Exists(_fileName)) File.Delete(_fileName);
            _workbook.SaveAs(_fileName);
            _excelApplication.Quit();
        }

        private void AddTable(IEnumerable<Supply> data, string[] headers)
        {
            insertHeadersInTable(headers);
            int index = 2;
            foreach (Supply item in data)
            {
                _worksheet.Cells[index, 1] = item.Id;
                _worksheet.Cells[index, 2] = item.Name;
                _worksheet.Cells[index, 3] = item.Count;
                _worksheet.Cells[index, 4] = item.Cost;
                _worksheet.Cells[index, 5] = item.Sum;

                Range cell = _worksheet.Cells[index, 1];
                cell.Font.Name = _fontNames[FontName]; 
                cell.Font.Size = FontSize;

                setBorders(index, headers.Length);
                index++;
            }
            _worksheet.Columns.AutoFit();
        }

        private void AddTable(IEnumerable<Supply> data, string[] headers, int startRow, int startColumn)
        {
            insertHeadersInTable(headers, startRow, startColumn);
            int index = 1;
            foreach (Supply item in data)
            {
                _worksheet.Cells[index + startRow, startColumn] = item.Id;
                _worksheet.Cells[index + startRow, startColumn + 1] = item.Name;
                _worksheet.Cells[index + startRow, startColumn + 2] = item.Count;
                _worksheet.Cells[index + startRow, startColumn + 3] = item.Cost;
                _worksheet.Cells[index + startRow, startColumn + 4] = item.Sum;

                setStyleToCell(index + startRow, startColumn, startColumn + 5);
                setBorders(index, headers.Length, startRow, startColumn);
                index++;
            }
            _worksheet.Columns[startColumn, Type.Missing].AutoFit();
        }

        private void setStyleToCell(int rowIndex, int columnStart, int columnEnd)
        {
            for (int i = columnStart; i < columnEnd; i++)
            {
                Range cell = _worksheet.Cells[rowIndex, i];
                cell.Font.Name = _fontNames[FontName];
                cell.Font.Size = FontSize;
            }
        }

        private void setBorders(int rowIndex, int columnCount, int startRow, int startColumn)
        {
            Range range = _worksheet.Range[_worksheet.Cells[startRow, startColumn], _worksheet.Cells[startRow + rowIndex, startColumn + columnCount - 1]];
            range.Borders.LineStyle = XlLineStyle.xlContinuous;
            range.Borders.Weight = XlBorderWeight.xlThin;
        }

        private void setBorders(int rowIndex, int columnCount)
        {
            Range range = _worksheet.Range[_worksheet.Cells[rowIndex, 1], _worksheet.Cells[rowIndex, columnCount]];
            range.Borders.LineStyle = XlLineStyle.xlContinuous;
            range.Borders.Weight = XlBorderWeight.xlThin;
        }

        private void insertHeadersInTable(string[] headers, int startRow, int startColumn)
        {
            for (int i = 0; i < headers.Length; i++)
            {
                _worksheet.Cells[startRow, startColumn + i] = headers[i];
            }
            setBorders(1, headers.Length, startRow, startColumn);
            setStyleToCell(startRow, startColumn, startColumn + headers.Length);
        }

        private void insertHeadersInTable(string[] headers)
        {
            for (int i = 0; i < headers.Length; i++)
            {
                _worksheet.Cells[1, i] = headers[i];
            }
            setBorders(1, headers.Length);
            setStyleToCell(1, 1, headers.Length);
        }

        private void InsertText(string text, int row, int column)
        {
            Range range = (Range)_worksheet.Cells[row, column];
            range.Value = text;
            range.Font.Name = _fontNames[FontName];
            range.Font.Size = FontSize;
        }
    }
}
