using System;

namespace Program
{
    public class GameManager
    {
        public const int tileSize = 50;

        public static void Main()
        {
            DesktopManager desktop = new DesktopManager();
            SnakeGame snakeGame = new SnakeGame(desktop);

            int iconsOnScreen = 1 + snakeGame.snakeSegments;

            desktop.SetIconsOnScreen(iconsOnScreen);
            snakeGame.UpdateLoop();
        }
    }
}
