namespace Code_Judge.WebUi.Dto;


public class ToastProps
{
    public ToastLevel Level { get; set; }
    public string Message { get; set; } = null!;
    public Guid Id { get; set; } = Guid.NewGuid();
    public int Duration { get; set; } = 5000;
}