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

namespace DinoGame;

internal class FpsTimer {
    private bool _running;
    private bool _paused;
    private ulong _startTicks;
    private ulong _pausedTicks;


    public bool Running => _running;
    public bool Paused => _paused;

    public void Start() {
        _running = true;
        _paused = false;
        _startTicks = Sdl.GetTicks();
    }

    public void Stop() {
        _running = false;
        _paused = false;
        _startTicks = 0;
        _pausedTicks = 0;
    }

    public void Pause() {
        if (_running && !_paused) {
            _paused = true;
            _pausedTicks = Sdl.GetTicks() - _startTicks;
        }
    }

    public void Unpause() {
        if (_running && _paused) {
            _paused = false;
            _startTicks = Sdl.GetTicks() - _pausedTicks;
            _pausedTicks = 0;
        }
    }

    public ulong GetTicks() {
        if (!_running) {
            return 0;
        }

        if (_paused) {
            return _pausedTicks;
        }

        return Sdl.GetTicks() - _startTicks;
    }
}
