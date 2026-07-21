using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using lib;
using lib.Graphics.Tilemaps;
using lib.Scenes;
using Entities;
using lib.Colliders;
using lib.Colliders.Objects;

namespace platformer_engine;

public class LevelScene : Scene
{
    private Player _player;
    private Tilemap _tilemap;
    public static HitboxViewer HitboxView;

    public override void Initialize()
    {
        _player = new();

        base.Initialize();
    }

    public override void LoadContent()
    {
        HitboxView = new HitboxViewer(Core.GraphicsDevice);

        LevelObject floor = new(new Vector2(100, 650));
        floor.GenerateHitbox(1000, 50);
        LevelObjects.Add(floor);

        LevelObject wall = new(new Vector2(500, 600));
        wall.GenerateHitbox(100, 50);
        LevelObjects.Add(wall);

        LevelObject wall2 = new(new Vector2(700, 550));
        wall2.GenerateHitbox(100, 20);
        LevelObjects.Add(wall2);

        LevelObject wall3 = new(new Vector2(200, 500));
        wall3.GenerateHitbox(20, 100);
        LevelObjects.Add(wall3);

        LevelObject wall4 = new(new Vector2(300, 300));
        wall4.GenerateHitbox(20, 100);
        LevelObjects.Add(wall4);

        LevelObject wall5 = new(new Vector2(200, 100));
        wall5.GenerateHitbox(20, 100);
        LevelObjects.Add(wall5);

        LevelObject walll2 = new(new Vector2(800, 150));
        walll2.GenerateHitbox(100, 20);
        LevelObjects.Add(walll2);

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
        _player.Update(gameTime, this);
    }

    public override void Draw(GameTime gameTime)
    {
        // Clear the back buffer.
        Core.GraphicsDevice.Clear(Color.DarkSlateGray);

        // Begin the sprite batch to prepare for rendering.
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // Draw the tilemap.
        // _tilemap.Draw(SpriteBatch);

        HitboxView.DrawHitbox(Core.SpriteBatch, _player.GetHitbox());
        HitboxView.DrawPoint(Core.SpriteBatch, _player.Position);

        foreach (ICollidable collider in LevelObjects)
        {
            HitboxView.DrawHitbox(Core.SpriteBatch, collider.GetHitbox());
        }

        // Always end the sprite batch when finished.
        Core.SpriteBatch.End();
    }
}
