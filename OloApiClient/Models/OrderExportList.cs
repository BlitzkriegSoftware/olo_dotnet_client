using System.Collections.Generic;

namespace OloApiClient.Models
{
    // Case of object is because it was code generated from SWAGGER

    /// <summary>
    /// Model: Return from GET orderexports
    /// </summary>
    public class OrderExportList
    {
        /// <summary>
        /// List of the batches that come back
        /// </summary>
        public List<Batch> batches { get; set; }
    }
}
