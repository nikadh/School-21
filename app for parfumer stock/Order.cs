using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PerfumeWarehouseWPF
{
    public class Order
    {
        [Key]
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public DateTime? OrderDate { get; set; }
        public int StatusID { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public int? EmployeeID { get; set; }
        public string Comment { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual OrderStatus Status { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}