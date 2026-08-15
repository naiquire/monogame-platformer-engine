using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using lib;
using lib.Graphics.Tilemaps;
using lib.Scenes;
using Entities;
using Levels;
using Levels.Objects;
using System.Collections.Generic;

namespace platformer_engine;

public struct Resources
{
    public string LevelData;
    public string TilemapData;
}

public class LevelScene : Scene
{
    private Resources _resources;

    private Player _player;
    public LevelManager Level { get; private set; }
    public TilemapManager Tilemap { get; private set; }
    public static HitboxViewer HitboxView;


    public static readonly bool ShowHitboxes = true; 


    public LevelScene(string LevelFile, string TilemapFile)
    {
        _resources = new()
        {
            LevelData = LevelFile,
            TilemapData = TilemapFile
        };
    }    

    public override void Initialize()
    {
        _player = new(this, new Vector2(600, 200));
        _player.Initialize();

        Core.Camera.CameraMode = CameraMode.Static;

        base.Initialize();
    }

    public override void LoadContent()
    {
        // Level = LevelManager.FromFile(Core.Content, _resources.LevelData);

        Tilemap = TilemapManager.FromFile(Core.Content, _resources.TilemapData);
        Tilemap.Scale = 3f * Vector2.One;

        Level = LevelManager.FromTilemap(Tilemap);

        HitboxView = new HitboxViewer(Core.GraphicsDevice);
    }

    public override void Update(GameTime gameTime)
    {
        _player.Update(gameTime);

        // Update the camera controller.
        Core.Camera.Update(_player);
    }

    public override void Draw(GameTime gameTime)
    {
        // Clear the back buffer.
        Core.GraphicsDevice.Clear(Color.Black);

        // Begin the sprite batch to prepare for rendering.
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // Draw the tilemap.
        Tilemap.Draw(Core.SpriteBatch);

        // Draw hitboxes
        if (ShowHitboxes)
        {
            foreach (SolidObject solid in Level.Solids)
            {
                HitboxView.DrawHitbox(Core.SpriteBatch, solid.GetHitbox(), Color.Blue);
            }
            foreach (HazardObject hazard in Level.Hazards)
            {
                HitboxView.DrawHitbox(Core.SpriteBatch, hazard.GetHitbox(), Color.Red);
            }
            foreach (TriggerObject trigger in Level.Triggers)
            {
                HitboxView.DrawHitbox(Core.SpriteBatch, trigger.GetHitbox(), Color.Green);
            }
        }

        HitboxView.DrawHitbox(Core.SpriteBatch, _player.GetHitbox(), Color.Yellow);
        HitboxView.DrawPoint(Core.SpriteBatch, _player.Position);

        // Always end the sprite batch when finished.
        Core.SpriteBatch.End();
    }
}
