using System.Collections.Generic;

namespace Mikos.XK.Fiscal.Model
{
    public class VoidCheckItems
    {
        public List<VoidCheckItem> Items { get; set; }
        public decimal Total { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalTendered { get; set; }
        public string TenderName { get; set; }
    }

    public class VoidCheckItem
    {
        public long ObjectNumber { get; set; }
        public decimal Quantity { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
