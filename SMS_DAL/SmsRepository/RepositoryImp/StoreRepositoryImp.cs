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

        public List<ItemPurchaseModel> SearchItemPurchase(DateTime fromDate, DateTime toDate, int orderId, int itemId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;

            var query = from purchase in dbContext.ItemPurchases
                        join purchaseDetail in dbContext.ItemPurchaseDetails on purchase.Id equals purchaseDetail.ItemPurchaseId
                        where ((EntityFunctions.TruncateTime(purchase.CreatedOn) >= fromDate.Date
                           && EntityFunctions.TruncateTime(purchase.CreatedOn) <= toDate.Date) 
                           || purchase.OrderId == orderId)
                           &&(itemId == 0 || purchaseDetail.ItemId == itemId)
                        select new ItemPurchaseModel
                        {
                            Id = purchase.Id,
                            Amount = purchase.Amount,
                            OrderId = purchase.OrderId,
                            CreatedOn = purchase.CreatedOn,
                            PurchaseDate = purchase.PurchaseDate
                        };
            return query.Distinct().ToList();
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

        public List<ItemIssuanceModel> SearchItemIssuanc(DateTime fromDate, DateTime toDate, int orderId, int itemId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;

            var query = from issuance in dbContext.ItemIssuances
                        join issuanceDetail in dbContext.ItemIssuanceDetails on issuance.Id equals issuanceDetail.ItemIssuanceId
                        where ((EntityFunctions.TruncateTime(issuance.CreatedOn) >= fromDate.Date
                           && EntityFunctions.TruncateTime(issuance.CreatedOn) <= toDate.Date)
                           || issuance.OrderId == orderId)
                           &&(itemId == 0 || itemId == issuanceDetail.ItemId)
                        select new ItemIssuanceModel
                        {
                            Id = issuance.Id,
                            Amount = issuance.Amount,
                            OrderId = issuance.OrderId,
                            CreatedOn = issuance.CreatedOn,
                            IssuanceDate = issuance.IssuanceDate
                        };
            return query.Distinct().ToList();
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
                        join unit in dbContext.ItemUnits on issuanceDetail.UnitId equals unit.Id
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
                            Total = issuanceDetail.Total,
                            UnitName = unit.Name
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

        public int AddIssuanceStockQuantity(IssuanceStockQuantity issuanceStockQuantity)
        {
            int result = -1;
            if (issuanceStockQuantity != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.IssuanceStockQuantities.Add(issuanceStockQuantity);
                dbContext.SaveChanges();
                result = issuanceStockQuantity.Id;
            }

            return result;
        }
        public void UpdateIssuanceStockQuantity(IssuanceStockQuantity issuanceStockQuantity)
        {
            if (issuanceStockQuantity != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Entry(issuanceStockQuantity).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void DeleteIssuanceStockQuantity(IssuanceStockQuantity issuanceStockQuantity)
        {
            if (issuanceStockQuantity != null)
            {
                dbContext.IssuanceStockQuantities.Remove(issuanceStockQuantity);
                dbContext.SaveChanges();
            }
        }

        public List<IssuanceStockQuantity> GetIssuanceQuanatityList(int itemIssuanceDetailId)
        {
            return dbContext.IssuanceStockQuantities.Where(x => x.ItemIssuanceDetailId == itemIssuanceDetailId).ToList();
        }

        public List<IssuanceStockQuantity> GetIssuanceReturnQuanatityList(int itemIssuanceDetailId)
        {
            return dbContext.IssuanceStockQuantities.Where(x => x.ItemReturnDetailId == itemIssuanceDetailId).ToList();
        }


        public List<ItemPurchaseDetailModel> GetItemPurchaseWithQuantity(int itemId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            var query = from purchaseDetail in dbContext.ItemPurchaseDetails
                        join purchase in dbContext.ItemPurchases on purchaseDetail.ItemPurchaseId equals purchase.Id
                        join item in dbContext.Items on purchaseDetail.ItemId equals item.Id
                        where purchaseDetail.ItemId == itemId
                        && purchaseDetail.Quantity - (purchaseDetail.IssuanceQuantity ?? 0) > 0
                        select new ItemPurchaseDetailModel
                        {
                            Id = purchaseDetail.Id,
                            ItemId = purchaseDetail.ItemId,
                            ItemName = item.ItemName,
                            OrderId = purchase.OrderId,
                            Quantity = purchaseDetail.Quantity,
                            Rate = purchaseDetail.Rate,
                            Total = purchaseDetail.Total
                        };

            return query.OrderBy(x => x.Id).ToList();
        }

        public List<ItemPurchaseDetailModel> GetItemPurchaseWithZeroQuantity(int itemId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            var query = from purchaseDetail in dbContext.ItemPurchaseDetails
                        join purchase in dbContext.ItemPurchases on purchaseDetail.ItemPurchaseId equals purchase.Id
                        join item in dbContext.Items on purchaseDetail.ItemId equals item.Id
                        where purchaseDetail.ItemId == itemId
                        && purchaseDetail.Quantity - (purchaseDetail.IssuanceQuantity ?? 0) == 0
                        select new ItemPurchaseDetailModel
                        {
                            Id = purchaseDetail.Id,
                            ItemId = purchaseDetail.ItemId,
                            ItemName = item.ItemName,
                            OrderId = purchase.OrderId,
                            Quantity = purchaseDetail.Quantity,
                            Rate = purchaseDetail.Rate,
                            Total = purchaseDetail.Total
                        };

            return query.OrderByDescending(x => x.Id).ToList();
        }


        public int AddItemReturn(ItemReturn itemReturn)
        {
            int result = -1;
            if (itemReturn != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.ItemReturns.Add(itemReturn);
                dbContext.SaveChanges();
                result = itemReturn.Id;
            }

            return result;
        }

        public ItemReturn GetItemReturnByOrderId(int orderId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.ItemReturns.Where(x => x.OrderId == orderId).FirstOrDefault();
        }

        public ItemReturn GetItemReturnById(int Id)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.ItemReturns.Where(x => x.Id == Id).FirstOrDefault();
        }

        public void UpdateItemReturn(ItemReturn itemReturn)
        {
            if (itemReturn != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Entry(itemReturn).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public List<ItemReturn> GetAllItemItemReturn()
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.ItemReturns.OrderByDescending(x => x.Id).ToList();
        }

        public void DeleteItemReturn(ItemReturn itemReturn)
        {
            if (itemReturn != null)
            {
                dbContext.ItemReturns.Remove(itemReturn);
                dbContext.SaveChanges();
            }
        }

        public List<ItemReturnModel> SearchItemReturn(DateTime fromDate, DateTime toDate, int orderId, int itemId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;

            var query = from ireturn in dbContext.ItemReturns
                        join returnDetail in dbContext.ItemReturnDetails on ireturn.Id equals returnDetail.ItemReturnId
                        where ((EntityFunctions.TruncateTime(ireturn.CreatedOn) >= fromDate.Date
                           && EntityFunctions.TruncateTime(ireturn.CreatedOn) <= toDate.Date)
                           || ireturn.OrderId == orderId)
                           && (itemId == 0 || itemId == returnDetail.ItemId)
                        select new ItemReturnModel
                        {
                            Id = ireturn.Id,
                            Amount = ireturn.Amount,
                            OrderId = ireturn.OrderId,
                            CreatedOn = ireturn.CreatedOn,
                            ReturnDate = ireturn.ReturnDate
                        };
            return query.Distinct().ToList();
        }

        public int AddItemReturnDetail(ItemReturnDetail itemReturnDetail)
        {
            int result = -1;
            if (itemReturnDetail != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.ItemReturnDetails.Add(itemReturnDetail);
                dbContext.SaveChanges();
                result = itemReturnDetail.Id;
            }

            return result;
        }

        public ItemReturnDetail GetItemReturnDetailById(int id)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.ItemReturnDetails.Where(x => x.Id == id).FirstOrDefault();
        }

        public void UpdateItemReturnDetail(ItemReturnDetail itemReturnDetail)
        {
            if (itemReturnDetail != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Entry(itemReturnDetail).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void DeleteItemReturnDetail(ItemReturnDetail itemReturnDetail)
        {
            if (itemReturnDetail != null)
            {
                dbContext.ItemReturnDetails.Remove(itemReturnDetail);
                dbContext.SaveChanges();
            }
        }

        public List<ItemReturnDetail> GetAllItemReturnDetailByItemReturnId(int itemReturnId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.ItemReturnDetails.Where(x => x.ItemReturnId == itemReturnId).ToList();
        }

        public List<ItemReturnDetailModel> GetAllItemReturnDetailModelByItemReturnId(int itemReturnId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            var query = from retuernDetail in dbContext.ItemReturnDetails
                        join item in dbContext.Items on retuernDetail.ItemId equals item.Id
                        join unit in dbContext.ItemUnits on retuernDetail.UnitId equals unit.Id
                        join ireturn in dbContext.ItemReturns on retuernDetail.ItemReturnId equals ireturn.Id
                        where retuernDetail.ItemReturnId == itemReturnId
                        select new ItemReturnDetailModel
                        {
                            Id = retuernDetail.Id,
                            ItemId = retuernDetail.ItemId,
                            ItemName = item.ItemName,
                            ItemReturnId = retuernDetail.ItemReturnId,
                            OrderId = ireturn.OrderId,
                            Quantity = retuernDetail.Quantity,
                            Rate = retuernDetail.Rate,
                            Total = retuernDetail.Total,
                            UnitName = unit.Name
                        };

            return query.ToList();
        }

        public int GetReturnOrderId()
        {
            int orderId = 1;
            var count = dbContext.ItemReturns.Count();
            if (count > 0)
            {
                orderId = dbContext.ItemReturns.Max(x => x.OrderId);
                orderId++;
            }

            return orderId;
        }


        public int AddItemVendor(ItemVendor itemVendor)
        {
            int result = -1;
            if (itemVendor != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.ItemVendors.Add(itemVendor);
                dbContext.SaveChanges();
                result = itemVendor.Id;
            }

            return result;
        }

        public ItemVendor GetItemVendorById(int id)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.ItemVendors.Where(x => x.Id == id).FirstOrDefault();
        }

        public List<ItemVendorModel> GetItemVendorsByItemId(int id)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            var query = from itemVend in dbContext.ItemVendors
                        join item in dbContext.Items on itemVend.ItemId equals item.Id
                        join vendor in dbContext.Vendors on itemVend.VendorId equals vendor.Id
                        where itemVend.ItemId == id
                        select new ItemVendorModel
                        {
                            Id = itemVend.Id,
                            CompanyName = vendor.CompanyName,
                            Email = vendor.Email,
                            ItemId = itemVend.ItemId,
                            ItemName = item.ItemName,
                            PhoneNo = vendor.PhoneNo,
                            VendorId = itemVend.VendorId,
                            VendorName = vendor.Name
                        };
            return query.ToList();
        }
        
        public void DeleteItemVendor(ItemVendor itemVendor)
        {
            if (itemVendor != null)
            {
                dbContext.ItemVendors.Remove(itemVendor);
                dbContext.SaveChanges();
            }
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
