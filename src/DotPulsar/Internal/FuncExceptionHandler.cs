namespace DotPulsar.Internal
{
    using DotPulsar.Abstractions;
    using System;
    using System.Threading.Tasks;

    public sealed class FuncExceptionHandler : IHandleException
    {
        private readonly Func<ExceptionContext, ValueTask> _exceptionHandler;

        public FuncExceptionHandler(Func<ExceptionContext, ValueTask> exceptionHandler)
            => _exceptionHandler = exceptionHandler;

        public ValueTask OnException(ExceptionContext exceptionContext)
            => _exceptionHandler(exceptionContext);
    }
}
