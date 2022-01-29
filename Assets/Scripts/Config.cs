using System;

[Serializable]
public class CityConfig
{
    public ushort cityWidth;
    public ushort cityLength;
    public float cellSize;
    public BuildingConfig[] buildingTypes;
}

[Serializable]
public class BuildingConfig
{
    public ushort width;
    public ushort length;
    public ushort height;
    public uint power;
}
