using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MedSystem.Data.Repositories;

namespace MedSystem.App.Pages
{
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            EmployeesCount.Text = EmployeeRepository.Count().ToString();
            StudentsCount.Text = StudentRepository.Count().ToString();
            MedicinesCount.Text = MedicineRepository.Count().ToString();
            AppealsCount.Text = AppealRepository.Count().ToString();
        }

        // ── Быстрые действия ─────────────────────────────────────────

        private void AddEmployee_Click(object sender, RoutedEventArgs e) =>
            Frame.Navigate(typeof(EmployeeFormPage), 0L);

        private async void AddStudent_Click(object sender, RoutedEventArgs e)
        {
            if (GroupRepository.GetAll().Count == 0)
            {
                var dialog = new ContentDialog
                {
                    Title = "Нет групп",
                    Content = "Сначала добавьте хотя бы одну учебную группу (раздел «Студенты» → «Группы»).",
                    CloseButtonText = "Понятно",
                    XamlRoot = XamlRoot,
                    RequestedTheme = ActualTheme,
                };
                await dialog.ShowAsync();
                return;
            }
            Frame.Navigate(typeof(StudentFormPage), 0L);
        }

        private void AddMedicine_Click(object sender, RoutedEventArgs e) =>
            Frame.Navigate(typeof(MedicineFormPage), 0L);

        private void AddAppeal_Click(object sender, RoutedEventArgs e) =>
            Frame.Navigate(typeof(AppealFormPage), 0L);
    }
}
