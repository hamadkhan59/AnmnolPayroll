using SMS_DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.SmsRepository.IRepository
{
    public interface IStoreRepository : IDisposable
    {
        int AddVendor(Vendor vendor);
        Vendor GetVendorByName(string vendorName);
        Vendor GetVendorByNameAndId(string vendorName, int id);
        void UpdateVendor(Vendor vendor);
        Vendor GetVendorById(int vendorId);
        void DeleteVendor(Vendor vendor);
        List<Vendor> GetAllVendors();
        List<Vendor> GetAllVendorsData();
        int AddItems(Item item);
        Item GetItemByName(string itemName);
        Item GetItemByNameAndId(string itemName, int id);
        void UpdateItem(Item item);
        Item GetItemById(int itemId);
        void DeleteItem(Item item);
        List<Item> GetAllItems();
        List<Item> GetAllItemsData();
        List<ItemModel> GetAllItemsModel();
        List<ItemUnit> GetAllItemUnits();
        int AddItemPurchase(ItemPurchase itemPurchase);
        ItemPurchase GetItemPurchaseByOrderId(int orderId);
        ItemPurchase GetItemPurchaseById(int Id);
        void UpdateItemPurchase(ItemPurchase itemPurchase);
        List<ItemPurchase> GetAllItemPurchase();
        void DeleteItemPurchase(ItemPurchase itemPurchase);
        int AddItemPurchaseDetail(ItemPurchaseDetail itemPurchaseDetail);
        ItemPurchaseDetail GetItemPurchaseDetailById(int id);
        void UpdateItemPurchaseDetail(ItemPurchaseDetail itemPurchaseDetail);
        void DeleteItemPurchaseDetail(ItemPurchaseDetail itemPurchaseDetail);
        List<ItemPurchaseDetail> GetAllItemPurchaseDetailByItemPurchaseId(int itemPurchaseId);
        List<ItemPurchaseDetailModel> GetAllItemPurchaseDetailModelByItemPurchaseId(int itemPurchaseId);
        int GetPurchaseOrderId();
    }
}
