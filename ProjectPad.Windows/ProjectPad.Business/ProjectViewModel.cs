using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPad.Business
{
    public class ProjectViewModel : ViewModelBase
    {
        public async static Task<ProjectViewModel> Get(string projectId)
        {
            ProjectViewModel p = new ProjectViewModel()
            {
                MetaData = new ProjectMetaData()
                {
                    Id = projectId,
                    Name = projectId
                }
            };

            try
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
            catch (FileNotFoundException)
            {
                p.IsAvailableOnLocal = false;
            }

            return p;
        }

        private bool _IsAvailableOnLocal = false;
        public bool IsAvailableOnLocal
        {
            get
            {
                return _IsAvailableOnLocal;
            }
            set
            {
                if(value!=_IsAvailableOnLocal)
                {
                    _IsAvailableOnLocal = value;
                    OnPropertyChanged(nameof(IsAvailableOnLocal));
                }
            }
        }

        public ProjectMetaData MetaData { get; set; }

        public class ProjectMetaData
        {
            public string Id { get; set; }
            public string Name { get; set; }

        }


        public async Task Save()
        {
            await ProjectPadApplication._settingsMgr.CreateFolder(this.MetaData.Id);
            string json = JsonConvert.SerializeObject(this.MetaData);
            await ProjectPadApplication._settingsMgr.WriteFile($"{this.MetaData.Id}\\meta.json", json);
            await ProjectPadApplication.Instance.AddToRecentList(this);
            IsAvailableOnLocal = true;
        }
    }

}
