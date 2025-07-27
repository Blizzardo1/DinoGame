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

using SharpSDL3;
using SharpSDL3.Enums;
using SharpSDL3.Structs;

namespace DinoGame.Entities;

public class Enemy : Entity {



    public Enemy(nint renderer, float scale) : base(renderer,
            Path.Combine("Assets", "Tilesets", "Enemies", "Enemy 014", "Enemy_014.png"),
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
        _isAttacking = true;
        Animation = 4; // Attack
        entity.Die();
    }

    public override void Die() {
    }

    public override void ResetEntity() {
        Animation = 1;
        UpdateDimensions();
        RandSpawn<Enemy>();
        _isAttacking = false;
    }

    public override void Update(Event sdlEvent) {
        if(TileSet is null) {
            return;
        }
        

        if (_isAttacking && Frame <= 0) {
            Frame = 0;
            return;
        }

        if (Frame <= 0 ) {
            Frame = TileSet.Width / TileSet.TileWidth - 1;
        }

        if (!_isAttacking) {
            UpdatePosition(Position.X - XSpeed * Scale, Position.Y); // Move the enemy left
        }

        Frame--;

        UpdateHitbox();

        if (Position.X + Position.W < 0) {
            ResetEntity();
        }
    }
}
