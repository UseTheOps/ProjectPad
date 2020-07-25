using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPad.Business
{
    public partial class ProjectViewModel : ViewModelBase
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
                await LoadMetaData(projectId, p);
            }
            catch (FileNotFoundException)
            {
                p.IsAvailableOnLocal = false;
            }

            if (p.IsAvailableOnLocal)
            {
                await p.LoadCoreData();
            }
            else
            {

            }

            return p;
        }

        public bool IsDirty
        {
            get
            {
                return false;
            }
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
                if (value != _IsAvailableOnLocal)
                {
                    _IsAvailableOnLocal = value;
                    OnPropertyChanged(nameof(IsAvailableOnLocal));
                }
            }
        }

        public ProjectMetaData MetaData { get; set; }


        private List<ProjectViewModelItem> _Items = new List<ProjectViewModelItem>();
        public IReadOnlyList<ProjectViewModelItem> ContentItems
        {
            get
            {
                return _Items.AsReadOnly();
            }
        }

    }

}
