using System.Globalization;
using Code_Judge.Application.TodoLists.Queries.ExportTodos;
using CsvHelper.Configuration;

namespace Code_Judge.Infrastructure.Files.Maps;

public class TodoItemRecordMap : ClassMap<TodoItemRecord>
{
    public TodoItemRecordMap()
    {
        AutoMap(CultureInfo.InvariantCulture);

        Map(m => m.Done).Convert(c => c.Value.Done ? "Yes" : "No");
    }
}
