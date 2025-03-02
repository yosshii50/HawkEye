using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace HawkEye
{
    class WindowEnumerator2
    {
        // EnumWindows 関数の定義（User32.dllを使用）
        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        // GetWindowText 関数（ウィンドウのタイトル取得）
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        // GetWindowTextLength 関数（ウィンドウのタイトルの長さ取得）
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        // IsWindowVisible 関数（ウィンドウが可視かどうか）
        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        // GetWindowThreadProcessId 関数（ウィンドウのプロセスID取得）
        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        // SetForegroundWindow 関数（ウィンドウにフォーカスを当てる）
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        // コールバック関数の定義
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

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

            // ウィンドウを列挙
            EnumWindows((hWnd, lParam) =>
            {
                if (IsWindowVisible(hWnd))  // 可視ウィンドウのみ
                {
                    int length = GetWindowTextLength(hWnd);
                    if (length > 0)
                    {
                        StringBuilder windowTitle = new StringBuilder(length + 1);
                        GetWindowText(hWnd, windowTitle, windowTitle.Capacity);

                        // ウィンドウのプロセスIDを取得
                        GetWindowThreadProcessId(hWnd, out uint processId);

                        // プロセスの開始時刻を取得
                        Process process = Process.GetProcessById((int)processId);
                        DateTime startTime = process.StartTime;

                        windowList.Add(new WindowInfo
                        {
                            Title = windowTitle.ToString(),
                            StartTime = startTime,
                            HWnd = hWnd
                        });
                    }
                }
                return true;  // 次のウィンドウへ
            }, IntPtr.Zero);

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
