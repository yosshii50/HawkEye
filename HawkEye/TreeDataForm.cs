using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace HawkEye
{
    public partial class TreeDataForm: Form
    {
        // 
        //public string InputText { get; private set; }

        public TreeDataForm()
        {
            InitializeComponent();

            string serializedTreeView = Properties.Settings.Default.TreeViewData;
            this.textBoxInput.Text = serializedTreeView;

        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            //this.InputText = textBoxInput.Text;

            //DeserializeTreeView(treeView1, inputData);
            Properties.Settings.Default.TreeViewData = this.textBoxInput.Text;
            Properties.Settings.Default.Save();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
