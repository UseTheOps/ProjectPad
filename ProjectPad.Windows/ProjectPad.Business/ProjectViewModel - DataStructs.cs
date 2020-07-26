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
        public class ProjectMetaData
        {
            public string Id { get; set; }
            public string Name { get; set; }

        }
        
        public enum ProjectItemKind
        {
            TextContent,
            Title
        }

        public class ProjectViewModelItem : ViewModelBase
        {
            private ProjectItemKind _ItemKind = ProjectItemKind.TextContent;

            public ProjectItemKind ItemKind
            {
                get
                {
                    return _ItemKind;
                }
                set
                {
                    if (value != _ItemKind)
                    {
                        _ItemKind = value;
                        // DisablePropertyChangeEvent = true when loading
                        if (!DisablePropertyChangeEvent)
                            dtChanged = DateTimeOffset.Now;
                        OnPropertyChanged(nameof(ItemKind));
                    }
                }
            }

            private string _StringContent = null;

            public string StringContent
            {
                get
                {
                    return _StringContent;
                }
                set
                {
                    if (value != _StringContent)
                    {
                        _StringContent = value;
                        // DisablePropertyChangeEvent = true when loading
                        if (!DisablePropertyChangeEvent) 
                            dtChanged = DateTimeOffset.Now;
                        OnPropertyChanged(nameof(StringContent));
                    }
                }
            }


            private Guid _guid = Guid.Empty;

            public Guid Guid
            {
                get
                {
                    return _guid;
                }
                set
                {
                    if (value != _guid)
                    {
                        _guid = value;
                        // DisablePropertyChangeEvent = true when loading
                        if (!DisablePropertyChangeEvent)
                            dtChanged = DateTimeOffset.Now;
                        OnPropertyChanged(nameof(Guid));
                    }
                }
            }


            private Guid? _parentGuid = Guid.Empty;

            public Guid? ParentGuid
            {
                get
                {
                    return _parentGuid;
                }
                set
                {
                    if (value != _parentGuid)
                    {
                        _parentGuid = value;
                        // DisablePropertyChangeEvent = true when loading
                        if (!DisablePropertyChangeEvent)
                            dtChanged = DateTimeOffset.Now;
                        OnPropertyChanged(nameof(ParentGuid));
                    }
                }
            }



            [JsonIgnoreAttribute]
            public DateTimeOffset dtLoaded = DateTimeOffset.MinValue;

            
            public DateTimeOffset dtChanged { get; set; } = DateTimeOffset.MinValue;

        }
    }

}
