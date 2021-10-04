using System;
using System.Linq;
using System.Windows.Forms;

namespace DesktopSnakeGame
{
    public class SnakeGame
    {
        //Player
        private Vector2 mainIconPos = new Vector2(0, 0);
        private Vector2 lastBindedPos = new Vector2(0, 0);
        private Vector2 currentFruitPos;
        public const int tilesPerSeconds = 10;
        public const int framesPerSeconds = 15;
        public int snakeSegments = 2;

        //Grid
        public Vector2 tileSize;      
        public Vector2 gridLimits;

        //Inputs
        public static Vector2 lastInput;

        //Game
        public double deltaTime;
        public bool isAlive = true;
        public DesktopPoint[] objectPositions;
        private DesktopManager desktopManager;

        #region Singleton
        private SnakeGame()
        {
            desktopManager = DesktopManager.Instance;
        }

        static SnakeGame instance;

        public static SnakeGame Instance
        {
            get
            {
                return instance ?? (instance = new SnakeGame());
            }
        }

        #endregion

        public Vector2 bindVectorToGrid(Vector2 vec)
        {
            return new Vector2((int)(Math.Floor(vec.x / tileSize.x) * tileSize.x), (int)(Math.Floor(vec.y / tileSize.y) * tileSize.y));
        }

        public bool isWithinTheGrid(Vector2 vec)
        {
            return vec.x >= 0 && vec.x < gridLimits.x && vec.y >= 0 && vec.y < gridLimits.y;
        }

        public void GetRandomFruit()
        {
            currentFruitPos = bindVectorToGrid(new Vector2(new Random().Next(0, (int)gridLimits.x), new Random().Next(0, (int)gridLimits.y)));
            objectPositions[snakeSegments] = new DesktopPoint((int)currentFruitPos.x, (int)currentFruitPos.y);
        }

        public bool OnFruitEaten()
        {
            snakeSegments++;

            if (snakeSegments == objectPositions.Length)
            {
                return false;
            }
            
            GetRandomFruit();

            return true;
        }

        public bool isColidingWithItself()
        {
            for(int i=3;i < snakeSegments; i++)
            {
                if(objectPositions[i].Equals(objectPositions[0]))
                {
                    return true;
                }
            }
            return false;
        }

        public void OnMoved(Vector2 newPos)
        {
            bool ateFruit = false;

            //is in the fruit position
            if (newPos.Equals(currentFruitPos))
            {
                if (OnFruitEaten())
                {
                    ateFruit = true;
                }
                else
                {
                    isAlive = false;
                    return;
                }
            }

            //Shift the snake segments
            for (int s = snakeSegments-1; s >= 1; s--)
            {
                objectPositions[s] = objectPositions[s - 1];
            }

            //new pos
            objectPositions[0] = new DesktopPoint((int)newPos.x,(int)newPos.y);

            if (!isWithinTheGrid(newPos) || isColidingWithItself())
            {
                isAlive = false;
                return;
            }

            desktopManager.SetIconsPositions(objectPositions);

            if (ateFruit)
            {
                desktopManager.SetIconsOnScreen(snakeSegments + 1);
            }
        }

        public void StopGame()
        {
            snakeSegments = 2;
            mainIconPos = new Vector2();
            lastBindedPos = new Vector2();
            objectPositions = new DesktopPoint[0];
            lastInput = new Vector2();
            isAlive = false;
        }

        //Update variables
        public void Update()
        {
            Vector2 screenResolution = DesktopManager.ScreenResolution();
            //New array in case a icon is created while the app is open
            objectPositions = new DesktopPoint[desktopManager.iconsCount];
            tileSize = DesktopManager.GetTileSize();
            gridLimits = new Vector2((float)Math.Floor(screenResolution.x / tileSize.x) * tileSize.x, (float)(Math.Floor(screenResolution.y / tileSize.y) * tileSize.y)-tileSize.y);

        }

        public void StartLoop()
        {
            Update();

            //Timing
            DateTime lastTime = DateTime.Now;
            double timer = 0;
            double deltaLimit = 1 / (double)framesPerSeconds;

            //Enable in case its not
            isAlive = true;

            //Inital fruit
            GetRandomFruit();

            while (isAlive)
            {
                DateTime firstTime = DateTime.Now;

                deltaTime = (firstTime.Ticks - lastTime.Ticks) / (double)10000000;

                #region Input

                float W_INPUT = Win32.GetKeyState(Keys.KEY_W) > 1 ? 1.0f : 0.0f;
                float S_INPUT = Win32.GetKeyState(Keys.KEY_S) > 1 ? 1.0f : 0.0f;
                float D_INPUT = Win32.GetKeyState(Keys.KEY_D) > 1 ? 1.0f : 0.0f;
                float A_INPUT = Win32.GetKeyState(Keys.KEY_A) > 1 ? 1.0f : 0.0f;

                if (new float[] { W_INPUT, S_INPUT }.Contains(1) && lastInput.y == 0)
                {
                    lastInput = new Vector2(0, S_INPUT - W_INPUT);
                }

                if (new float[] { D_INPUT, A_INPUT }.Contains(1) && lastInput.x == 0)
                {
                    lastInput = new Vector2(D_INPUT - A_INPUT, 0);
                }

                #endregion

                if (timer > deltaLimit)
                {
                    mainIconPos += new Vector2(lastInput.x * tileSize.x, lastInput.y * tileSize.y) * (float)(tilesPerSeconds * deltaLimit);

                    Vector2 bindedPos = bindVectorToGrid(mainIconPos);

                    if (!bindedPos.Equals(lastBindedPos))
                    {
                        OnMoved(bindedPos);
                        lastBindedPos = bindedPos;
                        
                    }

                    //float lastDist = MathF.Sqrt(new Vector2(mainIconPos.x - lastMainIconPos.x, mainIconPos.y - lastMainIconPos.y).Magnitude());
                    //double velocity = lastDist/deltaLimit;
                    //lastMainIconPos = mainIconPos;

                    timer = 0;
                }
                else
                {
                    timer += deltaTime;
                }
                lastTime = firstTime;
            }
        }
    }
}
