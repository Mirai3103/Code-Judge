using Code_Judge.WebUi.Dto;

namespace Code_Judge.WebUi.Services;

public class ToastService:IToastService
{
    public event Action<ToastProps> OnShow;
    public void Info(string message)
    {
        OnShow?.Invoke(new ToastProps
        {
            Level = ToastLevel.Info,
            Message = message
        });
    }

    public void Success(string message)
    {
        OnShow?.Invoke(new ToastProps
        {
            Level = ToastLevel.Success,
            Message = message
        });
    }

    public void Warning(string message)
    {
        OnShow?.Invoke(new ToastProps
        {
            Level = ToastLevel.Warning,
            Message = message
        });
    }

    public void Error(string message)
    {
        OnShow?.Invoke(new ToastProps
        {
            Level = ToastLevel.Error,
            Message = message
        });
    }
}