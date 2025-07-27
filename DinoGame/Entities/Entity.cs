/***
     A Game where you need to evade the enemy to gain points.
    Copyright (C) 2025  Adonis Deliannis (Blizzardo1)

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along
    with this program; if not, write to the Free Software Foundation, Inc.,
    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using DinoGame.GameObjects;
using SharpSDL3;
using SharpSDL3.Enums;

namespace DinoGame.Entities;

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
            UpdatePosition(
                // Reset position if it goes off screen
                Program.Rand.Next(Program.Width / 2 * Program.Rand.Next(2, 4),
                    Program.Width / 2 * Program.Rand.Next(4, 8)),
                Position.Y);
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
            _position = new() {
                X = 1000
            };
        } else if(GetType() == typeof(Player)) {
            _position = new() {
                X = 200
            };
        }

        UpdatePosition(Position.X, Program.GetFloorY(TileSet));

        UpdateSize(TileSet.TileWidth * Scale,
            TileSet.TileHeight * Scale
        );

        UpdateHitbox();
    }

    protected void UpdateHitbox() {
        UpdateHitbox(
            _position.X + _position.W / 4,
            _position.Y + _position.H / 2,
            _position.W / 2,
            _position.H / 2);       
    }

    public abstract void Attack(Entity? entity);

    public abstract void Die();
}
