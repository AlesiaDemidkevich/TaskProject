using C_Part1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static C_Part1.TaskService;

namespace TaskServiceApi.Repositories
{
    public interface ITaskRepository
    {
        event Action<TaskItem>? TaskAdded;
        event Action<int>? TaskDeleted;
        event Action<int>? TaskUpdated;

        Task<IReadOnlyList<TaskItem>> GetAllAsync();
        Task<TaskItem?> GetByIdAsync(int id);
        Task<TaskItem> AddAsync(TaskItem task);
        Task<bool> UpdateAsync(TaskItem task);
        Task<bool> DeleteAsync(int id);
        Task<List<TaskItem>> GetTodayTasksAsync();
        Task<List<TaskItem>> GetPendingTasksAsync();
        Task<List<TaskItem>> GetSortedTasksAsync(SortBy sortBy, bool ascending);
        Task<List<TaskItem>> GetCompletedTasksAsync();
        Task<List<TaskItem>> FindByNameAsync(string name);
    }
}
