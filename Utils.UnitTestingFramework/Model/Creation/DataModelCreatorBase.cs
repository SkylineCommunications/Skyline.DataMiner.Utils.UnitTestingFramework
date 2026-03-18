namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model.Creation
{
    using System;
    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;

    internal abstract class DataModelCreatorBase
    {
        protected Type GetTypeForDefinition(IParamsParam param)
        {
            // Map the column parameter type to the appropriate .NET type

            var interpreteType = param.Interprete.Type.Value.Value;

            switch (interpreteType)
            {
                case EnumParamInterpretType.String:
                    return typeof(string);

                case EnumParamInterpretType.Double:
                    return typeof(double);

                default:
                    return typeof(object);
            }
        }
    }
}