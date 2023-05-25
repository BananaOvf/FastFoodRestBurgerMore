using Microsoft.Office.Interop.Excel;
using System.Windows.Forms.DataVisualization.Charting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ChartArea = System.Windows.Forms.DataVisualization.Charting.ChartArea;
using Series = System.Windows.Forms.DataVisualization.Charting.Series;
using Button = System.Windows.Controls.Button;

namespace FastFoodRest.View
{
    /// <summary>
    /// Логика взаимодействия для MakeOrderWindow.xaml
    /// </summary>
    public partial class MakeOrderWindow : System.Windows.Window
    {
        public List<Classes.ProductsInOrder> productsInOrder;

        public double AmountOfMoney { get; set; }
        public double OrderСost { get; set; }

        ChartArea area;	
        Series series;

        public MakeOrderWindow(double amountOfMoney)
        {
            InitializeComponent();

            categoriesList.ItemsSource = App.ListCat;

            this.AmountOfMoney = amountOfMoney;
            moneyOnCard.Text = $"Сумма на карте: {amountOfMoney}₽" ;

            this.DataContext = this;
            productsInOrder = new List<Classes.ProductsInOrder>();
            OrderСost = 0;
            orderAmount.Text = $"Сумма заказа: {OrderСost}₽";

            area = new ChartArea("Default");
            chartAmount.ChartAreas.Add(area);
            series = new Series("Amount");
            chartAmount.Series.Add(series);
            chartAmount.Series["Amount"].ChartArea = "Default";
            chartAmount.Series["Amount"].ChartType = SeriesChartType.Pie;
            ChartShow();
        }

        private void btn_exitMenuFromMakeOrder_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_arrangeOrder_Click(object sender, RoutedEventArgs e)
        {
            View.ArrangeOrderWindow arrangeOrderWindow = new View.ArrangeOrderWindow(productsInOrder, AmountOfMoney);
            arrangeOrderWindow.Owner = this;
            this.Hide();
            arrangeOrderWindow.ShowDialog();
            this.Close();
        }
        private void categoriesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            productsList.ItemsSource = App.DB.Product.Where(x => x.CategoryId == (int)categoriesList.SelectedValue).ToList();
        }

        private void btn_addProduct_Click(object sender, RoutedEventArgs e)
        {
            Classes.ProductsInOrder productInOrder = null;
            //Объект из списка (блюдо) в строке которой нажали кнопку
            var product = (sender as Button).DataContext as Entity.Product;
            string productName = product.ProductName;		//Название блюда
            int productCost = product.ProductPrice;			//Стоимость блюда
            if (OrderСost + productCost <= AmountOfMoney)  //Проверка под сумму на карте
            {
                OrderСost += productCost;			//Общая сумма в заказе
                orderAmount.Text = $"Сумма заказа: {OrderСost}₽";
                //Поиск этого блюда среди заказанных блюд
                int index = productsInOrder.FindIndex(x => x.Name == productName);
                if (index < 0)        //Такого товара еще в заказе нет
                {
                    //Создаем новый элемент списка
                    productInOrder = new Classes.ProductsInOrder();
                    productInOrder.Name = productName;
                    productInOrder.Price = productCost;
                    productInOrder.Amount = 1;   //Для нового
                    productInOrder.Total = productCost;	//Стоимость
                    productsInOrder.Add(productInOrder);	//добавляем в список
                }
                else         //Такой товар уже есть в заказе, поэтому увеличиваем его количество 
                {
                    productsInOrder[index].Amount++;
                    productsInOrder[index].Total =
                                                productsInOrder[index].Price * productsInOrder[index].Amount;
                }
                ChartShow();					//Метод отображения диаграммы
            }
            else
            {
                MessageBox.Show("У Вас уже не хватает денег");
            }

        }

        public void ChartShow()
        {
            chartAmount.Series["Amount"].Points.Clear();  //Подготовить серию для заполнению
            chartAmount.Series["Amount"].Points.AddXY(0, AmountOfMoney - OrderСost);//Осталось
            chartAmount.Series["Amount"].Points.AddXY(0, OrderСost);     //Сумма за заказ
        }

    }
}
