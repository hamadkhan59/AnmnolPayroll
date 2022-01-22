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
        public Nullable<decimal> IssuanceQuantity { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<decimal> Total { get; set; }
        public string ItemName { get; set; }
        public int OrderId { get; set; }
    }

    public partial class ItemPurchaseModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Nullable<int> Amount { get; set; }
        public Nullable<System.DateTime> PurchaseDate { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
    }

    public partial class ItemIssuanceModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Nullable<int> Amount { get; set; }
        public Nullable<System.DateTime> IssuanceDate { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
    }

    public partial class ItemIssuanceDetailModel
    {
        public int Id { get; set; }
        public Nullable<int> ItemIssuanceId { get; set; }
        public Nullable<int> ItemId { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<decimal> Total { get; set; }
        public string ItemName { get; set; }
        public int OrderId { get; set; }
    }
}
