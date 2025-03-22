using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DisasterPrediction.Api.Data;

public class SendAlertRequest
{
    [Phone]
    [Required]
    [DefaultValue("+66926856469")]
    public string PhoneNumber { get; set; }

    [EmailAddress]
    [DefaultValue("kittakornkraikruan@gmail.com")]
    public string Email { get; set; }
}
