using FastFoodRest.Classes;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Excel = Microsoft.Office.Interop.Excel;
using Image = System.Drawing.Image;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using FormsDialogResult = System.Windows.Forms.DialogResult;
using MessageBox = System.Windows.MessageBox;
using Microsoft.Office.Interop.Word;
using Window = System.Windows.Window;

namespace FastFoodRest.View
{
    /// <summary>
    /// Логика взаимодействия для WorkWithCatalogWindow.xaml
    /// </summary>
    public partial class WorkWithCatalogWindow : Window
    {
        string currentCategory = null;
        int currentProductRow = 0;

        public WorkWithCatalogWindow()
        {
            InitializeComponent();
            categoriesListAdmin.ItemsSource = App.CreateListCategories();
        }

        private void btn_exitMenuFromCatalog_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void categoriesListAdmin_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (categoriesListAdmin.SelectedItem == null) return;

            currentCategory = categoriesListAdmin.SelectedItem.ToString();
            tB_Category.Text = currentCategory;

            productsListAdmin.ItemsSource = App.CreateListProducts(currentCategory);
        }

        private void btn_DeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            if (currentCategory == null)
            {
                MessageBox.Show("Вы не выбрали категорию");
                return;
            }

            App.excelApp.DisplayAlerts = false;
            App.excelBook.Sheets[currentCategory].Delete();
            App.excelApp.DisplayAlerts = true;

            currentCategory = null;
            currentProductRow = 0;
            productsListAdmin.ItemsSource = null;

