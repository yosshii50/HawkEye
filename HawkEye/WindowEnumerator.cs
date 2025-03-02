using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace HawkEye
{
    class WindowEnumerator
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

        // コールバック関数の定義
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        // IVirtualDesktopManager インターフェースの定義
        [ComImport]
        [Guid("a5cd92ff-29be-454c-8d04-d82879fb3f1b")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IVirtualDesktopManager
        {
            bool IsWindowOnCurrentVirtualDesktop(IntPtr hWnd);
            Guid GetWindowDesktopId(IntPtr hWnd);
            void MoveWindowToDesktop(IntPtr hWnd, [MarshalAs(UnmanagedType.LPStruct)] Guid desktopId);
        }

        // IVirtualDesktopManager インターフェースのインスタンスを取得
        private static readonly IVirtualDesktopManager virtualDesktopManager = (IVirtualDesktopManager)new VirtualDesktopManager();

        // VirtualDesktopManager クラスの定義
        [ComImport]
        [Guid("aa509086-5ca9-4c25-8f95-589d3c07b48a")]
        private class VirtualDesktopManager
        {
        }

        public static string[] GetVisibleWindows2()
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
                        Guid desktopId = virtualDesktopManager.GetWindowDesktopId(hWnd); // ←エラーになる
                        windowList.Add($"Title: {windowTitle}, Desktop ID: {desktopId}");
                    }
                }
                return true;  // 次のウィンドウへ
            }, IntPtr.Zero);

            return windowList.ToArray();
        }

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
                        windowList.Add(windowTitle.ToString());
                    }
                }
                return true;  // 次のウィンドウへ
            }, IntPtr.Zero);

            return windowList.ToArray();
        }
    }
}
