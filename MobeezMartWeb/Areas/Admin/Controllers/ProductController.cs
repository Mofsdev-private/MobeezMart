using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using MobeezMart.DataAccess;
using MobeezMart.DataAccess.Repository;
using MobeezMart.DataAccess.Repository.IRepository;
using MobeezMart.Models;
using MobeezMart.Models.ViewModels;
using MobeezMart.Utility;

namespace MobeezMartWeb.Controllers;
[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]

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

    //GET
    public IActionResult Upsert(int? id)
    {
        ProductVM productVM = new()
        {
            product = new(),
            BrandList = _unitOfWork.Brand.GetAll().Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            }),
            ConditionList = _unitOfWork.Condition.GetAll().Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            }),
        };

        if (id == null || id ==0)
        {
            //create product
            //ViewBag.BrandList = BrandList;
            //ViewData["ConditionList"] = ConditionList;
            return View(productVM);
        }
        else
        {
            productVM.product = _unitOfWork.Product.GetFirstOrDefault(u=>u.Id==id);
            return View(productVM);
            //update product
        }

    }
    //POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Upsert(ProductVM obj, IFormFile? file)
    {

        if (ModelState.IsValid)
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;
            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"images\products");
                var extension = Path.GetExtension(file.FileName);

                if (obj.product.ImageUrl != null)
                {
                    var oldImagePath = Path.Combine(wwwRootPath, obj.product.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    file.CopyTo(fileStreams);
                }
                obj.product.ImageUrl = @"\images\products\" + fileName + extension;
            }
            if (obj.product.Id == 0) { 
                _unitOfWork.Product.Add(obj.product);
            }
            else
            {
                _unitOfWork.Product.Update(obj.product);

            }
            _unitOfWork.Save();
            TempData["success"] = "Product created sucessfully";
            return RedirectToAction("Index");
        }
        //return RedirectToAction("Index");
        return View(obj);
    }
    #region API CALLS
    [HttpGet]
    public IActionResult GetAll()
    {
        var producList = _unitOfWork.Product.GetAll(includeProperties:"Brand,Condition");
        return Json(new {data = producList});
    }

    //POST
    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var obj = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
        if (obj == null)
        {
            return Json(new {success = false, message = "Error while deleting"});
        }

        var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
        if (System.IO.File.Exists(oldImagePath))
        {
            System.IO.File.Delete(oldImagePath);
        }

        _unitOfWork.Product.Remove(obj);
        _unitOfWork.Save();
        return Json(new { success = true, message = "Delete Successful" });
    }
    #endregion
}
