namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model.Parameter
{
    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data;

    internal abstract class GeneralParameterHandler : ISingleParameterHandler
    {
        public void LoadDefaultForParameter(IProtocolCache cache, IParamsParam parameter)
        {
            if (parameter.Interprete == null
                || parameter.Interprete.Type == null
                || parameter.Interprete.Type.Value == null)
            {
                return;
            }

            EnumParamInterpretType interpretType = (EnumParamInterpretType)parameter.Interprete.Type.Value;

            switch (interpretType)
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

        public abstract void ProcessString(IProtocolCache cache, IParamsParam parameter);

        public abstract void ProcessDouble(IProtocolCache cache, IParamsParam parameter);

        public virtual void ProcessHighNibble(IProtocolCache cache, IParamsParam parameter)
        {
            int parameterId = (int)parameter.Id.Value.Value;

            cache.Parameters.SetParameter(parameterId, null, null, false);
        }

        public virtual void ProcessUndefinedType(IProtocolCache cache, IParamsParam parameter)
        {
            int parameterId = (int)parameter.Id.Value.Value;

            cache.Parameters.SetParameter(parameterId, null, null, false);
        }
    }
}