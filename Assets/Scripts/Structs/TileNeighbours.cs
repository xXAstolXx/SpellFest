using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TileNeighbours
{
    public bool up;
    public bool down;
    public bool left;
    public bool right;

    public TileNeighbours(bool up, bool down, bool left, bool right)
    {
        this.up = up;
        this.down = down;
        this.left = left;
        this.right = right;
    }
}
