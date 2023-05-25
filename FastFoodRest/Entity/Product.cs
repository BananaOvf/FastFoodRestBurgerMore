//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FastFoodRest.Entity
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows.Media.Imaging;

    public partial class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int ProductPrice { get; set; }
        public int ProductCalory { get; set; }
        public int ProductWeight { get; set; }
        public int ProductDiscount { get; set; }
        public int CategoryId { get; set; }
        public string ProductPhotoBase64 { get; set; }
        public BitmapFrame ProductPhoto { get => ProductPhotoBase64 != "" ? BitmapFrame.Create(new MemoryStream(Convert.FromBase64String(ProductPhotoBase64))) :
                        BitmapFrame.Create(new Uri($@"{Environment.CurrentDirectory}\..\..\Resources\cross.png")); set { value = null; } }

        public double ProductDiscountPrice { get => Math.Round((100d - ProductDiscount) * ProductPrice / 100, 2); set { value = 0; } }

        public virtual Category Category { get; set; }
    }
}
