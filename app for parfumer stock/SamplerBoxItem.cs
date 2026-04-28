using System.ComponentModel.DataAnnotations;

namespace PerfumeWarehouseWPF
{
    public class SamplerBoxItem
    {
        [Key]
        public int BoxItemID { get; set; }
        public int BoxID { get; set; }
        public int ProductID { get; set; }
        public decimal VolumeML { get; set; }
        public int Quantity { get; set; }

        public virtual SamplerBox Box { get; set; }
        public virtual Product Product { get; set; }
    }
}