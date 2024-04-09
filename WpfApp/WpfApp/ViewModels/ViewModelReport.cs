using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WpfApp.Models;
using WpfApp.Models.MicrosoftOffice;
using WpfApp.Properties;

namespace WpfApp.ViewModels
{
    internal class ViewModelReport : BaseViewModel
    {
        public DelegateCommand SelectFileCommand { get; private set; }
        public DelegateCommand GenerateReportCommand { get; private set; }
        public DelegateCommand CancelCommand { get; private set; }
        public FontNames FontName { get; set; } = FontNames.TimesNewRoman;
        public int FontSize { get; set; } = 14;

        private OpenFileDialog _openFileDialog;
        private int _invoiceNumber;
        private string _supplierName;
        private string _buyerName;
        private IEnumerable<Supply> _data;
        public string[] EnumFontNames { get; private set; }

        string _fileName = string.Empty;
        public string FileName
        {
            get => _fileName;
            set
            {
                _fileName = value;
                OnPropertyChanged(nameof(FileName));
            }
        }

        bool _buttonsEnabled = true;
        public bool ButtonOkEnabled 
        {
            get => _buttonsEnabled;
            set 
            {
                _buttonsEnabled = value;
                OnPropertyChanged(nameof(ButtonOkEnabled));
            }
        }

        public ViewModelReport(int invoiceNumber, string supplierName, string buyerName, IEnumerable<Supply> data) 
        {
            _invoiceNumber = invoiceNumber;
            _supplierName = supplierName;
            _buyerName = buyerName;
            _data = data;

            _openFileDialog = new OpenFileDialog();
            _openFileDialog.Filter = "Word Documents (*.docx)|*.docx|Excel Files (*.xlsx, *.xls)|*.xlsx;*.xls";

            SelectFileCommand = new DelegateCommand(obj => selectedFile());
            GenerateReportCommand = new DelegateCommand(obj => generateReport());
            CancelCommand = new DelegateCommand(obj => closeWindow());

            EnumFontNames = Enum.GetNames(typeof(FontNames));
        }

        private void selectedFile() 
        {
            if ((bool)_openFileDialog.ShowDialog()) 
            {
                FileName = _openFileDialog.FileName;
            }
        }

        private async void generateReport()
        {
            if (!fileNameIsOK()) 
            {
                MessageBox.Show("Некорректный путь к файлу.", "Проверьте путь к файлу!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            } 
            bool statusOK = false;
            ButtonOkEnabled = false;
            Report report = createReport();
            settingsUpReport(report);

            try
            {
                await Task.Run(() =>
                {
                    report.GenerateReport();
                });

                MessageBox.Show($"Путь к отчету: {_fileName}", "Отчет создан!", MessageBoxButton.OK, MessageBoxImage.Information);
                statusOK = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Не удалось сгенерировать отчет", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (statusOK)
            {
                saveInvoiceNumber(++_invoiceNumber);
                closeWindow();
            }
        }

        private Report createReport() 
        {
            Report report;
            if (Path.GetExtension(_fileName) == ".docx") report = createWordReport();
            else report = createExcelReport();
            return report;
        }

        private bool fileNameIsOK() 
        {
            bool statusOK = true;
            if (string.IsNullOrEmpty(_fileName)) statusOK = false;
            return statusOK;
        }

        private Report createWordReport()
        {
            return new WordReport(_fileName, _invoiceNumber, _supplierName, _buyerName, _data);
            
        }

        private Report createExcelReport()
        {
            return new ExcelReport(_fileName, _invoiceNumber, _supplierName, _buyerName, _data);
        }

        private void settingsUpReport(Report report) 
        {
            report.FontSize = FontSize;
            report.FontName = FontName;
        }

        private void saveInvoiceNumber(int invoiceNumber) 
        {
            Settings.Default.Id = invoiceNumber.ToString();
            Settings.Default.Save();
        }

        private void closeWindow() 
        {
            var displayRootRegistry = (Application.Current as App).displayRootRegistry;
            displayRootRegistry.HidePresentation(this);
        }
    }

    public class IntToFontSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int fontSize)
            {
                return fontSize.ToString();
            }
            return null;
              
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ComboBoxItem comboBoxItem = (ComboBoxItem)value;
            return System.Convert.ToInt32(comboBoxItem.Content);
        }
    }
}
