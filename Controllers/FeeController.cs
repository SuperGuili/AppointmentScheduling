using AppointmentScheduling.Models;
using AppointmentScheduling.Services;
using AppointmentScheduling.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppointmentScheduling.Controllers
{
    [Authorize(Roles = Helper.Admin)]
    public class FeeController : Controller
    {
        private readonly IAppointmentService _appointmentService;

        public FeeController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        public IActionResult Index()
        {
            List<Fee> fees = _appointmentService.GetFeeList().ToList();

            return View(fees);
        }

        [HttpGet]
        public IActionResult AddFee()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddFee(Fee model)
        {
            if (ModelState.IsValid)
            {
                _appointmentService.AddFee(model);
                return RedirectToAction("Index");

            }

            return View();
        }

        [HttpGet]
        public IActionResult UpdateFee(int id)
        {
            Fee fee = _appointmentService.GetFeeById(id);

            return View(fee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateFee(Fee model)
        {
            if (ModelState.IsValid)
            {
                Fee fee = _appointmentService.GetFeeById(model.Id);

                fee.AdminId = model.AdminId;
                fee.Description = model.Description;
                fee.Duration = model.Duration;
                fee.Value = model.Value;

                _appointmentService.AddFee(fee);

                return RedirectToAction("Index");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteFee(int id)
        {
            Fee fee = _appointmentService.GetFeeById(id);

            if (fee != null)
            {
                _appointmentService.DeleteFee(id);
            }

            return RedirectToAction("Index");
        }

    }
}
