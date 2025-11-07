namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Interfaces;

    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model.Creation;

    internal static class ProtocolCacheBuilder
    {
        public static ProtocolCache Build(string customPathToProtocolXml)
        {
            var protocolModel = ProtocolModelBuilder.Build(customPathToProtocolXml);

            return Build(protocolModel);
        }

        /// <summary>
        /// Loads the parameter values of the parameters defined in the protocol.xml file in the cache.
        /// </summary>
        public static ProtocolCache Build(IProtocolModel protocolModel)
        {
            if (protocolModel is null)
            {
                throw new ArgumentNullException(nameof(protocolModel));
            }

            var cache = new ProtocolCache();

            var parameters = protocolModel.Protocol.Params.ToList();
            var excludedPids = new HashSet<int>();

            foreach (var parameter in parameters)
            {
                int parameterId = (int)parameter.Id.Value.Value;
                string parameterName = parameter.Name.Value;

                try
                {
                    var paramType = (EnumParamType)parameter.Type.Value;

                    if (!cache.Parameters.TryGetParameterId(parameterName, out _) || !(paramType is EnumParamType.Read))
                    {
                        cache.Parameters.LoadParameterName(parameterName, parameterId);
                    }

                    var modelCreator = ModelCreatorFactory.Create(paramType, excludedPids);
                    modelCreator.CreateModelAndAddToCache(cache, parameter);
                }
                catch(Exception ex)
                {
                    throw new InvalidOperationException($"An exception occurred while processing protocol parameter '{parameterName}' (ID: {parameterId}). See inner exception for more details.", ex);
                }
            }

            return cache;
        }
    }
}