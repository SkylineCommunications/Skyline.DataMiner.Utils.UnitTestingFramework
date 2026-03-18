namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data
{
    using System;
    using System.Collections.Generic;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Interfaces;

    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model.Creation;

    internal static class ElementDataBuilder
    {
        public static ElementData Build(string customPathToProtocolXml)
        {
            var protocolModel = ProtocolModelBuilder.Build(customPathToProtocolXml);

            return Build(protocolModel);
        }

        /// <summary>
        /// Loads the parameter values of the parameters defined in the protocol.xml file in the cache.
        /// </summary>
        public static ElementData Build(IProtocolModel protocolModel)
        {
            if (protocolModel is null)
            {
                throw new ArgumentNullException(nameof(protocolModel));
            }

            var elementData = new ElementData();
            var excludedPids = new HashSet<int>();

            var protocolModelParameterFinder = new ProtocolModelParameterFinder(protocolModel);

            foreach (var parameter in protocolModel.Protocol.Params)
            {
                try
                {
                    var parameterType = parameter.Type.Value.Value;

                    var modelCreator = ModelCreatorFactory.Create(parameterType, excludedPids);
                    modelCreator.CreateModelAndAddToElementData(elementData, parameter, protocolModelParameterFinder);
                }
                catch(Exception ex)
                {
                    throw new InvalidOperationException($"An exception occurred while processing protocol parameter '{parameter.Name.Value}' (ID: {(int)parameter.Id.Value.Value}). See inner exception for more details.", ex);
                }
            }

            return elementData;
        }
    }
}