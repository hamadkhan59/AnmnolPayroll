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

namespace SMS.Modules.FinanceMang.Reports
{
    public partial class FinanceLvlForm : Form
    {
        FinanceManagement finMang;
        public FinanceLvlForm(int visible = 0)
        {
            InitializeComponent();
            finMang = ProxyObjects.GetFinanceHelper();
            populate_cbAccounts();
            if (visible == 1)
                this.chbActiveAccounts.Visible = true;
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
            Proxy.reports.populate_report(this.cbLevels.SelectedIndex, (bool)this.chbActiveAccounts.Checked);
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

        private void chbActiveAccounts_CheckedChanged(object sender, EventArgs e)
        {
        }

    }
}
    