#pragma once

#define __CL_ENABLE_EXCEPTIONS
#define CL_USE_DEPRECATED_OPENCL_2_0_APIS
#define CL_VERSION_1_2
//#define CL_USE_DEPRECATED_OPENCL_1_1_APIS
#pragma warning (push)
#pragma warning (disable:4005)
#include "cl_ext.hpp"
#pragma warning (pop)

#include <time.h>
#include <functional>
#include <map>
#include <vector>

#include "OpenCLDevice.h"

struct JsonLog {
	std::string PlatformName;
	int PlatformNum;
	std::vector<OpenCLDevice> Devices;
};

class AMDOpenCLDeviceDetection {
public:
	AMDOpenCLDeviceDetection();
	~AMDOpenCLDeviceDetection();
		
	bool QueryDevices();
	void PrintDevicesJson();
	void PrintDevicesJsonDirty();
	std::vector<std::string> _platformNames;
	std::vector<JsonLog> _devicesPlatformsDevices;

private:

	static std::vector<cl::Device> getDevices(std::vector<cl::Platform> const& _platforms, unsigned _platformId);
	static std::vector<cl::Platform> getPlatforms();

	static std::string StringnNullTerminatorFix(const std::string& str);
};
