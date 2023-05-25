using FastFoodRest.Classes;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Media.TextFormatting;
using System.Windows.Shapes;
using Word = Microsoft.Office.Interop.Word;
using System.Drawing;
using Image = System.Drawing.Image;
using System.Drawing.Imaging;

namespace FastFoodRest.View
{
    /// <summary>
    /// Логика взаимодействия для ArrangeOrderWindow.xaml
    /// </summary>
    public partial class ArrangeOrderWindow : Window
    {
        List<ProductsInOrder> productsInOrder;
        double AmountOfMoney;

        /*Word.Application wordApp;           //Приложение Word
        Word.Document wordDoc;          //Документ Word
        Word.Table wordTable;               //Таблица 
        Word.InlineShape wordShape;         //Рисунок
        Word.Paragraph wordPar, tablePar;       //Абзацы документа и таблицы
        Word.Range wordRange, tablRange;		//Тест абзаца и таблицы*/


        public ArrangeOrderWindow(List<ProductsInOrder> productsInOrder, double amountOfMoney)
        {
            /*string base64 = Convert.ToBase64String(File.ReadAllBytes($@"{Environment.CurrentDirectory}\..\..\Resources\Logo.png"));
            var image = Image.FromStream(new MemoryStream(Convert.FromBase64String(base64)));

            image.Save("new.png", ImageFormat.Png);*/

            InitializeComponent();
            AmountOfMoney = amountOfMoney;
            tbMoneyOnCard.Text = $"Сумма на карте: {amountOfMoney}₽";

            this.productsInOrder = productsInOrder;
            dgOrder.ItemsSource = productsInOrder;

            double total = 0;
            foreach (var product in productsInOrder)
                total += product.Total;
            tbOrderCost.Text = $"Сумма заказа: {total}₽";
        }

        private void btn_exitMenuFromArrangeOrder_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show("Не могу освободить объект " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }


