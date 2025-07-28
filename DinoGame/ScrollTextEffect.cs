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
using SharpSDL3;
using SharpSDL3.Structs;
using SharpSDL3.TTF;

namespace DinoGame;

internal class ScrollTextEffect(nint renderer, float speed, string text, Font font, float textSize, float scale)
    : BaseTextEffect(renderer, text, font, textSize, scale) {
    private const int Buffer = 1;

    private readonly float _headerSize = Program.FontSize * 3f;
    private readonly float _creditSize = Program.FontSize * 1.5f;

    private float _yOffset = 1;
    private float _maxYOffset = 0;

    public float Speed => speed;
    public bool OutOfBounds {  get; private set; }
    
    public bool Animate { get; set; }

    public Direction TextDirection { get; set; }

    public Dictionary<string, List<string>> _credits = [];
    public override void Draw() {
        if (OutOfBounds || !Animate) return;

        _yOffset = 0; // Reset for rendering each frame
        foreach ((string header, List<string> credits) in _credits) {
            // Render header
            font.Size = _headerSize;
            measuredTextSize = Ttf.MeasureString(font, header);
            RenderText(Position.X + (Position.W / 2) - (measuredTextSize.Width / 2), Position.Y + _yOffset, header);
            _yOffset += _headerSize * 1.5f; // Space after header

            // Render credits
            foreach (string credit in credits) {
                font.Size = _creditSize;
                measuredTextSize = Ttf.MeasureString(font, credit);
                RenderText(Position.X + (Position.W / 2) - (measuredTextSize.Width / 2), Position.Y + _yOffset, credit);
                _yOffset += _creditSize * 1.2f; // Space between credits
            }
            _yOffset += _headerSize; // Extra space between sections
        }

#if DEBUG
        FRect fRect = Position;
        Program.SetColor(Colors.Black);
        Sdl.RenderRect(rendererPtr, ref fRect);
#endif
    }

    public void AddHeader(string header) {
        if (string.IsNullOrEmpty(header)) return;
        if (_credits.ContainsKey(header)) throw new ArgumentException($"Sequence already contains {header}");
        _credits.Add(header, []);
        _maxYOffset += _headerSize * 1.5f; // Account for header spacing
    }

    public void AddCredit(string header, string name) {
        if (string.IsNullOrEmpty(header) || string.IsNullOrEmpty(name)) return;
        if (!_credits.ContainsKey(header)) {
            AddHeader(header);
        }
        _credits[header].Add(name);
        _maxYOffset += _creditSize * 1.2f; // Account for credit spacing
        //_maxYOffset += _headerSize; // Account for section spacing
    }

    public void AddCredits(string header, params string[] names) {
        foreach (string name in names) {
            AddCredit(header, name);
        }
    }

    public void Reset() {
        OutOfBounds = false;
        _yOffset = 0;
    }

    public override void Update(Event sdlEvent) {
        if (OutOfBounds) return;
        CenterObjectX();

        float sX, sY;
        if (Animate) {
            switch (TextDirection) {
                case Direction.Down:
                    sX = Position.X;
                    sY = Position.Y + Speed;
                    OutOfBounds = Position.Y > Program.Height + Buffer;
                    break;
                case Direction.Up:
                    sX = Position.X;
                    sY = Position.Y - Speed;
                    OutOfBounds = Position.Y + Position.H < -Buffer;
                    break;
                case Direction.Left:
                    sX = Position.X - Speed;
                    sY = Position.Y;
                    OutOfBounds = Position.X + Position.W < -Buffer;
                    break;
                case Direction.Right:
                    sX = Position.X + Speed;
                    sY = Position.Y;
                    OutOfBounds = Position.X > Program.Width;
                    break;
                case Direction.UpLeft:
                    sX = Position.X - Speed;
                    sY = Position.Y - Speed;
                    OutOfBounds = Position.X + Position.W < -Buffer && Position.Y + Position.H < -Buffer;
                    break;
                case Direction.DownLeft:
                    sX = Position.X - Speed;
                    sY = Position.Y + Speed;
                    OutOfBounds = Position.X + Position.W < -Buffer && Position.Y > Program.Height;
                    break;
                case Direction.DownRight:
                    sX = Position.X + Speed;
                    sY = Position.Y + Speed;
                    OutOfBounds = Position.X > Program.Width && Position.Y > Program.Height;
                    break;
                case Direction.UpRight:
                    sX = Position.X + Speed;
                    sY = Position.Y - Speed;
                    OutOfBounds = Position.X > Program.Width && Position.Y + Position.H < -Buffer;
                    break;
                default:
                    sX = Position.X;
                    sY = Position.Y;
                    OutOfBounds = false;
                    break;
            }

            Animate = !OutOfBounds;

            UpdateSize(400, _maxYOffset*1.3f);
            UpdatePosition(sX, sY);
        }
    }
}
