using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.Reports
{
    public class DAL_Store_Reports : DAL.DABase
    {
        public DataSet GetItemPurchaseData(int orderId, int itemId, DateTime fromDate, DateTime toDate)
        {
            var sql = @"select RIGHT('0000000'+CAST(ips.OrderId AS VARCHAR(7)),7) as OrderId, ips.CreatedOn, item.ItemName,
                        ipd.Quantity, ipd.Rate, ipd.Total
                        from ItemPurchaseDetail ipd
                        inner join ItemPurchase ips on ips.Id = ipd.ItemPurchaseId
                        inner join Items item on item.Id = ipd.ItemId
                        where ips.CreatedOn >= '{0}' and ips.CreatedOn <= '{1}'
                        and (0 in ({2}) or ips.OrderId in ({2}))
                        and (0 in ({3}) or ipd.ItemId in ({3}))";
            sql = string.Format(sql, getDate(fromDate), getDate(toDate), orderId, itemId);
            return ExecuteDataSet(sql);
        }

        public DataSet GetItemIssuanceData(int orderId, int itemId, DateTime fromDate, DateTime toDate)
        {
            var sql = @"select RIGHT('0000000'+CAST(iss.OrderId AS VARCHAR(7)),7) as OrderId, iss.CreatedOn, item.ItemName,
                        isd.Quantity, isnull(isd.Rate, 0) as Rate, isnull(isd.Total, 0) as Total, unit.Name as Unit
                        from ItemIssuanceDetail isd
                        inner join ItemIssuance iss on iss.Id = isd.ItemIssuanceId
                        inner join Items item on item.Id = isd.ItemId
						inner join ItemUnit unit on isd.UnitId = unit.Id
                        where iss.CreatedOn >= '{0}' and iss.CreatedOn <= '{1}'
                        and (0 in ({2}) or iss.OrderId in ({2}))
                        and (0 in ({3}) or isd.ItemId in ({3}))";
            sql = string.Format(sql, getDate(fromDate), getDate(toDate), orderId, itemId);
            return ExecuteDataSet(sql);
        }

        public DataSet GetItemReturnData(int orderId, int itemId, DateTime fromDate, DateTime toDate)
        {
            var sql = @"select RIGHT('0000000'+CAST(irs.OrderId AS VARCHAR(7)),7) as OrderId, irs.CreatedOn, item.ItemName,
                        ird.Quantity, isnull(ird.Rate, 0) as Rate, isnull(ird.Total, 0) as Total, unit.Name as Unit
                        from ItemReturnDetail ird
                        inner join ItemReturn irs on irs.Id = ird.ItemReturnId
                        inner join Items item on item.Id = ird.ItemId
						inner join ItemUnit unit on ird.UnitId = unit.Id
                        where irs.CreatedOn >= '{0}' and irs.CreatedOn <= '{1}'
                        and (0 in ({2}) or irs.OrderId in ({2}))
                        and (0 in ({3}) or ird.ItemId in ({3}))";
            sql = string.Format(sql, getDate(fromDate), getDate(toDate), orderId, itemId);
            return ExecuteDataSet(sql);
        }

        public DataSet GetCurrentStockData()
        {
            var sql = @"select item.Id, item.ItemName, unit.Name as Unit,
                        sum(ipd.Quantity - isnull(ipd.IssuanceQuantity,0)) as Quantity,
                        sum(cast (ROUND((ipd.Quantity - isnull(ipd.IssuanceQuantity,0)) * ipd.Rate,2) as numeric(36,2))) as Total
                        from ItemPurchaseDetail ipd
                        inner join ItemPurchase ips on ips.Id = ipd.ItemPurchaseId
                        inner join Items item on item.Id = ipd.ItemId
						inner join ItemUnit unit on item.UnitId = unit.Id
                        where (ipd.Quantity - isnull(ipd.IssuanceQuantity,0)) > 0
                        Group By item.Id, item.ItemName, unit.Name
						order by item.Id";
            return ExecuteDataSet(sql);
        }

        public DataSet GetItemPurchase(int orderId)
        {
            var sql = @"select RIGHT('0000000'+CAST(ips.OrderId AS VARCHAR(7)),7) as OrderId, ips.CreatedOn, item.ItemName,
                        ipd.Quantity, ipd.Rate, ipd.Total
                        from ItemPurchaseDetail ipd
                        inner join ItemPurchase ips on ips.Id = ipd.ItemPurchaseId
                        inner join Items item on item.Id = ipd.ItemId
                        where ips.OrderId =  {0}";
            sql = string.Format(sql, orderId);
            return ExecuteDataSet(sql);
        }

        public DataSet GetItemIssuance(int orderId)
        {
            var sql = @"select RIGHT('0000000'+CAST(iss.OrderId AS VARCHAR(7)),7) as OrderId, iss.CreatedOn, item.ItemName,
                        isd.Quantity, isnull(isd.Rate, 0) as Rate, isnull(isd.Total, 0) as Total, unit.Name as Unit
                        from ItemIssuanceDetail isd
                        inner join ItemIssuance iss on iss.Id = isd.ItemIssuanceId
                        inner join Items item on item.Id = isd.ItemId
						inner join ItemUnit unit on isd.UnitId = unit.Id
                        where iss.OrderId =  {0}";
            sql = string.Format(sql, orderId);
            return ExecuteDataSet(sql);
        }

        public DataSet GetItemReturn(int orderId)
        {
            var sql = @"select RIGHT('0000000'+CAST(irs.OrderId AS VARCHAR(7)),7) as OrderId, irs.CreatedOn, item.ItemName,
                        ird.Quantity, isnull(ird.Rate, 0) as Rate, isnull(ird.Total, 0) as Total, unit.Name as Unit
                        from ItemReturnDetail ird
                        inner join ItemReturn irs on irs.Id = ird.ItemReturnId
                        inner join Items item on item.Id = ird.ItemId
						inner join ItemUnit unit on ird.UnitId = unit.Id
                        where irs.OrderId =  {0}";
            sql = string.Format(sql, orderId);
            return ExecuteDataSet(sql);
        }

        public DataSet GetVendorData()
        {
            var sql = @"select Name as VendorName, PhoneNo, Email, CompanyName
                        from Vendor";
            return ExecuteDataSet(sql);
        }

        public DataSet GetItemData()
        {
            var sql = @"select item.ItemName as ItemName, unit.Name as Unit,
                        item.ItemDescription as Description
                        from Items item 
                        inner join ItemUnit unit on item.UnitId = unit.Id";
            return ExecuteDataSet(sql);
        }



        private string getDate(DateTime date)
        {
            return date.Year + "-" + (date.Month.ToString().Length == 1 ? "0" + date.Month : date.Month.ToString())
                + "-" + (date.Day.ToString().Length == 1 ? "0" + date.Day : date.Day.ToString());
        }

    }
}
