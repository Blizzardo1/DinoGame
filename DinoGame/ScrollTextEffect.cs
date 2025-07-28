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

using DinoGame.Effects;
using SharpSDL3.Structs;
using SharpSDL3.TTF;

namespace DinoGame;

internal class ScrollTextEffect(nint renderer, float speed, string text, Font font, float textSize, float scale)
    : BaseTextEffect(renderer, text, font, textSize, scale) {
    public float Speed => speed;
    public bool OutOfBounds {  get; private set; }
    public bool Animate { get; set; }

    public Direction TextDirection { get; set; }

    public override void Draw() {
        if (OutOfBounds) return;
        RenderText(Position.X, Position.Y);
    }

    public override void Update(Event sdlEvent) {
        if (OutOfBounds) return;
        base.Update(sdlEvent);
        float sX, sY;
        if (Animate) {
            switch (TextDirection) {
                case Direction.Down:
                    sX = Position.X;
                    sY = Position.Y + Speed;
                    break;
                case Direction.Up:
                    sX = Position.X;
                    sY = Position.Y - Speed;
                    break;
                case Direction.Left:
                    sX = Position.X - Speed;
                    sY = Position.Y;
                    break;
                case Direction.Right:
                    sX = Position.X + Speed;
                    sY = Position.Y;
                    break;
                case Direction.UpLeft:
                    sX = Position.X - Speed;
                    sY = Position.Y - Speed;
                    break;
                case Direction.DownLeft:
                    sX = Position.X - Speed;
                    sY = Position.Y + Speed;
                    break;
                case Direction.DownRight:
                    sX = Position.X + Speed;
                    sY = Position.Y + Speed;
                    break;
                case Direction.UpRight:
                    sX = Position.X + Speed;
                    sY = Position.Y - Speed;
                    break;
                default:
                    sX = Position.X;
                    sY = Position.Y;
                    break;
            }
            UpdatePosition(sX, sY);
        }
        
        if (Position.X + Position.W < 0
            || Position.Y + Position.H < 0
            || Position.Y > Program.Height
            || Position.X > Program.Width) {
            // Box out of bounds
            OutOfBounds = true;
            Animate = false;
        }
    }
}
