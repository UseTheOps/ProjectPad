using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace ProjectPad.Business
{
    public abstract class CommandBase : ICommand
    {
        public event EventHandler CanExecuteChanged;

        protected void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public abstract bool CanExecute(object parameter);

        public abstract void Execute(object parameter);
    }

    public abstract class ProjectCommandBase : CommandBase
    {
        protected ProjectViewModel _project;
        public ProjectCommandBase(ProjectViewModel project)
        {
            _project = project;
        }

    }

}
