using AutoMapper;
using C_Part1;
using Microsoft.AspNetCore.Mvc;
using System;
using TaskServiceApi.DTOs;
using static C_Part1.TaskService;

namespace TaskServiceApi.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class TasksController : ControllerBase
    {
        private readonly TaskService _service;
        private readonly IMapper _mapper;

        public TasksController(TaskService taskService, IMapper mapper)
        {
            _service = taskService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskReadDto>>> GetAll()
        {
            var items = await _service.GetAllTasksAsync();
            var dto = _mapper.Map<IEnumerable<TaskReadDto>>(items);
            return Ok(dto);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TaskReadDto>> GetById(int id)
        {
            var item = await _service.GetTaskByIdAsync(id); 
            if (item is null) return NotFound();
            return Ok(_mapper.Map<TaskReadDto>(item));
        }

        [HttpPost]
        public async Task<ActionResult<TaskReadDto>> Create([FromBody] TaskCreateDto dto)
        {
            var createdTask = await _service.CreateAndAddTaskAsync(dto.Title, dto.Description, dto.DueDate);
            var read = _mapper.Map<TaskReadDto>(createdTask);
            return CreatedAtAction(nameof(GetById), new { id = read.Id }, read);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] TaskUpdateDto dto)
        {
            var ok = await _service.UpdateTaskAsync(id, dto.Title, dto.Description, dto.DueDate, dto.IsCompleted);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _service.DeleteTaskAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpPost("{id:int}/complete")]
        public async Task<IActionResult> Complete(int id)
        {
            var ok = await _service.CompleteTaskAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpGet("today")]
        public async Task<ActionResult<IEnumerable<TaskReadDto>>> GetToday()
        {
            var items = await _service.GetTodayTasksAsync();
            return Ok(_mapper.Map<IEnumerable<TaskReadDto>>(items));
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<TaskReadDto>>> Search([FromQuery] string q)
        {
            var items = await _service.FindTaskByNameAsync(q);
            return Ok(_mapper.Map<IEnumerable<TaskReadDto>>(items));
        }

        [HttpGet("completed")]
        public async Task<ActionResult<IEnumerable<TaskReadDto>>> Completed()
        {
            var items = await _service.GetCompletedTasksAsync();
            return Ok(_mapper.Map<IEnumerable<TaskReadDto>>(items));
        }

        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<TaskReadDto>>> Pending()
        {
            var items = await _service.GetPendingTasksAsync();
            return Ok(_mapper.Map<IEnumerable<TaskReadDto>>(items));
        }

        [HttpGet("sorted")]
        public async Task<ActionResult<IEnumerable<TaskReadDto>>> Sorted([FromQuery] string? sortBy = "date", [FromQuery] bool? asc = true)
        {
            if (!Enum.TryParse<TaskService.SortBy>(sortBy, true, out var sortByEnum))
                return BadRequest($"Некорректный параметр сортировки. Допустимые значения: {string.Join(", ", Enum.GetNames(typeof(TaskService.SortBy)))}");

            var items = await _service.GetSortedTasksAsync(sortByEnum, asc ?? true);
            var dto = _mapper.Map<IEnumerable<TaskReadDto>>(items);
            return Ok(dto);
        }

    }
}


