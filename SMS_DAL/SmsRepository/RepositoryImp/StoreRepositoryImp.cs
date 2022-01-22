using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.SmsRepository.RepositoryImp
{
    public class StoreRepositoryImp : IStoreRepository
    {
        private SC_WEBEntities2 dbContext1;

        public StoreRepositoryImp(SC_WEBEntities2 context)   
        {  
            dbContext1 = context;  
        }

        SC_WEBEntities2 dbContext
        {
            get
            {
                if (dbContext1 == null || this.disposed == true)
                    dbContext1 = new SC_WEBEntities2();
                this.disposed = false;
                return dbContext1;
            }
        }

        public int AddVendor(Vendor vendor)
        {
            int result = -1;
            if (vendor != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Vendors.Add(vendor);
                dbContext.SaveChanges();
                result = vendor.Id;
            }

            return result;
        }

        public Vendor GetVendorByName(string vendorName)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Vendors.Where(x => x.Name == vendorName).FirstOrDefault();
        }

        public Vendor GetVendorByNameAndId(string vendorName, int id)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Vendors.Where(x => x.Name == vendorName && x.Id != id).FirstOrDefault();
        }

        public void UpdateVendor(Vendor vendor)
        {
            if (vendor != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Entry(vendor).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public Vendor GetVendorById(int vendorId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Vendors.Where(x => x.Id == vendorId).FirstOrDefault();
        }

        public void DeleteVendor(Vendor vendor)
        {
            if (vendor != null)
            {
                dbContext.Vendors.Remove(vendor);
                dbContext.SaveChanges();
            }
        }

        public List<Vendor> GetAllVendors()
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Vendors.ToList();
        }

        public List<Vendor> GetAllVendorsData()
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Vendors.Select(c => new Vendor { Id = c.Id, Name = c.Name }).ToList();
        }

        public int AddItems(Item item)
        {
            int result = -1;
            if (item != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Items.Add(item);
                dbContext.SaveChanges();
                result = item.Id;
            }

            return result;
        }

        public Item GetItemByName(string itemName)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Items.Where(x => x.ItemName == itemName).FirstOrDefault();
        }

        public Item GetItemByNameAndId(string itemName, int id)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Items.Where(x => x.ItemName == itemName && x.Id != id).FirstOrDefault();
        }

        public void UpdateItem(Item item)
        {
            if (item != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Entry(item).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public Item GetItemById(int Id)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Items.Where(x => x.Id == Id).FirstOrDefault();
        }

        public void DeleteItem(Item item)
        {
            if (item != null)
            {
                dbContext.Items.Remove(item);
                dbContext.SaveChanges();
            }
        }

        public List<Item> GetAllItems()
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Items.ToList();
        }

        public List<Item> GetAllItemsData()
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Items.Select(c => new Item { Id = c.Id, ItemName = c.ItemName }).ToList();
        }

        public List<ItemModel> GetAllItemsModel()
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            var query = from item in dbContext.Items
                        join unit in dbContext.ItemUnits on item.UnitId equals unit.Id
                        select new ItemModel
                        {
                            ItemDescription = item.ItemDescription,
                            Id = item.Id,
                            ItemName = item.ItemName,
                            UnitId = unit.Id,
                            UnitName = unit.Name
                        };

            return query.ToList();
        }

        public List<ItemUnit> GetAllItemUnits()
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.ItemUnits.ToList();
        }

        public int AddItemPurchase(ItemPurchase itemPurchase)
        {
            int result = -1;
            if (itemPurchase != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.ItemPurchases.Add(itemPurchase);
                dbContext.SaveChanges();
                result = itemPurchase.Id;
            }

            return result;
        }

        public ItemPurchase GetItemPurchaseByOrderId(int orderId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.ItemPurchases.Where(x => x.OrderId == orderId).FirstOrDefault();
        }

        public ItemPurchase GetItemPurchaseById(int Id)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.ItemPurchases.Where(x => x.Id == Id).FirstOrDefault();
        }

        public void UpdateItemPurchase(ItemPurchase itemPurchase)
        {
            if (itemPurchase != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Entry(itemPurchase).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }
        
        public List<ItemPurchase> GetAllItemPurchase()
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.ItemPurchases.OrderByDescending(x => x.Id).ToList();
        }

        public void DeleteItemPurchase(ItemPurchase itemPurchase)
        {
            if (itemPurchase != null)
            {
                dbContext.ItemPurchases.Remove(itemPurchase);
                dbContext.SaveChanges();
            }
        }

        public List<ItemPurchaseModel> SearchItemPurchase(DateTime fromDate, DateTime toDate, int orderId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;

            var query = from purchase in dbContext.ItemPurchases
                        where ((EntityFunctions.TruncateTime(purchase.CreatedOn) >= fromDate.Date
                           && EntityFunctions.TruncateTime(purchase.CreatedOn) <= toDate.Date) 
                           || purchase.OrderId == orderId)
                        select new ItemPurchaseModel
                        {
                            Id = purchase.Id,
                            Amount = purchase.Amount,
                            OrderId = purchase.OrderId,
                            CreatedOn = purchase.CreatedOn,
                            PurchaseDate = purchase.PurchaseDate
                        };
            return query.ToList();
        }

        public int AddItemPurchaseDetail(ItemPurchaseDetail itemPurchaseDetail)
        {
            int result = -1;
            if (itemPurchaseDetail != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.ItemPurchaseDetails.Add(itemPurchaseDetail);
                dbContext.SaveChanges();
                result = itemPurchaseDetail.Id;
            }

            return result;
        }
        
        public ItemPurchaseDetail GetItemPurchaseDetailById(int id)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.ItemPurchaseDetails.Where(x => x.Id == id).FirstOrDefault();
        }

        public void UpdateItemPurchaseDetail(ItemPurchaseDetail itemPurchaseDetail)
        {
            if (itemPurchaseDetail != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Entry(itemPurchaseDetail).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void DeleteItemPurchaseDetail(ItemPurchaseDetail itemPurchaseDetail)
        {
            if (itemPurchaseDetail != null)
            {
                dbContext.ItemPurchaseDetails.Remove(itemPurchaseDetail);
                dbContext.SaveChanges();
            }
        }

        public List<ItemPurchaseDetail> GetAllItemPurchaseDetailByItemPurchaseId(int itemPurchaseId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.ItemPurchaseDetails.Where(x => x.ItemPurchaseId == itemPurchaseId).ToList();
        }
        
        public List<ItemPurchaseDetailModel> GetAllItemPurchaseDetailModelByItemPurchaseId(int itemPurchaseId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            var query = from purchaseDetail in dbContext.ItemPurchaseDetails
                        join item in dbContext.Items on purchaseDetail.ItemId equals item.Id
                        join purchase in dbContext.ItemPurchases on purchaseDetail.ItemPurchaseId equals purchase.Id
                        where purchaseDetail.ItemPurchaseId == itemPurchaseId
                        select new ItemPurchaseDetailModel
                        {
                            Id = purchaseDetail.Id,
                            ItemId = purchaseDetail.ItemId,
                            ItemName = item.ItemName,
                            ItemPurchaseId = purchaseDetail.ItemPurchaseId,
                            OrderId = purchase.OrderId,
                            Quantity = purchaseDetail.Quantity,
                            Rate = purchaseDetail.Rate,
                            Total = purchaseDetail.Total,
                            IssuanceQuantity = purchaseDetail.IssuanceQuantity
                        };

            return query.ToList();
        }

        public int GetPurchaseOrderId()
        {
            int orderId = 1;
            var count = dbContext.ItemPurchases.Count();
            if (count > 0)
            {
                orderId = dbContext.ItemPurchases.Max(x => x.OrderId);
                orderId++;
            }

            return orderId;
        }

        public int AddItemIssuance(ItemIssuance itemIssuance)
        {
            int result = -1;
            if (itemIssuance != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.ItemIssuances.Add(itemIssuance);
                dbContext.SaveChanges();
                result = itemIssuance.Id;
            }

            return result;
        }

        public ItemIssuance GetItemIssuanceByOrderId(int orderId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.ItemIssuances.Where(x => x.OrderId == orderId).FirstOrDefault();
        }

        public ItemIssuance GetItemIssuanceById(int Id)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.ItemIssuances.Where(x => x.Id == Id).FirstOrDefault();
        }

        public void UpdateItemIssuance(ItemIssuance itemIssuance)
        {
            if (itemIssuance != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Entry(itemIssuance).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public List<ItemIssuance> GetAllItemIssuance()
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.ItemIssuances.OrderByDescending(x => x.Id).ToList();
        }

        public void DeleteItemIssuance(ItemIssuance itemIssuance)
        {
            if (itemIssuance != null)
            {
                dbContext.ItemIssuances.Remove(itemIssuance);
                dbContext.SaveChanges();
            }
        }

        public List<ItemIssuanceModel> SearchItemIssuanc(DateTime fromDate, DateTime toDate, int orderId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;

            var query = from purchase in dbContext.ItemIssuances
                        where ((EntityFunctions.TruncateTime(purchase.CreatedOn) >= fromDate.Date
                           && EntityFunctions.TruncateTime(purchase.CreatedOn) <= toDate.Date)
                           || purchase.OrderId == orderId)
                        select new ItemIssuanceModel
                        {
                            Id = purchase.Id,
                            Amount = purchase.Amount,
                            OrderId = purchase.OrderId,
                            CreatedOn = purchase.CreatedOn,
                            IssuanceDate = purchase.IssuanceDate
                        };
            return query.ToList();
        }

        public int AddItemIssuanceDetail(ItemIssuanceDetail itemIssuanceDetail)
        {
            int result = -1;
            if (itemIssuanceDetail != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.ItemIssuanceDetails.Add(itemIssuanceDetail);
                dbContext.SaveChanges();
                result = itemIssuanceDetail.Id;
            }

            return result;
        }

        public ItemIssuanceDetail GetItemIssuanceDetailById(int id)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.ItemIssuanceDetails.Where(x => x.Id == id).FirstOrDefault();
        }

        public void UpdateItemIssuanceDetail(ItemIssuanceDetail itemIssuanceDetail)
        {
            if (itemIssuanceDetail != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Entry(itemIssuanceDetail).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void DeleteItemIssuanceDetail(ItemIssuanceDetail itemIssuanceDetail)
        {
            if (itemIssuanceDetail != null)
            {
                dbContext.ItemIssuanceDetails.Remove(itemIssuanceDetail);
                dbContext.SaveChanges();
            }
        }

        public List<ItemIssuanceDetail> GetAllItemIssuanceDetailByItemIssuanceId(int IiemIssuanceId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.ItemIssuanceDetails.Where(x => x.ItemIssuanceId == IiemIssuanceId).ToList();
        }

        public List<ItemIssuanceDetailModel> GetAllItemIssuanceDetailModelByItemIssuanceId(int itemIssuanceId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            var query = from issuanceDetail in dbContext.ItemIssuanceDetails
                        join item in dbContext.Items on issuanceDetail.ItemId equals item.Id
                        join issuance in dbContext.ItemIssuances on issuanceDetail.ItemIssuanceId equals issuance.Id
                        where issuanceDetail.ItemIssuanceId == itemIssuanceId
                        select new ItemIssuanceDetailModel
                        {
                            Id = issuanceDetail.Id,
                            ItemId = issuanceDetail.ItemId,
                            ItemName = item.ItemName,
                            ItemIssuanceId = issuanceDetail.ItemIssuanceId,
                            OrderId = issuance.OrderId,
                            Quantity = issuanceDetail.Quantity,
                            Rate = issuanceDetail.Rate,
                            Total = issuanceDetail.Total
                        };

            return query.ToList();
        }

        public int GetIssuanceOrderId()
        {
            int orderId = 1;
            var count = dbContext.ItemIssuances.Count();
            if (count > 0)
            {
                orderId = dbContext.ItemIssuances.Max(x => x.OrderId);
                orderId++;
            }

            return orderId;
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    dbContext.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }  
    }
}
