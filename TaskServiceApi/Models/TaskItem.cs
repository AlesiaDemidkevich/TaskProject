using System;
using System.Globalization;

namespace C_Part1
{
    // Класс, описывающий отдельную задачу
    public class TaskItem
    {
        // Уникальный идентификатор задачи
        public int Id { get; set; }
        // Название задачи
        public string Title { get; set; } = string.Empty;
        // Описание задачи
        public string Description { get; set; } = string.Empty;
        // Дата дедлайна задачи (используется DateOnly - дата без времени)
        public DateOnly DueDate { get; set; }
        // Статус выполнения задачи: true - выполнена, false - нет
        public bool IsCompleted { get; set; }

        // Конструктор по умолчанию
        public TaskItem() { }

        public TaskItem(string? taskName, string? taskDescription, DateOnly taskDueDate)
        {
            Title = taskName ?? string.Empty; // Защита от null
            Description = taskDescription ?? string.Empty;
            DueDate = taskDueDate;
            IsCompleted = false; // По умолчанию задача не выполнена
        }

        // Конструктор с параметрами для создания задачи
        public TaskItem(int id, string? taskName, string? taskDescription, DateOnly taskDueDate)
        {
            Id = id;
            Title = taskName ?? string.Empty; // Защита от null
            Description = taskDescription ?? string.Empty;
            DueDate = taskDueDate;
            IsCompleted = false; // По умолчанию задача не выполнена
        }

        // Переопределяем метод ToString для удобного вывода информации о задаче
        public override string ToString()
        {
            // Форматируем дату в "дд.мм.гггг" и выводим статус задачи
            return $"[{Id}] | {Title} | Дедлайн: {DueDate.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture)} | Статус: {(IsCompleted ? "Выполнено" : "Не выполнено")}\nОписание: {Description}\n";
        }
    }
}
