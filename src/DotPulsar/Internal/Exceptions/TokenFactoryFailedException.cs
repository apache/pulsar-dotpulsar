namespace DotPulsar.Internal.Exceptions;

using System;

public class TokenFactoryFailedException : Exception
{
    public TokenFactoryFailedException(Exception innerException) : base("Exception when trying to fetch token from token factory", innerException)
    {
    }
}
