using Raylib_cs;

namespace TerrariaClone.Core;

public static class Settings
{
    // ---------- Окно ----------
    public const string GameTitle      = "Terraria Clone";
    public const int    ScreenWidth    = 1280;
    public const int    ScreenHeight   = 720;
    public const int    TargetFps      = 0; // 0 = VSync берет управление на себя
    public const bool   VSyncEnabled   = true;

    // ---------- Тайлы ----------
    public const int TileSize          = 16;
    public const int TileBlockSize     = 16;
    
    // ---------- Взаимодействие ----------
    /// <summary>Максимальная дистанция копания/установки в тайлах (как в Terraria).</summary>
    public const float PlayerReach     = 6.5f; 

    // ---------- Физика ----------
    public const float Gravity         = 0.4f;
    public const float MaxFallSpeed    = 10.0f;
    public const float PlayerMoveSpeed = 3.0f;
    public const float PlayerAccel     = 0.3f;
    public const float PlayerJumpSpeed = -7.5f;
    public const float AirDrag         = 0.95f;

    // ---------- Цвета ----------
    public static readonly Color SkyColor         = new(92, 149, 229, 255);
    public static readonly Color GridColor        = new(0, 0, 0, 60);
    public static readonly Color HudBgColor       = new(0, 0, 0, 160);
    public static readonly Color HudTextColor     = new(240, 240, 240, 255);
    public static readonly Color PlayerBodyColor  = new(255, 222, 173, 255);
    public static readonly Color PlayerShirtColor = new(60, 130, 200, 255);
    public static readonly Color PlayerPantsColor = new(70, 70, 110, 255);
}