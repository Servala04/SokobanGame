using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Nodes;

namespace libs;

using System.Security.Cryptography;
using Newtonsoft.Json;

// Singleton class that manages the game state
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
public GameObject GetPlayer(){
    foreach (var gameObject in gameObjects)
    {
        if(gameObject is Player){
            return gameObject;
        }
    }
     return null;
}
public GameObject GetGoal(){
    foreach (var gameObject in gameObjects)
    {
        if(gameObject is Goal){
            return gameObject;
        }
    }
     return null;
}
public GameObject GetWall(){
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
    

   GameObject playerObj = GetPlayer();
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

    public void Render() {
        
        //Clean the map
        Console.Clear();

        map.Initialize();

        PlaceGameObjects();
        GameObject box = GetBox();
        GameObject goal = GetGoal();
        GameObject player = GetPlayer();
      
    	    //Console.WriteLine("Position Box: (" + box.PosX + ", " + box.PosY + ")");
              //  Console.WriteLine("Position Player: (" + player.PosX + ", " + player.PosY + ")");
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
      else {
        Console.WriteLine("Level finished!");
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
  

public bool CanMove(GameObject player,GameObject box, int dx, int dy)
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


    return true;
}
 
        
        public void UndoMove(GameObject player, GameObject box){
 
        
            // if (steps.Count > 0)
            // {
            //     steps.RemoveAt(steps.Count - 1);
            //     for (int i = 0; i < GameLevel.Length; i++)
            //     {
            //         PlayingLevel[i] = GameLevel[i];
            //     }
            //     getPlayerPosition();
            //     foreach (Direction s in steps)
            //     {
            //         this.Move(s);
            //     }
                
            
        
}

                }
