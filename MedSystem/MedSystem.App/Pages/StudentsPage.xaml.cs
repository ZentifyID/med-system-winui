using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MedSystem.Data.Repositories;

namespace MedSystem.App.Pages
{
    public class StudentRow
    {
        public long Id { get; set; }
        public string FullName { get; set; } = "";
        public string GroupName { get; set; } = "";
        public string Sanminimum { get; set; } = "";
        public string MedicalExam { get; set; } = "";
        public string Fluorography { get; set; } = "";
    }

    public sealed partial class StudentsPage : Page
    {
        private List<StudentRow> _allRows = new();
        public ObservableCollection<StudentRow> Rows { get; } = new();

        public StudentsPage()
        {
            InitializeComponent();
            StudentsList.ItemsSource = Rows;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LoadData();
        }

        private void LoadData()
        {
            _allRows = StudentRepository.GetAll().Select(s => new StudentRow
            {
                Id = s.Id,
                FullName = s.FullName,
                GroupName = s.GroupName,
                Sanminimum = s.SanminimumDate,
                MedicalExam = s.MedicalExamDate,
                Fluorography = s.FluorographyDate,
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
