using System;
using Raylib_cs;
using TerrariaClone.Core;
using TerrariaClone.World;

namespace TerrariaClone.Items;

public class Hotbar
{
    // Массив доступных блоков в хотбаре
    public TileType[] Slots { get; private set; }
    public int SelectedIndex { get; private set; }

    public Hotbar()
    {
        // Первые 3 слота: Земля, Камень, Трава
        Slots = new TileType[] { TileType.Dirt, TileType.Stone, TileType.Grass };
        SelectedIndex = 0;
    }

    public TileType SelectedTile => Slots[SelectedIndex];

    public void Update()
    {
        // Переключение цифрами 1, 2, 3
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_ONE)) SelectedIndex = 0;
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_TWO)) SelectedIndex = 1;
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_THREE)) SelectedIndex = 2;
        
        // Колесико мыши (опционально)
        float wheel = Raylib.GetMouseWheelMove();
        if (wheel > 0) SelectedIndex = (SelectedIndex - 1 + Slots.Length) % Slots.Length;
        if (wheel < 0) SelectedIndex = (SelectedIndex + 1) % Slots.Length;
    }

    public void Draw(World.World world)
    {
        int slotSize = 48;
        int padding = 5;
        int totalWidth = (slotSize + padding) * Slots.Length - padding;
        
        // Центрируем хотбар внизу экрана
        int startX = Settings.ScreenWidth / 2 - totalWidth / 2;
        int startY = Settings.ScreenHeight - slotSize - 20;

        for (int i = 0; i < Slots.Length; i++)
        {
            int x = startX + i * (slotSize + padding);
            int y = startY;

            // Фон слота
            Raylib.DrawRectangle(x, y, slotSize, slotSize, new Color(30, 30, 40, 200));
            
            // Рамка (желтая, если выбран)
            Color border = (i == SelectedIndex) ? new Color(255, 220, 50, 255) : new Color(100, 100, 100, 255);
            Raylib.DrawRectangleLines(x, y, slotSize, slotSize, border);
            Raylib.DrawRectangleLines(x+1, y+1, slotSize-2, slotSize-2, border); // Толще рамка

            // Отрисовка самого блока внутри слота
            // Создаем фейковые координаты для цвета
            Color blockColor = world.GetTileColor(Slots[i], i, i);
            
            // Рисуем 2х2 блока (как иконку)
            int iconSize = slotSize / 2;
            int iconX = x + slotSize / 4;
            int iconY = y + slotSize / 4;
            Raylib.DrawRectangle(iconX, iconY, iconSize, iconSize, blockColor);
            Raylib.DrawRectangleLines(iconX, iconY, iconSize, iconSize, new Color(0,0,0,100));

            // Номер слота
            Raylib.DrawText((i + 1).ToString(), x + 4, y + 4, 10, Settings.HudTextColor);
        }
    }
}