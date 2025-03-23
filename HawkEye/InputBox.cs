using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HawkEye
{
    public partial class InputBox : Form
    {
        public string InputText { get; private set; }

        public InputBox(string prompt, string title, string defaultText = "")
        {
            InitializeComponent();
            this.Text = title;
            this.labelPrompt.Text = prompt;
            this.textBoxInput.Text = defaultText;
        }

        private void InputBox_Load(object sender, EventArgs e)
        {

        }


        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.InputText = textBoxInput.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void textBoxInput_TextChanged(object sender, EventArgs e)
        {
            
            // 以下のコマンドでブラウザを起動できる。
            // start chrome --new-window --profile-directory="Profile 17" "https://www.yahoo.co.jp/" "https://www.yahoo.co.jp/"

        }
    }
}
