// csharp file that contains data properties for the captcha request

namespace NewEraFlowerStore.Areas.Identity.Data
{
    /// <summary>
    /// The class <see cref="CaptchaRequest"/> contains data properties for the captcha request.
    /// </summary>
    public class CaptchaRequest
    {
        /// <summary>
        /// The answer of the captcha request.
        /// </summary>
        public string Answer { get; set; }
        /// <summary>
        /// The key of the captcha request.
        /// </summary>
        public string CaptchaKey { get; set; }
    } // end class CaptchaRequest
} // end namespace NewEraFlowerStore.Areas.Identity.Data