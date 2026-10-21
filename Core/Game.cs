using System;
using System.Numerics;
using Raylib_cs;
using TerrariaClone.Entities;
using TerrariaClone.World;

namespace TerrariaClone.Core;

public sealed class Game : IDisposable
{
    private bool  _initialized;
    private bool  _disposed;
    private float _fpsTimer;
    private int   _fpsCounter;
    private int   _displayFps;

    private World.World _world = null!;
    private Player _player = null!;
    private Camera2D _camera;

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

    private void Init()
    {
        Raylib.SetConfigFlags(ConfigFlags.FLAG_VSYNC_HINT | ConfigFlags.FLAG_MSAA_4X_HINT);
        Raylib.InitWindow(Settings.ScreenWidth, Settings.ScreenHeight, Settings.GameTitle);
        Raylib.SetTargetFPS(Settings.TargetFps);
        Raylib.SetExitKey(KeyboardKey.KEY_NULL);

        _world = new World.World(800, 300);

        // Спавним игрока над центром мира
        float spawnX = _world.Width * Settings.TileSize / 2f;
        float spawnY = _world.Height * Settings.TileSize / 3f - 100f; // Чуть выше земли
        _player = new Player(spawnX, spawnY);

        // Камера нацелена на центр игрока
        _camera = new Camera2D
        {
            Target = new Vector2(_player.Position.X + Player.Width / 2f, _player.Position.Y + Player.Height / 2f),
            Offset = new Vector2(Settings.ScreenWidth / 2f, Settings.ScreenHeight / 2f),
            Zoom = 1.5f,
            Rotation = 0f
        };

        _initialized = true;
    }

    private void Update()
    {
        float dt = Raylib.GetFrameTime();
        
        // Обновляем FPS
        _fpsTimer += dt;
        _fpsCounter++;
        if (_fpsTimer >= 0.25f)
        {
            _displayFps = (int)MathF.Round(_fpsCounter / _fpsTimer);
            _fpsTimer   = 0f;
            _fpsCounter = 0;
        }

        // Обновляем физику игрока
        _player.Update(dt, _world);

        // Плавное следование камеры за игроком (Lerp)
        Vector2 playerCenter = new Vector2(_player.Position.X + Player.Width / 2f, _player.Position.Y + Player.Height / 2f);
        _camera.Target = Vector2.Lerp(_camera.Target, playerCenter, 10f * dt);
    }

    private void Draw()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Settings.SkyColor);

        Raylib.BeginMode2D(_camera);

        _world.Draw(_camera);
        _player.Draw();

        Raylib.EndMode2D();

        DrawHud();

        Raylib.EndDrawing();
    }

    private void DrawHud()
    {
        Raylib.DrawRectangle(10, 10, 220, 30, Settings.HudBgColor);
        Raylib.DrawText($"FPS: {_displayFps}", 20, 18, 14, Settings.HudTextColor);
        
        Raylib.DrawRectangle(10, 45, 280, 30, Settings.HudBgColor);
        Raylib.DrawText($"Pos: X:{_player.Position.X:F0} Y:{_player.Position.Y:F0}", 20, 53, 14, Settings.HudTextColor);
        
        Raylib.DrawText("A/D - Move | SPACE/W - Jump", 10, Settings.ScreenHeight - 30, 16, new Color(200, 200, 200, 255));
    }

    private void Shutdown()
    {
        if (!_initialized) return;
        Raylib.CloseWindow();
        _initialized = false;
    }

    public void Dispose()
    {
        if (_disposed) return;
        Shutdown();
        _disposed = true;
    }
}