﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
#if NETCOREAPP
using System.Runtime.Versioning;
#endif

namespace Unofficial.Microsoft.Extensions.Logging.TextFile
{
    #if NETCOREAPP
    [UnsupportedOSPlatform("browser")]
    #endif
    [ProviderAlias("File")]
    public sealed class FileLoggerProvider : ILoggerProvider
    {
        private readonly IDisposable? _onChangeToken;
        private FileLoggerConfiguration _currentConfig;
        private readonly ConcurrentDictionary<string, FileLogger> _loggers =
            new(StringComparer.OrdinalIgnoreCase);

        public FileLoggerProvider(IOptionsMonitor<FileLoggerConfiguration> config)
        {
            _currentConfig = config.CurrentValue;
            _onChangeToken = config.OnChange(updatedConfig => _currentConfig = updatedConfig);
        }

        public ILogger CreateLogger(string categoryName) =>
            _loggers.GetOrAdd(categoryName, name => new FileLogger(name, GetCurrentConfig));

        private FileLoggerConfiguration GetCurrentConfig() => _currentConfig;

        public void Dispose()
        {
            _loggers.Clear();
            _onChangeToken?.Dispose();
        }
    }
}
