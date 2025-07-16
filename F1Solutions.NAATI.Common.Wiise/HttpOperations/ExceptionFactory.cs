using System;


namespace F1Solutions.Naati.Common.Wiise.HttpOperations
{
    public delegate Exception ExceptionFactory(string methodName, IApiResponse response);
}
