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
            var autorSelectList = _unitOfWork.Author.GetAll().Select(s => new {  s.Id, Naziv = s.FirstName + " " + s.LastName }).ToList();
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
               }),
                AuthorList = new SelectList(autorSelectList, "Id", "Naziv"),
                //AuthorList=_unitOfWork.Author.GetAll().Select(
                //    u=> new SelectListItem
                //    {
                //       Text = u.FirstName + ' ' + u.LastName,
                //       Value = u.Id.ToString()
                //    }),
                Author2List = _unitOfWork.Author.GetAll().Select(
                    u => new SelectListItem
                    {
                        Text = u.FirstName + ' ' + u.LastName,
                        Value = u.Id.ToString()
                    })
            };

            productVM.Product.Reviews = new();

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
           
            Console.WriteLine(ModelState.Values);
            //if (ModelState.IsValid)
            //{
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

            obj.Product.Author = "";

                if (obj.Product.Id == 0)
                {
                    obj.Product.Author = _unitOfWork.Author.GetFirstOrDefault(x => x.Id == obj.AuthorId).FirstName + " " + _unitOfWork.Author.GetFirstOrDefault(x => x.Id == obj.AuthorId).LastName;

                    if(obj.Author2Id!=0 && obj.Author2Id != null)
                {
                    obj.Product.Author += ", " + _unitOfWork.Author.GetFirstOrDefault(x => x.Id == obj.AuthorId).FirstName + " " + _unitOfWork.Author.GetFirstOrDefault(x => x.Id == obj.AuthorId).LastName;
                }
                    obj.Product.CreatedAt=DateTime.Now;
                    _unitOfWork.Product.Add(obj.Product);
                    _unitOfWork.Save();

                var productAuthor = new AuthorProduct
                {
                    Product = obj.Product,
                    AuthorId = obj.AuthorId
                };

                    _unitOfWork.AuthorProduct.Add(productAuthor);
                _unitOfWork.Save();

                if (obj.Author2Id!=null && obj.Author2Id!=0)
                    {
                    var productAuthor2 = new AuthorProduct
                    {
                        Product = obj.Product,
                        AuthorId = obj.Author2Id
                    };
                        _unitOfWork.AuthorProduct.Add(productAuthor2);
                    _unitOfWork.Save();
                }
                    

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
                
            //}
            
            //return View(obj);
        }

        
        //here we create API endpoints for datatable
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var productList = _unitOfWork.Product.GetAll(includeProperties:"Category,CoverType").Select(s=>new ProductAdminVM
            {
                Id=s.Id,
                Title=s.Title,
                ISBN=s.ISBN,
                Price=s.Price,
                Author= string.Join(", ", _unitOfWork.AuthorProduct.GetAll(ap => ap.ProductId == s.Id, includeProperties: "Product,Author").Select(sl => sl.Author.FirstName + ' ' + sl.Author.LastName).ToArray()),
                Category=s.Category.Name
            });
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
