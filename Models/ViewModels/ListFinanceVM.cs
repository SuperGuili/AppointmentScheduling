using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AppointmentScheduling.Models.ViewModels
{
    public class ListFinanceVM
    {
        public string Description { get; set; }

        public DateTime Date { get; set; }

        [DataType(dataType: DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal Amount { get; set; }

        public bool IncomeIsPaid { get; set; }

        public string Type { get; set; }

        public int AppointmentId { get; set; }

        public int ExpenseId { get; set; }

    }
}
