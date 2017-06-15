using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace OpenRacingTelemetry.ViewModels.Authorization
{
    public class LogoutViewModel
    {
        [BindNever]
        public string RequestId { get; set; }
    }
}