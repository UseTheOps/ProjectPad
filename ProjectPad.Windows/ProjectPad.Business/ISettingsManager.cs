using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectPad.Business
{
    public interface ISettingsManager
    {
        string GetSetting(string name, bool shouldRoam);
        void SetSettings(string name, string value, bool shouldRoam);
    }
}
