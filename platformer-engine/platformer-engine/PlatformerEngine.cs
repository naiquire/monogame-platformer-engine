using lib;
using platformer_engine;

public class PlatformerEngine : Core
{
    public PlatformerEngine() : base("PlatformerEngine", 1920, 1080, false)
    {

    }

    protected override void Initialize()
    {
        base.Initialize();

        ChangeScene(new LevelScene("Levels/0.json", "Levels/tilemap.json"));
    }

    protected override void LoadContent()
    {
        base.LoadContent();
    }
}