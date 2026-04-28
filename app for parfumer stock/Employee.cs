using System;

namespace PerfumeWarehouseWPF
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string LoginID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string PassportNumber { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public int RoleID { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual Role Role { get; set; }
    }
}