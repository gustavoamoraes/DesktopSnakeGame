using System;
using System.Windows.Forms;

namespace DesktopSnakeGame
{
    public class GameManager
    {
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());  
        }

        public static void StartGameLoop()
        {
            DesktopManager deskManager = DesktopManager.Instance;
            int iconsOnScreen = 1 + SnakeGame.Instance.snakeSegments;

            //Set up desktop
            deskManager.Update();
            deskManager.SetIconsOnScreen(iconsOnScreen);
            //deskManager.SetIconsSpacing(DesktopManager.DesktopIconSize(), DesktopManager.DesktopIconSize());

            //Start game
            SnakeGame.Instance.StartLoop();
        }

        public static void Stop()
        {
            SnakeGame.Instance.StopGame();
            DesktopManager.Instance.ResetDesktop();
        }
    }
}
