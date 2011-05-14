using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Shapes;

namespace BonusBits.CodeSamples.WPF.Retro1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    internal partial class MainWindow : Window
    {
        private const Int32 WM_SYSCOMMAND = 0x112;

        private static readonly TimeSpan s_doubleClick
            = TimeSpan.FromMilliseconds(500);

        private HwndSource m_hwndSource;
        private DateTime   m_headerLastClicked;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event. 
        /// This method is invoked whenever 
        /// <see cref="P:System.Windows.FrameworkElement.IsInitialized"/> is set to true internally.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> 
        /// that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            AllowsTransparency    = false;
            ResizeMode            = ResizeMode.NoResize;
            Height                = 480;
            Width                 = 852;  
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            WindowStyle           = WindowStyle.None;
            
            SourceInitialized    += HandleSourceInitialized;

            GotKeyboardFocus     += HandleGotKeyboardFocus;
            LostKeyboardFocus    += HandleLostKeyboardFocus;

            base.OnInitialized(e);
        }

        /// <summary>
        /// Handles the source initialized.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> 
        /// instance containing the event data.</param>
        private void HandleSourceInitialized(Object sender, EventArgs e)
        {
            m_hwndSource = (HwndSource)PresentationSource.FromVisual(this);

            // Returns the HwndSource object for the window
            // which presents WPF content in a Win32 window.
            HwndSource.FromHwnd(m_hwndSource.Handle).AddHook(
                new HwndSourceHook(NativeMethods.WindowProc));
            
            // http://msdn.microsoft.com/en-us/library/aa969524(VS.85).aspx
            Int32 DWMWA_NCRENDERING_POLICY = 2;
            NativeMethods.DwmSetWindowAttribute(
                m_hwndSource.Handle,
                DWMWA_NCRENDERING_POLICY,
                ref DWMWA_NCRENDERING_POLICY,
                4);

            // http://msdn.microsoft.com/en-us/library/aa969512(VS.85).aspx
            NativeMethods.ShowShadowUnderWindow(m_hwndSource.Handle);
        }
        
        /// <summary>
        /// Handles the preview mouse move.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> 
        /// instance containing the event data.</param>
        [DebuggerStepThrough]
        private void HandlePreviewMouseMove(Object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton != MouseButtonState.Pressed)
            {
                Cursor = Cursors.Arrow;
            }
        }

        /// <summary>
        /// Handles the header preview mouse down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/>
        /// instance containing the event data.</param>
        private void HandleHeaderPreviewMouseDown(Object sender, MouseButtonEventArgs e)
        {
            
            if (DateTime.Now.Subtract(m_headerLastClicked) <= s_doubleClick)
            {
                // Execute the code inside the event handler for the 
                // restore button click passing null for the sender
                // and null for the event args.
                HandleRestoreClick(null, null);
            }

            m_headerLastClicked = DateTime.Now;

            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        /// <summary>
        /// Handles the minimize click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> 
        /// instance containing the event data.</param>
        private void HandleMinimizeClick(Object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Handles the restore click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> 
        /// instance containing the event data.</param>
        private void HandleRestoreClick(Object sender, RoutedEventArgs e)
        {
            WindowState = (WindowState == WindowState.Normal) 
                ? WindowState.Maximized : WindowState.Normal;

            m_frameGrid.IsHitTestVisible
                = WindowState == WindowState.Maximized
                ? false : true;

            m_resize.Visibility = (WindowState == WindowState.Maximized) 
                ? Visibility.Hidden : Visibility.Visible;

            m_roundBorder.Visibility = (WindowState == WindowState.Maximized)
                ? Visibility.Hidden : Visibility.Visible;
        }

        /// <summary>
        /// Handles the got keyboard focus.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyboardFocusChangedEventArgs"/>
        /// instance containing the event data.</param>
        public void HandleGotKeyboardFocus(Object sender, KeyboardFocusChangedEventArgs e)
        {
            m_roundBorder.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Handles the lost keyboard focus.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyboardFocusChangedEventArgs"/>
        /// instance containing the event data.</param>
        public void HandleLostKeyboardFocus(Object sender, KeyboardFocusChangedEventArgs e)
        {
            m_roundBorder.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Handles the close click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> 
        /// instance containing the event data.</param>
        private void HandleCloseClick(Object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Handles the rectangle mouse move.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> 
        /// instance containing the event data.</param>
        private void HandleRectangleMouseMove(Object sender, MouseEventArgs e)
        {
            Rectangle clickedRectangle = (Rectangle)sender;

            switch (clickedRectangle.Name)
            {
                case "top":
                    Cursor = Cursors.SizeNS;
                    break;
                case "bottom":
                    Cursor = Cursors.SizeNS;
                    break;
                case "left":
                    Cursor = Cursors.SizeWE;
                    break;
                case "right":
                    Cursor = Cursors.SizeWE;
                    break;
                case "topLeft":
                    Cursor = Cursors.SizeNWSE;
                    break;
                case "topRight":
                    Cursor = Cursors.SizeNESW;
                    break;
                case "bottomLeft":
                    Cursor = Cursors.SizeNESW;
                    break;
                case "bottomRight":
                    Cursor = Cursors.SizeNWSE;
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Handles the rectangle preview mouse down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> 
        /// instance containing the event data.</param>
        private void HandleRectanglePreviewMouseDown(Object sender, MouseButtonEventArgs e)
        {
            Rectangle clickedRectangle = (Rectangle)sender;

            switch (clickedRectangle.Name)
            {
                case "top":
                    Cursor = Cursors.SizeNS;
                    ResizeWindow(ResizeDirection.Top);
                    break;
                case "bottom":
                    Cursor = Cursors.SizeNS;
                    ResizeWindow(ResizeDirection.Bottom);
                    break;
                case "left":
                    Cursor = Cursors.SizeWE;
                    ResizeWindow(ResizeDirection.Left);
                    break;
                case "right":
                    Cursor = Cursors.SizeWE;
                    ResizeWindow(ResizeDirection.Right);
                    break;
                case "topLeft":
                    Cursor = Cursors.SizeNWSE;
                    ResizeWindow(ResizeDirection.TopLeft);
                    break;
                case "topRight":
                    Cursor = Cursors.SizeNESW;
                    ResizeWindow(ResizeDirection.TopRight);
                    break;
                case "bottomLeft":
                    Cursor = Cursors.SizeNESW;
                    ResizeWindow(ResizeDirection.BottomLeft);
                    break;
                case "bottomRight":
                    Cursor = Cursors.SizeNWSE;
                    ResizeWindow(ResizeDirection.BottomRight);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Resizes the window.
        /// </summary>
        /// <param name="direction">The direction.</param>
        private void ResizeWindow(ResizeDirection direction)
        {
            NativeMethods.SendMessage(m_hwndSource.Handle, WM_SYSCOMMAND, 
                (IntPtr)(61440 + direction), IntPtr.Zero);
        }

        public enum ResizeDirection
        {
            Left        = 1,
            Right       = 2,
            Top         = 3,
            TopLeft     = 4,
            TopRight    = 5,
            Bottom      = 6,
            BottomLeft  = 7,
            BottomRight = 8,
        }

        private sealed class NativeMethods
        {
            [DllImport("dwmapi.dll", PreserveSig = true)]
            internal static extern Int32 DwmSetWindowAttribute(
                IntPtr hwnd,
                Int32 attr,
                ref Int32 attrValue,
                Int32 attrSize);

            [DllImport("dwmapi.dll")]
            internal static extern Int32 DwmExtendFrameIntoClientArea(
                IntPtr hWnd, 
                ref MARGINS pMarInset);

            [DllImport("user32")]
            internal static extern Boolean GetMonitorInfo(
                IntPtr hMonitor, 
                MONITORINFO lpmi);

            [DllImport("User32")]
            internal static extern IntPtr MonitorFromWindow(
                IntPtr handle, 
                Int32 flags);

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            internal static extern IntPtr SendMessage(
                IntPtr hWnd, 
                UInt32 msg, 
                IntPtr wParam, 
                IntPtr lParam);

            [DebuggerStepThrough]
            internal static IntPtr WindowProc(
                IntPtr hwnd, 
                Int32  msg, 
                IntPtr wParam,
                IntPtr lParam, 
                ref Boolean handled)
            {
                switch (msg)
                {
                    case 0x0024:
                        WmGetMinMaxInfo(hwnd, lParam);
                        handled = true;
                        break;
                }

                return (IntPtr)0;
            }

            internal static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
            {
                MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

                // Adjust the maximized size and position to fit the work area 
                // of the correct monitor.
                Int32 MONITOR_DEFAULTTONEAREST = 0x00000002;

                IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
                if (monitor != IntPtr.Zero)
                {
                    MONITORINFO monitorInfo = new MONITORINFO();
                    GetMonitorInfo(monitor, monitorInfo);

                    RECT rcWorkArea     = monitorInfo.m_rcWork;
                    RECT rcMonitorArea  = monitorInfo.m_rcMonitor;

                    mmi.m_ptMaxPosition.m_x = Math.Abs(rcWorkArea.m_left - rcMonitorArea.m_left);
                    mmi.m_ptMaxPosition.m_y = Math.Abs(rcWorkArea.m_top  - rcMonitorArea.m_top);

                    mmi.m_ptMaxSize.m_x = Math.Abs(rcWorkArea.m_right  - rcWorkArea.m_left);
                    mmi.m_ptMaxSize.m_y = Math.Abs(rcWorkArea.m_bottom - rcWorkArea.m_top);
                }

                Marshal.StructureToPtr(mmi, lParam, true);
            }

            internal static void ShowShadowUnderWindow(IntPtr intPtr)
            {
                MARGINS marInset        = new MARGINS();
                marInset.m_bottomHeight = -1;
                marInset.m_leftWidth    = -1;
                marInset.m_rightWidth   = -1;
                marInset.m_topHeight    = -1;

                DwmExtendFrameIntoClientArea(intPtr, ref marInset);
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
            internal sealed class MONITORINFO
            {
                public Int32 m_cbSize;
                public RECT  m_rcMonitor;
                public RECT  m_rcWork;
                public Int32 m_dwFlags;

                public MONITORINFO()
                {
                    m_cbSize    = Marshal.SizeOf(typeof(MONITORINFO));
                    m_rcMonitor = new RECT();
                    m_rcWork    = new RECT();
                    m_dwFlags   = 0;
                }
            }

            [StructLayout(LayoutKind.Sequential, Pack = 0)]
            internal struct RECT
            {
                public static readonly RECT Empty = new RECT();

                public Int32 m_left;
                public Int32 m_top;
                public Int32 m_right;
                public Int32 m_bottom;

                public RECT(Int32 left, Int32 top, Int32 right, Int32 bottom)
                {
                    m_left   = left;
                    m_top    = top;
                    m_right  = right;
                    m_bottom = bottom;
                }

                public RECT(RECT rcSrc)
                {
                    m_left   = rcSrc.m_left;
                    m_top    = rcSrc.m_top;
                    m_right  = rcSrc.m_right;
                    m_bottom = rcSrc.m_bottom;
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            internal struct MARGINS
            {
                public Int32 m_leftWidth;
                public Int32 m_rightWidth;
                public Int32 m_topHeight;
                public Int32 m_bottomHeight;
            }

            [StructLayout(LayoutKind.Sequential)]
            private struct POINT
            {
                public Int32 m_x;
                public Int32 m_y;

                public POINT(Int32 x, Int32 y)
                {
                    m_x = x;
                    m_y = y;
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            private struct MINMAXINFO
            {
                public POINT m_ptReserved;
                public POINT m_ptMaxSize;
                public POINT m_ptMaxPosition;
                public POINT m_ptMinTrackSize;
                public POINT m_ptMaxTrackSize;
            };
        }
    }
}
