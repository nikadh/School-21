using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PerfumeWarehouseWPF
{
    public class ProductVariant
    {
        [Key]
        public int VariantID { get; set; }
        public int ProductID { get; set; }
        public int VolumeML { get; set; }
        public decimal AlcoholPercent { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string SKU { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual Product Product { get; set; }
    }
}