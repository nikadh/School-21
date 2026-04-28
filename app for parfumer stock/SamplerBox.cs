using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PerfumeWarehouseWPF
{
    public class SamplerBox
    {
        [Key]
        public int BoxID { get; set; }
        public string BoxName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string ImageFileName { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual ICollection<SamplerBoxItem> Items { get; set; }
    }
}