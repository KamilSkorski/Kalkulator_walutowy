using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Windows;
using Newtonsoft.Json;

namespace CurrencyCalculator
{
    public partial class MainWindow : Window
    {
        private List<string> calculateType = new List<string> { "Kod waluty", "Nazwa waluty" };
        private Dictionary<string, string> codeMidMap = new Dictionary<string, string>();
        private List<string> codes = new List<string>();
        private List<string> names = new List<string>();
        private List<string> mid = new List<string>();
        private List<Currency> currency;

        public MainWindow()
        {
            InitializeComponent();
            InitializeData();
        }

        private async void InitializeData()
        {

            try
            {
                ApiClient client = new ApiClient();
                Parser parser = new Parser();
                currency = parser.ParseData(client.Response);
                FillArrayLists();
                calculatePicker.ItemsSource = calculateType;
                calculatePicker.SelectedIndex = 0;
                fromPicker.ItemsSource = codes;
                fromPicker.SelectedIndex = codes.Count - 1;
                toPicker.ItemsSource = codes;
                toPicker.SelectedIndex = codes.Count - 1;
                SetLabel(fromPicker, fromLabel);
                SetLabel(toPicker, toLabel);
                SetDictionary(mid, codes);
                SetDatePicker();
                dateLabel.Content = "Dane z dnia: " + datePicker.SelectedDate;
            }
            catch (Exception)
            {
                connectionStatusLabel.Content = "Błąd połączenia";
                calculateButton.IsEnabled = false;
            }
        }

        private void FillArrayLists()
        {
            foreach (var curr in currency)
            {
                codes.Add(curr.Code);
                names.Add(curr.CurrencyName);
                mid.Add(curr.Mid);
            }
            codes.Add("PLN");
            names.Add("złoty polski");
            mid.Add("1");
        }

        private void SetDictionary(List<string> mid, List<string> codes)
        {
            for (int i = 0; i < mid.Count; i++)
            {
                codeMidMap[codes[i]] = mid[i];
            }

        }

        private void ChangeType(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (calculatePicker.SelectedIndex == 0)
            {
                fromPicker.ItemsSource = codes;
                fromPicker.SelectedIndex = codes.Count - 1;
                toPicker.ItemsSource = codes;
                toPicker.SelectedIndex = codes.Count - 1;
            }
            else
            {
                fromPicker.ItemsSource = names;
                fromPicker.SelectedIndex = names.Count - 1;
                toPicker.ItemsSource = names;
                toPicker.SelectedIndex = names.Count - 1;
            }
        }

        private void SetLabel(System.Windows.Controls.ComboBox box, System.Windows.Controls.Label label)
        {
            string? textToLabel = null;
            string boxValue = (string)box.SelectedValue;

            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i] == boxValue || names[i] == boxValue)
                {
                    textToLabel = codes[i];
                    break;
                }
            }
            label.Content = textToLabel;
        }

        private void SetFromLabel(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            SetLabel(fromPicker, fromLabel);
        }

        private void SetToLabel(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            resultLabel.Content = "";
            SetLabel(toPicker, toLabel);
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                float number = CalculateValue(codeMidMap[fromLabel.Content.ToString()], codeMidMap[toLabel.Content.ToString()]);
                resultLabel.Content = Math.Round(number, 2).ToString();


            }
            catch (Exception)
            {
                resultLabel.Content = "Błędne dane";

            }
        }

        private float CalculateValue(string currencyOne, string currencyTwo)
        {
            float one = float.Parse(currencyOne, CultureInfo.InvariantCulture);
            float two = float.Parse(currencyTwo, CultureInfo.InvariantCulture);
            float dataFromField = float.Parse(dataField.Text, CultureInfo.InvariantCulture);
            float result = (one * dataFromField) / two;
            return result;
        }

        private void SetDatePicker()
        {
            datePicker.SelectedDate = DateTime.Now;
            if (datePicker.SelectedDate.Value.DayOfWeek == DayOfWeek.Saturday)
            {
                datePicker.SelectedDate = datePicker.SelectedDate.Value.AddDays(-1);
            }
            if (datePicker.SelectedDate.Value.DayOfWeek == DayOfWeek.Sunday)
            {
                datePicker.SelectedDate = datePicker.SelectedDate.Value.AddDays(-2);
            }
        }

        private async void CalculateByDate(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                ApiClient client = new ApiClient("http://api.nbp.pl/api/exchangerates/tables/A/" + datePicker.SelectedDate.Value.ToString("yyyy-MM-dd"));
                Parser parser = new Parser();
                currency = parser.ParseData(client.Response);
                codes.Clear();
                mid.Clear();
                names.Clear();
                FillArrayLists();
                SetDictionary(mid, codes);
                dateLabel.Content = "Dane z dnia: " + datePicker.SelectedDate.Value.ToShortDateString();
                calculateButton.IsEnabled = true;
            }
            catch (Exception)
            {
                dateLabel.Content = "Brak danych z dnia: " + datePicker.SelectedDate.Value.ToShortDateString();
                calculateButton.IsEnabled = false;
            }
        }
    }
}

