using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using PixelIT.web.Models;
using Serilog;
using Serilog.Context;
using System;
using System.Linq;

namespace PixelIT.web.Controllers
{
    public class PixelCreatorController : Controller
    {
        private readonly PixelItRepo pixelRepo;
        public PixelCreatorController()
        {
            using MySqlConnection con = new(Globe.ConnectionString);
            pixelRepo = new(con);
        }


        public IActionResult Index(CreatorType creatorType)
        {
            using (LogContext.PushProperty("Controller", ControllerContext.ActionDescriptor.ControllerName))
            using (LogContext.PushProperty("Function", ControllerContext.ActionDescriptor.ActionName))
            {
                CreatorConfig creatorConfig = new();

                if (creatorType == CreatorType.Pixel_8x8)
                {
                    creatorConfig.SizeX = 8;
                }
                else if (creatorType == CreatorType.Pixel_8x32)
                {
                    creatorConfig.SizeX = 32;
                }

                Log.Information("Seite für 8x{SizeX} ausgeliefert", creatorConfig.SizeX);
                return View(creatorConfig);
            }
        }

        public IActionResult SavePopup(string bitmap)
        {
            ViewBag.Bitmap = bitmap;
            return PartialView();
        }

        [HttpPost]
        public IActionResult SaveBitmap(string bitmapname, string username, string bitmap)
        {
            using (LogContext.PushProperty("Controller", ControllerContext.ActionDescriptor.ControllerName))
            using (LogContext.PushProperty("Function", ControllerContext.ActionDescriptor.ActionName))
            {
                try
                {
                    if (String.IsNullOrEmpty(bitmap))
                    {
                        Log.Warning("Neues BMP mit den Namen {Name} von den User {User} konnte nicht gespeichert werden, Bitmap leer!", bitmapname, username);

                        return StatusCode(406);
                    }

                    if (String.IsNullOrEmpty(bitmapname))
                    {
                        Log.Warning("Neues BMP von den User {User} konnte nicht gespeichert werden, Bitmapname fehlt!", username, bitmap);

                        return StatusCode(406);
                    }

                    if (String.IsNullOrEmpty(username))
                    {
                        username = "N/A";
                    }

                    CleanBitmap(ref bitmap);

                    if (!ValidBitmap(bitmap))
                    {
                        Log.Warning("Neues BMP mit den Namen {Name} von den User {User} konnte nicht gespeichert werden, Bitmap ist nicht valid!", bitmapname, username, bitmap);
                        return StatusCode(406);
                    }

                    var sizeX = CalcSizeX(bitmap);

                    pixelRepo.SaveBMP(new PixelItBMP()
                    {
                        Animated = bitmap.Count(x => x == '[') > 1,
                        DateTime = DateTime.Now,
                        Name = bitmapname,
                        RGB565Array = bitmap,
                        SizeY = 8,
                        SizeX = sizeX,
                        Username = username
                    });
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "PixelCreator->SaveBitmap");
                }

                Log.Information("Neues BMP mit den Namen {Name} von den User {User} erfolgreich Gespeichert", bitmapname, username);
                return StatusCode(202);
            }
        }

        public void CleanBitmap(ref string bitmap)
        {
            var arr = bitmap.ToCharArray();
            arr = Array.FindAll<char>(arr, (c => (char.IsDigit(c) || c == ',' || c == '[') || c == ']'));
            bitmap = new string(arr);
        }

        public bool ValidBitmap(string bitmap)
        {
            return (bitmap.Any(x => x == '[')) && (bitmap.Any(x => x == ']'));
        }

        public int CalcSizeX(string bitmap)
        {
            // Determine the pixels
            var pixelGesammt = bitmap.Split(',').Length;
            // By 8 Rows
            var size = pixelGesammt / 8;
            // By the array count (If animated)
            size /= bitmap.Count(x => x == '[');

            return size;
        }
    }
}