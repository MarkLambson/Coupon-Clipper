#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;

namespace BeltExam.Models;
public class Association
{
    [Key]
    public int AssociationId { get; set; }
    public int? UserId { get; set; }
    public User? User { get; set; }
    public int CouponId { get; set; }
    public Coupon? Coupon { get; set; }
}