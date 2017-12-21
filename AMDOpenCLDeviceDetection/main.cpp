#include "AMDOpenCLDeviceDetection.h"
#include <array>

bool containsArgument(std::vector<std::string> &arguments, std::string value)
{
	return std::any_of(arguments.begin(), arguments.end(), [=](std::string v) {return v == value;});
}

int main(int argc, char* argv[]) {
	std::vector<std::string> arguments(argv + 1, argv + argc);

	bool isCompact = containsArgument(arguments, "/c");

	AMDOpenCLDeviceDetection AMDOpenCLDeviceDetection;
	if (AMDOpenCLDeviceDetection.QueryDevices()) {
		if (isCompact) {
			AMDOpenCLDeviceDetection.PrintDevicesJsonDirty();
		}
		else {
			AMDOpenCLDeviceDetection.PrintDevicesJson();
		}
	}
	return 0;
}

