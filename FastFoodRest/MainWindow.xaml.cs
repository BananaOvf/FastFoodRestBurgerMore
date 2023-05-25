using FastFoodRest.View;
using Microsoft.Office.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Policy;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Excel = Microsoft.Office.Interop.Excel;

namespace FastFoodRest
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        private double MoneyOnCard;
        public MainWindow()
        {
            InitializeComponent();
            Thread.Sleep(1000);

            Random random = new Random();
            MoneyOnCard = random.Next(999, 10000) + Math.Round(random.NextDouble(), 2);

            App.excelApp = new Excel.Application();

            App.DB = new Entity.DBFastFoodRestaurant();
            App.ListCat = App.DB.Category.ToList();
        }

        /// <summary>
        /// Пункт меню "Прайс-лист"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_priceList_Click(object sender, RoutedEventArgs e)
        {
            App.openWorkBook(App.fileMenuPath, true);
        }

        /// <summary>
        /// Пункт меню "Сделать заказ"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_makeOrder_Click(object sender, RoutedEventArgs e)
        {
            App.openWorkBook(App.fileMenuPath, false);
            if (App.excelBook == null) return;

            MakeOrderWindow makeOrderWindow = new MakeOrderWindow(MoneyOnCard);
            this.Hide();
            makeOrderWindow.ShowDialog();
            this.Show();
        }

        /// <summary>
        /// Пункт меню "Работа с каталогом"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_updateCatalog_Click(object sender, RoutedEventArgs e)
        {
            View.AuthorisationWindow authWindow = new View.AuthorisationWindow();
            this.Hide();
            authWindow.ShowDialog();
            this.Show();
        }

        /// <summary>
        /// Завершить работу приложения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_exit_Click(object sender, RoutedEventArgs e)
        {
            /*foreach (System.Windows.Window window in App.Current.Windows)
            {
                window.Close();
            }*/
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.excelApp.Quit();
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(App.excelApp);
            GC.Collect();
        }
    }
}
