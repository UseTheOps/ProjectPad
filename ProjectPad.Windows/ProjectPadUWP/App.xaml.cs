using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.UserActivities;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace ProjectPadUWP
{
    /// <summary>
    /// Fournit un comportement spécifique à l'application afin de compléter la classe Application par défaut.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initialise l'objet d'application de singleton.  Il s'agit de la première ligne du code créé
        /// à être exécutée. Elle correspond donc à l'équivalent logique de main() ou WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        protected async override void OnActivated(IActivatedEventArgs e)
        {

            Window.Current.Activate();

            if (e.Kind == ActivationKind.Protocol)
            {
                ProtocolActivatedEventArgs eventArgs = e as ProtocolActivatedEventArgs;
                string name = eventArgs.Uri.AbsolutePath;
                name = name.Substring(name.LastIndexOf("/") + 1);

                Frame rootFrame = Window.Current.Content as Frame;
                var app = ProjectPad.Business.ProjectPadApplication.Instance;

                if(rootFrame==null)
                {
                    await StartApp(e);
                    rootFrame = Window.Current.Content as Frame;
                }

                if (rootFrame != null && app != null && rootFrame.Content != null && !(rootFrame.Content is SplashScreen))
                {
                    var prj = await ProjectPad.Business.ProjectPadApplication.Instance.OpenProject(name);
                    rootFrame.Navigate(typeof(ProjectPage), prj);
                }
                else
                {
                    ActivatedProject = name;
                }
            }

        }

        public static string ActivatedProject { get; set; }

        /// <summary>
        /// Invoqué lorsque l'application est lancée normalement par l'utilisateur final.  D'autres points d'entrée
        /// seront utilisés par exemple au moment du lancement de l'application pour l'ouverture d'un fichier spécifique.
        /// </summary>
        /// <param name="e">Détails concernant la requête et le processus de lancement.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Ne répétez pas l'initialisation de l'application lorsque la fenêtre comporte déjà du contenu,
            // assurez-vous juste que la fenêtre est active
            if (rootFrame == null)
            {
                rootFrame = await StartApp(e);
            }
            else
            {

            }


            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // Quand la pile de navigation n'est pas restaurée, accédez à la première page,
                    // puis configurez la nouvelle page en transmettant les informations requises en tant que
                    // paramètre
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Vérifiez que la fenêtre actuelle est active
                Window.Current.Activate();
            }
        }

        private async Task<Frame> StartApp(IActivatedEventArgs e)
        {
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;

            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            Color c = new Color() { R = 0xF5, G = 0xF5, B = 0xF5 };
            titleBar.ButtonForegroundColor = Windows.UI.Colors.DimGray;
            titleBar.ButtonBackgroundColor = c;



            SettingsManager sett = new SettingsManager();
            var lang = await sett.GetSetting("chosen_culture", true);
            if (!string.IsNullOrEmpty(lang))
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo(lang);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
            }


            // Créez un Frame utilisable comme contexte de navigation et naviguez jusqu'à la première page
            Frame rootFrame = new Frame();
            rootFrame.NavigationFailed += OnNavigationFailed;

            bool loadState = (e.PreviousExecutionState == ApplicationExecutionState.Terminated);
            ExtendedSplash extendedSplash = new ExtendedSplash(e.SplashScreen, loadState);
            rootFrame.Content = extendedSplash;
            Window.Current.Content = rootFrame;

            if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
            }

            // Placez le frame dans la fenêtre active
            Window.Current.Content = rootFrame;


            ProjectPad.Business.ProjectPadApplication.Create(new TokenProvider(), sett);
            ProjectPad.Business.ProjectPadApplication.Instance.RefreshGlobals();


            return rootFrame;
        }



        /// <summary>
        /// Appelé lorsque la navigation vers une page donnée échoue
        /// </summary>
        /// <param name="sender">Frame à l'origine de l'échec de navigation.</param>
        /// <param name="e">Détails relatifs à l'échec de navigation</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Appelé lorsque l'exécution de l'application est suspendue.  L'état de l'application est enregistré
        /// sans savoir si l'application pourra se fermer ou reprendre sans endommager
        /// le contenu de la mémoire.
        /// </summary>
        /// <param name="sender">Source de la requête de suspension.</param>
        /// <param name="e">Détails de la requête de suspension.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: enregistrez l'état de l'application et arrêtez toute activité en arrière-plan
            deferral.Complete();
        }
    }
}
