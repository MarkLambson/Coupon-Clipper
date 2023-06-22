#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeltExam.Models;

public class Coupon
{
    [Key]
    public int CouponId { get; set; }

    [Required(ErrorMessage = "is required")]
    [Display(Name = "Coupon Code")]
    public string CouponCode { get; set; }

    [Required(ErrorMessage = "is required")]
    [Display(Name = "Website applicable on")]
    public string Website { get; set; }

    [Required(ErrorMessage = "is required")]
    [MinLength(10, ErrorMessage = "must be at least 10 characters.")]
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;


    public int UserId { get; set; }
    public User? User { get; set; }
    public List<Association> AllAssociations { get; set; } = new List<Association>();
}