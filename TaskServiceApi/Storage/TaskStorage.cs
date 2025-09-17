using C_Part1;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TaskServiceApi.Storage
{
    public class TaskStorage : ITaskStorage
    {
        private readonly string _filePath;
        private readonly JsonSerializerOptions _jsonOptions;

        public TaskStorage(IConfiguration config)
        {
            _filePath = config["TaskFilePath"] ?? "tasks.json";
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            _jsonOptions.Converters.Add(new Infrastructure.DateOnlyJsonConverter());
        }

        public void Save(List<TaskItem> tasks)
        {
            var json = JsonSerializer.Serialize(tasks, _jsonOptions);
            File.WriteAllText(_filePath, json);
        }

        public List<TaskItem>? Load()
        {
            if (!File.Exists(_filePath))
                return new List<TaskItem>();

            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<TaskItem>>(json, _jsonOptions) ?? new List<TaskItem>();

        }
    }
}
