#region Using Directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using NewEraFlowerStore.Services;
#endregion Using Directives

namespace NewEraFlowerStore.Controllers
{
    public class CaptchaController : Controller
    {
        private readonly ICaptchaManager _captchaManager;

        public CaptchaController(ICaptchaManager captchaManager)
        {
            _captchaManager = captchaManager;
        } // end constructor CaptchaController

        [HttpGet]
        public async Task<IActionResult> Image()
        {
            var captcha = await _captchaManager.CreateAsync();
            Response.Cookies.Append("CaptchaInfo", captcha.Content);
            return File(captcha.ImageData, captcha.FileFormat);
        } // end method Image
    } // end class CaptchaController
} // end namespace NewEraFlowerStore.Controllers