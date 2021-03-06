﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPad.Business
{
    public interface ITokenProvider
    {
        Task<bool> HasSilentToken();
        Task<string> GetToken();

        Task ClearToken();

        event EventHandler TokenChanged;

        ITokenProvider GetSubTokenProvider(string tokenType);

    }
}
