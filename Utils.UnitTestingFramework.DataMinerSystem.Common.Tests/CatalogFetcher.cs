namespace Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common.Tests
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;

    internal static class CatalogFetcher
    {
        private const string CatalogApiBaseUrl = "https://api.dataminer.services";
        private const string DataMinerServicesApiKeyHeader = "DATAMINER-SERVICES-API-KEY";
        private const string ProtocolXmlFileName = "Protocol.xml";

        private static readonly HttpClient HttpClient = new HttpClient();
        private static readonly ConcurrentDictionary<string, Lazy<string>> ProtocolXmlCache = new ConcurrentDictionary<string, Lazy<string>>(StringComparer.OrdinalIgnoreCase);

        public static string GetProtocolXml(Guid catalogId, string versionId, string dataMinerServicesApiKey)
        {
            if (catalogId == Guid.Empty)
            {
                throw new ArgumentException("The Catalog ID cannot be empty.", nameof(catalogId));
            }

            if (String.IsNullOrWhiteSpace(versionId))
            {
                throw new ArgumentException("The Catalog version ID cannot be empty or white space.", nameof(versionId));
            }

            if (String.IsNullOrWhiteSpace(dataMinerServicesApiKey))
            {
                throw new ArgumentException("The DataMiner Services API key cannot be empty or white space.", nameof(dataMinerServicesApiKey));
            }

            string cacheKey = $"{catalogId:D}/{versionId}";
            var protocolXml = ProtocolXmlCache.GetOrAdd(
                cacheKey,
                _ => new Lazy<string>(
                    () => DownloadProtocolXml(catalogId, versionId, dataMinerServicesApiKey),
                    LazyThreadSafetyMode.ExecutionAndPublication));

            try
            {
                return protocolXml.Value;
            }
            catch
            {
                ProtocolXmlCache.TryRemove(cacheKey, out _);
                throw;
            }
        }

        private static string DownloadProtocolXml(Guid catalogId, string versionId, string dataMinerServicesApiKey)
        {
            string escapedVersionId = Uri.EscapeDataString(versionId);
            string requestUrl = $"{CatalogApiBaseUrl}/api/key-catalog/v2-0/{catalogId:D}/versions/{escapedVersionId}/download";

            using (var request = new HttpRequestMessage(HttpMethod.Get, requestUrl))
            {
                request.Headers.Add(DataMinerServicesApiKeyHeader, dataMinerServicesApiKey);

                using (var response = HttpClient.SendAsync(request).GetAwaiter().GetResult())
                {
                    response.EnsureSuccessStatusCode();
                    byte[] package = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
                    return ExtractProtocolXml(package);
                }
            }
        }

        private static string ExtractProtocolXml(byte[] package)
        {
            using (var packageStream = new MemoryStream(package, writable: false))
            using (var archive = new ZipArchive(packageStream, ZipArchiveMode.Read, leaveOpen: false))
            {
                var protocolXmlEntry = archive.Entries.SingleOrDefault(entry =>
                    String.Equals(Path.GetFileName(entry.FullName), ProtocolXmlFileName, StringComparison.OrdinalIgnoreCase));

                if (protocolXmlEntry == null)
                {
                    throw new InvalidDataException($"The downloaded Catalog package does not contain '{ProtocolXmlFileName}'.");
                }

                using (var protocolXmlStream = protocolXmlEntry.Open())
                using (var reader = new StreamReader(protocolXmlStream, detectEncodingFromByteOrderMarks: true))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
