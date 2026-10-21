using System;
using Raylib_cs;

namespace TerrariaClone.Core;

/// <summary>
/// Главный менеджер игры. Управляет жизненным циклом, кадрами и передаёт
/// управление подсистемам (World, Player, Systems), которые мы добавим позже.
/// </summary>
public sealed class Game : IDisposable
{
    // ---------- Состояние ----------
    private bool  _initialized;
    private bool  _disposed;
    private float _totalTime;   // общее время с момента запуска (сек)
    private float _fpsTimer;    // аккумулятор для сглаженного FPS
    private int   _fpsCounter;
    private int   _displayFps;

    // ---------- Публичный API ----------

    /// <summary>Запуск главного цикла игры. Блокирует поток до закрытия окна.</summary>
    public void Run()
    {
        Init();

        while (!Raylib.WindowShouldClose() && _initialized)
        {
            Update();
            Draw();
        }

        Shutdown();
    }

    // ---------- Init / Update / Draw / Shutdown ----------

    private void Init()
    {
        Raylib.SetConfigFlags(ConfigFlags.FLAG_VSYNC_HINT | ConfigFlags.FLAG_MSAA_4X_HINT);
        Raylib.InitWindow(Settings.ScreenWidth, Settings.ScreenHeight, Settings.GameTitle);
        Raylib.SetTargetFPS(Settings.TargetFps);

        // В Terraria ESC не закрывает игру, а открывает меню.
        // Отключаем "выход по ESC", чтобы игрок случайно не вылетал.
        Raylib.SetExitKey(KeyboardKey.KEY_NULL);

        _initialized = true;
    }

    private void Update()
    {
        float dt = Raylib.GetFrameTime();
        _totalTime += dt;

        // Сглаженный FPS (обновляется раз в 0.25 сек)
        _fpsTimer += dt;
        _fpsCounter++;
        if (_fpsTimer >= 0.25f)
        {
            _displayFps = (int)MathF.Round(_fpsCounter / _fpsTimer);
            _fpsTimer   = 0f;
            _fpsCounter = 0;
        }

        // TODO (следующие шаги): Input, World.Update, Player.Update, Camera.Update
    }

    private void Draw()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Settings.SkyColor);

        DrawDebugGrid();
        DrawDebugPlayer();

        Raylib.EndDrawing();
    }

    private void Shutdown()
    {
        if (!_initialized) return;

        Raylib.CloseWindow();
        _initialized = false;
    }

    // ---------- Отладочный рендер (временный, до появления World) ----------

    /// <summary>
    /// Рисует сетку 16×16 px. Помогает сразу проверить масштаб и производительность.
    /// В Terraria такая сетка скрыта, но при отладке мира — незаменима.
    /// </summary>
    private void DrawDebugGrid()
    {
        int ts = Settings.TileSize;

        for (int x = 0; x <= Settings.ScreenWidth; x += ts)
            Raylib.DrawLine(x, 0, x, Settings.ScreenHeight, Settings.GridColor);

        for (int y = 0; y <= Settings.ScreenHeight; y += ts)
            Raylib.DrawLine(0, y, Settings.ScreenWidth, y, Settings.GridColor);
    }

    /// <summary>
    /// Временная «болванка» игрока — прямоугольники в стиле Terraria:
    /// тело 20×30, голова 14×14, ноги 20×14. Итоговый bbox ~20×44.
    /// </summary>
    private void DrawDebugPlayer()
    {
        const int playerW = 20;
        const int playerH = 44;
        int px = Settings.ScreenWidth  / 2 - playerW / 2;
        int py = Settings.ScreenHeight / 2 - playerH / 2;

        // Голова
        Raylib.DrawRectangle(px + 3, py, 14, 14, Settings.PlayerBodyColor);
        // Рубашка (тело)
        Raylib.DrawRectangle(px, py + 14, playerW, 18, Settings.PlayerShirtColor);
        // Ноги
        Raylib.DrawRectangle(px, py + 32, playerW, 12, Settings.PlayerPantsColor);

        // Отладочная подпись
        Raylib.DrawText($"Player bbox: {playerW}x{playerH}px", px + playerW + 10, py + 4,
                        12, Settings.HudTextColor);
    }

    // ---------- IDisposable ----------

    public void Dispose()
    {
        if (_disposed) return;
        Shutdown();
        _disposed = true;
    }
}