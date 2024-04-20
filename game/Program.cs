using libs;

class Program
{    
    static void Main(string[] args)
    {
        //Setup
        Console.CursorVisible = false;
        var engine = GameEngine.Instance;
        var inputHandler = InputHandler.Instance;
        
        engine.Setup();

        Console.WriteLine("Press 'S' to save game, 'L' to load game.");

        // Main game loop
        while (true)
        {
            engine.Render();

            // Handle keyboard input
            if (Console.KeyAvailable)  // Prevents blocking, reads only if a key is pressed
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                inputHandler.Handle(keyInfo);
            }

            // Game logic updates or delay to reduce CPU usage
            System.Threading.Thread.Sleep(100);
        }
    }
}