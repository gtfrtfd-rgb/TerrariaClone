namespace TerrariaClone.World;

/// <summary>
/// Данные одного тайла в мире.
/// </summary>
public struct Tile
{
    public TileType Type;
    
    /// <summary>Твердый ли блок (для коллизий). Воздух и некоторые объекты — не твердые.</summary>
    public bool IsSolid => Type != TileType.Air;

    public Tile(TileType type)
    {
        Type = type;
    }
}