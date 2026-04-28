using System.Collections.Generic;

namespace PerfumeWarehouseWPF
{
    public class Role
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
    }
}