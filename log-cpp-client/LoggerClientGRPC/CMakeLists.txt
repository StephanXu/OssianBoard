
add_executable(LoggerClientGRPC)

find_package(gRPC CONFIG REQUIRED)
find_package(spdlog CONFIG REQUIRED)

add_subdirectory(proto)

target_compile_features(LoggerClientGRPC PRIVATE cxx_std_17)
target_sources(LoggerClientGRPC PRIVATE "LoggerClientGRPC.cpp")
target_link_libraries(LoggerClientGRPC PRIVATE gRPC::grpc++ spdlog::spdlog proto)