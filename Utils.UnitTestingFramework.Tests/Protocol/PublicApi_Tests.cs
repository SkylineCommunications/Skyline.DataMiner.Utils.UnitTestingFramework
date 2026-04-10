namespace Skyline.DataMiner.Utils.UnitTestingFramework.Tests.Protocol
{
	using System.Reflection;
	using System.Threading.Tasks;

	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using PublicApiGenerator;

	using VerifyMSTest;

	[TestClass]
	[UsesVerify]
	public sealed partial class PublicApi_Tests
	{
		[TestMethod]
		public Task NoPublicApiChanges_Protocol()
		{
			// Arrange
			var assemblyName = "Skyline.DataMiner.Utils.UnitTestingFramework.Protocol";

			// Act
			var publicApi = Assembly.Load(assemblyName).GeneratePublicApi();

			// Assert
			return Verifier.Verify(publicApi)
				.UseFileName($"{assemblyName}_PublicApi")
				.AutoVerify(includeBuildServer: false, throwException: true);
		}
	}
}
