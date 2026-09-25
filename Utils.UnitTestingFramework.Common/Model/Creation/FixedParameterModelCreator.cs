namespace Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;

    internal class FixedParameterModelCreator : ParameterModelCreatorBase
    {
        private static readonly Regex HexString = new Regex(@"^(0x[0-9a-fA-F]{2})+$");
        private static readonly string[] HexStringSeparator = new[] { "0x" };

        private readonly HashSet<int> excludedPids;

        public FixedParameterModelCreator(HashSet<int> excludedPids)
        {
            this.excludedPids = excludedPids ?? throw new ArgumentNullException(nameof(excludedPids));
        }

        protected override void ProcessString(ParameterAndTableDefinitions definitions, IParamsParam parameter)
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

            var parameterDefinition = BuildDefinitionFromProtocolParameter(parameter, parameter.Interprete.ValueElement?.Value);

            definitions.AddParameterDefinition(parameterDefinition);
        }

        protected override void ProcessDouble(ParameterAndTableDefinitions definitions, IParamsParam parameter)
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

            var parameterDefinition = BuildDefinitionFromProtocolParameter(parameter, GetDoubleInitialValue(parameter.Interprete.ValueElement?.Value));

            definitions.AddParameterDefinition(parameterDefinition);
        }

        private bool IsTitleParameter(IParamsParam parameter)
        {
            return parameter.Measurement?.Type?.Value == EnumParamMeasurementType.Title;
        }

        private static object GetDoubleInitialValue(string value)
        {
            if (value == null)
            {
                return null;
            }

            if (!HexString.IsMatch(value))
            {
                return -1;
            }

            string[] parts = value.Split(HexStringSeparator, StringSplitOptions.RemoveEmptyEntries);
            return Int32.Parse(parts[0], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        }
    }
}
