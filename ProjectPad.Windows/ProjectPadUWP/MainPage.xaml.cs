using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using MUXC = Microsoft.UI.Xaml.Controls;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ProjectPadUWP
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();



            this.DataContext = ProjectPad.Business.ProjectPadApplication.Instance;

        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            AppTitleBar.Height = sender.Height;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);


            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;
            // Set XAML element as a draggable region.
            Window.Current.SetTitleBar(AppTitleBar);

            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            Color c = new Color() { R = 0xF5, G = 0xF5, B = 0xF5 };
            titleBar.ButtonForegroundColor = Windows.UI.Colors.DimGray;
            titleBar.ButtonBackgroundColor = c;

            var t = ProjectPad.Business.ProjectPadApplication.Instance.HasToken;
            if (!t)
            {
                // starts animation for login
                StartNonConnected.Begin();
            }
            else
            {

            }
        }


        protected Visibility ToVisible(bool isVisible)
        {
            return isVisible ? Visibility.Visible : Visibility.Collapsed;
        }
        protected Visibility ToNoyVisible(bool isVisible)
        {
            return isVisible ? Visibility.Collapsed : Visibility.Visible;
        }

        public ProjectPad.Business.ProjectPadApplication DataObject
        {
            get { return ProjectPad.Business.ProjectPadApplication.Instance; }
        }

        private void btnConnexion_Click(object sender, RoutedEventArgs e)
        {
            ProjectPad.Business.ProjectPadApplication.Instance.TryConnect();
        }

        private void btnDeconnexion_Click(object sender, RoutedEventArgs e)
        {
            ProjectPad.Business.ProjectPadApplication.Instance.ClearConnections();
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ProjectPage));
        }
    }
}
