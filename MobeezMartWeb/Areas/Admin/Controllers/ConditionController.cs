using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobeezMart.DataAccess;
using MobeezMart.DataAccess.Repository;
using MobeezMart.DataAccess.Repository.IRepository;
using MobeezMart.Models;
using MobeezMart.Utility;

namespace MobeezMartWeb.Controllers;
[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]

public class ConditionController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public ConditionController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        IEnumerable<Condition> objConditionList = _unitOfWork.Condition.GetAll();
        return View(objConditionList);
    }
    //GET
    public IActionResult Create()
    {
        return View();
    }
    //POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Condition obj)
    {
     
        if (ModelState.IsValid) {
            _unitOfWork.Condition.Add(obj);
            _unitOfWork.Save();
            TempData["success"] = "Condition created sucessfully";
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
        var ConditionFromDbFirst = _unitOfWork.Condition.GetFirstOrDefault(u => u.Id == id);
        //var brandFromDbSingle = _db.Brands.SingleOrDefault(u => u.Id == id);

        if (ConditionFromDbFirst == null)
        {
            return NotFound();
        }

        return View(ConditionFromDbFirst);
    }
    //POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Condition obj)
    {
       
        if (ModelState.IsValid)
        {
            _unitOfWork.Condition.Update(obj);
            _unitOfWork.Save();
            TempData["success"] = "Condition updated sucessfully";
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
        var ConditionFromDbFirst = _unitOfWork.Condition.GetFirstOrDefault(u=>u.Id== id);
        //var brandFromDbSingle = _db.Brands.SingleOrDefault(u => u.Id == id);

        if (ConditionFromDbFirst == null)
        {
            return NotFound();
        }

        return View(ConditionFromDbFirst);
    }
    //POST
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeletePOST(int? id)
    {
       var obj = _unitOfWork.Condition.GetFirstOrDefault(u => u.Id == id);
        if (obj == null)
        {
            return NotFound();
        }
        _unitOfWork.Condition.Remove(obj);
        _unitOfWork.Save();
        TempData["success"] = "Condition deleted sucessfully";
        return RedirectToAction("Index");
    }
}
