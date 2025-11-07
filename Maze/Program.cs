//Alex Gardner 11/01/25 Lab 9: Maze
//This program will create a simple maze for the user to navigate through in the console.
using System.Xml.Serialization;

class Program
{
    static void Main(string[] args)
    {
        string baseDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\.."));

        string[] maze = File.ReadAllLines(baseDirectory + "\\map.txt");

        for (int i = 0; i < maze.Length; i++)
        {
            Console.WriteLine(maze[i]);
        }

        Console.SetCursorPosition(0, 0);
        bool win = false;

        do
        {
            ConsoleKey keyPressed = Console.ReadKey(true).Key;

            switch(keyPressed)
            {
                case ConsoleKey.Escape:
                    return;
                case ConsoleKey.UpArrow:
                    win = CursorMove(maze, 0, -1);
                    break;
                case ConsoleKey.DownArrow:
                    win = CursorMove(maze, 0, 1);
                    break;
                case ConsoleKey.LeftArrow:
                    win = CursorMove(maze, -1, 0);
                    break;
                case ConsoleKey.RightArrow:
                    win = CursorMove(maze, 1, 0);
                    break;
            }

        } while(!win);

        Console.Clear();
        Console.WriteLine("Congratulations!\nYou've Won!");
    }

    static bool CursorMove(string[] maze, int x, int y)
    {
        char nextMove = maze[Console.CursorTop + y][Console.CursorLeft + x];
        if (nextMove == '*')
        {
            return true;
        }
        else if (nextMove != '#')
        {
            Console.CursorLeft += x;
            Console.CursorTop += y;
        }

        Console.CursorLeft = Math.Clamp(Console.CursorLeft, 0, maze[Console.CursorTop].Length);
        Console.CursorTop = Math.Clamp(Console.CursorTop, 0, maze.Length);

        return false;
    }
}