using System;

namespace Digger.Agent.Logging
{
    public class SerilogLogger : ILogger
    {
        private readonly Serilog.ILogger _serilogLogger;

        public SerilogLogger(Serilog.ILogger serilogLogger)
        {
            _serilogLogger = serilogLogger;
        }

        public void Trace(string messageTemplate, params object[] formattingArgs)
        {
            _serilogLogger.Verbose(messageTemplate, formattingArgs);
        }

        public void Debug(string messageTemplate, params object[] formattingArgs)
        {
            _serilogLogger.Debug(messageTemplate, formattingArgs);
        }

        public void Info(string messageTemplate, params object[] formattingArgs)
        {
            _serilogLogger.Information(messageTemplate, formattingArgs);
        }

        public void Warn(string messageTemplate, params object[] formattingArgs)
        {
            _serilogLogger.Warning(messageTemplate, formattingArgs);
        }

        public void Error(string messageTemplate, params object[] formattingArgs)
        {
            _serilogLogger.Error(messageTemplate, formattingArgs);
        }

        public void Error(Exception exception, string messageTemplate, params object[] formattingArgs)
        {
            _serilogLogger.Error(exception, messageTemplate, formattingArgs);
        }
    }
}