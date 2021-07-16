using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Serilog;
using Serilog.Context;
using System;
using System.Collections.Generic;

namespace PixelIT.web.Controllers
{
    public class PixelGalleryController : Controller
    {
        private readonly PixelItRepo pixelRepo;

        public PixelGalleryController()
        {
            using MySqlConnection con = new(Globe.ConnectionString);
            pixelRepo = new(con);
        }

        public IActionResult Index(string id)
        {
            using (LogContext.PushProperty("Controller", ControllerContext.ActionDescriptor.ControllerName))
            using (LogContext.PushProperty("Function", ControllerContext.ActionDescriptor.ActionName))
            {

                List<PixelItBMP> bmps = new();
                if (!String.IsNullOrWhiteSpace(id) && id.IsNumber())
                {
                    PixelItBMP bmp = pixelRepo.GetBMPByID(int.Parse(id));

                    if (bmp != null)
                    {
                        bmps.Add(bmp);
                    }

                    Log.Information("BMP ID {ID} was passed", id);
                }
                else
                {
                    bmps = pixelRepo.GetBMPAll();
                }

                if (bmps.Count == 0)
                {
                    Log.Warning("No BMP found for ID {ID}", id);
                }

                Log.Information("Page delivered Count:{FoundBMPs}", bmps.Count);
                return View(bmps);
            }
        }

        public IActionResult DetailPopup(int id)
        {
            using (LogContext.PushProperty("Controller", ControllerContext.ActionDescriptor.ControllerName))
            using (LogContext.PushProperty("Function", ControllerContext.ActionDescriptor.ActionName))
            {
                PixelItBMP bmp = pixelRepo.GetBMPByID(id);
                Log.Information("DetailPopup for the BMP with the ID {ID} and the name {Name} successfully delivered.", bmp.ID, bmp.Name);
                return PartialView("_DetailPopup", bmp);
            }
        }

        public IActionResult PixelCards(string search, BitmapType type)
        {
            using (LogContext.PushProperty("Controller", ControllerContext.ActionDescriptor.ControllerName))
            using (LogContext.PushProperty("Function", ControllerContext.ActionDescriptor.ActionName))
            {
                List<PixelItBMP> result;
                if (!String.IsNullOrEmpty(search))
                {
                    search = search.Trim();
                    result = pixelRepo.GetBMPBySearch(search, type);
                }
                else
                {
                    result = pixelRepo.GetBMPByTyp(type);
                }

                if (result == null)
                {
                    result = new();
                }

                Log.Information("Page with SearchParam {SearchParam} and BMPType {BMPType} delivered Count:{FoundBMPs}", search, type, result.Count);
                return PartialView("_PixelCards", result);
            }
        }
    }
}