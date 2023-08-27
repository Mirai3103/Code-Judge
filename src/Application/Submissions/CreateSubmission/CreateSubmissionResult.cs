using Code_Judge.Application.Common.Models;
using Code_Judge.Domain.Entities;

namespace Code_Judge.Application.Submissions.CreateSubmission;

public class CreateSubmissionResult
{
   public IEnumerable<ExecuteCodeResult> ExecuteCodeResults { get; set; } = new List<ExecuteCodeResult>();
    public Submission Submission { get; set; } = null!;
}