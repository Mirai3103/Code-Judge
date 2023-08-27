namespace Code_Judge.Domain.Enums;

public enum ProgramingLanguage { C, Cpp, Java, Python, CSharp, JavaScript }
public static class ProgramingLanguageExtensions
{
    public static string ToFriendlyString(this ProgramingLanguage language)
    {
        return language switch
        {
            ProgramingLanguage.C => "C",
            ProgramingLanguage.Cpp => "C++",
            ProgramingLanguage.Java => "Java",
            ProgramingLanguage.Python => "Python",
            ProgramingLanguage.CSharp => "C#",
            ProgramingLanguage.JavaScript => "JavaScript",
            _ => throw new NotImplementedException(),
        };
    }
    public static string ToMonacoId(this ProgramingLanguage language)
    {
        return language switch
        {
            ProgramingLanguage.C => "c",
            ProgramingLanguage.Cpp => "cpp",
            ProgramingLanguage.Java => "java",
            ProgramingLanguage.Python => "python",
            ProgramingLanguage.CSharp => "csharp",
            ProgramingLanguage.JavaScript => "javascript",
            _ => throw new NotImplementedException(),
        };
    }
}
