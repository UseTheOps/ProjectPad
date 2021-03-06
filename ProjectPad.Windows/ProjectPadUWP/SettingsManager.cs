﻿using ProjectPad.Business;
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
                await file.DeleteAsync();
                file = await st.CreateFileAsync(fileName);
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

        public async Task ClearAll(bool includeRoaming)
        {
            var st = ApplicationData.Current.LocalSettings;
            st.Values.Clear();
            var fold = ApplicationData.Current.LocalFolder;
            foreach (var f in await fold.GetFilesAsync())
                await f.DeleteAsync(StorageDeleteOption.PermanentDelete);
            foreach (var fo in await fold.GetFoldersAsync())
                await fo.DeleteAsync(StorageDeleteOption.PermanentDelete);
            fold = ApplicationData.Current.LocalCacheFolder;
            foreach (var f in await fold.GetFilesAsync())
                await f.DeleteAsync(StorageDeleteOption.PermanentDelete);
            foreach (var fo in await fold.GetFoldersAsync())
                await fo.DeleteAsync(StorageDeleteOption.PermanentDelete);

            if (includeRoaming)
            {
                st = ApplicationData.Current.RoamingSettings;
                st.Values.Clear();
                fold = ApplicationData.Current.RoamingFolder;
                foreach (var f in await fold.GetFilesAsync())
                    await f.DeleteAsync(StorageDeleteOption.PermanentDelete);
                foreach (var fo in await fold.GetFoldersAsync())
                    await fo.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }

        }

        public async Task<string> GetFullPath(string folderName)
        {
            var st = ApplicationData.Current.LocalFolder;
            try
            {
                var item = await st.GetItemAsync(folderName);
                return item.Path;
            }
            catch (FileNotFoundException)
            {
                
            }
            return null;
        }

        public async Task<List<string>> EnumerateFiles(string folderName)
        {
            var st = ApplicationData.Current.LocalFolder;
            try
            {
                List<string> ret = new List<string>();
                var item = await st.GetFolderAsync(folderName);
                await EnumerateFiles(item, item.Path, ret);
                return ret;
            }
            catch (FileNotFoundException)
            {

            }
            return new List<string>();
        }

        private async Task EnumerateFiles(StorageFolder folder, string root, List<string> ret)
        {
            var files = await folder.GetFilesAsync();
            foreach(var file in files)
            {
                string pth = file.Path;
                if (pth.StartsWith(root))
                    pth = pth.Substring(root.Length);
                if (pth.StartsWith("/") || pth.StartsWith("\\"))
                    pth = pth.Substring(1);
                ret.Add(pth);
            }

            var subFolders = await folder.GetFoldersAsync();
            foreach (var subfolder in subFolders)
                await EnumerateFiles(subfolder, root, ret);

        }


        public async Task<List<string>> EnumerateFolders(string folderName)
        {
            var st = ApplicationData.Current.LocalFolder;
            try
            {
                List<string> ret = new List<string>();
                var item = await st.GetFolderAsync(folderName);
                await EnumerateFiles(item, item.Path, ret);
                return ret;
            }
            catch (FileNotFoundException)
            {

            }
            return new List<string>();
        }

        private async Task EnumerateFolders(StorageFolder folder, string root, List<string> ret)
        {
            var subFolders = await folder.GetFoldersAsync();
            foreach (var subfolder in subFolders)
            {
                string pth = subfolder.Path;
                if (pth.StartsWith(root))
                    pth = pth.Substring(root.Length);
                if (pth.StartsWith("/") || pth.StartsWith("\\"))
                    pth = pth.Substring(1);
                ret.Add(pth);
                await EnumerateFiles(subfolder, root, ret);
            }

        }
    }
}
