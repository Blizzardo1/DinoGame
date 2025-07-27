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
using DinoGame.Entities;
using SharpSDL3;
using SharpSDL3.Enums;
using SharpSDL3.Structs;
using SharpSDL3.TTF;
using System.Diagnostics;

namespace DinoGame;

internal static class Program {
    private const string GameName = "Cat Evader";
    internal static Random Rand = new();
    private const int FramesPerSecond = 20;
    private const float Scale = 4f; // Scale factor for rendering tiles
    internal const float FontSize = 16f;


    private static readonly FpsTimer _fps = new();
    private static Biome? _biome;
    private static List<Enemy>? _enemies;
    private static Font _font;
    private static Player? _player;
    private static nint _rendererPtr;
    private static nint _windowPtr;

    private static bool _isRunning;
    private static bool _isFirstRun = true;
    private static bool _isTutorial = true;
    private static bool _isLicenseShown = false;
    private static bool _isPaused = false;

    private static ZoomTextEffect _licenseText;
    private static ZoomTextEffect _tutorialText;
    private static StaticTextEffect _notificationText;
    private static StaticTextEffect _gameTitle;
    private static StaticTextEffect _score;
    private static StaticTextEffect _memoryUsage;

    internal static Player? Player => _player;
    internal static Biome? Biome => _biome;

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
        debugText.ForEach(t => t.Dispose());
        debugText.Clear();

        enemyText.ForEach(t => t.Dispose());
        enemyText.Clear();

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

        SetColor(_biome!.CurrentBackgroundColor);
        Sdl.RenderClear(_rendererPtr);

        _biome?.Draw();
        _player?.Draw();
        if (_enemies is not null) {
            foreach (Enemy e in _enemies) {
                e.Draw();
            }
        }
#if DEBUG
        UpdateDebugText(debugText,
            $"Ticks: {Sdl.GetTicks()}",
            $"Player Dead? {_player!.IsDead}",
            $"Is On Ground? {_player.IsGrounded}",
            $"Is Jumping? {_player.IsJumping}",
            $"Is Maxed Jump? {_player.IsMaxJump}",
            $"Is Jump Locked? {_player.IsLockedJump}",
            $"Position Y: {_player.Position.Y}",
            $"Velocity: {_player.Velocity}",
            $"Alpha: {_biome!.ColorFadeEffect.GetAlpha()}");
        List<string> enemies = [];
        for (int i = 0; i < _enemies!.Count; i++) {
            Enemy enemy = _enemies![i];
            enemies.Add($"Enemy {i + 1} Passed? {enemy.IsPassed}");
        }
        UpdateDebugText(enemyText, [..enemies]);
        debugText.ForEach(t => t.Draw());
        enemyText.ForEach(t => t.Draw());
#endif
        _score.Draw();
        _memoryUsage.Draw();

        if (!_player!.IsDead && _isTutorial && !_isFirstRun) {
            _tutorialText.Draw();
        }

        if (_player!.IsDead) {
            _notificationText.Draw();
        }

        _gameTitle.Draw();

        if (_isLicenseShown) {
            _licenseText.Draw();
        }

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
        _memoryUsage = new(_rendererPtr, $"Memory Usage: 0 MB", _font, FontSize, Scale);
        
        _gameTitle = new(_rendererPtr, GameName, _font, FontSize * 6f, Scale);

        _tutorialText = new(_rendererPtr, "Press space to evade cats!", _font, 1.5f, 2f, Scale);

