using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MedSystem.App.Pages;
using MedSystem.Data.Repositories;

namespace MedSystem.App
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Title = "Med System";
            AppWindow.Resize(new Windows.Graphics.SizeInt32(1280, 820));
            ContentFrame.Navigate(typeof(HomePage));

            // Автоперевод групп на следующий курс (раз в год после 15 августа)
            DispatcherQueue.TryEnqueue(CheckAcademicYear);
        }

        private void CheckAcademicYear()
        {
            try
            {
                var count = GroupRepository.CheckAndAutoIncrementGroups();
                if (count > 0)
                {
                    StartupInfoBar.Message =
                        $"Начался новый учебный год! Групп переведено на следующий курс: {count}.";
                    StartupInfoBar.IsOpen = true;
                }
            }
            catch
            {
                // Не мешаем запуску приложения
            }
        }

        private void Nav_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is not NavigationViewItem item || item.Tag is not string tag)
                return;

            Type? pageType = tag switch
            {
                "home" => typeof(HomePage),
                "employees" => typeof(EmployeesPage),
                "students" => typeof(StudentsPage),
                "medicines" => typeof(MedicinesPage),
                "appeals" => typeof(AppealsPage),
                _ => null,
            };

            if (pageType != null && ContentFrame.CurrentSourcePageType != pageType)
                ContentFrame.Navigate(pageType);
        }
    }
}
