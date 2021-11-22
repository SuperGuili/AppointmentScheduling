using AppointmentScheduling.Models;
using AppointmentScheduling.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppointmentScheduling.Services
{
    public class FinanceService : IFinanceService
    {
        private readonly ApplicationDbContext _db;


        public FinanceService(ApplicationDbContext db)
        {
            _db = db;
        }

        public int AddExpense(Expense model)
        {
            if (model != null)
            {
                if (model.Id > 0)
                {
                    Expense expense = GetExpenseById(model.Id);

                    expense.ExpenseDescription = model.ExpenseDescription;
                    expense.ExpenseDate = model.ExpenseDate;
                    expense.ExpenseAmount = model.ExpenseAmount;
                    expense.ExpenseType = model.ExpenseType;
                    expense.ExpenseUserId = model.ExpenseUserId;

                    _db.Expenses.Update(expense);
                    _db.SaveChanges();

                    return 1;
                }
                else
                {
                    _db.Expenses.Add(model);
                    _db.SaveChanges();

                    return 1;
                }
            }

            return 0;
        }

        public int AddExpenseType(ExpenseType model)
        {
            if (model.Id > 0)
            {
                ExpenseType expenseType = _db.ExpenseTypes.FirstOrDefault(e => e.Id == model.Id);

                if (expenseType != null)
                {
                    expenseType = model;
                    _db.ExpenseTypes.Update(expenseType);
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
                    ExpenseType expenseType = new()
                    {
                        ExpenseTypeDescription = model.ExpenseTypeDescription
                    };

                    _db.ExpenseTypes.Add(expenseType);
                    _db.SaveChanges();

                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        public AppointmentVM GetAppointmentDetails(int id)
        {
            var appointment = _db.Appointments.FirstOrDefault(a => a.Id == id); // protect against null

            AppointmentVM vm = new() ///check 
            {
                Id = id,
                AdminId = appointment.AdminId,
                Title = appointment.Title,
                Description = appointment.Description,
                StartDate = appointment.StartDate.ToString(),
                EndDate = appointment.EndDate.ToString(),
                Duration = appointment.Duration,
                DoctorId = appointment.DoctorId,
                PatientId = appointment.PatientId,
                IsDoctorApproved = appointment.IsDoctorApproved,
                PaidDate = _db.Incomes.FirstOrDefault(i => i.AppointmentId == id).Date.ToString(),
                DoctorName = _db.Users.FirstOrDefault(d => d.Id == appointment.DoctorId).Name,
                PatientName = _db.Users.FirstOrDefault(p => p.Id == appointment.PatientId).Name,
                AdminName = _db.Users.FirstOrDefault(a => a.Id == appointment.AdminId).Name, 
                Value = _db.Incomes.FirstOrDefault(i => i.AppointmentId == id).Amount.ToString(),
                IsPaid = _db.Incomes.FirstOrDefault(i => i.AppointmentId == id).IsPaid,
                Telephone = _db.Users.FirstOrDefault(u => u.Id == appointment.PatientId).Telephone
            };

            return vm;

        }

        public Expense GetExpenseById(int id)
        {
            var expense = _db.Expenses.FirstOrDefault(e => e.Id == id);

            return expense;
        }
       
        public string GetExpenseUserById(string id) // user id
        {
            var user = _db.Users.FirstOrDefault(u => u.Id == id).Name;

            return user;
        }

        public IEnumerable<ListExpenseVM> GetExpenseList()
        {
            List<ListExpenseVM> listVM = new();

            foreach (var expense in _db.Expenses)
            {
                ListExpenseVM obj = new()
                {
                    Id = expense.Id,
                    ExpenseDescription = expense.ExpenseDescription,
                    ExpenseAmount = expense.ExpenseAmount.ToString(),
                    ExpenseDate = expense.ExpenseDate.ToShortDateString(),
                    ExpenseType = expense.ExpenseType,
                    ExpenseUserId = expense.ExpenseUserId
                };
                listVM.Add(obj);
            }

            var orderedList = listVM.OrderByDescending(e => e.ExpenseDate).ToList();

            return orderedList;
        }

        public IEnumerable<ExpenseType> GetExpenseTypeList()
        {
            List<ExpenseType> list = new();

            foreach (var item in _db.ExpenseTypes)
            {
                list.Add(item);
            }

            return list;
        }

        public IEnumerable<ListFinanceVM> GetFinanceList()
        {
            List<ListFinanceVM> listFinanceVM = new();

            foreach (var income in _db.Incomes.Where(i => i.Amount > 0))
            {
                ListFinanceVM obj = new()
                {
                    //Find Doctor Name
                    Description = _db.Users.Where(u => u.Id == income.DoctorId).Select(u => u.Name).FirstOrDefault(),
                    Date = income.Date,
                    Amount = income.Amount,
                    IncomeIsPaid = income.IsPaid,
                    Type = "IN",
                    AppointmentId = income.AppointmentId
                };
                listFinanceVM.Add(obj);
            }

            foreach (var expense in _db.Expenses)
            {
                ListFinanceVM obj = new()
                {
                    Description = expense.ExpenseDescription,
                    Date = expense.ExpenseDate,
                    Amount = -(expense.ExpenseAmount),
                    IncomeIsPaid = true,
                    Type = "OUT",
                    ExpenseId = expense.Id
                };
                listFinanceVM.Add(obj);
            }

            var orderedList = listFinanceVM.OrderByDescending(d => d.Date).ToList();

            return orderedList;
        }

        public IEnumerable<ListFinanceVM> SearchFinance(DateTime startDate, DateTime endDate,
                                                        string description, string inOut, string isPaid)
        {
            if (endDate.ToShortDateString() == "01/01/0001") // endDate empty changes to tomorrow
            {
                endDate = DateTime.Now + new TimeSpan(1, 0, 0, 0); // Add 1 day to end date
            }
            else // endDate gets extra 23:59hs
            {
                endDate += new TimeSpan(0, 23, 59, 0); // Add 23:59hs to include the whole day in the search
            }

            List<ListFinanceVM> listFinanceVM = new();

            // look just for doctor/description and Paid/Not Paid
            if (startDate.ToShortDateString() == "01/01/0001" && description != null && inOut == null)
            {
                // Check if the description is a doctor
                ApplicationUser doctor = _db.Users.FirstOrDefault(u => u.Name.Contains(description));

                if (doctor != null)
                {
                    if (isPaid == "PAID")
                    {
                        foreach (var income in _db.Incomes.Where(i => i.DoctorId == doctor.Id && i.Amount > 0 && i.IsPaid))
                        {
                            ListFinanceVM obj = new()
                            {
                                Description = _db.Users.Where(u => u.Id == income.DoctorId).Select(u => u.Name).FirstOrDefault(),
                                Date = income.Date,
                                Amount = income.Amount,
                                IncomeIsPaid = income.IsPaid,
                                Type = "IN",
                                AppointmentId = income.AppointmentId
                            };
                            listFinanceVM.Add(obj);
                        }
                    }
                    else if (isPaid == "NOT")
                    {
                        foreach (var income in _db.Incomes.Where(i => i.DoctorId == doctor.Id && i.Amount > 0 && !i.IsPaid))// not paid
                        {
                            ListFinanceVM obj = new()
                            {
                                Description = _db.Users.Where(u => u.Id == income.DoctorId).Select(u => u.Name).FirstOrDefault(),
                                Date = income.Date,
                                Amount = income.Amount,
                                IncomeIsPaid = income.IsPaid,
                                Type = "IN",
                                AppointmentId = income.AppointmentId
                            };
                            listFinanceVM.Add(obj);
                        }
                    }
                    else
                    {
                        foreach (var income in _db.Incomes.Where(i => i.DoctorId == doctor.Id && i.Amount > 0))
                        {
                            ListFinanceVM obj = new()
                            {
                                Description = _db.Users.Where(u => u.Id == income.DoctorId).Select(u => u.Name).FirstOrDefault(),
                                Date = income.Date,
                                Amount = income.Amount,
                                IncomeIsPaid = income.IsPaid,
                                Type = "IN",
                                AppointmentId = income.AppointmentId
                            };
                            listFinanceVM.Add(obj);
                        }
                    }

                }
                else
                {
                    foreach (var expense in _db.Expenses.Where(e => e.ExpenseDescription.Contains(description)))
                    {
                        ListFinanceVM obj = new()
                        {
                            Description = expense.ExpenseDescription,
                            Date = expense.ExpenseDate,
                            Amount = -(expense.ExpenseAmount),
                            IncomeIsPaid = true,
                            Type = "OUT",
                            ExpenseId = expense.Id
                        };
                        listFinanceVM.Add(obj);
                    }
                }

            }

            // look just for dates
            else if (startDate.ToShortDateString() != "01/01/0001" && description == null && inOut == null) // Check for start date and description empty
            {

                foreach (var income in _db.Incomes.Where(i => i.Date >= startDate && i.Date <= endDate && i.Amount > 0))
                {
                    ListFinanceVM obj = new()
                    {
                        //Find Doctor Name
                        Description = _db.Users.Where(u => u.Id == income.DoctorId).Select(u => u.Name).FirstOrDefault(),
                        Date = income.Date,
                        Amount = income.Amount,
                        IncomeIsPaid = income.IsPaid,
                        Type = "IN",
                        AppointmentId = income.AppointmentId
                    };
                    listFinanceVM.Add(obj);
                }

                foreach (var expense in _db.Expenses.Where(e => e.ExpenseDate >= startDate && e.ExpenseDate <= endDate))
                {
                    ListFinanceVM obj = new()
                    {
                        Description = expense.ExpenseDescription,
                        Date = expense.ExpenseDate,
                        Amount = -(expense.ExpenseAmount),
                        IncomeIsPaid = true,
                        Type = "OUT",
                        ExpenseId = expense.Id
                    };
                    listFinanceVM.Add(obj);
                }

            }

            //Look for doctor/description and dates
            else if (startDate.ToShortDateString() != "01/01/0001" && description != null && inOut == null)
            {
                // Check if the description is a doctor
                ApplicationUser doctor = _db.Users.FirstOrDefault(u => u.Name.Contains(description));

                if (doctor != null)
                {
                    foreach (var income in _db.Incomes.Where(i => i.DoctorId == doctor.Id && i.Date >= startDate
                    && i.Date <= endDate && i.Amount > 0))
                    {
                        ListFinanceVM obj = new()
                        {
                            Description = _db.Users.Where(u => u.Id == income.DoctorId).Select(u => u.Name).FirstOrDefault(),
                            Date = income.Date,
                            Amount = income.Amount,
                            IncomeIsPaid = income.IsPaid,
                            Type = "IN",
                            AppointmentId = income.AppointmentId
                        };
                        listFinanceVM.Add(obj);
                    }
                }
                else
                {
                    foreach (var expense in _db.Expenses.Where(e => e.ExpenseDescription.Contains(description)))
                    {
                        ListFinanceVM obj = new()
                        {
                            Description = expense.ExpenseDescription,
                            Date = expense.ExpenseDate,
                            Amount = -(expense.ExpenseAmount),
                            IncomeIsPaid = true,
                            Type = "OUT",
                            ExpenseId = expense.Id
                        };
                        listFinanceVM.Add(obj);
                    }
                }
            }

            // Look for IN/OUT with dates
            else if (startDate.ToShortDateString() != "01/01/0001" && inOut != null)
            {
                if (inOut == "IN")
                {
                    foreach (var income in _db.Incomes.Where(i => i.Date >= startDate && i.Date <= endDate && i.Amount > 0))
                    {
                        ListFinanceVM obj = new()
                        {
                            Description = _db.Users.Where(u => u.Id == income.DoctorId).Select(u => u.Name).FirstOrDefault(),
                            Date = income.Date,
                            Amount = income.Amount,
                            IncomeIsPaid = income.IsPaid,
                            Type = "IN",
                            AppointmentId = income.AppointmentId
                        };
                        listFinanceVM.Add(obj);
                    }
                }
                else if (inOut == "OUT")
                {
                    foreach (var expense in _db.Expenses.Where(e => e.ExpenseDate >= startDate && e.ExpenseDate <= endDate))
                    {
                        ListFinanceVM obj = new()
                        {
                            Description = expense.ExpenseDescription,
                            Date = expense.ExpenseDate,
                            Amount = -(expense.ExpenseAmount),
                            IncomeIsPaid = true,
                            Type = "OUT",
                            ExpenseId = expense.Id
                        };
                        listFinanceVM.Add(obj);
                    }
                }

            }

            // Return the whole list in case all variables are empty
            else
            {
                return GetFinanceList();
            }

            //Order list by Dates Descending
            var orderedList = listFinanceVM.OrderByDescending(d => d.Date).ToList();

            return orderedList;

        }


    }
}
