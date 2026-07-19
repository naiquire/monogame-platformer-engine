using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using lib;
using lib.Graphics.Tilemaps;
using lib.Scenes;
using Entities;

namespace platformer_engine;

public class LevelScene : Scene
{
    private Player _player;
    public static Rectangle _roomBounds;
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
        _roomBounds = new Rectangle(100, 650, 1000, 50);

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
        Core.GraphicsDevice.Clear(Color.CornflowerBlue);

        // Begin the sprite batch to prepare for rendering.
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // Draw the tilemap.
        // _tilemap.Draw(SpriteBatch);

        HitboxView.DrawHitbox(Core.SpriteBatch, _player.GetHitbox());
        HitboxView.DrawPoint(Core.SpriteBatch, _player.Position);

        HitboxView.DrawHitbox(Core.SpriteBatch, _roomBounds);

        // Always end the sprite batch when finished.
        Core.SpriteBatch.End();
    }
}
