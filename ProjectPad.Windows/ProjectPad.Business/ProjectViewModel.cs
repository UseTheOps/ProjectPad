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
        public AddContentCommand AddContent { get; private set; }
        protected internal ProjectViewModel()
        {
            AddContent = new AddContentCommand(this);
        }

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
                foreach(var r in _Items)
                {
                    
                    if (r.dtChanged > r.dtLoaded)
                        return true;
                }

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
            private set
            {
                if (value != _IsAvailableOnLocal)
                {
                    _IsAvailableOnLocal = value;
                    OnPropertyChanged(nameof(IsAvailableOnLocal));
                }
            }
        }

        private bool _IsSaving = false;
        public bool IsSaving
        {
            get
            {
                return _IsSaving;
            }
            private set
            {
                if (value != _IsSaving)
                {
                    _IsSaving = value;
                    OnPropertyChanged(nameof(IsSaving));
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


        public ProjectViewModelItem AddItem(ProjectItemKind kind, bool sameLevel)
        {
            return InsertItemAt(_Items.Count, kind, sameLevel);
        }

        public ProjectViewModelItem InsertItemAt(int index, ProjectItemKind kind, bool sameLevel)
        {
            var dt = DateTime.Now;
            var it = new ProjectViewModelItem()
            {
                ItemKind = kind,
                StringContent = "Nouveau",
                dtChanged = dt,
                dtLoaded = DateTimeOffset.Now.AddDays(-1)
            };
            _Items.Insert(index, it);
            OnPropertyChanged(nameof(ContentItems));
            return it;
        }


    }

}
