using System.Text;
using System.Text.RegularExpressions;

namespace Code_Judge.Application.Common.Slug;

public static partial class SlugExtensions
{
    public static string Slugify(this string text)
    {
        var regex = MyRegex();
        string temp =text.Normalize(NormalizationForm.FormD);
        return regex.Replace(temp, string.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D')
            .Replace(' ', '-').ToLower();
    }

    [GeneratedRegex("\\p{IsCombiningDiacriticalMarks}+")]
    private static partial Regex MyRegex();
}