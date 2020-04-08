# CMakeList.txt : CMake project for LoggerClient, include source and define
# project specific logic here.
#
cmake_minimum_required (VERSION 3.8)


find_library(SignalRClient_LIB signalrclient)
find_path(SignalRClient_INCLUDE signalrclient/hub_connection.h)
message("FOUND SignalRClient: ${SignalRClient_LIB}")
message("FOUND SignalRClient Headers: ${SignalRClient_INCLUDE}")

add_library(signalrclient UNKNOWN IMPORTED GLOBAL)
set_property(TARGET signalrclient PROPERTY IMPORTED_LOCATION "${SignalRClient_LIB}")
set_property(TARGET signalrclient PROPERTY INTERFACE_INCLUDE_DIRECTORIES "${SignalRClient_INCLUDE}")
set_property(TARGET signalrclient PROPERTY INTERFACE_COMPILE_OPTIONS "-DNO_SIGNALRCLIENT_EXPORTS")

find_package(spdlog CONFIG REQUIRED)
find_package(fmt CONFIG REQUIRED)

add_executable (LoggerClient)

target_compile_features(LoggerClient PRIVATE cxx_std_17)
target_sources(LoggerClient 
	PRIVATE
		"LoggerClient.cpp"
		"LoggerClient.h"
	)

target_compile_definitions(LoggerClient PUBLIC "-DNO_SIGNALRCLIENT_EXPORTS")
target_link_libraries(LoggerClient
	PRIVATE
		signalrclient
		spdlog::spdlog
		fmt::fmt
	)
