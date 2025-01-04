namespace Snake
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Xml.Linq;

    class Program
    {
        static void Main()
        {
            Console.SetWindowSize(40, 22);
            Console.ForegroundColor = ConsoleColor.Green;
            GameScreen gs = new GameScreen(40, 20);
            Snake snake = new Snake(40, 20);
            Food apple = new Food(40, 20);
            bool gameResume = true;
            while (gameResume)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey();
                    switch (key.Key)
                    {
                        case ConsoleKey.W://up
                            snake.ChangeDirection(0, -1);
                            break;
                        case ConsoleKey.S://down
                            snake.ChangeDirection(0, 1);
                            break;
                        case ConsoleKey.A://left
                            snake.ChangeDirection(-1, 0);
                            break;
                        case ConsoleKey.D://right
                            snake.ChangeDirection(1, 0);
                            break;
                    }
                }
                snake.Move();
                if (snake.HeadX == apple.PositionX && snake.HeadY == apple.PositionY)
                {
                    Snake.Score++;
                    snake.Grow();
                    snake.SpeedLevel();
                    apple.GenerateNewPosition();
                }
                int[] bodyX = snake.bodyX.ToArray();
                int[] bodyY = snake.bodyY.ToArray();
                for (int i = 1; i < bodyX.Length; i++)
                {
                    if (snake.HeadX == bodyX[i] && snake.HeadY == bodyY[i])
                    {
                        gameResume = false;
                        break;
                    }
                }
                try
                {
                    gs.DrawSnake(snake);
                }
                catch (IndexOutOfRangeException)
                {
                    gameResume = false;
                }
                gs.DrawFood(apple);
                gs.DrawBorders();
                gs.Render();
                gs.Clear();
                Thread.Sleep(snake.Speed);
            }
            Console.Clear();
            Console.WriteLine($"GameOver! Score: {Snake.Score}");
            Console.WriteLine("Press any key to exit:");
            Console.ReadKey();
        }
        class GameScreen
        {
            private char[,] screen;
            private int width;
            private int height;
            public GameScreen(int width, int height)
            {
                this.width = width;
                this.height = height;
                screen = new char[height, width];
                Clear();
            }
            public void Clear()
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        screen[y, x] = ' ';
                    }
                }
            }
            public void DrawBorders()
            {
                for (int x = 0; x < width; x++)
                {
                    screen[0, x] = '═';
                    screen[height - 1, x] = '═';
                }
                for (int y = 0; y < height; y++)
                {
                    screen[y, 0] = '║';
                    screen[y, width - 1] = '║';
                }
            }
            public void DrawFood(Food apple)
            {
                screen[apple.PositionY, apple.PositionX] = '@';
            }
            public void DrawSnake(Snake snake)
            {
                int[] bodyX = snake.bodyX.ToArray();
                int[] bodyY = snake.bodyY.ToArray();
                for (int i = 0; i < bodyX.Length; i++)
                {
                    screen[bodyY[i], bodyX[i]] = '0';
                }
            }
            public void Render()
            {
                Console.Clear();
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Console.Write(screen[y, x]);
                    }
                    Console.WriteLine();
                }
            }
        }
        class Food
        {
            public int PositionX { get; private set; }
            public int PositionY { get; private set; }
            private int width;
            private int height;
            private Random random;
            public Food(int width, int height)
            {
                this.width = width;
                this.height = height;
                GenerateNewPosition();
            }
            public void GenerateNewPosition()
            {
                random = new Random();
                PositionX = random.Next(1, width - 1);
                PositionY = random.Next(1, height - 1);
            }
        }
        class Snake
        {
            static public int Score;
            public int Speed = 250;
            public LinkedList<int> bodyX = new LinkedList<int>();
            public LinkedList<int> bodyY = new LinkedList<int>();
            private int directionX = 1;
            private int directionY = 0;
            public int HeadX => bodyX.First.Value;
            public int HeadY => bodyY.First.Value;
            public Snake(int width, int height)
            {
                int startX = width / 2;
                int startY = height / 2;
                bodyX.AddFirst(startX);
                bodyY.AddFirst(startY);
            }
            public void SpeedLevel()
            {
                if (Speed >= 50) 
                { 
                    Speed -= 5; 
                }
            }
            public void ChangeDirection(int dx, int dy)
            {
                if (directionX + dx != 0 || directionY + dy != 0)
                {
                    directionX = dx;
                    directionY = dy;
                }
            }
            public void Move()
            {
                var newHeadX = HeadX + directionX;
                var newHeadY = HeadY + directionY;
                bodyX.AddFirst(newHeadX);
                bodyY.AddFirst(newHeadY);
                if (bodyX.Count > 1 && bodyY.Count > 1)
                {
                    bodyX.RemoveLast();
                    bodyY.RemoveLast();
                }
            }
            public void Grow()
            {
                var newHeadX = HeadX + directionX;
                var newHeadY = HeadY + directionY;
                bodyX.AddFirst(newHeadX);
                bodyY.AddFirst(newHeadY);
            }
        }
    }
}