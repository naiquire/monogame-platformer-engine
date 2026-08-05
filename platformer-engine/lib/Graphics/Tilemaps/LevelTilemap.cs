using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using lib.Graphics.Textures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace lib.Graphics.Tilemaps;
public class LevelTilemap(List<Tilemap> Layers)
{
    /// <summary>
    /// Stores a list of the Tilemaps in the level.
    /// </summary>
    public List<Tilemap> TileLayers { get; } = Layers;

    /// <summary>
    /// Gets or Sets the scale factor to draw each tile at.
    /// </summary>
    public Vector2 Scale {
        get 
        {
            return TileLayers[0].Scale;
        }
        set
        {
            foreach (Tilemap layer in TileLayers)
            {
                layer.Scale = value;
            }   
        }
    }

    /// <summary>
    /// Draws all of the Tilemaps in the level to the screen.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch used to draw this tilemap.</param>
    public void Draw(SpriteBatch spriteBatch)
    {
        DrawLayer(spriteBatch, LayerType.Base);
        DrawLayer(spriteBatch, LayerType.Detail);
    }

    /// <summary>
    /// Draws the Tilemaps matching a specific layer to the screen.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch used to draw this tilemap.</param>
    /// <param name="layerType">The layer type to draw.</param>
    private void DrawLayer(SpriteBatch spriteBatch, LayerType layerType)
    {
        foreach (Tilemap tilemap in TileLayers)
        {
            if (tilemap.LayerType == layerType)
            {
                tilemap.Draw(spriteBatch);
            }
        }
    }

    /// <summary>
    /// Creates a new LevelScreen from the content of a JSON file.
    /// </summary>
    /// <param name="content">The content manager used to load the texture for the tileset.</param>
    /// <param name="filename">The path to the json file, relative to the content root directory.</param>
    /// <returns></returns>
    public static LevelTilemap FromFile(ContentManager content, string filename)
    {
        string filePath = Path.Combine(content.RootDirectory, filename);
        List<Tilemap> level = [];

        using (Stream stream = TitleContainer.OpenStream(filePath))
        {
            LevelScreenData levelData = JsonSerializer.Deserialize<LevelScreenData>(stream);
            
            string contentPath = levelData.spritesheetPath;
            Texture2D texture = content.Load<Texture2D>(contentPath);

            foreach (LevelScreenData.Layer layer in levelData.layers)
            {
                Point position = new(layer.spritesheet.x, layer.spritesheet.y);
                Point dimensions = new(layer.spritesheet.width, layer.spritesheet.height);

                TextureRegion textureRegion = new(texture, position, dimensions);
                Tileset tileset = new(textureRegion, layer.tilemap.gridSize, layer.tilemap.gridSize);
                Tilemap tilemap = new(tileset, layer.tilemap.columns, layer.tilemap.rows, layer.type);

                for (int i = 0; i < tilemap.Count; i++)
                {
                    tilemap.SetTile(i, layer.tilemap.grid[i]);
                }

                level.Add(tilemap);
            }
        }

        return new LevelTilemap(level);
    }
}

internal class LevelScreenData
{
    public required string spritesheetPath { get; init; }
    public required Layer[] layers { get; init; }

    public class Layer
    {
        public required LayerType type { get; init; }
        public required Spritesheet spritesheet { get; init; }
        public required Tilemap tilemap { get; init; }
    }

    public struct Spritesheet
    {
        public required int x { get; init; }
        public required int y { get; init; }
        public required int width { get; init; }
        public required int height { get; init; }
    }
    public struct Tilemap
    {
        public required int gridSize { get; init; }
        public required int rows { get; init; }
        public required int columns { get; init; }
        public required List<int> grid { get; init; }
    }
}