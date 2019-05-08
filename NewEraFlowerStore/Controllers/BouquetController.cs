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
    public class BouquetController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ILogger<BouquetController> _logger;
        private readonly string BouquetPhotoRootPath;
        private readonly string DefaultPhoto1FileName;
        private readonly string DefaultPhoto2FileName;

        public BouquetController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IHostingEnvironment hostingEnvironment,
            ILogger<BouquetController> logger)
        {
            _context = context;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
            BouquetPhotoRootPath = _hostingEnvironment.WebRootPath + $@"\img\bouquets\";
            DefaultPhoto1FileName = "_default1.jpg";
            DefaultPhoto2FileName = "_default2.jpg";
        } // end constructor BouquetController

        #region Remote Validation
        [HttpGet]
        public async Task<IActionResult> VerifyNameNotInUseAsync()
        {
            var id = Request.Query["Bouquet.ID"].ToString();
            var name = Request.Query["Bouquet.Name"].ToString().Trim();
            var user = await _userManager.GetUserAsync(User);
            var idValue = 0;

            if (user != null
                && (string.IsNullOrWhiteSpace(id)
                    || (!string.IsNullOrWhiteSpace(id)
                        && int.TryParse(id, out idValue)))
                && await _context.Bouquets.FirstOrDefaultAsync(bouquet => bouquet.Name.Equals(name)) != null)
            {
                var currentBouquet = await _context.Bouquets.FindAsync(idValue);

                if (currentBouquet == null
                    || (currentBouquet != null
                        && currentBouquet.Name != name))
                    return Json($"The name \"{name}\" is already in use.");
            }

            return Json(true);
        } // end method VerifyNameNotInUseAsync
        #endregion Remote Validation

        #region Photo Uploader
        #region Photo 1
        [HttpPost]
        public async Task<IActionResult> SavePhoto1Async(int? id)
        {
            if (id == null)
            {
                Response.Clear();
                Response.StatusCode = 404;
                _logger.LogError("Error! File cannot be saved. Valid ID of bouquet is required.");
            }
            else
            {
                var bouquetToUpdate = await _context.Bouquets.FindAsync(id);

                if (bouquetToUpdate == null)
                {
                    Response.Clear();
                    Response.StatusCode = 404;
                    _logger.LogError("Error! File cannot be saved. Valid ID of bouquet is required.");
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
                                    var newFileName = id + "-1" + Path.GetExtension(formFile.FileName);

                                    using (var fileStream = new FileStream(BouquetPhotoRootPath + newFileName, FileMode.Create))
                                    {
                                        await formFile.CopyToAsync(fileStream);
                                    }

                                    _logger.LogInformation("File saved successfully.");

                                    if (newFileName != bouquetToUpdate.PhotoUrl1)
                                    {
                                        if (bouquetToUpdate.PhotoUrl1 != DefaultPhoto1FileName)
                                            if (!DeleteFile(BouquetPhotoRootPath + bouquetToUpdate.PhotoUrl1))
                                                return new EmptyResult();

                                        bouquetToUpdate.PhotoUrl1 = newFileName;
                                        _context.Attach(bouquetToUpdate).State = EntityState.Modified;

                                        try
                                        {
                                            await _context.SaveChangesAsync();
                                        }
                                        catch (DbUpdateConcurrencyException e)
                                        {
                                            if (!await _context.Bouquets.AnyAsync(bouquet => bouquet.ID == id))
                                            {
                                                Response.Clear();
                                                Response.StatusCode = 404;
                                                _logger.LogError(e, "Error! Failed to update URL of Photo 1. Bouquet does not exist.");
                                                return new EmptyResult();
                                            }
                                            else
                                            {
                                                Response.Clear();
                                                Response.StatusCode = 404;
                                                _logger.LogError(e, "Error! Failed to update URL of Photo 1.");
                                                return new EmptyResult();
                                            }
                                        }
                                    }

                                    _logger.LogInformation("Photo 1 has changed successfully.");
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
        } // end method SavePhoto1Async

        [HttpPost]
        public async Task<IActionResult> DeletePhoto1Async(int? id)
        {
            if (id == null)
            {
                Response.Clear();
                Response.StatusCode = 404;
                _logger.LogError("Error! File cannot be saved. Valid ID of bouquet is required.");
            }
            else
            {
                var bouquetToUpdate = await _context.Bouquets.FindAsync(id);

                if (bouquetToUpdate == null)
                {
                    Response.Clear();
                    Response.StatusCode = 404;
                    _logger.LogError("Error! File cannot be saved. Valid ID of bouquet is required.");
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
                                    var newFileName = id + "-1" + Path.GetExtension(formFile.FileName);

                                    bouquetToUpdate.PhotoUrl1 = DefaultPhoto1FileName;
                                    _context.Attach(bouquetToUpdate).State = EntityState.Modified;

                                    try
                                    {
                                        if (!DeleteFile(BouquetPhotoRootPath + newFileName))
                                            return new EmptyResult();

                                        await _context.SaveChangesAsync();
                                    }
                                    catch (DbUpdateConcurrencyException e)
                                    {
                                        if (!await _context.Bouquets.AnyAsync(bouquet => bouquet.ID == id))
                                        {
                                            Response.Clear();
                                            Response.StatusCode = 404;
                                            _logger.LogError(e, "Error! Failed to update URL of Photo 1. Bouquet does not exist.");
                                        }
                                        else
                                        {
                                            Response.Clear();
                                            Response.StatusCode = 404;
                                            _logger.LogError(e, "Error! Failed to update URL of Photo 1.");
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
        } // end method DeletePhoto1Async
        #endregion Photo 1

        #region Photo 2
        [HttpPost]
        public async Task<IActionResult> SavePhoto2Async(int? id)
        {
            if (id == null)
            {
                Response.Clear();
                Response.StatusCode = 404;
                _logger.LogError("Error! File cannot be saved. Valid ID of bouquet is required.");
            }
            else
            {
                var bouquetToUpdate = await _context.Bouquets.FindAsync(id);

                if (bouquetToUpdate == null)
                {
                    Response.Clear();
                    Response.StatusCode = 404;
                    _logger.LogError("Error! File cannot be saved. Valid ID of bouquet is required.");
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
                                    var newFileName = id + "-2" + Path.GetExtension(formFile.FileName);

                                    using (var fileStream = new FileStream(BouquetPhotoRootPath + newFileName, FileMode.Create))
                                    {
                                        await formFile.CopyToAsync(fileStream);
                                    }

                                    _logger.LogInformation("File saved successfully.");

                                    if (newFileName != bouquetToUpdate.PhotoUrl2)
                                    {
                                        if (bouquetToUpdate.PhotoUrl2 != DefaultPhoto2FileName)
                                            if (!DeleteFile(BouquetPhotoRootPath + bouquetToUpdate.PhotoUrl2))
                                                return new EmptyResult();

                                        bouquetToUpdate.PhotoUrl2 = newFileName;
                                        _context.Attach(bouquetToUpdate).State = EntityState.Modified;

                                        try
                                        {
                                            await _context.SaveChangesAsync();
                                        }
                                        catch (DbUpdateConcurrencyException e)
                                        {
                                            if (!await _context.Bouquets.AnyAsync(bouquet => bouquet.ID == id))
                                            {
                                                Response.Clear();
                                                Response.StatusCode = 404;
                                                _logger.LogError(e, "Error! Failed to update URL of Photo 2. Bouquet does not exist.");
                                                return new EmptyResult();
                                            }
                                            else
                                            {
                                                Response.Clear();
                                                Response.StatusCode = 404;
                                                _logger.LogError(e, "Error! Failed to update URL of Photo 2.");
                                                return new EmptyResult();
                                            }
                                        }
                                    }

                                    _logger.LogInformation("Photo 2 has changed successfully.");
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
        } // end method SavePhoto2Async

        [HttpPost]
        public async Task<IActionResult> DeletePhoto2Async(int? id)
        {
            if (id == null)
            {
                Response.Clear();
                Response.StatusCode = 404;
                _logger.LogError("Error! File cannot be saved. Valid ID of bouquet is required.");
            }
            else
            {
                var bouquetToUpdate = await _context.Bouquets.FindAsync(id);

                if (bouquetToUpdate == null)
                {
                    Response.Clear();
                    Response.StatusCode = 404;
                    _logger.LogError("Error! File cannot be saved. Valid ID of bouquet is required.");
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
                                    var newFileName = id + "-2" + Path.GetExtension(formFile.FileName);

                                    bouquetToUpdate.PhotoUrl2 = DefaultPhoto2FileName;
                                    _context.Attach(bouquetToUpdate).State = EntityState.Modified;

                                    try
                                    {
                                        if (!DeleteFile(BouquetPhotoRootPath + newFileName))
                                            return new EmptyResult();

                                        await _context.SaveChangesAsync();
                                    }
                                    catch (DbUpdateConcurrencyException e)
                                    {
                                        if (!await _context.Bouquets.AnyAsync(bouquet => bouquet.ID == id))
                                        {
                                            Response.Clear();
                                            Response.StatusCode = 404;
                                            _logger.LogError(e, "Error! Failed to update URL of Photo 2. Bouquet does not exist.");
                                        }
                                        else
                                        {
                                            Response.Clear();
                                            Response.StatusCode = 404;
                                            _logger.LogError(e, "Error! Failed to update URL of Photo 2.");
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
        } // end method DeletePhoto2Async
        #endregion Photo 2

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
        #endregion Photo Uploader
    } // end class BouquetController
} // end namespace NewEraFlowerStore.Controllers