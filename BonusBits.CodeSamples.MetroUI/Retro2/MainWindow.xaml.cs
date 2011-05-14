using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BonusBits.CodeSamples.WPF.Retro2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    internal partial class MainWindow : Window
    {
        private const Int32 WM_SYSCOMMAND = 0x112;
        private const Int32 c_edgeWndSize = 23;

        private static readonly TimeSpan s_doubleClick
            = TimeSpan.FromMilliseconds(500);

        private Window m_wndT;
        private Window m_wndL;
        private Window m_wndB;
        private Window m_wndR;

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
            AllowsTransparency    = true;
            Height                = 480;
            Width                 = 852;  
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            WindowStyle           = WindowStyle.None;

            LocationChanged += HandleLocationChanged;
            SizeChanged     += HandleLocationChanged;
            StateChanged    += HandleWndStateChanged;

            GotKeyboardFocus  += HandleGotKeyboardFocus;
            LostKeyboardFocus += HandleLostKeyboardFocus;

            InitializeSurrounds();
            ShowSurrounds();
            
            base.OnInitialized(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Window.SourceInitialized"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.
        /// </param>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            m_hwndSource = (HwndSource)PresentationSource.FromVisual(this);

            // Returns the HwndSource object for the window
            // which presents WPF content in a Win32 window.
            HwndSource.FromHwnd(m_hwndSource.Handle).AddHook(
                new HwndSourceHook(NativeMethods.WindowProc));
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Window.Closed"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnClosed(EventArgs e)
        {
            CloseSurrounds();
            base.OnClosed(e);
        }

        /// <summary>
        /// Handles the location changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> 
        /// instance containing the event data.</param>
        private void HandleLocationChanged(Object sender, EventArgs e)
        {
            m_wndT.Left   = Left  - c_edgeWndSize;
            m_wndT.Top    = Top   - m_wndT.Height;
            m_wndT.Width  = Width + c_edgeWndSize * 2;
            m_wndT.Height = c_edgeWndSize;

            m_wndL.Left   = Left - m_wndL.Width;
            m_wndL.Top    = Top;
            m_wndL.Width  = c_edgeWndSize;
            m_wndL.Height = Height;

            m_wndB.Left   = Left  - c_edgeWndSize;
            m_wndB.Top    = Top   + Height;
            m_wndB.Width  = Width + c_edgeWndSize * 2;
            m_wndB.Height = c_edgeWndSize;

            m_wndR.Left   = Left + Width;
            m_wndR.Top    = Top;
            m_wndR.Width  = c_edgeWndSize;
            m_wndR.Height = Height;
        }
        
        /// <summary>
        /// Handles the windows state changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/>
        /// instance containing the event data.</param>
        private void HandleWndStateChanged(Object sender, EventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                ShowSurrounds();
            }
            else
            {
                HideSurrounds();
            }
        }

        /// <summary>
        /// Initializes the surrounding windows.
        /// </summary>
        private void InitializeSurrounds()
        {
            // Top.
            m_wndT = CreateTransparentWindow();
            
            // Left.
            m_wndL = CreateTransparentWindow();

            // Bottom.
            m_wndB = CreateTransparentWindow();

            // Right.
            m_wndR = CreateTransparentWindow();

            SetSurroundShadows();
        }

        /// <summary>
        /// Creates an empty window.
        /// </summary>
        /// <returns></returns>
        private static Window CreateTransparentWindow()
        {
            Window wnd             = new Window();
            wnd.AllowsTransparency = true;
            wnd.ShowInTaskbar      = false;
            wnd.WindowStyle        = WindowStyle.None;
            wnd.Background         = null;
            
            return wnd;
        }

        /// <summary>
        /// Sets the artificial drop shadow.
        /// </summary>
        /// <param name="active">if set to <c>true</c> [active].</param>
        private void SetSurroundShadows(Boolean active = true)
        {
            if (active)
            {
                Double cornerRadius = 1.75;

                m_wndT.Content = GetDecorator("Images/ACTIVESHADOWTOP.PNG");
                m_wndL.Content = GetDecorator("Images/ACTIVESHADOWLEFT.PNG",  cornerRadius);
                m_wndB.Content = GetDecorator("Images/ACTIVESHADOWBOTTOM.PNG");
                m_wndR.Content = GetDecorator("Images/ACTIVESHADOWRIGHT.PNG", cornerRadius);
            }
            else
            {
                m_wndT.Content = GetDecorator("Images/INACTIVESHADOWTOP.PNG");
                m_wndL.Content = GetDecorator("Images/INACTIVESHADOWLEFT.PNG");
                m_wndB.Content = GetDecorator("Images/INACTIVESHADOWBOTTOM.PNG");
                m_wndR.Content = GetDecorator("Images/INACTIVESHADOWRIGHT.PNG");
            }
        }

        [DebuggerStepThrough]
        private Decorator GetDecorator(String imageUri, Double radius = 0)
        {
            Border border       = new Border();
            border.CornerRadius = new CornerRadius(radius);
            border.Background   = new ImageBrush(
                new BitmapImage(
                    new Uri(BaseUriHelper.GetBaseUri(this),
                        imageUri)));
            
            return border;
        }

        /// <summary>
        /// Shows the surrounding windows.
        /// </summary>
        private void ShowSurrounds()
        {
            m_wndT.Show();
            m_wndL.Show();
            m_wndB.Show();
            m_wndR.Show();
        }

        /// <summary>
        /// Hides the surrounding windows.
        /// </summary>
        private void HideSurrounds()
        {
            m_wndT.Hide();
            m_wndL.Hide();
            m_wndB.Hide();
            m_wndR.Hide();
        }
        /// <summary>
        /// Closes the surrounding windows.
        /// </summary>
        private void CloseSurrounds()
        {
            m_wndT.Close();
            m_wndL.Close();
            m_wndB.Close();
            m_wndR.Close();
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
            SetSurroundShadows(true);
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
            SetSurroundShadows(false);
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
            internal static IntPtr WindowProc(IntPtr hwnd, Int32 msg, IntPtr wParam,
                IntPtr lParam, ref Boolean handled)
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