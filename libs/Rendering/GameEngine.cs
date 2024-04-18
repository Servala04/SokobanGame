using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Nodes;
using Newtonsoft.Json;


namespace libs;
using libs.Rendering;
using System.Security.Cryptography;
using Newtonsoft.Json;

// Singleton class that manages the game state
public sealed class GameEngine
{
    private static GameEngine? _instance;
    private IGameObjectFactory gameObjectFactory;
            private Stack<GameState> gameStates;
 private int currentLevelIndex = 0; // Assume the initial level index is 0
    private string[] levelFilePaths = { "level00.json", "level01.json", "level02.json" };


private int moveCount;
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
                    gameStates = new Stack<GameState>();

                moveCount = 0;

    }

    private GameObject? _focusedObject;

    private Map map = new Map();

    private List<GameObject> gameObjects = new List<GameObject>();


    public Map GetMap() {
        return map;
    }

    public GameObject GetFocusedObject(){
        return _focusedObject;
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
public Player GetPlayerObject(){
    foreach (var gameObject in gameObjects)
    {
        if(gameObject is Player){
            return (Player)gameObject;
        }
    }
     return null;
}
public GameObject GetGoalObject(){
    foreach (var gameObject in gameObjects)
    {
        if(gameObject is Goal){
            return gameObject;
        }
    }
     return null;
}
public GameObject GetWallObject(){
    foreach (var gameObject in gameObjects)
    {
        if(gameObject is Obstacle){
            return gameObject;
        }
    }
     return null;
}

public void CanMoveBox(GameObject wall, GameObject player, GameObject box, Direction playerdirection)
{
    

   GameObject playerObj = GetPlayerObject();
   GameObject boxObj = GetBox();

   foreach (GameObject obj in gameObjects){
         if(obj is Obstacle){
            wall = obj;
             switch (playerdirection)
                {
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

    public void LoadLevel(string levelFilePath)
    {
        // Read the level data from the file
        string levelData = File.ReadAllText(levelFilePath);

        // Parse the level data into a dynamic object
        dynamic level = JsonConvert.DeserializeObject(levelData);

        // Clear the existing game objects
        gameObjects.Clear();

        // Set up the map dimensions
        map.MapWidth = level.map.width;
        map.MapHeight = level.map.height;

        // Create and add game objects from the level data
        foreach (var gameObjectData in level.gameObjects)
        {
            AddGameObject(CreateGameObject(gameObjectData));
        }

        // Set the focused object to the player
        _focusedObject = gameObjects.OfType<Player>().FirstOrDefault();
    }
 public bool finishLevel(GameObject box, GameObject goal)
{
    // Check if the box is on the goal
    if (box.PosX == goal.PosX && box.PosY == goal.PosY)
    {
       // Console.WriteLine("Level finished!");

        // Increment the current level index
        currentLevelIndex++;

        // Check if there are more levels to load
        if (currentLevelIndex < levelFilePaths.Length)
        {
            string nextLevelFilePath = Path.Combine("..", "libs", "levels", levelFilePaths[currentLevelIndex]);
            Console.WriteLine($"Loading next level: {nextLevelFilePath}");
            // Load the next level
           LoadLevel(nextLevelFilePath);

        }
        else
        {
            Console.WriteLine("All levels completed!");
        }

        return true;
    }
    else
    {
        return false;
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
      
    	    //Console.WriteLine("Position Box: (" + box.PosX + ", " + box.PosY + ")");
              //  Console.WriteLine("Position Player: (" + player.PosX + ", " + player.PosY + ")");
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
  
 public void AddMoveCount()
    {
        moveCount++;
        
    }

public bool CanMove(GameObject player, GameObject box, int dx, int dy)
{
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

 GameState currentState = new GameState(GetBoxObjects(), GetPlayerObject());
            gameStates.Push(currentState);
    return true;
}
 
  
  

    // public void Move(Direction direction)
    // {
    //     GameObject player = GetPlayer();
    //     GameObject box = GetBox();
    //     int dx = 0;
    //     int dy = 0;

    //     switch (direction)
    //     {
    //         case Direction.Up:
    //             dy = -1;
    //             break;
    //         case Direction.Down:
    //             dy = 1;
    //             break;
    //         case Direction.Left:
    //             dx = -1;
    //             break;
    //         case Direction.Right:
    //             dx = 1;
    //             break;
    //         default:
    //             break;
    //     }

    //     if (CanMove(player, box, dx, dy))
    //     {
    //         player.Move(dx, dy);
    //         box.Move(dx, dy);
    //         AddMoveCount();
    //     }
    // }
    
        
        

    // public void Move(Direction direction)
    // {
    //     GameObject player = GetPlayer();
    //     GameObject box = GetBox();
    //     int dx = 0;
    //     int dy = 0;

    //     switch (direction)
    //     {
    //         case Direction.Up:
    //             dy = -1;
    //             break;
    //         case Direction.Down:
    //             dy = 1;
    //             break;
    //         case Direction.Left:
    //             dx = -1;
    //             break;
    //         case Direction.Right:
    //             dx = 1;
    //             break;
    //         default:
    //             break;
    //     }

    //     if (CanMove(player, box, dx, dy))
    //     {
    //         player.Move(dx, dy);
    //         box.Move(dx, dy);
    //         AddMoveCount();
    //     }
    // }
    
        
     public void UndoMove(Player player, List<GameObject> boxObjects) {
    if (moveCount > 0) {
        // Decrement move count
        moveCount--;
        Console.WriteLine("Move count: " + moveCount);
        
        // Restore previous game state from the stack
        if (gameStates.Count > 0) {
            GameState previousState = gameStates.Pop();
            
            // Restore player position
            player.PosX = previousState.Player.PosX;
            player.PosY = previousState.Player.PosY;
            
            // Restore box positions
            for (int i = 0; i < boxObjects.Count; i++) {
                boxObjects[i].PosX = previousState.BoxObjects[i].PosX;
                boxObjects[i].PosY = previousState.BoxObjects[i].PosY;
            }
            
            // Render the game with the restored state
            Render();
        } else {
            Console.WriteLine("No moves to undo");
        }
    }
}


        
}

                
