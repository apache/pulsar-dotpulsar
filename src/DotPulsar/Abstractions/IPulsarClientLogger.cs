namespace DotPulsar.Abstractions
{
    using System;

    public interface IPulsarClientLogger
    {
         void Log(DotPulsarLogLevel logLevel, Exception? optionalException, string formatMessage, params object[] formatValues);
    }
}
