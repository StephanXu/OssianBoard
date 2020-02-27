// LoggerClient.cpp : Defines the entry point for the application.
//
#include "LoggerClient.h"

#include <fmt/format.h>

#include <signalrclient/hub_connection_builder.h>
#include <signalrclient/log_writer.h>
#include <signalrclient/signalr_value.h>

#include <spdlog/spdlog.h>
#include <spdlog/sinks/stdout_color_sinks.h>
#include <spdlog/sinks/base_sink.h>

#include <iostream>
#include <memory>
#include <future>
#include <type_traits>
#include <random>
#include <tuple>
#include <mutex>
#include <utility>
#include "RPC.hpp"

class Logger : public signalr::log_writer
{
	// Inherited via log_writer
	virtual void __cdecl write(const std::string& entry) override
	{
		spdlog::info(entry);
	}
};

class OnlineLogHub :public BaseHub
{
public:
	OnlineLogHub() = delete;

	OnlineLogHub(signalr::hub_connection&& connection) :BaseHub(std::move(connection))
	{
		HUB_REGISTER_CALLBACK(ReloadSettings);
		Start();
	}

	static auto ReloadSettings(std::string settings)->void
	{
		std::cout << settings << std::endl;
	}
};

template<typename Mutex>
class online_logger_sink : public spdlog::sinks::base_sink<Mutex>
{
	OnlineLogHub m_Hub;
	std::vector<std::string> m_Logs;
	std::string m_LogId;
public:
	online_logger_sink(std::string url)
		:m_Hub(signalr::hub_connection_builder::create(url)
			   .with_logging(std::make_shared<Logger>(), signalr::trace_level::all)
			   .build())
	{
		std::promise<void> waitTask;
		m_Hub.Invoke("CreateLog", std::string("Untitled"), std::string("A log")).Then<std::string>(
			[&waitTask, this](const std::string& id, std::exception_ptr)
			{
				m_LogId = id;
				waitTask.set_value();
			});
		waitTask.get_future().get();
	}

protected:
	void sink_it_(const spdlog::details::log_msg& msg) override
	{
		spdlog::memory_buf_t formatted;
		spdlog::sinks::base_sink<Mutex>::formatter_->format(msg, formatted);
		m_Logs.push_back(fmt::to_string(formatted));
	}

	void flush_() override
	{
		m_Hub.Invoke("AddLog", m_LogId, m_Logs);
		m_Logs.clear();
	}
};

using online_logger_sink_mt = online_logger_sink<std::mutex>;
using online_logger_sink_st = online_logger_sink<spdlog::details::null_mutex>;

int main()
{
	const auto console = spdlog::stderr_color_mt("console");
	spdlog::set_default_logger(console);
	spdlog::set_pattern("[%T.%e] [%-5t] %^[%l]%$  %v");
	spdlog::set_level(spdlog::level::info);
	spdlog::flush_on(spdlog::level::trace);
	spdlog::flush_every(std::chrono::seconds(1));
	
	auto onlineLogger = std::make_shared<spdlog::logger>(
		"onlineLogger",
		std::make_shared<online_logger_sink_mt>("http://host.docker.internal:5000/logger"));
	onlineLogger->set_pattern("[%Y-%m-%dT%T.%e%z] [%-5t] %^[%l]%$ %v");
	spdlog::register_logger(onlineLogger);

	std::default_random_engine random(std::time(nullptr));
	onlineLogger->flush_on(spdlog::level::trace);
	while (true)
	{
		onlineLogger->log(static_cast<spdlog::level::level_enum>(random() % 6),
						  "Hello world: {}", random());
		
		std::this_thread::sleep_for(std::chrono::milliseconds(100));
	}
	return 0;
}
