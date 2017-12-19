using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoPoolMiner.ViewModels
{
    public class OptionViewModel : Screen
    {
        public void GoBack()
        {
            (Parent as ShellViewModel).Close(this);
        }

        public void Message()
        {
            var msg = new MessageBoxViewModel("test", "some message", (r) => { });
            (Parent as ShellViewModel).ShowMessage(msg);
        }
    }
}
