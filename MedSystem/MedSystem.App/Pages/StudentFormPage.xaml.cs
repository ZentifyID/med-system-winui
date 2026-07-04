using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MedSystem.Core.Models;
using MedSystem.Core.Validation;
using MedSystem.Data.Repositories;

namespace MedSystem.App.Pages
{
    /// <summary>Форма добавления/редактирования студента.
    /// Параметр навигации: long id студента; 0 — новый.</summary>
    public sealed partial class StudentFormPage : Page
    {
        private long _studentId;
        private List<Group> _groups = new();

        public StudentFormPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _studentId = e.Parameter is long id ? id : 0;

            _groups = GroupRepository.GetAll();
            GroupBox.ItemsSource = _groups;

            if (_studentId > 0)
            {
                TitleText.Text = "Редактирование студента";
                var s = StudentRepository.GetById(_studentId);
                if (s != null)
                    FillForm(s);
            }
        }

        private void FillForm(Student s)
        {
            LastNameBox.Text = s.LastName;
            FirstNameBox.Text = s.FirstName;
            MiddleNameBox.Text = s.MiddleName;
            BirthDateBox.Text = s.BirthDate;
            GroupBox.SelectedValue = s.GroupId;
            OmsBox.Text = s.Oms;
            AddressBox.Text = s.Address;
            SanminimumBox.Text = s.SanminimumDate;
            MedicalExamBox.Text = s.MedicalExamDate;
            FluorographyBox.Text = s.FluorographyDate;
        }

        private Student CollectForm() => new()
        {
            Id = _studentId,
            GroupId = GroupBox.SelectedValue is long gid ? gid : 0,
            LastName = LastNameBox.Text.Trim(),
            FirstName = FirstNameBox.Text.Trim(),
            MiddleName = MiddleNameBox.Text.Trim(),
            BirthDate = BirthDateBox.Text.Trim(),
            Oms = OmsBox.Text.Trim(),
            Address = AddressBox.Text.Trim(),
            SanminimumDate = SanminimumBox.Text.Trim(),
            MedicalExamDate = MedicalExamBox.Text.Trim(),
            FluorographyDate = FluorographyBox.Text.Trim(),
        };

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var student = CollectForm();

            var errors = Validators.ValidateStudent(student);
            if (errors.Count > 0)
            {
                await ShowErrorsAsync(errors);
                return;
            }

            try
            {
                if (_studentId > 0)
                    StudentRepository.Update(student);
                else
                    StudentRepository.Insert(student);
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

        private async Task ShowErrorsAsync(List<string> errors)
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
