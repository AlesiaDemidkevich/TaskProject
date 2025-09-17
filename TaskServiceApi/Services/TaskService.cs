using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskServiceApi.Messaging;
using TaskServiceApi.Repositories;
using static C_Part1.TaskService;

namespace C_Part1
{
    public class TaskService
    {
       
        private ITaskRepository _taskRepository;
        private readonly RabbitMqPublisher _publisher;
        private readonly ILogger<TaskService> _logger;
        public enum SortBy
        {
            Date,
            Title,
            Status
        }

        public TaskService(ITaskRepository taskRepository, ILogger<TaskService> logger, RabbitMqPublisher publisher) {
            _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
            _logger = logger;
            _publisher = publisher;

            _taskRepository.TaskAdded += (task) =>
            {
                _publisher.PublishTaskAdded(task);
            };

            _taskRepository.TaskDeleted += (id) =>
            {
                _publisher.PublishTaskDeleted(id);
            };

            _taskRepository.TaskUpdated += (id) =>
            {
                _publisher.PublishTaskUpdated(id);
            };
            
        }
                    
        public async Task<IReadOnlyList<TaskItem>> GetAllTasksAsync()
        {
            return await _taskRepository.GetAllAsync();
        }

        public async Task<TaskItem?> GetTaskByIdAsync(int Id)
        {
            return await _taskRepository.GetByIdAsync(Id);
        }

        // Поиск задач по названию с учётом регистра (игнорируем регистр)
        public async Task<List<TaskItem>> FindTaskByNameAsync(string? taskName)
        {
            return await _taskRepository.FindByNameAsync(taskName ?? string.Empty);
        }

        public async Task<List<TaskItem>> GetCompletedTasksAsync()
        {
            return await _taskRepository.GetCompletedTasksAsync();
        }

        public async Task<List<TaskItem>> GetPendingTasksAsync()
        {
            return  await _taskRepository.GetPendingTasksAsync();
        }

        public async Task<List<TaskItem>> GetSortedTasksAsync(SortBy type, bool ascending)
        {            
            return await _taskRepository.GetSortedTasksAsync(type, ascending);
        }

        // Возвращает список задач, у которых дедлайн — сегодня
        public async Task< List<TaskItem>> GetTodayTasksAsync()
        {
            return await _taskRepository.GetTodayTasksAsync();
        }

        // Отметить задачу как выполненную
        public async Task<bool> CompleteTaskAsync(int id)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null) return false;

            task.IsCompleted = true;
            return await _taskRepository.UpdateAsync(task);
        }

        //Добавить задачу
        public async Task<TaskItem> CreateAndAddTaskAsync(string? taskName, string? taskDescription, DateOnly taskDueDate) {
            var task = new TaskItem
            {
                Title = taskName ?? string.Empty,
                Description = taskDescription ?? string.Empty,
                DueDate = taskDueDate,
                IsCompleted = false
            };
            return await _taskRepository.AddAsync(task);
        }

        //Удалить задачу
        public async Task<bool> DeleteTaskAsync(int Id) {
            return await _taskRepository.DeleteAsync(Id);
        }
        
        // Обновить задачу
        public async Task<bool> UpdateTaskAsync(int id, string? newTitle, string? newDesc, DateOnly? newDueDate, bool? newIsCompleted)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null) return false;

            task.Title = newTitle ?? task.Title;
            task.Description = newDesc ?? task.Description;
            task.DueDate = newDueDate ?? task.DueDate;
            task.IsCompleted = newIsCompleted ?? task.IsCompleted;
            return await _taskRepository.UpdateAsync(task);
        }

    }
}
