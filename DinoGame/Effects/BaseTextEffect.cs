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

using SharpSDL3.Enums;
using SharpSDL3.TTF;
using SharpSDL3;
using SharpSDL3.Structs;

namespace DinoGame.Effects; 
internal abstract class BaseTextEffect : BaseEffect, IDisposable {
    protected Font font;

    private float _textSize;
    protected Size measuredTextSize;

    private TextEngine _engine;
    private Text _text;
    private string _textStr;
    private bool disposedValue;

    ~BaseTextEffect() {
        Dispose(false);
    }

    public BaseTextEffect(nint renderer, string text, Font font, float textSize, float scale) : base(renderer, scale) {
        this.font = Ttf.CopyFont(font);
        Sdl.LogInfo(LogCategory.System, $"{font.Handle} = {this.font.Handle}");
        _textSize = textSize;
        _textStr = text;
        _engine = Ttf.CreateRendererTextEngine(rendererPtr);
        if (_engine.Handle == nint.Zero) {
            Sdl.LogError(LogCategory.Error, $"Error creating text engine: {Sdl.GetError()}");
            return;
        }

        _textStr = text;
        //_text = Ttf.CreateText(_engine, font, _textStr);
        UpdateTextSize();
    }

    public string Text {
        get => _textStr;
        set {
            _textStr = value;
            if(_text.Handle != nint.Zero)
                Ttf.SetTextString(_text, value, (ulong)value.Length);
        }
    }

    protected void RenderText(float x, float y, bool shadow = true) {
        Color previousColor = Sdl.GetRenderDrawColor(rendererPtr);
        Color currentColor = new() { R = 255, G = 255, B = 255, A = 255 };
        Program.SetColor(currentColor);
        if (Text is null || Text.IsEmpty()) {
            return;
        }

        if (rendererPtr == nint.Zero) {
            Sdl.LogError(LogCategory.Error, "Renderer is not initialized");
            return;
        }
        
        if (shadow) {
            Ttf.SetTextColor(_text, Program.Biome!.CurrentShadowColor);
            Ttf.DrawRendererText(_text, x + 2, y + 2);
        }

        Ttf.SetTextColor(_text, currentColor);
        Ttf.DrawRendererText(_text, x, y);

        Program.SetColor(previousColor);
    }

    public override void Update(Event sdlEvent) {
        if (_textStr is null || _textStr.IsEmpty()) return;
        font.Size = _textSize;
        measuredTextSize = Ttf.MeasureString(font, _textStr);
        UpdateSize(measuredTextSize.Width, measuredTextSize.Height);
    }

    protected void UpdateTextSize() {
        if(_text.Handle != nint.Zero)
            Ttf.DestroyText(_text);
        _text = Ttf.CreateText(_engine, font, _textStr);
    }

    protected virtual void Dispose(bool disposing) {
        if (!disposedValue) {
            if (disposing) {
                // TODO: dispose managed state (managed objects)
            }

            Ttf.DestroyText(_text);
            Ttf.DestroyRendererTextEngine(_engine);

            disposedValue = true;
        }
    }

    public void Dispose() {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
