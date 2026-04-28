using System.Collections.Generic;

namespace PerfumeWarehouseWPF
{
    public class Brand
    {
        public int BrandID { get; set; }
        public string BrandName { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}