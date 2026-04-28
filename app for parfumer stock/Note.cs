using System.Collections.Generic;

namespace PerfumeWarehouseWPF
{
    public class Note
    {
        public int NoteID { get; set; }
        public string NoteName { get; set; }
        public string NoteType { get; set; }
        public virtual ICollection<ProductNote> ProductNotes { get; set; }
    }
}