using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;
using Newtonsoft.Json.Linq;

namespace HawkEye
{
    public partial class chrome_profile: Form
    {
        public chrome_profile()
        {
            InitializeComponent();
            // DataGridViewのReadOnlyプロパティを設定
            dataGridView1.ReadOnly = true;
            // DataGridViewのセルの内容をコピーするためのイベントを追加
            dataGridView1.KeyDown += DataGridView1_KeyDown;
            // DataGridViewの行番号を描画するためのイベントを追加
            dataGridView1.RowPostPaint += DataGridView1_RowPostPaint;
        }

        // セルの内容をコピーするためのイベント
        private void DataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                DataObject dataObj = dataGridView1.GetClipboardContent();
                if (dataObj != null)
                {
                    Clipboard.SetDataObject(dataObj);
                    e.Handled = true;
                }
            }
        }

        // 行番号を描画するためのイベント
        private void DataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            using (SolidBrush b = new SolidBrush(dataGridView1.RowHeadersDefaultCellStyle.ForeColor))
            {
                e.Graphics.DrawString((e.RowIndex + 1).ToString(), e.InheritedRowStyle.Font, b, e.RowBounds.Location.X + 20, e.RowBounds.Location.Y + 4);
            }
        }

        // フォームがロードされたときのイベント
        private void chrome_profile_Load(object sender, EventArgs e)
        {
            /*
            // サンプルデータを作成
            var dataTable = new DataTable();
            dataTable.Columns.Add("ID", typeof(int));
            dataTable.Columns.Add("Name", typeof(string));
            dataTable.Columns.Add("Age", typeof(int));

            dataTable.Rows.Add(1, "Alice", 30);
            dataTable.Rows.Add(2, "Bob", 25);
            dataTable.Rows.Add(3, "Charlie", 35);

            // DataGridViewにデータをバインド
            dataGridView1.DataSource = dataTable;
            */

            GetProfileInfo();
        }

        private void GetProfileInfo()
        {
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string chromeUserDataPath = Path.Combine(userProfile, "AppData", "Local", "Google", "Chrome", "User Data", "Local State");
            string jsonContent = File.ReadAllText(chromeUserDataPath, Encoding.UTF8);
            JObject jObject = JObject.Parse(jsonContent);

            //StringBuilder textToCopy = new StringBuilder();
            var dataTable = new DataTable();
            dataTable.Columns.Add("Profile", typeof(string));
            dataTable.Columns.Add("Name", typeof(string));
            dataTable.Columns.Add("ID", typeof(string));

            var profileInfoCache = jObject["profile"]["info_cache"] as JObject;
            if (profileInfoCache != null)
            {
                foreach (var item in profileInfoCache)
                {
                    string key = item.Key;
                    string shortcutName = item.Value["shortcut_name"]?.ToString() ?? "";
                    string userName = item.Value["user_name"]?.ToString() ?? "";

                    string line = $"{key}\t{shortcutName}\t{userName}";
                    Console.WriteLine(line);

                    dataTable.Rows.Add(key, shortcutName, userName);
                    //textToCopy.AppendLine(line);
                }
            }

            //string result = textToCopy.ToString();
            dataGridView1.DataSource = dataTable;

            // 各カラムの幅を表示文字に合わせて自動調整
            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

        }
    }
}
