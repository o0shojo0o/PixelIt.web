using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Serilog;
using Serilog.Context;
using System.Collections.Generic;

namespace PixelIT.web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class APIController : ControllerBase
    {
        private readonly PixelItRepo pixelRepo;

        public APIController()
        {
            using MySqlConnection con = new(Globe.ConnectionString);
            pixelRepo = new PixelItRepo(con);
        }

        [HttpGet("GetBMPByID/{id}")]
        public JsonResult GetBMPByID(int id)
        {
            // Enrich log with controller and function
            using (LogContext.PushProperty("Controller", ControllerContext.ActionDescriptor.ControllerName))
            using (LogContext.PushProperty("Function", ControllerContext.ActionDescriptor.ActionName))
            { 
                PixelItBMP bmpResult = pixelRepo.GetBMPByID(id);
               
                if (bmpResult != null)
                {
                    Log.Information("BMP with ID {ID} and name {Name} successfully delivered", bmpResult.ID, bmpResult.Name);         
                }
                else
                {
                    bmpResult = new();
                    Log.Warning("No BMP found for ID:{ID}", id);
                }

                return new JsonResult(bmpResult);
            }
        }

        [HttpGet("GetBMPNewst")]
        public JsonResult GetBMPNewst()
        {
            // Enrich log with controller and function
            using (LogContext.PushProperty("Controller", ControllerContext.ActionDescriptor.ControllerName))
            using (LogContext.PushProperty("Function", ControllerContext.ActionDescriptor.ActionName))           
            {
                PixelItBMP bmpResult = pixelRepo.GetBMPNewst();
                return new JsonResult(bmpResult);
            }
        }

        [HttpGet("GetBMPAll")]
        public JsonResult GetBMPAll()
        {
            // Enrich log with controller and function
            using (LogContext.PushProperty("Controller", ControllerContext.ActionDescriptor.ControllerName))
            using (LogContext.PushProperty("Function", ControllerContext.ActionDescriptor.ActionName))
            {
                List<PixelItBMP> bmpList = pixelRepo.GetBMPAll();
                return new JsonResult(bmpList);
            }
        }

    }
}
