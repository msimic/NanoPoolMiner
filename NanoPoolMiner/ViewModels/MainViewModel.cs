using Caliburn.Micro;
using NanoPoolMiner.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoPoolMiner.ViewModels
{
    public class MainViewModel : Screen
    {
        string _wallet;
        string _accountInfo;
        string _worker;
        decimal _hashrate;
        decimal _hashrate1h;
        string _hashrateUnit;
        string _currency;
        decimal _balance;
        decimal _ubalance;
        decimal _balanceCurrency;
        decimal _ubalanceCurrency;
        NanoPoolXMRApi _api;
        private PriceViewModel _price;
        PropertyChangedBase _hardware;
        private HardwareViewModel _hardwareVm;

        public MainViewModel(NanoPoolXMRApi api, HardwareViewModel hm)
        {
            _api = api;
            hm.Parent = this;
            _hardwareVm = hm;
            Hardware = hm;
            Wallet = Properties.Settings.Default.Wallet;
            Worker = Properties.Settings.Default.Worker;
            Currency = "€";
            Hashrate = 0;
            Hashrate1h = 0;
            HashrateUnit = "H/s";
            RefreshData();
        }

        internal void StopEditHardware()
        {
            Hardware = _hardwareVm;
        }

        public void EditHardware(HardwareItemViewModel hi)
        {
            Hardware = hi;
        }

        public string Wallet
        {
            get { return _wallet; }
            set
            {
                _wallet = value; _api.SetWallet(value);
                Properties.Settings.Default.Wallet = value;

                NotifyOfPropertyChange(() => Wallet);
                NotifyOfPropertyChange(() => CanShowBalance);
                RefreshData();
            }
        }

        public string Worker
        {
            get { return _worker; }
            set
            {
                _worker = value;
                Properties.Settings.Default.Worker = value;

                NotifyOfPropertyChange(() => Worker);
                NotifyOfPropertyChange(() => CanShowWorker);
                RefreshData();
            }
        }

        public string AccountInfo
        {
            get { return _accountInfo; }
            set
            {
                _accountInfo = value;
                NotifyOfPropertyChange(() => AccountInfo);
            }
        }

        private void RefreshData()
        {
            Price = new PriceViewModel(_api.GetPrice());
            if (CanShowAccountInfo)
            {
                ShowAccountInfo();
            }
        }

        public decimal Balance
        {
            get { return _balance; }
            set
            {
                _balance = value;
                NotifyOfPropertyChange(() => Balance);
                CalcCurrencyBalance();
            }
        }

        private void CalcCurrencyBalance()
        {
            if (Price?.Price != null)
            {
                BalanceCurrency = Math.Round(Balance * Price.Price.price_eur, 2);
                UBalanceCurrency = Math.Round(UBalance * Price.Price.price_eur, 2);
            }
        }

        public decimal UBalance
        {
            get { return _ubalance; }
            set
            {
                _ubalance = value;
                NotifyOfPropertyChange(() => UBalance);
                CalcCurrencyBalance();
            }
        }

        public decimal BalanceCurrency
        {
            get { return _balanceCurrency; }
            set
            {
                _balanceCurrency = value;
                NotifyOfPropertyChange(() => BalanceCurrency);
            }
        }

        public decimal UBalanceCurrency
        {
            get { return _ubalanceCurrency; }
            set
            {
                _ubalanceCurrency = value;
                NotifyOfPropertyChange(() => UBalanceCurrency);
            }
        }

        public bool CanShowBalance
        {
            get { return !string.IsNullOrWhiteSpace(Wallet); }
        }

        public void ShowBalance()
        {
            Balance = _api.GetBalance();
        }

        public bool CanShowWorker
        {
            get { return !string.IsNullOrWhiteSpace(Worker); }
        }

        public void ShowWorker()
        {

        }

        public bool CanShowAccountInfo
        {
            get { return CanShowBalance; }
        }

        public PriceViewModel Price
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

        public PropertyChangedBase Hardware
        {
            get
            {
                return _hardware;
            }

            set
            {
                _hardware = value;
                NotifyOfPropertyChange(() => Hardware);
            }
        }

        public string HashrateUnit
        {
            get
            {
                return _hashrateUnit;
            }

            set
            {
                _hashrateUnit = value;
                NotifyOfPropertyChange(() => HashrateUnit);
            }
        }

        public decimal Hashrate
        {
            get
            {
                return _hashrate;
            }

            set
            {
                _hashrate = value;
                NotifyOfPropertyChange(() => Hashrate);
            }
        }

        public decimal Hashrate1h
        {
            get
            {
                return _hashrate1h;
            }

            set
            {
                _hashrate1h = value;
                NotifyOfPropertyChange(() => Hashrate1h);
            }
        }

        public string Currency
        {
            get
            {
                return _currency;
            }

            set
            {
                _currency = value;
                NotifyOfPropertyChange(() => Currency);
            }
        }

        public void ShowAccountInfo()
        {
            var ai = _api.GetAccountInfo();
            AccountInfo = JsonConvert.SerializeObject(ai, Newtonsoft.Json.Formatting.Indented);
            Balance = ai.balance;
            UBalance = ai.unconfirmed_balance;
            var worker = ai.workers.FirstOrDefault(w => w.id == Worker);
            if (worker != null)
            {
                Hashrate = worker.hashrate;
                Hashrate1h = worker.h1;
            }
        }

        public void ShowOptions()
        {
            (Parent as ShellViewModel).ShowOptions();
        }

        public void VisitAccount()
        {
            if (!CanShowAccountInfo)
            {
                (Parent as ShellViewModel).ShowMessage(new MessageBoxViewModel("Missing data", "Have you filled in your wallet address?", (r) => { }));
                return;
            }
            Process.Start(@"https://xmr.nanopool.org/account/" + Wallet);
        }

        public void VisitWorker()
        {
            if (!CanShowAccountInfo || !CanShowWorker)
            {
                (Parent as ShellViewModel).ShowMessage(new MessageBoxViewModel("Missing data", "Have you filled in your wallet address and worker name?", (r) => { }));
                return;
            }
            Process.Start(@"https://xmr.nanopool.org/account/" + Wallet + "/" + Worker);
        }
    }
}
