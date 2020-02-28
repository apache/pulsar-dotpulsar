using System;
using System.Threading.Tasks;
using DotPulsar.Abstractions;
using DotPulsar.Internal;
using Xunit.Abstractions;

namespace DotPulsar.Stress.Tests
{
    internal class XunitExceptionHandler : IHandleException
    {
        private readonly ITestOutputHelper _output;
        private readonly IHandleException _exceptionHandler;

        public XunitExceptionHandler(ITestOutputHelper output, IHandleException exceptionHandler)
        {
            _output = output;
            _exceptionHandler = exceptionHandler;
        }

        public XunitExceptionHandler(ITestOutputHelper output) : this(output, new DefaultExceptionHandler(TimeSpan.FromSeconds(3)))
        {
        }

        public async ValueTask OnException(ExceptionContext exceptionContext)
        {
            await _exceptionHandler.OnException(exceptionContext);

            if (!exceptionContext.ExceptionHandled)
                _output.WriteLine(
                    $"{exceptionContext.Exception.GetType().Name} " +
                    $"{exceptionContext.Exception.Message}{Environment.NewLine}" +
                    $"{exceptionContext.Exception.StackTrace}");
        }
    }
}