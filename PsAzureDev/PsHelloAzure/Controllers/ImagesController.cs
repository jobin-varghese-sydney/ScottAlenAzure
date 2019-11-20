using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PsHelloAzure.Models;
using PsHelloAzure.Services;

namespace PsHelloAzure.Controllers
{
    [Route("[controller]/[action]")]
    public class ImagesController : Controller
    {
        private readonly ImageStore imageStore;
        private readonly ImageAnalysisStore imageAnalysisStore;

        public ImagesController(ImageStore imageStore, ImageAnalysisStore imageAnalysisStore)
        {
            this.imageStore = imageStore;
            this.imageAnalysisStore = imageAnalysisStore;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile image)
        {
            if(image != null)
            {
                using (var stream = image.OpenReadStream())
                { 
                    var imageId = await imageStore.SaveImage(stream);
                    return RedirectToAction("Show", new { imageId });
                }
            }
            return View();
        }

        [HttpGet("{imageId}")]
        public ActionResult Show(string imageId)
        {
            var model = new ShowModel
            {
                Uri = imageStore.UriFor(imageId),
                Analysis = ""
            };

            var analysis = imageAnalysisStore.GetImageAnalysis(imageId);
            if (analysis != null)
            {
                model.Analysis = JsonConvert.SerializeObject(analysis, 
                                                    Formatting.Indented);
            }

            return View(model);
        }
    }
}