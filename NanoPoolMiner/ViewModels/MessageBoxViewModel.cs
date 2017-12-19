using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoPoolMiner.ViewModels
{
    public enum MBResult
    {
        Ok,
        Cancel,
        Yes,
        No
    }

    public class MessageBoxViewModel : Screen
    {
        Action<MBResult> _action;

        public MessageBoxViewModel(string title, string message, Action<MBResult> click)
        {
            Title = title;
            Message = message;
            _action = click;
        }

        public void Ok()
        {
            (Parent as ShellViewModel).Close(this);
            _action(MBResult.Ok);
        }

        public void Cancel()
        {
            (Parent as ShellViewModel).Close(this);
            _action(MBResult.Cancel);
        }

        public string Title { get; set; }
        public string Message { get; set; }

    }
}
