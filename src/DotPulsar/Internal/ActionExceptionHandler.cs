namespace DotPulsar.Internal
{
    using DotPulsar.Abstractions;
    using System;
    using System.Threading.Tasks;

    public sealed class ActionExceptionHandler : IHandleException
    {
        private readonly Action<ExceptionContext> _exceptionHandler;

        public ActionExceptionHandler(Action<ExceptionContext> exceptionHandler)
            => _exceptionHandler = exceptionHandler;

        public ValueTask OnException(ExceptionContext exceptionContext)
        {
            _exceptionHandler(exceptionContext);
            return new ValueTask();
        }
    }
}
