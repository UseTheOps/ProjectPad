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
                            dtChanged = DateTime.Now;
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
                            dtChanged = DateTime.Now;
                        OnPropertyChanged(nameof(StringContent));
                    }
                }
            }

            [JsonIgnoreAttribute]
            public DateTime dtLoaded = DateTime.MinValue;

            [JsonIgnoreAttribute]
            public DateTime dtChanged = DateTime.MinValue;

        }
    }

}
