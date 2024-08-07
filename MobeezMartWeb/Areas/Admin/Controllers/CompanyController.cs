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

public class CompanyController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public CompanyController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {

        return View();
    }

    //GET
    public IActionResult Upsert(int? id)
    {
        Company company = new();
       

        if (id == null || id ==0)
        {
            return View(company);
        }
        else
        {
            company = _unitOfWork.Company.GetFirstOrDefault(u=>u.Id==id);
            return View(company);
        }

    }
    //POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Upsert(Company obj, IFormFile? file)
    {

        if (ModelState.IsValid)
        {
             
            if (obj.Id == 0) { 
                _unitOfWork.Company.Add(obj);
                TempData["success"] = "Company created sucessfully";
            }
            else
            {
                _unitOfWork.Company.Update(obj);
                TempData["success"] = "Company updated sucessfully";


            }
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
        return View(obj);
    }
    #region API CALLS
    [HttpGet]
    public IActionResult GetAll()
    {
        var CompanyList = _unitOfWork.Company.GetAll();
        return Json(new {data = CompanyList });
    }

    //POST
    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var obj = _unitOfWork.Company.GetFirstOrDefault(u => u.Id == id);
        if (obj == null)
        {
            return Json(new {success = false, message = "Error while deleting"});
        }

        _unitOfWork.Company.Remove(obj);
        _unitOfWork.Save();
        return Json(new { success = true, message = "Delete Successful" });
    }
    #endregion
}
