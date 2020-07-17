using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPad.Business
{
    /// <summary>
    /// Acts as "main view model"
    /// </summary>
    public class ProjectPadApplication : ViewModelBase
    {
        private ProjectPadApplication()
        {
            RecentProjects = new List<RecentProject>();
        }

        internal static ITokenProvider _graphApiTokenProvider;
        internal static ISettingsManager _settingsMgr;

        private static ProjectPadApplication _singleton;

        public static ProjectPadApplication Create(ITokenProvider tokenProvider, ISettingsManager settingsMgr)
        {
            _singleton = new ProjectPadApplication();
            _graphApiTokenProvider = tokenProvider;
            _graphApiTokenProvider.TokenChanged += _tokenProvider_TokenChanged;
            _settingsMgr = settingsMgr;
            return _singleton;
        }

        private static void _tokenProvider_TokenChanged(object sender, EventArgs e)
        {
            _singleton.RefreshGlobals();
            _singleton.RefreshRecent();
        }

        public async void RefreshGlobals()
        {
            bool newHasToken = await _graphApiTokenProvider.HasSilentToken();
            if (newHasToken != HasToken)
            {
                HasToken = newHasToken;
                OnPropertyChanged("HasToken");
            }

            if(Me==null)
            {
                var tmp = await GraphApiHelper.GetMe();
                Me = new UserData()
                {
                    ConnectionType = "GraphApi",
                    Image = tmp.ImageData,
                    Name = tmp.displayName
                };
                OnPropertyChanged("Me");
            }
        }

        public async Task RefreshRecent()
        {
            if (RecentProjects.Count == 0)
            {
                RecentProjects.Add(new RecentProject() { Id = "project1", Name = "Project 1", LastChange = DateTime.Now.AddDays(-1) }); ;
                RecentProjects.Add(new RecentProject() { Id = "project2", Name = "Project 2", LastChange = DateTime.Now.AddDays(-2) });
            }

            OnPropertyChanged("RecentProjects");
        }

        public List<RecentProject> RecentProjects { get; set; }

        public static ProjectPadApplication Instance { get { return _singleton; } }

        public bool HasToken { get; private set; }

        public UserData Me { get; private set; }

        public void TryConnect()
        {
            _graphApiTokenProvider.GetToken();
        }

        public void ClearConnections()
        {
            _graphApiTokenProvider.ClearToken();
        }

        public async Task<ProjectViewModel> GetProject(string id)
        {
            return await ProjectViewModel.Get(id);
        }

        public async Task<ProjectViewModel> GetProject(RecentProject t)
        {
            return await ProjectViewModel.Get(t.Id);
        }
    }

    public class RecentProject
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Kind { get; set; }
        public DateTime LastChange { get; set; }
    }

    public class UserData
    {
        public string Name { get; set; }
        public string ConnectionType { get; set; }
        public byte[] Image { get; set; }
    }

}
