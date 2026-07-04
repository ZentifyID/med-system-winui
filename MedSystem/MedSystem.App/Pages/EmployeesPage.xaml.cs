using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MedSystem.Data.Repositories;

namespace MedSystem.App.Pages
{
    /// <summary>Строка таблицы сотрудников (модель для отображения).</summary>
    public class EmployeeRow
    {
        public long Id { get; set; }
        public string FullName { get; set; } = "";
        public string Affiliation { get; set; } = "";
        public string Sanminimum { get; set; } = "";
        public string MedicalExam { get; set; } = "";
        public string Fluorography { get; set; } = "";
    }

    public sealed partial class EmployeesPage : Page
    {
        private List<EmployeeRow> _allRows = new();
        public ObservableCollection<EmployeeRow> Rows { get; } = new();

        public EmployeesPage()
        {
            InitializeComponent();
            EmployeesList.ItemsSource = Rows;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LoadData();
        }

        private void LoadData()
        {
            _allRows = EmployeeRepository.GetAll().Select(emp => new EmployeeRow
            {
                Id = emp.Id,
                FullName = emp.FullName,
                Affiliation = emp.Affiliation == "внешний" ? "внешний совместитель" : emp.Affiliation,
                Sanminimum = emp.SanminimumDate,
                MedicalExam = emp.MedicalExamDate,
                Fluorography = emp.FluorographyDate,
            }).ToList();
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            var query = SearchBox.Text?.Trim().ToLowerInvariant() ?? "";
            var filtered = string.IsNullOrEmpty(query)
                ? _allRows
                : _allRows.Where(r => r.FullName.ToLowerInvariant().Contains(query)).ToList();

            Rows.Clear();
            foreach (var row in filtered)
                Rows.Add(row);

            CountText.Text = $"Всего: {filtered.Count}";
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }
    }
}
