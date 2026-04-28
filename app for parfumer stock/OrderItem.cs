using System.ComponentModel.DataAnnotations;

namespace PerfumeWarehouseWPF
{
    public class OrderItem
    {
        [Key]
        public int OrderItemID { get; set; }
        public int OrderID { get; set; }
        public int VariantID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public virtual Order Order { get; set; }
        public virtual ProductVariant Variant { get; set; }
    }
}