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
using SharpSDL3.Structs;
using SharpSDL3.TTF;

namespace DinoGame.Effects; 
internal class ZoomTextEffect : BaseTextEffect {
    private const float ZoomSpeed = 0.1f;

    private float _initialZoom;
    private float _zoom;
    private float _zoomSpeed = ZoomSpeed;
    private float _maxZoom;


    public bool UseUpdate { get; set; }
    public bool Animated { get; set; }

    public bool IsGrowing { get; set; }

    public ZoomTextEffect(nint rendererPtr, string text, Font font,
        float initialZoom, float maxZoom, float scale)
        : base(rendererPtr, text, font, font.Size, scale) {
        _initialZoom = initialZoom;
        _zoom = _initialZoom;
        _maxZoom = maxZoom;
        base.font = font;
        UseUpdate = true;
    }

    public override void Draw() {
        RenderText(Position.X, Position.Y);
    }

    public void Grow() {
        if (Animated && IsGrowing && _zoom < _maxZoom) {
            _zoom += _zoomSpeed;
            return;
        }
        Animated = false;
    }

    public void Reset() {
        font.Size = Program.FontSize * _initialZoom;
    }

    public void Shrink() {
        if (Animated && !IsGrowing && _zoom > _initialZoom) {
            _zoom -= _zoomSpeed;
            return;
        }
        Animated = false;
    }

    public override void Update(Event sdlEvent) {
        if (Text is null || Text.IsEmpty()) return;

        if (!UseUpdate) {
            base.Update(sdlEvent);
            return;
        }

        if (Animated) {
            _zoom += _zoomSpeed;
            font.Size = Program.FontSize * _zoom;
        } else {
            Reset();
        }

        if (Animated && UseUpdate && (_zoom > _maxZoom || _zoom < _initialZoom)) {
            _zoomSpeed = -_zoomSpeed;
        }

        //if (Animated && UseUpdate) {
        //    _zoom = _zoom > _maxZoom ? _maxZoom : _zoom < _initialZoom ? _initialZoom : _zoom;
        //    Animated = !(_zoom > _maxZoom || _zoom < _initialZoom);
        //}

        measuredTextSize = Ttf.MeasureString(font, Text);
        UpdateSize(measuredTextSize.Width, measuredTextSize.Height * _zoom);
        UpdateTextSize();
    }
}
