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
    public class FlowerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ILogger<AvatarUploaderController> _logger;
        private readonly string CoverPhotoRootPath;
        private readonly string DefaultCoverPhotoFileName;

        public FlowerController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IHostingEnvironment hostingEnvironment,
            ILogger<AvatarUploaderController> logger)
        {
            _context = context;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
            CoverPhotoRootPath = _hostingEnvironment.WebRootPath + $@"\img\cover_photos\flowers\";
            DefaultCoverPhotoFileName = "_default.jpg";
        } // end constructor FlowerController

        #region Remote Validation
        [HttpGet]
        public async Task<IActionResult> VerifyNameNotInUseAsync()
        {
            var id = Request.Query["Flower.ID"].ToString();
            var name = Request.Query["Flower.Name"].ToString().Trim();
            var user = await _userManager.GetUserAsync(User);
            var idValue = 0;

            if (user != null
                && (string.IsNullOrWhiteSpace(id)
                    || (!string.IsNullOrWhiteSpace(id)
                        && int.TryParse(id, out idValue)))
                && await _context.Flowers.FirstOrDefaultAsync(flower => flower.Name.Equals(name)) != null)
            {
                var currentFlower = await _context.Flowers.FindAsync(idValue);

                if (currentFlower == null
                    || (currentFlower != null
                        && currentFlower.Name != name))
                    return Json($"The name \"{name}\" is already in use.");
            }

            return Json(true);
        } // end method VerifyNameNotInUseAsync
        #endregion Remote Validation

        #region Cover Photo Uploader
        [HttpPost]
        public async Task<IActionResult> SaveAsync(int? id)
        {
            if (id == null)
            {
                Response.Clear();
                Response.StatusCode = 404;
                _logger.LogError("Error! File cannot be saved. Valid ID of flower is required.");
            }
            else
            {
                var flowerToUpdate = await _context.Flowers.FindAsync(id);

                if (flowerToUpdate == null)
                {
                    Response.Clear();
                    Response.StatusCode = 404;
                    _logger.LogError("Error! File cannot be saved. Valid ID of flower is required.");
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

                                    if (newFileName != flowerToUpdate.CoverPhotoUrl)
                                    {
                                        if (flowerToUpdate.CoverPhotoUrl != DefaultCoverPhotoFileName)
                                            if (!DeleteFile(CoverPhotoRootPath + flowerToUpdate.CoverPhotoUrl))
                                                return new EmptyResult();

                                        flowerToUpdate.CoverPhotoUrl = newFileName;
                                        _context.Attach(flowerToUpdate).State = EntityState.Modified;

                                        try
                                        {
                                            await _context.SaveChangesAsync();
                                        }
                                        catch (DbUpdateConcurrencyException e)
                                        {
                                            if (!await _context.Flowers.AnyAsync(flower => flower.ID == id))
                                            {
                                                Response.Clear();
                                                Response.StatusCode = 404;
                                                _logger.LogError(e, "Error! Failed to update cover photo URL. Flower does not exist.");
                                                return new EmptyResult();
                                            }
                                            else
                                            {
                                                Response.Clear();
                                                Response.StatusCode = 404;
                                                _logger.LogError(e, "Error! Failed to update cover photo URL.");
                                                return new EmptyResult();
                                            }
                                        }
                                    }

                                    _logger.LogInformation("Cover photo has changed successfully.");
                                }
                            }
                            else
                            {
                                Response.Clear();
                                Response.StatusCode = 404;
                                _logger.LogError("Error! File cannot be saved. User does not exist.");
                            }
                        }
                        else
                        {
                            Response.Clear();
                            Response.StatusCode = 404;
                            _logger.LogError("Error! Failed to get file list from the request.");
                        }
                    }
                    catch (Exception e)
                    {
                        Response.Clear();
                        Response.StatusCode = 404;
                        _logger.LogError(e, "Error! Failed to save file.");
                    }
                }
            }

            return new EmptyResult();
        } // end method SaveAsync

        [HttpPost]
        public async Task<IActionResult> DeleteAsync(int? id)
        {
            if (id == null)
            {
                Response.Clear();
                Response.StatusCode = 404;
                _logger.LogError("Error! File cannot be saved. Valid ID of flower is required.");
            }
            else
            {
                var flowerToUpdate = await _context.Flowers.FindAsync(id);

                if (flowerToUpdate == null)
                {
                    Response.Clear();
                    Response.StatusCode = 404;
                    _logger.LogError("Error! File cannot be saved. Valid ID of flower is required.");
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

                                    flowerToUpdate.CoverPhotoUrl = DefaultCoverPhotoFileName;
                                    _context.Attach(flowerToUpdate).State = EntityState.Modified;

                                    try
                                    {
                                        if (!DeleteFile(CoverPhotoRootPath + newFileName))
                                            return new EmptyResult();

                                        await _context.SaveChangesAsync();
                                    }
                                    catch (DbUpdateConcurrencyException e)
                                    {
                                        if (!await _context.Flowers.AnyAsync(flower => flower.ID == id))
                                        {
                                            Response.Clear();
                                            Response.StatusCode = 404;
                                            _logger.LogError(e, "Error! Failed to update cover photo URL. Flower does not exist.");
                                        }
                                        else
                                        {
                                            Response.Clear();
                                            Response.StatusCode = 404;
                                            _logger.LogError(e, "Error! Failed to update cover photo URL.");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Response.Clear();
                                Response.StatusCode = 404;
                                _logger.LogError("Error! Cannot delete file. User does not exist.");
                            }
                        }
                        else
                        {
                            Response.Clear();
                            Response.StatusCode = 404;
                            _logger.LogError("Error! Failed to get file list from the request.");
                        }
                    }
                    catch (Exception e)
                    {
                        Response.Clear();
                        Response.StatusCode = 404;
                        _logger.LogError(e, "Error! Failed to delete file.");
                    }
                }
            }

            return new EmptyResult();
        } // end method DeleteAsync

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
                }
            }
            catch (Exception e)
            {
                Response.Clear();
                Response.StatusCode = 404;
                _logger.LogError(e, "Error! Failed to delete file.");
                return false;
            }
        } // end method DeleteFile
        #endregion Cover Photo Uploader
    } // end class FlowerController
} // end namespace NewEraFlowerStore.Controllers