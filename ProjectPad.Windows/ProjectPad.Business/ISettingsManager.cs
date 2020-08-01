using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPad.Business
{
    public interface ISettingsManager
    {
        Task<string> GetSetting(string name, bool shouldRoam);
        Task SetSettings(string name, string value, bool shouldRoam);

        Task<string> GetFullPath(string relativePath);

        Task<StreamReader> OpenFileForRead(string fileName);
        Task CreateFolder(string folderName);
        Task WriteFile(string fileName, string content);

        Task ClearAll(bool includeRoaming);
    }
}
