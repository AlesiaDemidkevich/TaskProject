namespace TaskServiceApi.Decorators
{
    using C_Part1;
    using global::TaskServiceApi.Repositories;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using static C_Part1.TaskService;

    namespace TaskServiceApi.Decorators
    {
        public class LoggingTaskRepository : ITaskRepository
        {
            private readonly ITaskRepository _innerRepository;
            private readonly ILogger<LoggingTaskRepository> _logger;

            public event Action<TaskItem>? TaskAdded;
            public event Action<int>? TaskDeleted;
            public event Action<int>? TaskUpdated;

            public LoggingTaskRepository(ITaskRepository innerRepository, ILogger<LoggingTaskRepository> logger)
            {
                _innerRepository = innerRepository;
                _logger = logger;

                // Подписываемся на события внутреннего репозитория
                _innerRepository.TaskAdded += task =>
                {
                    _logger.LogInformation($"[Событие] Задача '{task.Title}' добавлена.");
                    TaskAdded?.Invoke(task);
                };

                _innerRepository.TaskDeleted += id =>
                {
                    _logger.LogInformation($"[Событие] Задача с ID {id} удалена.");
                    TaskDeleted?.Invoke(id);
                };

                _innerRepository.TaskUpdated += id =>
                {
                    _logger.LogInformation($"[Событие] Задача с ID {id} обновлена.");
                    TaskUpdated?.Invoke(id);
                };
            }

            public Task<TaskItem> AddAsync(TaskItem task)
            {
                _logger.LogInformation($"Добавление задачи '{task.Title}'...");
                return _innerRepository.AddAsync(task);
            }

            public Task<bool> DeleteAsync(int id)
            {
                _logger.LogInformation($"Удаление задачи с ID {id}...");
                return _innerRepository.DeleteAsync(id);
            }

            public Task<IReadOnlyList<TaskItem>> GetAllAsync()
            {
                _logger.LogInformation("Получение всех задач...");
                return _innerRepository.GetAllAsync();
            }

            public Task<TaskItem?> GetByIdAsync(int id)
            {
                _logger.LogInformation($"Получение задачи по ID {id}...");
                return _innerRepository.GetByIdAsync(id);
            }

            public Task<List<TaskItem>> FindByNameAsync(string name)
            {
                _logger.LogInformation($"Поиск задач по имени '{name}'...");
                return _innerRepository.FindByNameAsync(name);
            }

            public Task<List<TaskItem>> GetCompletedTasksAsync()
            {
                _logger.LogInformation("Получение выполненных задач...");
                return _innerRepository.GetCompletedTasksAsync();
            }

            public Task<List<TaskItem>> GetPendingTasksAsync()
            {
                _logger.LogInformation("Получение невыполненных задач...");
                return _innerRepository.GetPendingTasksAsync();
            }

            public Task<List<TaskItem>> GetTodayTasksAsync()
            {
                _logger.LogInformation("Получение задач с дедлайном сегодня...");
                return _innerRepository.GetTodayTasksAsync();
            }

            public Task<List<TaskItem>> GetSortedTasksAsync(SortBy sortBy, bool ascending)
            {
                _logger.LogInformation($"Сортировка задач по {sortBy}, {(ascending ? "по возрастанию" : "по убыванию")}...");
                return _innerRepository.GetSortedTasksAsync(sortBy, ascending);
            }

            public Task<bool> UpdateAsync(TaskItem task)
            {
                _logger.LogInformation($"Обновление задачи '{task.Title}'...");
                return _innerRepository.UpdateAsync(task);
            }
        }
    }

}
