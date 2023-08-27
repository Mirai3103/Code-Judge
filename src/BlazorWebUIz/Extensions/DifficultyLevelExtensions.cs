using Code_Judge.Domain.Enums;

namespace BlazorWebUI.Extensions;

public static class DifficultyLevelExtensions
{
     public static string GetComponentClassName(this DifficultyLevel difficultyLevel)
     {
         return difficultyLevel switch
         {
             DifficultyLevel.Easy => "bg-green-100 text-green-800 text-xs font-medium mr-2 px-2.5 py-0.5 rounded dark:bg-green-900 dark:text-green-300",
             DifficultyLevel.Medium => "bg-yellow-100 text-yellow-800 text-xs font-medium mr-2 px-2.5 py-0.5 rounded dark:bg-yellow-900 dark:text-yellow-300",
             DifficultyLevel.Hard => "bg-red-100 text-red-800 text-xs font-medium mr-2 px-2.5 py-0.5 rounded dark:bg-red-900 dark:text-red-300",
             _ => throw new ArgumentOutOfRangeException(nameof(difficultyLevel), difficultyLevel, null)
         };
     }
}