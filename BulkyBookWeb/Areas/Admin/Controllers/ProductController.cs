﻿using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            IEnumerable<Product> products = _unitOfWork.Product.GetAll();
            return View(products);
        }


        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new ProductVM();
            if (id == 0 | id == null)
            {
                productVM.Product = new();
                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(
                    u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    });
                productVM.CoverTypeList = _unitOfWork.CoverType.GetAll().Select(
                   u => new SelectListItem
                   {
                       Text = u.Name,
                       Value = u.Id.ToString()
                   });

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
        public IActionResult Upsert(ProductVM obj, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                if (obj.Product.Id != 0 | obj.Product.Id != null)
                {
                    //_unitOfWork.Product.Update(obj.Product);
                    _unitOfWork.Save();
                    TempData["success"] = "Product edited successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    _unitOfWork.Product.Add(obj.Product);
                    _unitOfWork.Save();
                    TempData["success"] = "Product created successfully";
                    return RedirectToAction("Index");
                }
            }
            return View("Edit",obj);
        }

        public IActionResult Delete(int? id)
        {
            if (id == 0 | id == null)
                return NotFound();

            var objFromDb = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == id);
            if (objFromDb == null)
                return NotFound();

            _unitOfWork.Product.Remove(objFromDb);
            _unitOfWork.Save();
            TempData["success"] = "Product deleted successfully";
            return RedirectToAction("Index");
        }

    }
}