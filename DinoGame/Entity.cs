using SharpSDL3;
using SharpSDL3.Enums;
using SharpSDL3.Structs;
using System.ComponentModel.Design;

namespace DinoGame;

public abstract class Entity : GameObject {
    internal TileSet? TileSet;

    protected bool _isDead = false;

    protected bool _isPassed;

    protected bool _isAttacking;

    protected const float XSpeed = 6f;

    public bool IsPassed => _isPassed;

    public bool IsAttacking => _isAttacking;

    public Entity(nint renderer, string tileSet, int tileWidth, int tileHeight, int xOffset = 0, int yOffset = 0, int xSpacing = 0, int ySpacing = 0, float scale = 1f, FlipMode flipMode = FlipMode.None) : base(renderer) {
        TileSet = new (renderer, tileSet, tileWidth, tileHeight, xOffset, yOffset, xSpacing, ySpacing, flipMode);
        Scale = scale;
    }

    public bool Collides(Entity? entity) {
        if (entity is null) return false;

        return CollidesX(entity)
            && CollidesY(entity);
    }

    public bool EndsCollisionX(Entity? entity) {
        if (entity is null) {
            return false;
        }
        bool result = entity.HitBox.X > HitBox.X + HitBox.W;
        if (result) {
            _isPassed = true;
        }
        return result;
    }

    public bool CollidesX(Entity? entity) {
        if (entity is null) {
            return false;
        }
        return entity.HitBox.X < HitBox.X + HitBox.W
            && entity.HitBox.X + entity.HitBox.W > HitBox.X;
    }

    public bool CollidesY(Entity? entity) {
        if (entity is null) {
            return false;
        }

        return entity.HitBox.Y < HitBox.Y + HitBox.H
            && entity.HitBox.Y + entity.HitBox.H > HitBox.Y;
    }

    public void RandSpawn<T>() where T: Entity {
        if (typeof(T) == typeof(Enemy)) {
            Position = Position with {
                // Reset position if it goes off screen
                X = Program.Rand.Next((Program.Width / 2) * Program.Rand.Next(2, 4),
                    (Program.Width / 2) * Program.Rand.Next(4, 8))
            };
            _isPassed = false;
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
                X = 200
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
