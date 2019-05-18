// csharp file that controls validation related to occasions

#region Using Directives
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using NewEraFlowerStore.Data;
#endregion Using Directives

namespace NewEraFlowerStore.Controllers
{
    /// <summary>
    /// Extending from class <see cref="Controller"/>, the class <see cref="OccasionController"/> controls validation related to occasions.
    /// </summary>
    public class OccasionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ILogger<AvatarUploaderController> _logger;
        private readonly string CoverPhotoRootPath;
        private readonly string DefaultCoverPhotoFileName;

        public OccasionController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IHostingEnvironment hostingEnvironment,
            ILogger<AvatarUploaderController> logger)
        {
            _context = context;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
            CoverPhotoRootPath = _hostingEnvironment.WebRootPath + $@"\img\cover_photos\occasions\";
            DefaultCoverPhotoFileName = "_default.jpg";
        } // end constructor OccasionController

        #region Remote Validation
        /// <summary>
        /// Verify whether the specified occasion name is in use or not.
        /// This method is decorated with <see cref="HttpGetAttribute"/>.
        /// </summary>
        /// <returns>a <see cref="JsonResult"/> object that serialises the verification result to JSON</returns>
        [HttpGet]
        public async Task<IActionResult> VerifyNameNotInUseAsync()
        {
            var id = Request.Query["Occasion.ID"].ToString();
            var name = Request.Query["Occasion.Name"].ToString().Trim();
            var user = await _userManager.GetUserAsync(User);
            var idValue = 0;

            if (user != null
                && (string.IsNullOrWhiteSpace(id)
                    || (!string.IsNullOrWhiteSpace(id)
                        && int.TryParse(id, out idValue)))
                && await _context.Occasions.FirstOrDefaultAsync(occasion => occasion.Name.Equals(name)) != null)
            {
                var currentOccasion = await _context.Occasions.FindAsync(idValue);

                if (currentOccasion == null
                    || (currentOccasion != null
                        && currentOccasion.Name != name))
                    return Json($"The name \"{name}\" is already in use.");
            } // end if

            return Json(true);
        } // end method VerifyNameNotInUseAsync
        #endregion Remote Validation

        #region Cover Photo Uploader
        /// <summary>
        /// Save a specified occasion photo.
        /// This method is decorated with <see cref="HttpPostAttribute"/>.
        /// </summary>
        /// <param name="id">ID of a specified occasion</param>
        /// <returns>an <see cref="EmptyResult"/> object</returns>
        [HttpPost]
        public async Task<IActionResult> SaveAsync(int? id)
        {
            if (id == null)
            {
                Response.Clear();
                Response.StatusCode = 404;
                _logger.LogError("Error! File cannot be saved. Valid ID of occasion is required.");
            }
            else
            {
                var occasionToUpdate = await _context.Occasions.FindAsync(id);

                if (occasionToUpdate == null)
                {
                    Response.Clear();
                    Response.StatusCode = 404;
                    _logger.LogError("Error! File cannot be saved. Valid ID of occasion is required.");
                }
                else
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
                                    var newFileName = id + Path.GetExtension(formFile.FileName);

                                    using (var fileStream = new FileStream(CoverPhotoRootPath + newFileName, FileMode.Create))
                                    {
                                        await formFile.CopyToAsync(fileStream);
                                    }

                                    _logger.LogInformation("File saved successfully.");

                                    if (newFileName != occasionToUpdate.CoverPhotoUrl)
                                    {
                                        if (occasionToUpdate.CoverPhotoUrl != DefaultCoverPhotoFileName)
                                            // call the specified method to delete the specified file
                                            if (!DeleteFile(CoverPhotoRootPath + occasionToUpdate.CoverPhotoUrl))
                                                return new EmptyResult();

                                        occasionToUpdate.CoverPhotoUrl = newFileName;
                                        _context.Attach(occasionToUpdate).State = EntityState.Modified;

                                        try
                                        {
                                            await _context.SaveChangesAsync();
                                        }
                                        catch (DbUpdateConcurrencyException e)
                                        {
                                            if (!await _context.Occasions.AnyAsync(occasion => occasion.ID == id))
                                            {
                                                Response.Clear();
                                                Response.StatusCode = 404;
                                                _logger.LogError(e, "Error! Failed to update cover photo URL. Occasion does not exist.");
                                                return new EmptyResult();
                                            }
                                            else
                                            {
                                                Response.Clear();
                                                Response.StatusCode = 404;
                                                _logger.LogError(e, "Error! Failed to update cover photo URL.");
                                                return new EmptyResult();
                                            } // end if...else
                                        } // end try...catch
                                    } // end if

                                    _logger.LogInformation("Cover photo has changed successfully.");
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
                } // end if...else
            } // end if...else

            return new EmptyResult();
        } // end method SaveAsync

        /// <summary>
        /// Delete a specified occasion photo.
        /// This method is decorated with <see cref="HttpPostAttribute"/>.
        /// </summary>
        /// <param name="id">ID of a specified occasion</param>
        /// <returns>an <see cref="EmptyResult"/> object</returns>
        [HttpPost]
        public async Task<IActionResult> DeleteAsync(int? id)
        {
            if (id == null)
            {
                Response.Clear();
                Response.StatusCode = 404;
                _logger.LogError("Error! File cannot be saved. Valid ID of occasion is required.");
            }
            else
            {
                var occasionToUpdate = await _context.Occasions.FindAsync(id);

                if (occasionToUpdate == null)
                {
                    Response.Clear();
                    Response.StatusCode = 404;
                    _logger.LogError("Error! File cannot be saved. Valid ID of occasion is required.");
                }
                else
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
                                    var newFileName = id + Path.GetExtension(formFile.FileName);

                                    occasionToUpdate.CoverPhotoUrl = DefaultCoverPhotoFileName;
                                    _context.Attach(occasionToUpdate).State = EntityState.Modified;

                                    try
                                    {
                                        // call the specified method to delete the specified file
                                        if (!DeleteFile(CoverPhotoRootPath + newFileName))
                                            return new EmptyResult();

                                        await _context.SaveChangesAsync();
                                    }
                                    catch (DbUpdateConcurrencyException e)
                                    {
                                        if (!await _context.Occasions.AnyAsync(occasion => occasion.ID == id))
                                        {
                                            Response.Clear();
                                            Response.StatusCode = 404;
                                            _logger.LogError(e, "Error! Failed to update cover photo URL. Occasion does not exist.");
                                        }
                                        else
                                        {
                                            Response.Clear();
                                            Response.StatusCode = 404;
                                            _logger.LogError(e, "Error! Failed to update cover photo URL.");
                                        } // end if...else
                                    } // end try...catch
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
                } // end if...else
            } // end if...else

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
        #endregion Cover Photo Uploader
    } // end class OccasionController
} // end namespace NewEraFlowerStore.Controllers