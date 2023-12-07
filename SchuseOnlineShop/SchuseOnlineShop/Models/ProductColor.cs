﻿using SchuseOnlineShop.Models.Common;

namespace SchuseOnlineShop.Models
{
    public class ProductColor  :BaseEntity
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int ColorId { get; set; }
        public Color Color { get; set; }
    }
}