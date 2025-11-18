namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data;

    internal class FixedParameterHandler : GeneralParameterHandler
    {
        private static readonly Regex HexString = new Regex(@"^(0x[0-9a-fA-F]{2})+$");
        private static readonly string[] hexStringSeparator = new[] { "0x" };
        private readonly HashSet<int> excludedPids;

        public FixedParameterHandler(HashSet<int> excludedPids)
        {
            this.excludedPids = excludedPids ?? throw new ArgumentNullException(nameof(excludedPids));
        }

        protected override void ProcessString(IProtocolCache cache, IParamsParam parameter)
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

            string fixedValueString = parameter.Interprete.ValueElement.Value;
            cache.Parameters.SetParameter(parameterId, fixedValueString, checkIfExists: false);
        }

        protected override void ProcessDouble(IProtocolCache cache, IParamsParam parameter)
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

            string fixedValueString = parameter.Interprete.ValueElement.Value;
            int fixedValueInt = -1;

            if (HexString.IsMatch(fixedValueString))
            {
                string[] parts = fixedValueString.Split(hexStringSeparator, StringSplitOptions.RemoveEmptyEntries);
                fixedValueInt = Int32.Parse(parts[0], System.Globalization.NumberStyles.HexNumber);
            }

            cache.Parameters.SetParameter(parameterId, fixedValueInt, checkIfExists: false);
        }

        private bool IsTitleParameter(IParamsParam parameter)
        {
            return parameter.Measurement?.Type?.Value == EnumParamMeasurementType.Title;
        }
    }
}