using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace PyLibSharp.Win32Friendly
{
    public static class WindowApi
    {
        public static Exception lastError;

        public enum MouseButton
        {
            Left   = 0,
            Middle = 1,
            Right  = 2
        }

        public static class InternalConst
        {
            private const uint SWP_NOSIZE   = 0x1; //忽略 cx、cy, 保持大小
            private const uint SWP_NOMOVE   = 0x2; //忽略 X、Y, 不改变位置
            private const uint SWP_NOZORDER = 0x4; //忽略 hWndInsertAfter, 保持 Z 顺序

            public const uint WM_MOUSEMOVE = 0x200;

            public const uint WM_LBUTTONDOWN   = 0x201;
            public const uint WM_LBUTTONUP     = 0x202;
            public const uint WM_LBUTTONDBLCLK = 0x203;

            public const uint WM_RBUTTONDOWN   = 0x204;
            public const uint WM_RBUTTONUP     = 0x205;
            public const uint WM_RBUTTONDBLCLK = 0x206;

            public const uint WM_MBUTTONDOWN   = 0x207;
            public const uint WM_MBUTTONUP     = 0x208;
            public const uint WM_MBUTTONDBLCLK = 0x209;

            public const uint WM_CLOSE = 0x010;

            private const int VK_RETURN    = 0x0D;
            private const int BM_CLICK     = 0xF5;
            private const int GW_HWNDFIRST = 0;
            private const int GW_HWNDNEXT  = 2;
            private const int GWL_STYLE    = (-16);
            private const int WS_VISIBLE   = 268435456;
            private const int WS_BORDER    = 8388608;
        }


        public static class InternalApi
        {
            public delegate bool WndEnumProc(IntPtr hWnd, int lParam);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool EnumWindows(WndEnumProc lpEnumFunc, int lParam);

            [DllImport("user32.dll")]
            public static extern int EnumChildWindows(IntPtr hWndParent, WndEnumProc lpfn, int lParam);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern int GetWindowRect(IntPtr hwnd, out Rect lpRect);

            [DllImport("user32.dll", SetLastError = true, EntryPoint = "FindWindowW", CharSet = CharSet.Unicode)]
            public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

            [DllImport("user32.dll", SetLastError = true, EntryPoint = "FindWindowExW", CharSet = CharSet.Unicode)]
            public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass,
                                                     string lpszWindow);

            [DllImport("user32", SetLastError = true)]
            public static extern bool PostMessage(
                IntPtr hWnd,
                uint Msg,
                int wParam,
                int lParam
            );

            [DllImport("user32.dll", EntryPoint = "SendMessageA", SetLastError = true)]
            public static extern int SendMessage(IntPtr hwnd, uint wMsg, int wParam, int lParam);

            [DllImport("user32.dll", SetLastError = true, EntryPoint = "GetWindowTextW", CharSet = CharSet.Unicode)]
            public static extern int GetWindowText(IntPtr hWnd,
                                                   [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpString,
                                                   int nMaxCount);

            [DllImport("user32.dll", SetLastError = true, EntryPoint = "GetClassNameW", CharSet = CharSet.Unicode)]
            public static extern int GetClassName(IntPtr hWnd,
                                                  [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpString,
                                                  int nMaxCount);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern IntPtr GetForegroundWindow();

            [DllImport("user32.dll", SetLastError = true)]
            public static extern int GetWindow(int hWnd, int wCmd);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern int GetWindowLongA(int hWnd, int wIndx);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern int GetWindowTextLength(IntPtr hWnd);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern int GetWindowThreadProcessId(IntPtr hWnd, ref int lpdwProcessId);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool IsWindowVisible(IntPtr hWnd);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndlnsertAfter, int X, int Y, int cx, int cy,
                                                   uint Flags);

            public struct Rect
            {
                public int Left;
                public int Top;
                public int Right;
                public int Bottom;
            }

            public struct WindowInfo
            {
                public IntPtr hWnd;
                public string szWindowName;
                public string szClassName;
                public string currStatus;
            }
        }

        public class Win32Window : IEnumerable<Win32Window>
        {
            private StringBuilder _buffer;

            /// <summary>
            /// 该窗体的标题文本
            /// </summary>
            public string WindowText { get; set; }

            /// <summary>
            /// 该窗体的类名字符串
            /// </summary>
            public string ClassName { get; set; }

            /// <summary>
            /// 该窗体的坐标和大小
            /// <para>坐标取的是窗体左上角的坐标</para>
            /// </summary>
            public Rectangle WindowPosition { get; set; }

            private IntPtr _internalPtr;

            public IntPtr Handler
            {
                get => _internalPtr;
                set
                {
                    if (ApiOptions.IsThrowErrorWhenHandlerIsNull &&
                        value == IntPtr.Zero) throw new ArgumentNullException(nameof(value), "该句柄无效或未找到对应窗口");

                    try
                    {
                        _internalPtr = value;
                        bool isSuccGetText = InternalApi.GetWindowText(value, _buffer, 255) > 0;
                        if (isSuccGetText)
                            WindowText = _buffer.ToString();
                        else
                            WindowText = "";

                        bool isSuccGetClass = InternalApi.GetClassName(value, _buffer, 255) > 0;
                        if (isSuccGetClass)
                            ClassName = _buffer.ToString();
                        else
                            ClassName = "";

                        bool isSuccGetRect = InternalApi.GetWindowRect(value, out InternalApi.Rect rect) > 0;
                        if (isSuccGetRect)
                        {
                            WindowPosition = new Rectangle(new Point(rect.Left, rect.Top),
                                                           new Size(rect.Right - rect.Left, rect.Bottom - rect.Top));
                        }
                        else
                        {
                            WindowPosition = new Rectangle();
                        }
                    }
                    catch (Exception eX)
                    {
                        lastError = eX;
                    }
                }
            }

            private void initBuffer(StringBuilder sbBuffer)
            {
                if (sbBuffer is null) _buffer = new StringBuilder(256);
                else _buffer                  = sbBuffer;
            }

            public Win32Window(IntPtr hWnd, StringBuilder sbBuffer = null)
            {
                initBuffer(sbBuffer);
                Handler = hWnd;
            }

            public Win32Window(string sWindowText, StringBuilder sbBuffer = null)
            {
                initBuffer(sbBuffer);
                Handler = InternalApi.FindWindow(null, sWindowText);
            }

            public Win32Window(string sWindowText, string sClassName, StringBuilder sbBuffer = null)
            {
                initBuffer(sbBuffer);
                Handler = InternalApi.FindWindow(sClassName, sWindowText);
            }

            public void Click(MouseButton btn = MouseButton.Left)
            {
                uint eMove  = InternalConst.WM_MOUSEMOVE;
                uint eMDown = 0, eMClk = 0, eMUp = 0;
                switch (btn)
                {
                    case MouseButton.Left:
                        eMDown = InternalConst.WM_LBUTTONDOWN;
                        eMClk  = InternalConst.WM_LBUTTONDBLCLK;
                        eMUp   = InternalConst.WM_LBUTTONUP;
                        break;
                    case MouseButton.Middle:
                        eMDown = InternalConst.WM_MBUTTONDOWN;
                        eMClk  = InternalConst.WM_MBUTTONDBLCLK;
                        eMUp   = InternalConst.WM_MBUTTONUP;
                        break;
                    case MouseButton.Right:
                        eMDown = InternalConst.WM_RBUTTONDOWN;
                        eMClk  = InternalConst.WM_RBUTTONDBLCLK;
                        eMUp   = InternalConst.WM_RBUTTONUP;
                        break;
                }

                InternalApi.PostMessage(Handler, eMDown, 0x2, 1 + 1 * 65536);
                InternalApi.PostMessage(Handler, eMClk, 0x1, 1  + 1 * 65536);
                InternalApi.PostMessage(Handler, eMUp, 0x0, 1   + 1 * 65536);
            }

            public void Close()
            {
                InternalApi.PostMessage(Handler, InternalConst.WM_CLOSE, 0, 0);
            }

            public IEnumerator<Win32Window> GetEnumerator()
            {
                //已经访问过的句柄，不再重复访问
                List<IntPtr> lstVisitedHandlers = new List<IntPtr>();

                //先遍历一遍所有句柄，预筛选
                InternalApi.EnumChildWindows(Handler, (h, l) =>
                                                      {
                                                          //窗口不可见
                                                          if (ApiOptions.IsSkipInvisibleWindow &&
                                                              !InternalApi.IsWindowVisible(h)) return true;

                                                          //如果已经访问过，则看下一个
                                                          if (lstVisitedHandlers.Contains(h)) return true;
                                                          lstVisitedHandlers.Add(h);

                                                          return true;
                                                      }, 0);

                StringBuilder buffer = new StringBuilder(256);
                //最后再次筛选，并获取对应属性
                foreach (IntPtr hWnd in lstVisitedHandlers)
                {
                    Win32Window currentWindow = new Win32Window(hWnd, buffer);

                    if (ApiOptions.IsSkipEmptyWindowText && currentWindow.WindowText == "")
                        continue;

                    if (ApiOptions.IsSkipEmptyWindowClass && currentWindow.ClassName == "")
                        continue;

                    Debug.Print(currentWindow.WindowText);
                    yield return currentWindow;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public static class ApiOptions
        {
            public static bool IsSkipEmptyWindowClass        { get; set; } = true;
            public static bool IsSkipEmptyWindowText         { get; set; } = true;
            public static bool IsSkipInvisibleWindow         { get; set; } = true;
            public static bool IsThrowErrorWhenHandlerIsNull { get; set; } = true;
        }

        /// <summary>
        /// 根据指定窗体属性来获取第一个符合条件的子窗体对象
        /// </summary>
        /// <param name="wParent"></param>
        /// <param name="sWindowText"></param>
        /// <param name="sClassName"></param>
        /// <returns></returns>
        public static Win32Window GetSubWindowByProps(this Win32Window wParent, string sWindowText = null,
                                                      string sClassName = null)
            => GetWindowByProps(wParent.Handler, sWindowText, sClassName);


        /// <summary>
        /// 根据指定窗体属性来获取第一个符合条件的窗体对象
        /// </summary>
        /// <param name="hParent"></param>
        /// <param name="sWindowText"></param>
        /// <param name="sClassName"></param>
        /// <returns></returns>
        public static Win32Window GetWindowByProps(IntPtr hParent, string sWindowText = null,
                                                   string sClassName = null)
        {
            IntPtr retPtr = IntPtr.Zero;
            retPtr =
                InternalApi.FindWindowEx(hParent, IntPtr.Zero, sClassName, sWindowText);

            return new Win32Window(retPtr);
        }


        /// <summary>
        /// 根据指定窗体属性来遍历符合条件的子窗体对象
        /// </summary>
        /// <param name="wParent"></param>
        /// <param name="sWindowText"></param>
        /// <param name="sClassName"></param>
        /// <returns></returns>
        public static IEnumerable<Win32Window> GetSubWindowsByProps(this Win32Window wParent, string sWindowText = null,
                                                                    string sClassName = null)
            => GetWindowsByProps(wParent.Handler, sWindowText, sClassName);


        /// <summary>
        /// 根据指定窗体属性来遍历符合条件的窗体对象
        /// </summary>
        /// <param name="hParent"></param>
        /// <param name="sWindowText"></param>
        /// <param name="sClassName"></param>
        /// <returns></returns>
        public static IEnumerable<Win32Window> GetWindowsByProps(IntPtr hParent, string sWindowText = null,
                                                                 string sClassName = null)
        {
            //预留buffer，节约时间和空间
            StringBuilder buffer     = new StringBuilder(256);
            IntPtr        lastPtr    = IntPtr.Zero;
            IntPtr        retPtr     = IntPtr.Zero;
            List<IntPtr>  visitedPtr = new List<IntPtr>();
            do
            {
                retPtr =
                    InternalApi.FindWindowEx(hParent, lastPtr, sClassName, sWindowText);
                if (visitedPtr.Contains(retPtr)) continue;
                visitedPtr.Add(retPtr);

                lastPtr = retPtr;
                //窗口文本空
                if (ApiOptions.IsSkipEmptyWindowText && sWindowText == "")
                {
                    continue;
                }

                //窗口类名空
                if (ApiOptions.IsSkipEmptyWindowClass && sClassName == "")
                {
                    continue;
                }

                //窗口不可见
                if (ApiOptions.IsSkipInvisibleWindow && !InternalApi.IsWindowVisible(retPtr))
                {
                    continue;
                }

                yield return new Win32Window(retPtr, buffer);
            } while (retPtr != IntPtr.Zero);
        }
    }
}