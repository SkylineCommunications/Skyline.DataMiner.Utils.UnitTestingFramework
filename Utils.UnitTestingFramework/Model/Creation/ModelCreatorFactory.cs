namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model.Creation
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Enums;

    internal static class ModelCreatorFactory
    {
        public static IParameterHandler Create(EnumParamType paramType, HashSet<int> excludedPids)
        {
            switch (paramType)
            {
                case EnumParamType.Read:
                    return new ReadParameterHandler(excludedPids);

                case EnumParamType.Fixed:
                    return new FixedParameterHandler(excludedPids);

                case EnumParamType.Write:
                    return new WriteParameterHandler(excludedPids);

                case EnumParamType.Array:
                    return new ArrayHandler(excludedPids);

                default:
                    return new UndefinedParamTypeHandler(excludedPids);
            }
        }
    }
}