        private void btn_Order_Click(object sender, RoutedEventArgs e)
        {
            //Создание чека заказа
            //Объявление необходимых величин
            Word.Application wordApp;           //Сервер Word
            Word.Document wordDoc;          //Документ Word
            Word.Paragraph wordPar;         //Абзац документа
            Word.Range wordRange;           //Тест абзаца
            Word.Table wordTable;           //Таблица 
            Word.InlineShape wordShape;     //Рисунок
                                            //Создание сервера Word
            try
            {
                wordApp = new Word.Application();
                wordApp.Visible = false;
            }
            catch
            {
                MessageBox.Show("Товарный чек в Word создать не удалось");
                return;
            }
            //Создание документа Word
            wordDoc = wordApp.Documents.Add();      //Добавить новый пустой документ
            wordDoc.PageSetup.Orientation = Word.WdOrientation.wdOrientPortrait; // Книжная

            //**********Первый параграф – заголовок документа: логотип и дата
            wordPar = wordDoc.Paragraphs.Add();
            wordPar.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            wordRange = wordPar.Range;
            wordPar.set_Style("Заголовок 1");           //Стиль, взятый из Word
                                                        //Текст первого абзаца – заголовка документа
            wordRange.Text = "Дата заказа: " + DateTime.Now.ToLongDateString();
            //Добавить логитип-картинку
            wordShape = wordDoc.InlineShapes.AddPicture($@"{Environment.CurrentDirectory}\..\..\Resources\Logo.png",
                                                                       Type.Missing, Type.Missing, wordRange);
            wordShape.Width = 100;
            wordShape.Height = 100;

            //********Второй параграф - просто текст
            wordRange.InsertParagraphAfter();
            wordPar = wordDoc.Paragraphs.Add();
            wordPar.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            wordRange = wordPar.Range;
            wordRange.Font.Size = 16;
            wordRange.Font.Color = Word.WdColor.wdColorBlue;
            wordRange.Font.Name = "Arial";
            wordRange.Text = "Список заказанных блюд";

            //************Третий параграф - таблица
            wordRange = wordPar.Range;
            //Число строк в таблицы совпадает с число строк в таблице заказов формы
            wordTable = wordDoc.Tables.Add(wordRange, productsInOrder.Count + 1, 4);
            wordTable.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            wordTable.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleDouble;
            //Заголовков таблицы из ЭУ DataGrid
            Word.Range cellRange;
            for (int col = 1; col <= 4; col++)
            {
                cellRange = wordTable.Cell(1, col).Range;
                cellRange.Text = dgOrder.Columns[col - 1].Header.ToString();
            }
            //Можно выполнить заливку заголовка таблицы
            wordTable.Rows[1].Shading.ForegroundPatternColor = Word.WdColor.wdColorLightYellow;
            wordTable.Rows[1].Shading.BackgroundPatternColorIndex = Word.WdColorIndex.wdBlue;
            wordTable.Rows[1].Range.Bold = 1;
            wordTable.Rows[1].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            wordRange.Font.Size = 14;
            wordRange.Font.Color = Word.WdColor.wdColorBlue;
            wordRange.Font.Name = "Time New Roman";
            //wordRange.Font.Italic = 1;
            //Заполнение ячеек таблицы из списка заказов
            wordPar.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            wordPar.set_Style("Заголовок 2");               //Стиль, взятый из Word
            for (int row = 2; row <= productsInOrder.Count + 1; row++)
            {
                cellRange = wordTable.Cell(row, 1).Range;
                cellRange.Text = productsInOrder[row - 2].Name;
                wordRange.Font.Size = 14;
                wordRange.Font.Color = Word.WdColor.wdColorBlack;
                wordRange.Font.Name = "Time New Roman";
                //wordRange.Font.Italic = 0;
                cellRange = wordTable.Cell(row, 2).Range;
                cellRange.Text = productsInOrder[row - 2].Price.ToString();
                cellRange = wordTable.Cell(row, 3).Range;
                cellRange.Text = productsInOrder[row - 2].Amount.ToString();
                cellRange = wordTable.Cell(row, 4).Range;
                cellRange.Text = productsInOrder[row - 2].Total.ToString();
            }

            //*************Четвертый параграф - итоги
            wordRange.InsertParagraphAfter();
            wordPar = wordDoc.Paragraphs.Add();
            wordRange = wordPar.Range;
            wordPar.set_Style("Заголовок 1");               //Стиль, взятый из Word
            wordRange.Font.Color = Word.WdColor.wdColorRed;
            wordRange.Font.Size = 20;
            wordRange.Bold = 3;
            double total = 0;
            foreach (var product in productsInOrder)
                total += product.Total;
            wordRange.Text = "Стоимость заказа: " + total.ToString() + " рублей";
            wordApp.Visible = true;
            //Сохранение документа
            string fileName = Environment.CurrentDirectory + @"\Чек";
            wordDoc.SaveAs(fileName + ".docx");
            wordDoc.SaveAs(fileName + ".pdf", Word.WdExportFormat.wdExportFormatPDF);
            //Завершение работы с Word
            wordDoc.Close(true, null, null);                //Сначала закрыть документ
            wordApp.Quit();                     //Выход из Word
                                                //Вызвать свою подпрограмму убивания процессов
            releaseObject(wordPar);                 //Уничтожить абзац
            releaseObject(wordDoc);                 //Уничтожить документ
            releaseObject(wordApp);					//Удалить из Диспетчера задач

        }

        private void butInc_Click(object sender, RoutedEventArgs e)
        {
            var productType = (sender as Button).DataContext as Classes.ProductsInOrder;
            double total = double.Parse(tbOrderCost.Text.Split(' ')[2].Replace("₽", "").Replace(',', '.'));
            if (total + productType.Price <= AmountOfMoney)
            {
                productsInOrder.Remove(productType);
                productType.Amount++;
                productType.Total += productType.Price;
                productsInOrder.Add(productType);

                total += productType.Price;
                tbOrderCost.Text = $"Сумма заказа: {total}₽";

                dgOrder.Items.Refresh();
            }
            else MessageBox.Show("Недостаточно средств на карте");
        }

        private void butDec_Click(object sender, RoutedEventArgs e)
        {
            var productType = (sender as Button).DataContext as Classes.ProductsInOrder;
            if (productType.Amount > 1)
            {
                productsInOrder.Remove(productType);
                productType.Amount--;
                productType.Total -= productType.Price;
                productsInOrder.Add(productType);

                double total = double.Parse(tbOrderCost.Text.Split(' ')[2].Replace("₽", "").Replace(',', '.')) - productType.Price;
                tbOrderCost.Text = $"Сумма заказа: {total}₽";

                dgOrder.Items.Refresh();
            }
            else MessageBox.Show("Если хотите полностью убрать товар, нажмите на кнопку \"x\"");
        }

        private void butDel_Click(object sender, RoutedEventArgs e)
        {
            var productType = (sender as Button).DataContext as Classes.ProductsInOrder;
            productsInOrder.Remove(productType);

            double total = double.Parse(tbOrderCost.Text.Split(' ')[2].Replace("₽", "").Replace(',', '.')) - productType.Total;
            tbOrderCost.Text = $"Сумма заказа: {total}₽";

            dgOrder.Items.Refresh();
        }
    }
}
