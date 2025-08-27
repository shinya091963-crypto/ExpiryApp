using System.ComponentModel.DataAnnotations;

namespace ExpiryApp.Models;

public class ExpiryTrack : IValidatableObject
{
    [Key]
    public int ExpiryTrackId { get; set; }

    [Required(ErrorMessage = "機器を選択してください。")]
    [Display(Name = "機器")]
    public int DeviceId { get; set; }

    [Required(ErrorMessage = "種別を選択してください。")]
    [StringLength(50)]
    [Display(Name = "種別")]
    public string TrackType { get; set; } = "Calibration";

    [DataType(DataType.Date)]
    [Display(Name = "期限日")]
    public DateTime ExpiryDate { get; set; }

    [Range(0, 3650, ErrorMessage = "リード日は0〜3650の範囲で入力してください。")]
    [Display(Name = "リード日数")]
    public int LeadDays { get; set; } = 30;

    [Display(Name = "メモ")]
    public string? Note { get; set; }

    public Device? Device { get; set; }

    // モデル全体の整合性チェック（過去日禁止 など）
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (ExpiryDate.Date < DateTime.Today)
        {
            yield return new ValidationResult(
                "期限日は今日以降の日付を指定してください。",
                new[] { nameof(ExpiryDate) }
            );
        }

        // TrackType の許容値を固定（任意）
        var allowed = new[] { "Calibration", "Inspection", "Sanitation" };
        if (!allowed.Contains(TrackType))
        {
            yield return new ValidationResult(
                "種別が不正です。",
                new[] { nameof(TrackType) }
            );
        }
    }
}
