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
using Windows.UI.Xaml.Navigation;

// Pour en savoir plus sur le modèle d'élément Contrôle utilisateur, consultez la page https://go.microsoft.com/fwlink/?LinkId=234236

namespace ProjectPadUWP
{
    public sealed partial class BasicHeader : UserControl
    {
        public BasicHeader()
        {
            this.InitializeComponent();
        }

        public CoreApplicationViewTitleBar InitHeaderBar()
        {
            Window.Current.SetTitleBar(this);

            // changing title bar to custom
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            Window.Current.Activated += Current_Activated;
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;
            coreTitleBar.IsVisibleChanged += CoreTitleBar_IsVisibleChanged;

            coreTitleBar.IsVisibleChanged += CoreTitleBar_IsVisibleChanged1;

            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            Color c = new Color() { R = 0xF5, G = 0xF5, B = 0xF5 };
            
            titleBar.ButtonForegroundColor = Windows.UI.Colors.DimGray;
            titleBar.ButtonBackgroundColor = c;
            titleBar.ButtonInactiveForegroundColor = Windows.UI.Colors.DimGray;
            titleBar.ButtonInactiveBackgroundColor = c;
            return coreTitleBar;
        }

        private void CoreTitleBar_IsVisibleChanged1(CoreApplicationViewTitleBar sender, object args)
        {
            this.Visibility = sender.IsVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Current_Activated(object sender, Windows.UI.Core.WindowActivatedEventArgs e)
        {
            if (e.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
            {
                AppTitleBar.Opacity = 0.25;
            }
            else
            {
                AppTitleBar.Opacity = 1;
            }
        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            //AppTitleBar.Height = sender.Height;
            

            LeftPaddingColumn.Width = new GridLength(sender.SystemOverlayLeftInset);
            RightPaddingColumn.Width = new GridLength(sender.SystemOverlayRightInset);
            pnlTitre.Margin = new Thickness(sender.SystemOverlayLeftInset, 0, sender.SystemOverlayRightInset, 0);
        }

        private void CoreTitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {
            if (sender.IsVisible)
            {
                AppTitleBar.Visibility = Visibility.Visible;
            }
            else
            {
                AppTitleBar.Visibility = Visibility.Collapsed;
            }
        }
    }
}
