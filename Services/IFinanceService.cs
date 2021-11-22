using AppointmentScheduling.Models;
using AppointmentScheduling.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppointmentScheduling.Services
{
    public interface IFinanceService
    {
        public IEnumerable<ListFinanceVM> GetFinanceList();

        public IEnumerable<ListFinanceVM> SearchFinance(DateTime startDate, DateTime endDate, string description, string inOut, string isPaid);

        public AppointmentVM GetAppointmentDetails(int id);

        public IEnumerable<ListExpenseVM> GetExpenseList();

        public int AddExpenseType(ExpenseType model);

        public IEnumerable<ExpenseType> GetExpenseTypeList();

        public int AddExpense(Expense model);

        public Expense GetExpenseById(int id);

        public string GetExpenseUserById(string id);

    }
}
