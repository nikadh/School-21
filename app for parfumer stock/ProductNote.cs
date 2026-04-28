namespace PerfumeWarehouseWPF
{
    public class ProductNote
    {
        public int ProductID { get; set; }
        public int NoteID { get; set; }

        public virtual Product Product { get; set; }
        public virtual Note Note { get; set; }
    }
}