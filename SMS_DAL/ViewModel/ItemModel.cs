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
        public int VendorId { get; set; }
        public Nullable<int> ItemPurchaseId { get; set; }
        public Nullable<int> ItemId { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        public Nullable<decimal> IssuanceQuantity { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<decimal> Total { get; set; }
        public string ItemName { get; set; }
        public string VendorName { get; set; }
        public int OrderId { get; set; }
    }

    public partial class ItemPurchaseModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public Nullable<int> Amount { get; set; }
        public Nullable<System.DateTime> PurchaseDate { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
    }

    public partial class ItemIssuanceModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int IssuerId { get; set; }
        public string IssuerName { get; set; }
        public Nullable<int> Amount { get; set; }
        public Nullable<System.DateTime> IssuanceDate { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
    }

    public partial class ItemReturnModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Nullable<int> Amount { get; set; }
        public Nullable<System.DateTime> ReturnDate { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
    }

    public partial class ItemIssuanceDetailModel
    {
        public int Id { get; set; }
        public int IssuerId { get; set; }
        public Nullable<int> ItemIssuanceId { get; set; }
        public Nullable<int> ItemId { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<decimal> Total { get; set; }
        public string ItemName { get; set; }
        public string UnitName { get; set; }
        public string IssuerName { get; set; }
        public int OrderId { get; set; }
    }

    public partial class ItemReturnDetailModel
    {
        public int Id { get; set; }
        public Nullable<int> ItemReturnId { get; set; }
        public Nullable<int> ItemId { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<decimal> Total { get; set; }
        public string ItemName { get; set; }
        public string UnitName { get; set; }
        public int OrderId { get; set; }
    }


    public partial class ItemVendorModel
    {
        public int Id { get; set; }
        public Nullable<int> ItemId { get; set; }
        public Nullable<int> VendorId { get; set; }
        public string ItemName { get; set; }
        public string VendorName { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
    }
    
}
