using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Level {

    public string levelName;

    public List<TerrainTile> terrainTileList;
    public List<PuzzleObject> puzzleObjectList;
    public List<Character> characterList;
    public List<WireData> wireList;

    public int iPressurePlates; //how many pressure plates the player is given for this level, etc...
    public int iRemoteExplosives;
    public Level(string name)
    {
        levelName = name;
        terrainTileList = new List<TerrainTile>();
        puzzleObjectList = new List<PuzzleObject>();
        characterList = new List<Character>();
        wireList = new List<WireData>();
        iPressurePlates = 0;
        iRemoteExplosives = 0;
    }

    public void AddTile(GameObject newTile)
    {
        TerrainTile bufferTile = new TerrainTile(newTile.name, new Vector2(newTile.transform.position.x, newTile.transform.position.y));
        terrainTileList.Add(bufferTile);
    }

    public void AddPuzzleObject(GameObject newPuzzleObject)
    {
        PuzzleObject bufferObject = new PuzzleObject(newPuzzleObject.name, new Vector2(newPuzzleObject.transform.position.x, newPuzzleObject.transform.position.y));
        puzzleObjectList.Add(bufferObject);
    }

    public void AddCharacter(GameObject newCharacter)
    {
        Character bufferCharacter = new Character(newCharacter.name, new Vector2(newCharacter.transform.position.x, newCharacter.transform.position.y));
        characterList.Add(bufferCharacter);
    }

    public void AddWire(GameObject newWire)
    {
        Wire bufferWire = newWire.GetComponent<Wire>();
        WireData bufferWireData = new WireData(bufferWire.origin, bufferWire.destination, bufferWire.bSelectable);
        wireList.Add(bufferWireData);
    }


    public string GetName()
    {
        return levelName;
    }

    public List<TerrainTile> GetTileList()
    {
        return terrainTileList;
    }
    public List<PuzzleObject> GetPuzzleObjectList()
    {
        return puzzleObjectList;
    }

    public List<Character> GetCharacterList()
    {
        return characterList;
    }

    public List<WireData> GetWireList()
    {
        return wireList;
    }
}
