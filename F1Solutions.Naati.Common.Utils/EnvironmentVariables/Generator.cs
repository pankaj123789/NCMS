using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace F1Solutions.Naati.Common.Utils.EnvironmentVariables
{
    public class Generator : IDisposable
    {
        private RegistryKey _registryKey;
        private const string NCMSPrefix = "ncms";
        private const string MYNAATIPrefix = "mynaati";
        private const string YAMLBasePath = @"..\..\..\Deployment Scripts\Helm\naati\Charts\{0}\templates\{0}-config-map.yaml";
        private readonly IDeserializer _deserializer;

        public Generator()
        {
            _deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();
        }

        public void Dispose()
        {
            if (_registryKey != null)
            {
                _registryKey.Dispose();
            }
        }

        public void Generate()
        {
            ReadEnvironmentRegistry();
            GenerateEnvironmentVariablesFrom(String.Format(YAMLBasePath, NCMSPrefix));
            GenerateEnvironmentVariablesFrom(String.Format(YAMLBasePath, MYNAATIPrefix));
        }

        private void GenerateEnvironmentVariablesFrom(string yamlFilePath)
        {
            var definition = ReadConfigMap(yamlFilePath);
            foreach (var k in definition.Data)
            {
                SetEnvironmentVariable(k);
            }
        }

        private ConfigMap ReadConfigMap(string yamlFilePath)
        {
            var yaml = File.ReadAllText(yamlFilePath);
            var definition = _deserializer.Deserialize<ConfigMap>(yaml);
            return definition;
        }

        private void ReadEnvironmentRegistry()
        {
            _registryKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Environment", true);
        }

        private void SetEnvironmentVariable(KeyValuePair<string, string> variable)
        {
            _registryKey.SetValue(variable.Key, variable.Value);
        }
    }
}
