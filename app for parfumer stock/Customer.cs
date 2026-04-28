using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PerfumeWarehouseWPF
{
    public class Customer
    {
        [Key]
        public int CustomerID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public int? LoyaltyPoints { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}