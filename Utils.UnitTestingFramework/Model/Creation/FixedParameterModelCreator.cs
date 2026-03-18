namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model.Standalone;

    internal class FixedParameterModelCreator : ParameterModelCreatorBase
    {
        private static readonly Regex HexString = new Regex(@"^(0x[0-9a-fA-F]{2})+$");
        private static readonly string[] hexStringSeparator = new[] { "0x" };
        private readonly HashSet<int> excludedPids;

        public FixedParameterModelCreator(HashSet<int> excludedPids)
        {
            this.excludedPids = excludedPids ?? throw new ArgumentNullException(nameof(excludedPids));
        }

        protected override void ProcessString(ElementData element, IParamsParam parameter)
        {
            int parameterId = (int)parameter.Id.Value.Value;

            if (excludedPids.Contains(parameterId))
            {
                return;
            }

            if (IsTitleParameter(parameter))
            {
                // Skip title parameters
                return;
            }

            var parameterDefinition = new ParameterDefinition(parameter.Name.Value, GetTypeForDefinition(parameter), parameterId);

            string fixedValueString = parameter.Interprete.ValueElement.Value;
            var parameterModel = new ParameterModel(parameterDefinition, fixedValueString);

            element.AddParameter(parameterModel);
        }

        protected override void ProcessDouble(ElementData elementData, IParamsParam parameter)
        {
            int parameterId = (int)parameter.Id.Value.Value;

            if (excludedPids.Contains(parameterId))
            {
                return;
            }

            if (IsTitleParameter(parameter))
            {
                // Skip title parameters
                return;
            }

            var parameterDefinition = new ParameterDefinition(parameter.Name.Value, GetTypeForDefinition(parameter), parameterId);

            string fixedValueString = parameter.Interprete.ValueElement.Value;
            int fixedValueInt = -1;

            if (HexString.IsMatch(fixedValueString))
            {
                string[] parts = fixedValueString.Split(hexStringSeparator, StringSplitOptions.RemoveEmptyEntries);
                fixedValueInt = Int32.Parse(parts[0], System.Globalization.NumberStyles.HexNumber);
            }

            var parameterModel = new ParameterModel(parameterDefinition, fixedValueInt);

            elementData.AddParameter(parameterModel);
        }

        private bool IsTitleParameter(IParamsParam parameter)
        {
            return parameter.Measurement?.Type?.Value == EnumParamMeasurementType.Title;
        }
    }
}