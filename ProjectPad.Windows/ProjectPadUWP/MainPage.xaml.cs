using ProjectPad.Business;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
            viewConnecte.Margin = new Thickness(0, sender.Height, 0, 0);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            CoreApplicationViewTitleBar coreTitleBar = AppTitleBar.InitHeaderBar();
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;


            // disabling back button
            var frame = this.Frame;
            frame.BackStack.Clear();

            var t = ProjectPad.Business.ProjectPadApplication.Instance.HasToken;
            if (!t)
            {
                // starts animation for login
                StartNonConnected.Begin();
            }
            else
            {

            }

            if (App.ActivatedProject != null)
            {
                string s = App.ActivatedProject;
                App.ActivatedProject = null;
                await OpenProject(s);
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

       

        private void SwipeItem_Invoked(SwipeItem sender, SwipeItemInvokedEventArgs args)
        {
          
        }

        private async void btnOpenProject_Click(object sender, RoutedEventArgs e)
        {

            var t = (sender as FrameworkElement).Tag;
            await OpenProject(t);
        }

        private async Task OpenProject(object t)
        {
            if (t != null)
            {
                var prj = await ProjectPadApplication.Instance.OpenProject(t as string);
                this.Frame.Navigate(typeof(ProjectPage), prj);

            }
        }

        private void bntSettings_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SettingsPage));
        }

        public bool HasMoreProjects(int? count)
        {
            return count.GetValueOrDefault(0) > 3;
        }
    }
}
