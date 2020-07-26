using ProjectPad.Business;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class NewProjectPage : Page
    {
        public class NewProjectData
        {
            public string Name { get; set; }
            public string Id { get; set; }
        }

        public NewProjectPage()
        {
            this.InitializeComponent();

            this.NewProjectItem = new NewProjectData()
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = "Nouveau projet"
            };
            this.DataContext = this.NewProjectItem;
        }

        public NewProjectData NewProjectItem { get; set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            CoreApplicationViewTitleBar coreTitleBar = AppTitleBar.InitHeaderBar();
        }

        private async void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(NewProjectItem.Name))
                return;

            var t = await ProjectPadApplication.Instance.CreateProject(NewProjectItem.Name, NewProjectItem.Id);
            this.Frame.Navigate(typeof(ProjectPage), t);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }
    }
}
