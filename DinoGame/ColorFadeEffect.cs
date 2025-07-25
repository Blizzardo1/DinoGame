using SharpSDL3.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoGame; 
internal class ColorFadeEffect {

    private readonly Color _night = new() { R = 34, G = 1, B = 53, A = 255 }; // Russian Violet
    private readonly Color _day = new() { R = 111, G = 144, B = 244, A = 255 }; // Cornflower Blue
    private int _fadeSteps;
    private int currentStep = 0;
    private bool _fadingNight = false;

    public bool IsTriggered { get; private set; }

    public ColorFadeEffect(int fadeSteps) {
        if (fadeSteps < 1) throw new ArgumentException("Fade Steps must be a positive integer");
        _fadeSteps = fadeSteps;
    }

    public void Trigger() {
        IsTriggered = !IsTriggered;
    }

    public Color GetNextColor() {
        if (!IsTriggered) {
            return GetCurrentColor();
        }

        float t = (float)currentStep / _fadeSteps;
        if (!_fadingNight) t = 1f - t;

        byte r = (byte)(_night.R * t + _day.R * (1f - t));
        byte g = (byte)(_night.G * t + _day.G * (1f - t));
        byte b = (byte)(_night.B * t + _day.B * (1f - t));

        currentStep++;
        if (currentStep > _fadeSteps) {
            currentStep = 0;
            _fadingNight = !_fadingNight;
            IsTriggered = false;
        }
        return new() { R = r, G = g, B = b, A = 255 };
    }

    public Color GetCurrentColor() {
        return _fadingNight ? _day : _night;
    }
}
