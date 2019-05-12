// csharp file that contains data properties for the captcha info

namespace NewEraFlowerStore.Areas.Identity.Data
{
    /// <summary>
    /// The class <see cref="CaptchaInfo"/> contains data properties for the captcha info.
    /// </summary>
    public class CaptchaInfo
    {
        /// <summary>
        /// The image data of the captcha info.
        /// </summary>
        public byte[] ImageData { get; set; }
        /// <summary>
        /// The content of the captcha info.
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// The file format of the captcha info.
        /// </summary>
        public string FileFormat { get; set; }
    } // end class CaptchaInfo
} // end namespace NewEraFlowerStore.Areas.Identity.Data