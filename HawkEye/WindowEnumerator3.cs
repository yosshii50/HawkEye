// この方法は遅いので没

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HawkEye
{
    class WindowEnumerator3
    {
        // SetForegroundWindow 関数（ウィンドウにフォーカスを当てる）
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        // ウィンドウ情報を保持するクラス
        public class WindowInfo
        {
            public string Title { get; set; }
            public DateTime StartTime { get; set; }
            public IntPtr HWnd { get; set; }
        }


        public static List<WindowInfo> GetVisibleWindows()
        {
            List<WindowInfo> windowList = new List<WindowInfo>();

            string query = "SELECT Name, ProcessId FROM Win32_Process";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);

            foreach (ManagementObject obj in searcher.Get())
            {
                try
                {
                    int processId = Convert.ToInt32(obj["ProcessId"]);
                    Process process = Process.GetProcessById(processId);

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
                catch
                {
                    // 例外発生時は無視（プロセスが終了している場合など）
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
    }
}
