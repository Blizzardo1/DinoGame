using SharpSDL3.Structs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoGame; 
internal class ColorFadeEffect {

    private readonly Color _russianViolet = new() { R = 34, G = 1, B = 53, A = 255 };
    private readonly Color _cornflowerBlue = new() { R = 111, G = 144, B = 244, A = 255 };
    private readonly Color _maroon = new() { R = 128, G = 16, B = 8, A = 255 };
    private readonly Color _black = new() { R = 0, G = 0, B = 0, A = 255 };
    private readonly Color _transparent = new() { R = 0, G = 0, B = 0, A = 0 };
    
    
    public Color RussianViolet => _russianViolet;
    
    public Color CornflowerBlue => _cornflowerBlue;

    public Color Maroon => _maroon;

    public Color Black => _black;

    public Color Transparent => _transparent;
    

    private int _fadeSteps;
    private int _currentStep = 0;
    private bool _fadingNight = false;

    public bool IsTriggered { get; private set; }

    public bool IsDay => _fadingNight;

    public ColorFadeEffect(int fadeSteps) {
        if (fadeSteps < 1) throw new ArgumentException("Fade Steps must be a positive integer");
        _fadeSteps = fadeSteps;
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

        return (new() { R = r, G = g, B = b, A = 255 }, new() { R = 0, G = 0, B = 0, A = GetAlpha() });
    }

    public (Color background, Color shadow) FadeToDeath() {
        return Fade(GetCurrentColor().background, _maroon, true);
    }

    public (Color background, Color shadow) ResetDeath() {
        return Fade(_maroon, _russianViolet, true);
    }

    public (Color background, Color shadow) GetNextColor() {
        return Fade(_russianViolet, _cornflowerBlue, !_fadingNight);
    }

    public (Color background, Color shadow) GetCurrentColor() {
        return _fadingNight ? (_cornflowerBlue, _black) : (_russianViolet, _transparent);
    }
}
