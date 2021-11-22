using AppointmentScheduling.Models;
using AppointmentScheduling.Models.ViewModels;
using AppointmentScheduling.Services;
using AppointmentScheduling.Utils;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppointmentScheduling.Controllers.Api
{
    [Route("api/Finance")]
    [ApiController]
    public class FinanceApiController : Controller
    {
        private readonly IFinanceService _financeService;

        public FinanceApiController(IFinanceService financeService)
        {
            _financeService = financeService;
        }

        [HttpGet]
        [Route("Details/{id}")]
        public IActionResult Details(int id) // application id
        {
            CommonResponseVM<AppointmentVM> commonResponseVM = new CommonResponseVM<AppointmentVM>();

            try
            {
                commonResponseVM.dataenum = _financeService.GetAppointmentDetails(id);
                commonResponseVM.status = Helper.success_code;
            }
            catch (Exception e)
            {
                commonResponseVM.message = e.Message;
                commonResponseVM.status = Helper.failure_code;
            }

            return Ok(commonResponseVM);

        }

        [HttpGet]
        [Route("DetailsExpense/{id}")]
        public IActionResult DetailsExpense(int id) // expense id
        {
            CommonResponseVM<Expense> commonResponseVM = new CommonResponseVM<Expense>();

            try
            {
                commonResponseVM.dataenum = _financeService.GetExpenseById(id);
                commonResponseVM.message = _financeService.GetExpenseUserById(commonResponseVM.dataenum.ExpenseUserId);
                commonResponseVM.status = Helper.success_code;
            }
            catch (Exception e)
            {
                commonResponseVM.message = e.Message;
                commonResponseVM.status = Helper.failure_code;
            }

            return Ok(commonResponseVM);

        }

    }
}
