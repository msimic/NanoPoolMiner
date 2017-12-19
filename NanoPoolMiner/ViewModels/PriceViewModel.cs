using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NanoPoolMiner.API.NanoPoolXMRApi;
using NanoPoolMiner.API;

namespace NanoPoolMiner.ViewModels
{
    public class PriceViewModel : Screen
    {
        Price _price;

        public PriceViewModel(Price price)
        {
            Price = price;
        }

        public Price Price
        {
            get
            {
                return _price;
            }

            set
            {
                _price = value;
                NotifyOfPropertyChange(() => Price);
            }
        }
    }
}
