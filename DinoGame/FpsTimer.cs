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
