using AppointmentScheduling.Models;
using AppointmentScheduling.Models.ViewModels;
using AppointmentScheduling.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AppointmentScheduling.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly ApplicationDbContext _db;

        private readonly IEmailSender _emailSender;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _loginUserId;

        public AppointmentService(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor,
                                  IEmailSender emailSender)
        {
            _db = db;
            _emailSender = emailSender;
            _httpContextAccessor = httpContextAccessor;

            _loginUserId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        }

        public int AddFee(Fee model)
        {
            if (model.Id > 0)
            {
                Fee fee = _db.Fees.FirstOrDefault(f => f.Id == model.Id);

                if (fee != null)
                {
                    fee = model;
                    _db.Fees.Update(fee);
                    _db.SaveChanges();
                    return 1;
                }
                else
                {
                    return 0;
                }

            }
            else
            {
                if (model != null)
                {
                    Fee fee = new()
                    {
                        Description = model.Description,
                        Duration = model.Duration,
                        Value = model.Value,
                        AdminId = model.AdminId
                    };

                    _db.Fees.Add(fee);
                    _db.SaveChanges();

                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        public async Task<int> AddUpdate(AppointmentVM model)
        {
            var startDate = DateTime.Parse(model.StartDate);
            var endDate = DateTime.Parse(model.StartDate).AddMinutes(Convert.ToDouble(model.Duration));

            var patient = _db.Users.FirstOrDefault(u => u.Id == model.PatientId);
            var doctor = _db.Users.FirstOrDefault(u => u.Id == model.DoctorId);

            if (model != null && model.Id > 0) //UPDATE
            {
                // Id > 0 = Update ( returns # 1)
                var appointment = _db.Appointments.FirstOrDefault(a => a.Id == model.Id);

                if (appointment != null)
                {
                    appointment.StartDate = startDate;
                    appointment.Title = model.Title;
                    appointment.Duration = model.Duration;
                    appointment.EndDate = endDate;
                    appointment.Description = model.Description;
                    appointment.IsDoctorApproved = false; // Every time you edit an appointment it must be confirmed again.
                    appointment.PatientId = model.PatientId;
                    appointment.AdminId = _loginUserId;


                    _db.Appointments.Update(appointment);

                    Income income = _db.Incomes.FirstOrDefault(i => i.AppointmentId == model.Id);

                    if (income != null)
                    {
                        income.PatientId = model.PatientId;

                        //check for value change on Update
                        if (Decimal.Parse(model.Value) > income.Amount)
                        {
                            income.IsPaid = false;
                        }

                        income.Amount = Decimal.Parse(model.Value);

                        _db.Incomes.Update(income);
                    }
                    else
                    {
                        income = new Income()
                        {
                            PatientId = model.PatientId,
                            Amount = Decimal.Parse(model.Value),
                            IsPaid = model.IsPaid,
                            AppointmentId = (int)model.Id,
                            DoctorId = model.DoctorId
                        };

                        _db.Incomes.Add(income);

                    }

                    await _db.SaveChangesAsync();

                }

                return 1;
            }
            else //CREATE
            {
                // Id == 0 = Create ( returns # 2)
                Appointment appointment = new Appointment()
                {
                    Title = model.Title,
                    Description = model.Description,
                    StartDate = startDate,
                    EndDate = endDate,
                    Duration = model.Duration,
                    DoctorId = model.DoctorId,
                    PatientId = model.PatientId,
                    IsDoctorApproved = false,
                    AdminId = _loginUserId ///get user id
                };

                Income income = new Income()
                {
                    DoctorId = model.DoctorId,
                    PatientId = model.PatientId,
                    Amount = Decimal.Parse(model.Value),
                    IsPaid = false
                };

                //await _emailSender.SendEmailAsync(doctor.Email, "Appointment created",
                //    $"An Appoitment with {patient.Name} is created and in pending status, please confirm it for {appointment.StartDate}."
                //    );
                //await _emailSender.SendEmailAsync(patient.Email, $"Appointment with Dr. {doctor.Name} on {appointment.StartDate}",
                //    $"An Appoitment with {doctor.Name} is created and in pending status, please confirm it for {appointment.StartDate}."
                //    );

                _db.Appointments.Add(appointment);
                await _db.SaveChangesAsync();

                income.AppointmentId = appointment.Id;
                _db.Incomes.Add(income);

                await _db.SaveChangesAsync();

                return 2;
            }

            //SEED DATABASE----------------------START---------------- TEMP >>>>>>>>>>>>

            //startDate = DateTime.Parse("2021-11-01 08:00:00"); // Start date time to seed calendar

            //  // Id == 0 = Create ( returns # 2)
            //  for (int i = 0; i < 396; i++)
            //  {
            //      var temp = startDate.DayOfWeek;

            //      if (startDate.ToShortTimeString() == "12:00")
            //      {
            //          startDate += (new TimeSpan(0, 1, 0, 0));///////// 1 Hour lunch time
            //      }

            //      if (startDate.ToShortTimeString() == "18:00")
            //      {
            //          if (startDate.DayOfWeek.ToString() == "Friday")
            //          {
            //              startDate += (new TimeSpan(2, 14, 0, 0));///////// 2 days, 14 Hour break from 18:00 Friday to 8:00 Monday
            //          }
            //          else
            //          {
            //              startDate += (new TimeSpan(0, 14, 0, 0));///////// 14 Hour break from 18:00 to 8:00
            //          }
            //      }

            //      Appointment appointment = new Appointment()
            //      {
            //          Title = "Appointment SEED TITLE",
            //          Description = "Appointment SEED Description",
            //          StartDate = startDate,
            //          EndDate = startDate + new TimeSpan(0, 30, 0),
            //          Duration = 30,
            //          DoctorId = "f02de620-aabb-498d-86c8-6109cf790e6e",
            //          PatientId = "694a288c-c5fb-4b9a-92ab-bb43a2fa9f59",
            //          IsDoctorApproved = true,
            //          AdminId = "4d405622-62f0-48e1-abbd-575c7c417bfe"
            //      };
            //      _db.Appointments.Add(appointment);
            //      await _db.SaveChangesAsync();

            //      Income income = new Income()
            //      {
            //          DoctorId = "f02de620-aabb-498d-86c8-6109cf790e6e",
            //          PatientId = "694a288c-c5fb-4b9a-92ab-bb43a2fa9f59",
            //          Amount = Decimal.Parse("80.00"),
            //          IsPaid = true,
            //          Date = startDate,
            //          UserId = "4d405622-62f0-48e1-abbd-575c7c417bfe"
            //      };

            //      income.AppointmentId = appointment.Id;
            //      _db.Incomes.Add(income);

            //      await _db.SaveChangesAsync();

            //      startDate += (new TimeSpan(0, 0, 30, 0));///////// 30 minutes interval for next appointment
            //  }

            //  return 2;
            //SEED DATABASE---------------------- END ------------------------------->>>>>>
        }

        public async Task<int> ConfirmAppointment(int id)
        {

            var appointment = _db.Appointments.FirstOrDefault(a => a.Id == id);

            if (appointment != null)
            {
                appointment.IsDoctorApproved = true;

                return await _db.SaveChangesAsync(); //returns the number of records updated.
            }

            return 0;

        }

        public async Task<int> ConfirmPayment(int id) // appointment ID
        {
            var income = _db.Incomes.FirstOrDefault(i => i.AppointmentId == id);

            if (income != null)
            {
                income.IsPaid = true;
                income.Date = DateTime.Now;
                income.UserId = _loginUserId;
                return await _db.SaveChangesAsync(); //returns the number of records updated.
            }
            else
            {
                // message error: no income for this appointment.

            }

            return 0;
        }

        public async Task<int> DeleteAppointment(int id)
        {
            var appointment = _db.Appointments.FirstOrDefault(a => a.Id == id);
            var income = _db.Incomes.FirstOrDefault(i => i.AppointmentId == id);

            if (appointment != null)
            {
                if (!income.IsPaid) // check for payment before deleting >>>>>>>
                {
                    _db.Appointments.Remove(appointment);
                    _db.Incomes.Remove(income);

                    await _db.SaveChangesAsync(); //returns the number of records updated.

                    return 1;
                }
                else
                {
                    return 2; // error code for already paid appointment
                }
            }

            return 0;
        }

        public int DeleteFee(int id)
        {
            Fee fee = _db.Fees.Find(id);

            if (fee != null)
            {
                _db.Fees.Remove(fee);
                return _db.SaveChanges();                
            }

            return 0;
        }

        public List<AppointmentVM> DoctorsEventById(string doctorId)
        {
            return _db.Appointments.Where(a => a.DoctorId == doctorId).ToList().Select(x => new AppointmentVM()
            {
                Id = x.Id,
                Description = x.Description,
                StartDate = x.StartDate.ToString("yyyy-MM-dd HH:mm"),
                EndDate = x.EndDate.ToString("yyyy-MM-dd HH:mm"),
                Title = x.Title,
                Duration = x.Duration,
                IsDoctorApproved = x.IsDoctorApproved,
                PatientName = _db.Users.Where(u => u.Id == x.PatientId).Select(u => u.Name).FirstOrDefault(),
                IsPaid = _db.Incomes.Where(i => i.AppointmentId == x.Id).Select(p => p.IsPaid).FirstOrDefault()

            }).ToList();

        }

        public AppointmentVM GetAppointmentById(int id)
        {
            AppointmentVM appointment = _db.Appointments.Where(a => a.Id == id).ToList().Select(x => new AppointmentVM()
            {
                Id = x.Id,
                Description = x.Description,
                StartDate = x.StartDate.ToString(),
                EndDate = x.EndDate.ToString(),
                Title = x.Title,
                Duration = x.Duration,
                IsDoctorApproved = x.IsDoctorApproved,
                PatientId = x.PatientId,
                DoctorId = x.DoctorId,
                PatientName = _db.Users.Where(u => u.Id == x.PatientId).Select(u => u.Name).FirstOrDefault(),
                DoctorName = _db.Users.Where(u => u.Id == x.DoctorId).Select(u => u.Name).FirstOrDefault(),
                Telephone = _db.Users.Where(u => u.Id == x.PatientId).Select(u => u.Telephone).FirstOrDefault()


            }).SingleOrDefault();

            Income income = _db.Incomes.FirstOrDefault(i => i.AppointmentId == appointment.Id);

            if (income == null)///
            {
                appointment.Value = "0.00";
            }
            else
            {
                appointment.Value = income.Amount.ToString();
                appointment.IsPaid = income.IsPaid;
            }

            return appointment;
        }

        public List<DoctorViewModel> GetDoctorList()
        {
            var doctors = (from user in _db.Users
                           join userRoles in _db.UserRoles on user.Id equals userRoles.UserId
                           join roles in _db.Roles.Where(x => x.Name == Helper.Doctor) on userRoles.RoleId equals roles.Id
                           select new DoctorViewModel
                           {
                               Id = user.Id,
                               Name = user.Name
                           }).ToList();

            return doctors;
        }

        public Fee GetFeeById(int Id)
        {
            return _db.Fees.FirstOrDefault(f => f.Id == Id);
        }

        public IEnumerable<Fee> GetFeeList()
        {
            var fees = (from fee in _db.Fees
                        select new Fee
                        {
                            Id = fee.Id,
                            Description = fee.Description,
                            Duration = fee.Duration,
                            Value = fee.Value,
                            AdminId = fee.AdminId

                        }).ToList();

            return fees;
        }

        public List<PatientViewModel> GetPatientList()
        {
            var patients = (from user in _db.Users
                            join userRoles in _db.UserRoles on user.Id equals userRoles.UserId
                            join roles in _db.Roles.Where(x => x.Name == Helper.Patient) on userRoles.RoleId equals roles.Id
                            select new PatientViewModel
                            {
                                Id = user.Id,
                                Name = user.Name
                            }).ToList();

            return patients;
        }

        public List<AppointmentVM> PatientsEventById(string patientId)
        {
            return _db.Appointments.Where(a => a.PatientId == patientId).ToList().Select(x => new AppointmentVM()
            {
                Id = x.Id,
                Description = x.Description,
                StartDate = x.StartDate.ToString("yyyy-MM-dd HH:mm"),
                EndDate = x.EndDate.ToString("yyyy-MM-dd HH:mm"),
                Title = x.Title,
                Duration = x.Duration,
                IsDoctorApproved = x.IsDoctorApproved,
                PatientName = _db.Users.Where(u => u.Id == x.PatientId).Select(u => u.Name).FirstOrDefault()

            }).ToList();
        }
    }
}
