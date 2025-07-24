using SharpSDL3;
using SharpSDL3.Enums;
using SharpSDL3.Structs;
using System.ComponentModel.Design;

namespace DinoGame;

public abstract class Entity : GameObject {
    internal TileSet? TileSet;

    protected bool _isDead = false;

    public Entity(nint renderer, string tileSet, int tileWidth, int tileHeight, int xOffset = 0, int yOffset = 0, int xSpacing = 0, int ySpacing = 0, float scale = 1f, FlipMode flipMode = FlipMode.None) : base(renderer) {
        TileSet = new (renderer, tileSet, tileWidth, tileHeight, xOffset, yOffset, xSpacing, ySpacing, flipMode);
        Scale = scale;
    }

    public virtual bool Collides(Entity? entity) {
        if (entity is null) return false;

        return entity.HitBox.X < HitBox.X + HitBox.W &&
           entity.HitBox.X + entity.HitBox.W > HitBox.X &&
           entity.HitBox.Y < HitBox.Y + HitBox.H &&
           entity.HitBox.Y + entity.HitBox.H > HitBox.Y;
    }

    public void RandSpawn<T>() where T: Entity {
        if (typeof(T) == typeof(Enemy)) {
            Position = Position with {
                // Reset position if it goes off screen
                X = Program.Rand.Next(Program.Width + Program.Rand.Next(32, 1024),
                    Program.Width + Program.Rand.Next(1024, 1337))
            };
            return;
        }

        Sdl.LogWarn(LogCategory.Application, $"{typeof(T).Name} cannot have a random spawn");
    }

    public abstract void ResetEntity();

    public void UpdateDimensions() {
        if (TileSet is null) {
            Sdl.LogError(LogCategory.Error,
                "TileSet cannot be null for entity. Failed to update Rect Dimensions.");
            return;
        }
        if (GetType() == typeof(Enemy)) {
            Position = new() {
                X = 1000
            };
        } else if(GetType() == typeof(Player)) {
            Position = new() {
                X = 100
            };
        }

        Position = Position with {
            Y = Program.GetFloorY(TileSet),
            W = TileSet.TileWidth * Scale,
            H = TileSet.TileHeight * Scale
        };

        UpdateHitbox();
    }

    protected void UpdateHitbox() {
        HitBox = HitBox with {
            X = Position.X + (Position.W / 4),
            Y = Position.Y + (Position.H / 2),
            W = Position.W / 2,
            H = Position.H / 2
        };
    }

    public abstract void Attack(Entity? entity);

    public abstract void Die();
}
