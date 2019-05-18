// csharp file that controls the connection to the captcha manager

#region Using Directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using NewEraFlowerStore.Services;
#endregion Using Directives

namespace NewEraFlowerStore.Controllers
{
    /// <summary>
    /// Extending from class <see cref="Controller"/>, the class <see cref="CaptchaController"/> controls the connection to the captcha manager.
    /// </summary>
    public class CaptchaController : Controller
    {
        private readonly ICaptchaManager _captchaManager;

        public CaptchaController(ICaptchaManager captchaManager)
        {
            _captchaManager = captchaManager;
        } // end constructor CaptchaController

        /// <summary>
        /// Add a new cookie and the relevant captcha content, and then return a captcha image file.
        /// </summary>
        /// <returns>a captcha image file</returns>
        [HttpGet]
        public async Task<IActionResult> Image()
        {
            var captcha = await _captchaManager.CreateAsync();
            Response.Cookies.Append("CaptchaInfo", captcha.Content);
            return File(captcha.ImageData, captcha.FileFormat);
        } // end method Image
    } // end class CaptchaController
} // end namespace NewEraFlowerStore.Controllers