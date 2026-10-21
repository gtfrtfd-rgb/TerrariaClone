using TerrariaClone.Core;

namespace TerrariaClone;

/// <summary>
/// Точка входа. Отвечает только за жизненный цикл игры.
/// </summary>
public static class Program
{
    public static void Main()
    {
        // using гарантирует корректное освобождение ресурсов (CloseWindow)
        using var game = new Game();
        game.Run();
    }
}