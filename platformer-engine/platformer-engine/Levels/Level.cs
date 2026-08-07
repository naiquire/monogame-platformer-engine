using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Levels.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Levels;
public class Level(List<Interactable> objs)
{
    public List<Interactable> ObjectsinLevel { get; } = objs;
    public List<SolidObject> Solids => [.. ObjectsinLevel.OfType<SolidObject>()];
    public List<HazardObject> Hazards => [.. ObjectsinLevel.OfType<HazardObject>()];
    public List<TriggerObject> Triggers => [.. ObjectsinLevel.OfType<TriggerObject>()];


    public static Level FromFile(ContentManager content, string filename)
    {
        string filePath = Path.Combine(content.RootDirectory, filename);

        List<Interactable> objs = [];

        using (Stream stream = TitleContainer.OpenStream(filePath))
        {
            LevelData levelData = JsonSerializer.Deserialize<LevelData>(stream);

            objs.AddRange(ParseObjectType(levelData.solids, (position) => new SolidObject(position)));
            objs.AddRange(ParseObjectType(levelData.hazards, (position) => new HazardObject(position)));
            objs.AddRange(ParseObjectType(levelData.triggers, (position) => new TriggerObject(position)));
        }

        return new Level(objs);

        static List<Interactable> ParseObjectType(List<LevelData.ObjectData> objects, Func<Vector2, Interactable> map)
        {
            List<Interactable> colliders = [];

            foreach (LevelData.ObjectData obj in objects)
            {
                Vector2 position = new (obj.x, obj.y);
                var collider = map(position);
                collider.GenerateHitbox(obj.width, obj.height);
                colliders.Add(collider);
            }

            return colliders;
        }
    }

    private class LevelData
    {
        public required List<ObjectData> solids { get; init; }
        public required List<ObjectData> hazards { get; init; }
        public required List<ObjectData> triggers { get; init; }
        public class ObjectData
        {
            public required int x { get; init; }
            public required int y { get; init; }
            public required int width  { get; init; }
            public required int height  { get; init; }
        }
    }
}