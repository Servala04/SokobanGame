using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Nodes;

namespace libs;

using System.Security.Cryptography;
using Newtonsoft.Json;


public sealed class GameEngine
{
    private static GameEngine? _instance;
    private IGameObjectFactory gameObjectFactory;


    public static GameEngine Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameEngine();
            }
            return _instance;
        }
    }

    private GameEngine()
    {
        //INIT PROPS HERE IF NEEDED
        gameObjectFactory = new GameObjectFactory();
    }

    private GameObject? _focusedObject;

    private Map map = new Map();

    private List<GameObject> gameObjects = new List<GameObject>();


    public Map GetMap()
    {
        return map;
    }

    public GameObject GetFocusedObject()
    {
        return _focusedObject;
    }

    public GameObject GetBoxObject()
    {
        foreach (var gameObject in gameObjects)
        {
            if (gameObject is Box)
            {
                return gameObject;
            }
        }
        return null;
    }

    public GameObject GetGoalObject()
    {
        foreach (var gameObject in gameObjects)
        {
            if (gameObject is Goal)
            {
                return gameObject;
            }
        }
        return null;
    }

    public GameObject GetPlayerObject()
    {
        foreach (var gameObject in gameObjects)
        {
            if (gameObject is Player)
            {
                return gameObject;
            }
        }
        return null;
    }

    public GameObject GetWallObject()
    {
        foreach (var gameObject in gameObjects)
        {
            if (gameObject is Obstacle)
            {
                return gameObject;
            }
        }
        return null;
    }

    public void CheckWallCollision(GameObject wall, GameObject player, GameObject box, Direction playerDirection)
    {
        GameObject playerObject = GetPlayerObject(); // Rename the local variable to 'playerObject'
        GameObject boxObject = GetBoxObject(); // Rename the local variable to 'boxObject'

        foreach (GameObject obj in gameObjects)
        {
            if (obj.Type == GameObjectType.Obstacle)
            {
                wall = obj;

                switch (playerDirection)
                {
                    case Direction.Up:
                        if (playerObject.PosY == wall.PosY && playerObject.PosX == wall.PosX)
                        {
                            playerObject.PosY++;
                        }
                        else if (boxObject.PosY == wall.PosY && boxObject.PosX == wall.PosX)
                        {
                            boxObject.PosY++;
                            playerObject.PosY++;
                        }
                        break;
                    case Direction.Down:
                        if (playerObject.PosY == wall.PosY && playerObject.PosX == wall.PosX)
                        {
                            playerObject.PosY--;
                        }
                        else if (boxObject.PosY == wall.PosY && boxObject.PosX == wall.PosX)
                        {
                            boxObject.PosY--;
                            playerObject.PosY--;
                        }

                        break;
                    case Direction.Left:
                        if (playerObject.PosX == wall.PosX && playerObject.PosY == wall.PosY)
                        {
                            playerObject.PosX++;
                        }
                        else if (boxObject.PosX == wall.PosX && boxObject.PosY == wall.PosY)
                        {
                            boxObject.PosX++;
                            playerObject.PosX++;
                        }
                        break;
                    case Direction.Right:
                        if (playerObject.PosX == wall.PosX && playerObject.PosY == wall.PosY)
                        {
                            playerObject.PosX--;
                        }
                        else if (boxObject.PosX == wall.PosX && boxObject.PosY == wall.PosY)
                        {
                            boxObject.PosX--;
                            playerObject.PosX--;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
    public void Setup()
    {

        //Added for proper display of game characters
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        dynamic gameData = FileHandler.ReadJson();

        map.MapWidth = gameData.map.width;
        map.MapHeight = gameData.map.height;

        foreach (var gameObject in gameData.gameObjects)
        {
            GameObject newObj = new GameObject();
            int type = (int)gameObject.Type;

            AddGameObject(CreateGameObject(gameObject));
        }

        _focusedObject = gameObjects.OfType<Player>().First();

    }

    public bool finishLevel(GameObject box, GameObject goal)
    {
        // Check if the box is on the goal
        if (box.PosX == goal.PosX && box.PosY == goal.PosY)
        {
            Console.WriteLine("Level finished!");
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Render()
    {
        //Clean the map
        Console.Clear();

        map.Initialize();

        PlaceGameObjects();
        GameObject box = GetBoxObject();
        GameObject goal = GetGoalObject();
        GameObject player = GetPlayerObject();

        Console.WriteLine(map);

        Console.WriteLine("Position Box: (" + box.PosX + ", " + box.PosY + ")");
        Console.WriteLine("Position Player: (" + player.PosX + ", " + player.PosY + ")");

        if (!finishLevel(box, goal))
        {

            //Render the map
            for (int i = 0; i < map.MapHeight; i++)
            {
                for (int j = 0; j < map.MapWidth; j++)
                {
                    DrawObject(map.Get(i, j));
                }
                Console.WriteLine();
            }
        }
        else
        {
            Console.WriteLine("Level finished!");
        }
    }



    // Method to create GameObject using the factory from clients
    public GameObject CreateGameObject(dynamic obj)
    {
        return gameObjectFactory.CreateGameObject(obj);
    }

    public void AddGameObject(GameObject gameObject)
    {
        gameObjects.Add(gameObject);
    }

    private void PlaceGameObjects()
    {

        gameObjects.ForEach(delegate (GameObject obj)
        {
            map.Set(obj);
        });
    }

    private void DrawObject(GameObject gameObject)
    {

        Console.ResetColor();

        if (gameObject != null)
        {
            Console.ForegroundColor = gameObject.Color;
            Console.Write(gameObject.CharRepresentation);
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(' ');
        }
    }

    // Save MapState

    private Stack<MapState> stateHistory = new Stack<MapState>();

    public void SaveState()
    {
        stateHistory.Push(new MapState(map));
    }

    public void Rewind()
    {
        if (stateHistory.Count > 1) // Ensure there is a previous state to revert to
        {
            Console.WriteLine("Current top state before rewind: " + stateHistory.Peek());
            stateHistory.Pop(); // Remove current state
            var previousState = stateHistory.Peek();
            Console.WriteLine("Restoring to state: " + previousState);
            map.RestoreState(previousState);
            Render();
        }
        else
        {
            Console.WriteLine("Not enough states in history to rewind.");
        }
    }

}
