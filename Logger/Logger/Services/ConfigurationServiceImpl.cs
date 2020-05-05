using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Logger.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Logger.Protos;

namespace Logger.Services
{
    public class ConfigurationServiceImpl : ConfigurationService.ConfigurationServiceBase
    {
        private readonly IArgumentsService _argumentsService;

        public ConfigurationServiceImpl(IArgumentsService argumentsService)
        {
            _argumentsService = argumentsService;
        }

        public override async Task<Configuration> GetConfiguration
            (GetConfigurationRequest request, ServerCallContext context) =>
            Configuration.Parser.ParseJson(_argumentsService.GetArguments().Arguments);

        public override async Task<ArchiveConfigurationResponse> ArchiveConfiguration
            (ArchiveConfigurationRequest request, ServerCallContext context)
        {
            _argumentsService.ArchiveArguments(request.LogId, request.Config);
            return new ArchiveConfigurationResponse();
        }
    }
}