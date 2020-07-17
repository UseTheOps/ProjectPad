using ProjectPad.Business;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
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
    public sealed partial class ProjectPage : Page
    {
        public ProjectPage()
        {
            this.InitializeComponent();
            CurrentProject = new ProjectViewModel()
            {
                MetaData = new ProjectViewModel.ProjectMetaData()
                {
                    Id = "--new--",
                    Name = "New"
                }
            };
        }

        public ProjectViewModel CurrentProject { get; set; }

        public object NavigationViewPanePosition { get; private set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if(e.Parameter !=null && e.Parameter is ProjectViewModel)
            {
                var prj = e.Parameter as ProjectViewModel;
                this.DataContext = prj;
                this.CurrentProject = prj;
                Bindings.Update();
            }

            var coreApp = CoreApplication.GetCurrentView();
            var coreTitleBar = coreApp.TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;
            
            // Set XAML element as a draggable region.
            Window.Current.SetTitleBar(AppTitleBar);

            var app = ApplicationView.GetForCurrentView();
            app.Title = this.CurrentProject?.MetaData.Name + "-";


            var titleBar = app.TitleBar;
            Color c = new Color() { R = 0x12, G = 0x58, B = 0x87 };
            titleBar.ButtonForegroundColor = Windows.UI.Colors.White;
            titleBar.ButtonBackgroundColor = c;

            Type pageType = typeof(Views.MainView);
            FrameNavigationOptions navOptions = new FrameNavigationOptions();
            navOptions.IsNavigationStackEnabled = false;
            contentFrame.NavigateToType(pageType, null, navOptions);
            nvMainPage.SelectedItem = nvMainPage.MenuItems[0];

            KeyboardAccelerator GoBack = new KeyboardAccelerator();
            GoBack.Key = VirtualKey.GoBack;
            GoBack.Invoked += GoBack_Invoked;
            SystemNavigationManager.GetForCurrentView().BackRequested += ProjectPage_BackRequested;
        }

        private void ProjectPage_BackRequested(object sender, BackRequestedEventArgs e)
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
            if (this.Frame.CanGoBack)
                this.Frame.GoBack();
        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            AppTitleBar.Height = sender.Height;
        }

        private void nvMainPage_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            FrameNavigationOptions navOptions = new FrameNavigationOptions();
            navOptions.TransitionInfoOverride = args.RecommendedNavigationTransitionInfo;
            navOptions.IsNavigationStackEnabled = false;

            var item = args.SelectedItem as Microsoft.UI.Xaml.Controls.NavigationViewItem;
            if (item == null)
                return;

            Type pageType = null;
            switch(item.Tag.ToString().ToLowerInvariant())
            {
                case "main":
                    pageType = typeof(Views.MainView);
                    break;
                case "wit":
                    pageType = typeof(Views.CompactWorkItemView);
                    break;
                default:
                    return;
            }

            if (contentFrame.Content != null && contentFrame.Content.GetType().Equals(pageType))
                return;

            contentFrame.NavigateToType(pageType, null, navOptions);
        }

        private async void btnClosePage_Click(object sender, RoutedEventArgs e)
        {
            await this.CurrentProject.Save();
            this.Frame.Navigate(typeof(MainPage));
        }
    }
}
