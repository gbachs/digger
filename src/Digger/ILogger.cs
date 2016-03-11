using System;

namespace Digger
{
    public interface ILogger
    {
        void Trace(string messageTemplate, params object[] formattingArgs);
        void Debug(string messageTemplate, params object[] formattingArgs);
        void Info(string messageTemplate, params object[] formattingArgs);
        void Warn(string messageTemplate, params object[] formattingArgs);
        void Error(string messageTemplate, params object[] formattingArgs);
        void Error(Exception exception, string messageTemplate, params object[] formattingArgs);
    }
}