using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using lib.Graphics.Textures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace lib.Graphics.Tilemaps;
public class LevelScreen(List<Tilemap> Layers)
{
    /// <summary>
    /// Stores a list of the Tilemaps in the level.
    /// </summary>
    public List<Tilemap> TileLayers { get; } = Layers;

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
    public static LevelScreen FromFile(ContentManager content, string filename)
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
                Point position = new(layer.x, layer.y);
                Point dimensions = new Point(layer.rows, layer.columns) * layer.gridSize;

                TextureRegion textureRegion = new(texture, position, dimensions);
                Tileset tileset = new(textureRegion, layer.gridSize, layer.gridSize);
                Tilemap tilemap = new(tileset, layer.columns, layer.rows, layer.type);

                for (int i = 0; i < tilemap.Count; i++)
                {
                    tilemap.SetTile(i, layer.grid[i]);
                }

                level.Add(tilemap);
            }
        }

        return new LevelScreen(level);
    }
}

internal class LevelScreenData
{
    public string spritesheetPath;
    public Layer[] layers;

    public class Layer
    {
        public LayerType type;
        public int x;
        public int y;
        public int gridSize;
        public int rows;
        public int columns;
        public List<int> grid;
    }
}