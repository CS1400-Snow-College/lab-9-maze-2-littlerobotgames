//Alex Gardner 11/06/25 Lab 9: Maze
//This program will create a simple maze for the user to navigate through in the console.

using System.Diagnostics.Contracts;
using System.Security.Principal;

class Program
{
    public static Thing[,] gameObjects;
    public static int playerX = 0;
    public static int playerY = 0;
    public static int points = 0;
    public static bool isRunning = true;
    public static int keys = 0;
    public static DateTime startTime;

    static void Main(string[] args)
    {
        startTime = DateTime.Now;
        LoadMaze("maze.txt");

        Gameloop(1);
    }
    static void Gameloop(int fps)
    {
        int fpsSleep = (1 / fps) * 1000;

        while(isRunning)
        {
            DrawMaze();
            GetPlayerKeyPresses();
            MoveEnemies();
        }
    }
    static void GetPlayerKeyPresses()
    {
        ConsoleKey keyPressed = Console.ReadKey(false).Key;
        
        switch(keyPressed)
        {
            case ConsoleKey.UpArrow:
                PlayerMove(0, -1);
                break;
            case ConsoleKey.DownArrow:
                PlayerMove(0, 1);
                break;
            case ConsoleKey.LeftArrow:
                PlayerMove(-1, 0);
                break;
            case ConsoleKey.RightArrow:
                PlayerMove(1, 0);
                break;
            case ConsoleKey.Escape:
                Environment.Exit(0);
                break;
        }
    }
    static Thing CheckCollision(int x, int y)
    {
        x = Math.Clamp(x, 0, gameObjects.GetLength(1) - 1);
        y = Math.Clamp(y, 0, gameObjects.GetLength(0) - 1);

        return gameObjects[y, x];
    }
    static void PlayerMove(int x, int y)
    {
        Thing nextMove = CheckCollision(playerX + x, playerY + y);

        if (nextMove != null)
        {
            if (!nextMove.isSolid)
            {
                playerX += x;
                playerY += y;
            }
        }
        else
        {
            playerX += x;
            playerY += y;
        }

        Thing onPlayer = CheckCollision(playerX, playerY);

        if (onPlayer != null)
        {
            if (onPlayer.GetType() == typeof(Goal))
            {
                GameWin();
                return;
            }
            else if (onPlayer.GetType() == typeof(Enemy))
            {
                GameLose();
                return;
            }
            else if (onPlayer.GetType() == typeof(Key))
            {
                Key tempPoints = (Key)onPlayer;
                points += tempPoints.Worth;

                gameObjects[playerY, playerX] = null;
                keys--;

                if (keys == 0)
                {
                    //Remove all gates
                    for (int yy = 0; yy < gameObjects.GetLength(0); yy++)
                    {
                        for (int xx = 0; xx < gameObjects.GetLength(1); xx++)
                        {
                            if (gameObjects[yy, xx] != null)
                            {
                                if (gameObjects[yy, xx].GetType() == typeof(Gate))
                                {
                                    gameObjects[yy, xx] = null;
                                }
                            }
                        }
                    }
                }
            }
            else if (onPlayer.GetType() == typeof(Gold))
            {
                Gold tempPoints = (Gold)onPlayer;
                points += tempPoints.Worth;

                gameObjects[playerY, playerX] = null;
            }
        }


        playerX = Math.Clamp(playerX, 0, gameObjects.GetLength(1) - 1);
        playerY = Math.Clamp(playerY, 0, gameObjects.GetLength(0) - 1);
    }
    static void GameWin()
    {
        isRunning = false;
        Console.Clear();
        Console.WriteLine("Congratulations!\nYou've Won!");
        Console.WriteLine($"You won the level in {DateTime.Now - startTime}");
    }
    static void GameLose()
    {
        isRunning = false;
        Console.Clear();
        Console.WriteLine("You've Lost!");
    }
    static void DrawMaze()
    {
        Console.Clear();

        for (int y = 0; y < gameObjects.GetLength(0); y++)
        {
            for (int x = 0;  x < gameObjects.GetLength(1); x++)
            {
                if (gameObjects[y , x] != null)
                {
                    Console.Write(gameObjects[y, x].Icon);
                }
                else
                {
                    if (y == playerY && x == playerX)
                    {
                        Console.Write('A');
                    }
                    else
                    {
                        Console.Write(' ');
                    }  
                }
                
            }
            Console.WriteLine();
        }
        Console.WriteLine();
        Console.WriteLine($"Points: {points}");
    }
    static void LoadMaze(string fileName)
    {
        string baseDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\.."));
        string[] maze = File.ReadAllLines(baseDirectory + "\\maze.txt");
        gameObjects = new Thing[maze.Length, maze[0].Length];

        for (int i = 0; i < maze.Length; i++)
        {
            for (int j = 0; j < maze[i].Length; j++)
            {
                char t = maze[i][j];

                switch (t)
                {
                    case '*':
                        gameObjects[i, j] = new Wall('#', true);
                        break;
                    case '^':
                        gameObjects[i, j] = new Key('^', false, 20);
                        keys++;
                        break;
                    case '$':
                        gameObjects[i, j] = new Gold('$', false, 100);
                        break;
                    case '#':
                        gameObjects[i, j] = new Goal('*', false);
                        break;
                    case '|':
                        gameObjects[i, j] = new Gate('|', true);
                        break;
                    case '%':
                        gameObjects[i, j] = new Enemy('%', false);
                        break;
                }
            }
        }
    }
    static void MoveEnemies()
    {
        //Reset all enemy moved
        for (int y = 0; y < gameObjects.GetLength(0); y++)
        {
            for (int x = 0; x < gameObjects.GetLength(1); x++)
            {
                if (gameObjects[y, x] != null)
                {
                    if (gameObjects[y, x].GetType() == typeof(Enemy))
                    {
                        Enemy enemy = (Enemy)gameObjects[y, x];
                        enemy.moved = false;
                    }
                }
            }
        }
        //Move all not moved enemies
        for (int y = 0; y < gameObjects.GetLength(0); y++)
        {
            for (int x = 0;  x < gameObjects.GetLength(1); x++)
            {
                if (gameObjects[y, x] != null)
                {
                    if (gameObjects[y, x].GetType() == typeof(Enemy))
                    {
                        Enemy enemy = (Enemy)gameObjects[y, x];

                        if (!enemy.moved)
                        {
                            if (enemy.movingDown)
                            {
                                Thing moveTo = CheckCollision(x, y + 1);

                                if (moveTo != null)
                                {
                                    if (!moveTo.isSolid)
                                    {
                                        gameObjects[y + 1, x] = enemy;
                                        gameObjects[y, x] = null;
                                        enemy.moved = true;
                                    }
                                    else
                                    {
                                        enemy.movingDown = !enemy.movingDown;
                                    }
                                }
                                else
                                {
                                    gameObjects[y + 1, x] = enemy;
                                    gameObjects[y, x] = null;
                                    enemy.moved = true;
                                }
                            }
                            else
                            {
                                Thing moveTo = CheckCollision(x, y - 1);

                                if (moveTo != null)
                                {
                                    if (!moveTo.isSolid)
                                    {
                                        gameObjects[y - 1, x] = enemy;
                                        gameObjects[y, x] = null;
                                        enemy.moved = true;
                                    }
                                    else
                                    {
                                        enemy.movingDown = !enemy.movingDown;
                                    }
                                }
                                else
                                {
                                    gameObjects[y - 1, x] = enemy;
                                    gameObjects[y, x] = null;
                                    enemy.moved = true;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

class Thing
{
    public char Icon;
    public bool isSolid;
    public Thing(char icon, bool solid)
    {
        Icon = icon;
        isSolid = solid;
    }
}
class Key : Thing
{
    public int Worth;
    public Key(char icon, bool solid, int worth) : base(icon, solid)
    {
        Icon = icon;
        isSolid = solid;
        Worth = worth;
    }
}
class Gold : Thing
{
    public int Worth;
    public Gold(char icon, bool solid, int worth) : base(icon, solid)
    {
        Icon = icon;
        isSolid = solid;
        Worth = worth;
    }
}
class Wall : Thing
{
    public Wall(char icon, bool solid) : base(icon, solid)
    {
        Icon = icon;
        isSolid = solid;
    }
}
class Gate : Thing
{
    public Gate(char icon, bool solid) : base(icon, solid)
    {
        Icon = icon;
        isSolid = solid;
    }
}
class Enemy : Thing
{
    public bool movingDown = true;
    public bool moved = false;
    public Enemy(char icon, bool solid) : base (icon, solid)
    {
        Icon = icon;
        isSolid = solid;
    }
}
class Goal : Thing
{
    public Goal(char icon, bool solid) : base (icon, solid)
    {
        Icon = icon;
        isSolid = solid;
    }
}