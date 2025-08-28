using System.ComponentModel.DataAnnotations;

namespace ExpiryApp.Models
{
    public class Device
    {
        [Key]
        public int DeviceId { get; set; }

        // 資産管理番号 → 任意入力（NULL可）に
        [Display(Name = "ID")]
        public string? AssetTag { get; set; }

        [Display(Name = "名称")]
        [Required(ErrorMessage = "名称は必須です。")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "設置場所")]
        public string? Location { get; set; }

        [Display(Name = "所管部門")]
        public string? OwnerDept { get; set; }
    }
}
