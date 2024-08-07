using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobeezMart.DataAccess;
using MobeezMart.DataAccess.Repository;
using MobeezMart.DataAccess.Repository.IRepository;
using MobeezMart.Models;
using MobeezMart.Utility;

namespace MobeezMartWeb.Controllers;
[Area("Admin")]
[Authorize(Roles =SD.Role_Admin)]
public class BrandController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public BrandController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        IEnumerable<Brand> objBrandList = _unitOfWork.Brand.GetAll();
        return View(objBrandList);
    }
    //GET
    public IActionResult Create()
    {
        return View();
    }
    //POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Brand obj)
    {
        if (obj.Name == obj.DisplayOrder.ToString())
        {
            ModelState.AddModelError("name", "The DisplayOrder cannont exactly match the Name.");
        }
        if (ModelState.IsValid) {
            _unitOfWork.Brand.Add(obj);
            _unitOfWork.Save();
            TempData["success"] = "Brand created sucessfully";
            return RedirectToAction("Index");
        }
        return View(obj);
    }
    //GET
    public IActionResult Edit(int? id)
    {
        if(id == null || id ==0)
        {
            return NotFound();
        }
        //var brandFromDb = _db.Brands.Find(id);
        var brandFromDbFirst = _unitOfWork.Brand.GetFirstOrDefault(u => u.Id == id);
        //var brandFromDbSingle = _db.Brands.SingleOrDefault(u => u.Id == id);

        if (brandFromDbFirst == null)
        {
            return NotFound();
        }

        return View(brandFromDbFirst);
    }
    //POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Brand obj)
    {
        if (obj.Name == obj.DisplayOrder.ToString())
        {
            ModelState.AddModelError("name", "The DisplayOrder cannont exactly match the Name.");
        }
        if (ModelState.IsValid)
        {
            _unitOfWork.Brand.Update(obj);
            _unitOfWork.Save();
            TempData["success"] = "Brand updated sucessfully";
            return RedirectToAction("Index");
        }
        return View(obj);
    }
    //GET
    public IActionResult Delete(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }
        //var brandFromDb = _db.Brands.Find(id);
        var brandFromDbFirst = _unitOfWork.Brand.GetFirstOrDefault(u=>u.Id== id);
        //var brandFromDbSingle = _db.Brands.SingleOrDefault(u => u.Id == id);

        if (brandFromDbFirst == null)
        {
            return NotFound();
        }

        return View(brandFromDbFirst);
    }
    //POST
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeletePOST(int? id)
    {
       var obj = _unitOfWork.Brand.GetFirstOrDefault(u => u.Id == id);
        if (obj == null)
        {
            return NotFound();
        }
        _unitOfWork.Brand.Remove(obj);
        _unitOfWork.Save();
        TempData["success"] = "Brand deleted sucessfully";
        return RedirectToAction("Index");
    }
}
