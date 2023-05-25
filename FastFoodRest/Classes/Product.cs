using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace FastFoodRest.Classes
{
    public class Product
    {
        public BitmapFrame Photo { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public int Discount { get; set; }
        public double DiscountPrice { get; set; }
        public int Weight { get; set; }
        public int Calories { get; set; }
    }
}
