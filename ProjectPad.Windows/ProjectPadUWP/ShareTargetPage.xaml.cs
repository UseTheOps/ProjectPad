using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using ProjectPad.Business;
using Windows.ApplicationModel.Core;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace ProjectPadUWP
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class ShareTargetPage : Page
    {
        public ShareTargetPage()
        {
            this.InitializeComponent();
        }


        public List<RecentProject> RecentProjects { get; set; }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            CoreApplicationViewTitleBar coreTitleBar = AppTitleBar.InitHeaderBar();

            await ProjectPadApplication.Instance.RefreshRecent();
            var rct = ProjectPadApplication.Instance.RecentProjects;
            RecentProjects = rct;
            Bindings.Update();

            if (e.Parameter != null && e.Parameter is ShareOperation)
            {
                await HandleDataShare(e.Parameter as ShareOperation);
            }
            else
            {

            }

        }

        private async Task HandleDataShare(ShareOperation shareOperation)
        {
            if (shareOperation.Data.Contains(StandardDataFormats.WebLink))
            {
                var uri = await shareOperation.Data.GetWebLinkAsync();
                var data = await DataImporterHelper.InspectUrl(uri);

            }
        }
    }
}
