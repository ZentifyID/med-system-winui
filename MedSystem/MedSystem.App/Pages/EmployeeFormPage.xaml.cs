using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MedSystem.Core.Models;
using MedSystem.Core.Validation;
using MedSystem.Data.Repositories;

namespace MedSystem.App.Pages
{
    /// <summary>Форма добавления/редактирования сотрудника.
    /// Параметр навигации: long id сотрудника; 0 — новый.</summary>
    public sealed partial class EmployeeFormPage : Page
    {
        private long _employeeId;

        public EmployeeFormPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _employeeId = e.Parameter is long id ? id : 0;

            if (_employeeId > 0)
            {
                TitleText.Text = "Редактирование сотрудника";
                var emp = EmployeeRepository.GetById(_employeeId);
                if (emp != null)
                    FillForm(emp);
            }
        }

        private void FillForm(Employee e)
        {
            LastNameBox.Text = e.LastName;
            FirstNameBox.Text = e.FirstName;
            MiddleNameBox.Text = e.MiddleName;
            BirthDateBox.Text = e.BirthDate;
            AffiliationBox.SelectedIndex = e.Affiliation == "внешний" ? 1 : 0;
            OmsBox.Text = e.Oms;
            AddressBox.Text = e.Address;
            PassportSeriesBox.Text = e.PassportSeries;
            PassportNumberBox.Text = e.PassportNumber;
            PassportIssueDateBox.Text = e.PassportIssueDate;
            PassportDeptCodeBox.Text = e.PassportDepartmentCode;
            PassportIssuedByBox.Text = e.PassportIssuedBy;
            SanminimumBox.Text = e.SanminimumDate;
            MedicalExamBox.Text = e.MedicalExamDate;
            FluorographyBox.Text = e.FluorographyDate;
        }

        private Employee CollectForm() => new()
        {
            Id = _employeeId,
            LastName = LastNameBox.Text.Trim(),
            FirstName = FirstNameBox.Text.Trim(),
            MiddleName = MiddleNameBox.Text.Trim(),
            BirthDate = BirthDateBox.Text.Trim(),
            Affiliation = AffiliationBox.SelectedIndex == 1 ? "внешний" : "основной",
            Oms = OmsBox.Text.Trim(),
            Address = AddressBox.Text.Trim(),
            PassportSeries = PassportSeriesBox.Text.Trim(),
            PassportNumber = PassportNumberBox.Text.Trim(),
            PassportIssueDate = PassportIssueDateBox.Text.Trim(),
            PassportDepartmentCode = PassportDeptCodeBox.Text.Trim(),
            PassportIssuedBy = PassportIssuedByBox.Text.Trim(),
            SanminimumDate = SanminimumBox.Text.Trim(),
            MedicalExamDate = MedicalExamBox.Text.Trim(),
            FluorographyDate = FluorographyBox.Text.Trim(),
        };

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var employee = CollectForm();

            var errors = Validators.ValidateEmployee(employee);
            if (errors.Count > 0)
            {
                await ShowErrorsAsync(errors);
                return;
            }

            try
            {
                if (_employeeId > 0)
                    EmployeeRepository.Update(employee);
                else
                    EmployeeRepository.Insert(employee);
            }
            catch (Exception ex)
            {
                await ShowErrorsAsync(new() { $"Ошибка базы данных: {ex.Message}" });
                return;
            }

            if (Frame.CanGoBack)
                Frame.GoBack();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
        }

        private async Task ShowErrorsAsync(System.Collections.Generic.List<string> errors)
        {
            var dialog = new ContentDialog
            {
                Title = "Проверьте данные",
                Content = string.Join("\n", errors),
                CloseButtonText = "Понятно",
                XamlRoot = XamlRoot,
            };
            await dialog.ShowAsync();
        }
    }
}
