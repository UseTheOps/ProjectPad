using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPad.Business
{
    partial class ProjectViewModel
    {
        public async Task Save()
        {
            await ProjectPadApplication._settingsMgr.CreateFolder(this.MetaData.Id);
            string json = JsonConvert.SerializeObject(this.MetaData);
            await ProjectPadApplication._settingsMgr.WriteFile($"{this.MetaData.Id}\\meta.json", json);
            json = JsonConvert.SerializeObject(this._Items);
            await ProjectPadApplication._settingsMgr.WriteFile($"{this.MetaData.Id}\\content.json", json);
            await ProjectPadApplication.Instance.AddToRecentList(this);
            IsAvailableOnLocal = true;
        }


        public async static void ClearLocalCache()
        {

        }

        public async static Task<ProjectViewModel> NewProject(string name)
        {
            ProjectViewModel t = new ProjectViewModel();
            t.MetaData = new ProjectMetaData()
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = name
            };
            t._Items.Add(new ProjectViewModelItem()
            {
                ItemKind = ProjectItemKind.Title,
                StringContent = name
            });
            await t.Save();
            return t;
        }


        public async Task Load()
        {
            await LoadMetaData(this.MetaData.Id, this);
            await LoadCoreData();
        }

        private static async Task LoadMetaData(string projectId, ProjectViewModel p)
        {
            using (var t = await ProjectPadApplication._settingsMgr.OpenFileForRead($"{projectId}\\meta.json"))
            {
                p.DisablePropertyChangeEvent = true;
                // chargement des données
                string json = t.ReadToEnd();
                var meta = JsonConvert.DeserializeObject<ProjectMetaData>(json);

                p.MetaData = meta;

                p.IsAvailableOnLocal = true;
                p.DisablePropertyChangeEvent = false;
            }
        }

        private async Task LoadCoreData()
        {
            using (var t = await ProjectPadApplication._settingsMgr.OpenFileForRead($"{this.MetaData.Id}\\content.json"))
            {
                // chargement des données
                string json = t.ReadToEnd();
                DateTime dtLoaded = DateTime.Now;

                DisablePropertyChangeEvent = true;

                this._Items.Clear();
                this._Items.Add(new ProjectViewModelItem()
                {
                    ItemKind = ProjectItemKind.Title,
                    StringContent = "Le titre du projet"
                });

                this._Items.Add(new ProjectViewModelItem()
                {
                    ItemKind = ProjectItemKind.TextContent,
                    StringContent = "Ceci est le premier explicatif de la ligne de projet"
                });


                foreach (var dt in _Items)
                    dt.dtLoaded = dtLoaded;

                IsAvailableOnLocal = true;
                DisablePropertyChangeEvent = false;
            }
        }


    }
}
