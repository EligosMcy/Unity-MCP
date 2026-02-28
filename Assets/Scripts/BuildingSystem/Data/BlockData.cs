using System;
using UnityEngine;

[Serializable]
public class BlockData
{
    public int X;
    public int Y;
    public int Z;
    public MaterialType MaterialType;
    public Color Color;

    public BlockData(int x, int y, int z, MaterialType materialType, Color color)
    {
        X = x;
        Y = y;
        Z = z;
        MaterialType = materialType;
        Color = color;
    }
}