// csharp file that contains prototypes of methods of a captcha manager

#region Using Directives
using System.Threading.Tasks;

using NewEraFlowerStore.Areas.Identity.Data;
#endregion Using Directives

namespace NewEraFlowerStore.Services
{
    /// <summary>
    /// The interface <see cref="ICaptchaManager"/> contains prototypes of methods of a captcha manager.
    /// </summary>
    public interface ICaptchaManager
    {
        // create a captcha image
        Task<CaptchaInfo> CreateAsync(int charCount = 4, int width = 85, int height = 40);

        // verify the user's answer with the captcha (the default timeout value is 120 seconds)
        Task<CaptchaResponse> VerifyAsync(CaptchaRequest captchaRequest, int timeOut = 120);
    } // end interface ICaptchaManager
} // end namespace NewEraFlowerStore.Services