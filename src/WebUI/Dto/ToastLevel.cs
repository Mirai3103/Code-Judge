namespace Code_Judge.WebUi.Dto;

public enum ToastLevel
{
    Info,
    Success,
    Warning,
    Error
}

public static class ToastLevelExtensions
{
    public static string ToCssClass(this ToastLevel level) => level switch
    {
        ToastLevel.Info => "alert-info",
        ToastLevel.Success => "alert-success",
        ToastLevel.Warning => "alert-warning",
        ToastLevel.Error => "alert-error",
        _ => throw new NotImplementedException(),
    };
}