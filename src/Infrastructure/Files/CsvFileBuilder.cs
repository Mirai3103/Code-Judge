using System.Globalization;
using Code_Judge.Application.Common.Interfaces;
using Code_Judge.Application.TodoLists.Queries.ExportTodos;
using Code_Judge.Infrastructure.Files.Maps;
using CsvHelper;

namespace Code_Judge.Infrastructure.Files;

public class CsvFileBuilder : ICsvFileBuilder
{
    public byte[] BuildTodoItemsFile(ICollection<TodoItemRecord> records)
    {
        using var memoryStream = new MemoryStream();
        using (var streamWriter = new StreamWriter(memoryStream))
        {
            using var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);

            csvWriter.Context.RegisterClassMap<TodoItemRecordMap>();
            csvWriter.WriteRecords(records);
        }

        return memoryStream.ToArray();
    }
}
