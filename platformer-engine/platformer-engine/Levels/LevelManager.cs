using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Levels.Objects;
using lib.Graphics.Textures;
using lib.Graphics.Tilemaps;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Levels;
public class LevelManager(List<Interactable> objs)
{
    public List<Interactable> Objects { get; } = objs;

    public List<SolidObject> Solids => [.. Objects.OfType<SolidObject>()];
    public List<HazardObject> Hazards => [.. Objects.OfType<HazardObject>()];
    public List<TriggerObject> Triggers => [.. Objects.OfType<TriggerObject>()];


    public static LevelManager FromFile(ContentManager content, string filename)
    {
        string filePath = Path.Combine(content.RootDirectory, filename);

        List<Interactable> objs = [];

        using (Stream stream = TitleContainer.OpenStream(filePath))
        {
            LevelData levelData = JsonSerializer.Deserialize<LevelData>(stream);

            objs.AddRange(ParseObjectType(levelData.solids, (position) => new SolidObject(position)));
            objs.AddRange(ParseObjectType(levelData.hazards, (position) => new HazardObject(position)));
            objs.AddRange(ParseTriggers(levelData.triggers));
        }

        return new LevelManager(objs);

        static List<Interactable> ParseObjectType(List<LevelData.ObjectData> objects, Func<Vector2, Interactable> CreateObject)
        {
            List<Interactable> colliders = [];

            foreach (LevelData.ObjectData obj in objects)
            {
                Vector2 position = new(obj.x, obj.y);
                var collider = CreateObject(position);
                collider.GenerateHitbox(obj.width, obj.height);
                colliders.Add(collider);
            }

            return colliders;
        }

        static List<Interactable> ParseTriggers(List<LevelData.TriggerData> triggers)
        {
            List<Interactable> colliders = [];

            foreach (LevelData.TriggerData trigger in triggers)
            {
                Vector2 position = new(trigger.x, trigger.y);
                var collider = new TriggerObject(position, trigger.type);
                collider.GenerateHitbox(trigger.width, trigger.height);
                colliders.Add(collider);
            }

            return colliders;
        }
    }

    public static LevelManager FromTilemap(TilemapManager tilemap)
    {
        List<Interactable> colliders = [];
        int tileCount = tilemap.TileLayers[0].Count;

        for (int i = 0; i < tilemap.TileLayers[0].Rows; i++)
        {
            for (int j = 0; j < tilemap.TileLayers[0].Columns; j++)
            {
                if (tilemap.TileLayers[0].GetTilesetID(i, j) == -1) continue;

                Vector2 position = new(j * tilemap.TileLayers[0].TileWidth, i * tilemap.TileLayers[0].TileHeight);
                var collider = new SolidObject(position);
                collider.GenerateHitbox((int)tilemap.TileLayers[0].TileWidth, (int)tilemap.TileLayers[0].TileHeight);
                colliders.Add(collider);
            }
        }

        return new LevelManager(colliders);
    }

    private class LevelData
    {
        public required List<ObjectData> solids { get; init; }
        public required List<ObjectData> hazards { get; init; }
        public required List<TriggerData> triggers { get; init; }
        public class ObjectData
        {
            public required int x { get; init; }
            public required int y { get; init; }
            public required int width  { get; init; }
            public required int height  { get; init; }
        }
        public class TriggerData : ObjectData
        {
            public required TriggerType type { get; init; }
        }
    }
}
