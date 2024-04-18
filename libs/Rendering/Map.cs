namespace libs;
using Newtonsoft.Json;

public class Map {
    private char[,] RepresentationalLayer;
    private GameObject?[,] GameObjectLayer;

    private int _mapWidth;
    private int _mapHeight;

    public Map () {
        _mapWidth = 30;
        _mapHeight = 8;
        RepresentationalLayer = new char[_mapHeight, _mapWidth];
        GameObjectLayer = new GameObject[_mapHeight, _mapWidth];
    }

    public Map (int width, int height) {
        _mapWidth = width;
        _mapHeight = height;
        RepresentationalLayer = new char[_mapHeight, _mapWidth];
        GameObjectLayer = new GameObject[_mapHeight, _mapWidth];
    }

    public void Initialize()
    {
        RepresentationalLayer = new char[_mapHeight, _mapWidth];
        GameObjectLayer = new GameObject[_mapHeight, _mapWidth];

        // Initialize the map with some default values
        for (int i = 0; i < GameObjectLayer.GetLength(0); i++)
        {
            for (int j = 0; j < GameObjectLayer.GetLength(1); j++)
            {
                GameObjectLayer[i, j] = new Floor();
            }
        }
    }

    public int MapWidth
    {
        get { return _mapWidth; } // Getter
        set { _mapWidth = value; Initialize();} // Setter
    }

    public int MapHeight
    {
        get { return _mapHeight; } // Getter
        set { _mapHeight = value; Initialize();} // Setter
    }

    public GameObject Get(int x, int y){
        return GameObjectLayer[x, y];
    }

    public void Set(GameObject gameObject){
        int posY = gameObject.PosY;
        int posX = gameObject.PosX;
        int prevPosY = gameObject.GetPrevPosY();
        int prevPosX = gameObject.GetPrevPosX();
        
        if (prevPosX >= 0 && prevPosX < _mapWidth &&
                prevPosY >= 0 && prevPosY < _mapHeight)
        {
            GameObjectLayer[prevPosY, prevPosX] = new Floor();
        }

        if (posX >= 0 && posX < _mapWidth &&
                posY >= 0 && posY < _mapHeight)
        {
            GameObjectLayer[posY, posX] = gameObject;
            RepresentationalLayer[gameObject.PosY, gameObject.PosX] = gameObject.CharRepresentation;
        }
    }

    public GameObject?[,] GetGameObjectLayer() {
        return GameObjectLayer.Clone() as GameObject?[,];
    }

    // Method to set a specific position, used by RestoreState
    public void SetGameObjectAt(int x, int y, GameObject gameObject) {
        if (x >= 0 && x < _mapWidth && y >= 0 && y < _mapHeight) {
            GameObjectLayer[x, y] = gameObject;
        }
    }

    public void RestoreState(MapState state) {
        var stateGameObjects = state.GetGameObjects();
        for (int i = 0; i < MapHeight; i++) {
            for (int j = 0; j < MapWidth; j++) {
                SetGameObjectAt(i, j, stateGameObjects[i, j]);
            }
        }
    }


}

public class MapState {
    private GameObject[,] GameObjects;

    public MapState(Map map) {
        GameObjects = map.GetGameObjectLayer();
    }

    public GameObject[,] GetGameObjects() {
        return GameObjects;
    }

    public override string ToString()
    {
        // Optionally summarize the state, like counting non-null GameObjects
        int count = GameObjects.Cast<GameObject?>().Count(go => go != null);
        return $"State with {count} active game objects.";
    }
}