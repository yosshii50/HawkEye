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

        // コールバック関数の定義
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        public static string[] GetVisibleWindows()
        {
            List<string> windowList = new List<string>();

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
                        string startTimeFormatted = startTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

                        windowList.Add($"Title: {windowTitle}, Start Time: {startTimeFormatted}");
                    }
                }
                return true;  // 次のウィンドウへ
            }, IntPtr.Zero);

            return windowList.ToArray();
        }
    }
}

