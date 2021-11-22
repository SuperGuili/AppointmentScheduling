using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AppointmentScheduling.Models
{
    public class Income
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string DoctorId { get; set; }
        [Required]
        public string PatientId { get; set; }
        [Required]
        public int AppointmentId { get; set; }

        [DataType(dataType: DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal Amount { get; set; }

        public bool IsPaid { get; set; }

        public string UserId { get; set; }

        public DateTime Date { get; set; }
    }
}
