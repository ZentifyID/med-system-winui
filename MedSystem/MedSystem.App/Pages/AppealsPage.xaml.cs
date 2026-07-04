using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MedSystem.Data.Repositories;

namespace MedSystem.App.Pages
{
    public class AppealRow
    {
        public long Id { get; set; }
        public string Number { get; set; } = "";
        public string CreatedAt { get; set; } = "";
        public string Sender { get; set; } = "";
        public string Complaints { get; set; } = "";
    }

    public sealed partial class AppealsPage : Page
    {
        private List<AppealRow> _allRows = new();
        public ObservableCollection<AppealRow> Rows { get; } = new();

        public AppealsPage()
        {
            InitializeComponent();
            AppealsList.ItemsSource = Rows;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LoadData();
        }

        private void LoadData()
        {
            _allRows = AppealRepository.GetAll().Select(a => new AppealRow
            {
                Id = a.Id,
                Number = a.Number.ToString(),
                CreatedAt = a.CreatedAt,
                Sender = FormatInitials(a.Sender),
                Complaints = a.Complaints,
            }).ToList();
            ApplyFilter();
        }

        /// <summary>"Иванов Пётр Сергеевич" → "Иванов П. С."</summary>
        private static string FormatInitials(string fullName)
        {
            var parts = fullName.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
            return parts.Length switch
            {
                0 => "",
                1 => parts[0],
                2 => $"{parts[0]} {parts[1][0]}.",
                _ => $"{parts[0]} {parts[1][0]}. {parts[2][0]}.",
            };
        }

        private void ApplyFilter()
        {
            var query = SearchBox.Text?.Trim().ToLowerInvariant() ?? "";
            var filtered = string.IsNullOrEmpty(query)
                ? _allRows
                : _allRows.Where(r => r.Number.Contains(query)
                                   || r.Sender.ToLowerInvariant().Contains(query)).ToList();

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
