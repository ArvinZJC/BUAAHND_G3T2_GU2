// csharp file that controls validation related to the avatar uploader

#region Using Directives
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using NewEraFlowerStore.Data;
#endregion Using Directives

namespace NewEraFlowerStore.Controllers
{
    /// <summary>
    /// Extending from class <see cref="Controller"/>, the class <see cref="AvatarUploaderController"/> controls validation related to the avatar uploader.
    /// </summary>
    public class AvatarUploaderController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ILogger<AvatarUploaderController> _logger;
        private readonly string AvatarRootPath;
        private readonly string DefaultAvatarFileName;

        public AvatarUploaderController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IHostingEnvironment hostingEnvironment,
            ILogger<AvatarUploaderController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
            AvatarRootPath = _hostingEnvironment.WebRootPath + $@"\img\avatars\";
            DefaultAvatarFileName = "_default.jpg";
        } // end constructor AvatarUploaderController

        /// <summary>
        /// Save the avatar file.
        /// This method is decorated with <see cref="HttpPostAttribute"/>.
        /// </summary>
        /// <returns>an <see cref="EmptyResult"/> object</returns>
        [HttpPost]
        public async Task<IActionResult> SaveAsync()
        {
            try
            {
                var files = Request.Form.Files;

                if (files != null)
                {
                    var user = await _userManager.GetUserAsync(User);

                    if (user != null)
                    {
                        foreach (var formFile in files)
                        {
                            var newFileName = $@"{user.Id}" + Path.GetExtension(formFile.FileName);

                            using (var fileStream = new FileStream(AvatarRootPath + newFileName, FileMode.Create))
                            {
                                await formFile.CopyToAsync(fileStream);
                            }
                            
                            _logger.LogInformation("File saved successfully.");

                            if (newFileName != user.AvatarUrl)
                            {
                                if (user.AvatarUrl != DefaultAvatarFileName)
                                    if (!DeleteFile(AvatarRootPath + user.AvatarUrl))
                                        return new EmptyResult();

                                user.AvatarUrl = newFileName;

                                var result = await _userManager.UpdateAsync(user);

                                if (result.Succeeded)
                                    await _signInManager.RefreshSignInAsync(user);
                                else
                                {
                                    Response.Clear();
                                    Response.StatusCode = 404;
                                    _logger.LogError("Error! Failed to update avatar URL.");
                                    return new EmptyResult();
                                } // end if...else
                            } // end if

                            _logger.LogInformation("User changed avatar successfully.");
                        } // end foreach
                    }
                    else
                    {
                        Response.Clear();
                        Response.StatusCode = 404;
                        _logger.LogError("Error! File cannot be saved. User does not exist.");
                    } // end if...else
                }
                else
                {
                    Response.Clear();
                    Response.StatusCode = 404;
                    _logger.LogError("Error! Failed to get file list from the request.");
                } // end if...else       
            }
            catch (Exception e)
            {
                Response.Clear();
                Response.StatusCode = 404;
                _logger.LogError(e, "Error! Failed to save file.");
            } // end try...catch
            
            return new EmptyResult();
        } // end method SaveAsync

        /// <summary>
        /// Delete the avatar file.
        /// This method is decorated with <see cref="HttpPostAttribute"/>.
        /// </summary>
        /// <returns>an <see cref="EmptyResult"/> object</returns>
        [HttpPost]
        public async Task<IActionResult> DeleteAsync()
        {
            try
            {
                var files = Request.Form.Files;

                if (files != null)
                {
                    var user = await _userManager.GetUserAsync(User);

                    if (user != null)
                    {
                        foreach (var formFile in files)
                        {
                            var newFileName = $@"{user.Id}" + Path.GetExtension(formFile.FileName);

                            user.AvatarUrl = DefaultAvatarFileName;

                            var result = await _userManager.UpdateAsync(user);

                            if (result.Succeeded)
                            {
                                // call the specified method to delete the specified file
                                if (!DeleteFile(AvatarRootPath + newFileName))
                                    return new EmptyResult();

                                await _signInManager.RefreshSignInAsync(user);
                                _logger.LogInformation("User avatar changed to default.");
                            }
                            else
                            {
                                Response.Clear();
                                Response.StatusCode = 404;
                                _logger.LogError("Error! Failed to delete file.");
                            } // end if...else
                        } // end foreach
                    }
                    else
                    {
                        Response.Clear();
                        Response.StatusCode = 404;
                        _logger.LogError("Error! Cannot delete file. User does not exist.");
                    } // end if...else
                }
                else
                {
                    Response.Clear();
                    Response.StatusCode = 404;
                    _logger.LogError("Error! Failed to get file list from the request.");
                } // end if...else
            }
            catch (Exception e)
            {
                Response.Clear();
                Response.StatusCode = 404;
                _logger.LogError(e, "Error! Failed to delete file.");
            } // end try...catch

            return new EmptyResult();
        } // end method DeleteAsync

        // delete the specified file
        private bool DeleteFile(string filePath)
        {
            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    _logger.LogInformation("File deleted successfully.");

                    return true;
                }
                else
                {
                    Response.Clear();
                    Response.StatusCode = 204;
                    _logger.LogWarning("File cannot be found.");

                    return true;
                } // end if...else
            }
            catch (Exception e)
            {
                Response.Clear();
                Response.StatusCode = 404;
                _logger.LogError(e, "Error! Failed to delete file.");
                return false;
            } // end try...catch
        } // end method DeleteFile
    } // end class AvatarUploaderController
} // end namespace NewEraFlowerStore.Controllers