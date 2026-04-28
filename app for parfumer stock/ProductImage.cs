using System.ComponentModel.DataAnnotations;

namespace PerfumeWarehouseWPF
{
    public class ProductImage
    {
        [Key]
        public int ImageID { get; set; }
        public int ProductID { get; set; }
        public string ImageFileName { get; set; }
        public bool IsMain { get; set; }

        public virtual Product Product { get; set; }
    }
}