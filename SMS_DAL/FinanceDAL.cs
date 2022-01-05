using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL
{
    public class FinanceDAL
    {
        SC_WEBEntities2 db = new SC_WEBEntities2();
        public int getFirstLevelID(int? id)
        {
            FinanceSeccondLvlAccount level = db.FinanceSeccondLvlAccounts.FirstOrDefault(x => x.Id == id);
            if (level != null)
                return level.FirstLvlAccountId;
            return -1;
        }
        public int getSecondLevelID(int? id)
        {
            FinanceThirdLvlAccount level = db.FinanceThirdLvlAccounts.FirstOrDefault(x => x.Id == id);
            if (level != null)
                return level.SeccondLvlAccountId;
            return -1;
        }
        public int getThirdLevelID(int? id)
        {
            FinanceFourthLvlAccount level = db.FinanceFourthLvlAccounts.FirstOrDefault(x => x.Id == id);
            if (level != null)
                return level.ThirdLvlAccountId;
            return -1;
        }
        public int getFourthLevelID(int? id)
        {
            FinanceFifthLvlAccount level = db.FinanceFifthLvlAccounts.FirstOrDefault(x => x.Id == id);
            if (level != null)
                return level.FourthLvlAccountId;
            return -1;
        }
        

    }
}
