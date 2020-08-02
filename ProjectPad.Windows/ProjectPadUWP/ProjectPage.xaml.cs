using ProjectPad.Business;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.UserActivities;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
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
        DispatcherTimer _timerSave = null;

        public UserActivitySession _editProjectSession;

        public ProjectPage()
        {
            this.InitializeComponent();
            CurrentProject = null;
            _timerSave = new DispatcherTimer();
            _timerSave.Interval = TimeSpan.FromSeconds(5);
            _timerSave.Tick += _timerSave_Tick;
        }

        private async void _timerSave_Tick(object sender, object e)
        {
            if (this.CurrentProject.IsLocalCacheDirty)
            {
                _timerSave.Stop();
                await this.CurrentProject.SaveToLocalCache();
                _timerSave.Start();
            }
        }

        public ProjectViewModel CurrentProject { get; set; }

        public object NavigationViewPanePosition { get; private set; }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if(e.Parameter !=null && e.Parameter is ProjectViewModel)
            {
                var prj = e.Parameter as ProjectViewModel;
                this.DataContext = prj;
                this.CurrentProject = prj;
                Bindings.Update();
                await AddActivity(prj);
                _timerSave.Start();
            }

            var coreApp = CoreApplication.GetCurrentView();
            var coreTitleBar = coreApp.TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;
            
            // Set XAML element as a draggable region.
            Window.Current.SetTitleBar(AppTitleBar);
            Window.Current.Activated += Current_Activated;

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

        private async Task AddActivity(ProjectPad.Business.ProjectViewModel project)
        {
            var channel = UserActivityChannel.GetDefault();
            var activity = await channel.GetOrCreateUserActivityAsync("open:" + project.MetaData.Id);

            activity.ActivationUri = new Uri("useopsprjpad://projects/" + project.MetaData.Id);
            activity.IsRoamable = true;
            activity.VisualElements.DisplayText = project.MetaData.Name;
            activity.VisualElements.Description = LocalizationHelper.FormatActivityForProject(project);
            
            await activity.SaveAsync();

            _editProjectSession = activity.CreateSession();
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            _timerSave.Stop();

            if (_editProjectSession != null)
                _editProjectSession.Dispose();
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
            LeftPaddingColumn.Width = new GridLength(sender.SystemOverlayLeftInset);
            RightPaddingColumn.Width = new GridLength(sender.SystemOverlayRightInset);
            stackTitleInteractive.Margin = new Thickness(sender.SystemOverlayLeftInset, 0, sender.SystemOverlayRightInset, 0);
        }

        private void Current_Activated(object sender, Windows.UI.Core.WindowActivatedEventArgs e)
        {
            if (e.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
            {
                AppTitleBar.Opacity = 0.75;
                stackTitleInteractive.Opacity = 0.25;
                gridCommandBar.Opacity = 0.75;
            }
            else
            {
                AppTitleBar.Opacity = 1;
                stackTitleInteractive.Opacity = 1;
                gridCommandBar.Opacity = 1;
            }
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

            contentFrame.NavigateToType(pageType, this.CurrentProject, navOptions);
        }

        private async void btnClosePage_Click(object sender, RoutedEventArgs e)
        {
            await this.CurrentProject.SaveToLocalCache();
            this.Frame.Navigate(typeof(MainPage));
        }

        private void AddContentMenuItem_Clicked(object sender, RoutedEventArgs e)
        {
           
        }

        private async void btnSaveAs_Click(object sender, RoutedEventArgs e)
        {
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".prjpad" });
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = this.CurrentProject.MetaData.Name;

            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                CachedFileManager.DeferUpdates(file);
                using(var st = await file.OpenStreamForWriteAsync())
                    await this.CurrentProject.SaveToFile(st, file.Path);
                var status = await CachedFileManager.CompleteUpdatesAsync(file);

                if (!StorageApplicationPermissions.FutureAccessList.ContainsItem(this.CurrentProject.MetaData.Id))
                {
                    StorageApplicationPermissions.FutureAccessList.AddOrReplace(this.CurrentProject.MetaData.Id, file);
                }
            }
        }
    }
}
