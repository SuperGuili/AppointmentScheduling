using AppointmentScheduling.Models.ViewModels;
using AppointmentScheduling.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppointmentScheduling.Controllers
{
    [Authorize]
    public class AppointmentController : Controller
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        public IActionResult Index()
        {
            ViewBag.DoctorsList = _appointmentService.GetDoctorList();

            ViewBag.PatientsList = _appointmentService.GetPatientList();

            ViewBag.Duration = AppointmentScheduling.Utils.Helper.GetTimeDropDown();

            ViewBag.Value = _appointmentService.GetFeeList();

            return View();
        }
    }
}
