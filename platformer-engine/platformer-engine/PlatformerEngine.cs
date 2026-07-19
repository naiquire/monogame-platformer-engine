using lib;
using platformer_engine;

public class PlatformerEngine : Core
{
    public PlatformerEngine() : base("PlatformerEngine", 1280, 720, false)
    {

    }

    protected override void Initialize()
    {
        base.Initialize();

        ChangeScene(new LevelScene());
    }
    
    protected override void LoadContent()
    {
        base.LoadContent();
    }
}