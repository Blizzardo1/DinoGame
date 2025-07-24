using SharpSDL3;
using SharpSDL3.Enums;
using SharpSDL3.Structs;
using SharpSDL3.TTF;
using System.Runtime.CompilerServices;
using System.Xml;

namespace DinoGame;

internal static class Program {
    internal static Random Rand = new();
    private const int FramesPerSecond = 20;
    private const float Scale = 4f; // Scale factor for rendering tiles

    private static readonly FpsTimer _fps = new();
    private static Biome? _biome;
    private static List<Enemy>? _enemies;
    private static Font _font;
    private static Player? _player;
    private static nint _rendererPtr;
    private static bool _running;
    private static nint _windowPtr;

    /// <summary>
    /// The height of the game window in pixels.
    /// </summary>
    internal static int Height { get; set; }

    /// <summary>
    /// The width of the game window in pixels.
    /// </summary>
    internal static int Width { get; set; }

    internal static float GetFloorY(TileSet tileSet) {
        if (_biome is null) return Height - (16 * Scale);

        return Height - (_biome.TileSet!.TileHeight * Scale) - (tileSet.TileHeight * Scale);
    }

    private static string ChooseRenderer() {
        var numRenderers = Sdl.GetNumRenderDrivers();
        if (numRenderers < 1) {
            Sdl.LogError(LogCategory.Error, "No render drivers available.");
            return string.Empty;
        }

        for (int i = 0; i < numRenderers; i++) {
            Sdl.LogInfo(LogCategory.Application, $"Renderer {i}: {Sdl.GetRenderDriver(i)}");
        }

        Console.Write("Enter the renderer driver name to use: ");
        return Console.ReadLine() ?? string.Empty;
    }

    private static void Cleanup() {
        if (_font.Handle != nint.Zero) {
            Ttf.CloseFont(_font);
            _font = default; // Reset the font to default
        }

        if (_rendererPtr != nint.Zero) {
            Sdl.DestroyRenderer(_rendererPtr);
            _rendererPtr = nint.Zero;
        }
        if (_windowPtr != nint.Zero) {
            Sdl.DestroyWindow(_windowPtr);
            _windowPtr = nint.Zero;
        }
        Sdl.LogInfo(LogCategory.Application, "Goodbye! :)");
        Sdl.Quit();
    }

    private static void Draw() {

        SetColor(0, 0, 0, 255); // Clear to black
        Sdl.RenderClear(_rendererPtr);

        _biome?.Draw();
        _player?.Draw();
        if (_enemies is not null) {
            foreach (Enemy e in _enemies) {
                e.Draw();
            }
        }
#if DEBUG
        DebugText(5,
            $"Player Dead? {_player!.IsDead}",
            $"Is On Ground? {_player.IsGrounded}",
            $"Is Jumping? {_player.IsJumping}",
            $"Is Maxed Jump? {_player.IsMaxJump}",
            $"Is Jump Locked? {_player.IsLockedJump}",
            $"Position Y: {_player.Position.Y}",
            $"Velocity: {_player.Velocity}");
#endif

        Sdl.RenderPresent(_rendererPtr);
    }

    private static void LoadAssets() {
        _biome = new(_rendererPtr, Scale);
        // 17, 0 is Grass

        // 0, 0 is Player Idle
        // 0, 1 is Player Walking
        // 0, 6 is Player Jumping
        // 0, 9 is Player Dying

        // 0, 0 is Enemy Idle
        // 0, 1 is Enemy Walking
        _player = new Player(_rendererPtr, Scale);
        for (int i = 0; i < 5; i++) {
            Enemy enemy = new(_rendererPtr, Scale);
            enemy.RandSpawn<Enemy>();
            _enemies ??= [];
            _enemies.Add(enemy);
        }

    }

    private static void Main(string[] args) {
        if (!Sdl.Init(InitFlags.Everything)) {
            Sdl.LogError(LogCategory.Error, $"Failed to initialize SDL3 and its dependencies. {Sdl.GetError()}");
            return;
        }

        _windowPtr = Sdl.CreateWindow("Dino Game", 1920, 1080, WindowFlags.Resizable | WindowFlags.MouseCapture);

        if (_windowPtr == nint.Zero) {
            Sdl.LogError(LogCategory.Error, $"Failed to create window. {Sdl.GetError()}");
            return;
        }

        

        _rendererPtr = Sdl.CreateRenderer(_windowPtr, "opengl");

        if (_rendererPtr == nint.Zero) {
            Sdl.LogError(LogCategory.Error, $"Failed to create renderer. {Sdl.GetError()}");
            Cleanup();
            return;
        }

        Sdl.LogInfo(LogCategory.Application, $"Using renderer: {Sdl.GetRendererName(_rendererPtr)}");

        if (!Ttf.Init()) {
            Sdl.LogError(LogCategory.Error, $"Failed to initialize TTF. {Sdl.GetError()}");
            Cleanup();
            return;
        }
        _font = Ttf.OpenFont("C:\\Windows\\Fonts\\consola.ttf", 16);
        if (_font.Handle == nint.Zero) {
            Sdl.LogError(LogCategory.Error, $"Failed to open font: consola.ttf. {Sdl.GetError()}");
            Cleanup();
            return;
        }

        LoadAssets();
        UpdateWindowDimensions();
        Sdl.LogInfo(LogCategory.Application, "Starting game loop...");
        _running = true;
        Span<bool> keyboardState = Sdl.GetKeyboardState(out int numKeys);

        while (_running) {
            keyboardState = Sdl.GetKeyboardState(out numKeys);
            _fps.Start();

            _ = Sdl.PollEvent(out Event @event);

            
            Update(@event);
            Draw();

            if (keyboardState[(int)Scancode.Space]) {
                _player!.Jump();
            } else if (!keyboardState[(int)Scancode.Space]) {
                _player!.EndJump();
            }

                uint delta = (uint)_fps.GetTicks();
            if (delta < 1000 / FramesPerSecond) {
                Sdl.Delay((1000 / FramesPerSecond) - delta);
            }
            _fps.Stop();
        }

        Sdl.LogInfo(LogCategory.Application, "Exiting game loop.");
        Cleanup();
    }


