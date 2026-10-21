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
        int baseSurfaceHeight = Height / 3;
        int[] surfaceHeights = new int[Width];

        for (int x = 0; x < Width; x++)
        {
            float h1 = (float)Math.Sin(x * 0.01f) * 30f; 
            float h2 = (float)Math.Sin(x * 0.05f) * 10f; 
            float h3 = (float)Math.Sin(x * 0.2f) * 3f;   

            surfaceHeights[x] = baseSurfaceHeight + (int)(h1 + h2 + h3);
        }

        for (int x = 0; x < Width; x++)
        {
            int surfaceY = surfaceHeights[x];
            int dirtDepth = 6 + (int)(Math.Sin(x * 0.03f) * 4f);

            for (int y = 0; y < Height; y++)
            {
                if (y < surfaceY)
                    Tiles[x, y] = new Tile(TileType.Air);
                else if (y == surfaceY)
                    Tiles[x, y] = new Tile(TileType.Grass);
                else if (y > surfaceY && y < surfaceY + dirtDepth)
                    Tiles[x, y] = new Tile(TileType.Dirt);
                else
                    Tiles[x, y] = new Tile(TileType.Stone);
            }
        }
    }

    /// <summary>
    /// Возвращает координату Y (в пикселях) поверхности в заданном столбце X (в тайлах).
    /// </summary>
    public int GetSurfaceY(int tileX)
    {
        if (tileX < 0) tileX = 0;
        if (tileX >= Width) tileX = Width - 1;

        for (int y = 0; y < Height; y++)
        {
            // Первый твердый блок сверху — это поверхность
            if (Tiles[tileX, y].IsSolid)
            {
                return y * Settings.TileSize;
            }
        }
        return 0; // Если мира нет (пустота), спавним на самом верху
    }

    public Tile GetTile(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            return new Tile(TileType.Air);

        return Tiles[x, y];
    }

    public bool IsTileSolid(int x, int y)
    {
        return GetTile(x, y).IsSolid;
    }

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
        int hash = (x * 73856093) ^ (y * 19349663);
        int noise = (hash & 0x7FFFFFFF) % 20 - 10; 
        static int C(int val) => Math.Clamp(val, 0, 255);

        return type switch
        {
            TileType.Grass => new Color(C(80 + noise), C(180 + noise), C(60 + noise), 255),
            TileType.Dirt  => new Color(C(130 + noise), C(80 + noise), C(40 + noise), 255),
            TileType.Stone => new Color(C(100 + noise), C(100 + noise), C(100 + noise), 255),
            _ => new Color(255, 255, 255, 255)
        };
    }
}