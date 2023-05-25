using FastFoodRest.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Excel = Microsoft.Office.Interop.Excel;

namespace FastFoodRest
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Entity.DBFastFoodRestaurant DB;
        public static List<Entity.Category> ListCat;

        public static string adminLogin = "admin";
        public static string adminPassword = "admin";

        public static Excel.Application excelApp;
        public static Excel.Workbook    excelBook;
        public static Excel.Worksheet   excelSheet;
        public static Excel.Range       excelCells;

        public static string fileMenuPath = $@"{Environment.CurrentDirectory}\Menu.xlsx";

        public static List<string> CreateListCategories()
        {
            List<string> list = new List<string>();
            foreach (Excel.Worksheet worksheet in excelBook.Worksheets)
                list.Add(worksheet.Name);
            return list;
        }

        public static List<Classes.Product> CreateListProducts(string category)
        {
            double calcDiscount(int price, int discount) => 
                Math.Round((100d - discount) * price / 100, 2);

            List<Classes.Product> products = new List<Classes.Product>();
            excelCells = excelBook.Sheets[category].Cells;

            Classes.Product product;
            for (int row = 1; excelCells[row, 1].value2 != null; row++)
            {
                product = new Classes.Product();


                product.Name = excelCells.Cells[row, 1].value2;

                product.Price = (int)excelCells.Cells[row, 2].value2;

                product.Calories = (int)excelCells.Cells[row, 3].value2;

                product.Weight = (int)excelCells.Cells[row, 4].value2;

                product.Discount = (int)excelCells.Cells[row, 5].value2;

                product.DiscountPrice = calcDiscount(product.Price, product.Discount);

                string Base64 = "";
                for (int column = 6; excelCells[row, column].value2 != null; column++)
                    Base64 += excelCells[row, column].value2;
                try
                {
                    product.Photo = Base64 != "" ? BitmapFrame.Create(new MemoryStream(Convert.FromBase64String(Base64))) : 
                        BitmapFrame.Create(new Uri($@"{Environment.CurrentDirectory}\..\..\Resources\cross.png"));
                }
                catch (Exception)
                {
                    MessageBox.Show($"Не удалось привести изображение товара \"{product.Name}\" к типу 'int'\nДальнейшая работа может привести к ошибкам");
                    return null;
                }
                products.Add(product);
            }
            return products;
        }

        public static void openWorkBook(string path, bool visibility)
        {
            if (File.Exists(path))
            {
                try
                {
                    excelBook = excelApp.Workbooks.Open(path);
                    excelApp.Visible = visibility;
                }
                catch (Exception) { MessageBox.Show("Кажется что-то не так с Microsoft Excel. Повторите попытку позже"); }
            }
            else
            { MessageBox.Show("Файл с меню отсутствует"); }
        }
    }
}
