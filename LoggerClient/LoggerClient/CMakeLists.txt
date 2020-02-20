# CMakeList.txt : CMake project for LoggerClient, include source and define
# project specific logic here.
#
cmake_minimum_required (VERSION 3.8)

add_library(signalrclient UNKNOWN IMPORTED)
set_property(TARGET signalrclient PROPERTY IMPORTED_LOCATION "/tmp/SignalR-Client-Cpp/build.release/bin/libsignalrclient.so")
set_property(TARGET signalrclient PROPERTY INTERFACE_INCLUDE_DIRECTORIES "/tmp/SignalR-Client-Cpp/include")

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
