using AppointmentScheduling.Models.ViewModels;
using AppointmentScheduling.Services;
using AppointmentScheduling.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace AppointmentScheduling.Controllers
{
    [Authorize(Roles = Helper.Admin)]
    public class FinanceController : Controller
    {
        private readonly IFinanceService _financeService;

        public FinanceController(IFinanceService financeService)
        {
            _financeService = financeService;
        }

        public IActionResult Index(int pageNum = 1, int pageSize = 100)
        {
            //IEnumerable<ListFinanceVM> list = _financeService.GetFinanceList();
            int countRows = 0;
            // Paginated list            
            var pagedList = _financeService.GetFinanceList().ToPagedList(pageNum, pageSize);

            if (pageNum > 1)
            {
                countRows = pagedList.FirstItemOnPage -1;/////
            }

            ViewBag.countRows = countRows;

            return View(pagedList);
        }

        public IActionResult SearchFinance(DateTime startDate, DateTime endDate,
            string description, string inOut, string isPaid, int pageSize = 100, int pageNum = 1)
        {
            int countRows = 0;

            // Paginated list
            var pagedList = _financeService.SearchFinance(
                startDate, endDate, description, inOut, isPaid).ToPagedList(pageNum, pageSize);

            //store variables for other pages to keep the search
            ViewBag.isSearch = true;
            ViewBag.pageSize = pageSize;
            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;
            ViewBag.description = description;
            ViewBag.inOut = inOut;
            ViewBag.isPaid = isPaid;

            if (pageNum > 1)
            {
                countRows = pagedList.FirstItemOnPage - 1;/////
            }

            ViewBag.countRows = countRows;

            return View("Index", pagedList); //return the filtered List to index
        }


    }
}
