namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model.Creation
{
    using System;
    using System.Linq;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Interfaces;

    internal class ProtocolModelParameterFinder : IProtocolModelParameterFinder
    {
        private readonly IProtocolModel protocolModel;

        public ProtocolModelParameterFinder(IProtocolModel protocolModel)
        {
            this.protocolModel = protocolModel ?? throw new ArgumentNullException(nameof(protocolModel));
        }

        public IParamsParam FindParameter(int parameterId)
        {
            var parameter = protocolModel.Protocol.Params.SingleOrDefault(p => p.Id.Value.Value == parameterId);

            return parameter;
        }
    }
}