using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HawkEye
{
    public partial class Form1 : Form
    {
        private List<WindowEnumerator2.WindowInfo> windows;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // ウィンドウ一覧を取得してリストボックスに追加
            UpdateWindowList();
        }

        private void UpdateWindowList()
        {
            listBox1.Items.Clear();

            // WindowEnumerator2 クラスを使用してウィンドウ一覧を取得
            windows = WindowEnumerator2.GetVisibleWindows();

            // リストボックスにウィンドウ一覧を追加
            foreach (var window in windows)
            {
                listBox1.Items.Add($"Start Time: {window.StartTime.ToString("yyyy-MM-dd HH:mm:ss.fff")} - Title: {window.Title}");
            }
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                var selectedWindow = windows[listBox1.SelectedIndex];
                WindowEnumerator2.FocusWindow(selectedWindow.HWnd);
            }
        }
    }
}
