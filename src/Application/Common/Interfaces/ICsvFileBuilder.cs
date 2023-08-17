using Code_Judge.Application.TodoLists.Queries.ExportTodos;

namespace Code_Judge.Application.Common.Interfaces;

public interface ICsvFileBuilder
{
    byte[] BuildTodoItemsFile(IEnumerable<TodoItemRecord> records);
}
