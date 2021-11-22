using AppointmentScheduling.Models;
using AppointmentScheduling.Services;
using AppointmentScheduling.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AppointmentScheduling.Controllers
{
    [Authorize(Roles = Helper.Admin)]
    public class ExpenseController : Controller
    {
        private readonly IFinanceService _financeService;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _loginUserId;

        public ExpenseController(IFinanceService financeService, IHttpContextAccessor httpContextAccessor)
        {
            _financeService = financeService;
            _httpContextAccessor = httpContextAccessor;

            _loginUserId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public IActionResult Index()
        {
            var listVM = _financeService.GetExpenseList();            

            return View(listVM);
        }

        [HttpGet]
        public IActionResult AddExpense()
        {            
            
            ViewBag.ExpenseTypeList = GetExpenseTypeList();
            ViewBag.LoginUserId = _loginUserId;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddExpense(Expense model)
        {
            if (ModelState.IsValid)
            {
                _financeService.AddExpense(model);
                return RedirectToAction("Index");
            }                       

            ViewBag.ExpenseTypeList = GetExpenseTypeList();

            return View();
        }


        [HttpGet]
        public IActionResult AddExpenseType()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddExpenseType(ExpenseType model)
        {
            if (ModelState.IsValid)
            {
                _financeService.AddExpenseType(model);
                return RedirectToAction("AddExpense");

            }

            return View();
        }

        [HttpGet]
        public IActionResult UpdateExpense(int id)
        {
            //get expense by ID
            var expense = _financeService.GetExpenseById(id);

            ViewBag.ExpenseTypeList = GetExpenseTypeList();
            ViewBag.LoginUserId = _loginUserId;

            return View(expense);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateExpense(Expense model)
        {
            if (ModelState.IsValid)
            {
                _financeService.AddExpense(model);
                return RedirectToAction("Index");
            }

            ViewBag.ExpenseTypeList = GetExpenseTypeList();
            return View();
        }

        private List<SelectListItem> GetExpenseTypeList()
        {
            List<SelectListItem> list = new();
            var types = _financeService.GetExpenseTypeList();

            foreach (var description in types)
            {
                list.Add(new SelectListItem { Text = description.ExpenseTypeDescription });
            }

            return list;
        }

    }
}
