namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Interfaces;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data;

    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model.Parameter;

    /// <summary>
    /// Extended protocol model.
    /// </summary>
    /// <seealso cref="IProtocolModelExt" />
    public class ProtocolModelExt : IProtocolModelExt
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProtocolModelExt"/> class.
        /// </summary>
        /// <remarks>This assumes that the protocol.xml file resided in the same folder as the Visual Studio solution (.sln) file.</remarks>
        public ProtocolModelExt()
        {
            DirectoryInfo path = GetDirectory();
            var protocolPath = path.FullName + "\\protocol.xml";

            ProtocolModel = new ProtocolModel(File.ReadAllText(protocolPath));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtocolModelExt"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
        public ProtocolModelExt(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            ProtocolModel = new ProtocolModel(File.ReadAllText(path));
        }

        public IProtocolModel ProtocolModel { get; }

        /// <summary>
        /// Loads the parameter values of the parameters defined in the protocol.xml file in the cache.
        /// </summary>
        /// <param name="cache">The protocol cache.</param>
        /// <exception cref="ArgumentNullException"><paramref name="cache"/> is <see langword="null"/>.</exception>
        public void LoadParameterValues(IProtocolCache cache)
        {
            if (cache == null)
            {
                throw new ArgumentNullException(nameof(cache));
            }

            List<IParamsParam> parameters = ProtocolModel.Protocol.Params.ToList();
            HashSet<int> excludedPids = new HashSet<int>();

            foreach (IParamsParam parameter in parameters)
            {
                int parameterId = (int)parameter.Id.Value.Value;
                string parameterName = parameter.Name.Value;

                EnumParamType paramType = (EnumParamType)(parameter.Type.Value);

                if (!cache.Parameters.TryGetParameterId(parameterName, out _) || !(paramType is EnumParamType.Read))
                {
                    cache.Parameters.LoadParameterName(parameterName, parameterId);
                }

                var handler = HandlerParameterFactory.GetHandlerForType(paramType, excludedPids);
                handler.LoadDefaultForParameter(cache, parameter);
            }
        }

        private static DirectoryInfo GetDirectory(string currentPath = null)
        {
            var directory = new DirectoryInfo(currentPath ?? Directory.GetCurrentDirectory());

            while (directory != null && !directory.GetFiles("*.sln").Any())
            {
                directory = directory.Parent;
            }

            return directory;
        }
    }
}