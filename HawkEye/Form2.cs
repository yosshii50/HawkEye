using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace HawkEye
{
    public partial class Form2 : Form
    {
        //private List<WindowEnumerator2.WindowInfo> windows;
        //private List<WindowEnumerator3.WindowInfo> windows;
        private List<WindowEnumerator4.WindowInfo> windows;
        private IntPtr hookID = IntPtr.Zero;
        private IntPtr mainWindowHandle;
        private bool isUpdating = false; // フラグを追加
        private WinEventDelegate winEventDelegate;

        public Form2()
        {
            Console.WriteLine("Form2");
            InitializeComponent();
            winEventDelegate = new WinEventDelegate(WinEventProc);
        }

        protected override void OnLoad(EventArgs e)
        {
            Console.WriteLine("OnLoad");
            base.OnLoad(e);
            mainWindowHandle = this.Handle; // 自分自身のウィンドウハンドルを取得
            //hookID = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, WinEventProc, 0, 0, WINEVENT_OUTOFCONTEXT);
            hookID = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, winEventDelegate, 0, 0, WINEVENT_OUTOFCONTEXT);
            //UpdateWindowList();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Console.WriteLine("OnFormClosed");
            UnhookWinEvent(hookID);
            base.OnFormClosed(e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            UpdateWindowList();
        }

        private void UpdateWindowList2()
        {
            if (isUpdating) return; // フラグが立っている場合は処理をスキップ

            isUpdating = true; // フラグを立てる
            Console.WriteLine("UpdateWindowList:Start");
            treeView1.Nodes.Clear();
            Console.WriteLine("UpdateWindowList:1");
            // WindowEnumerator2 クラスを使用してウィンドウ一覧を取得
            //windows = WindowEnumerator2.GetVisibleWindows();
            //windows = WindowEnumerator3.GetVisibleWindows();
            windows = WindowEnumerator4.GetTaskbarWindows();
            Console.WriteLine("UpdateWindowList:2");

            // TreeView にウィンドウ一覧を追加
            foreach (var window in windows)
            {
                TreeNode node = new TreeNode($"Start Time: {window.StartTime.ToString("yyyy-MM-dd HH:mm:ss.fff")} - Title: {window.Title}");
                node.Tag = window.HWnd; // ウィンドウハンドルをタグに保存
                treeView1.Nodes.Add(node);
            }

            Console.WriteLine("UpdateWindowList:End");
            isUpdating = false; // フラグを下ろす
        }
        private async void UpdateWindowList()
        {
            if (isUpdating) return; // フラグが立っている場合は処理をスキップ

            isUpdating = true; // フラグを立てる
            Console.WriteLine($"UpdateWindowList:Start {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}");

            /*
            // 非同期タスクでウィンドウ一覧を取得
            var windows = await Task.Run(() =>
            {
                Console.WriteLine($"UpdateWindowList:1 {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
                // WindowEnumerator2 クラスを使用してウィンドウ一覧を取得
                //windows = WindowEnumerator2.GetVisibleWindows();
                //windows = WindowEnumerator3.GetVisibleWindows();
                return WindowEnumerator4.GetTaskbarWindows();
            });
            */

            //Console.WriteLine($"UpdateWindowList:2 {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}");

            // UIスレッドでTreeViewを更新
            treeView1.BeginInvoke(new Action(() =>
            {
                windows = WindowEnumerator4.GetTaskbarWindows();

                Console.WriteLine($"UpdateWindowList:4 {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
                treeView1.Nodes.Clear();

                Console.WriteLine($"UpdateWindowList:5 {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
                // TreeView にウィンドウ一覧を追加
                foreach (var window in windows)
                {
                    TreeNode node = new TreeNode($"Start Time: {window.StartTime.ToString("yyyy-MM-dd HH:mm:ss.fff")} - Title: {window.Title} / {window.HWnd}");
                    node.Tag = window.HWnd; // ウィンドウハンドルをタグに保存
                    treeView1.Nodes.Add(node);
                }

                Console.WriteLine($"UpdateWindowList:END {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
                isUpdating = false; // フラグを下ろす
            }));

            Console.WriteLine($"UpdateWindowList:3 {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
        }


        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node != null && e.Node.Tag is IntPtr hWnd)
            {
                //WindowEnumerator2.FocusWindow(hWnd);
                WindowEnumerator3.FocusWindow(hWnd);
            }
        }

        private void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (eventType == EVENT_SYSTEM_FOREGROUND && hwnd != mainWindowHandle && !isUpdating)
            {
                UpdateWindowList();
            }
            Console.WriteLine("WinEventProc:" + eventType);
        }

        private const uint EVENT_SYSTEM_FOREGROUND = 0x0003;
        private const uint WINEVENT_OUTOFCONTEXT = 0;

        [DllImport("user32.dll")]
        private static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        private static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        private delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        private void button3_Click(object sender, EventArgs e)
        {
            TreeView_test treeViewForm = new TreeView_test();
            treeViewForm.Show();
        }
    }
}
