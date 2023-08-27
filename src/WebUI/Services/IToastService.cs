using Code_Judge.WebUi.Dto;

namespace Code_Judge.WebUi.Services;

public interface IToastService
{
    public event Action<ToastProps> OnShow;
     public void Info(string message);
     public void Success(string message);
     public void Warning(string message);
     public void Error(string message);
}