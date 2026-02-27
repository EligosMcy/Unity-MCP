using System;
using UnityEngine;

[Serializable]
public class BlockData
{
    public int x;
    public int y;
    public int z;
    public MaterialType materialType;
    public Color color;

    public BlockData(int x, int y, int z, MaterialType materialType, Color color)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.materialType = materialType;
        this.color = color;
    }
}