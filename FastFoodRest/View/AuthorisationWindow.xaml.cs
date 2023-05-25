using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace FastFoodRest.View
{
    /// <summary>
    /// Логика взаимодействия для AuthorisationWindow.xaml
    /// </summary>
    public partial class AuthorisationWindow : Window
    {
        public AuthorisationWindow()
        {
            InitializeComponent();
        }

        private void btn_exitMenu_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_Enter_Click(object sender, RoutedEventArgs e)
        {
            login.Text = "admin"; password.Password = "admin";
            if (login.Text.Equals(App.adminLogin))
            {
                loginValid.Source = BitmapFrame.Create(new Uri("pack://application:,,,/Resources/checkMark.png"));
                if (password.Password.Equals(App.adminPassword))
                {
                    passwordValid.Source = BitmapFrame.Create(new Uri("pack://application:,,,/Resources/checkMark.png"));
                    this.Dispatcher.Invoke(() => this, DispatcherPriority.ApplicationIdle);
                    Thread.Sleep(350);

                    App.openWorkBook(App.fileMenuPath, false);
                    if (App.excelBook == null) return;
                    View.WorkWithCatalogWindow workWithCatalogWindow = new View.WorkWithCatalogWindow();
                    this.Hide();
                    workWithCatalogWindow.ShowDialog();
                    this.Close();
                }
            }
            else loginValid.Source = BitmapFrame.Create(new Uri("pack://application:,,,/Resources/cross.png"));
        }
    }
}