        _licenseText = new(_rendererPtr,
            @"DinoGame version 1, Copyright (C) 2025 Adonis Deliannis (Blizzardo1)
DinoGame comes with ABSOLUTELY NO WARRANTY; This is free software,
and you are welcome to redistribute it under certain conditions.", _font, 0.02f, 1.2f, Scale);
        _notificationText = new(_rendererPtr, $"Welcome to {GameName}! Press F2 to begin", _font, FontSize * 2, Scale);
        _score = new(_rendererPtr, "Score: 0", _font, FontSize * 1.5f, Scale);
        
        _player = new Player(_rendererPtr, Scale);
        for (int i = 0; i < 5; i++) {
            Enemy enemy = new(_rendererPtr, Scale);
            enemy.RandSpawn<Enemy>();
            _enemies ??= [];
            _enemies.Add(enemy);
        }

        debugText = CreateDebugText(5,
    $"Ticks: {Sdl.GetTicks()}",
    $"Player Dead? {_player!.IsDead}",
    $"Is On Ground? {_player.IsGrounded}",
    $"Is Jumping? {_player.IsJumping}",
    $"Is Maxed Jump? {_player.IsMaxJump}",
    $"Is Jump Locked? {_player.IsLockedJump}",
    $"Position Y: {_player.Position.Y}",
    $"Velocity: {_player.Velocity}",
    $"Alpha: {_biome!.ColorFadeEffect.GetAlpha()}");
        List<string> enemies = [];
        for (int i = 0; i < _enemies!.Count; i++) {
            Enemy enemy = _enemies![i];
            enemies.Add($"Enemy {i + 1} Passed? {enemy.IsPassed}");
        }
        enemyText = CreateDebugText(Width / 1.5f, [.. enemies]);

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

        _font = Ttf.OpenFont("C:\\Windows\\Fonts\\consola.ttf", FontSize);
        if (_font.Handle == nint.Zero) {
            Sdl.LogError(LogCategory.Error, $"Failed to open font: consola.ttf. {Sdl.GetError()}");
            Cleanup();
            return;
        }

        UpdateWindowDimensions();
        LoadAssets();
        
        Sdl.LogInfo(LogCategory.Application, "Starting game loop...");
        _isRunning = true;
        Span<bool> keyboardState;
        while (_isRunning) {
            keyboardState = Sdl.GetKeyboardState(out _);
            _fps.Start();

            _ = Sdl.PollEvent(out Event @event);


            Update(@event);
            Draw();

            if (keyboardState[(int)Scancode.Space]) {
                if (_isTutorial) {
                    _isTutorial = false;
                }
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


    private static List<StaticTextEffect> debugText = [];
    private static List<StaticTextEffect> enemyText = [];

    private static void UpdateDebugText(List<StaticTextEffect> lst, params string[] texts) {
        for (int i = 0; i < texts.Length; i++) {
            lst[i].Text = texts[i];
        }
    }

    private static List<StaticTextEffect> CreateDebugText(float x, params string[] texts) {
        List<StaticTextEffect> lst = [];
        float y = 2;
        foreach (string text in texts) {
            StaticTextEffect tEffect = new(_rendererPtr, text, _font, FontSize, Scale);
            tEffect.UpdatePosition(x, y);
            lst.Add(tEffect);
            y += FontSize;
        }
        return lst;
    }
    

    private static void Reset() {
        foreach(Enemy e in _enemies!) {
            e.ResetEntity();
        }
        _biome!.DeathTriggered = false;
        _biome.ResetTriggered = true;
        _biome.ResetEntity();
        _player!.ResetEntity();
        _biome.ColorFadeEffect.Trigger();
    }

    internal static void SetColor(byte r, byte g, byte b, byte a = 255) => Sdl.SetRenderDrawColor(_rendererPtr, r, g, b, a);
    internal static void SetColor(Color color) => Sdl.SetRenderDrawColor(_rendererPtr, color);

    private static void Update(Event sdlEvent) {
        switch (sdlEvent.Type) {
            case EventType.First:
                break;
            case EventType.Quit:
                _isRunning = false;
                break;
            case EventType.KeyDown:
                switch (sdlEvent.Key.ScanCode) {
                    case Scancode.Escape:
                        if(_isLicenseShown) {
                            _isPaused = false;
                            _licenseText.IsGrowing = false;
                            _licenseText.Animated = true;
                            break;
                        }
                        _isRunning = false;
                        break;
                    case Scancode.F2:
                        if (_isFirstRun) {
                            _isFirstRun = false;
                        }
                        _biome!.FramesToTriggerFade = 0;
                        Reset();
                        break;

                    case Scancode.F1:
                        _isPaused = true;
                        _isLicenseShown = true;
                        _licenseText.Animated = true;
                        _licenseText.UseUpdate = false;
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

        if (_isLicenseShown) {
            if (!_licenseText.Animated && !_isPaused) {
                _isLicenseShown = false;
            }
            if (_licenseText.IsGrowing) {
                _licenseText.Grow();
            } else if (!_licenseText.IsGrowing) {
                _licenseText.Shrink();
            }

            _licenseText.Update(sdlEvent);

            _licenseText.UpdatePosition(
                 (Width / 2) - (_licenseText.Position.W / 2),
                (Height / 2) - (_licenseText.Position.H / 2)
            );

            return;
        }

#if DEBUG
        debugText.ForEach(t => t.Update(sdlEvent));
        enemyText.ForEach(t => t.Update(sdlEvent));
#endif
        _biome?.Update(sdlEvent);

        _score.Text = $"Score: {_player!.Score}";
        _score.Update(sdlEvent);
        _score.UpdatePosition(Width - (_score.Position.W + 4), _score.Position.Y);

        _memoryUsage.Text = $"Memory: {Process.GetCurrentProcess().PrivateMemorySize64 / 1024 / 1024}MB";
        _memoryUsage.Update(sdlEvent);
        _memoryUsage.UpdatePosition(Width - (_memoryUsage.Position.W + 4), _score.Position.Y + _score.Position.H + 12);

        _gameTitle.Text = GameName;
        _gameTitle.Update(sdlEvent);
        _gameTitle.UpdatePosition((Width / 2) - (_gameTitle.Position.W / 2), _gameTitle.Position.Y);

        if (_isTutorial) {
            Enemy? enemy = _enemies!.FirstOrDefault();
            if (enemy is not null && _isFirstRun) {
                if (enemy.Position.X > 1000) {
                    enemy.UpdatePosition(1000, enemy.Position.Y);
                }
            }
            if(!_tutorialText.Animated)
                _tutorialText.Animated = true;
            _tutorialText.Update(sdlEvent);
            _tutorialText.UpdatePosition(
                (Width / 2) - (_tutorialText.Position.W / 2),
                (Height / 2) - (_tutorialText.Position.H / 2)
            );
            
        }

        if(_player!.IsDead) {
            _notificationText!.Text = _isFirstRun ? $"Welcome to {GameName}! Press F2 to begin"
                : "You are Dead. Press F2 to restart";
            _notificationText.Update(sdlEvent);
            _notificationText.UpdatePosition(
                (Width / 2) - (_notificationText.Position.W / 2),
                (Height / 2) - (_notificationText.Text.Length / 2)
            );

            if (!_biome!.ColorFadeEffect.IsTriggered && !_biome.DeathTriggered) {
                _biome.DeathTriggered = true;
                _biome.FramesToTriggerFade = 0;
                _biome.ColorFadeEffect.Trigger();
            }

            if (!_player.IsGrounded) {
                _player.EndJump();
            }
            _player.Update(sdlEvent);
            Enemy? enemy = _enemies.FirstOrDefault(e => e.IsAttacking);
            enemy?.Update(sdlEvent);
            return;
        }


        _player?.Update(sdlEvent);
        if (_enemies is not null) {
            foreach (Enemy e in _enemies) {
                e.Update(sdlEvent);

                if(e.IsPassed) {
                    continue;
                }

                if(e.EndsCollisionX(_player!)) {
                    _player!.AddScore(1);
                }

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