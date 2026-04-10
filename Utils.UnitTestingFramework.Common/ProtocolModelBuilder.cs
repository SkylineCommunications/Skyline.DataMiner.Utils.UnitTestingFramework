namespace Skyline.DataMiner.Utils.UnitTestingFramework.Common
{
    using System;
    using System.IO;
    using System.Linq;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Interfaces;

    internal static class ProtocolModelBuilder
    {
        internal static IProtocolModel Build(string customPathToProtocolXml)
        {
            string pathToProtocolXml = String.IsNullOrEmpty(customPathToProtocolXml) ? GetSolutionDirectory().FullName + "\\protocol.xml" : customPathToProtocolXml;

            if (!File.Exists(pathToProtocolXml))
            {
                throw new FileNotFoundException($"Protocol XML file not found at path: '{pathToProtocolXml}'");
            }

            var protocolModel = new ProtocolModel(File.ReadAllText(pathToProtocolXml));

            return protocolModel;
        }

        private static DirectoryInfo GetSolutionDirectory()
        {
            var directory = new DirectoryInfo(Directory.GetCurrentDirectory());

            while (directory != null && !directory.ContainsSolutionFile())
            {
                directory = directory.Parent;
            }

            return directory;
        }

        private static bool ContainsSolutionFile(this DirectoryInfo directory)
        {
            return directory.GetFiles("*.sln").Any() || directory.GetFiles("*.slnx").Any();
        }
    }
}