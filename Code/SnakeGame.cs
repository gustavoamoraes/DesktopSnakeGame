using System;
using System.Linq;
using System.Collections;
using System.Diagnostics;
using System.Globalization;

namespace Program
{
    public class SnakeGame
    {
        //Player
        private Vector2 mainIconPos = new Vector2(0, 0);
        private Vector2 lastMainIconPos = new Vector2(0, 0);
        private Vector2 lastBindedPos = new Vector2(0, 0);

        public const int tilesPerSeconds = 15;
        public int snakeSegments = 2;

        //Grid
        public int tileSize = GameManager.tileSize;        
        //public const float tileOffset = 1;
        public Vector2 gridLimits;

        //Inputs
        public static Vector2 lastInput;

        //Game
        public double deltaTime;
        public bool isAlive = true;

        public Vector2[] objectPositions;
        

        //Refs
        public DesktopManager deskManager;
        
        public SnakeGame(DesktopManager desktop)
        {
            deskManager = desktop;

            Vector2 screenResolution = deskManager.ScreenResolution();

            objectPositions = new Vector2[deskManager.iconsCount];

            gridLimits = new Vector2(MathF.Floor(screenResolution.x/ tileSize) * tileSize, (MathF.Floor(screenResolution.y / tileSize) * tileSize) - tileSize);

            Console.Write(new Tuple<float, float>(screenResolution.x, screenResolution.y));
        }
        
        public Vector2 bindVectorToGrid(Vector2 vec)
        {
            return new Vector2((int)(MathF.Floor(vec.x / tileSize) * tileSize), (int)(MathF.Floor(vec.y / tileSize) * tileSize));
        }

        public bool isWithinTheGrid(Vector2 vec)
        {
            return vec.x >= 0 && vec.x < gridLimits.x && vec.y >= 0 && vec.y < gridLimits.y;
        }

        public void GetRandomFruit()
        {
            objectPositions[snakeSegments] = bindVectorToGrid(new Vector2(new Random().Next(0,(int)gridLimits.x),new Random().Next(0, (int)gridLimits.y)));
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

        public bool isColidingWithItself(Vector2 currentPos)
        {
            for(int i=2;i < snakeSegments; i++)
            {
                if(objectPositions[i].Equals(currentPos))
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
            if (newPos.Equals(objectPositions[snakeSegments]))
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
            objectPositions[0] = newPos;

            if (!isWithinTheGrid(newPos) || isColidingWithItself(newPos))
            {
                isAlive = false;
                return;
            }

            deskManager.SetIconsPositions(objectPositions);

            if (ateFruit)
            {
                deskManager.SetIconsOnScreen(snakeSegments + 1);
            }
        }

        public void UpdateLoop()
        {
            GetRandomFruit();
            DateTime lastTime = DateTime.Now;
            double timer = 0;
            double deltaLimit = 1 / (double)tilesPerSeconds;

            while (isAlive)
            {
                DateTime firstTime = DateTime.Now;

                deltaTime = (firstTime.Ticks - lastTime.Ticks) / (double)10000000;

                if (timer > deltaLimit)
                {

                    #region Input

                    float W_INPUT = new BitArray(new int[] { Win32.GetKeyState(Keys.KEY_W) })[7] ? 1.0f : 0.0f;
                    float S_INPUT = new BitArray(new int[] { Win32.GetKeyState(Keys.KEY_S) })[7] ? 1.0f : 0.0f;
                    float D_INPUT = new BitArray(new int[] { Win32.GetKeyState(Keys.KEY_D) })[7] ? 1.0f : 0.0f;
                    float A_INPUT = new BitArray(new int[] { Win32.GetKeyState(Keys.KEY_A) })[7] ? 1.0f : 0.0f;

                    if (new float[] { W_INPUT, S_INPUT }.Contains(1) && lastInput.y == 0)
                    {
                        lastInput = new Vector2(0, S_INPUT - W_INPUT);
                    }

                    if (new float[] { D_INPUT, A_INPUT }.Contains(1) && lastInput.x == 0)
                    {
                        lastInput = new Vector2(D_INPUT - A_INPUT, 0);
                    }

                    #endregion
  
                    mainIconPos += lastInput * (float)(tileSize * tilesPerSeconds * deltaLimit);

                    Vector2 bindedPos = bindVectorToGrid(mainIconPos);

                    if (!bindedPos.Equals(lastBindedPos))
                    {
                        OnMoved(bindedPos);
                        lastBindedPos = bindedPos;
                    }

                    //float lastDist = MathF.Sqrt(new Vector2(mainIconPos.x - lastMainIconPos.x, mainIconPos.y - lastMainIconPos.y).Magnitude());
                    //double velocity = lastDist/deltaLimit;

                    lastMainIconPos = mainIconPos;
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
