using AppointmentScheduling.Models;
using AppointmentScheduling.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppointmentScheduling.Services
{
    public interface IAppointmentService
    {
        public List<DoctorViewModel> GetDoctorList();

        public List<PatientViewModel> GetPatientList();

        public Task<int> AddUpdate(AppointmentVM model);

        public List<AppointmentVM> DoctorsEventById(string doctorId);

        public List<AppointmentVM> PatientsEventById(string patientId);

        public AppointmentVM GetAppointmentById(int Id);

        public Task<int> DeleteAppointment(int id);

        public Task<int> ConfirmAppointment(int id);

        public IEnumerable<Fee> GetFeeList();

        public int AddFee(Fee model);

        public Task<int> ConfirmPayment(int id);

        public Fee GetFeeById(int Id);

        public int DeleteFee(int id);

    }
}
