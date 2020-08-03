using ProjectPad.Business;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace ProjectPadUWP
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
            this.CurrentSettings = ProjectPadApplication.Instance;
        }

        private void btnDeconnexion_Click(object sender, RoutedEventArgs e)
        {
            ProjectPad.Business.ProjectPadApplication.Instance.ClearConnections();
            this.Frame.Navigate(typeof(MainPage));
        }

        public ProjectPadApplication CurrentSettings { get; set; }


        static SettingsPage()
        {
            Cultures = new AvailableCulture[]
            {
                new AvailableCulture() { Code ="en-US", Label = "English"},
                new AvailableCulture() { Code ="fr-FR", Label = "Français"},
            };
        }
        public static AvailableCulture[] Cultures { get; private set; }

        public class AvailableCulture
        {
            public string Code { get; set; }
            public string Label { get; set; }

            public override string ToString()
            {
                if (!string.IsNullOrEmpty(Code))
                    return $"{Label} ({Code})";
                else
                    return Label;
            }
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            CoreApplicationViewTitleBar coreTitleBar = AppTitleBar.InitHeaderBar();

            await RefreshCultureCombo();
            await RefreshAdoPanel();

            KeyboardAccelerator GoBack = new KeyboardAccelerator();
            GoBack.Key = VirtualKey.GoBack;
            GoBack.Invoked += GoBack_Invoked;
            SystemNavigationManager.GetForCurrentView().BackRequested += SettingsPage_BackRequested;
        }

        private async System.Threading.Tasks.Task RefreshCultureCombo()
        {
            cboLang.Items.Clear();
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();

            string lang = await ProjectPadApplication.Instance.GetPreferredCulture();

            cboLang.Items.Add(new AvailableCulture { Code = "", Label = resourceLoader.GetString("Text_CultureFromSystem") });
            foreach (var r in Cultures)
            {
                cboLang.Items.Add(r);
            }
            if (string.IsNullOrEmpty(lang))
                cboLang.SelectedIndex = 0;
            else
                cboLang.SelectedValue = lang;
        }

        private void SettingsPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            e.Handled = true;
            GoBack();
        }

        private void GoBack_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            args.Handled = true;
            GoBack();
        }

        private void GoBack()
        {
            if (this.Frame.CanGoBack && btnBack.IsEnabled)
                this.Frame.GoBack();
        }

        private async void cboLang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboLang.SelectedIndex < 0)
                return;

            var t = cboLang.SelectedValue as string;
            string lang = await ProjectPadApplication.Instance.GetPreferredCulture();
            bool hasChanged = false;

            if (!string.IsNullOrEmpty(t))
            {
                if (lang == null || !lang.Equals(t))
                    hasChanged = true;
            }
            else if (!string.IsNullOrEmpty(lang)) // lang was not null and should now be
                hasChanged = true;

            if (!hasChanged)
                return;

            // if changed, save & refresh display
            if (string.IsNullOrEmpty(t))
                await ProjectPadApplication.Instance.SetPreferredCulture(null);
            else
                await ProjectPadApplication.Instance.SetPreferredCulture(t);



            brdCultureChanged.Visibility = Visibility.Visible;
            btnBack.IsEnabled = false;
        }

        private async System.Threading.Tasks.Task RefreshAdoPanel()
        {
            bntAdoConnect.IsEnabled = false;
            var adoTP = new AzureDevOpsTokenProvider();
            if (await adoTP.HasSilentToken())
            {
                bntAdoConnect.Visibility = Visibility.Collapsed;
                txtAdoConnected.Visibility = Visibility.Visible;
                btnScrollToAzureDevops.IsEnabled = true;
                btnScrollToAzureDevopsLeft.IsEnabled = true;
            }
            else
            {
                bntAdoConnect.Visibility = Visibility.Visible;
                txtAdoConnected.Visibility = Visibility.Collapsed;
                bntAdoConnect.IsEnabled = true;
                btnScrollToAzureDevops.IsEnabled = false;
                btnScrollToAzureDevopsLeft.IsEnabled = false;
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            GoBack();
        }

        private async void btnClearCache_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            try
            {
                await ProjectPad.Business.ProjectPadApplication.Instance.ClearData(true);
                this.Frame.Navigate(typeof(MainPage));
            }
            finally
            {
                this.IsEnabled = true;
            }
        }

        private void btnScrollToClicked_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn != null && !string.IsNullOrEmpty(btn.Tag as string))
            {
                var elm = scrSettings.FindName(btn.Tag as string) as FrameworkElement;
                elm.StartBringIntoView();
            }
        }

        private async void bntAdoConnect_Click(object sender, RoutedEventArgs e)
        {
            bntAdoConnect.IsEnabled = false;
            try
            {
                var t = new AzureDevOpsTokenProvider();
                var tk = await t.GetToken();
                await RefreshAdoPanel();
            }
            finally
            {
                bntAdoConnect.IsEnabled = true;
            }
        }
    }
}
