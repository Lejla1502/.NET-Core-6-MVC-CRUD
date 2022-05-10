using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin)]

    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Upsert(int? id)
        {
            //this here will populate both Edit and Create in proper way
            ProductVM productVM = new ProductVM
            {
                Product = new(),
                CategoryList = _unitOfWork.Category.GetAll().Select(
                u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                CoverTypeList = _unitOfWork.CoverType.GetAll().Select(
               u => new SelectListItem
               {
                   Text = u.Name,
                   Value = u.Id.ToString()
               })
            };

            if (id == 0 | id == null)
            {
                

                //ViewBag.CategoryList = CategoryList;
                ////same thing as above, just different approach
                //ViewData["CoverTypeList"] = CoverTypeList;

                return View(productVM);
            }
            else
            {
               
                productVM.Product = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == id);
                if (productVM.Product == null)
                    return NotFound();

                return View(productVM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM obj, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                //getting wwwroot path
                string wwwRootPath = _hostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();

                    //to find a final location where the file needs to be uploaded
                    var uploads=Path.Combine(wwwRootPath, @"images\products");
                    //to keep the same extension of the file
                    var extension=Path.GetExtension(file.FileName);

                    //check if the file already exists in the DB, if so, delete it
                    if(obj.Product.ImageUrl!=null)
                    {
                        //get the old image path
                        //we need to trim the backward slash, because when we are adding image to DB
                        //we don't use that
                        var oldImagePath = Path.Combine(wwwRootPath, obj.Product.ImageUrl.TrimStart('\\'));

                        //check if any file exists in this old path, if yes, delete
                        if(System.IO.File.Exists(oldImagePath))
                            System.IO.File.Delete(oldImagePath);

                    }

                    //we need to copy the file that was uploaded inside the product folder
                    //we copy that using FileStream
                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create)) 
                    {
                        //copy file to location in FileStream
                        file.CopyTo(fileStreams);
                    }

                    obj.Product.ImageUrl = @"\images\products\" + fileName + extension;
                }

                if (obj.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(obj.Product);
                    _unitOfWork.Save();
                    TempData["success"] = "Product created successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    _unitOfWork.Product.Update(obj.Product);
                    _unitOfWork.Save();
                    TempData["success"] = "Product edited successfully";
                    return RedirectToAction("Index");
                }
                
            }
            return View(obj);
        }

        
        //here we create API endpoints for datatable
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var productList = _unitOfWork.Product.GetAll(includeProperties:"Category,CoverType");
            return Json(new { data = productList });
        }

        //POST
        [HttpDelete]
        public IActionResult Delete(int? id)
        {

            var objFromDb = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == id);
            if (objFromDb == null)
                return Json(new { success = false, message = "Error while deleting" });

            var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, objFromDb.ImageUrl.TrimStart('\\'));

            //check if any file exists in this old path, if yes, delete
            if (System.IO.File.Exists(oldImagePath))
                System.IO.File.Delete(oldImagePath);

            _unitOfWork.Product.Remove(objFromDb);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete successful" });
        }
        #endregion
    }
}
