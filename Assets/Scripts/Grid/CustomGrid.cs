using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using System.Linq;
using System.Collections;
using Transform = UnityEngine.Transform;
using UnityEngine.Events;


public class CustomGrid : MonoBehaviour
{
    static CustomGrid instance;
    public static CustomGrid Instance
    {
        get
        {
            return instance;
        }
    }

    public UnityEvent OnElementWaveFinished { get; private set; } = new UnityEvent();

    public Tilemap wallMap;
    public Tilemap groundMap;
    public Tilemap elementMap;
    public Tilemap arenaMap;
    private Vector3 arenaMin;
    private Vector3 arenaMax;

    public Dictionary<Vector3Int, ElementType> elementDict;
    public bool[,] validTileGrid;

    [SerializeField]
    TileObject fireTile;
    [SerializeField]
    TileObject waterTile;
    [SerializeField]
    TileObject steamTile;
    [SerializeField]
    TileObject iceTile;

    List<Vector3Int> neighboursToCheck = new List<Vector3Int>();


    private void Awake()
    {
        instance = this;

        groundMap.CompressBounds();
        wallMap.CompressBounds();
        elementMap.CompressBounds();

        Initialize();

        Debug.Log(arenaMap.localBounds.max);
        arenaMap.CompressBounds();
        Debug.Log(arenaMap.localBounds.max);
        arenaMin = arenaMap.localBounds.min;
        arenaMax = arenaMap.localBounds.max;
    }

    private void Start()
    {
        
    }

    public ElementType GetElement(Vector3Int position)
    {
        return elementDict.GetValueOrDefault(position);
    }

    private void DeleteElementTile(Vector3Int cellPosition)
    {
        elementMap.SetTile(cellPosition, null);
    }

    private void AddElementTile(Vector3Int cellPosition, ElementType element)
    {
        TileObject tileObject = null;
        TileNeighbours neighbours = GetNeighbours(cellPosition, element);

        switch (element)
        {
            case ElementType.FIRE:
                tileObject = fireTile;
                break;
            case ElementType.WATER:
                tileObject = waterTile;
                break;
            case ElementType.ICE:
                tileObject = iceTile;
                break;
            case ElementType.STEAM:
                tileObject = steamTile;
                break;
        }
        tileObject.gameObject = TilePlacement.GetValidTile(element, neighbours);

        elementMap.SetTile(cellPosition, tileObject);
    }

    public void SpawnTiles(List<Vector3Int> targetTiles, ElementType element)
    {
        StartCoroutine(ElementAttackMultiple(targetTiles, element));
    }

    public IEnumerator ElementAttackMultiple(List<Vector3Int> targetTiles, ElementType element)
    {
        neighboursToCheck = new List<Vector3Int>();

        foreach (Vector3Int targetTile in targetTiles)
        {
            Vector3Int cellPosition = targetTile;

            if (elementDict.ContainsKey(cellPosition))
            {
                ElementType elementOnTile = GetElement(cellPosition);
                if (element == elementOnTile)
                {
                    continue;
                }
                ElementType result;
                GlobalSettings.Instance.elementSettings.transformElements.TryGetValue(new ElementTransform(element, elementOnTile), out result);

                elementDict.Remove(cellPosition);
                elementDict.Add(cellPosition, result);

                neighboursToCheck.Add(cellPosition);
                TileNeighbours neighbours = GetNeighbours(cellPosition, result);
                neighboursToCheck.Add(new Vector3Int(cellPosition.x, cellPosition.y + 1));
                neighboursToCheck.Add(new Vector3Int(cellPosition.x, cellPosition.y - 1));
                neighboursToCheck.Add(new Vector3Int(cellPosition.x + 1, cellPosition.y));
                neighboursToCheck.Add(new Vector3Int(cellPosition.x - 1, cellPosition.y));

            }
            else if (groundMap.HasTile(cellPosition) && wallMap.HasTile(cellPosition) == false)
            {
                elementDict.Add(cellPosition, element);

                neighboursToCheck.Add(cellPosition);
                TileNeighbours neighbours = GetNeighbours(cellPosition, element);
                neighboursToCheck.Add(new Vector3Int(cellPosition.x, cellPosition.y + 1));
                neighboursToCheck.Add(new Vector3Int(cellPosition.x, cellPosition.y - 1));
                neighboursToCheck.Add(new Vector3Int(cellPosition.x + 1, cellPosition.y));
                neighboursToCheck.Add(new Vector3Int(cellPosition.x - 1, cellPosition.y));
            }
        }

        List<Vector3Int> tilesToReset = neighboursToCheck.Distinct().ToList();

        foreach (Vector3Int tile in neighboursToCheck)
        {
            if (elementDict.ContainsKey(tile))
            {
                DeleteElementTile(tile);
                AddElementTile(tile, GetElement(tile));
            }
        }

        yield return null;
    }

