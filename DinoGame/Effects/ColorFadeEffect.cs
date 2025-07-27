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

    private static readonly Color _russianViolet = new() { R = 34, G = 1, B = 53, A = 255 };
    private static readonly Color _cornflowerBlue = new() { R = 111, G = 144, B = 244, A = 255 };
    private static readonly Color _maroon = new() { R = 128, G = 16, B = 8, A = 255 };
    private static readonly Color _black = new() { R = 0, G = 0, B = 0, A = 255 };
    private static readonly Color _transparent = new() { R = 0, G = 0, B = 0, A = 0 };

    private Color _currentColor;
    private Color _targetColor;
    private Color _currentShadow;
    
    public static Color RussianViolet => _russianViolet;
    
    public static Color CornflowerBlue => _cornflowerBlue;

    public static Color Maroon => _maroon;

    public static Color Black => _black;

    public static Color Transparent => _transparent;
    
    public Color CurrentColor => _currentColor;
    public Color TargetColor {
        get => _targetColor;
        set => _targetColor = value;
    }


    private int _fadeSteps;
    private int _currentStep = 0;
    private bool _fadingNight = false;

    public bool IsTriggered { get; private set; }

    public bool IsDay => _fadingNight;

    public ColorFadeEffect(int fadeSteps, Color currentColor, Color targetColor) {
        if (fadeSteps < 1) throw new ArgumentException("Fade Steps must be a positive integer");
        _fadeSteps = fadeSteps;
        _currentColor = currentColor;
        _targetColor = targetColor;
    }

    public byte GetAlpha() {
        // Calculate interpolation factor based on fade progress
        float t = (float)(_fadeSteps - _currentStep) / _fadeSteps;
        // Smooth alpha transition between 127 (half opacity) and 255 (full opacity)
        // Adjust range (127, 255) to (0, 255) or other if desired
        return (byte)Math.Round(255 * (_fadingNight ? t : 1f - t));
    }

    public void Trigger() {
        IsTriggered = !IsTriggered;
    }

    private (Color background, Color shadow) Fade(Color c1, Color c2, bool condition) {
        if (!IsTriggered) {
            return GetCurrentColor();
        }
        float t = (float)_currentStep / _fadeSteps;
        if (condition) t = 1f - t;

        byte r = (byte)(c1.R * t + c2.R * (1f - t));
        byte g = (byte)(c1.G * t + c2.G * (1f - t));
        byte b = (byte)(c1.B * t + c2.B * (1f - t));

        _currentStep++;
        if (_currentStep > _fadeSteps) {
            _currentStep = 0;
            _fadingNight = condition;
            IsTriggered = false;
        }
        _currentColor = new() { R = r, G = g, B = b, A = 255 };
        _currentShadow = new() { R = 0, G = 0, B = 0, A = GetAlpha() };

        return (_currentColor, _currentShadow);
    }

    public (Color background, Color shadow) FadeToDeath() {
        return Fade(_currentColor, _maroon, true);
    }

    public (Color background, Color shadow) GetNextColor() {
        return Fade(_currentColor, _targetColor, !_fadingNight);
    }

    public (Color background, Color shadow) GetCurrentColor() {
        return (_currentColor, _currentShadow);
    }
}
