namespace Code_Judge.Domain.Enums;

public enum DifficultyLevel
{
    Easy = 0,
    Medium = 1,
    Hard = 2
}
public static class DifficultyLevelExtensions
{
    public static string ToFriendlyString(this DifficultyLevel me)
    {
        return me switch
        {
            DifficultyLevel.Easy => "Dễ",
            DifficultyLevel.Medium => "Trung bình",
            DifficultyLevel.Hard => "Khó",
            _ => throw new ArgumentOutOfRangeException(nameof(me), me, null)
        };
    }
}