    private static void DebugText(float x, params string[] texts) {
        int y = 2;
        foreach (string text in texts) {
            RenderText(x, y, text);
            y += _font.Height;
        }
    }
    private static void RenderText(float x, float y, string text) {
        Color previousColor = Sdl.GetRenderDrawColor(_rendererPtr);
        Color currentColor = new() { R = 255, G = 255, B = 255, A = 255 };
        SetColor(currentColor);
        if (text is null || string.IsNullOrEmpty(text)) {
            return;
        }

        if (_rendererPtr == nint.Zero) {
            Sdl.LogError(LogCategory.Error, "Renderer is not initialized");
            return;
        }
        TextEngine textEngine = Ttf.CreateRendererTextEngine(_rendererPtr);
        if (textEngine.Handle == nint.Zero) {
            Sdl.LogError(LogCategory.Error, $"Error creating text engine: {Sdl.GetError()}");
            return;
        }

        Text txt = Ttf.CreateText(textEngine, _font, text);

        Ttf.SetTextColor(txt, currentColor);
        Ttf.DrawRendererText(txt, x, y);

        Ttf.DestroyText(txt);
        Ttf.DestroyRendererTextEngine(textEngine);
        SetColor(previousColor);
    }

    private static void Reset() {
        foreach(Enemy e in _enemies!) {
            e.ResetEntity();
        }

        _player!.ResetEntity();
    }

    internal static void SetColor(byte r, byte g, byte b, byte a = 255) => Sdl.SetRenderDrawColor(_rendererPtr, r, g, b, a);
    internal static void SetColor(Color color) => Sdl.SetRenderDrawColor(_rendererPtr, color);

    private static void Update(Event sdlEvent) {
        switch (sdlEvent.Type) {
            case EventType.First:
                break;
            case EventType.Quit:
                _running = false;
                break;
            case EventType.KeyDown:
                switch (sdlEvent.Key.ScanCode) {
                    case Scancode.Escape:
                        _running = false;
                        break;
                    case Scancode.F2:
                        Reset();
                        break;
                }
                break;
            case EventType.KeyUp:
                break;
            case EventType.WindowShown:
                break;
            case EventType.WindowHidden:
                break;
            case EventType.WindowExposed:
                break;
            case EventType.WindowMoved:
                break;
            case EventType.WindowResized:
                UpdateWindowDimensions();
                break;
            case EventType.WindowMinimized:
                break;
            case EventType.WindowMaximized:
                break;
            case EventType.WindowRestored:
                break;
            case EventType.WindowMouseEnter:
                break;
            case EventType.WindowMouseLeave:
                break;
            case EventType.WindowFocusGained:
                break;
            case EventType.WindowFocusLost:
                break;
            case EventType.WindowCloseRequested:
                break;
            case EventType.PollSentinel:
                break;
            case EventType.Last:
                break;
            default:
                break;
        }

        if(_player!.IsDead) {
            if (!_player.IsGrounded) {
                _player.EndJump();
            }
            string message = "You are Dead. Press F2 to restart";
            Size sz = Ttf.MeasureString(_font, message);
            RenderText((Width / 2) - (sz.Width / 2), (Height / 2) - (sz.Height / 2), message);
            return;
        }

        _biome?.Update(sdlEvent);
        _player?.Update(sdlEvent);
        if (_enemies is not null) {
            foreach (Enemy e in _enemies) {
                e.Update(sdlEvent);
                if(e.Collides(_player)) {
                    e.Attack(_player!);
                }
            }
        }
    }

    private static void UpdateWindowDimensions() {
        Sdl.GetWindowSize(_windowPtr, out int width, out int height);
        Width = width;
        Height = height;
        _player?.UpdateDimensions();
        if (_enemies is not null) {
            foreach (Enemy e in _enemies) {
                e.UpdateDimensions();
            }
        }
    }
}