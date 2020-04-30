#include <iostream>
#include <string>
#include <thread>
#include <mutex>
#include <condition_variable>
#include <memory>

#include <grpcpp/grpcpp.h>
#include <google/protobuf/util/json_util.h>
#include <google/protobuf/message.h>
#include <spdlog/spdlog.h>
#include <spdlog/sinks/base_sink.h>
#include <spdlog/sinks/stdout_color_sinks.h>


#include "Config.pb.h"
#include "Log.grpc.pb.h"
#include "Config.grpc.pb.h"

#define OSSIAN_SERVICE_SETUP(Signature)	\
	using _OssianServiceInjectorType=Signature;	\
	Signature

class OnlineDebugException : public std::runtime_error
{
public:
	OnlineDebugException(std::string what) : std::runtime_error(what)
	{
	}
};

class OnlineDebugRPC
{
	std::unique_ptr<OssianConfig::OnlineLogService::Stub> m_Stub;
	std::string m_LogId;
	std::mutex m_Mutex;
	std::condition_variable m_ConditionVariable;

public:
	OnlineDebugRPC(std::shared_ptr<grpc::Channel> channel)
		: m_Stub(OssianConfig::OnlineLogService::NewStub(channel))
	{
	}

	auto CreateLog(std::string name,
	               std::string description,
	               OssianConfig::Configuration config) -> std::string
	{
		grpc::ClientContext context;
		OssianConfig::CreateLogRequest request;
		OssianConfig::CreateLogResponse response;
		request.set_name(name);
		request.set_description(description);
		request.set_allocated_config(&config);
		const auto status = m_Stub->CreateLog(&context, request, &response);
		return response.logid();
	}

	auto AddLog(std::string logId,
	            const OssianConfig::AddLogRequest& logs)
	{
		grpc::ClientContext context;
		OssianConfig::AddLogResponse response;
		auto writer = m_Stub->AddLog(&context, logs, &response);
	}

	auto AddLog(std::string logId,
	            const std::vector<std::string>& logs)
	{
		OssianConfig::AddLogRequest request;
		request.mutable_log()->Reserve(logs.size());
		for(auto&& item:logs)
		{
			request.add_log(item);
		}
		AddLog(logId, request);
	}

	auto AddLogAsync(std::string logId,
	                 const OssianConfig::AddLogRequest& logs)
	{
		std::thread([this, logId, logs]()
		{
			AddLog(logId, logs);
		}).detach();
	}

	auto AddLogAsync(std::string logId,
	                 const std::vector<std::string>& logs)
	{
		std::thread([this, logId, logs]()
		{
			AddLog(logId, logs);
		}).detach();
	}
};

template <typename Mutex>
class online_logger_sink : public spdlog::sinks::base_sink<Mutex>
{
	OnlineDebugRPC& m_Hub;
	OssianConfig::AddLogRequest m_Logs;
	std::string m_LogId;
public:
	online_logger_sink(OnlineDebugRPC& onlineDebugHub, std::string logId)
		: m_Hub(onlineDebugHub)
		  , m_LogId(logId)
	{
	}

protected:
	void sink_it_(const spdlog::details::log_msg& msg) override
	{
		spdlog::memory_buf_t formatted;
		spdlog::sinks::base_sink<Mutex>::formatter_->format(msg, formatted);
		m_Logs.add_log(fmt::to_string(formatted));
	}

	void flush_() override
	{
		m_Hub.AddLogAsync(m_LogId, m_Logs);
		m_Logs.clear_log();
	}
};

using online_logger_sink_mt = online_logger_sink<std::mutex>;
using online_logger_sink_st = online_logger_sink<spdlog::details::null_mutex>;


int main()
{
	auto channelCreds = grpc::SslCredentials(grpc::SslCredentialsOptions());
	auto channel      = grpc::CreateChannel("debug.fenzhengrou.wang:5000", channelCreds);
	auto stub         = OssianConfig::ConfigurationService::NewStub(channel);

	grpc::ClientContext context;

	std::cout << (int)channel->GetState(true) << std::endl;

	OssianConfig::GetConfigurationRequest request{};
	OssianConfig::Configuration config;
	auto res = stub->GetConfiguration(&context, request, &config);
	std::cout << config.serialport().portname() << std::endl;
	std::string output;
	google::protobuf::util::MessageToJsonString(config, &output);
	std::cout << output << std::endl;
}
