using System.ComponentModel.DataAnnotations;

namespace ExpiryApp.Models;

public class Device
{
    public int DeviceId { get; set; }

    [Required(ErrorMessage = "資産番号は必須です。")]
    [StringLength(50, ErrorMessage = "資産番号は{1}文字以内で入力してください。")]
    [Display(Name = "資産番号")]
    public string AssetTag { get; set; } = "";

    [Required(ErrorMessage = "機器名は必須です。")]
    [StringLength(100, ErrorMessage = "機器名は{1}文字以内で入力してください。")]
    [Display(Name = "機器名")]
    public string Name { get; set; } = "";

    [Display(Name = "設置場所")]
    public string? Location { get; set; }

    [Display(Name = "部署")]
    public string? OwnerDept { get; set; }

    public List<ExpiryTrack> ExpiryTracks { get; set; } = new();
}
