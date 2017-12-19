using Caliburn.Micro;
using NanoPoolMiner.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoPoolMiner.ViewModels
{
    public class ShellViewModel : Caliburn.Micro.Conductor<object>.Collection.AllActive
    {
        MainViewModel _main;
        OptionViewModel _options;

        public ShellViewModel(MainViewModel main, OptionViewModel options)
        {
            DisplayName = "Nanopool XMR Miner";
            _main = main;
            _options = options;
            ShowMain();
        }

        public void ShowMain()
        {
            ActivateItem(_main);
        }

        public void Close(object vm)
        {
            DeactivateItem(vm, true);
        }

        public void ShowMessage(object vm)
        {
            ActivateItem(vm);
        }

        public void ShowOptions()
        {
            ActivateItem(_options);
        }
    }
}
