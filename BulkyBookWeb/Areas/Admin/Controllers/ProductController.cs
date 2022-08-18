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
            if (_unitOfWork.Product.GetAll(x => x.Featured == true).FirstOrDefault() == null)
                ViewBag.Featured = null;
            else
                ViewBag.Featured = _unitOfWork.Product.GetAll(x => x.Featured == true).First().Title;

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

                var apList = _unitOfWork.AuthorProduct.GetAll(x => x.ProductId == id);
                productVM.AuthorId = apList.First().AuthorId;

                if (apList.Count() > 1)
                    productVM.Author2Id = apList.Last().AuthorId;

                var author = new Author()
                {
                    Id=0,
                    FirstName="",
                    LastName="",
                    Bio=""
                };

                ViewBag.Author=author;

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
                    var author2Name = "";

                    var apList = _unitOfWork.AuthorProduct.GetAll(x => x.ProductId == obj.Product.Id, includeProperties: "Author");



                    _unitOfWork.AuthorProduct.Remove(apList.First());
                    _unitOfWork.Save();

                    var newAP1 = new AuthorProduct
                    {
                        ProductId = obj.Product.Id,
                        AuthorId = obj.AuthorId
                    };
                    _unitOfWork.AuthorProduct.Add(newAP1);
                    _unitOfWork.Save();

                    var firstAuthor = _unitOfWork.AuthorProduct.GetAll(x => x.AuthorId == newAP1.AuthorId && x.ProductId == newAP1.ProductId, includeProperties: "Author").First();

                    obj.Product.Author = firstAuthor.Author.FirstName + " " + firstAuthor.Author.LastName;

                //update authors for product
                //set second author to null if none was chosen
                if (obj.Author2Id==0 || obj.Author2Id==null)
                    {

                        var o = _unitOfWork.AuthorProduct.GetAll(x => x.ProductId == obj.Product.Id);
                        if(o.Count()>1)
                        {
                            _unitOfWork.AuthorProduct.Remove(o.Last());
                            _unitOfWork.Save();
                        }
                    }
                    else
                    {
                        var ap2 = _unitOfWork.AuthorProduct.GetAll(x => x.ProductId == obj.Product.Id, includeProperties: "Author");
                        if (ap2.Count() > 1)
                        {
                            
                            _unitOfWork.AuthorProduct.Remove(ap2.Last());
                            _unitOfWork.Save();

                            var newAP2 = new AuthorProduct
                            {
                                ProductId = obj.Product.Id,
                                AuthorId = obj.Author2Id
                            };
                            _unitOfWork.AuthorProduct.Add(newAP2);
                            _unitOfWork.Save();

                            var secondAuthor = _unitOfWork.AuthorProduct.GetAll(x => x.AuthorId == newAP2.AuthorId && x.ProductId == newAP2.ProductId, includeProperties: "Author").First();

                            author2Name = secondAuthor.Author.FirstName + " " + secondAuthor.Author.LastName;
                        }
                        else
                        {
                            var newAp = new AuthorProduct
                            {
                                AuthorId = obj.Author2Id,
                                ProductId = obj.Product.Id
                            };

                            _unitOfWork.AuthorProduct.Add(newAp);
                            _unitOfWork.Save();

                            var secondAuthor = _unitOfWork.AuthorProduct.GetAll(x => x.AuthorId == obj.Author2Id && x.ProductId == obj.Product.Id, includeProperties: "Author").First();
                            author2Name = secondAuthor.Author.FirstName + " " + secondAuthor .Author.LastName;

                        }
                    }      



                    //var apList = _unitOfWork.AuthorProduct.GetAll(x => x.ProductId == obj.Product.Id, includeProperties: "Author");

       
                    
                    //_unitOfWork.AuthorProduct.Remove(apList.First());
                    //_unitOfWork.Save();

                    //var newAP1 = new AuthorProduct
                    //{
                    //    ProductId= obj.Product.Id,
                    //    AuthorId=obj.AuthorId
                    //};
                    //_unitOfWork.AuthorProduct.Add(newAP1);
                    //_unitOfWork.Save();

                //}
                //else
                //{
                //    var temp = apList.First();
                //    _unitOfWork.AuthorProduct.Remove(apList.First());
                //    _unitOfWork.Save();

                //    _unitOfWork.AuthorProduct.Add(temp);
                //    _unitOfWork.Save();

                //    var firstAuthor = _unitOfWork.AuthorProduct.GetAll(x => x.AuthorId == temp.AuthorId && x.ProductId == temp.ProductId, includeProperties: "Author").First();
                //    firstAuthorName = firstAuthor.Author.FirstName + " " + firstAuthor.Author.LastName;
                //}
                //ap.AuthorId = obj.AuthorId;
                //    _unitOfWork.AuthorProduct.Update(ap);


                //update product
               
                    
                    if (!string.IsNullOrEmpty(author2Name))
                        obj.Product.Author += ", "+ author2Name;


                    _unitOfWork.Product.Update(obj.Product);
                    _unitOfWork.Save();
                    TempData["success"] = "Product edited successfully";
                    return RedirectToAction("Index");
                }
                
            //}
            
            //return View(obj);
        }

        public IActionResult UpdateFeaturedProduct()
        {
            var productVM = new ProductFeaturedVM
            {
                Products = _unitOfWork.Product.GetAll().Select(
                u => new SelectListItem
                {
                    Text = u.Title,
                    Value = u.Id.ToString()
                }),
            };

            if (_unitOfWork.Product.GetFirstOrDefault(x => x.Featured == true) != null)
                productVM.ProductId = _unitOfWork.Product.GetFirstOrDefault(x => x.Featured == true).Id;

            return PartialView("_UpdateFeaturedProduct", productVM);
        }

        [HttpPost]
        public IActionResult UpdateFeaturedProduct(ProductFeaturedVM obj)
        {
            _unitOfWork.Product.UpdateStatusFeaturedProduct(obj.ProductId);
            _unitOfWork.Save();


            return RedirectToAction("Index");
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
            //first we delete every row in joint table AuthorProduct with containing product
            var apObjFromDb = _unitOfWork.AuthorProduct.GetAll(x => x.ProductId == id);
            if (apObjFromDb == null)
                return Json(new { success = false, message = "Error while deleting" });

            foreach (var ap in apObjFromDb)
            {
                _unitOfWork.AuthorProduct.Remove(ap);
                _unitOfWork.Save();
            }

            //then we delete every row in Review table with containing product we aim to delete
            var reviewsWithProducts = _unitOfWork.Review.GetAll(x => x.ProductId == id);

            foreach(var review in reviewsWithProducts)
            {
                _unitOfWork.Review.Remove(review);
                _unitOfWork.Save();
            }

            //when we've removed products from linked tables, now we can delete actual Product object
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
