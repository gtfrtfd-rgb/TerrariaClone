using System;
using System.Numerics;
using Raylib_cs;
using TerrariaClone.Entities;
using TerrariaClone.Items;
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
    private Hotbar _hotbar = null!;
    private Camera2D _camera;

    private int _targetTileX = -1;
    private int _targetTileY = -1;
    private bool _isTargetValid = false;

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
        _hotbar = new Hotbar();

        int spawnTileX = _world.Width / 2;
        int surfacePixelY = _world.GetSurfaceY(spawnTileX);
        float spawnX = spawnTileX * Settings.TileSize;
        float spawnY = surfacePixelY - Player.Height - Settings.TileSize; 
        
        _player = new Player(spawnX, spawnY);

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
        
        _fpsTimer += dt;
        _fpsCounter++;
        if (_fpsTimer >= 0.25f)
        {
            _displayFps = (int)MathF.Round(_fpsCounter / _fpsTimer);
            _fpsTimer   = 0f;
            _fpsCounter = 0;
        }

        _player.Update(dt, _world);
        _hotbar.Update();

        Vector2 playerCenter = new Vector2(_player.Position.X + Player.Width / 2f, _player.Position.Y + Player.Height / 2f);
        _camera.Target = Vector2.Lerp(_camera.Target, playerCenter, 10f * dt);

        HandleInteraction();
    }

    private void HandleInteraction()
    {
        Vector2 mouseWorldPos = Raylib.GetScreenToWorld2D(Raylib.GetMousePosition(), _camera);
        
        _targetTileX = (int)Math.Floor(mouseWorldPos.X / Settings.TileSize);
        _targetTileY = (int)Math.Floor(mouseWorldPos.Y / Settings.TileSize);

        Vector2 playerCenter = new Vector2(_player.Position.X + Player.Width / 2f, _player.Position.Y + Player.Height / 2f);
        float distance = Vector2.Distance(playerCenter, new Vector2(_targetTileX * Settings.TileSize, _targetTileY * Settings.TileSize));
        
        _isTargetValid = (distance / Settings.TileSize) <= Settings.PlayerReach;

        if (_isTargetValid)
        {
            if (Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT))
            {
                _world.RemoveTile(_targetTileX, _targetTileY);
            }

            if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_RIGHT))
            {
                _world.PlaceTile(_targetTileX, _targetTileY, _hotbar.SelectedTile);
            }
        }
    }

    private void Draw()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Settings.SkyColor);

        Raylib.BeginMode2D(_camera);

        _world.Draw(_camera);
        _player.Draw();

        // Рисуем сетку выделения тайла ТОЛЬКО если это имеет смысл
        if (_targetTileX != -1 && _targetTileY != -1)
        {
            bool isAir = _world.IsTileAir(_targetTileX, _targetTileY);
            bool canPlace = _world.CanPlaceTile(_targetTileX, _targetTileY);

            // Если это воздух и поставить туда блок нельзя (нет соседей) — обводку НЕ рисуем
            if (!isAir || canPlace)
            {
                Color highlightColor = _isTargetValid 
                    ? new Color(255, 255, 255, 100) // Белый (можно копать/ставить)
                    : new Color(255, 0, 0, 100);    // Красный (слишком далеко)

                Raylib.DrawRectangleLines(
                    _targetTileX * Settings.TileSize, 
                    _targetTileY * Settings.TileSize, 
                    Settings.TileSize, 
                    Settings.TileSize, 
                    highlightColor);
            }
        }

        Raylib.EndMode2D();

        DrawHud();
        _hotbar.Draw(_world);

        Raylib.EndDrawing();
    }

    private void DrawHud()
    {
        Raylib.DrawRectangle(10, 10, 220, 30, Settings.HudBgColor);
        Raylib.DrawText($"FPS: {_displayFps}", 20, 18, 14, Settings.HudTextColor);
        
        Raylib.DrawRectangle(10, 45, 280, 30, Settings.HudBgColor);
        Raylib.DrawText($"Pos: X:{_player.Position.X:F0} Y:{_player.Position.Y:F0}", 20, 53, 14, Settings.HudTextColor);
        
        Raylib.DrawText("LMB - Mine | RMB - Place | 1,2,3 - Select", 10, Settings.ScreenHeight - 30, 16, new Color(200, 200, 200, 255));
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