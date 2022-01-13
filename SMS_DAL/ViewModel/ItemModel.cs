using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.ViewModel
{
    public class ItemModel
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public Nullable<int> UnitId { get; set; }
        public string UnitName { get; set; }
        public string ItemDescription { get; set; }
    }

    public partial class ItemPurchaseDetailModel
    {
        public int Id { get; set; }
        public Nullable<int> ItemPurchaseId { get; set; }
        public Nullable<int> ItemId { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<decimal> Total { get; set; }
        public string ItemName { get; set; }
        public int OrderId { get; set; }
    }
}
