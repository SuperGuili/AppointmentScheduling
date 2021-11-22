using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AppointmentScheduling.Models
{
    public class ExpenseType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ExpenseTypeDescription { get; set; }
    }
}
