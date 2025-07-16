using System;
using System.Configuration;
using F1Solutions.Global.Common.SystemLifecycle;

namespace F1Solutions.Naati.Common.Bl
{
    public class EnvironmentConfigurationHelper
    {
        public EnvironmentDetailsDto GetEnvironmentDetails()
        {
            var environmentTypeName = ConfigurationManager.AppSettings["EnvironmentType"];
            var environmentName = ConfigurationManager.AppSettings["EnvironmentName"];
            var envType = (EnvironmentTypeName)Enum.Parse(typeof(EnvironmentTypeName), environmentTypeName);
            return new EnvironmentDetailsDto(envType, environmentName);
        }
    }
    
    

    public class EnvironmentDetailsDto
    {
        public EnvironmentDetailsDto (EnvironmentTypeName environtmentType, string environmentName )
        {
            EnvironmentType = environtmentType;
            EnvironmentName = environmentName;
            EnvironmentDisplayName = environtmentType != EnvironmentTypeName.Prod ? EnvironmentName.ToUpper() + " Environment" : string.Empty;


        }
        public EnvironmentTypeName EnvironmentType { get; }
        public string EnvironmentName { get; }

        public string EnvironmentDisplayName { get; }
    }
}
