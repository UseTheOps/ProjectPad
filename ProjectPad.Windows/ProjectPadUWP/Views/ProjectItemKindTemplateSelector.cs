using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ProjectPadUWP.Views
{
    public class ProjectItemKindTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Title { get; set; }
        public DataTemplate TextContent { get; set; }


        public ProjectItemKindTemplateSelector()
        {
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return SelectTemplateCore(item);
        }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item != null && item is ProjectPad.Business.ProjectViewModel.ProjectViewModelItem)
            {
                var prjItem = item as ProjectPad.Business.ProjectViewModel.ProjectViewModelItem;

                switch (prjItem.ItemKind)
                {
                    case ProjectPad.Business.ProjectViewModel.ProjectItemKind.Title:
                        return Title;
                    case ProjectPad.Business.ProjectViewModel.ProjectItemKind.TextContent:
                        return TextContent;

                }
            }
            return null;
        }
    }
}
