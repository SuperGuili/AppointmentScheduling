using AppointmentScheduling.Models.ViewModels;
using AppointmentScheduling.Services;
using AppointmentScheduling.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AppointmentScheduling.Controllers.Api
{
    [Route("api/Appointment")]
    [ApiController]
    public class AppointmentApiController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IAppointmentService _appointmentService;

        private readonly string _loginUserId;
        private readonly string _role;

        public AppointmentApiController(IAppointmentService db, IHttpContextAccessor httpContextAccessor)
        {
            _appointmentService = db;
            _httpContextAccessor = httpContextAccessor;

            _loginUserId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            _role = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
        }

        [HttpPost]
        [Route("SaveCalendarData")]
        public IActionResult SaveCalendarData(AppointmentVM data)
        {
            CommonResponseVM<int> commonResponseVM = new CommonResponseVM<int>();

            try
            {
                commonResponseVM.status = _appointmentService.AddUpdate(data).Result;
                if (commonResponseVM.status == 1)
                {
                    commonResponseVM.message = Helper.appointmentUpdated;//////
                }
                if (commonResponseVM.status == 2)
                {
                    commonResponseVM.message = Helper.appointmentAdded;
                }
            }
            catch (Exception e)
            {
                commonResponseVM.message = e.Message;
                commonResponseVM.status = Helper.failure_code;
            }

            return Ok(commonResponseVM);
        }

        [HttpGet]
        [Route("GetCalendarData")]
        public IActionResult GetCalendarData(string doctorId)
        {
            CommonResponseVM<List<AppointmentVM>> commonResponseVM = new CommonResponseVM<List<AppointmentVM>>();

            try
            {
                if (_role == Helper.Patient)
                {
                    commonResponseVM.dataenum = _appointmentService.PatientsEventById(_loginUserId);
                    commonResponseVM.status = Helper.success_code;
                }
                else if (_role == Helper.Doctor)
                {
                    commonResponseVM.dataenum = _appointmentService.DoctorsEventById(_loginUserId);
                    commonResponseVM.status = Helper.success_code;
                }
                else
                {
                    commonResponseVM.dataenum = _appointmentService.DoctorsEventById(doctorId);
                    commonResponseVM.status = Helper.success_code;
                }
            }
            catch (Exception e)
            {
                commonResponseVM.message = e.Message;
                commonResponseVM.status = Helper.failure_code;
            }

            return Ok(commonResponseVM);
        }

        [HttpGet]
        [Route("GetCalendarDataById/{id}")]
        public IActionResult GetCalendarDataById(int id)
        {
            CommonResponseVM<AppointmentVM> commonResponseVM = new CommonResponseVM<AppointmentVM>();

            try
            {
                commonResponseVM.dataenum = _appointmentService.GetAppointmentById(id);
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
        [Route("DeleteAppointment/{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            CommonResponseVM<int> commonResponseVM = new CommonResponseVM<int>();

            try
            {
                commonResponseVM.status = await _appointmentService.DeleteAppointment(id);// Response = 1 OK
                if (commonResponseVM.status == 1 || commonResponseVM.status == 0)
                {
                    commonResponseVM.message = commonResponseVM.status == 1 ? Helper.appointmentDeleted : Helper.somethingWentWrong;
                }
                else // Response 2 - Error on delete a paid appointment
                {
                    commonResponseVM.message = Helper.DeletePaidAppError;
                }


            }
            catch (Exception e)
            {
                commonResponseVM.message = e.Message;
                commonResponseVM.status = Helper.failure_code;
            }

            return Ok(commonResponseVM);
        }

        [HttpGet]
        [Route("ConfirmAppointment/{id}")]
        public IActionResult ConfirmAppointment(int id)
        {
            CommonResponseVM<int> commonResponseVM = new CommonResponseVM<int>();

            try
            {
                var result = _appointmentService.ConfirmAppointment(id).Result;

                if (result > 0) // 0 == error
                {
                    commonResponseVM.status = Helper.success_code;
                    commonResponseVM.message = Helper.appointmentConfirmed;
                }
                else
                {
                    commonResponseVM.status = Helper.failure_code;
                    commonResponseVM.message = Helper.appointmentConfirmedError;
                }

            }
            catch (Exception e)
            {
                commonResponseVM.message = e.Message;
                commonResponseVM.status = Helper.failure_code;
            }

            return Ok(commonResponseVM);
        }

        [HttpGet]
        [Route("ConfirmPayment/{id}")]
        public IActionResult ConfirmPayment(int id) //appointment ID
        {
            CommonResponseVM<int> commonResponseVM = new CommonResponseVM<int>();

            try
            {
                var result = _appointmentService.ConfirmPayment(id).Result;

                if (result > 0) // 0 == error
                {
                    commonResponseVM.status = Helper.success_code;
                    commonResponseVM.message = Helper.PaymentConfirmed;
                }
                else
                {
                    commonResponseVM.status = Helper.failure_code;
                    commonResponseVM.message = Helper.PaymentConfirmedError;
                }

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
