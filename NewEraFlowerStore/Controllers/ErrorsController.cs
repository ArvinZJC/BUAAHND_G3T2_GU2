// csharp file that controls links to the customised error pages for the error code 404 and 500

#region Using Directives
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
#endregion Using Directives

namespace NewEraFlowerStore.Controllers
{
    public class ErrorsController : Controller
    {
        private readonly IHostingEnvironment _environment;

        public ErrorsController(IHostingEnvironment environment)
        {
            _environment = environment;
        } // end constructor ErrorsController

        /// <summary>
        /// Show the specified customised error page according to the status code.
        /// </summary>
        /// <param name="statusCode">the status code 404 or 500</param>
        /// <returns>a <see cref="PhysicalFileResult"/> instance with the provided file name and content type</returns>
        [Route("errors/{statusCode}")]
        public IActionResult ShowErrorPage(int statusCode)
        {
            var filePath = $"{_environment.WebRootPath}/errors/{(statusCode == 404 ? 404 : 500)}.html";
            return new PhysicalFileResult(filePath, new MediaTypeHeaderValue("text/html").ToString());
        } // end method ShowErrorPage
    } // end class ErrorsController
} // end namespace NewEraFlowerStore.Controllers