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
                MetaData = new ProjectData()
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



                    p.DisablePropertyChangeEvent = false;
                }
            }
            catch (FileNotFoundException)
            {
            }

            return p;
        }



        public ProjectData MetaData { get; set; }

        public class ProjectData
        {
            public string Id { get; set; }
            public string Name { get; set; }

        }

    }

}
