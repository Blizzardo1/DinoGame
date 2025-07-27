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
using SharpSDL3.TTF;

namespace DinoGame.Effects;
internal class StaticTextEffect(nint renderer, string text, Font font, float textSize, float scale)
    : BaseTextEffect(renderer, text, font, textSize, scale) {

    public override void Draw() {
        RenderText(Position.X, Position.Y);
    }

    public override void Update(Event sdlEvent) {
        base.Update(sdlEvent);
    }
}