            App.excelBook.Save();
            categoriesListAdmin.ItemsSource = App.CreateListCategories();
            img_Product.Source = null;
        }

        private void btn_AlterCategoryName_Click(object sender, RoutedEventArgs e)
        {
            if (currentCategory == null)
            {
                MessageBox.Show("Вы не выбрали категорию");
                return;
            }
            if (tB_Category.Text == "")
            {
                MessageBox.Show("Поле должно быть заполнено!");
                return;
            }

            Excel.Worksheet sheet = App.excelBook.Sheets[currentCategory];
            try
            {
                sheet.Name = tB_Category.Text;
                currentCategory = tB_Category.Text;
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                MessageBox.Show("Такая категория уже существует. Придумайте другое название и повторите попытку");
            }

            App.excelBook.Save();
            categoriesListAdmin.ItemsSource = App.CreateListCategories();
        }

        private void btn_AddCategory_Click(object sender, RoutedEventArgs e)
        {
            if (tB_Category.Text == "")
            {
                MessageBox.Show("Поле должно быть заполнено!");
                return;
            }
            string categoryName = tB_Category.Text;

            var newWorksheet = (Excel.Worksheet)App.excelBook.Sheets.Add();
            try
            {
                newWorksheet.Name = categoryName;
                currentCategory = categoryName;
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                newWorksheet.Delete();
                MessageBox.Show("Такая категория уже существует");
                return;
            }

            App.excelBook.Save();
            categoriesListAdmin.ItemsSource = App.CreateListCategories();
        }

        private void productsListAdmin_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (productsListAdmin.SelectedItem == null) return;

            if (currentCategory == null)
            {
                MessageBox.Show("Вы не выбрали категорию");
                return;
            }
            List<Product> products = App.CreateListProducts(currentCategory);

            for(int i = 0; i < products.Count; i++)
                if (products[i].Name.Equals(((Product)productsListAdmin.SelectedItem).Name))
                {
                    currentProductRow = i + 1;
                    break;
                }

            tB_ProductName.Text = products[currentProductRow - 1].Name;
            tB_ProductPrice.Text = products[currentProductRow - 1].Price.ToString();
            tB_ProductCalories.Text = products[currentProductRow - 1].Calories.ToString();
            tB_ProductWeight.Text = products[currentProductRow - 1].Weight.ToString();
            tB_ProductDiscount.Text = products[currentProductRow - 1].Discount.ToString();

            img_Product.Source = products[currentProductRow - 1].Photo;
        }

        private void btn_AddProduct_Click(object sender, RoutedEventArgs e)
        {
            if(currentCategory == null)
            {
                MessageBox.Show("Вы не выбрали категорию");
                return;
            }
            Excel.Worksheet sheet = App.excelBook.Sheets[currentCategory];

            int row = 1;
            for(; sheet.Cells[row, 1].value2 != null; row++)
                if(sheet.Cells[row, 1].value2 == tB_ProductName.Text)
                {
                    MessageBox.Show("Такой продукт уже существует");
                    return;
                }

            if (tB_ProductName.Text == "" || tB_ProductPrice.Text == "" ||
                tB_ProductDiscount.Text == "" || tB_ProductCalories.Text == "" || tB_ProductWeight.Text == "")
            {
                MessageBox.Show("Все поля должны быть заполнены!");
                return;
            }


            if (!int.TryParse(tB_ProductPrice.Text, out int price))
            {
                MessageBox.Show("Не удалось преобразовать данные из поля price из типа string в тип int");
                return;
            }
            if (!int.TryParse(tB_ProductWeight.Text, out int weight))
            {
                MessageBox.Show("Не удалось преобразовать данные из поля weight из типа string в тип int");
                return;
            }
            if (!int.TryParse(tB_ProductCalories.Text, out int calories))
            {
                MessageBox.Show("Не удалось преобразовать данные из поля calories из типа string в тип int");
                return;
            }
            if (!int.TryParse(tB_ProductDiscount.Text, out int discount))
            {
                MessageBox.Show("Не удалось преобразовать данные из поля discount из типа string в тип int");
                return;
            }

            if (price < 0 || weight < 0 || calories < 0 || discount < 0)
            {
                MessageBox.Show("Значения не могут быть меньше нуля");
                return;
            }
            if (discount > 100)
            {
                MessageBox.Show("Проценты не могут быть больше 100");
                return;
            }

            sheet.Cells[row, 1].value2 = tB_ProductName.Text;
            sheet.Cells[row, 2].value2 = price;
            sheet.Cells[row, 3].value2 = weight;
            sheet.Cells[row, 4].value2 = calories;
            sheet.Cells[row, 5].value2 = discount > 100 ? 100 : discount;
            App.excelBook.Save();

            currentProductRow = row;
            List<Product> products = App.CreateListProducts(currentCategory);
            productsListAdmin.ItemsSource = products;
        }

        private void btn_AlterProductName_Click(object sender, RoutedEventArgs e)
        {
            if (currentCategory == null)
            {
                MessageBox.Show("Вы не выбрали категорию");
                return;
            }
            if (currentProductRow == 0)
            {
                MessageBox.Show("Вы не выбрали продукт");
                return;
            }
            if(tB_ProductName.Text == "" || tB_ProductPrice.Text == "" || 
                tB_ProductDiscount.Text == "" || tB_ProductCalories.Text == "" || tB_ProductWeight.Text == "")
            {
                MessageBox.Show("Все поля должны быть заполнены!");
                return;
            }

            Excel.Worksheet sheet = App.excelBook.Sheets[currentCategory];
            for (int row = 1; sheet.Cells[row, 1].value2 != null; row++)
                if (sheet.Cells[row, 1].value2 == tB_ProductName.Text && row != currentProductRow)
                {
                    MessageBox.Show("Такой продукт уже существует");
                    return;
                }

            if (!int.TryParse(tB_ProductPrice.Text, out int price))
            {
                MessageBox.Show("Не удалось преобразовать данные из поля price из типа string в тип int");
                return;
            }
            if (!int.TryParse(tB_ProductWeight.Text, out int weight))
            {
                MessageBox.Show("Не удалось преобразовать данные из поля weight из типа string в тип int");
                return;
            }
            if (!int.TryParse(tB_ProductCalories.Text, out int calories))
            {
                MessageBox.Show("Не удалось преобразовать данные из поля calories из типа string в тип int");
                return;
            }
            if (!int.TryParse(tB_ProductDiscount.Text, out int discount))
            {
                MessageBox.Show("Не удалось преобразовать данные из поля discount из типа string в тип int");
                return;
            }

            if(price < 0 || weight < 0 || calories < 0 || discount < 0)
            {
                MessageBox.Show("Значения не могут быть меньше нуля");
                return;
            }
            if(discount > 100)
            {
                MessageBox.Show("Проценты не могут быть больше 100");
                return;
            }

            sheet.Cells[currentProductRow, 1].value2 = tB_ProductName.Text;
            sheet.Cells[currentProductRow, 2].value2 = price;
            sheet.Cells[currentProductRow, 4].value2 = weight;
            sheet.Cells[currentProductRow, 3].value2 = calories;
            sheet.Cells[currentProductRow, 5].value2 = discount;
            App.excelBook.Save();

            List<Product> products = App.CreateListProducts(currentCategory);
            productsListAdmin.ItemsSource = products;
        }

        private void btn_DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (currentCategory == null)
            {
                MessageBox.Show("Вы не выбрали категорию");
                return;
            }
            if(currentProductRow == 0)
            {
                MessageBox.Show("Вы не выбрали продукт");
                return;
            }

            Excel.Worksheet sheet = App.excelBook.Sheets[currentCategory];
            for (int col = 1; sheet.Cells[currentProductRow, col].value2 != null; col++)
                sheet.Cells[currentProductRow, col] = null;
            App.excelBook.Save();

            currentProductRow = 0;
            List<Product> products = App.CreateListProducts(currentCategory);
            productsListAdmin.ItemsSource = products;
            img_Product.Source = null;
        }

        private void btn_AlterProductImage_Click(object sender, RoutedEventArgs e)
        {
            if (currentCategory == null)
            {
                MessageBox.Show("Вы не выбрали категорию");
                return;
            }
            if(currentProductRow == 0)
            {
                MessageBox.Show("Вы не выбрали продукт");
                return;
            }

            string base64 = null;
            OpenFileDialog openFileDialog;
            using (openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Выберите картинку";
                openFileDialog.RestoreDirectory = true;
                openFileDialog.FileName = "";
                openFileDialog.Filter = "JPG|*.jpg|PNG|*.png|все|*.*";
                openFileDialog.FilterIndex = 2;

                if (openFileDialog.ShowDialog().Equals(FormsDialogResult.OK))
                    base64 = Convert.ToBase64String(File.ReadAllBytes(openFileDialog.FileName));
                else return;
            }

            int numOfColumns = (int)Math.Ceiling(base64.Length / 32767d);
            for (int col = 6; col < 6 + numOfColumns; col++)
            {
                App.excelCells.Cells[currentProductRow, col].value2 = base64.Length > 32767 ? base64.Remove(32767) : base64;
                base64 = base64.Length > 32767 ? base64.Remove(0, 32767) : base64;
            }
            for (int col = 6 + numOfColumns; App.excelCells.Cells[currentProductRow, col].value2 != null; col++)
                App.excelCells.Cells[currentProductRow, col].value2 = null;
            App.excelBook.Save();

            List<Product> products = App.CreateListProducts(currentCategory);
            img_Product.Source = products[currentProductRow - 1].Photo;

            MessageBox.Show("Картинка обновлена");
        }

        private void btn_DeleteImage_Click(object sender, RoutedEventArgs e)
        {
            if (currentCategory == null)
            {
                MessageBox.Show("Вы не выбрали категорию");
                return;
            }
            if (currentProductRow == 0)
            {
                MessageBox.Show("Вы не выбрали продукт");
                return;
            }

            for (int col = 6; App.excelCells.Cells[currentProductRow, col].value2 != null; col++)
                App.excelCells.Cells[currentProductRow, col].value2 = null;
            App.excelBook.Save();

            List<Product> products = App.CreateListProducts(currentCategory);
            img_Product.Source = products[currentProductRow - 1].Photo;

            MessageBox.Show("Картинка удалена");
        }
    }
}
