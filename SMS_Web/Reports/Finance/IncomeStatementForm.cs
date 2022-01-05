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
    public partial class IncomeStatementForm : Form
    {
        FinanceManagement finMang;
        int searchCriteria;
        public IncomeStatementForm(int i = 0)
        {
            InitializeComponent();
            finMang = ProxyObjects.GetFinanceHelper();
            searchCriteria = i;
            if (searchCriteria == 10 || searchCriteria == 11 || searchCriteria == 12)
            {
                this.cbLevels.Visible = false;
                this.label3.Visible = false;
                if (searchCriteria == 10 || searchCriteria == 11)
                {
                    if (searchCriteria == 11)
                        this.chbDetail.Text = "Include Cash Withdrawl";
                    this.chbDetail.Visible = true;
                }
            }
            else
            {
                populate_cbAccounts();
            }
        }

        private void populate_cbAccounts()
        {
            this.cbLevels.Items.Clear();
            this.cbLevels.Items.Add("First Level");
            this.cbLevels.Items.Add("Seccond Level");
            this.cbLevels.Items.Add("Third Level");
            this.cbLevels.Items.Add("Fourth Level");
            this.cbLevels.Items.Add("Fifth Level");
            this.cbLevels.SelectedIndex = 0;
        }

       
        private void btnFind_Click(object sender, EventArgs e)
        {
            DateTime fromDate = CacheClient.GetFromDate(dtpFrom.Value);
            DateTime toDate = CacheClient.GetToDate(dtpTo.Value);
            
            DataTable table;
            if (searchCriteria == 10)
            {
                int isChecked = -1;
                if ((bool)this.chbDetail.Checked == true)
                    isChecked = 1;
                table = finMang.GetCapitalInvestmentReportData(fromDate, toDate);
                Proxy.reports.Populate_ReportViewer(table, fromDate, toDate, isChecked);
            }
            else if(searchCriteria == 11)
            {
                table = finMang.GetProfitData(fromDate, toDate);
                //DataTable table1 = finMang.GetProfitPercentage(fromDate, toDate);
                Proxy.reports.Populate_ReportViewer(table, fromDate, toDate, (bool)this.chbDetail.Checked);
            }
            else if (searchCriteria == 12)
            {
                table = finMang.GetCapitalWithDrawlReportData(fromDate, toDate);
                Proxy.reports.Populate_ReportViewer(table, fromDate, toDate, this.cbLevels.SelectedIndex);
            }
            else
            {
                int index = this.cbLevels.SelectedIndex;
                //if (searchCriteria == 162)
                //    index = 4;
                table = finMang.GetIncomeStatement(fromDate, toDate, index);
                Proxy.reports.Populate_ReportViewer(table, fromDate, toDate, index);
            }
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
    