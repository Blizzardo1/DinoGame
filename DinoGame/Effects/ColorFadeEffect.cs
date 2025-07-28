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

namespace DinoGame.Effects;

internal class ColorFadeEffect {
    private readonly Color[] _colorCycle = [Colors.DarkSlateBlue, Colors.CornflowerBlue, Colors.Maroon];
    private int _currentColorIndex = 0;
    private Color _currentColor;
    private Color _targetColor;
    private Color _currentShadow;
    private readonly float _fadeSteps;
    private float _currentFadeSteps;
    private float _currentStep = 0;
    private bool _isFading = false;

    public Color CurrentColor => _currentColor;
    public Color TargetColor {
        get => _targetColor;
        set => _targetColor = value;
    }

    public bool IsTriggered { get; private set; }
    public bool IsDay => _currentColorIndex == 1; // CornflowerBlue is "day"

    public ColorFadeEffect(int fadeSteps, int currentColorIndex = 0) {
        if (fadeSteps < 1) throw new ArgumentException("Fade Steps must be a positive integer");
        _fadeSteps = fadeSteps;
        _currentFadeSteps = fadeSteps;
        _currentColor = _colorCycle[currentColorIndex % (_colorCycle.Length - 1)];
        _targetColor = _colorCycle[0]; // Start with Russian Violet
        _currentShadow = new() { R = (byte)(_currentColor.R * 0.5f), G = (byte)(_currentColor.G * 0.5f), B = (byte)(_currentColor.B * 0.5f), A = 255 };
    }

    private float GetFadeProgress() => Math.Clamp(_currentStep / _currentFadeSteps, 0f, 1f);

    private byte GetAlpha(Color from, Color to) {
        // If both colors are opaque, keep alpha at 255
        if (from.A == 255 && to.A == 255) return 255;
        float t = GetFadeProgress();
        return (byte)Math.Round(from.A * (1f - t) + to.A * t);
    }

    public void Trigger() {
        if (!_isFading) {
            IsTriggered = true;
            _isFading = true;
            _currentStep = 0;
            _currentColorIndex = (_currentColorIndex + 1) % _colorCycle.Length - 1;
            _targetColor = _colorCycle[_currentColorIndex];
        }
    }

    private (Color background, Color shadow) Fade(Color from, Color to) {
        if (!_isFading) {
            return (_currentColor, _currentShadow);
        }

        float t = GetFadeProgress();

        byte r = (byte)Math.Round(from.R * (1f - t) + to.R * t);
        byte g = (byte)Math.Round(from.G * (1f - t) + to.G * t);
        byte b = (byte)Math.Round(from.B * (1f - t) + to.B * t);
        byte a = GetAlpha(from, to);

        byte shadowR = (byte)Math.Round(r * 0.5f);
        byte shadowG = (byte)Math.Round(g * 0.5f);
        byte shadowB = (byte)Math.Round(b * 0.5f);

        _currentColor = new() { R = r, G = g, B = b, A = a };
        _currentShadow = new() { R = shadowR, G = shadowG, B = shadowB, A = a };

        _currentStep++;
        if (_currentStep >= _currentFadeSteps) {
            _currentStep = 0;
            _currentFadeSteps = _fadeSteps;
            _isFading = false;
            IsTriggered = false;
            _currentColor = new() { R = to.R, G = to.G, B = to.B, A = to.A };
            _currentShadow = new() { R = (byte)(to.R * 0.5f), G = (byte)(to.G * 0.5f), B = (byte)(to.B * 0.5f), A = to.A };
        }

        return (_currentColor, _currentShadow);
    }

    public (Color background, Color shadow) FadeToDeath() {
        if (!IsTriggered || !_isFading) {
            _targetColor = Colors.Maroon;
            _isFading = true;
            IsTriggered = true;
            _currentFadeSteps = 6;
            _currentStep = 0;
        }
        return Fade(_currentColor, _targetColor);
    }

    public (Color background, Color shadow) GetNextColor() => Fade(_currentColor, _targetColor);

    public (Color background, Color shadow) GetColorFromIndex(int index) =>
        (_colorCycle[index % (_colorCycle.Length - 1)], _currentShadow);

    public (Color background, Color shadow) GetCurrentColor() => (_currentColor, _currentShadow);
}
