using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Nodes;

namespace libs;

using System.Security.Cryptography;
using Newtonsoft.Json;

public sealed class GameEngine
{
    private static GameEngine? _instance;
    private IGameObjectFactory gameObjectFactory;

    public static GameEngine Instance {
        get{
            if(_instance == null)
            {
                _instance = new GameEngine();
            }
            return _instance;
        }
    }

    private GameEngine() {
        //INIT PROPS HERE IF NEEDED
        gameObjectFactory = new GameObjectFactory();
    }

    private GameObject? _focusedObject;

    private Map map = new Map();

    private List<GameObject> gameObjects = new List<GameObject>();


    public void SaveGame(string filePath)
    {
        var gameState = new
        {
            MapWidth = map.MapWidth,
            MapHeight = map.MapHeight,
            Player = new { X = GetPlayerObject().PosX, Y = GetPlayerObject().PosY },
            Boxes = gameObjects.OfType<Box>().Select(b => new { X = b.PosX, Y = b.PosY }).ToList(),
            Goals = gameObjects.OfType<Goal>().Select(g => new { X = g.PosX, Y = g.PosY }).ToList(), // Ensure Goals are saved
            Obstacles = gameObjects.OfType<Obstacle>().Select(o => new { X = o.PosX, Y = o.PosY }).ToList()
        };

        string json = JsonConvert.SerializeObject(gameState, Formatting.Indented);
        File.WriteAllText(filePath, json);
    }

    public void LoadGame(string filePath)
    {
        string json = File.ReadAllText(filePath);
        dynamic gameState = JsonConvert.DeserializeObject<dynamic>(json);

        map.MapWidth = gameState.MapWidth;
        map.MapHeight = gameState.MapHeight;
        // map.Initialize()

        gameObjects.Clear(); // Clear existing game objects
        AddGameObject(new Player { PosX = gameState.Player.X, PosY = gameState.Player.Y });

        foreach (var box in gameState.Boxes)
        {
            AddGameObject(new Box { PosX = box.X, PosY = box.Y });
        }

        foreach (var goal in gameState.Goals) // Ensure Goals are reconstructed
        {
            AddGameObject(new Goal { PosX = goal.X, PosY = goal.Y });
        }

        foreach (var obstacle in gameState.Obstacles)
        {
            AddGameObject(new Obstacle { PosX = obstacle.X, PosY = obstacle.Y });
        }

        // Optionally reset the focused object and other necessary states
        _focusedObject = gameObjects.OfType<Player>().First();
    }

    public Map GetMap() {
        return map;
    }

    public GameObject? GetFocusedObject(){
        return _focusedObject;
    }

    public GameObject GetPlayerObject(){
        foreach (var gameObject in gameObjects)
        {
            if (gameObject is Player)
            {
                return gameObject;
            }
        }
        return null;
    }

  
    public GameObject GetBox(){
        foreach (var gameObject in gameObjects)
        {
            if(gameObject is Box){
                return gameObject;
            }
        }
        return null;
    }

    public List<GameObject> GetBoxObjects(){
        List<GameObject> boxObjects = new List<GameObject>();

        foreach (var gameObject in gameObjects){
            if (gameObject is Box){
                boxObjects.Add(gameObject);
            }
        }

        return boxObjects;
}

    public GameObject GetGoalObject(){
        foreach (var gameObject in gameObjects)
        {
            if (gameObject is Goal)
            {
                return gameObject;
            }
        }
        return null;
    }
    public GameObject GetWallObject(){
        foreach (var gameObject in gameObjects)
        {
            if (gameObject is Obstacle)
            {
                return gameObject;
            }
        }
        return null;
    }

