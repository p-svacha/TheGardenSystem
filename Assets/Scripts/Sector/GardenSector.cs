using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A garden sector represents an area as a set of owned tiles with an own shed and inventory.
/// </summary>
public class GardenSector
{
    public string Name { get; private set; }
    public MapTile ShedTile;
    public List<MapTile> Tiles;
    public List<Object> Objects;

    public GardenSector(string name, MapTile shedTile)
    {
        Name = name;
        ShedTile = shedTile;
        Tiles = new List<MapTile>();
        Objects = new List<Object>();
    }

    #region Actions

    public void Scatter()
    {
        List<Object> remainingObjects = new List<Object>(Objects);
        List<MapTile> shuffledGardenTiles = EmptyTiles.GetShuffledList();

        foreach (MapTile tile in shuffledGardenTiles)
        {
            if (remainingObjects.Count == 0) break;

            Object pickedObject = remainingObjects.RandomElement();
            Game.Instance.PlaceObject(pickedObject, tile);
            remainingObjects.Remove(pickedObject);
        }
    }

    public void AddObject(Object obj)
    {
        Objects.Add(obj);
    }

    public void RemoveObject(Object obj)
    {
        Objects.Remove(obj);
    }

    public void AddTile(MapTile tile)
    {
        if (Tiles.Contains(tile)) return;
        Tiles.Add(tile);
    }

    public void RemoveTile(MapTile tile)
    {
        Tiles.Remove(tile);
    }

    #endregion

    #region Getters

    public int NumTiles => Tiles.Count;
    public int NumObjects => Objects.Count;
    public List<MapTile> EmptyTiles => Tiles.Where(t => !t.HasObject).ToList();

    #endregion
}
