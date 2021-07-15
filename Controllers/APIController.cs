using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Serilog;
using Serilog.Context;

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
            // tokenRepo = new TokenRepo(con);
        }

        [HttpGet("GetBMPByID/{id}")]
        public JsonResult GetBMPByID(int id)
        {
            // Log anreichern mit Controller und Function
            using (LogContext.PushProperty("Controller", ControllerContext.ActionDescriptor.ControllerName))
            using (LogContext.PushProperty("Function", ControllerContext.ActionDescriptor.ActionName))
            { 
                PixelItBMP bmpResult = pixelRepo.GetBMPByID(id);
               
                if (bmpResult != null)
                {
                    Log.Information("BMP mit der ID {ID} und den Name {Name} erfolgreich ausgeliefert", bmpResult.ID, bmpResult.Name);         
                }
                else
                {
                    bmpResult = new();
                    Log.Warning("Kein BMP zu der ID:{ID} gefunden", id);
                }

                return new JsonResult(bmpResult);
            }
        }

        [HttpGet("GetBMPNewst")]
        public JsonResult GetBMPNewst()
        {
            // Log anreichern mit Controller und Function
            using (LogContext.PushProperty("Controller", ControllerContext.ActionDescriptor.ControllerName))
            using (LogContext.PushProperty("Function", ControllerContext.ActionDescriptor.ActionName))           
            {
                PixelItBMP bmpResult = pixelRepo.GetBMPNewst();
                return new JsonResult(bmpResult);
            }
        }

    }
}
