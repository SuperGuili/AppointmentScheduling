using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppointmentScheduling.Models.ViewModels
{
    public class ListExpenseVM
    {

        public int Id { get; set; }

        public string ExpenseDescription { get; set; }

        public string ExpenseDate { get; set; }

        public string ExpenseAmount { get; set; }


        public string ExpenseUserId { get; set; }


        public string ExpenseType { get; set; }
    }
}
