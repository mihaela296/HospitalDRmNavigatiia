using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HospitalD
{
    /// <summary>
    /// Логика взаимодействия для DepartmentsPage.xaml
    /// </summary>
    public partial class PatientsPage : Page
    {
        public PatientsPage()
        {
            InitializeComponent();
            PatientsDataGrid.ItemsSource = HospitalDRmEntities.GetContext().Patients.ToList();
        }
        private void ButtonEdit_OnClick(object sender, RoutedEventArgs e)
        {
            var selectedPatient = (Patients)PatientsDataGrid.SelectedItem;

            if(selectedPatient!= null)
            {
                NavigationService.Navigate(new AddEditPatientPage(selectedPatient));
            }
            else
            {
                MessageBox.Show("выберите пациента для редактирования!","Ошибка",MessageBoxButton.OK,MessageBoxImage.Warning);
            }
        }

        private void ButtonAdd_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEditPatientPage(null));
        }
        private void ButtonDel_OnClick(object sender, RoutedEventArgs e)
        {
            var selectedPatient = (Patients)PatientsDataGrid.SelectedItem;

            if (selectedPatient != null)
            {
                var context = HospitalDRmEntities.GetContext();

                // Проверка наличия связанных данных в таблице Users
                var hasRelatedData = context.Users.Any(u => u.ID_User == selectedPatient.ID_Patient);

                if (hasRelatedData)
                {
                    MessageBox.Show("Невозможно удалить пациента, так как у него есть связанные данные!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Удаление пациента
                context.Patients.Remove(selectedPatient);
                context.SaveChanges();

                // Обновление DataGrid
                PatientsDataGrid.ItemsSource = context.Patients.ToList();

                MessageBox.Show("Пациент успешно удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Выберите пациента для удаления!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
