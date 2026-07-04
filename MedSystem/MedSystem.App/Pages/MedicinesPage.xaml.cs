using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MedSystem.Data.Repositories;

namespace MedSystem.App.Pages
{
    public class MedicineRow
    {
        public long Id { get; set; }
        public string Name { get; set; } = "";
        public string Quantity { get; set; } = "";
        public string Dosage { get; set; } = "";
        public string ExpirationDate { get; set; } = "";
    }

    public sealed partial class MedicinesPage : Page
    {
        private List<MedicineRow> _allRows = new();
        public ObservableCollection<MedicineRow> Rows { get; } = new();

        public MedicinesPage()
        {
            InitializeComponent();
            MedicinesList.ItemsSource = Rows;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LoadData();
        }

        private void LoadData()
        {
            _allRows = MedicineRepository.GetAll().Select(m => new MedicineRow
            {
                Id = m.Id,
                Name = m.Name,
                Quantity = m.Quantity.ToString(),
                Dosage = m.Dosage,
                ExpirationDate = m.ExpirationDate,
            }).ToList();
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            var query = SearchBox.Text?.Trim().ToLowerInvariant() ?? "";
            var filtered = string.IsNullOrEmpty(query)
                ? _allRows
                : _allRows.Where(r => r.Name.ToLowerInvariant().Contains(query)).ToList();

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
