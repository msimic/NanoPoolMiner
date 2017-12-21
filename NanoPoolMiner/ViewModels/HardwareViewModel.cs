using Caliburn.Micro;
using OpenHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenCLDevices;

namespace NanoPoolMiner.ViewModels
{
    public class HardwareViewModel : Screen
    {
        HardwareType[] supportedHw = { HardwareType.CPU, HardwareType.GpuAti, HardwareType.GpuNvidia };
        Computer _computer;
        UpdateVisitor _visitor;
        Timer _t;
        bool _isLoadingVisible = true;
        string _loadingMessage = "Probing hardware ...";
        private List<Platform> _devs;

        public HardwareViewModel()
        {
            Hardware = new ObservableCollection<HardwareItemViewModel>();
            _visitor = new UpdateVisitor();
            _t = new Timer(OnTimer, null, 1500, 10000);
        }

        public void Enable(HardwareItemViewModel hi)
        {
            hi.IsEnabled = true;
        }

        public void Disable(HardwareItemViewModel hi)
        {
            hi.IsEnabled = false;
        }

        public void Edit(HardwareItemViewModel hi)
        {
            (Parent as MainViewModel).EditHardware(hi);
        }

        public void StopEdit()
        {
            (Parent as MainViewModel).StopEditHardware();
        }

        protected override void OnDeactivate(bool close)
        {
            if (close)
            {
                _computer.Close();
            }
            base.OnDeactivate(close);
        }
        private void HardwareRemoved(IHardware hardware)
        {
            var toRemove = Hardware.FirstOrDefault(h => h.Id == hardware.Identifier.ToString());
            Hardware.Remove(toRemove);
        }

        private void HardwareAdded(IHardware hardware)
        {
            if (supportedHw.Contains(hardware.HardwareType))
            {
                Execute.OnUIThreadAsync(() => Hardware.Add(new HardwareItemViewModel(hardware, this)));
            }
        }

        private void OnTimer(object state)
        {
            bool done = false;
            if (_computer == null)
            {
                _devs = OpenCLDevices.AMDDevices.Enumerate();
                done = true;
                var computer = new Computer();
                computer.CPUEnabled = true;
                computer.GPUEnabled = true;
                computer.HardwareAdded += HardwareAdded;
                computer.HardwareRemoved += HardwareRemoved;
                computer.Open();
                _computer = computer;
            }
            try
            {
                //_computer.Accept(_visitor);

                foreach (var item in Hardware.ToList())
                {
                    item.Refresh();
                }
            }
            finally
            {
                if (done)
                {
                    IsLoading = false;
                }
            }
        }

        public ObservableCollection<HardwareItemViewModel> Hardware { get; set; }

        public bool IsLoading
        {
            get
            {
                return _isLoadingVisible;
            }

            set
            {
                _isLoadingVisible = value;
                NotifyOfPropertyChange(() => IsLoading);
            }
        }

        public string LoadingMessage
        {
            get
            {
                return _loadingMessage;
            }

            set
            {
                _loadingMessage = value;
                NotifyOfPropertyChange(() => LoadingMessage);
            }
        }
    }

    public class SensorData
    {
        public string Name { get; set; }
        public float Value { get; set; }
        public string Type { get; set; }
    }

    public class HardwareItemViewModel : Screen
    {
        IHardware _h;
        private HardwareViewModel _vm;
        bool _progressIsVisible;
        bool _isEnabled;
        ObservableCollection<SensorData> _sensors;

        public HardwareItemViewModel(IHardware h, HardwareViewModel vm)
        {
            _h = h;
            _vm = vm;
            CreateSensors(_h);
        }

        private void CreateSensors(IHardware h)
        {
            var ss = new ObservableCollection<SensorData>();

            foreach (var item in h.Sensors)
            {
                if (item.Value.HasValue)
                {
                    ss.Add(new SensorData
                    {
                        Name = item.Name,
                        Value = item.Value.Value,
                        Type = GetSensorType(item.SensorType)
                    });
                }
            }

            _sensors = ss;
            NotifyOfPropertyChange(() => Sensors);
        }

        private string GetSensorType(SensorType sensorType)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(sensorType.ToString());

