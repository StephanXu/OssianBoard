﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DnsClient.Protocol;
using Logger.Models;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Logger.Services
{
    public interface ILogService
    {
        public IEnumerable<RecordModel> GetLog(string logId, int page, int itemsPerPage);
        public IEnumerable<PlotRequest> GetPlots(string logId);
        public Task<LogModel> CreateLog(string name, string description);
        public IEnumerable<LogModel> ListLogs();
        public LogModel GetLogMeta(string logId);
        public Task<IncrementRequest> AddRecord(string logId, IEnumerable<string> record);
        public Task<LogModel> RemoveLog(string logId);
    }

    public class LogService : ILogService
    {
        private readonly IMongoCollection<LogModel> _logCollection;
        private readonly IMongoCollection<RecordModel> _recordCollection;
        private readonly IMongoCollection<PlotModel> _plotCollection;
        private readonly IMongoCollection<VariableModel> _variableCollection;
        private readonly IMongoCollection<DotModel> _dotCollection;

        public LogService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("OnlineLogger"));
            var database = client.GetDatabase("OnlineLogger");
            _logCollection = database.GetCollection<LogModel>("logs");
            _recordCollection = database.GetCollection<RecordModel>("records");
            _plotCollection = database.GetCollection<PlotModel>("plots");
            _variableCollection = database.GetCollection<VariableModel>("variables");
            _dotCollection = database.GetCollection<DotModel>("dots");
        }

        public IEnumerable<RecordModel> GetLog(string logId, int page, int itemsPerPage) =>
            _recordCollection.AsQueryable()
                .Where(item => item.LogId == logId)
                .Skip(itemsPerPage * (page - 1))
                .Take(itemsPerPage);

        public IEnumerable<PlotRequest> GetPlots(string logId)
        {
            var plots = _plotCollection.AsQueryable()
                .Where(plot => plot.LogId == logId)
                .ToList();
            var result = new List<PlotRequest>();
            foreach (var plotModel in plots)
            {
                var plotRequest = new PlotRequest
                {
                    Name = plotModel.Name,
                    Variables = new List<VariableRequest>()
                };
                foreach (var varId in plotModel.Variables)
                {
                    plotRequest.Variables.Add(new VariableRequest
                    {
                        Name = _variableCollection.AsQueryable()
                            .First(variable => variable.Id == varId).Name,
                        Dots = _dotCollection.AsQueryable()
                            .Where(dot => dot.VariableId == varId)
                            .Select(dot => new DotRequest
                            {
                                Time = dot.Time,
                                Value = dot.Value
                            }).ToList()
                    });
                }

                result.Add(plotRequest);
            }

            return result;
        }

        public async Task<LogModel> CreateLog(string name, string description)
        {
            var log = new LogModel
            {
                CreateTime = DateTime.Now,
                Name = name,
                Description = description,
                RecordCount = 0
            };
            await _logCollection.InsertOneAsync(log);
            return log.Id == null ? null : log;
        }

        public IEnumerable<LogModel> ListLogs() =>
            _logCollection.AsQueryable()
                .Where(item => true);

        public LogModel GetLogMeta(string logId) => 
            _logCollection.AsQueryable().First(item => item.Id == logId);

        public async Task<IncrementRequest> AddRecord(string logId, IEnumerable<string> record)
        {
            var log = (await _logCollection.FindAsync(item => item.Id == logId)).FirstOrDefault();
            var dots = new List<DotModel>(); // Cache for insertMany
            var incrementPlot = new List<PlotRequest>();
            var incrementRecord = new List<RecordModel>();
            foreach (var item in record)
            {
                var matches = Regex.Matches(item, @"\[.*?\]");
                var matchesStr =
                    matches.Select(item => item.ToString().Substring(1, item.Length - 2).Trim())
                        .ToList();
                var timestamp = DateTimeOffset.Parse(matchesStr[0]).ToUnixTimeMilliseconds();
                var content = item.Substring(matches[2].Index + matches[2].Length + 1).TrimEnd();
                var newRecord = new RecordModel
                {
                    LogId = log.Id,
                    Time = DateTime.Parse(matchesStr[0]),
                    ThreadId = Int32.Parse(matchesStr[1]),
                    Level = matchesStr[2],
                    Content = content
                };
                incrementRecord.Add(newRecord);
                _recordCollection.InsertOne(newRecord);
                log.RecordCount += 1;

                // Parse plot
                var plotName = Regex.Matches(content, @"@(?<plotName>\w+)")[0].Groups["plotName"].Value;
                var plot = _plotCollection.FindSync(curt => curt.LogId == log.Id && curt.Name == plotName)
                    .FirstOrDefault();
                if (plot == null)
                {
                    plot = new PlotModel
                    {
                        Id = null,
                        LogId = log.Id,
                        Name = plotName,
                        Variables = new List<string>()
                    };
                    _plotCollection.InsertOne(plot);
                }

                var deltaPlot = incrementPlot.Find(curt => curt.Name == plotName);
                if (deltaPlot == null)
                {
                    deltaPlot = new PlotRequest
                    {
                        Name = plotName,
                        Variables = new List<VariableRequest>()
                    };
                    incrementPlot.Add(deltaPlot);
                }

                // Parse variable assignment
                var assignment = Regex.Matches(content, @"\[(?<assignment>.*?)\]")[0].Groups["assignment"].Value;
                foreach (Match match in Regex.Matches(assignment, @"\$(?<varName>\w+)\s*=\s*(?<value>[0-9e\-+.]*)"))
                {
                    var varName = match.Groups["varName"].Value;
                    var value = match.Groups["value"].Value;
                    var variable = _variableCollection
                        .FindSync(curt => curt.LogId == log.Id && curt.Name == varName)
                        .FirstOrDefault();
                    if (variable == null)
                    {
                        variable = new VariableModel
                        {
                            Id = null,
                            LogId = log.Id,
                            Name = varName
                        };
                        _variableCollection.InsertOne(variable);
                        plot.Variables.Add(variable.Id);
                        _plotCollection.ReplaceOne(item => item.Id == plot.Id, plot);
                    }

                    var deltaVariable = deltaPlot.Variables.Find(curt => curt.Name == varName);
                    if (deltaVariable == null)
                    {
                        deltaVariable = new VariableRequest
                        {
                            Name = varName,
                            Dots = new List<DotRequest>()
                        };
                        deltaPlot.Variables.Add(deltaVariable);
                    }

                    deltaVariable.Dots.Add(new DotRequest
                    {
                        Time = timestamp,
                        Value = Double.Parse(value)
                    });
                    dots.Add(new DotModel
                    {
                        Id = null,
                        Time = timestamp,
                        Value = Double.Parse(value),
                        VariableId = variable.Id,
                        ZoomLevel = 0,
                        RecordId = newRecord.Id
                    });
                }
            }

            await _dotCollection.InsertManyAsync(dots);
            await _logCollection.ReplaceOneAsync(item => item.Id == log.Id, log);
            return new IncrementRequest
            {
                Plots = incrementPlot,
                Records = incrementRecord
            };
        }

        public async Task<LogModel> RemoveLog(string logId)
        {
            var log = _logCollection.AsQueryable().First(item => item.Id == logId);
            await _recordCollection.DeleteManyAsync(item => item.LogId == log.Id);
            var varIds = _variableCollection.AsQueryable()
                .Where(item => item.LogId == log.Id)
                .Select(item => item.Id)
                .ToList();
            await _dotCollection.DeleteManyAsync(dot => varIds.Contains(dot.VariableId));
            await _variableCollection.DeleteManyAsync(item => item.LogId == logId);
            await _plotCollection.DeleteManyAsync(item => item.LogId == logId);
            await _logCollection.DeleteOneAsync(item => item.Id == log.Id);
            return log;
        }
    }
}