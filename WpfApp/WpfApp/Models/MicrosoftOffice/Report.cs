using System.Collections.Generic;

namespace WpfApp.Models.MicrosoftOffice
{
    public enum FontNames
    {
        TimesNewRoman,
        Calibri
    }

    abstract class Report
    {

        public FontNames FontName { get; set; } = FontNames.TimesNewRoman;
        public int FontSize { get; set; } = 14;

        protected Dictionary<FontNames, string> _fontNames;
        protected IEnumerable<Supply> _objects;
        protected string _fileName;
        protected int _invoiceId;
        protected string _supplierName;
        protected string _buyerName;
        protected decimal _totalSum;
        protected string[] _headers = { "Код", "Название товара", "Кол-во", "Цена", "Сумма" };

        public Report(string fileName, int invoiceNumber, string supplier, string buyer, IEnumerable<Supply> objects)
        {
            _fileName = fileName;
            _objects = objects;
            _invoiceId = invoiceNumber;
            _supplierName = supplier;
            _buyerName = buyer;
            setTotalSum(_objects);


            _fontNames = new Dictionary<FontNames, string>()
            {
                {FontNames.TimesNewRoman, "Times new roman" },
                {FontNames.Calibri, "Calibri" }
            };
        }
        public void setHeaders(params string[] headers)
        {
            _headers = headers;
        }

        void setTotalSum(IEnumerable<Supply> items)
        {
            foreach (Supply item in items)
            {
                _totalSum += item.Sum;
            }
        }

        public abstract void GenerateReport();
    }
}
