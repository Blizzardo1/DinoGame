using SharpSDL3.Enums;
using SharpSDL3;
using SharpSDL3.Structs;

namespace DinoGame;

public class Enemy : Entity {

    public Enemy(nint renderer, float scale) : base(renderer,
            Path.Combine("Tilesets", "Enemies", "Enemy 014", "Enemy_014.png"),
            32, 32, scale: scale, flipMode: FlipMode.Horizontal) {
        ResetEntity();
    }

    public override void Draw() {
        if(TileSet is null) {
            return;
        }

        TileSet.RenderTile(Frame, Animation, (int)Position.X, (int)Position.Y, Scale);
#if DEBUG
        Program.SetColor(255, 128, 255);
        FRect fRect = Position;
        FRect fHitbox = HitBox;
        Sdl.RenderRect(rendererPtr, ref fRect);
        Program.SetColor(255, 0, 0);
        Sdl.RenderRect(rendererPtr, ref fHitbox);
#endif
    }



    public override void Attack(Entity? entity) {
        if(entity is null) { return; }

        Animation = 4; // Attack
        entity.Die();
    }

    public override void Die() {
    }

    public override void ResetEntity() {
        Animation = 1;
        UpdateDimensions();
        RandSpawn<Enemy>();
    }

    public override void Update(Event sdlEvent) {
        if(TileSet is null) {
            return;
        }
        Frame--;
        if (Frame <= 0 ) {
            Frame = (TileSet.Width / TileSet.TileWidth) - 1;
        }
        Position = Position with { X = Position.X - 6 * Scale }; // Move the enemy left

        UpdateHitbox();

        if (Position.X + Position.W < 0) {
            RandSpawn<Enemy>();
        }
    }
}
