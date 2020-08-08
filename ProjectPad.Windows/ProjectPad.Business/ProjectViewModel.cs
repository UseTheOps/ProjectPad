using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                await LoadMetaDataFromLocalCache(projectId, p);
            }
            catch (FileNotFoundException)
            {
                p.IsAvailableOnLocal = false;
            }

            if (p.IsAvailableOnLocal)
            {
                await p.LoadCoreDataFromLocalCache();
            }
            else
            {

            }

            return p;
        }

        public bool IsLocalCacheDirty
        {
            get
            {
                foreach (var r in _Items)
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
            if (kind != ProjectItemKind.Title)
                sameLevel = true;

            var dt = DateTime.Now;
            var it = new ProjectViewModelItem()
            {
                ItemKind = kind,
                StringContent = "Nouveau",
                dtChanged = dt,
                dtLoaded = DateTimeOffset.Now.AddDays(-1)
            };


            ProjectViewModelItem parent = null;
            if (index >= _Items.Count)
            {
                if (index > 0)
                    parent = _Items[_Items.Count - 1];
            }
            else
            {
                if (index > 0)
                    parent = _Items[index - 1];
            }

            if (parent != null)
            {
                if(parent.ItemKind == ProjectItemKind.Title 
                    && parent.TitleLevel == 1)
                {
                    it.ParentGuid = parent.Guid;
                    if (ProjectItemKind.Title == kind)
                        it.TitleLevel = parent.TitleLevel +1;
                }
                else if (sameLevel)
                {
                    if (parent.ParentGuid.HasValue)
                    {
                        if (parent.ItemKind == ProjectItemKind.Title)
                            it.ParentGuid = parent.Guid;
                        else
                            it.ParentGuid = parent.ParentGuid.Value;
                    }
                    else
                    {
                        // ca veut dire qu'il faut obligatoirement 
                        // que ce soit un sous item
                        it.ParentGuid = parent.Guid;
                    }
                    if (ProjectItemKind.Title == kind)
                    {
                        ProjectViewModelItem title = Get(it.ParentGuid.Value);
                        if(title != null)
                        {
                            it.TitleLevel = title.TitleLevel;
                            it.ParentGuid = title.Guid;
                        }
                    }
                }
                else
                {
                    if(parent.ItemKind == ProjectItemKind.Title)
                        it.ParentGuid = parent.Guid;
                    else
                        it.ParentGuid = parent.ParentGuid.Value;

                    if (ProjectItemKind.Title == kind)
                    {
                        var title = Get(it.ParentGuid.Value);
                        if (title != null)
                        {
                            it.TitleLevel = title.TitleLevel + 1;
                            it.ParentGuid = title.Guid;
                        }
                    }
                }
            }
            else
            {
                if (ProjectItemKind.Title == kind)
                    it.TitleLevel = 1;
            }


            _Items.Insert(index, it);
            OnPropertyChanged(nameof(ContentItems));
            return it;
        }



        //private ProjectViewModelItem GetParent(Guid parentGuid)
        //{
        //    return (from z in _Items
        //            where z.ParentGuid.HasValue && z.ParentGuid == parentGuid
        //               && z.ItemKind == ProjectItemKind.Title
        //            select z).FirstOrDefault();
        //}

        private ProjectViewModelItem Get(Guid itemGuid)
        {
            return (from z in _Items
                    where z.Guid.Equals(itemGuid)
                    select z).FirstOrDefault();
        }

    }

}
