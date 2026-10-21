using Raylib_cs;

namespace TerrariaClone.Core;

/// <summary>
/// Глобальные константы игры. Соответствуют параметрам оригинальной Terraria.
/// </summary>
public static class Settings
{
    // ---------- Окно ----------
    public const string GameTitle      = "Terraria Clone";
    public const int    ScreenWidth    = 1280;
    public const int    ScreenHeight   = 720;
    public const int    TargetFps      = 60;
    public const bool   VSyncEnabled   = true;

    // ---------- Тайлы ----------
    /// <summary>Размер одного тайла в пикселях. В Terraria = 16.</summary>
    public const int TileSize          = 16;
    /// <summary>Размер одного "блока" в тайлах. В Terraria = 16x16 тайлов = 256 px.</summary>
    public const int TileBlockSize     = 16;

    // ---------- Физика (значения подобраны как в Terraria при 60 FPS) ----------
    /// <summary>Ускорение свободного падения (пикселей/кадр²).</summary>
    public const float Gravity         = 0.4f;
    /// <summary>Максимальная скорость падения.</summary>
    public const float MaxFallSpeed    = 10.0f;
    /// <summary>Базовая скорость передвижения игрока.</summary>
    public const float PlayerMoveSpeed = 3.0f;
    /// <summary>Ускорение разгона по земле.</summary>
    public const float PlayerAccel     = 0.3f;
    /// <summary>Начальная скорость прыжка.</summary>
    public const float PlayerJumpSpeed = -7.5f;
    /// <summary>Сопротивление воздуха в прыжке.</summary>
    public const float AirDrag         = 0.95f;

    // ---------- Цвета (палитра в стиле Terraria) ----------
    public static readonly Color SkyColor         = new(92, 149, 229, 255);
    public static readonly Color DaySkyColor      = new(112, 168, 240, 255);
    public static readonly Color NightSkyColor    = new(28, 40, 80, 255);
    public static readonly Color GridColor        = new(0, 0, 0, 60);
    public static readonly Color HudBgColor       = new(0, 0, 0, 160);
    public static readonly Color HudTextColor     = new(240, 240, 240, 255);
    public static readonly Color PlayerBodyColor  = new(255, 222, 173, 255);
    public static readonly Color PlayerShirtColor = new(60, 130, 200, 255);
    public static readonly Color PlayerPantsColor = new(70, 70, 110, 255);
}