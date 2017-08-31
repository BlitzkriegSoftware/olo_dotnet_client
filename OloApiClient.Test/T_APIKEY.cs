using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;
using OloApiClient.Models;

namespace OloApiClient.Test
{
    /// <summary>
    /// Test: API (with IP Address Restrictions) of APIKEY
    /// </summary>
    [TestClass]
    public class T_APIKEY
    {
        #region "Configuration Variables"
        static string ApiKey = ConfigurationManager.AppSettings["APIKEY"];
        static string URL = ConfigurationManager.AppSettings["URL"];
        static string DownloadFolder = ConfigurationManager.AppSettings["DownloadFolder"];
        #endregion

        #region "Boilerplate"
        /// <summary>
        /// TestContext
        /// </summary>
        public TestContext TestContext { get; set; }
        #endregion

        #region "Starting Conditions"
        private void ValidateConfiguration()
        {
            if (string.IsNullOrWhiteSpace(URL)) throw new ConfigurationErrorsException(string.Format("Missing 'URL' value: {0}", URL));
            if (string.IsNullOrWhiteSpace(DownloadFolder)) throw new ConfigurationErrorsException(string.Format("Missing 'DownloadFolder' value: {0}", DownloadFolder));
            if (string.IsNullOrWhiteSpace(ApiKey)) throw new ConfigurationErrorsException(string.Format("Missing 'ApiKey' value: {0}", ApiKey));

            if (this.OloClient == null) throw new InvalidOperationException("OloClient instance is null");

            if (!Directory.Exists(DownloadFolder)) Directory.CreateDirectory(DownloadFolder);

        }
        #endregion

        #region "Shared Test Values"
        /// <summary>
        /// Last Batch Id
        /// </summary>
        public int LastBatchId { get; set; }

        private OloClient _oloclient = null;

        /// <summary>
        /// OLO Client instance for testing
        /// </summary>
        public OloClient OloClient
        {
            get
            {
                if(_oloclient == null)
                {
                    var loggingHandler = new DefaultOloLoggingHandler(new HttpClientHandler());
                    _oloclient = new OloClient(loggingHandler, URL, ApiKey);
                }
                return _oloclient;
            }
        }
        
        #endregion

        /// <summary>
        /// Get list of batches
        /// </summary>
        [TestMethod]
        public void T_GetList()
        {
            ValidateConfiguration();
            LastBatchId = SharedMethods.GetList(this.OloClient, this.TestContext);
        }

        /// <summary>
        /// Get the ZIP for one batch
        /// </summary>
        [TestMethod]
        public void T_GetBatchZip()
        {
            ValidateConfiguration();
            if (LastBatchId <= 0) LastBatchId = SharedMethods.GetList(this.OloClient, this.TestContext);
            SharedMethods.GetZip(this.OloClient, this.TestContext, DownloadFolder, LastBatchId);
        }

        /// <summary>
        /// Delete a batch
        /// </summary>
        [TestMethod]
        public void T_DeleteBatch()
        {
            ValidateConfiguration();
            if (LastBatchId <= 0) LastBatchId = SharedMethods.GetList(this.OloClient, this.TestContext);
            SharedMethods.DeleteBatch(this.OloClient, this.TestContext, LastBatchId);
        }
    }
}
