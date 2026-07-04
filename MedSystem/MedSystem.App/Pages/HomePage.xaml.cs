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
            EmployeesCount.Text = EmployeeRepository.GetAll().Count.ToString();
            StudentsCount.Text = StudentRepository.GetAll().Count.ToString();
            MedicinesCount.Text = MedicineRepository.GetAll().Count.ToString();
            AppealsCount.Text = AppealRepository.GetAll().Count.ToString();
            DbPathText.Text = Db.DbPath;
        }
    }
}
