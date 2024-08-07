using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobeezMart.Models
{
    public class Condition
    {
        [Key]
        public int Id { get; set; }
        [Display(Name= "Condition")]
        [Required]
        [MaxLength(55)]
        public string Name { get; set; }
    }
}