    public void CanMoveBox(GameObject wall, GameObject player, GameObject box, Direction playerdirection){

        GameObject playerObj = GetPlayerObject();
        GameObject boxObj = GetBox();

        foreach (GameObject obj in gameObjects){
            if(obj is Obstacle){
                wall = obj;
                    switch (playerdirection){
                        case Direction.Up:
                            if(playerObj.PosX == wall.PosX && playerObj.PosY == wall.PosY){
                                playerObj.PosY++;
                            }
                            else if(boxObj.PosX == wall.PosX && boxObj.PosY == wall.PosY){
                                playerObj.PosY++;
                                boxObj.PosY++;
                            }
                            break;
                        case Direction.Down:
                            if(playerObj.PosX == wall.PosX && playerObj.PosY == wall.PosY){
                                playerObj.PosY--;
                            }
                            else if(boxObj.PosX == wall.PosX && boxObj.PosY == wall.PosY){
                                playerObj.PosY--;
                                boxObj.PosY--;
                            }
                            break;
                        case Direction.Left:
                            if(playerObj.PosX == wall.PosX && playerObj.PosY == wall.PosY){
                                playerObj.PosX++;
                            }
                            else if(boxObj.PosX == wall.PosX && boxObj.PosY == wall.PosY){
                                playerObj.PosX++;
                                boxObj.PosX++;
                            }
                            break;
                        case Direction.Right:
                            if(playerObj.PosX == wall.PosX && playerObj.PosY == wall.PosY){
                                playerObj.PosX--;
                            }
                            else if(boxObj.PosX == wall.PosX && boxObj.PosY == wall.PosY){
                                playerObj.PosX--;
                                boxObj.PosX--;
                            }
                            break;
                        default:
                            break;
                    }
            }
        }             
    }


    public void Setup(){

        //Added for proper display of game characters
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        dynamic gameData = FileHandler.ReadJson();
        
        map.MapWidth = gameData.map.width;
        map.MapHeight = gameData.map.height;

        foreach (var gameObject in gameData.gameObjects)
        {
            AddGameObject(CreateGameObject(gameObject));
        }
        
        _focusedObject = gameObjects.OfType<Player>().First();

    }

    public bool finishLevel(GameObject box, GameObject goal)
    {
        // Check if either the box or goal is null before attempting to access their properties
        if (box == null)
        {
            Console.WriteLine("Error: The box object is null.");
            return false; // Return false to indicate that the level cannot be finished due to error
        }
        if (goal == null)
        {
            Console.WriteLine("Error: The goal object is null.");
            return false; // Return false to indicate that the level cannot be finished due to error
        }

        // Check if the box is on the goal
        if (box.PosX == goal.PosX && box.PosY == goal.PosY)
        {
            Console.WriteLine("Level finished!");
            return true; // Return true to indicate that the level has been successfully finished
        }
        else
        {
            return false; // Return false to indicate that the level is not yet finished
        }
    }


    public void Render() {
        
        //Clean the map
        Console.Clear();

        map.Initialize();

        PlaceGameObjects();
        GameObject box = GetBox();
        GameObject goal = GetGoalObject();
        GameObject player = GetPlayerObject();

        if (finishLevel(box, goal))
                {
                    
                    //Render the map
                        Console.WriteLine("Level finished!");

                }
            else {
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
    }
    
    // Method to create GameObject using the factory from clients
    public GameObject CreateGameObject(dynamic obj)
    {
        return gameObjectFactory.CreateGameObject(obj);
    }

    public void AddGameObject(GameObject gameObject){
        gameObjects.Add(gameObject);
    }

    private void PlaceGameObjects(){
        
        gameObjects.ForEach(delegate(GameObject obj)
        {
            map.Set(obj);
        });
    }

    private void DrawObject(GameObject gameObject){
        
        Console.ResetColor();

        if(gameObject != null)
        {
            Console.ForegroundColor = gameObject.Color;
            Console.Write(gameObject.CharRepresentation);
        }
        else{
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(' ');
        }
    }

    public bool CanMove(GameObject player, GameObject box, int dx, int dy){

        int newPosX = player.PosX + dx;
        int newPosY = player.PosY + dy;

        int newBoxPosX = box.PosX + dx;
        int newBoxPosY = box.PosY + dy;

        if (newPosX < 0 || newPosX >= map.MapWidth || newPosY < 0 || newPosY >= map.MapHeight || newBoxPosX < 0 || newBoxPosX >= map.MapWidth || newBoxPosY < 0 || newBoxPosY >= map.MapHeight)
        {
            Console.WriteLine("Out of bounds");
            return false;
        }

        GameObject gameObject = map.Get(newPosY, newPosX);

        if (gameObject is Obstacle )
        {
            Console.WriteLine("Obstacle or wall");
            return false;
        }


        return true;
    }
}