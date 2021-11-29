using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AppointmentScheduling.Models.StoredProcedures
{
    public class SP_SelectAllUsers
    {
        [Key]
        public string UserId { get; set; }

        public string UserName { get; set; }

        public string Name { get; set; }

        public string Telephone { get; set; }

        public string RoleId { get; set; }

        public string NormalizedName { get; set; }
    }
}
