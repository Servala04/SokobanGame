namespace libs;

// public sealed class InputHandler{

//     private static InputHandler? _instance;
//     private GameEngine engine;

//     public static InputHandler Instance {
//         get{
//             if(_instance == null)
//             {
//                 _instance = new InputHandler();
//             }
//             return _instance;
//         }
//     }

//     private InputHandler() {
//         //INIT PROPS HERE IF NEEDED
//         engine = GameEngine.Instance;
//     }

//     public void Handle(ConsoleKeyInfo keyInfo)
//     {
//         engine.SaveState();

//         GameObject focusedObject = engine.GetFocusedObject();
//         GameObject box = engine.GetBoxObject();
//         GameObject goal = engine.GetGoalObject();
//         GameObject player = engine.GetPlayerObject();
//         GameObject wall = engine.GetWallObject();

//         if (focusedObject != null) {
//             // Handle keyboard input to move the player
//             switch (keyInfo.Key)
//             {
//                 case ConsoleKey.UpArrow:
//                     focusedObject.Move(0, -1);
//                     focusedObject.CheckBoxCollision(box, player,Direction.Up);
//                     engine.CheckWallCollision(wall, player, box, Direction.Up);
                  
//                     break;
//                 case ConsoleKey.DownArrow:
//                     focusedObject.Move(0, 1);
//                     focusedObject.CheckBoxCollision(box, player, Direction.Down);
//                     engine.CheckWallCollision(wall, player, box, Direction.Down);
                   
//                     break;
//                 case ConsoleKey.LeftArrow:
//                     focusedObject.Move(-1, 0);
//                     focusedObject.CheckBoxCollision(box, player,Direction.Left);
//                     engine.CheckWallCollision(wall, player, box, Direction.Left);
                   
//                     break;
//                 case ConsoleKey.RightArrow:
//                     focusedObject.Move(1, 0);
//                     focusedObject.CheckBoxCollision(box, player, Direction.Right);
//                     engine.CheckWallCollision(wall, player, box, Direction.Right);
                  
//                     break;
//                 default:
//                     break;
//             }
//         }
        
//     }

// }

public sealed class InputHandler
{
    private static InputHandler? _instance;
    private GameEngine engine;

    public static InputHandler Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new InputHandler();
            }
            return _instance;
        }
    }

    private InputHandler()
    {
        engine = GameEngine.Instance;
    }

    public void Handle(ConsoleKeyInfo keyInfo)
    {
        if (keyInfo.Key == ConsoleKey.Z)
        {
            engine.Rewind();
        }
        else
        {
            GameObject focusedObject = engine.GetFocusedObject();
            GameObject box = engine.GetBoxObject();
            GameObject goal = engine.GetGoalObject();
            GameObject player = engine.GetPlayerObject();
            GameObject wall = engine.GetWallObject();

            engine.SaveState();

            if (focusedObject != null)
            {
                // Existing logic to handle arrow keys...
                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        focusedObject.Move(0, -1);
                        focusedObject.CheckBoxCollision(box, player, Direction.Up);
                        engine.CheckWallCollision(wall, player, box, Direction.Up);
                        break;
                    case ConsoleKey.DownArrow:
                        
                        focusedObject.CheckBoxCollision(box, player, Direction.Down);
                        engine.CheckWallCollision(wall, player, box, Direction.Down);
                        break;
                    case ConsoleKey.LeftArrow:
                        focusedObject.Move(-1, 0);
                        focusedObject.CheckBoxCollision(box, player, Direction.Left);
                        engine.CheckWallCollision(wall, player, box, Direction.Left);
                        break;
                    case ConsoleKey.RightArrow:
                        focusedObject.Move(1, 0);
                        focusedObject.CheckBoxCollision(box, player, Direction.Right);
                        engine.CheckWallCollision(wall, player, box, Direction.Right);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
