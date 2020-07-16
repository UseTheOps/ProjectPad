using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectPad.Business
{
    /// <summary>
    /// Acts as "main view model"
    /// </summary>
    public class ProjectPadApplication : ViewModelBase
    {
        private ProjectPadApplication()
        {

        }

        private static ITokenProvider _tokenProvider;
        private static ISettingsManager _settingsMgr;

        private static ProjectPadApplication _singleton;

        public static ProjectPadApplication Create(ITokenProvider tokenProvider, ISettingsManager settingsMgr)
        {
            _singleton = new ProjectPadApplication();
            _tokenProvider = tokenProvider;
            _settingsMgr = settingsMgr;
            return _singleton;
        }

        public async void RefreshGlobals()
        {
            HasToken = await _tokenProvider.HasSilentToken();
            // Important enough to raised an notification even if not changed
            OnPropertyChanged("HasToken");
        }

        public static ProjectPadApplication Instance { get { return _singleton; } }

        public bool HasToken { get; private set; }

        public void TryConnect()
        {
            _tokenProvider.GetToken();
        }
    }
}
