namespace Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Creation
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Enums;

    internal static class ModelCreatorFactory
    {
        public static IDataModelCreator Create(EnumParamType paramType, HashSet<int> excludedPids)
        {
            switch (paramType)
            {
                case EnumParamType.Read:
                    return new ReadParameterModelCreator(excludedPids);

                case EnumParamType.Fixed:
                    return new FixedParameterModelCreator(excludedPids);

                case EnumParamType.Write:
                    return new WriteParameterModelCreator(excludedPids);

                case EnumParamType.Array:
                    return new TableModelCreator(excludedPids);

                default:
                    return new UndefinedParamTypeHandler(excludedPids);
            }
        }
    }
}