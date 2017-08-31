namespace OloApiClient.Models
{
    /// <summary>
    /// DTO: Error for OLO
    /// </summary>
    public class Error
    {
        /// <summary>
        /// Error Number
        /// </summary>
        public int Num { get; set; }

        /// <summary>
        /// Error Message
        /// </summary>
        public string Message { get; set; }
    }
}
