// csharp file that contains data properties for the captcha response

namespace NewEraFlowerStore.Areas.Identity.Data
{
    /// <summary>
    /// The class <see cref="CaptchaResponse"/> contains data properties for the captcha response.
    /// </summary>
    public class CaptchaResponse
    {
        /// <summary>
        /// The code of the captcha response.
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// The message of the captcha response.
        /// </summary>
        public string Message { get; set; }
    } // end class CaptchaResponse
} // end namespace NewEraFlowerStore.Areas.Identity.Data