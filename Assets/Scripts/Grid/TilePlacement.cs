using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TilePlacement
{
    public static GameObject GetValidTile(ElementType element, TileNeighbours neighbours)
    {
        GameObject tile;
        string elementFolderName;

        if(element == ElementType.STEAM)
        {
            elementFolderName = "Prefabs/Tiles/Steam/SteamTile";
            tile = Resources.Load<GameObject>(elementFolderName);
            return tile;
        }

        switch (element)
        {
            case ElementType.FIRE:
            default:
                elementFolderName = "Prefabs/Tiles/Fire/FireTile";
                break;
            case ElementType.ICE:
                elementFolderName = "Prefabs/Tiles/Ice/IceTile";
                break;
            case ElementType.WATER:
                elementFolderName = "Prefabs/Tiles/Water/WaterTile";
                break;
        }
        if(neighbours.up)
        {
            if (neighbours.down)
            {
                if (neighbours.left)
                {
                    if (neighbours.right)
                    {
                        tile = Resources.Load<GameObject>(elementFolderName + "_M_M");
                    }
                    else
                    {
                        tile = Resources.Load<GameObject>(elementFolderName + "_M_R");
                    }
                }
                else
                {
                    if (neighbours.right)
                    {
                        tile = Resources.Load<GameObject>(elementFolderName + "_M_L");
                    }
                    else
                    {
                        tile = Resources.Load<GameObject>(elementFolderName + "_M_L_C");
                    }
                }
            }
            else
            {
                if (neighbours.left)
                {
                    if (neighbours.right)
                    {
                        tile = Resources.Load<GameObject>(elementFolderName + "_B_M");
                    }
                    else
                    {
                        tile = Resources.Load<GameObject>(elementFolderName + "_B_R");
                    }
                }
                else
                {
                    if (neighbours.right)
                    {
                        tile = Resources.Load<GameObject>(elementFolderName + "_B_L");
                    }
                    else
                    {
                        tile = Resources.Load<GameObject>(elementFolderName + "_B_L_C");
                    }
                }
            }
        }
        else
        {
            if (neighbours.down)
            {
                if (neighbours.left)
                {
                    if (neighbours.right)
                    {
                        tile = Resources.Load<GameObject>(elementFolderName + "_T_M");
                    }
                    else
                    {
                        tile = Resources.Load<GameObject>(elementFolderName + "_T_R");
                    }
                }
                else
                {
                    if (neighbours.right)
                    {
                        tile = Resources.Load<GameObject>(elementFolderName + "_T_L");
                    }
                    else
                    {
                        tile = Resources.Load<GameObject>(elementFolderName + "_T_L_C");
                    }
                }
            }
            else
            {
                if (neighbours.left)
                {
                    if (neighbours.right)
                    {
                        tile = Resources.Load<GameObject>(elementFolderName + "_B_M_C");//!!!
                    }
                    else
                    {
                        tile = Resources.Load<GameObject>(elementFolderName + "_B_R_C");
                    }
                }
                else
                {
                    if (neighbours.right)
                    {
                        tile = Resources.Load<GameObject>(elementFolderName + "_B_L_C_C");
                    }
                    else
                    {
                        tile = Resources.Load<GameObject>(elementFolderName + "");
                    }
                }
            }
        }
        return tile;
    }
}
