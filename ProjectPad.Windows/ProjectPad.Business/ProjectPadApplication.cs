﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private async static void _tokenProvider_TokenChanged(object sender, EventArgs e)
        {
            _singleton.RefreshGlobals();
            await _singleton.RefreshRecent();
        }

        public ITokenProvider GetTokenProvider(string type)
        {
            if (string.IsNullOrEmpty(type))
                return _graphApiTokenProvider;
            else
                return _graphApiTokenProvider.GetSubTokenProvider(type);
        }

        public async void RefreshGlobals()
        {
            bool newHasToken = await _graphApiTokenProvider.HasSilentToken();
            if (newHasToken != HasToken)
            {
                HasToken = newHasToken;
                OnPropertyChanged("HasToken");
            }

            if (newHasToken)
            {
                // only if a token is available
                if (Me == null)
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
        }

        public async Task<ProjectViewModel> CreateProject(string name, string id)
        {
            var p = new ProjectViewModel()
            {
                MetaData = new ProjectViewModel.ProjectMetaData()
                {
                    Id = id,
                    Name = name
                },
            };
            var t = p.AddItem(ProjectViewModel.ProjectItemKind.Title, true);
            t.StringContent = name;

            await p.SaveToLocalCache();
            await AddToRecentList(p);
            return p;
        }

        public async Task<string> GetPreferredCulture()
        {
            return await _settingsMgr.GetSetting("chosen_culture", true);
        }
        public async Task SetPreferredCulture(string language)
        {
            await _settingsMgr.SetSettings("chosen_culture", language, true); ;
        }


        public async Task RefreshRecent()
        {

            var tmp = await _settingsMgr.GetSetting("recent_projects", true);
            if (tmp != null)
            {
                RecentProjects = JsonConvert.DeserializeObject<List<RecentProject>>(tmp);
            }
            else
            {
                RecentProjects = new List<RecentProject>();
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

        public async Task ClearData(bool includeRoaming)
        {
            await _settingsMgr.ClearAll(includeRoaming);
            await RefreshRecent();
        }

        public async Task<ProjectViewModel> OpenProject(string id)
        {
            var project = await ProjectViewModel.Get(id);
            await AddToRecentList(project);
            OnProjectOpened(project);
            return project;
        }


        public class ProjectOpenedEventArgs : EventArgs
        {
            public ProjectViewModel Project { get; set; }
        }

        public static event EventHandler<ProjectOpenedEventArgs> ProjectOpened;
        internal static void OnProjectOpened(ProjectViewModel project)
        {
            ProjectOpened?.Invoke(typeof(ProjectPadApplication), new ProjectOpenedEventArgs()
            {
                Project = project
            });
        }

        public async Task<ProjectViewModel> OpenProject(RecentProject t)
        {
            var project = await ProjectViewModel.Get(t.Id);
            await AddToRecentList(project);
            OnProjectOpened(project);
            return project;
        }

        internal async Task AddToRecentList(ProjectViewModel project)
        {
            var lst = (from z in RecentProjects
                       where !z.Id.Equals(project.MetaData.Id, StringComparison.InvariantCultureIgnoreCase)
                       select z).ToList();
            lst.Insert(0, RecentProject.From(project, DateTime.Now));
            RecentProjects = lst;
            OnPropertyChanged("RecentProjects");
            var json = JsonConvert.SerializeObject(lst);
            await _settingsMgr.SetSettings("recent_projects", json, true);
        }
    }

    public class RecentProject
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string LastPath { get; set; }
        public string Kind { get; set; }
        public DateTime LastChange { get; set; }

        internal static RecentProject From(ProjectViewModel project, DateTime lastChange)
        {
            var t = new RecentProject()
            {
                Id = project.MetaData.Id,
                LastChange = lastChange,
                Kind = "",
                Name = project.MetaData.Name,
                LastPath = project.MetaData.LastPath
            };
            return t;
        }
    }

    public class UserData
    {
        public string Name { get; set; }
        public string ConnectionType { get; set; }
        public byte[] Image { get; set; }
    }

}
