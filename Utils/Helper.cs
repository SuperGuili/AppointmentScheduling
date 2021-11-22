using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppointmentScheduling.Utils
{
    public static class Helper
    {
        public const string Admin = "Admin";
        public const string Reception = "Reception";
        public static string Patient = "Patient";
        public static string Doctor = "Doctor";

        public static string appointmentAdded = "Appointment added succesfully.";
        public static string appointmentUpdated = "Appointment updated succesfully.";
        public static string appointmentDeleted = "Appointment deleted succesfully.";
        public static string appointmentExists = "Appointment for select date and time already exists.";
        public static string appointmentNotExists = "Appointment do not exists.";
        public static string appointmentConfirmed = "Appointment is confirmed succesfully.";
        public static string PaymentConfirmed = "Payment Confirmed successfully.";

        public static string appointmentAddError = "Something went wrong on adding, please try again.";
        public static string appointmentUpdateError = "Something went wrong on update, please try again.";
        public static string somethingWentWrong = "Something went wrong, please try again.";
        public static string appointmentConfirmedError = "Something went wrong on confirmation, please try again.";
        public static string PaymentConfirmedError = "Payment NOT Confirmed ERROR, please try again.";
        public static string DeletePaidAppError = "Cannot delete a paid appointment, please try again.";

        public static int success_code = 1;
        public static int failure_code = 0;

        public static List<SelectListItem> GetRolesForDropDown(bool isAdmin)
        {
            if (isAdmin)
            {
                return new List<SelectListItem>
                {
                    new SelectListItem{Value=Helper.Admin, Text=Helper.Admin},
                    new SelectListItem{Value=Helper.Patient, Text=Helper.Patient},
                    new SelectListItem{Value=Helper.Doctor, Text=Helper.Doctor},
                    new SelectListItem{Value=Helper.Reception, Text=Helper.Reception}
                };
            }
            else
            {
                return new List<SelectListItem>
                {                    
                    new SelectListItem{Value=Helper.Patient, Text=Helper.Patient},
                    //new SelectListItem{Value=Helper.Doctor, Text=Helper.Doctor}
                };
            }
        }

        public static List<SelectListItem> GetTimeDropDown()
        {
            //duration for the fullCalendar is in minutes
            int hour = 60;
            int day = hour * 24;
            int week = day * 7;

            List<SelectListItem> duration = new();

            // Fifteen minutes
            duration.Add(new SelectListItem { Value = (hour / 4).ToString(), Text = " 15 min" });

            // Half Hour
            duration.Add(new SelectListItem { Value = (hour / 2).ToString(), Text = " 30 min" });

            // One hour to i-hours
            for (int i = 1; i < 7; i++)
            {
                duration.Add(new SelectListItem { Value = hour.ToString(), Text = i + " Hr" });
                hour += 30;
                duration.Add(new SelectListItem { Value = hour.ToString(), Text = i + " Hr 30 min" });
                hour += 30;
            }

            //Day Off 24hs
            duration.Add(new SelectListItem { Value = day.ToString(), Text = "Day Off" });
            //Week Off 7 days
            duration.Add(new SelectListItem { Value = week.ToString(), Text = "Week Off" });

            return duration;
        }



    }
}
