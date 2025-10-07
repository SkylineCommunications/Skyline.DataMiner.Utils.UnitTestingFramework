namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data
{
    using System;
    using System.Collections.Generic;

    using Moq;

    using Skyline.DataMiner.Scripting;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Constants;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model;

    /// <summary>
    /// Represents the parameter cache.
    /// </summary>
    public class ParametersCache
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParametersCache"/> class.
        /// </summary>
        public ParametersCache()
        {
            ParametersToValues = new Dictionary<int, IParameterModel>();
            NamesToParameterID = new Dictionary<string, int>();
            ParameterIDToNames = new Dictionary<int, string>();
        }

        /// <summary>
        /// Gets the parameter IDs to values dictionary.
        /// </summary>
        /// <value>
        /// The parameter IDs to values dictionary.
        /// </value>
        protected IDictionary<int, IParameterModel> ParametersToValues { get; }

        /// <summary>
        /// Gets the names to parameter ID dictionary.
        /// </summary>
        /// <value>
        /// The names to parameter ID dictionary.
        /// </value>
        protected IDictionary<string, int> NamesToParameterID { get; }

        /// <summary>
        /// Gets parameter ID to names dictionary.
        /// </summary>
        /// <value>
        /// The parameter ID to names dictionary.
        /// </value>
        protected IDictionary<int, string> ParameterIDToNames { get; }

        /// <summary>
        /// Sets the name of the parameter with the specified ID.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="id">The identifier.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        public void LoadParameterName(string name, int id)
        {
            NamesToParameterID[name] = id;
            ParameterIDToNames[id] = name;
        }

        /// <summary>
        /// Loads the specified parameter IDs and corresponding values in the parameter cache.
        /// </summary>
        /// <param name="parameterIds">The parameter IDs.</param>
        /// <param name="values">The values.</param>
        /// <param name="timestamps">The timestamps.</param>
        /// <exception cref="ArgumentNullException"><paramref name="parameterIds"/>, <paramref name="values"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Size of <paramref name="values"/> or <paramref name="timestamps"/> is smaller than <paramref name="parameterIds"/>.</exception>
        public void LoadParameters(IList<int> parameterIds, IList<object> values, IList<DateTime> timestamps = null)
        {
            if (parameterIds == null)
            {
                throw new ArgumentNullException(nameof(parameterIds));
            }

            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (values.Count < parameterIds.Count)
            {
                throw new ArgumentException($"Size of '{nameof(values)}' list is smaller than the '{nameof(parameterIds)}' list.");
            }

            if (timestamps != null && timestamps.Count < parameterIds.Count)
            {
                throw new ArgumentException($"Size of '{nameof(timestamps)}' list is smaller than the '{nameof(parameterIds)}' list.");
            }

            for (int index = 0; index < parameterIds.Count; index++)
            {
                if (timestamps != null)
                {
                    SetParameter(parameterIds[index], values[index], timestamps[index], false);
                }
                else
                {
                    SetParameter(parameterIds[index], values[index], null, false);
                }
            }
        }

        /// <summary>
        /// Gets the parameter model.
        /// </summary>
        /// <param name="parameterId">The parameter ID.</param>
        /// <returns>The parameter model.</returns>
        /// <exception cref="ArgumentException">There is no parameter with ID <paramref name="parameterId"/>.</exception>
        public IParameterModel GetParameterModel(int parameterId)
        {
            if (ParametersToValues.TryGetValue(parameterId, out IParameterModel output))
            {
                return output;
            }

            throw new ArgumentException($"There is no parameter with ID '{parameterId}'");
        }

        /// <summary>
        /// Gets the parameter model.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <returns>The parameter model.</returns>
        /// <exception cref="ArgumentException">There is no parameter with name <paramref name="parameterName"/>.</exception>
        public IParameterModel GetParameterModel(string parameterName)
        {
            if (NamesToParameterID.TryGetValue(parameterName, out int parameterId))
            {
                return GetParameterModel(parameterId);
            }

            throw new ArgumentException($"There is no parameter with name '{parameterName}'");
        }

        /// <summary>
        /// Tries retrieving the parameter model of the parameter with the specified ID.
        /// </summary>
        /// <param name="parameterId">The parameter ID.</param>
        /// <param name="parameterModel">The parameter model.</param>
        /// <returns><c>true</c> if a parameter with the specified ID was found; otherwise, <c>false</c>.</returns>
        public bool TryGetParameterModel(int parameterId, out IParameterModel parameterModel)
        {
            return ParametersToValues.TryGetValue(parameterId, out parameterModel);
        }

        /// <summary>
        /// Tries retrieving the parameter model of the parameter with the specified name.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="parameterModel">The parameter model.</param>
        /// <returns><c>true</c> if a parameter with the specified name was found; otherwise, <c>false</c>.</returns>
        public bool TryGetParameterModel(string parameterName, out IParameterModel parameterModel)
        {
            if (NamesToParameterID.TryGetValue(parameterName, out int parameterId))
            {
                parameterModel = GetParameterModel(parameterId);
                return true;
            }

            parameterModel = null;
            return false;
        }

        /// <summary>
        /// Tries the get parameter ID of the parameter with the specified name.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="parameterId">The parameter ID.</param>
        /// <returns><c>true</c> if a parameter was found with the specified name; otherwise, <c>false</c>.</returns>
        public bool TryGetParameterId(string parameterName, out int parameterId)
        {
            if (NamesToParameterID.TryGetValue(parameterName, out parameterId))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the parameter value.
        /// </summary>
        /// <param name="pid">The parameter ID.</param>
        /// <returns>The parameter value.</returns>
        public object GetParameter(int pid)
        {
            if (ParametersToValues.TryGetValue(pid, out IParameterModel value))
            {
                return value.Value;
            }

            throw new ArgumentException($"There is no parameter with ID '{pid}'");
        }

        /// <summary>
        /// Tries retrieving the parameter value of the parameter with the specified ID.
        /// </summary>
        /// <param name="pid">The parameter ID.</param>
        /// <param name="value">The parameter value.</param>
        /// <returns><c>true</c> if a parameter was found with the specified ID; otherwise, <c>false</c>.</returns>
        public bool TryGetParameter(int pid, out object value)
        {
            if (ParametersToValues.TryGetValue(pid, out IParameterModel parameterModel))
            {
                value = parameterModel.Value;
                return true;
            }

            value = null;
            return false;
        }

        /// <summary>
        /// Gets the value of the parameter with the specified name.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <returns>The parameter value.</returns>
        public object GetParameterByName(string name)
        {
            if (NamesToParameterID.TryGetValue(name, out int parameterID))
            {
                return GetParameter(parameterID);
            }

            return null;
        }

        /// <summary>
        /// Tries retrieving the parameter value of the parameter with the specified name.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The parameter value.</param>
        /// <returns><c>true</c> if a parameter was found with the specified name; otherwise, <c>false</c>.</returns>
        public bool TryGetParameterByName(string name, out object value)
        {
            if (NamesToParameterID.TryGetValue(name, out int parameterID))
            {
                value = GetParameter(parameterID);
                return true;
            }

            value = null;
            return false;
        }

        /// <summary>
        /// Gets the parameters names by their pid. This method is NOT in the SlProtocol.
        /// </summary>
        /// <param name="pid">The collection of pid.</param>
        /// <returns>An array with the names of the parameters. If a position in the collection
        /// is null it is because the parameter does not have a name set, the parameter ID
        /// is invalid or does not exist.</returns>
        public string[] GetParametersNamesByPID(int[] pid)
        {
            int arraySize = pid.Length;
            string[] paramNames = new string[arraySize];

            for (int i = 0; i < arraySize; i++)
            {
                if (ParameterIDToNames.TryGetValue(pid[i], out string paramName))
                {
                    paramNames[i] = paramName;
                }
                else
                {
                    paramNames[i] = null;
                }
            }

            return paramNames;
        }

        /// <summary>
        /// Gets the parameter name by its pid. This method is NOT in the SlProtocol.
        /// </summary>
        /// <param name="parameterId">The pid.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <returns><c>true</c> if a parameter was found with the specified ID; otherwise, <c>false</c>.</returns>
        public bool TryGetParameterNameByPID(int parameterId, out string parameterName)
        {
            if (ParameterIDToNames.TryGetValue(parameterId, out parameterName))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the values of parameters with the specified IDs.
        /// </summary>
        /// <param name="parameters">The IDs of the parameters.</param>
        /// <returns>The parameter values.</returns>
        public object GetParameters(uint[] parameters)
        {
            int length = parameters.Length;
            object[] output = new object[length];

            for (int i = 0; i < length; i++)
            {
                if (ParametersToValues.TryGetValue((int)parameters[i], out IParameterModel value))
                {
                    output[i] = value.Value;
                }
            }

            return output;
        }

        /// <summary>
        /// Sets the parameter.
        /// </summary>
        /// <param name="pid">The parameter ID.</param>
        /// <param name="value">The value.</param>
        /// <param name="timestamp">The timestamp.</param>
        /// <param name="checkIfExists">if set to <c>true</c> [check if exists].</param>
        /// <returns>Value 0.</returns>
        /// <remarks>The set will only be executed if either <paramref name="checkIfExists"/> was set to <c>false</c>, or if the cache contains a parameter with the specified ID.</remarks>
        public int SetParameter(int pid, object value, DateTime? timestamp = null, bool checkIfExists = true)
        {
            var parameterModel = new ParameterModel(value, timestamp);
            
            if (!checkIfExists || ParametersToValues.ContainsKey(pid))
            {
                ParametersToValues[pid] = parameterModel;
            }

            return 0;
        }

        /// <summary>
        /// Sets the parameter with the specified name to the specified value.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value to set.</param>
        /// <returns>Value 0.</returns>
        public int SetParameterByName(string name, object value)
        {
            if (NamesToParameterID.TryGetValue(name, out int pid))
            {
                return SetParameter(pid, value);
            }

            return 0;
        }

        /// <summary>
        /// Sets the parameters with the specified name to the specified values.
        /// </summary>
        /// <param name="names">The parameter names.</param>
        /// <param name="values">The values to set.</param>
        /// <returns>Array containing the result code for each item.</returns>
        public object SetParametersByName(string[] names, object[] values)
        {
            if (names.Length != values.Length)
            {
                return Constants.HRESULT_FAIL_DIFFLEN;
            }

            int length = names.Length;
            var result = new uint[length];

            for (int index = 0; index < names.Length; index++)
            {
                SetParameterByName(names[index], values[index]);
                result[index] = 0;
            }

            return result;
        }

        /// <summary>
        /// Sets the parameters with the specified IDs to the specified values.
        /// </summary>
        /// <param name="parameterIDs">The parameter IDs.</param>
        /// <param name="values">The values to set.</param>
        /// <param name="timestamps">The timestamps.</param>
        /// <returns>Array containing the result code for each item.</returns>
        public object SetParameters(int[] parameterIDs, object[] values, DateTime[] timestamps = null)
        {
            if (parameterIDs.Length != values.Length || timestamps != null && parameterIDs.Length != timestamps.Length)
            {
                return Constants.HRESULT_FAIL_DIFFLEN;
            }

            timestamps = timestamps ?? new DateTime[parameterIDs.Length];

            int length = parameterIDs.Length;
            int[] result = new int[length];

            for (int index = 0; index < length; index++)
            {
                var parameterModel = new ParameterModel(values[index], timestamps[index]);

                if (ParametersToValues.ContainsKey(parameterIDs[index]))
                {
                    ParametersToValues[parameterIDs[index]] = parameterModel;

                    result[index] = 0;
                }
                else
                {
                    result[index] = Constants.HRESULT_FAIL_IDINEXISTENT;
                }
            }

            return result;
        }

        /// <summary>
        /// Sets the parameters with the specified name to the specified values.
        /// </summary>
        /// <param name="iID">The ID of the parameter.</param>
        /// <param name="mock">The mock of the SLProtocol.</param>
        /// <returns>True if the value has been initialized. Otherwise, false.</returns>
        public bool IsEmpty(int iID, Mock<SLProtocol> mock)
        {
            if (ParametersToValues.TryGetValue(iID, out IParameterModel output))
            {
                if (output.Value == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            mock.Object.Log($"NT_GET_DATA for '{iID}' failed. 0x80040239");
            return true;
        }
    }
}