using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MedSystem.Core;
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
        public bool IsExpired { get; set; }
        public bool IsExpiring { get; set; }
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
            _allRows = EmployeeRepository.GetAll().Select(emp =>
            {
                var (isExpired, isExpiring) = ExpirationRules.GetPersonStatus(
                    new[] { emp.SanminimumDate, emp.MedicalExamDate, emp.FluorographyDate });
                return new EmployeeRow
                {
                    Id = emp.Id,
                    FullName = emp.FullName,
                    Affiliation = emp.Affiliation == "внешний" ? "внешний совместитель" : emp.Affiliation,
                    Sanminimum = emp.SanminimumDate,
                    MedicalExam = emp.MedicalExamDate,
                    Fluorography = emp.FluorographyDate,
                    IsExpired = isExpired,
                    IsExpiring = isExpiring,
                };
            }).ToList();
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (SearchBox == null || FilterBox == null)
                return;

            var query = SearchBox.Text?.Trim().ToLowerInvariant() ?? "";
            IEnumerable<EmployeeRow> filtered = _allRows;

            if (!string.IsNullOrEmpty(query))
                filtered = filtered.Where(r => r.FullName.ToLowerInvariant().Contains(query));

            filtered = FilterBox.SelectedIndex switch
            {
                1 => filtered.Where(r => r.IsExpired),   // Просроченные
                2 => filtered.Where(r => r.IsExpiring),  // Истекают (2 недели)
                _ => filtered,
            };

            var list = filtered.ToList();
            Rows.Clear();
            foreach (var row in list)
                Rows.Add(row);

            CountText.Text = $"Всего: {list.Count}";
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilter();

        private void FilterBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilter();
    }
}
