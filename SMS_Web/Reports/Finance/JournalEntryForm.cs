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
    public partial class JournalEntryForm : Form
    {
        FinanceManagement finMang;
        int searchCriteria;
        public JournalEntryForm(int search = 0)
        {
            InitializeComponent();
            finMang = ProxyObjects.GetFinanceHelper();
            searchCriteria = search;
            populate_mode();
            populate_cbHeads();
        }


        private void FetchJournalVoucherData()
        {
            int firstLvlIdtemp = -1, seccondLvlIdtemp = -1, thirdLvlIdtemp = -1, fourthLvlIdtemp = -1, fifthLvlIdTemp = -1, entryId = -1;
            if (this.cbFirstLvl.SelectedIndex > 0)
                firstLvlIdtemp = finMang.GetFinanceHeadId(this.cbFirstLvl.SelectedItem.ToString());
            if (this.cbSeccondLvl.SelectedIndex > 0 && firstLvlIdtemp != -1)
                seccondLvlIdtemp = finMang.GetFinanceSubHeadId(firstLvlIdtemp, this.cbSeccondLvl.SelectedItem.ToString());
            if (this.cbThirdLvl.SelectedIndex > 0 && seccondLvlIdtemp != -1)
                thirdLvlIdtemp = finMang.GetSubHeadTypeId(seccondLvlIdtemp, this.cbThirdLvl.SelectedItem.ToString());
            if (this.cbFourthLvl.SelectedIndex >= 0 && thirdLvlIdtemp != -1)
                fourthLvlIdtemp = finMang.GetFourthLvlAccountId(thirdLvlIdtemp, cbFourthLvl.SelectedItem.ToString());
            if (this.cbFifthLvl.SelectedIndex >= 0 && fourthLvlIdtemp != -1)
                fifthLvlIdTemp = finMang.GetFifthLvlAccountId(fourthLvlIdtemp, cbFifthLvl.SelectedItem.ToString());

            if (this.txtEntryId.Text.Length > 0)
                entryId = int.Parse(this.txtEntryId.Text);

            DateTime fromDate = CacheClient.GetFromDate(this.dtpFrom.Value);
            DateTime toDate = CacheClient.GetToDate(this.dtpTo.Value);

            //DataTable table = finMang.GetJournalVoucherReportData(fromDate, toDate, firstLvlIdtemp, seccondLvlIdtemp, thirdLvlIdtemp, fourthLvlIdtemp, firstLvlIdtemp, this.cbModes.SelectedIndex - 1);
            DataTable table = finMang.GetJVReportData(firstLvlIdtemp, seccondLvlIdtemp, thirdLvlIdtemp, fourthLvlIdtemp, fifthLvlIdTemp, this.cbModes.SelectedIndex - 1, fromDate, toDate, entryId);
            if (table.Rows.Count > 0)
            {
                //table.Columns["CreatedOn"].ColumnName = "Created On";
                Proxy.reports.Populate_ReportViewer(table, fromDate, toDate);
            }
            this.Close();
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            if (searchCriteria == 1)
            {
                FetchJournalVoucherData();
                return;
            }
            DataTable table;

            DateTime fromDate = CacheClient.GetFromDate(this.dtpFrom.Value);
            DateTime toDate = CacheClient.GetToDate(this.dtpTo.Value);
            int firstLvlId = -1, seccondLvlId = -1, firstLvlIdCr = -1, seccondLvlIdCr = -1;
            int thirdLvlId = -1, fourthLvlId = -1, thirdLvlIdCr = -1, fourthLvlIdCr = -1;
            int fifthLvlId = -1, fifthLvlIdCr = -1;
            int mode = -1, firstLvlIdtemp = -1, seccondLvlIdtemp = -1, thirdLvlIdtemp = -1, fourthLvlIdtemp = -1, fifthLvlIdTemp = -1, entryId = -1;

            if (this.txtEntryId.Text.Length > 0)
                entryId = int.Parse(this.txtEntryId.Text);

            if (this.cbFirstLvl.SelectedIndex > 0)
                firstLvlId = finMang.GetFinanceHeadId(this.cbFirstLvl.SelectedItem.ToString());
            if (this.cbSeccondLvl.SelectedIndex > 0 && firstLvlId != -1)
                seccondLvlId = finMang.GetFinanceSubHeadId(firstLvlId, this.cbSeccondLvl.SelectedItem.ToString());
            if (this.cbThirdLvl.SelectedIndex > 0 && seccondLvlId != -1)
                thirdLvlId = finMang.GetSubHeadTypeId(seccondLvlId, this.cbThirdLvl.SelectedItem.ToString());
            if (this.cbFourthLvl.SelectedIndex >= 0 && thirdLvlId != -1)
                fourthLvlId = finMang.GetFourthLvlAccountId(thirdLvlId, cbFourthLvl.SelectedItem.ToString());
            if (this.cbFifthLvl.SelectedIndex >= 0 && fourthLvlId != -1)
                fifthLvlId = finMang.GetFifthLvlAccountId(fourthLvlId, cbFifthLvl.SelectedItem.ToString());

            if (firstLvlId != -1)
                mode = this.cbModes.SelectedIndex - 1;

            //if (mode == 0)
            //{
            //    firstLvlId = firstLvlIdtemp;
            //    seccondLvlId = seccondLvlIdtemp;
            //    thirdLvlId = thirdLvlIdtemp;
            //    fifthLvlId = fifthLvlIdTemp;
            //    fourthLvlId = fourthLvlIdtemp;
            //}
            //else if (mode == 1)
            //{
            //    firstLvlIdCr = firstLvlIdtemp;
            //    seccondLvlIdCr = seccondLvlIdtemp;
            //    thirdLvlIdCr = thirdLvlIdtemp;
            //    fifthLvlIdCr = fifthLvlIdTemp;
            //    fourthLvlIdCr = fourthLvlIdtemp;
            //}
            //else if (mode == -1)
            //{
            //    firstLvlIdCr = firstLvlIdtemp;
            //    seccondLvlIdCr = seccondLvlIdtemp;
            //    thirdLvlId = thirdLvlIdtemp;
            //    fifthLvlId = fifthLvlIdTemp;
            //    fourthLvlId = fourthLvlIdtemp;

            //    firstLvlId = firstLvlIdtemp;
            //    seccondLvlId = seccondLvlIdtemp;
            //    thirdLvlIdCr = thirdLvlIdtemp;
            //    fifthLvlIdCr = fifthLvlIdTemp;
            //    fourthLvlIdCr = fourthLvlIdtemp;
            //}


            //if (mode == -1)
            //    table = finMang.GetAllJournalEntriesTable(firstLvlId, seccondLvlId, thirdLvlId, fourthLvlId, firstLvlIdCr, seccondLvlIdCr, thirdLvlIdCr, fourthLvlIdCr, fromDate, toDate,fifthLvlId, fifthLvlIdCr);
            //else
            //    table = finMang.GetJournalEntriesTable(firstLvlId, seccondLvlId, thirdLvlId, fourthLvlId, firstLvlIdCr, seccondLvlIdCr, thirdLvlIdCr, fourthLvlIdCr, fromDate, toDate, fifthLvlId, fifthLvlIdCr);
            table = finMang.GetJournalEntryReportData(firstLvlId, seccondLvlId, thirdLvlId, fourthLvlId, fifthLvlId, mode, fromDate, toDate, entryId);

            if (table.Rows.Count > 0)
            {
                //table.Columns["CreatedOn"].ColumnName = "Created On";
                Proxy.reports.Populate_ReportViewer(table, fromDate, toDate);
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


        private void populate_cbHeads()
        {
            IList list = finMang.GetAllFinanceHeads();
            if (list != null && list.Count > 0)
            {
                this.cbFirstLvl.Items.Clear();
                this.cbFirstLvl.Items.Add("All");
                foreach (object[] obj in list)
                {
                    this.cbFirstLvl.Items.Add(obj.ElementAt(1).ToString());
                }
                this.cbFirstLvl.SelectedIndex = 0;
            }
        }

        private void populate_cbSubHead()
        {
            int headId = finMang.GetFinanceHeadId(this.cbFirstLvl.SelectedItem.ToString());
            IList list = finMang.GetAllFInanceSubHeads(headId);
            this.cbSeccondLvl.Items.Clear();

            if (list != null && list.Count > 0)
            {
                this.cbSeccondLvl.Items.Add("All");
                foreach (object[] obj in list)
                {
                    this.cbSeccondLvl.Items.Add(obj.ElementAt(0).ToString());
                }
                this.cbSeccondLvl.SelectedIndex = 0;
            }
        }

        private void cbFirstLvl_SelectedIndexChanged(object sender, EventArgs e)
        {
            populate_cbSubHead();
        }

        private void cbSeccondLvl_SelectedIndexChanged(object sender, EventArgs e)
        {
            int firstLvlId = finMang.GetFinanceHeadId(this.cbFirstLvl.SelectedItem.ToString());
            int seccondLvlId = finMang.GetFinanceSubHeadId(firstLvlId, this.cbSeccondLvl.SelectedItem.ToString());
            IList list = finMang.GetAllSubHeadType(seccondLvlId);

            this.cbThirdLvl.Items.Clear();

            if (list != null && list.Count > 0)
            {
                this.cbThirdLvl.Items.Add("All");
                foreach (object[] obj in list)
                {
                    this.cbThirdLvl.Items.Add(obj.ElementAt(0).ToString());
                }
                this.cbThirdLvl.SelectedIndex = 0;
            }
        }

        private void cbThirdLvl_SelectedIndexChanged(object sender, EventArgs e)
        {
            int firstLvlId = finMang.GetFinanceHeadId(this.cbFirstLvl.SelectedItem.ToString());
            int seccondLvlId = finMang.GetFinanceSubHeadId(firstLvlId, this.cbSeccondLvl.SelectedItem.ToString());
            int thirdLvlAccountId = finMang.GetSubHeadTypeId(seccondLvlId, this.cbThirdLvl.SelectedItem.ToString());

            IList list = finMang.GetAllFourthLvlAccount(thirdLvlAccountId);

            this.cbFourthLvl.Items.Clear();
            

            if (list != null && list.Count > 0)
            {
                this.cbFourthLvl.Items.Add("All");
                foreach (object[] obj in list)
                {
                    this.cbFourthLvl.Items.Add(obj.ElementAt(0).ToString());
                }
                this.cbFourthLvl.SelectedIndex = 0;
            }
        }

        private void populate_mode()
        {
            this.cbModes.Items.Clear();
            if (searchCriteria == 0)
            {
                this.cbModes.Items.Add("Both");
                this.cbModes.Items.Add("Debit");
                this.cbModes.Items.Add("Credit");
            }
            else
            {
                this.cbModes.Items.Add("Both");
                this.cbModes.Items.Add("UnCleared");
                this.cbModes.Items.Add("Cleared");
            }
            this.cbModes.SelectedIndex = 0;
        }

        private void cbFourthLvl_SelectedIndexChanged(object sender, EventArgs e)
        {
            int firstLvlAccountId = finMang.GetFinanceHeadId(this.cbFirstLvl.SelectedItem.ToString());
            int seccondLvlAccountId = finMang.GetFinanceSubHeadId(firstLvlAccountId, this.cbSeccondLvl.SelectedItem.ToString());
            int thirdLvlAccountId = finMang.GetSubHeadTypeId(seccondLvlAccountId, this.cbThirdLvl.SelectedItem.ToString());
            int fourthLvlAccountId = finMang.GetFourthLvlAccountId(thirdLvlAccountId, this.cbFourthLvl.SelectedItem.ToString());

            IList list = finMang.GetAllFifthLvlAccount(fourthLvlAccountId);

            this.cbFifthLvl.Items.Clear();
            this.cbFifthLvl.Text = "";

            if (list != null && list.Count > 0)
            {
                this.cbFifthLvl.Items.Add("All");
                foreach (object[] obj in list)
                {
                    this.cbFifthLvl.Items.Add(obj.ElementAt(0).ToString());
                }
                this.cbFifthLvl.SelectedIndex = 0;

            }
        }
    }
}
