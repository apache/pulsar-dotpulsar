namespace DotPulsar.Internal;

using Abstractions;
using Exceptions;
using System;
using System.Threading.Tasks;

internal static class TokenFactoryExtensions
{
    public static async Task<string> GetToken(this Func<Task<string>> tokenFactory, IExecute executor)
    {
        return await executor.Execute(tokenFactory.GetToken);
    }

    public static async Task<string> GetToken(this Func<Task<string>> tokenFactory)
    {
        try
        {
            return await tokenFactory();
        }
        catch (Exception e)
        {
            throw new TokenFactoryFailedException(e);
        }
    }
}
