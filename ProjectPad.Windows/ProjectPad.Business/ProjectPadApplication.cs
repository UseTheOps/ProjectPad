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
            _tokenProvider.TokenChanged += _tokenProvider_TokenChanged;
            _settingsMgr = settingsMgr;
            return _singleton;
        }

        private static void _tokenProvider_TokenChanged(object sender, EventArgs e)
        {
            _singleton.RefreshGlobals();
        }

        public async void RefreshGlobals()
        {
            bool newHasToken = await _tokenProvider.HasSilentToken();
            if (newHasToken != HasToken)
            {
                HasToken = newHasToken;
                OnPropertyChanged("HasToken");
            }
        }

        public static ProjectPadApplication Instance { get { return _singleton; } }

        public bool HasToken { get; private set; }

        public void TryConnect()
        {
            _tokenProvider.GetToken();
        }

        public void ClearConnections()
        {
            _tokenProvider.ClearAllTokens();
        }
    }
}
