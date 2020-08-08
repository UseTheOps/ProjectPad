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
        public DataTemplate Title2 { get; set; }
        public DataTemplate Title3 { get; set; }
        public DataTemplate Title4 { get; set; }
        public DataTemplate Title5 { get; set; }
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
                        switch(prjItem.TitleLevel)
                        {
                            case 0: return Title;
                            case 1: return Title;
                            case 2: return Title2;
                            case 3: return Title3;
                            case 4: return Title4;
                            default : return Title5;
                        }
                        
                    case ProjectPad.Business.ProjectViewModel.ProjectItemKind.TextContent:
                        return TextContent;

                }
            }
            return null;
        }
    }
}
