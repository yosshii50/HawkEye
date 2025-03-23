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

        //private static Dictionary<int, IntPtr> processWindowHandleCache = new Dictionary<int, IntPtr>();
        private static Dictionary<int, WindowInfo> processWindowInfoCache = new Dictionary<int, WindowInfo>();

        public static List<WindowInfo> GetTaskbarWindows()
        {
            List<WindowInfo> windowList = new List<WindowInfo>();

            //Console.WriteLine($"GetTaskbarWindows:Start {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}");

            // ここの処理は0.012秒
            Process[] processes = Process.GetProcesses();

            Console.WriteLine($"GetTaskbarWindows:Count {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} [" + processes.Length + "]");
            //Console.WriteLine("ウィンドウの件数: " + processes.Length);

            int count = 0;
            foreach (Process process in processes)
            {
                count++;
                //Console.WriteLine($"GetTaskbarWindows:Loop1 {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
                //Console.WriteLine($"GetTaskbarWindows:Loop1 {count} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
                //Console.WriteLine($"GetTaskbarWindows:Loop1 {count} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} HWnd: {process.MainWindowHandle}");
                //if (!string.IsNullOrEmpty(process.MainWindowTitle))
                //if (process.MainWindowHandle == 0)
                //IntPtr hWnd = GetMainWindowHandle(process.Id);

                // 2025-03-16 [process.MainWindowHandle]は１プロセス1ミリ秒かかるが、[process.Id]はすぐに終わる
                // IntPtr hWnd = process.MainWindowHandle;
                // int processId = process.Id;


                // キャッシュを使わずに全部再取得するコマンドが必要
                // 処理中に再取得があった場合、最後にもう一度処理する必要がある
                // プロセスIDが変わらず Windowsが表示される場合があるのでは？


                //IntPtr hWnd;
                WindowInfo windowInfo;
                //if (!processWindowHandleCache.TryGetValue(process.Id, out hWnd))
                if (!processWindowInfoCache.TryGetValue(process.Id, out windowInfo))
                {
                    Console.WriteLine($"GetTaskbarWindows:LoopNew {count} {process.Id} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
                    //hWnd = process.MainWindowHandle;
                    //processWindowHandleCache[process.Id] = hWnd;

                    string title = "";
                    DateTime startTime = DateTime.MinValue;
                    IntPtr hWnd = process.MainWindowHandle;

                    if(hWnd != IntPtr.Zero)
                    {
                        title = process.MainWindowTitle;
                        startTime = process.StartTime;
                    }

                    windowInfo = new WindowInfo
                    {
                        //Title = process.MainWindowTitle,
                        Title = title,
                        //StartTime = process.StartTime,
                        StartTime = startTime,
                        HWnd = hWnd
                    };
                    processWindowInfoCache[process.Id] = windowInfo;
                }

                //if (hWnd != IntPtr.Zero)
                if (windowInfo.HWnd != IntPtr.Zero)
                {
                    windowList.Add(windowInfo);
                    /*
                    windowList.Add(new WindowInfo
                    {
                        Title = process.MainWindowTitle,
                        StartTime = process.StartTime,
                        HWnd = hWnd
                    });
                    */
                }


                /*
                if (process.MainWindowHandle != IntPtr.Zero)
                //if (hWnd != IntPtr.Zero)
                {
                    //Console.WriteLine($"GetTaskbarWindows:LoopIN1 {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
                    windowList.Add(new WindowInfo
                    {
                        Title = process.MainWindowTitle,
                        StartTime = process.StartTime,
                        HWnd = process.MainWindowHandle
                    });
                    //Console.WriteLine($"GetTaskbarWindows:LoopIN2 {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
                }
                //Console.WriteLine($"GetTaskbarWindows:LoopNext {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
                */
            }

            // ここの処理は0.001秒以内
            // 開始日時で降順にソート
            windowList = windowList.OrderByDescending(w => w.StartTime).ToList();

            Console.WriteLine($"GetTaskbarWindows:Emd {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
            return windowList;
        }

        /*
        private static IntPtr GetMainWindowHandle(int processId)
        {
            IntPtr mainWindowHandle = IntPtr.Zero;
            EnumWindows((hWnd, lParam) =>
            {
                int windowProcessId;
                GetWindowThreadProcessId(hWnd, out windowProcessId);
                if (windowProcessId == processId && IsMainWindow(hWnd))
                {
                    mainWindowHandle = hWnd;
                    return false; // 停止
                }
                return true; // 続行
            }, IntPtr.Zero);
            return mainWindowHandle;
        }

        private static bool IsMainWindow(IntPtr hWnd)
        {
            return GetWindow(hWnd, GW_OWNER) == IntPtr.Zero && IsWindowVisible(hWnd);
        }
        */

        public static bool FocusWindow(IntPtr hWnd)
        {
            return SetForegroundWindow(hWnd);
        }

        // Windows API 関数の宣言
        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
        private const uint GW_OWNER = 4;

    }
}


