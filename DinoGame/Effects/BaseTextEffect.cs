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
    private bool _disposedValue;
    
    public bool ShowBox { get; set; }

    public bool Use3DBorder { get; set; }
    public bool Use3DBorderUpsideDown { get; set; }

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

    protected void RenderText(float x, float y, string text, bool shadow = true) {
        Color previousColor = Sdl.GetRenderDrawColor(rendererPtr);
        if (ShowBox) {
            FRect fRect = Position;
            Color top = new() { R = 0, G = 0, B = 0, A = 255 };
            Color bottom = new() { R = 0, G = 0, B = 0, A = 255 };

            Program.SetColor(0, 0, 0, 128);
            Sdl.RenderFillRect(rendererPtr, ref fRect);
            if (Use3DBorder) {
                top = new() { R = 255, G = 255, B = 255, A = 255 };
                bottom = new() { R = 128, G = 128, B = 128, A = 255 };
            } else if (Use3DBorderUpsideDown) {
                top = new() { R = 128, G = 128, B = 128, A = 255 };
                bottom = new() { R = 255, G = 255, B = 255, A = 255 };
            }

            Program.SetColor(top);
            Sdl.RenderLine(rendererPtr,
                Position.X,
                Position.Y,
                Position.X + Position.W,
                Position.Y);
            Sdl.RenderLine(rendererPtr,
                Position.X,
                Position.Y,
                Position.X,
                Position.Y + Position.H
                );

            Program.SetColor(bottom);
            Sdl.RenderLine(rendererPtr,
                Position.X + Position.W,
                Position.Y,
                Position.X + Position.W,
                Position.Y + Position.H
                );
            Sdl.RenderLine(rendererPtr,
                Position.X,
                Position.Y + Position.H,
                Position.X + Position.W,
                Position.Y + Position.H
                );
        }

        Color currentColor = new() { R = 255, G = 255, B = 255, A = 255 };
        Program.SetColor(currentColor);
        if (text is null || text.IsEmpty()) {
            return;
        }

        if (rendererPtr == nint.Zero) {
            Sdl.LogError(LogCategory.Error, "Renderer is not initialized");
            return;
        }

        // God I hope this works
        Ttf.SetTextString(_text, text, (ulong)text.Length);

        if (shadow) {
            Ttf.SetTextColor(_text, Program.Biome!.CurrentShadowColor);
            Ttf.DrawRendererText(_text, x + 2, y + 2);
        }

        Ttf.SetTextColor(_text, currentColor);
        Ttf.DrawRendererText(_text, x, y);

        Program.SetColor(previousColor);
    }

    protected void RenderText(float x, float y, bool shadow = true) {
        RenderText(x, y, _textStr, shadow);
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

    public override void CenterObject(float xOffset, float yOffset) {
        CenterObjectX(xOffset);
        CenterObjectY(yOffset);
    }

    public override void CenterObjectX(float xOffset = 0) {
        _position.X = (Program.Width / 2) - (Position.W / 2) + xOffset;
    }

    public override void CenterObjectY(float yOffset = 0) {
        _position.Y = (Program.Height / 2) - (Position.Y / 2) + yOffset;
    }

    protected virtual void Dispose(bool disposing) {
        if (!_disposedValue) {
            if (disposing) {
                // TODO: dispose managed state (managed objects)
            }

            Ttf.DestroyText(_text);
            Ttf.DestroyRendererTextEngine(_engine);

            _disposedValue = true;
        }
    }

    public void Dispose() {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
