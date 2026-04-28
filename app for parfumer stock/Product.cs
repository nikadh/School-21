using System;
using System.Collections.Generic;

namespace PerfumeWarehouseWPF
{
    public class Product
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int BrandID { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Brand Brand { get; set; }
        public virtual ICollection<ProductVariant> Variants { get; set; }
        public virtual ICollection<ProductImage> Images { get; set; }
        public virtual ICollection<ProductNote> ProductNotes { get; set; }
    }
}