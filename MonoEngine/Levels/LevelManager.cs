using System.Collections.Generic;
using System.Linq;
using MonoEngine.Levels.Objects;
using MonoLibrary.Graphics.Tilemaps;
using Microsoft.Xna.Framework;
using MonoLibrary.Structures;

namespace MonoEngine.Levels;
public class LevelManager(List<LevelObject> objs)
{
    public List<LevelObject> Objects { get; } = objs;

    public List<SolidObject<Polygon>> Solids => [.. Objects.OfType<SolidObject<Polygon>>()];
    public List<HazardObject<Polygon>> Hazards => [.. Objects.OfType<HazardObject<Polygon>>()];
    public List<TriggerObject> Triggers => [.. Objects.OfType<TriggerObject>()];


    // public static LevelManager FromFile(ContentManager content, string filename)
    // {
    //     string filePath = Path.Combine(content.RootDirectory, filename);

    //     List<LevelObject<Polygon>> objs = [];

    //     using (Stream stream = TitleContainer.OpenStream(filePath))
    //     {
    //         LevelData levelData = JsonSerializer.Deserialize<LevelData>(stream);

    //         objs.AddRange(ParseObjectType(levelData.solids, (position, dimensions) => new BlockSolid(position, dimensions)));
    //         objs.AddRange(ParseObjectType(levelData.hazards, (position) => new HazardObject(position)));
    //         objs.AddRange(ParseTriggers(levelData.triggers));
    //     }

    //     return new LevelManager(objs);

    //     static List<SolidObject<Quadrilateral>> ParseObjectType(List<LevelData.ObjectData> objects, Func<Vector2, Vector2, SolidObject<Polygon>> CreateObject)
    //     {
    //         List<SolidObject<Quadrilateral>> colliders = [];

    //         foreach (LevelData.ObjectData obj in objects)
    //         {
    //             Vector2 position = new(obj.x, obj.y);
    //             Vector2 dimensions = new(obj.width, obj.height);
    //             var collider = new BlockSolid(position, dimensions);
    //             colliders.Add(collider);
    //         }

    //         return colliders;
    //     }

    //     static List<Interactable> ParseTriggers(List<LevelData.TriggerData> triggers)
    //     {
    //         List<Interactable> colliders = [];

    //         foreach (LevelData.TriggerData trigger in triggers)
    //         {
    //             Vector2 position = new(trigger.x, trigger.y);
    //             Vector2 dimensions = new(trigger.width, trigger.height);
    //             var collider = TriggerObject.Build(position, dimensions, trigger.type);
    //             colliders.Add(collider);
    //         }

    //         return colliders;
    //     }
    // }

    /// <summary>
    /// Constructs a Level with solid objects corresponding to a given tilemap manager.
    /// </summary>
    /// <param name="tilemap"></param>
    /// <returns>The constructed level.</returns>
    public static LevelManager FromTilemap(TilemapManager tilemapManager)
    {
        List<LevelObject> colliders = [];
        foreach (Tilemap tilemap in tilemapManager.TileLayers)
        {
            if (tilemap.LayerType != LayerType.Base) continue;
            colliders.AddRange(FromTilemap(tilemap));
        }

        return new LevelManager(colliders);
    }

    /// <summary>
    /// Constructs a list of solid objects corresponding to a given tilemap.
    /// </summary>
    /// <param name="tilemap"></param>
    /// <returns>The constructed level.</returns>
    public static List<LevelObject> FromTilemap(Tilemap tilemap)
    {
        List<LevelObject> colliders = [];

        bool[,] visitedTiles = new bool[tilemap.Rows, tilemap.Columns];
        bool IsInvalidTilePosition(int X, int Y)
        {
            return visitedTiles[X, Y] || tilemap.GetTilesetID(X, Y) == -1;
        }

        for (int i = 0; i < tilemap.Rows; i++)
        {
            for (int j = 0; j < tilemap.Columns; j++)
            {
                // pass over visited tiles and air tiles
                if (IsInvalidTilePosition(i, j)) continue;

                // visit the current tile
                visitedTiles[i, j] = true;
                int rectangleWidth = 0;
                int rectangleHeight = 0;

                // maximise width of rectangle within non-air tiles
                while (true)
                {
                    rectangleWidth++;
                    int column = j + rectangleWidth;

                    // validation checks
                    if (column >= tilemap.Columns) break;
                    if (IsInvalidTilePosition(i, column)) break;

                    visitedTiles[i, column] = true;
                }

                bool outsideTiles = false;
                while (!outsideTiles)
                {
                    rectangleHeight++;
                    int row = i + rectangleHeight;

                    // validation checks
                    if (row >= tilemap.Rows) break;
                    for (int k = 0; k < rectangleWidth; k++)
                    {
                        if (IsInvalidTilePosition(i + rectangleHeight, j + k))
                        {
                            outsideTiles = true;
                            break;
                        }
                    }

                    // add current row to rectangle
                    if (outsideTiles) continue;
                    for (int k = 0; k < rectangleWidth; k++)
                    {
                        visitedTiles[i + rectangleHeight, j + k] = true;
                    }
                }

                // generate object and hitbox
                Vector2 position = new(j * tilemap.TileWidth, i * tilemap.TileHeight);
                Vector2 dimensions = new((int)tilemap.TileWidth * rectangleWidth, (int)tilemap.TileHeight * rectangleHeight);
                LevelObject collider = new BlockSolid(position, dimensions);
                colliders.Add(collider);
            }
        }

        return colliders;
    }



    private class LevelData
    {
        public required List<ObjectData> Solids { get; init; }
        public required List<ObjectData> Hazards { get; init; }
        public required List<TriggerData> Triggers { get; init; }
        public class ObjectData
        {
            public required int X { get; init; }
            public required int Y { get; init; }
            public required int Width  { get; init; }
            public required int Height  { get; init; }
        }
        public class TriggerData : ObjectData
        {
            public required TriggerType Type { get; init; }
        }
    }
}
