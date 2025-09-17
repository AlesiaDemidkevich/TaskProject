public class Program
{
    static void Main(string[] args)
    {
        var listener = new TaskNotificationListener("localhost");
        Console.ReadLine(); // Чтобы приложение не завершилось
    }
}