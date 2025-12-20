using MudBlazor;

namespace LeapDataScienceTool.Common.Services
{
    public class ResponseService
    {
        private readonly ISnackbar snackbar;

        public ResponseService(ISnackbar snackbar)
        {
            this.snackbar = snackbar;
        }

        public void ShowInfoResponse(string message)
        {
            snackbar.Add(message, Severity.Info, GetOptions(Severity.Info), key: Severity.Info.ToString());
        }

        public void ShowErrorResponse(string message)
        {
            snackbar.Add(message, Severity.Error, GetOptions(Severity.Error), key: Severity.Error.ToString());
        }

        public void ShowSuccessfullResponse(string message)
        {
            snackbar.Add(message, Severity.Success, GetOptions(Severity.Success), key: Severity.Success.ToString());
        }

        private Action<SnackbarOptions> GetOptions(Severity severity)
        {
            var config = (SnackbarOptions options) =>
            {
                options.HideIcon = true;
                options.CloseAfterNavigation = true;
                options.VisibleStateDuration = 4000;
                options.ShowTransitionDuration = 100;
            };
            return config;
        }
    }
}
