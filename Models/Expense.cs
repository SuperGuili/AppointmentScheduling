using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AppointmentScheduling.Models
{
    public class Expense
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ExpenseDescription { get; set; }

        [Required]
        public DateTime ExpenseDate { get; set; }

        [Required]
        [DataType(dataType: DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal ExpenseAmount { get; set; }

        [Required]
        public string ExpenseUserId { get; set; }

        [Required]
        public string ExpenseType { get; set; }
    }
}
