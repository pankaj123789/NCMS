using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Wiise.HttpOperations
{
    public interface IReadableConfiguration
    {
        string AccessToken { get; }

        IDictionary<string, string> ApiKey { get; }

        IDictionary<string, string> ApiKeyPrefix { get; }

        string BasePath { get; set; }

        string WiiseResource { get; set; }
        string WiiseAuthRedirectUri { get; set; }

        string WiiseAuthClientId { get; set; }

        string WiiseClientSecret { get; set; }

        string DateTimeFormat { get; }

        IDictionary<string, string> DefaultHeader { get; }

        string TempFolderPath { get; }

        int Timeout { get; }

        string UserAgent { get; }

        string Username { get; }

        string Password { get; }

        string GetApiKeyWithPrefix(string apiKeyIdentifier);
    }
}
