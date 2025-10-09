namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Interfaces;

    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model.Creation;

    internal static class ProtocolCacheBuilder
    {
        /// <summary>
        /// Loads the parameter values of the parameters defined in the protocol.xml file in the cache.
        /// </summary>
        public static ProtocolCache Build(string customPathToProtocolXml = null)
        {
            var protocolModel = GetProtocolModel(customPathToProtocolXml);

            var cache = new ProtocolCache();

            var parameters = protocolModel.Protocol.Params.ToList();
            var excludedPids = new HashSet<int>();

            foreach (var parameter in parameters)
            {
                int parameterId = (int)parameter.Id.Value.Value;
                string parameterName = parameter.Name.Value;

                var paramType = (EnumParamType)parameter.Type.Value;

                if (!cache.Parameters.TryGetParameterId(parameterName, out _) || !(paramType is EnumParamType.Read))
                {
                    cache.Parameters.LoadParameterName(parameterName, parameterId);
                }

                var modelCreator = ModelCreatorFactory.Create(paramType, excludedPids);
                modelCreator.CreateModelAndAddToCache(cache, parameter);
            }

            return cache;
        }

        internal static IProtocolModel GetProtocolModel(string customPathToProtocolXml)
        {
            IProtocolModel protocolModel;
            if (customPathToProtocolXml == null)
            {
                var solutionDirectory = GetSolutionDirectory();
                string protocolPath = solutionDirectory.FullName + "\\protocol.xml";

                protocolModel = new ProtocolModel(File.ReadAllText(protocolPath));
            }
            else
            {
                protocolModel = new ProtocolModel(File.ReadAllText(customPathToProtocolXml));
            }

            return protocolModel;
        }

        private static DirectoryInfo GetSolutionDirectory(string currentPath = null)
        {
            var directory = new DirectoryInfo(currentPath ?? Directory.GetCurrentDirectory());

            while (directory != null && !directory.GetFiles("*.sln").Any())
            {
                directory = directory.Parent;
            }

            return directory;
        }
    }
}