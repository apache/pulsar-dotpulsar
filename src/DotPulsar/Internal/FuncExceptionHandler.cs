namespace DotPulsar.Internal
{
    using System;
    using System.Threading.Tasks;
    using DotPulsar.Abstractions;

    public sealed class FuncExceptionHandler : IHandleException
    {
        private readonly Func<ExceptionContext, ValueTask> _exceptionHandler;

        public FuncExceptionHandler(Func<ExceptionContext, ValueTask> exceptionHandler)
            => _exceptionHandler = exceptionHandler;

        public ValueTask OnException(ExceptionContext exceptionContext)
            => _exceptionHandler(exceptionContext);
    }
}
