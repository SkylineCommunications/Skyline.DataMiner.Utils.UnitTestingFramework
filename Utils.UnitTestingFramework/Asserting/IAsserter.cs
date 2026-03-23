namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Asserting
{
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Standalone;

    /// <summary>
    /// Assertion interface.
    /// </summary>
    public interface IAsserter
    {
        /// <summary>
        /// Retrieves the table model of the parameter with the specified table ID.
        /// </summary>
        /// <param name="tablePid">The table ID.</param>
        /// <returns>The table model reader.</returns>
        ITableAsserter Table(int tablePid);

        /// <summary>
        /// Retrieves the parameter model of the parameter with the specified ID.
        /// </summary>
        /// <param name="parameterId">The parameter ID.</param>
        /// <returns>The parameter model.</returns>
        IParameterAsserter Parameter(int parameterId);

        /// <summary>
        /// Retrieves the parameter model of the parameter with the specified name.
        /// </summary>
        /// <param name="parameterName">The parameter name.</param>
        /// <returns>The parameter model.</returns>
        IParameterAsserter Parameter(string parameterName);
    }
}