using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPad.Business
{
    public interface ITokenProvider
    {
        Task<bool> HasSilentGraphApiToken();
        Task<string> GetGraphApiToken();

        Task ClearAllTokens();

        event EventHandler TokenChanged;
    }
}
