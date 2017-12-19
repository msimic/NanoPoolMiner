#include <AMDOpenCLDeviceDetection.h>

using namespace System;
using namespace System::Collections::Generic;
using namespace std;

namespace OpenCLDevices
{
	public ref class OpenCLDevice {
	public:
		property unsigned int DeviceID;
		property String^ OPENCL_DEVICE_NAME;
		property String^ OPENCL_DEVICE_TYPE;
		property unsigned long long OPENCL_DEVICE_GLOBAL_MEM_SIZE;
		property String^ OPENCL_DEVICE_VENDOR;
		property String^ OPENCL_DEVICE_VERSION;
		property String^ OPENCL_DRIVER_VERSION;
		property int AMD_BUS_ID; // -1 indicates that it is not set
	};

	public ref class Platform {
	public:
		property String^ PlatformName;
		property int PlatformNum;
		property List<OpenCLDevice^>^ Devices;
	};

	public ref class AMDDevices
	{
	private:
	public:
		static List<Platform^>^ Enumerate()
		{
			auto ret = gcnew List<Platform^>();

			AMDOpenCLDeviceDetection detection;

			if (!detection.QueryDevices()) 
			{
				return ret;
			}

			for (size_t i = 0; i < detection._devicesPlatformsDevices.size(); i++)
			{
				auto pl = detection._devicesPlatformsDevices[i];
				auto platform = gcnew Platform();
				platform->PlatformName = gcnew String(pl.PlatformName.c_str());
				platform->PlatformNum = pl.PlatformNum;
				platform->Devices = gcnew List<OpenCLDevice^>();

				for (size_t l = 0; l < pl.Devices.size(); l++)
				{
					auto dev = pl.Devices[l];
					auto device = gcnew OpenCLDevice();
					device->DeviceID = dev.DeviceID;
					device->OPENCL_DEVICE_NAME = gcnew String(dev._CL_DEVICE_NAME.c_str());
					device->OPENCL_DEVICE_TYPE = gcnew String(dev._CL_DEVICE_TYPE.c_str());
					device->OPENCL_DEVICE_GLOBAL_MEM_SIZE = dev._CL_DEVICE_GLOBAL_MEM_SIZE;
					device->OPENCL_DEVICE_VENDOR = gcnew String(dev._CL_DEVICE_VENDOR.c_str());
					device->OPENCL_DEVICE_VERSION = gcnew String(dev._CL_DEVICE_VERSION.c_str());
					device->OPENCL_DRIVER_VERSION = gcnew String(dev._CL_DRIVER_VERSION.c_str());
					device->AMD_BUS_ID = dev.AMD_BUS_ID;

					platform->Devices->Add(device);
				}

				ret->Add(platform);
			}
			
			return ret;
		}
	};
};