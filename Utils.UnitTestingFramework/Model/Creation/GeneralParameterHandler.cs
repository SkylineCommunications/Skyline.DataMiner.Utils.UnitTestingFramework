namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model.Creation
{
    using System;
    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data;

    internal abstract class GeneralParameterHandler : IParameterHandler
    {
        public void CreateModelAndAddToCache(IProtocolCache cache, IParamsParam parameter)
        {
            if (cache is null)
            {
                throw new ArgumentNullException(nameof(cache));
            }

            if (parameter is null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            if (parameter.Interprete?.Type?.Value == null)
            {
                return;
            }

            var interpreteType = parameter.Interprete.Type.Value.Value;

            switch (interpreteType)
            {
                case EnumParamInterpretType.String:
                    ProcessString(cache, parameter);
                    break;

                case EnumParamInterpretType.Double:
                    ProcessDouble(cache, parameter);
                    break;

                case EnumParamInterpretType.HighNibble:
                    ProcessHighNibble(cache, parameter);
                    break;

                default:
                    ProcessUndefinedType(cache, parameter);
                    break;
            }
        }

        protected abstract void ProcessString(IProtocolCache cache, IParamsParam parameter);

        protected abstract void ProcessDouble(IProtocolCache cache, IParamsParam parameter);

        protected virtual void ProcessHighNibble(IProtocolCache cache, IParamsParam parameter)
        {
            int parameterId = (int)parameter.Id.Value.Value;

            cache.Parameters.SetParameter(parameterId, null, null, false);
        }

        protected virtual void ProcessUndefinedType(IProtocolCache cache, IParamsParam parameter)
        {
            int parameterId = (int)parameter.Id.Value.Value;

            cache.Parameters.SetParameter(parameterId, null, null, false);
        }
    }
}