    private void Initialize()
    {
        elementDict = new Dictionary<Vector3Int, ElementType>();
        int childCount = elementMap.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform tileTransform = elementMap.transform.GetChild(i);
            ElementType element = tileTransform.gameObject.GetComponent<ElementTile>().element;

            Vector3Int cellPosition = new Vector3Int(Mathf.FloorToInt(tileTransform.position.x), Mathf.FloorToInt(tileTransform.position.y), 0);
            elementDict.Add(cellPosition, element);
        }

        for (int i = 0; i < childCount; i++)
        {
            Transform tileTransform = elementMap.transform.GetChild(i);
            ElementType element = tileTransform.gameObject.GetComponent<ElementTile>().element;

            TileObject tileObject = null;

            switch (element)
            {
                case ElementType.FIRE:
                    tileObject = fireTile;
                    break;
                case ElementType.WATER:
                    tileObject = waterTile;
                    break;
                case ElementType.ICE:
                    tileObject = iceTile;
                    break;
            }

            Vector3Int cellPosition = new Vector3Int(Mathf.FloorToInt(tileTransform.position.x), Mathf.FloorToInt(tileTransform.position.y), 0);
            AddElementTile(cellPosition, element);
            Destroy(tileTransform.gameObject);
        }
    }

    TileNeighbours GetNeighbours(Vector3Int position, ElementType element)
    {
        TileNeighbours neighbours = new TileNeighbours();

        neighbours.up = element == GetElement(new Vector3Int(position.x, position.y + 1)) ? true : false;
        neighbours.down = element == GetElement(new Vector3Int(position.x, position.y - 1)) ? true : false;
        neighbours.right = element == GetElement(new Vector3Int(position.x + 1, position.y)) ? true : false;
        neighbours.left = element == GetElement(new Vector3Int(position.x - 1, position.y)) ? true : false;

        return neighbours;
    }

    public void TileWave(Vector3Int startPosition, float timePerRing)
    {
        StartCoroutine(SpawnRing(startPosition, timePerRing));
        Debug.Log("@@@@@@@@ TileWave");
    }

    IEnumerator SpawnRing(Vector3Int startPosition, float duration)
    {
        bool expandUp = true;
        bool expandDown = true;
        bool expandRight = true;
        bool expandLeft = true;

        int ringCount = 0;

        int minXBound = Mathf.FloorToInt(arenaMin.x);
        int maxXBound = Mathf.FloorToInt(arenaMax.x);
        int minYBound = Mathf.FloorToInt(arenaMin.y);
        int maxYBound = Mathf.FloorToInt(arenaMax.y);

        Debug.Log("start position: " + startPosition);

        Debug.Log("minXBound: " + minXBound);
        Debug.Log("maxXBound: " + maxXBound);
        Debug.Log("minYBound: " + minYBound);
        Debug.Log("maxYBound: " + maxYBound);


        while (true)
        {
            ringCount++;
            Debug.Log("ringcount: "+ringCount);
            int leftX = startPosition.x - ringCount;
            int rightX = startPosition.x + ringCount;
            int downY = startPosition.y - ringCount;
            int upY = startPosition.y + ringCount;

            Debug.Log("leftX: " + leftX);
            Debug.Log("rightX: " + rightX);
            Debug.Log("downY: " + downY);
            Debug.Log("upY: " + upY);


            int lineCount = 2 * ringCount - 1;

            Debug.Log("lineCount: " + lineCount);

            // up row
            if (expandUp)
            {
                if (upY <= maxYBound)
                {
                    Debug.Log("upY: " + upY);
                    Debug.Log("maxXBound: " + maxXBound);
                    Debug.Log("expandUp!");

                    for (int i = 1; i <= lineCount; i++)
                    {
                        Vector3Int tile = new Vector3Int(leftX + i, upY);

                        if (arenaMap.HasTile(tile) == true)
                        {
                            TileNeighbours neighbours = new TileNeighbours();
                            TileObject tileObject;

                            ElementType element = GetElement(tile);
                            if (GetElement(tile) == ElementType.ICE)
                            {
                                elementDict.Remove(tile);
                                elementDict.Add(tile, ElementType.WATER);
                                DeleteElementTile(tile);

                                neighbours.up = GetElement(new Vector3Int(tile.x, tile.y + 1)) == ElementType.ICE ? true : false;
                                neighbours.down = GetElement(new Vector3Int(tile.x, tile.y - 1)) == ElementType.WATER ? true : false;
                                neighbours.left = GetElement(new Vector3Int(tile.x - 1, tile.y)) == ElementType.WATER ? true : false;
                                neighbours.right = GetElement(new Vector3Int(tile.x + 1, tile.y)) == ElementType.ICE ? true : false;

                                tileObject = waterTile;
                                tileObject.gameObject = TilePlacement.GetValidTile(ElementType.WATER, neighbours);

                                elementMap.SetTile(tile, tileObject);
                            }
                            else
                            {
                                elementDict.Remove(tile);
                                elementDict.Add(tile, ElementType.FIRE);

                                neighbours.up = GetElement(new Vector3Int(tile.x, tile.y + 1)) == ElementType.FIRE ? true : false;
                                neighbours.down = false;
                                neighbours.left = GetElement(new Vector3Int(tile.x - 1, tile.y)) == ElementType.WATER ? false : true;
                                neighbours.right = GetElement(new Vector3Int(tile.x + 1, tile.y)) == ElementType.ICE ? false : true;

                                tileObject = fireTile;
                                tileObject.gameObject = TilePlacement.GetValidTile(ElementType.FIRE, neighbours);

                                elementMap.SetTile(tile, tileObject);
                            }

                            Vector3Int tileBehind = new Vector3Int(tile.x, tile.y - 1);
                            if(GetElement(tileBehind) == ElementType.FIRE)
                            {
                                elementDict.Remove(tileBehind);
                                elementMap.SetTile(tileBehind, null);
                            }
                        }
                    }
                }
                else
                {
                    expandUp = false;
                }
            }          

            // down row
            if(expandDown)
            {
                if (downY >= minYBound)
                {
                    Debug.Log("downY: " + downY);
                    Debug.Log("minYBound: " + minYBound);
                    Debug.Log("expandDown!");

                    for (int i = 1; i <= lineCount; i++)
                    {
                        Vector3Int tile = new Vector3Int(leftX + i, downY);

                        if (arenaMap.HasTile(tile) == true)
                        {
                            TileNeighbours neighbours = new TileNeighbours();
                            TileObject tileObject;

                            ElementType element = GetElement(tile);
                            if (element == ElementType.ICE)
                            {
                                elementDict.Remove(tile);
                                elementDict.Add(tile, ElementType.WATER);
                                DeleteElementTile(tile);

                                neighbours.up = GetElement(new Vector3Int(tile.x, tile.y + 1)) == ElementType.WATER ? true : false;
                                neighbours.down = GetElement(new Vector3Int(tile.x, tile.y - 1)) == ElementType.ICE ? true : false;
                                neighbours.left = GetElement(new Vector3Int(tile.x - 1, tile.y)) == ElementType.WATER ? true : false;
                                neighbours.right = GetElement(new Vector3Int(tile.x + 1, tile.y)) == ElementType.ICE ? true : false;

                                tileObject = waterTile;
                                tileObject.gameObject = TilePlacement.GetValidTile(ElementType.WATER, neighbours);

                                elementMap.SetTile(tile, tileObject);
                            }
                            else
                            {
                                elementDict.Remove(tile);
                                elementDict.Add(tile, ElementType.FIRE);

                                neighbours.up = false;
                                neighbours.down = GetElement(new Vector3Int(tile.x, tile.y - 1)) == ElementType.FIRE ? true : false;
                                neighbours.left = GetElement(new Vector3Int(tile.x - 1, tile.y)) == ElementType.WATER ? false : true;
                                neighbours.right = GetElement(new Vector3Int(tile.x + 1, tile.y)) == ElementType.ICE ? false : true;

                                tileObject = fireTile;
                                tileObject.gameObject = TilePlacement.GetValidTile(ElementType.FIRE, neighbours);

                                elementMap.SetTile(tile, tileObject);
                            }

                            Vector3Int tileBehind = new Vector3Int(tile.x, tile.y + 1);
                            if (GetElement(tileBehind) == ElementType.FIRE)
                            {
                                elementDict.Remove(tileBehind);
                                elementMap.SetTile(tileBehind, null);
                            }
                        }
                    }
                }
                else
                {
                    expandDown = false;
                }
            }

            // left row
            if(expandLeft) 
            {
                if (leftX >= minXBound)
                {
                    Debug.Log("expandLeft!");

                    for (int i = 1; i <= lineCount; i++)
                    {
                        Vector3Int tile = new Vector3Int(leftX, downY + i);

                        if (arenaMap.HasTile(tile) == true)
                        {
                            TileNeighbours neighbours = new TileNeighbours();
                            TileObject tileObject;

                            ElementType element = GetElement(tile);
                            if (GetElement(tile) == ElementType.ICE)
                            {
                                elementDict.Remove(tile);
                                elementDict.Add(tile, ElementType.WATER);
                                DeleteElementTile(tile);

                                neighbours.up = GetElement(new Vector3Int(tile.x, tile.y + 1)) == ElementType.ICE ? true : false;
                                neighbours.down = GetElement(new Vector3Int(tile.x, tile.y - 1)) == ElementType.WATER ? true : false;
                                neighbours.left = GetElement(new Vector3Int(tile.x - 1, tile.y)) == ElementType.ICE ? true : false;
                                neighbours.right = GetElement(new Vector3Int(tile.x + 1, tile.y)) == ElementType.WATER ? true : false;

                                tileObject = waterTile;
                                tileObject.gameObject = TilePlacement.GetValidTile(ElementType.WATER, neighbours);

                                elementMap.SetTile(tile, tileObject);
                            }
                            else
                            {
                                elementDict.Remove(tile);
                                elementDict.Add(tile, ElementType.FIRE);

                                neighbours.up = GetElement(new Vector3Int(tile.x, tile.y + 1)) == ElementType.ICE ? false : true;
                                neighbours.down = GetElement(new Vector3Int(tile.x, tile.y - 1)) == ElementType.WATER ? false : true;
                                neighbours.left = GetElement(new Vector3Int(tile.x - 1, tile.y)) == ElementType.FIRE ? true : false;
                                neighbours.right = false;

                                tileObject = fireTile;
                                tileObject.gameObject = TilePlacement.GetValidTile(ElementType.FIRE, neighbours);

                                elementMap.SetTile(tile, tileObject);
                            }

                            Vector3Int tileBehind = new Vector3Int(tile.x + 1, tile.y);
                            if (GetElement(tileBehind) == ElementType.FIRE)
                            {
                                elementDict.Remove(tileBehind);
                                elementMap.SetTile(tileBehind, null);
                            }
                        }
                    }
                }
                else
                {
                    expandLeft = false;
                }
            }

            // right row
            if (expandRight)
            {
                if (rightX <= maxXBound)
                {
                    Debug.Log("expandRight!");

                    for (int i = 1; i <= lineCount; i++)
                    {
                        Vector3Int tile = new Vector3Int(rightX, downY + i);

                        if (arenaMap.HasTile(tile) == true)
                        {
                            TileNeighbours neighbours = new TileNeighbours();
                            TileObject tileObject;

                            ElementType element = GetElement(tile);

                            if (GetElement(tile) == ElementType.ICE)
                            {
                                elementDict.Remove(tile);
                                elementDict.Add(tile, ElementType.WATER);
                                DeleteElementTile(tile);

                                neighbours.up = GetElement(new Vector3Int(tile.x, tile.y + 1)) == ElementType.ICE ? true : false;
                                neighbours.down = GetElement(new Vector3Int(tile.x, tile.y - 1)) == ElementType.WATER ? true : false;
                                neighbours.left = GetElement(new Vector3Int(tile.x - 1, tile.y)) == ElementType.ICE ? true : false;
                                neighbours.right = GetElement(new Vector3Int(tile.x + 1, tile.y)) == ElementType.WATER ? true : false;

                                tileObject = waterTile;
                                tileObject.gameObject = TilePlacement.GetValidTile(ElementType.WATER, neighbours);

                                elementMap.SetTile(tile, tileObject);
                            }
                            else
                            {
                                elementDict.Remove(tile);
                                elementDict.Add(tile, ElementType.FIRE);

                                neighbours.up = GetElement(new Vector3Int(tile.x, tile.y + 1)) == ElementType.ICE ? false : true;
                                neighbours.down = GetElement(new Vector3Int(tile.x, tile.y - 1)) == ElementType.WATER ? false : true;
                                neighbours.left = false;
                                neighbours.right = GetElement(new Vector3Int(tile.x + 1, tile.y)) == ElementType.FIRE ? true : false;

                                tileObject = fireTile;
                                tileObject.gameObject = TilePlacement.GetValidTile(ElementType.FIRE, neighbours);

                                elementMap.SetTile(tile, tileObject);
                            }

                            Vector3Int tileBehind = new Vector3Int(tile.x - 1, tile.y);
                            if (GetElement(tileBehind) == ElementType.FIRE)
                            {
                                elementDict.Remove(tileBehind);
                                elementMap.SetTile(tileBehind, null);
                            }
                        }
                    }
                }
                else
                {
                    expandRight = false;
                }
            }

            if(expandLeft && expandUp)
            {
                Vector3Int tile = new Vector3Int(leftX, upY);

                if (arenaMap.HasTile(tile) == true)
                {
                    TileNeighbours neighbours = new TileNeighbours();
                    TileObject tileObject;

                    if (GetElement(tile) == ElementType.ICE)
                    {
                        elementDict.Remove(tile);
                        elementDict.Add(tile, ElementType.WATER);
                        DeleteElementTile(tile);

                        neighbours.up = GetElement(new Vector3Int(tile.x, tile.y + 1)) == ElementType.ICE ? true : false;
                        neighbours.down = GetElement(new Vector3Int(tile.x, tile.y - 1)) == ElementType.WATER ? true : false;
                        neighbours.left = GetElement(new Vector3Int(tile.x - 1, tile.y)) == ElementType.ICE ? true : false;
                        neighbours.right = GetElement(new Vector3Int(tile.x + 1, tile.y)) == ElementType.WATER ? true : false;

                        tileObject = waterTile;
                        tileObject.gameObject = TilePlacement.GetValidTile(ElementType.WATER, neighbours);

                        elementMap.SetTile(tile, tileObject);
                    }
                    else
                    {
                        elementDict.Remove(tile);
                        elementDict.Add(tile, ElementType.FIRE);

                        neighbours.up = GetElement(new Vector3Int(tile.x, tile.y + 1)) == ElementType.FIRE ? true : false;
                        neighbours.down = GetElement(new Vector3Int(tile.x, tile.y - 1)) == ElementType.FIRE ? true : false;
                        neighbours.left = GetElement(new Vector3Int(tile.x - 1, tile.y)) == ElementType.FIRE ? true : false;
                        neighbours.right = GetElement(new Vector3Int(tile.x + 1, tile.y)) == ElementType.FIRE ? true : false;

                        tileObject = fireTile;
                        tileObject.gameObject = TilePlacement.GetValidTile(ElementType.FIRE, neighbours);

                        elementMap.SetTile(tile, tileObject);
                    }
                }
            }

            if (expandRight && expandUp)
            {
                Vector3Int tile = new Vector3Int(rightX, upY);

                if (arenaMap.HasTile(tile) == true)
                {
                    TileNeighbours neighbours = new TileNeighbours();
                    TileObject tileObject;

                    if (GetElement(tile) == ElementType.ICE)
                    {
                        elementDict.Remove(tile);
                        elementDict.Add(tile, ElementType.WATER);
                        DeleteElementTile(tile);

                        neighbours.up = GetElement(new Vector3Int(tile.x, tile.y + 1)) == ElementType.ICE ? true : false;
                        neighbours.down = GetElement(new Vector3Int(tile.x, tile.y - 1)) == ElementType.WATER ? true : false;
                        neighbours.left = GetElement(new Vector3Int(tile.x - 1, tile.y)) == ElementType.WATER ? true : false;
                        neighbours.right = GetElement(new Vector3Int(tile.x + 1, tile.y)) == ElementType.ICE ? true : false;

                        tileObject = waterTile;
                        tileObject.gameObject = TilePlacement.GetValidTile(ElementType.WATER, neighbours);

                        elementMap.SetTile(tile, tileObject);
                    }
                    else
                    {
                        elementDict.Remove(tile);
                        elementDict.Add(tile, ElementType.FIRE);

                        neighbours.up = GetElement(new Vector3Int(tile.x, tile.y + 1)) == ElementType.FIRE ? true : false;
                        neighbours.down = GetElement(new Vector3Int(tile.x, tile.y - 1)) == ElementType.FIRE ? true : false;
                        neighbours.left = GetElement(new Vector3Int(tile.x - 1, tile.y)) == ElementType.FIRE ? true : false;
                        neighbours.right = GetElement(new Vector3Int(tile.x + 1, tile.y)) == ElementType.FIRE ? true : false;

                        tileObject = fireTile;
                        tileObject.gameObject = TilePlacement.GetValidTile(ElementType.FIRE, neighbours);

                        elementMap.SetTile(tile, tileObject);
                    }
                }
            }

            if (expandRight && expandDown)
            {
                Vector3Int tile = new Vector3Int(rightX, downY);

                if (arenaMap.HasTile(tile) == true)
                {
                    TileNeighbours neighbours = new TileNeighbours();
                    TileObject tileObject;

                    if (GetElement(tile) == ElementType.ICE)
                    {
                        elementDict.Remove(tile);
                        elementDict.Add(tile, ElementType.WATER);
                        DeleteElementTile(tile);

                        neighbours.up = GetElement(new Vector3Int(tile.x, tile.y + 1)) == ElementType.WATER ? true : false;
                        neighbours.down = GetElement(new Vector3Int(tile.x, tile.y - 1)) == ElementType.ICE ? true : false;
                        neighbours.left = GetElement(new Vector3Int(tile.x - 1, tile.y)) == ElementType.WATER ? true : false;
                        neighbours.right = GetElement(new Vector3Int(tile.x + 1, tile.y)) == ElementType.ICE ? true : false;

                        tileObject = waterTile;
                        tileObject.gameObject = TilePlacement.GetValidTile(ElementType.WATER, neighbours);

                        elementMap.SetTile(tile, tileObject);
                    }
                    else
                    {
                        elementDict.Remove(tile);
                        elementDict.Add(tile, ElementType.FIRE);

                        neighbours.up = GetElement(new Vector3Int(tile.x, tile.y + 1)) == ElementType.FIRE ? true : false;
                        neighbours.down = GetElement(new Vector3Int(tile.x, tile.y - 1)) == ElementType.FIRE ? true : false;
                        neighbours.left = GetElement(new Vector3Int(tile.x - 1, tile.y)) == ElementType.FIRE ? true : false;
                        neighbours.right = GetElement(new Vector3Int(tile.x + 1, tile.y)) == ElementType.FIRE ? true : false;

                        tileObject = fireTile;
                        tileObject.gameObject = TilePlacement.GetValidTile(ElementType.FIRE, neighbours);

                        elementMap.SetTile(tile, tileObject);
                    }
                }
            }

            if (expandLeft && expandDown)
            {
                Vector3Int tile = new Vector3Int(leftX, downY);
                if (arenaMap.HasTile(tile) == true)
                {
                    TileNeighbours neighbours = new TileNeighbours();
                    TileObject tileObject;

                    if (GetElement(tile) == ElementType.ICE)
                    {
                        elementDict.Remove(tile);
                        elementDict.Add(tile, ElementType.WATER);
                        DeleteElementTile(tile);

                        neighbours.up = GetElement(new Vector3Int(tile.x, tile.y + 1)) == ElementType.WATER ? true : false;
                        neighbours.down = GetElement(new Vector3Int(tile.x, tile.y - 1)) == ElementType.ICE ? true : false;
                        neighbours.left = GetElement(new Vector3Int(tile.x - 1, tile.y)) == ElementType.ICE ? true : false;
                        neighbours.right = GetElement(new Vector3Int(tile.x + 1, tile.y)) == ElementType.WATER ? true : false;

                        tileObject = waterTile;
                        tileObject.gameObject = TilePlacement.GetValidTile(ElementType.WATER, neighbours);

                        elementMap.SetTile(tile, tileObject);
                    }
                    else
                    {
                        elementDict.Remove(tile);
                        elementDict.Add(tile, ElementType.FIRE);

                        neighbours.up = GetElement(new Vector3Int(tile.x, tile.y + 1)) == ElementType.FIRE ? true : false;
                        neighbours.down = GetElement(new Vector3Int(tile.x, tile.y - 1)) == ElementType.FIRE ? true : false;
                        neighbours.left = GetElement(new Vector3Int(tile.x - 1, tile.y)) == ElementType.FIRE ? true : false;
                        neighbours.right = GetElement(new Vector3Int(tile.x + 1, tile.y)) == ElementType.FIRE ? true : false;

                        tileObject = fireTile;
                        tileObject.gameObject = TilePlacement.GetValidTile(ElementType.FIRE, neighbours);

                        elementMap.SetTile(tile, tileObject);
                    }
                }
            }

            if (expandLeft == false && expandRight == false && expandUp == false && expandDown == false)
            {
                //OnElementWaveFinished.Invoke();
                break;
            }

            yield return new WaitForSeconds(duration);
        }
    }
}
