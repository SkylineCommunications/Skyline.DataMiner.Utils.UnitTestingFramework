namespace Skyline.DataMiner.Utils.UnitTestingFramework.Dev.Common.Dom
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Builder class for constructing a <see cref="DomConfiguration"/> instance.
    /// </summary>
    public class DomConfigurationBuilder
    {
        private readonly List<Func<DomModule>> modules = new List<Func<DomModule>>();

        /// <summary>
        /// Adds a module to the configuration.
        /// </summary>
        /// <param name="moduleId">The ID of the module.</param>
        /// <param name="configureModule">An action to configure the module.</param>
        /// <returns>The current <see cref="DomConfigurationBuilder"/> instance.</returns>
        public DomConfigurationBuilder WithModule(string moduleId, Action<DomModuleBuilder> configureModule)
        {
            modules.Add(() =>
            {
                var builder = new DomModuleBuilder(moduleId);
                configureModule(builder);
                return builder.Build();
            });

            return this;
        }

        /// <summary>
        /// Builds the <see cref="DomConfiguration"/> instance based on the configured modules.
        /// </summary>
        /// <returns>The constructed <see cref="DomConfiguration"/> instance.</returns>
        public DomConfiguration Build()
        {
            var domConfiguration = new DomConfiguration(modules.ConvertAll(m => m()));

            return domConfiguration;
        }
    }
}
