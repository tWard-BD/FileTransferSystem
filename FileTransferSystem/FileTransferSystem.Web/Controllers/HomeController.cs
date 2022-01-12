using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FileTransferSystem.Services.Services.Contracts;
using FileTransferSystem.Web.Models;

namespace FileTransferSystem.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMoveItApiClient apiClient;
        
        public HomeController(IMoveItApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(IFormFile file)
        {
            string folderId = await this.apiClient.GetFolderId();
            await this.apiClient.UploadFileToFolder(folderId, file);
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