            switch (sensorType)
            {
                case SensorType.Voltage:
                    sb.Append("V/mV");
                    break;
                case SensorType.Clock:
                    sb.Append("MHz");
                    break;
                case SensorType.Temperature:
                    sb.Append("°C");
                    break;
                case SensorType.Load:
                    sb.Append("%");
                    break;
                case SensorType.Fan:
                    sb.Append("RPM");
                    break;
                case SensorType.Flow:
                    break;
                case SensorType.Control:
                    sb.Append("");
                    break;
                case SensorType.Level:
                    break;
                case SensorType.Factor:
                    break;
                case SensorType.Power:
                    sb.Append("W");
                    break;
                case SensorType.Data:
                    break;
                default:
                    break;
            }

            return sb.ToString();
        }

        public override void Refresh()
        {
            _h.Update();
            CreateSensors(_h);
            base.Refresh();
        }

        public void GoBack()
        {
            _vm.StopEdit();
        }

        public ObservableCollection<SensorData> Sensors
        {
            get
            {
                return _sensors;
            }
        }

        public string Id
        {
            get
            {
                return _h.Identifier.ToString();
            }
        }

        public string Name 
        {
            get
            {
                return _h.Name;
            }
        }

        public string Status
        {
            get
            {
                return GetHardwareType(_h.HardwareType) + ": " + GetSensors();
            }
        }

        private string GetHardwareType(HardwareType hardwareType)
        {
            switch (hardwareType)
            {
                case HardwareType.CPU:
                    return "CPU";
                case HardwareType.GpuNvidia:
                    return "nVidia GPU";
                case HardwareType.GpuAti:
                    return "AMD GPU";
                default:
                    return "Unsupported";
            }
        }

        public bool ProgressIsVisible
        {
            get
            {
                return _progressIsVisible;
            }
            set
            {
                _progressIsVisible = value;
                NotifyOfPropertyChange(() => ProgressIsVisible);
            }
        }

        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }

            set
            {
                _isEnabled = value;
                NotifyOfPropertyChange(() => IsEnabled);
                ProgressIsVisible = value;
            }
        }

        private string GetSensors()
        {
            int had = 0;
            if (_h.HardwareType == HardwareType.CPU)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(' ');
                var ss = _h.Sensors.Where(s => s.SensorType == SensorType.Clock).FirstOrDefault();
                if (ss != null && ss.Value.HasValue)
                {
                    had++;
                    sb.Append("Clock: " + (int)ss.Value + "Mhz ");
                }
                ss = _h.Sensors.Where(s => s.SensorType == SensorType.Temperature).FirstOrDefault();
                if (ss != null && ss.Value.HasValue)
                {
                    if (had > 0) sb.Append(", ");
                    had++;
                    sb.Append("Temp: " + (int)ss.Value + "°C ");
                }
                ss = _h.Sensors.Where(s => s.Name == "CPU Total").FirstOrDefault();
                if (ss != null && ss.Value.HasValue)
                {
                    if (had > 0) sb.Append(", ");
                    had++;
                    sb.Append("Load: " + (int)ss.Value + "% ");
                }

                return had > 0 ? sb.ToString() : "<no info> ";
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(' ');
                var ss = _h.Sensors.Where(s => s.SensorType == SensorType.Clock).FirstOrDefault();
                if (ss != null && ss.Value.HasValue)
                {
                    had++;
                    sb.Append("Clock: " + (int)ss.Value + "Mhz ");
                }
                ss = _h.Sensors.Where(s => s.SensorType == SensorType.Temperature).FirstOrDefault();
                if (ss != null && ss.Value.HasValue)
                {
                    if (had > 0) sb.Append(", ");
                    had++;
                    sb.Append("Temp: " + (int)ss.Value + "°C ");
                }
                ss = _h.Sensors.Where(s => s.Name == "GPU Core" && s.SensorType == SensorType.Load).FirstOrDefault();
                if (ss != null && ss.Value.HasValue)
                {
                    if (had > 0) sb.Append(", ");
                    had++;
                    sb.Append("Load: " + (int)ss.Value + "%  ");
                }
                return had > 0 ? sb.ToString() : "<no info> ";
            }
        }
    }

    public class UpdateVisitor : IVisitor
    {
        public void VisitComputer(IComputer computer)
        {
            computer.Traverse(this);
        }

        public void VisitHardware(IHardware hardware)
        {
            hardware.Update();
            foreach (IHardware subHardware in hardware.SubHardware)
                subHardware.Accept(this);
        }

        public void VisitSensor(ISensor sensor) { }

        public void VisitParameter(IParameter parameter) { }
    }
}
