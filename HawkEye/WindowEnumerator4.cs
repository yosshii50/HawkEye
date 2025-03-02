// 十分速いのでこの方法が良いのでは？

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HawkEye
{
    class WindowEnumerator4
    {
        // ウィンドウ情報を保持するクラス
        public class WindowInfo
        {
            public string Title { get; set; }
            public DateTime StartTime { get; set; }
            public IntPtr HWnd { get; set; }
        }

        public static List<WindowInfo> GetTaskbarWindows()
        {
            List<WindowInfo> windowList = new List<WindowInfo>();

            Process[] processes = Process.GetProcesses();
            Console.WriteLine("ウィンドウの件数: " + processes.Length);

            foreach (Process process in processes)
            {
                if (!string.IsNullOrEmpty(process.MainWindowTitle))
                {
                    windowList.Add(new WindowInfo
                    {
                        Title = process.MainWindowTitle,
                        StartTime = process.StartTime,
                        HWnd = process.MainWindowHandle
                    });
                }
            }

            // 開始日時で降順にソート
            windowList = windowList.OrderByDescending(w => w.StartTime).ToList();

            return windowList;
        }

        public static bool FocusWindow(IntPtr hWnd)
        {
            return SetForegroundWindow(hWnd);
        }

        // SetForegroundWindow 関数（ウィンドウにフォーカスを当てる）
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}


