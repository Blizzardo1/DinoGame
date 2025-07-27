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

namespace DinoGame.Entities; 
internal class Biome : Entity {

    private const int FadingSteps = 30;

    private int _framesToTriggerFade = 0;
    private ColorFadeEffect _colorFade;

    private bool _deathTriggered = false;
    private bool _resetTriggered = false;

    private Color _currentBackgroundColor;
    private Color _currentShadowColor;

    public Biome(nint renderer, float scale) : base(renderer,
            Path.Combine("Assets", "Tilesets", "Tilesets", "PS_Tileset_07.png"), 16, 16, 0, 0, 0, 0, scale) {
        _colorFade = new ColorFadeEffect(FadingSteps, ColorFadeEffect.RussianViolet);
        _currentBackgroundColor = _colorFade.GetCurrentColor().background;
        _currentShadowColor = _colorFade.GetCurrentColor().shadow;
    }

    public int FramesToTriggerFade { get => _framesToTriggerFade; set => _framesToTriggerFade = value; }

    public ColorFadeEffect ColorFadeEffect => _colorFade;

    public Color CurrentBackgroundColor => _currentBackgroundColor;
    public Color CurrentShadowColor => _currentShadowColor;

    public bool DeathTriggered { get => _deathTriggered; set => _deathTriggered = value; }
    public bool ResetTriggered {get => _resetTriggered; set=> _resetTriggered = value;}


    private float FloorWidth => Program.Width / TileSet!.TileWidth / Scale + 10;

    public override void Draw() {
        // Theoretically, 120 tiles across the bottom
        for (int i = 0; i <= FloorWidth; i++) {
            TileSet.RenderTile(17, 0, i * TileSet.TileWidth * Scale + Position.X, Position.Y, Scale);
        }
    }

    public override void Attack(Entity? entity) {
    }

    public override void Die() {
    }

    public override void ResetEntity() {
        _deathTriggered = false;
        _resetTriggered = true;
        _colorFade.TargetColor = _colorFade.IsDay ? ColorFadeEffect.RussianViolet : ColorFadeEffect.CornflowerBlue;
        UpdatePosition(0, Position.Y);
        _colorFade.Trigger();
    }

    public override void Update(Event sdlEvent) {
        var player = Program.Player!;
        var width = Program.Width;
        var height = Program.Height;

        if (_deathTriggered && !_colorFade.IsTriggered) {
            _colorFade.Trigger();
        } else if (_resetTriggered && _colorFade.IsTriggered) {
            (_currentBackgroundColor, _currentShadowColor) = _colorFade.GetNextColor();
            if (++_framesToTriggerFade > FadingSteps) {
                _framesToTriggerFade = 0;
                _resetTriggered = false;
            }
        } else {
            (_currentBackgroundColor, _currentShadowColor) =
                !player.IsDead ? _colorFade.GetNextColor() : _colorFade.FadeToDeath();
        }

        if (!player.IsDead && ++_framesToTriggerFade > FadingSteps * 20) {
            _framesToTriggerFade = 0;
            _colorFade.Trigger();
        }

        if (player.IsDead) return;

        UpdatePosition(Position.X - XSpeed, height - TileSet!.TileHeight * Scale);
        UpdateSize(width, TileSet!.TileHeight * Scale);

        if (Position.X < -FloorWidth * 3) {
            UpdatePosition(0, Position.Y);
        }
    }
}
