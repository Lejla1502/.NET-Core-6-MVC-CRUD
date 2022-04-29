using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            //IEnumerable<Company> obj = _unitOfWork.Company.GetAll();

            return View();
        }


        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var companiesList = _unitOfWork.Company.GetAll();
            return Json(new { data = companiesList });
        }

        #endregion
    }
}
