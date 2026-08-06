using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Levels.Objects;
using lib.Colliders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Levels;
public class Level(List<ICollidable> solids, List<ICollidable> hazards, List<ICollidable> triggers)
{
    public List<ICollidable> Solids = solids;
    public List<ICollidable> Hazards = hazards;
    public List<ICollidable> Triggers = triggers;

    public static Level FromFile(ContentManager content, string filename)
    {
        string filePath = Path.Combine(content.RootDirectory, filename);

        List<ICollidable> solids = [];
        List<ICollidable> hazards = [];
        List<ICollidable> triggers = [];

        using (Stream stream = TitleContainer.OpenStream(filePath))
        {
            LevelData levelData = JsonSerializer.Deserialize<LevelData>(stream);
            foreach (LevelData.ObjectData solid in levelData.solids)
            {
                Vector2 position = new(solid.x, solid.y);
                var obj = new SolidObject(position);
                obj.GenerateHitbox(solid.width, solid.height);
                solids.Add(obj);
            }
            foreach (LevelData.ObjectData hazard in levelData.hazards)
            {
                Vector2 position = new(hazard.x, hazard.y);
                var obj = new HazardObject(position);
                obj.GenerateHitbox(hazard.width, hazard.height);
                hazards.Add(obj);
            }
            foreach (LevelData.ObjectData trigger in levelData.triggers)
            {
                Vector2 position = new(trigger.x, trigger.y);
                var obj = new TriggerObject(position);
                obj.GenerateHitbox(trigger.width, trigger.height);
                triggers.Add(obj);
            }
        }

        return new Level(solids, hazards, triggers);
    }

    private class LevelData
    {
        public required List<ObjectData> solids { get; init; }
        public List<ObjectData> hazards { get; init; }
        public List<ObjectData> triggers { get; init; }
        public class ObjectData
        {
            public int x { get; init; }
            public int y { get; init; }
            public int width  { get; init; }
            public int height  { get; init; }
        }
    }
}