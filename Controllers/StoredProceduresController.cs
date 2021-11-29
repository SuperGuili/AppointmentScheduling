using AppointmentScheduling.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AppointmentScheduling.Models.StoredProcedures;
using Microsoft.Data.SqlClient;

namespace AppointmentScheduling.Controllers
{
    public class StoredProceduresController : Controller
    {
        private readonly ApplicationDbContext _db;

        public StoredProceduresController(ApplicationDbContext applicationDbContext)
        {
            _db = applicationDbContext;
        }
        public IActionResult Index()
        {
            string query = "EXEC spSelectAllUsers";
            List<SP_SelectAllUsers> list = _db.SP_SelectAllUsers.FromSqlRaw<SP_SelectAllUsers>(query).ToList();

            return View(list);
        }

        [HttpPost]
        public IActionResult SP_UpdatePhone(string userId, string phone)
        {
            string query = "EXEC spUpdateUserPhone @userId, @phone";

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter {ParameterName="@userId", Value=userId},
                new SqlParameter {ParameterName="@phone", Value=phone}            
            };

            var rowsAffected = _db.Database.ExecuteSqlRaw(query, parameters.ToArray());

            return RedirectToAction("Index");
        }
        
    }
}
