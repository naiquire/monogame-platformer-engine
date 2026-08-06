using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using lib;
using lib.Colliders;
using lib.Graphics.Tilemaps;
using lib.Scenes;
using Entities;
using Levels;

namespace platformer_engine;

public class LevelScene : Scene
{
    private Player _player;
    public static HitboxViewer HitboxView;

    /// <summary>
    /// Stores a list of collidable objects within the current scene.
    /// </summary>
    public Level Level { get; private set; }

    private TilemapLayers _tilemap;

    public override void Initialize()
    {
        _player = new(this, new Vector2(200, 200));
        _player.Initialize();

        base.Initialize();
    }

    public override void LoadContent()
    {
        _tilemap = TilemapLayers.FromFile(Core.Content, "Levels/tilemap.json");
        _tilemap.Scale = 2f * Vector2.One;

        Level = Level.FromFile(Core.Content, "Levels/0.json");

        HitboxView = new HitboxViewer(Core.GraphicsDevice);

        // Create the texture atlas from the XML configuration file.
        // TextureAtlas atlas = TextureAtlas.FromFile(Content, "Images/TextureAtlas.xml");

        // _player.LoadContent(atlas.CreateAnimatedSprite("bat-animation"));
        // _player.Texture.Scale = new Vector2(4.0f, 4.0f);
    
        // Create the tilemap from the XML configuration file.
        // _tilemap = Tilemap.FromFile(Content, "Images/TilemapDefinition.xml");
        // _tilemap.Scale = new Vector2(2.0f, 2.0f);
    }

    public override void Update(GameTime gameTime)
    {
        _player.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        // Clear the back buffer.
        Core.GraphicsDevice.Clear(Color.DarkSlateGray);

        // Begin the sprite batch to prepare for rendering.
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // Draw the tilemap.
        _tilemap.Draw(Core.SpriteBatch);

        HitboxView.DrawHitbox(Core.SpriteBatch, _player.GetHitbox());
        HitboxView.DrawPoint(Core.SpriteBatch, _player.Position);

        foreach (ICollidable solid in Level.Solids)
        {
            HitboxView.DrawHitbox(Core.SpriteBatch, solid.GetHitbox());
        }
        foreach (ICollidable hazard in Level.Hazards)
        {
            HitboxView.DrawHitbox(Core.SpriteBatch, hazard.GetHitbox());
        }
        foreach (ICollidable trigger in Level.Triggers)
        {
            HitboxView.DrawHitbox(Core.SpriteBatch, trigger.GetHitbox());
        }

        // Always end the sprite batch when finished.
        Core.SpriteBatch.End();
    }
}
