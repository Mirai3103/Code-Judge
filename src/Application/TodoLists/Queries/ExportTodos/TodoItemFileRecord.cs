using Code_Judge.Application.Common.Mappings;
using Code_Judge.Domain.Entities;

namespace Code_Judge.Application.TodoLists.Queries.ExportTodos;

public class TodoItemRecord : IMapFrom<TodoItem>
{
    public string? Title { get; init; }

    public bool Done { get; init; }
}
