using C_Part1;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Drawing;
using System.Text.Json;
using System.Threading.Tasks;
using TaskServiceApi.Db;
using TaskServiceApi.Storage;
using static C_Part1.TaskService;

namespace TaskServiceApi.Repositories
{   
    public class TaskRepository: ITaskRepository
    {     
        private readonly AppDbContext _context;

        public event Action<TaskItem>? TaskAdded;
        public event Action<int>? TaskDeleted;
        public event Action<int>? TaskUpdated;

        private readonly ILogger<TaskRepository> _logger;

        public TaskRepository(AppDbContext context, ILogger<TaskRepository> logger) {
           _context = context;
           _logger = logger;
        }

        // Получить все задачи
        public async Task<IReadOnlyList<TaskItem>> GetAllAsync() {
            
            return await _context.Tasks.AsNoTracking().ToListAsync();
        }

        // Получить задачу по Id
        public async Task<TaskItem?> GetByIdAsync(int id)
        {
            return await _context.Tasks.FindAsync(id);
        }

        public async Task<TaskItem> AddAsync(TaskItem task)
        {
            try
            {
                _context.Tasks.Add(task);
                await _context.SaveChangesAsync();

                TaskAdded?.Invoke(task);
                _logger.LogInformation("Задача добавлена: {Title}, ID: {Id}", task.Title, task.Id);

                return task;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при добавлении задачи: {Title}", task.Title);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var task = await _context.Tasks.FindAsync(id);
                if (task == null)
                {
                    _logger.LogWarning("Попытка удалить несуществующую задачу с ID {Id}", id);
                    return false;
                }

                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();

                TaskDeleted?.Invoke(id);
                _logger.LogInformation("Задача с ID {Id} удалена", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении задачи с ID {Id}", id);
                throw;
            }
        }

        public async Task<bool> UpdateAsync(TaskItem task)
        {
            try
            {
                var existingTask = await _context.Tasks.FindAsync(task.Id);
                if (existingTask == null)
                {
                    _logger.LogWarning("Попытка обновить несуществующую задачу с ID {Id}", task.Id);
                    return false;
                }

                existingTask.Title = task.Title;
                existingTask.Description = task.Description;
                existingTask.DueDate = task.DueDate;
                existingTask.IsCompleted = task.IsCompleted;

                await _context.SaveChangesAsync();

                TaskUpdated?.Invoke(task.Id);
                _logger.LogInformation("Задача с ID {Id} обновлена", task.Id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении задачи с ID {Id}", task.Id);
                throw;
            }
        }


        public async Task<List<TaskItem>> FindByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return new List<TaskItem>();

            var trimmedName = name.Trim().ToLower();
            return await _context.Tasks
                .Where(t => t.Title.ToLower().Contains(trimmedName))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<TaskItem>> GetCompletedTasksAsync()
        {
            return await _context.Tasks
                .Where(t => t.IsCompleted)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<TaskItem>> GetPendingTasksAsync()
        {
            return await _context.Tasks
                .Where(t => !t.IsCompleted)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<TaskItem>> GetTodayTasksAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            return await _context.Tasks
                .Where(t => t.DueDate == today)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<TaskItem>> GetSortedTasksAsync(SortBy sortBy, bool ascending)
        {
            IQueryable<TaskItem> query = _context.Tasks.AsNoTracking();

            switch (sortBy)
            {
                case SortBy.Title:
                    query = ascending
                        ? query.OrderBy(t => t.Title.ToLower()).ThenBy(t => t.Id)
                        : query.OrderByDescending(t => t.Title.ToLower()).ThenByDescending(t => t.Id);
                    break;

                case SortBy.Status:
                    query = ascending
                        ? query.OrderBy(t => t.IsCompleted).ThenBy(t => t.Id)
                        : query.OrderByDescending(t => t.IsCompleted).ThenByDescending(t => t.Id);
                    break;

                case SortBy.Date:
                default:
                    query = ascending
                        ? query.OrderBy(t => t.DueDate).ThenBy(t => t.Id)
                        : query.OrderByDescending(t => t.DueDate).ThenByDescending(t => t.Id);
                    break;
            }

            return await query.ToListAsync();
        }

    }
}
