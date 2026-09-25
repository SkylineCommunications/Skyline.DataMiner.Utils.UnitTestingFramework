namespace Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common.Tests
{
    using System;
    using System.Reflection;

    using Microsoft.Extensions.Configuration;

    internal sealed class Config
    {
        private const string DataMinerServicesApiKeyConfigurationKey = "DataMinerServicesApiKey";

        private Config(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            DataMinerServicesApiKey = configuration[DataMinerServicesApiKeyConfigurationKey];

            if (String.IsNullOrWhiteSpace(DataMinerServicesApiKey))
            {
                throw new InvalidOperationException(
                    $"Configuration value '{DataMinerServicesApiKeyConfigurationKey}' is missing. " +
                    $"Set it with 'dotnet user-secrets set \"{DataMinerServicesApiKeyConfigurationKey}\" \"<your-api-key>\"' " +
                    "from the DataMinerSystem.Common.Tests project folder, or provide it as an environment variable.");
            }
        }

        public string DataMinerServicesApiKey { get; }

        public static Config Load()
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets(Assembly.GetExecutingAssembly())
                .AddEnvironmentVariables()
                .Build();

            return new Config(configuration);
        }
    }
}
