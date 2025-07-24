using SharpSDL3.Structs;

namespace DinoGame;

public abstract class GameObject(nint renderer) {
    protected nint rendererPtr = renderer;
    public FRect Position { get; set; }
    public FRect HitBox { get; set; }

    protected float Scale { get; init; }

    protected int Animation = 0;
    protected int Frame = 0;

    public abstract void Draw();
    public abstract void Update(Event sdlEvent);
}
