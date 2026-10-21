using System;
using System.Numerics;
using Raylib_cs;
using TerrariaClone.Core;
using TerrariaClone.World;

namespace TerrariaClone.Entities;

public class Player
{
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    
    public const int Width = 20;
    public const int Height = 42;

    private bool _isGrounded;
    
    public Player(float startX, float startY)
    {
        Position = new Vector2(startX, startY);
        Velocity = Vector2.Zero;
    }

    public void Update(float dt, World.World world)
    {
        float timeScale = dt * 60f;

        // --- Ввод по X ---
        float moveDirection = 0;
        if (Raylib.IsKeyDown(KeyboardKey.KEY_A) || Raylib.IsKeyDown(KeyboardKey.KEY_LEFT))  moveDirection -= 1;
        if (Raylib.IsKeyDown(KeyboardKey.KEY_D) || Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT)) moveDirection += 1;

        // Разгон и ограничение скорости
        if (moveDirection != 0)
        {
            Velocity = new Vector2(Velocity.X + Settings.PlayerAccel * moveDirection * timeScale, Velocity.Y);
            Velocity = new Vector2(Math.Clamp(Velocity.X, -Settings.PlayerMoveSpeed, Settings.PlayerMoveSpeed), Velocity.Y);
        }
        else
        {
            float drag = _isGrounded ? 0.8f : Settings.AirDrag;
            Velocity = new Vector2(Velocity.X * (float)Math.Pow(drag, timeScale), Velocity.Y);
            if (Math.Abs(Velocity.X) < 0.1f) Velocity = new Vector2(0, Velocity.Y);
        }

        // --- Прыжок ---
        if (_isGrounded && (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE) || Raylib.IsKeyPressed(KeyboardKey.KEY_W) || Raylib.IsKeyPressed(KeyboardKey.KEY_UP)))
        {
            Velocity = new Vector2(Velocity.X, Settings.PlayerJumpSpeed);
            _isGrounded = false;
        }

        // --- Гравитация ---
        Velocity = new Vector2(Velocity.X, Velocity.Y + Settings.Gravity * timeScale);
        if (Velocity.Y > Settings.MaxFallSpeed) Velocity = new Vector2(Velocity.X, Settings.MaxFallSpeed);

        // --- Применение движения и коллизии ---
        // 1. Двигаем по X
        Vector2 nextPosX = new Vector2(Position.X + Velocity.X * timeScale, Position.Y);
        if (CheckCollision(nextPosX, world))
        {
            Velocity = new Vector2(0, Velocity.Y);
        }
        else
        {
            Position = nextPosX;
        }

        // 2. Двигаем по Y
        Vector2 nextPosY = new Vector2(Position.X, Position.Y + Velocity.Y * timeScale);
        _isGrounded = false;
        
        if (CheckCollision(nextPosY, world))
        {
            if (Velocity.Y > 0) _isGrounded = true;
            Velocity = new Vector2(Velocity.X, 0);
        }
        else
        {
            Position = nextPosY;
        }
    }

    private bool CheckCollision(Vector2 nextPos, World.World world)
    {
        int ts = Settings.TileSize;
        
        int minX = (int)Math.Floor(nextPos.X / ts);
        int maxX = (int)Math.Floor((nextPos.X + Width - 1) / ts);
        int minY = (int)Math.Floor(nextPos.Y / ts);
        int maxY = (int)Math.Floor((nextPos.Y + Height - 1) / ts);

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                if (world.IsTileSolid(x, y))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void Draw()
    {
        int px = (int)Position.X;
        int py = (int)Position.Y;

        // Голова
        Raylib.DrawRectangle(px + 3, py, 14, 14, Settings.PlayerBodyColor);
        // Рубашка
        Raylib.DrawRectangle(px, py + 14, Width, 16, Settings.PlayerShirtColor);
        // Ноги
        Raylib.DrawRectangle(px, py + 30, Width, 12, Settings.PlayerPantsColor);
        
        // Отладочный хитбокс
        Raylib.DrawRectangleLines(px, py, Width, Height, new Color(255, 0, 0, 100));
    }
}