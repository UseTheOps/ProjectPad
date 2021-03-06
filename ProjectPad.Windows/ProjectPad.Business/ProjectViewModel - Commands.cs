﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProjectPad.Business
{
    partial class ProjectViewModel
    {


        public class AddContentCommand : ProjectCommandBase
        {
            public AddContentCommand(ProjectViewModel project) : base(project)
            {
            }

            public override bool CanExecute(object parameter)
            {
                return true;
            }

            public override void Execute(object parameter)
            {
                bool sameLevel = true;
                var kind = ProjectViewModel.ProjectItemKind.TextContent;
                switch (parameter?.ToString()?.ToLowerInvariant())
                {
                    case "title":
                        kind = ProjectViewModel.ProjectItemKind.Title;
                        break;
                    case "subtitle":
                        kind = ProjectViewModel.ProjectItemKind.Title;
                        sameLevel = false;
                        break;
                }

                _project.AddItem(kind, sameLevel);
            }
        }
    }
}
