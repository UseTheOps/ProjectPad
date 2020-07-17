using ProjectPad.Business;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace ProjectPadUWP
{
    class SettingsManager : ISettingsManager
    {
        public async Task CreateFolder(string folderName)
        {
            var st = ApplicationData.Current.LocalFolder;
            try
            {
                var folder = await st.GetFolderAsync(folderName);
            }
            catch (FileNotFoundException)
            {
                var file = await st.CreateFolderAsync(folderName);
            }
        }

        public Task<string> GetSetting(string name, bool shouldRoam)
        {
            ApplicationDataContainer st;
            if (shouldRoam)
                st = ApplicationData.Current.RoamingSettings;
            else
                st = ApplicationData.Current.LocalSettings;

            if (st.Values.ContainsKey(name))
                return Task.FromResult<string>(st.Values[name] as string);
            else
                return Task.FromResult<string>(null);
        }

        public async Task<StreamReader> OpenFileForRead(string fileName)
        {
            var st = ApplicationData.Current.LocalFolder;
            var file = await st.GetFileAsync(fileName);
            if (file != null)
            {   
                var stream = await file.OpenReadAsync();
                return new StreamReader(stream.AsStream());
            }

            throw new FileNotFoundException();
        }

        public async Task WriteFile(string fileName, string content)
        {
            var st = ApplicationData.Current.LocalFolder;
            StorageFile file = null;
            try
            {
                file = await st.GetFileAsync(fileName);
            }
            catch(FileNotFoundException)
            {
                file = await st.CreateFileAsync(fileName);
            }

            using (var stream = await file.OpenStreamForWriteAsync())
            using(var wri = new StreamWriter(stream))
            {
                wri.Write(content);
            }
        }

        public Task SetSettings(string name, string value, bool shouldRoam)
        {
            ApplicationDataContainer st;
            if(shouldRoam)
                st = ApplicationData.Current.RoamingSettings;
            else
                st = ApplicationData.Current.LocalSettings;

            if (value!=null)
                st.Values[name] = value;
            else
                st.Values.Remove(name);

            return Task.CompletedTask;
        }


    }
}
