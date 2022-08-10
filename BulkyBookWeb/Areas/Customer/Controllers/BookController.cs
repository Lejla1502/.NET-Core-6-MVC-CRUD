using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.Pagination;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class BookController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        //private readonly ILogger<HomeController> _logger;

        public BookController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index( int? CategoryId, int? AuthorId, string BookTitle, int pg = 1)
        {
            //IEnumerable<Product> listOfProducts = _unitOfWork.Product.GetAll(includeProperties:"Category,CoverType");

            ProductReviewVM productReviewVM = new ProductReviewVM();

            //if (CategoryId == null && String.IsNullOrEmpty(BookTitle) && AuthorId==null)
            //{
            //    productReviewVM.Products = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType").Select(x =>  new ProductReviewVM.ProductInfo
            //    {
            //        Id = x.Id,
            //        Title = x.Title,
            //        Description = x.Description,
            //        ISBN = x.ISBN,
            //        Author = string.Join(", ",_unitOfWork.AuthorProduct.GetAll(ap => ap.ProductId == x.Id, includeProperties: "Product,Author").Select(s => s.Author.FirstName + ' ' + s.Author.LastName).ToArray()),//.First().Author.FirstName+' '+ _unitOfWork.AuthorProduct.GetAll(ap => ap.ProductId == x.Id, includeProperties: "Product,Author").First().Author.LastName,
            //        ListPrice = x.ListPrice,
            //        Price = x.Price,
            //        Price50 = x.Price50,
            //        Price100 = x.Price100,
            //        IsFavourite = x.IsFavourite,
            //        ImageUrl = x.ImageUrl,
            //        AvgRating = (_unitOfWork.Review.GetAll(y => y.ProductId == x.Id).Count() > 0) ? (_unitOfWork.Review.GetAll(y => y.ProductId == x.Id).Select(s => s.Rating).Sum() / _unitOfWork.Review.GetAll(y => y.ProductId == x.Id).Count()) : 0
            //    }).ToList();

               
            //}
            if (CategoryId != null)
            {
                if (String.IsNullOrEmpty(BookTitle) && AuthorId == null)
                {
                    productReviewVM.Products = _unitOfWork.Product.GetAll(p => p.CategoryId == CategoryId, includeProperties: "Category,CoverType").Select(x => new ProductReviewVM.ProductInfo
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Description = x.Description,
                        ISBN = x.ISBN,
                        Author = _unitOfWork.AuthorProduct.GetAll(ap => ap.ProductId == x.Id, includeProperties: "Product,Author").Select(s => s.Author.FirstName + ' ' + s.Author.LastName).First(),//.First().Author.FirstName+' '+ _unitOfWork.AuthorProduct.GetAll(ap => ap.ProductId == x.Id, includeProperties: "Product,Author").First().Author.LastName,
                        ListPrice = x.ListPrice,
                        Price = x.Price,
                        Price50 = x.Price50,
                        Price100 = x.Price100,
                        IsFavourite = x.IsFavourite,
                        ImageUrl = x.ImageUrl,
                        AvgRating = (_unitOfWork.Review.GetAll(y => y.ProductId == x.Id).Count() > 0) ? (_unitOfWork.Review.GetAll(y => y.ProductId == x.Id).Select(s => s.Rating).Sum() / _unitOfWork.Review.GetAll(y => y.ProductId == x.Id).Count()) : 0
                    }).ToList();
                }
                if(!String.IsNullOrEmpty(BookTitle)  && AuthorId==null)
                {
                    productReviewVM.Products = _unitOfWork.Product.GetAll(p => p.CategoryId == CategoryId && p.Title.Contains(BookTitle), includeProperties: "Category,CoverType").Select(x => new ProductReviewVM.ProductInfo
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Description = x.Description,
                        ISBN = x.ISBN,
                        Author = _unitOfWork.AuthorProduct.GetAll(ap => ap.ProductId == x.Id, includeProperties: "Product,Author").Select(s => s.Author.FirstName + ' ' + s.Author.LastName).First(),//.First().Author.FirstName+' '+ _unitOfWork.AuthorProduct.GetAll(ap => ap.ProductId == x.Id, includeProperties: "Product,Author").First().Author.LastName,
                        ListPrice = x.ListPrice,
                        Price = x.Price,
                        Price50 = x.Price50,
                        Price100 = x.Price100,
                        IsFavourite = x.IsFavourite,
                        ImageUrl = x.ImageUrl,
                        AvgRating = (_unitOfWork.Review.GetAll(y => y.ProductId == x.Id).Count() > 0) ? (_unitOfWork.Review.GetAll(y => y.ProductId == x.Id).Select(s => s.Rating).Sum() / _unitOfWork.Review.GetAll(y => y.ProductId == x.Id).Count()) : 0
                    }).ToList();
                }
                if(String.IsNullOrEmpty(BookTitle) && AuthorId!= null)
                {
                    productReviewVM.Products = _unitOfWork.AuthorProduct.GetAll(p => p.AuthorId == AuthorId && p.Product.CategoryId==CategoryId, includeProperties: "Product,Author").Select(x => new ProductReviewVM.ProductInfo
                    {
                        Id = x.Product.Id,
                        Title = x.Product.Title,
                        Description = x.Product.Description,
                        ISBN = x.Product.ISBN,
                        Author = x.Author.FirstName + ' ' + x.Author.LastName,
                        ListPrice = x.Product.ListPrice,
                        Price = x.Product.Price,
                        Price50 = x.Product.Price50,
                        Price100 = x.Product.Price100,
                        IsFavourite = x.Product.IsFavourite,
                        ImageUrl = x.Product.ImageUrl,
                        AvgRating = (_unitOfWork.Review.GetAll(y => y.ProductId == x.Product.Id).Count() > 0) ? (_unitOfWork.Review.GetAll(y => y.ProductId == x.Product.Id).Select(s => s.Rating).Sum() / _unitOfWork.Review.GetAll(y => y.ProductId == x.Product.Id).Count()) : 0
                    }).ToList();
                }
                if(!String.IsNullOrEmpty(BookTitle) && AuthorId!=null)
                {
                    productReviewVM.Products = _unitOfWork.AuthorProduct.GetAll(p => p.AuthorId == AuthorId && p.Product.CategoryId == CategoryId && p.Product.Title.Contains(BookTitle), includeProperties: "Product,Author").Select(x => new ProductReviewVM.ProductInfo
                    {
                        Id = x.Product.Id,
                        Title = x.Product.Title,
                        Description = x.Product.Description,
                        ISBN = x.Product.ISBN,
                        Author = x.Author.FirstName + ' ' + x.Author.LastName,
                        ListPrice = x.Product.ListPrice,
                        Price = x.Product.Price,
                        Price50 = x.Product.Price50,
                        Price100 = x.Product.Price100,
                        IsFavourite = x.Product.IsFavourite,
                        ImageUrl = x.Product.ImageUrl,
                        AvgRating = (_unitOfWork.Review.GetAll(y => y.ProductId == x.Product.Id).Count() > 0) ? (_unitOfWork.Review.GetAll(y => y.ProductId == x.Product.Id).Select(s => s.Rating).Sum() / _unitOfWork.Review.GetAll(y => y.ProductId == x.Product.Id).Count()) : 0
                    }).ToList();
                }
            }
            else
            {
                //CategoryId==null
                if (String.IsNullOrEmpty(BookTitle) && AuthorId == null)
                {
                    productReviewVM.Products = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType").Select(x => new ProductReviewVM.ProductInfo
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Description = x.Description,
                        ISBN = x.ISBN,
                        Author = string.Join(", ", _unitOfWork.AuthorProduct.GetAll(ap => ap.ProductId == x.Id, includeProperties: "Product,Author").Select(s => s.Author.FirstName + ' ' + s.Author.LastName).ToArray()),//.First().Author.FirstName+' '+ _unitOfWork.AuthorProduct.GetAll(ap => ap.ProductId == x.Id, includeProperties: "Product,Author").First().Author.LastName,
                        ListPrice = x.ListPrice,
                        Price = x.Price,
                        Price50 = x.Price50,
                        Price100 = x.Price100,
                        IsFavourite = x.IsFavourite,
                        ImageUrl = x.ImageUrl,
                        AvgRating = (_unitOfWork.Review.GetAll(y => y.ProductId == x.Id).Count() > 0) ? (_unitOfWork.Review.GetAll(y => y.ProductId == x.Id).Select(s => s.Rating).Sum() / _unitOfWork.Review.GetAll(y => y.ProductId == x.Id).Count()) : 0
                    }).ToList();
                }
                if (!String.IsNullOrEmpty(BookTitle) && AuthorId == null)
                {
                    productReviewVM.Products = _unitOfWork.Product.GetAll(p => p.Title.Contains(BookTitle), includeProperties: "Category,CoverType").Select(x => new ProductReviewVM.ProductInfo
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Description = x.Description,
                        ISBN = x.ISBN,
                        Author = _unitOfWork.AuthorProduct.GetAll(ap => ap.ProductId == x.Id, includeProperties: "Product,Author").Select(s => s.Author.FirstName + ' ' + s.Author.LastName).First(),//.First().Author.FirstName+' '+ _unitOfWork.AuthorProduct.GetAll(ap => ap.ProductId == x.Id, includeProperties: "Product,Author").First().Author.LastName,
                        ListPrice = x.ListPrice,
                        Price = x.Price,
                        Price50 = x.Price50,
                        Price100 = x.Price100,
                        IsFavourite = x.IsFavourite,
                        ImageUrl = x.ImageUrl,
                        AvgRating = (_unitOfWork.Review.GetAll(y => y.ProductId == x.Id).Count() > 0) ? (_unitOfWork.Review.GetAll(y => y.ProductId == x.Id).Select(s => s.Rating).Sum() / _unitOfWork.Review.GetAll(y => y.ProductId == x.Id).Count()) : 0
                    }).ToList();
                }
                if (String.IsNullOrEmpty(BookTitle) && AuthorId != null)
                {
                    productReviewVM.Products = _unitOfWork.AuthorProduct.GetAll(p => p.AuthorId == AuthorId, includeProperties: "Product,Author").Select(x => new ProductReviewVM.ProductInfo
                    {
                        Id = x.Product.Id,
                        Title = x.Product.Title,
                        Description = x.Product.Description,
                        ISBN = x.Product.ISBN,
                        Author = x.Author.FirstName + ' ' + x.Author.LastName,
                        ListPrice = x.Product.ListPrice,
                        Price = x.Product.Price,
                        Price50 = x.Product.Price50,
                        Price100 = x.Product.Price100,
                        IsFavourite = x.Product.IsFavourite,
                        ImageUrl = x.Product.ImageUrl,
                        AvgRating = (_unitOfWork.Review.GetAll(y => y.ProductId == x.Product.Id).Count() > 0) ? (_unitOfWork.Review.GetAll(y => y.ProductId == x.Product.Id).Select(s => s.Rating).Sum() / _unitOfWork.Review.GetAll(y => y.ProductId == x.Product.Id).Count()) : 0
                    }).ToList();
                }
                if (!String.IsNullOrEmpty(BookTitle) && AuthorId != null)
                {
                    productReviewVM.Products = _unitOfWork.AuthorProduct.GetAll(p => p.AuthorId == AuthorId && p.Product.Title.Contains(BookTitle), includeProperties: "Product,Author").Select(x => new ProductReviewVM.ProductInfo
                    {
                        Id = x.Product.Id,
                        Title = x.Product.Title,
                        Description = x.Product.Description,
                        ISBN = x.Product.ISBN,
                        Author = x.Author.FirstName + ' ' + x.Author.LastName,
                        ListPrice = x.Product.ListPrice,
                        Price = x.Product.Price,
                        Price50 = x.Product.Price50,
                        Price100 = x.Product.Price100,
                        IsFavourite = x.Product.IsFavourite,
                        ImageUrl = x.Product.ImageUrl,
                        AvgRating = (_unitOfWork.Review.GetAll(y => y.ProductId == x.Product.Id).Count() > 0) ? (_unitOfWork.Review.GetAll(y => y.ProductId == x.Product.Id).Select(s => s.Rating).Sum() / _unitOfWork.Review.GetAll(y => y.ProductId == x.Product.Id).Count()) : 0
                    }).ToList();
                }
                //productReviewVM.Products = _unitOfWork.Product.GetAll(p => p.Author.Contains(BookTitle) == true || p.Title.Contains(BookTitle) == true, includeProperties: "Category,CoverType").Select(x => new ProductReviewVM.ProductInfo
                //{
                //    Id = x.Id,
                //    Title = x.Title,
                //    Description = x.Description,
                //    ISBN = x.ISBN,
                //    Author = x.Author,
                //    ListPrice = x.ListPrice,
                //    Price = x.Price,
                //    Price50 = x.Price50,
                //    Price100 = x.Price100,
                //    IsFavourite = x.IsFavourite,
                //    ImageUrl = x.ImageUrl,
                //    AvgRating = (_unitOfWork.Review.GetAll(y => y.ProductId == x.Id).Count() > 0) ? (_unitOfWork.Review.GetAll(y => y.ProductId == x.Id).Select(s => s.Rating).Sum() / _unitOfWork.Review.GetAll(y => y.ProductId == x.Id).Count()) : 0
                //}).ToList();
            }
            //else
            //{
            //    productReviewVM.Products = _unitOfWork.Product.GetAll(p => p.CategoryId == CategoryId && ((p.Author.Contains(BookTitle) == true) || (p.Title.Contains(BookTitle) == true)), includeProperties: "Category,CoverType").Select(x => new ProductReviewVM.ProductInfo
            //    {
            //        Id = x.Id,
            //        Title = x.Title,
            //        Description = x.Description,
            //        ISBN = x.ISBN,
            //        Author = x.Author,
            //        ListPrice = x.ListPrice,
            //        Price = x.Price,
            //        Price50 = x.Price50,
            //        Price100 = x.Price100,
            //        IsFavourite = x.IsFavourite,
            //        ImageUrl = x.ImageUrl,
            //        AvgRating = (_unitOfWork.Review.GetAll(y => y.ProductId == x.Id).Count() > 0) ? (_unitOfWork.Review.GetAll(y => y.ProductId == x.Id).Select(s => s.Rating).Sum() / _unitOfWork.Review.GetAll(y => y.ProductId == x.Id).Count()) : 0
            //    }).ToList();
            //}

            productReviewVM.CategoryList = _unitOfWork.Category.GetAll().Select(
                u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }
            );

            productReviewVM.AuthorList = _unitOfWork.Author.GetAll().Select(
               u => new SelectListItem
               {
                   Text = u.FirstName + ' ' + u.LastName,
                   Value = u.Id.ToString()
               }
           );

            const int pgSize = 8;

            if (pg < 1)
                pg = 1;

            int recsCount = productReviewVM.Products.Count();
            var pager = new Pager(recsCount, pg, pgSize);

            int recSkip = (pg - 1) * pgSize;

            productReviewVM.Products=productReviewVM.Products.Skip(recSkip).Take(pager.PageSize).ToList();  

            this.ViewBag.Pager=pager;

            return View(productReviewVM);
        }

        //public IActionResult GetBestseller()
        //{
        //    //returns id 
        //    var something= _unitOfWork.OrderDetail.GetAll().GroupBy(x => x.ProductId).Select(s => new { Bestseller_PrdouctID = s.Key, Count = s.Count() }).OrderByDescending(y=>y.Count).First();

        //    return View(something.Bestseller_PrdouctID);
        //}

        [HttpGet()]
        [AllowAnonymous]
        public async Task<ActionResult<int>> GetBestseller()
        {
            var something=  _unitOfWork.OrderDetail.GetAll().GroupBy(x => x.ProductId).Select(s => new { Bestseller_PrdouctID = s.Key, Count = s.Count() }).OrderByDescending(y=>y.Count).First();


            if (something == null)
            {
                return NotFound();
            }

            return something.Bestseller_PrdouctID;
        }

        [HttpGet()]
        [AllowAnonymous]
        public async Task<List<Product>> GetListOfBestsellers()
        {
            var something = _unitOfWork.OrderDetail.GetAll().GroupBy(x => x.ProductId).Select(s => new { Bestseller_PrdouctID = s.Key, Count = s.Count() }).OrderByDescending(y => y.Count).Take(4);

            List<Product> bestsellers = new();

            foreach(var p in something)
            {
                bestsellers.Add(_unitOfWork.Product.GetFirstOrDefault(x=>x.Id==p.Bestseller_PrdouctID));
            } 

            if (bestsellers.Count()<1)
            {
                return null;
            }

            return bestsellers;
        }

    }
}
