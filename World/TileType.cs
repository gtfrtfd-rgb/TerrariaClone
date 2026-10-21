namespace TerrariaClone.World;

/// <summary>
/// Типы тайлов (блоков) в мире.
/// </summary>
public enum TileType : byte
{
    Air = 0,    // Пустота (воздух)
    Dirt = 1,   // Земля
    Grass = 2,  // Трава (верхний слой)
    Stone = 3   // Камень
}