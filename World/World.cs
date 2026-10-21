using System;
using Raylib_cs;
using TerrariaClone.Core;

namespace TerrariaClone.World;

public class World
{
    public int Width { get; private set; }
    public int Height { get; private set; }
    
    public Tile[,] Tiles { get; private set; }

    public World(int width, int height)
    {
        Width = width;
        Height = height;
        Tiles = new Tile[Width, Height];
        GenerateWorld();
    }

    private void GenerateWorld()
    {
        Random rng = new Random();
        int baseSurfaceHeight = Height / 3;

        for (int x = 0; x < Width; x++)
        {
            float noise = (float)Math.Sin(x * 0.05) * 15f + rng.Next(-2, 3);
            int surfaceY = baseSurfaceHeight + (int)noise;

            for (int y = 0; y < Height; y++)
            {
                if (y < surfaceY)
                {
                    Tiles[x, y] = new Tile(TileType.Air);
                }
                else if (y == surfaceY)
                {
                    Tiles[x, y] = new Tile(TileType.Grass);
                }
                else if (y > surfaceY && y < surfaceY + rng.Next(4, 8))
                {
                    Tiles[x, y] = new Tile(TileType.Dirt);
                }
                else
                {
                    Tiles[x, y] = new Tile(TileType.Stone);
                }
            }
        }
    }

    /// <summary>
    /// Возвращает тайл по координатам сетки. Если координаты вне мира, возвращает пустоту (или камень за границей).
    /// </summary>
    public Tile GetTile(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            return new Tile(TileType.Air); // За пределами мира — пустота

        return Tiles[x, y];
    }

    /// <summary>
    /// Проверяет, является ли блок в указанных координатах сетки твердым.
    /// </summary>
    public bool IsTileSolid(int x, int y)
    {
        return GetTile(x, y).IsSolid;
    }

    /// <summary>
    /// Отрисовка видимых тайлов.
    /// </summary>
    public void Draw(Camera2D camera)
    {
        int ts = Settings.TileSize;
        
        int startX = (int)(camera.Target.X - (Settings.ScreenWidth / 2f) / camera.Zoom) / ts - 2;
        int endX = (int)(camera.Target.X + (Settings.ScreenWidth / 2f) / camera.Zoom) / ts + 2;
        int startY = (int)(camera.Target.Y - (Settings.ScreenHeight / 2f) / camera.Zoom) / ts - 2;
        int endY = (int)(camera.Target.Y + (Settings.ScreenHeight / 2f) / camera.Zoom) / ts + 2;

        startX = Math.Clamp(startX, 0, Width - 1);
        endX = Math.Clamp(endX, 0, Width - 1);
        startY = Math.Clamp(startY, 0, Height - 1);
        endY = Math.Clamp(endY, 0, Height - 1);

        for (int x = startX; x <= endX; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                Tile tile = Tiles[x, y];
                if (tile.Type == TileType.Air) continue;

                Color color = GetTileColor(tile.Type, x, y);
                Raylib.DrawRectangle(x * ts, y * ts, ts, ts, color);
                Raylib.DrawRectangleLines(x * ts, y * ts, ts, ts, new Color(0, 0, 0, 40));
            }
        }
    }

    private Color GetTileColor(TileType type, int x, int y)
    {
        int noise = (x * 7 + y * 13) % 20 - 10; 

        return type switch
        {
            TileType.Grass  => new Color(80 + noise, 180 + noise, 60 + noise, 255),
            TileType.Dirt   => new Color(130 + noise, 80 + noise, 40 + noise, 255),
            TileType.Stone  => new Color(100 + noise, 100 + noise, 100 + noise, 255),
            _ => new Color(255, 255, 255, 255)
        };
    }
}