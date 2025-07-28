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

using SharpSDL3.Structs;

namespace DinoGame.GameObjects;

public abstract class GameObject(nint renderer) {
    protected nint rendererPtr = renderer;
    protected FRect _position;
    protected FRect _hitBox;

    public FRect Position => _position;
    public FRect HitBox => _hitBox;

    protected float Scale { get; init; }

    protected int Animation = 0;
    protected int Frame = 0;

    public abstract void Draw();
    public abstract void Update(Event sdlEvent);

    public void UpdateHitbox(float x, float y, float w, float h) {
        _hitBox.X = x;
        _hitBox.Y = y;
        _hitBox.W = w;
        _hitBox.H = h;
    }

    public void UpdatePosition(float x, float y) {
        _position.X = x;
        _position.Y = y;
    }

    public virtual void CenterObject(float xOffset, float yOffset) {
        _position.X = (Program.Width / 2) - xOffset;
        _position.Y = (Program.Height / 2) - yOffset;
    }

    public void UpdateSize(float w, float h) {
        _position.W = w;
        _position.H = h;
    }
}
