namespace ImageCaption.Services
{
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// An interface that defines the contract with the caption service.
    /// </summary>
    internal interface ICaptionService
    {
        /// <summary>
        /// Gets the caption of an image stream.
        /// </summary>
        /// <param name="stream">The stream to an image.</param>
        /// <returns>Description if caption found, null otherwise.</returns>
        Task<string> GetCaptionAsync(Stream stream);

        /// <summary>
        /// Gets the caption of an image URL.
        /// </summary>
        /// <param name="url">The URL to an image.</param>
        /// <returns>Description if caption found, null otherwise.</returns>
        Task<string> GetCaptionAsync(string url);
        /// <summary>
        /// Gets the text of an image stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        Task<string> GetOcrTextAsync(Stream stream);
        /// <summary>
        /// Gets the text of an image url.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        Task<string> GetOcrTextAsync(string url);
    }
}