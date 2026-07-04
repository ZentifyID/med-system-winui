using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using MedSystem.Data.Repositories;

namespace MedSystem.App.Pages
{
    public class IcdRow
    {
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
    }

    /// <summary>Справочник кодов МКБ: просмотр, добавление, изменение, удаление.</summary>
    public sealed partial class IcdReferencePage : Page
    {
        private List<IcdRow> _allRows = new();
        public ObservableCollection<IcdRow> Rows { get; } = new();

        public IcdReferencePage()
        {
            InitializeComponent();
            // Страница кэшируется: поиск и фильтр сохраняются между переходами,
            // данные всё равно перезагружаются в OnNavigatedTo
            NavigationCacheMode = NavigationCacheMode.Required;
            IcdList.ItemsSource = Rows;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LoadData();
        }

        private void LoadData()
        {
            _allRows = IcdRepository.GetAll()
                .Select(c => new IcdRow { Code = c.Code, Name = c.Name })
                .ToList();
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            var query = SearchBox.Text?.Trim().ToLowerInvariant() ?? "";
            var filtered = string.IsNullOrEmpty(query)
                ? _allRows
                : _allRows.Where(r => r.Code.ToLowerInvariant().Contains(query)
                                   || r.Name.ToLowerInvariant().Contains(query)).ToList();

            Rows.Clear();
            foreach (var row in filtered)
                Rows.Add(row);

            CountText.Text = $"Всего: {filtered.Count}";
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilter();

        // ── Добавление / изменение ───────────────────────────────────

        private async void AddButton_Click(object sender, RoutedEventArgs e) =>
            await ShowEditDialogAsync(null);

        private void IcdList_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (IcdList.SelectedItem is IcdRow row)
                _ = ShowEditDialogAsync(row);
        }

        private void EditMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement { Tag: string code })
            {
                var row = _allRows.FirstOrDefault(r => r.Code == code);
                if (row != null)
                    _ = ShowEditDialogAsync(row);
            }
        }

        private async Task ShowEditDialogAsync(IcdRow? existing)
        {
            var codeBox = new TextBox
            {
                Header = "Код",
                PlaceholderText = "Например: J06.9",
                Text = existing?.Code ?? "",
                MaxLength = 10,
            };
            var nameBox = new TextBox
            {
                Header = "Наименование диагноза",
                Text = existing?.Name ?? "",
                MaxLength = 255,
                Margin = new Thickness(0, 12, 0, 0),
            };
            var panel = new StackPanel();
            panel.Children.Add(codeBox);
            panel.Children.Add(nameBox);

            var dialog = new ContentDialog
            {
                Title = existing == null ? "Новый код МКБ" : "Изменение кода МКБ",
                Content = panel,
                PrimaryButtonText = "Сохранить",
                CloseButtonText = "Отмена",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = XamlRoot,
            };

            if (await dialog.ShowAsync() != ContentDialogResult.Primary)
                return;

            var code = codeBox.Text.Trim();
            var name = nameBox.Text.Trim();
            if (code.Length == 0 || name.Length == 0)
            {
                await ShowMessageAsync("Ошибка", "Код и наименование не могут быть пустыми.");
                return;
            }

            var ok = existing == null
                ? IcdRepository.Insert(code, name)
                : IcdRepository.Update(existing.Code, code, name);

            if (!ok)
            {
                await ShowMessageAsync("Ошибка", $"Код «{code}» уже существует в справочнике.");
                return;
            }

            LoadData();
        }

        // ── Удаление ─────────────────────────────────────────────────

        private async void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not FrameworkElement { Tag: string code })
                return;

            var dialog = new ContentDialog
            {
                Title = "Удаление",
                Content = $"Удалить код «{code}» из справочника?",
                PrimaryButtonText = "Удалить",
                CloseButtonText = "Отмена",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = XamlRoot,
            };

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                IcdRepository.Delete(code);
                LoadData();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
        }

        private async Task ShowMessageAsync(string title, string message)
        {
            var dialog = new ContentDialog
            {
                Title = title,
                Content = message,
                CloseButtonText = "Понятно",
                XamlRoot = XamlRoot,
            };
            await dialog.ShowAsync();
        }
    }
}
