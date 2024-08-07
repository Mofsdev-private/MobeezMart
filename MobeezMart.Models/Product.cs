using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobeezMart.Models;

public class Product
{
    public int Id { get; set; }
    
    [Required]
    public string DeviceName { get; set; }
    public string Description { get; set; }
   
    [Required]
    public string IMEI { get; set; }
   
    [Required]
    [Range(1,1000)]
    [Display(Name = "List Price")]
    public double ListPrice { get; set; }
    
    [Required]
    [Range(1, 1000)]
    [Display(Name = "Price for 50-100")]
    public double Price50 { get; set; }

    [Required]
    [Display(Name = "Price for 100+")]
    [Range(1, 1000)]
    public double Price100 { get; set; }
    [ValidateNever]
    public string ImageUrl { get; set; }
    [Required]
    [Display(Name = "Brand")]
    public int BrandId { get; set; }
    [ForeignKey ("BrandId")]
    [ValidateNever]
    public Brand Brand { get; set; }

    [Required]
    [Display(Name ="Condition")]
    public int ConditionId { get; set; }
    [ForeignKey("ConditionId")]
    [ValidateNever]
    public Condition Condition { get; set; }


}
