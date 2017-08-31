using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using OloApiClient.Models;
using System.IO;

namespace OloApiClient.Test
{
    /// <summary>
    /// Shared Methods for Test Cases
    /// </summary>
    public static class SharedMethods
    {
        /// <summary>
        /// Get Batch List
        /// </summary>
        /// <param name="client">OloClient</param>
        /// <param name="testContext">(sic)</param>
        /// <returns>Last Batch Id</returns>
        public static int GetList(OloClient client, TestContext testContext)
        {
            var lastBatchId = 0;

            string oeText = client.GetJsonAsString("/v1.1/orderexports", null);
            testContext.WriteLine("Order Exports:\n{0}", oeText);

            var list = JsonConvert.DeserializeObject<OrderExportList>(oeText);

            if ((list.batches == null) || (list.batches.Count <= 0)) Assert.Inconclusive("No Test Data Fetched");

            lastBatchId = list.batches[0].batchId;

            testContext.WriteLine("Last Batch Id: {0}", lastBatchId);

            Assert.IsTrue(lastBatchId > 0, "Batch Id Should Be > 0");

            return lastBatchId;
        }

        /// <summary>
        /// Get a Zip for a Batch
        /// </summary>
        /// <param name="client">OloClient</param>
        /// <param name="testContext">TestContext</param>
        /// <param name="downloadFolder">(sic)</param>
        /// <param name="batchId">(sic)</param>
        public static void GetZip(OloClient client, TestContext testContext, string downloadFolder, int batchId)
        {
            testContext.WriteLine("Last Batch Id: {0}", batchId);

            var path = string.Format("/v1.1/orderexports/{0}", batchId);
            byte[] zip = client.PostToGetUrl(path, null, null);

            var filename = string.Format("{0}.zip", batchId);
            filename = Path.Combine(downloadFolder, filename);
            if (File.Exists(filename)) File.Delete(filename);

            testContext.WriteLine("Filename: {0}", filename);

            File.WriteAllBytes(filename, zip);

            Assert.IsTrue(File.Exists(filename), "Zip file not found");
        }

        /// <summary>
        /// Delete a Batch
        /// </summary>
        /// <param name="client">OloClient</param>
        /// <param name="testContext">TestContext</param>
        /// <param name="batchId">(sic)</param>
        public static void DeleteBatch(OloClient client, TestContext testContext, int batchId)
        {
            var path = string.Format("/v1.1/orderexports/{0}", batchId);
            client.Delete(path, null);

            string oeText = client.GetJsonAsString("/v1.1/orderexports", null);
            testContext.WriteLine("Order Exports:\n{0}", oeText);

            var list = JsonConvert.DeserializeObject<OrderExportList>(oeText);

            if ((list.batches == null) || (list.batches.Count <= 0)) Assert.Inconclusive("No Test Data Fetched");

            var b = list.batches.AsQueryable().Where(p => p.batchId == batchId).FirstOrDefault();

            Assert.IsNull(b, "Should be missing");
        }

    }
}
