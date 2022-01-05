using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SMS.Modules.Helper;
using SMS.Modules.DataBase.Domain;
using SMS.Modules.OutLook;
using SMS.Modules.Cache;

namespace SMS.Modules.FinanceMang.Reports
{
    public partial class BankBookForm : Form
    {
        FinanceManagement finMang;
        int searchCriteria;
        public BankBookForm(int criteria = -1)
        {
            InitializeComponent();
            finMang = ProxyObjects.GetFinanceHelper();
            searchCriteria = criteria;
            if (searchCriteria != -1)
            {
                this.cbPayTo.Visible = false;
                this.label3.Visible = false;
            }
            populate_BankAccounts();
        }

       
        private void populate_BankAccounts()
        {
            IList list = finMang.GetAllBanks();
            this.cbPayTo.Items.Clear();
            this.cbPayTo.Items.Add("All");
            if (list != null && list.Count > 0)
            {
                foreach (object[] obj in list)
                    this.cbPayTo.Items.Add(obj.ElementAt(1).ToString() + ", " + obj.ElementAt(2).ToString() + ", " + obj.ElementAt(5).ToString());
            }
            this.cbPayTo.SelectedIndex = 0;
        }

        private int GetBankId()
        {
            string bank = this.cbPayTo.SelectedItem.ToString();
            string[] bankTemp = bank.Split(',');
            int bankId = finMang.GetBankId(bankTemp[1], bankTemp[0]);
            return bankId;
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            DateTime fromDate = CacheClient.GetFromDate(dtpFrom.Value);
            DateTime toDate = CacheClient.GetToDate(dtpTo.Value);
            DataTable table;
            int bankId = -1;

            if (this.cbPayTo.SelectedIndex == 0)
                bankId = -1;
            else if (this.cbPayTo.SelectedIndex == 1)
                bankId = 0;
            else
                bankId = GetBankId();
            if (searchCriteria == -1)
                table = finMang.GetBankBookData(bankId, fromDate, toDate);
            else if (searchCriteria == 159)
                table = finMang.GetGeneralLedgerBookData(fromDate, toDate);
            else if (searchCriteria == 160)
                table = finMang.GetFeeCollectionReportData(fromDate, toDate);
            else if (searchCriteria == 161)
                table = finMang.GetFeeCollectionJournalReport(fromDate, toDate);
            else
                table = finMang.GetCashBookData(fromDate, toDate);
            Proxy.reports.Populate_ReportViewer(table, fromDate, toDate);
            this.Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SearchItemStockSale_Load(object sender, EventArgs e)
        {
            designControls();
        }

    }
}
    