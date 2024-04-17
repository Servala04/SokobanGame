namespace libs;

public sealed class InputHandler{

    private static InputHandler? _instance;
    private GameEngine engine;

    public static InputHandler Instance {
        get{
            if(_instance == null)
            {
                _instance = new InputHandler();
            }
            return _instance;
        }
    }

    private InputHandler() {
        //INIT PROPS HERE IF NEEDED
        engine = GameEngine.Instance;
    }

    public void Handle(ConsoleKeyInfo keyInfo)
    {
        GameObject focusedObject = engine.GetFocusedObject();
        List<GameObject> boxes = engine.GetBoxObjects();
        GameObject player = engine.GetPlayerObject();
        GameObject goal = engine.GetGoalObject();
        GameObject wall = engine.GetWallObject();


        if (focusedObject != null) {

            int dx = 0;
            int dy = 0;

            // Handle keyboard input to move the player
            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    dy = -1;
                    focusedObject.CheckBoxCollision(boxes, player,Direction.Up, dx, dy);
                    break;
                case ConsoleKey.DownArrow:
                    dy = 1;
                    focusedObject.CheckBoxCollision(boxes, player, Direction.Down, dx, dy);
                    break;
                case ConsoleKey.LeftArrow:
                    dx = -1;
                    focusedObject.CheckBoxCollision(boxes, player,Direction.Left, dx, dy);
                    break;
                case ConsoleKey.RightArrow:
                    dx = 1;
                    focusedObject.CheckBoxCollision(boxes, player, Direction.Right, dx, dy);
                    break;
                default:
                    break;
            }
            Console.WriteLine("Player position after movement: ({0}, {1})", player.PosX, player.PosY);

            foreach (var box in boxes)
        {
            Console.WriteLine("Box position after movement: ({0}, {1})", box.PosX, box.PosY);
        }

            if (engine.CanMove(focusedObject, boxes, dx, dy)){
                focusedObject.Move(dx, dy);
                Direction playerDirection = GetDirectionFromConsoleKey(keyInfo.Key);
            
                 // Move the box if possible
                engine.CanMoveBox(wall, player, boxes, playerDirection);
                engine.Render();
            }
            else{
                Console.WriteLine("You can't move there!");
            }

        }
        
    }

    private Direction GetDirectionFromConsoleKey(ConsoleKey key)
{
    switch (key)
    {
        case ConsoleKey.UpArrow:
            return Direction.Up;
        case ConsoleKey.DownArrow:
            return Direction.Down;
        case ConsoleKey.LeftArrow:
            return Direction.Left;
        case ConsoleKey.RightArrow:
            return Direction.Right;
        default:
            throw new ArgumentException("Invalid ConsoleKey");
    }
}

}