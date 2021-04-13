using System;
using DotPulsar.Abstractions;

namespace DotPulsar.Internal.Extensions
{
    public static class PulsarClientLoggerExtensions
    {
        public static void Trace(this IPulsarClientLogger? logger, string callerClass, string callerSite, string formatString, params object[] formatValues)
        {
            Log(logger, callerClass, callerSite, DotPulsarLogLevel.Trace, null, formatString, formatValues);
        }

        public static void Debug(this IPulsarClientLogger? logger, string callerClass, string callerSite, string formatString, params object[] formatValues)
        {
            Log(logger, callerClass, callerSite, DotPulsarLogLevel.Debug, null, formatString, formatValues);
        }

        public static void DebugException(this IPulsarClientLogger? logger, string callerClass, string callerSite, Exception exception, string formatString, params object[] formatValues)
        {
            Log(logger, callerClass, callerSite, DotPulsarLogLevel.Debug, exception, formatString, formatValues);
        }

        public static void Info(this IPulsarClientLogger? logger, string callerClass, string callerSite, string formatString, params object[] formatValues)
        {
            Log(logger, callerClass, callerSite, DotPulsarLogLevel.Info, null, formatString, formatValues);
        }

        public static void InfoException(this IPulsarClientLogger? logger, string callerClass, string callerSite, Exception exception, string formatString, params object[] formatValues)
        {
            Log(logger, callerClass, callerSite, DotPulsarLogLevel.Info, exception, formatString, formatValues);
        }

        public static void Warn(this IPulsarClientLogger? logger, string callerClass, string callerSite, string formatString, params object[] formatValues)
        {
            Log(logger, callerClass, callerSite, DotPulsarLogLevel.Warn, null, formatString, formatValues);
        }

        public static void WarnException(this IPulsarClientLogger? logger, string callerClass, string callerSite, Exception exception, string formatString, params object[] formatValues)
        {
            Log(logger, callerClass, callerSite, DotPulsarLogLevel.Warn, exception, formatString, formatValues);
        }

        public static void Error(this IPulsarClientLogger? logger, string callerClass, string callerSite, string formatString, params object[] formatValues)
        {
            Log(logger, callerClass, callerSite, DotPulsarLogLevel.Error, null, formatString, formatValues);
        }

        public static void ErrorException(this IPulsarClientLogger? logger, string callerClass, string callerSite, Exception exception, string formatString, params object[] formatValues)
        {
            Log(logger, callerClass, callerSite, DotPulsarLogLevel.Error, exception, formatString, formatValues);
        }

        public static void Fatal(this IPulsarClientLogger? logger, string callerClass, string callerSite, string formatString, params object[] formatValues)
        {
            Log(logger, callerClass, callerSite, DotPulsarLogLevel.Fatal, null, formatString, formatValues);
        }

        public static void FatalException(this IPulsarClientLogger? logger, string callerClass, string callerSite, Exception exception, string formatString, params object[] formatValues)
        {
            Log(logger, callerClass, callerSite, DotPulsarLogLevel.Fatal, exception, formatString, formatValues);
        }

        private static void Log(this IPulsarClientLogger? logger, string callerClass, string callerSite, DotPulsarLogLevel logLevel, Exception? exception, string formatString, params object[] formatValues)
        {
            if (logger == null)
                return;

            logger.Log(logLevel, exception, $"{callerClass}::{callerSite} - {formatString}", formatValues);
        }
    }
}
