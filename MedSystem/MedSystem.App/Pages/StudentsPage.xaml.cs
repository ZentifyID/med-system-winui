using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using MedSystem.Core;
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
        public bool IsExpired { get; set; }
        public bool IsExpiring { get; set; }
    }

    public sealed partial class StudentsPage : Page
    {
        private List<StudentRow> _allRows = new();
        public ObservableCollection<StudentRow> Rows { get; } = new();

        public StudentsPage()
        {
            InitializeComponent();
            // Страница кэшируется: поиск и фильтр сохраняются между переходами,
            // данные всё равно перезагружаются в OnNavigatedTo
            NavigationCacheMode = NavigationCacheMode.Required;
            StudentsList.ItemsSource = Rows;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LoadData();
        }

        private void LoadData()
        {
            _allRows = StudentRepository.GetAll().Select(s =>
            {
                var (isExpired, isExpiring) = ExpirationRules.GetPersonStatus(
                    new[] { s.SanminimumDate, s.MedicalExamDate, s.FluorographyDate });
                return new StudentRow
                {
                    Id = s.Id,
                    FullName = s.FullName,
                    GroupName = s.GroupName,
                    Sanminimum = s.SanminimumDate,
                    MedicalExam = s.MedicalExamDate,
                    Fluorography = s.FluorographyDate,
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
            IEnumerable<StudentRow> filtered = _allRows;

            if (!string.IsNullOrEmpty(query))
                filtered = filtered.Where(r => r.FullName.ToLowerInvariant().Contains(query));

            filtered = FilterBox.SelectedIndex switch
            {
                1 => filtered.Where(r => r.IsExpired),
                2 => filtered.Where(r => r.IsExpiring),
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

        // ── Действия ─────────────────────────────────────────────────

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (GroupRepository.GetAll().Count == 0)
            {
                var dialog = new ContentDialog
                {
                    Title = "Нет групп",
                    Content = "Сначала добавьте хотя бы одну учебную группу.",
                    PrimaryButtonText = "К группам",
                    CloseButtonText = "Отмена",
                    XamlRoot = XamlRoot,
                };
                if (await dialog.ShowAsync() == ContentDialogResult.Primary)
                    Frame.Navigate(typeof(GroupsPage));
                return;
            }
            Frame.Navigate(typeof(StudentFormPage), 0L);
        }

        private void GroupsButton_Click(object sender, RoutedEventArgs e) =>
            Frame.Navigate(typeof(GroupsPage));

        private void StudentsList_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (StudentsList.SelectedItem is StudentRow row)
                Frame.Navigate(typeof(StudentFormPage), row.Id);
        }

        private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement { Tag: long id })
                Frame.Navigate(typeof(StudentFormPage), id);
        }

        private async void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not FrameworkElement { Tag: long id })
                return;

            var row = _allRows.FirstOrDefault(r => r.Id == id);
            var dialog = new ContentDialog
            {
                Title = "Удаление",
                Content = $"Удалить студента «{row?.FullName}»?",
                PrimaryButtonText = "Удалить",
                CloseButtonText = "Отмена",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = XamlRoot,
            };

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                StudentRepository.Delete(id);
                LoadData();
            }
        }
    }
}
