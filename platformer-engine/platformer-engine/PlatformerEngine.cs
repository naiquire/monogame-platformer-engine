using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using lib;
using lib.Graphics.Textures;
using lib.Graphics.Tilemaps;
using Entities;

namespace platformer_engine;

public class PlatformerEngine : Core
{
    private Player _player;
    private Tilemap _tilemap;
    public static HitboxViewer HitboxViewer;

    public PlatformerEngine() : base("PlatformerEngine", 1280, 720, false)
    {

    }

    protected override void Initialize()
    {
        _player = new();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        base.LoadContent();

        HitboxViewer = new(GraphicsDevice);

        // Create the texture atlas from the XML configuration file.
        TextureAtlas atlas = TextureAtlas.FromFile(Content, "Images/TextureAtlas.xml");

        _player.LoadContent(atlas.CreateAnimatedSprite("bat-animation"));
        _player.Texture.Scale = new Vector2(4.0f, 4.0f);
    
        // Create the tilemap from the XML configuration file.
        _tilemap = Tilemap.FromFile(Content, "Images/TilemapDefinition.xml");
        _tilemap.Scale = new Vector2(4.0f, 4.0f);
    }

    protected override void Update(GameTime gameTime)
    {
        _player.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        // Clear the back buffer.
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // Begin the sprite batch to prepare for rendering.
        SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // Draw the tilemap.
        _tilemap.Draw(SpriteBatch);

        // draw bat texture and hitbox
        _player.Texture.Draw(SpriteBatch, _player.GetSpritePosition());
        HitboxViewer.DrawHitbox(SpriteBatch, _player.Hitbox);
        HitboxViewer.DrawPoint(SpriteBatch, _player.Position);

        // Always end the sprite batch when finished.
        SpriteBatch.End();

        base.Draw(gameTime);
    }
}
