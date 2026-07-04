using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MedSystem.Data;
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
            DbPathText.Text = Db.DbPath;
        }
    }
}
