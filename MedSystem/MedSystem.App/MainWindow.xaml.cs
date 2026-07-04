using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MedSystem.App.Pages;

namespace MedSystem.App
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Title = "Med System";
            ContentFrame.Navigate(typeof(HomePage));
        }

        private void Nav_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is not NavigationViewItem item || item.Tag is not string tag)
                return;

            Type? pageType = tag switch
            {
                "home" => typeof(HomePage),
                "employees" => typeof(EmployeesPage),
                _ => null,
            };

            if (pageType != null && ContentFrame.CurrentSourcePageType != pageType)
                ContentFrame.Navigate(pageType);
        }
    }
}
