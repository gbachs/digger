using System;

namespace Digger
{
    public delegate ILogger LoggerResolver(Type forType);

    public static class LoggerFactory
    {
        public static LoggerResolver Resolve { get; set; }

        static LoggerFactory()
        {
            UseDefault();
        }

        public static void UseDefault()
        {
            Resolve = t => new NullLogger();
        }

        private class NullLogger : ILogger
        {
            public void Trace(string messageTemplate, params object[] formattingArgs) { }

            public void Debug(string messageTemplate, params object[] formattingArgs) { }

            public void Info(string messageTemplate, params object[] formattingArgs) { }

            public void Warn(string messageTemplate, params object[] formattingArgs) { }

            public void Error(string messageTemplate, params object[] formattingArgs) { }

            public void Error(Exception exception, string messageTemplate, params object[] formattingArgs) { }
        }
    }
}