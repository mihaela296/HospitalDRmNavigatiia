using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BCrypt.Net;


namespace HospitalD
{
    public partial class AddEditPatientPage : Page
    {
        private Patients _currentPatient = new Patients();

        public AddEditPatientPage(Patients selectedPatient = null)
        {
            InitializeComponent();

            if (selectedPatient != null)
            {
                _currentPatient = selectedPatient;
                TextBoxFullName.Text = _currentPatient.FullName;
                DatePickerBirthDate.SelectedDate = _currentPatient.BirthDate;
                ComboBoxGender.SelectedItem = _currentPatient.Gender;
                TextBoxPhone.Text = _currentPatient.Phone;
                TextBoxAddress.Text = _currentPatient.Address;
            }
        }

        private void SavePatient_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка заполнения полей
                if (string.IsNullOrWhiteSpace(TextBoxFullName.Text) ||
                    DatePickerBirthDate.SelectedDate == null ||
                    ComboBoxGender.SelectedItem == null ||
                    string.IsNullOrWhiteSpace(TextBoxPhone.Text) ||
                    string.IsNullOrWhiteSpace(TextBoxAddress.Text))
                {
                    MessageBox.Show("Заполните все обязательные поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Заполнение данных пациента
                _currentPatient.FullName = TextBoxFullName.Text;
                _currentPatient.BirthDate = DatePickerBirthDate.SelectedDate.Value;
                _currentPatient.Gender = (ComboBoxGender.SelectedItem as ComboBoxItem)?.Content.ToString();
                _currentPatient.Phone = TextBoxPhone.Text;
                _currentPatient.Address = TextBoxAddress.Text;

                // Получаем контекст базы данных
                var context = HospitalDRmEntities.GetContext();

                // Находим роль "Пациент" в таблице Roles по ID
                var patientRole = context.Roles.FirstOrDefault(r => r.ID_Role == 3);

                // Если роль найдена, присваиваем ID_Role пациенту
                if (patientRole != null)
                {
                    _currentPatient.ID_Role = patientRole.ID_Role;
                }
                else
                {
                    try
                    {
                        // Если роль не найдена, добавляем ее
                        var newRole = new Roles { ID_Role = 3, Name = "Пациент" };
                        context.Roles.Add(newRole);
                        context.SaveChanges();
                        _currentPatient.ID_Role = newRole.ID_Role;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при добавлении роли: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return; // Прерываем сохранение, если возникла ошибка при добавлении роли
                    }
                }

                // Добавление или обновление пациента в базе данных
                if (_currentPatient.ID_Patient == 0) // Новый пациент
                {
                    context.Patients.Add(_currentPatient);
                }
                context.SaveChanges();

                MessageBox.Show("Данные пациента успешно сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
