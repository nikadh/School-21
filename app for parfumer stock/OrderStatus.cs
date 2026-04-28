using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PerfumeWarehouseWPF
{
    public class OrderStatus
    {
        [Key]
        public int StatusID { get; set; }
        public string StatusName { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}