using System;
using Caliburn.Micro;

namespace BonusBits.CodeSamples.WP7
{
    public sealed class MainPageViewModel : PropertyChangedBase
    {
        private readonly INavigationService m_navigator;
       
        /// <summary>
        /// Initializes a new instance of the <see cref="MainPageViewModel"/> class.
        /// </summary>
        public MainPageViewModel(INavigationService navigator)
        {
            // INavigationService is available to us using Constructor Injection.
            m_navigator = navigator;
        }

        /// <summary>
        /// Navigates to threading page.
        /// </summary>
        public void NavigateToThreadingPage()
        {
            m_navigator.Navigate(new Uri("/ThreadingPage.xaml", UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Navigates to sterling page.
        /// </summary>
        public void NavigateToSterlingPage()
        {
            m_navigator.Navigate(new Uri("/SterlingPage.xaml", UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Navigates to sterling extensions page.
        /// </summary>
        public void NavigateToSterlingExtensionsPage()
        {
            m_navigator.Navigate(new Uri("/SterlingExtensionsPage.xaml", UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Navigates to validation in code page.
        /// </summary>
        public void NavigateToCodeOnlyPage()
        {
            m_navigator.Navigate(new Uri("/CodeOnlyPageView.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}
