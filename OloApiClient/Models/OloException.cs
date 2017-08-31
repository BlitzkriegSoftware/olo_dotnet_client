using System;
using System.Net.Http;

namespace OloApiClient.Models
{
    /// <summary>
    /// OLO Client Exception
    /// </summary>
    public class OloException : HttpRequestException
    {
        /// <summary>
        /// CTOR
        /// </summary>
        public Error OloError { get; set; }

        /// <summary>
        /// CTOR w. OLO Error Object
        /// </summary>
        /// <param name="oloError">Error</param>
        public OloException(Error oloError) : base()
        {
            this.OloError = oloError;
        }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="oloError">Error</param>
        public OloException(string message, Error oloError) : base(message)
        {
            this.OloError = oloError;
        }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="inner">Inner Exception</param>
        /// <param name="oloError">Error</param>
        public OloException(string message, Exception inner, Error oloError) : base(message, inner)
        {
            this.OloError = oloError;
        }

        /// <summary>
        /// Debug version of object
        /// </summary>
        /// <returns>String of the exception</returns>
        public override string ToString()
        {
            return string.Format("{0}: {1}", this.OloError.Num, this.OloError.Message);
        }
    }
}
