namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol
{
    using System.IO;
    using System.Linq;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Interfaces;

    internal static class ProtocolModelBuilder
    {
        internal static IProtocolModel Build(string customPathToProtocolXml)
        {
            IProtocolModel protocolModel;
            if (customPathToProtocolXml == null)
            {
                var solutionDirectory = GetSolutionDirectory();
                string protocolPath = solutionDirectory.FullName + "\\protocol.xml";

                protocolModel = new ProtocolModel(File.ReadAllText(protocolPath));
            }
            else
            {
                protocolModel = new ProtocolModel(File.ReadAllText(customPathToProtocolXml));
            }

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