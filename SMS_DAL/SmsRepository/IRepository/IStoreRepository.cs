﻿using SMS_DAL.ViewModel;
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
        List<ItemPurchaseModel> SearchItemPurchase(DateTime fromDate, DateTime toDate, int orderId, int itemId);
        int AddItemPurchaseDetail(ItemPurchaseDetail itemPurchaseDetail);
        ItemPurchaseDetail GetItemPurchaseDetailById(int id);
        void UpdateItemPurchaseDetail(ItemPurchaseDetail itemPurchaseDetail);
        void DeleteItemPurchaseDetail(ItemPurchaseDetail itemPurchaseDetail);
        List<ItemPurchaseDetail> GetAllItemPurchaseDetailByItemPurchaseId(int itemPurchaseId);
        List<ItemPurchaseDetailModel> GetAllItemPurchaseDetailModelByItemPurchaseId(int itemPurchaseId);
        int GetPurchaseOrderId();
        int AddItemIssuance(ItemIssuance itemIssuance);
        ItemIssuance GetItemIssuanceByOrderId(int orderId);
        ItemIssuance GetItemIssuanceById(int Id);
        void UpdateItemIssuance(ItemIssuance itemIssuance);
        List<ItemIssuance> GetAllItemIssuance();
        void DeleteItemIssuance(ItemIssuance itemIssuance);
        List<ItemIssuanceModel> SearchItemIssuanc(DateTime fromDate, DateTime toDate, int orderId, int itemId);
        int AddItemIssuanceDetail(ItemIssuanceDetail itemIssuanceDetail);
        ItemIssuanceDetail GetItemIssuanceDetailById(int id);
        void UpdateItemIssuanceDetail(ItemIssuanceDetail itemIssuanceDetail);
        void DeleteItemIssuanceDetail(ItemIssuanceDetail itemIssuanceDetail);
        List<ItemIssuanceDetail> GetAllItemIssuanceDetailByItemIssuanceId(int IiemIssuanceId);
        List<ItemIssuanceDetailModel> GetAllItemIssuanceDetailModelByItemIssuanceId(int itemIssuanceId);
        int GetIssuanceOrderId();
        int AddIssuanceStockQuantity(IssuanceStockQuantity issuanceStockQuantity);
        void UpdateIssuanceStockQuantity(IssuanceStockQuantity issuanceStockQuantity);
        void DeleteIssuanceStockQuantity(IssuanceStockQuantity issuanceStockQuantity);
        List<IssuanceStockQuantity> GetIssuanceQuanatityList(int itemIssuanceDetailId);
        List<IssuanceStockQuantity> GetIssuanceReturnQuanatityList(int itemIssuanceDetailId);
        List<ItemPurchaseDetailModel> GetItemPurchaseWithQuantity(int itemId);
        List<ItemPurchaseDetailModel> GetItemPurchaseWithZeroQuantity(int itemId);
        int AddItemReturn(ItemReturn itemReturn);
        ItemReturn GetItemReturnByOrderId(int orderId);
        ItemReturn GetItemReturnById(int Id);
        void UpdateItemReturn(ItemReturn itemReturn);
        List<ItemReturn> GetAllItemItemReturn();
        void DeleteItemReturn(ItemReturn itemReturn);
        List<ItemReturnModel> SearchItemReturn(DateTime fromDate, DateTime toDate, int orderId, int itemId);
        int AddItemReturnDetail(ItemReturnDetail itemReturnDetail);
        ItemReturnDetail GetItemReturnDetailById(int id);
        void UpdateItemReturnDetail(ItemReturnDetail itemReturnDetail);
        void DeleteItemReturnDetail(ItemReturnDetail itemReturnDetail);
        List<ItemReturnDetail> GetAllItemReturnDetailByItemReturnId(int itemReturnId);
        List<ItemReturnDetailModel> GetAllItemReturnDetailModelByItemReturnId(int itemReturnId);
        int GetReturnOrderId();
    }
}
