using Code_Judge.Application.Common.Interfaces;

namespace Code_Judge.Infrastructure.Services;

public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}
