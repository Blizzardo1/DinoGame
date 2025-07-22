using SharpSDL3;
using SharpSDL3.Enums;
using SharpSDL3.Structs;
using SharpSDL3.Image;
using SharpSDL3.TTF;

namespace DinoGame;

internal class TileSet {
    private const int TileWidth = 32;
    private const int TileHeight = 32;

    /// <summary>
    /// Pointer to the image surface loaded from the tileset image file.
    /// </summary>
    private nint _imagePtr;
    private nint _texturePtr;
    private nint _rendererPtr;

    public TileSet(nint rendererPtr, string image) {
        _rendererPtr = rendererPtr;

        _imagePtr = Sdl.LoadImage(image);
        if (_imagePtr == nint.Zero) {
            Sdl.LogError(LogCategory.Error, $"Failed to load image: {image}. {Sdl.GetError()}");
        } else {
            Sdl.LogInfo(LogCategory.Application, $"TileSet image loaded successfully: {image}");
        }

        _texturePtr = Sdl.CreateTextureFromSurface(rendererPtr, _imagePtr);
        if (_texturePtr == nint.Zero) {
            Sdl.LogError(LogCategory.Error, $"Failed to create texture from surface. {Sdl.GetError()}");
        }
    }

    ~TileSet() {
        if (_texturePtr != nint.Zero) {
            Sdl.DestroyTexture(_texturePtr);
            _texturePtr = nint.Zero;
        }
        if (_imagePtr != nint.Zero) {
            Sdl.DestroySurface(_imagePtr);
            _imagePtr = nint.Zero;
        }
    }

    public Surface GetSurface(int tileX, int tileY) {
        if (_imagePtr == nint.Zero) {
            Sdl.LogError(LogCategory.Error, "TileSet image not loaded.");
            return default;
        }
        // Calculate the source rectangle for the tile
        FRect srcRect = new() {
            X = tileX * TileWidth,
            Y = tileY * TileHeight,
            W = TileWidth,
            H = TileHeight
        };
        // Create a new surface for the tile
        nint tileSurfacePtr = Sdl.CreateSurface(TileWidth, TileHeight, PixelFormat.Rgba8888);
        Sdl.BlitSurface(_imagePtr, Sdl.StructureToPointer(ref srcRect), tileSurfacePtr, Sdl.StructureToPointer(ref srcRect));
        if (tileSurfacePtr == nint.Zero) {
            Sdl.LogError(LogCategory.Error, $"Failed to create tile surface. {Sdl.GetError()}");
            return default;
        }
        
        return Sdl.PointerToStructure<Surface>(tileSurfacePtr);
    }

    public void RenderTile(int tileX, int tileY, int x, int y, float scale = 1) {
        if (_imagePtr == nint.Zero) {
            Sdl.LogError(LogCategory.Error, "TileSet image not loaded.");
            return;
        }
        
        // Calculate the source rectangle for the tile
        FRect srcRect = new() {
            X = tileX * TileWidth,
            Y = tileY * TileHeight,
            W = 256,
            H = 256
        };

        // Calculate the destination rectangle for rendering
        FRect dstRect = new() {
            X = x,
            Y = y,
            W = TileWidth * scale,
            H = TileHeight * scale
        };

        Sdl.RenderTextureTiled(_rendererPtr, _texturePtr, ref srcRect, scale, ref dstRect);

        /*
        Sdl.SetRenderDrawColor(_rendererPtr, 255, 255, 0, 255);
        Sdl.RenderRect(_rendererPtr, ref dstRect);
        */
    }
}


internal static class Program {
    private static nint _windowPtr;
    private static nint _rendererPtr;
    private static bool _running;
    private static bool _isOnGround = true;
    private const float Scale = 4f; // Scale factor for rendering tiles
    private static float _velocityY = 0f; // Vertical velocity for the jump
    private static float _positionY = FloorY;

    private static int[] _animFrames = [0, 1, 0, 2];
    private static int _animDelay = 1;
    private static int _currDelay = 0;
    private static Font _font;

    private static FRect _playerRect;
    private static FRect _floorRect;
    private static FRect _smallCactiRect;
    private static FRect _largeCactiRect;

    /// <summary>
    /// The width of the game window in pixels.
    /// </summary>
    private static int Width { get; set; }

    /// <summary>
    /// The height of the game window in pixels.
    /// </summary>
    private static int Height { get; set; }

    private static float FloorY => Height - FloorHeight - (32 * Scale);

    // private static Animation _animation = new ();
    private const float Gravity = .25f; // Gravity effect on the jump
    private const float FloorHeight = 8 * Scale; // Height of the floor in pixels

    private static Random _rand = new ();

    private static TileSet _tileset;

    private static FRect smallScrollRect = new() {
        X = 0,
        Y = 0,
        W = 1920 / Scale,
        H = 1080 / Scale
    };

    private static FRect largeScrollRect = new() {
        X = 0,
        Y = 0,
        W = 1920 / Scale,
        H = 1080 / Scale
    };

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

    // private static void SetupAnimation() {
    //     _animation.Frames = [
    //         _tileset.GetSurface(0,0),
    //         _tileset.GetSurface(1,0),
    //         _tileset.GetSurface(0,0),
    //         _tileset.GetSurface(2,0),
    //         ];
    //     _animation.Delays = [100, 100, 100, 100]; // Delay in milliseconds for each frame
    // }

    private static void SetColor(byte r, byte g, byte b, byte a = 255) => Sdl.SetRenderDrawColor(_rendererPtr, r, g, b, a);
    private static void SetColor(Color color) => Sdl.SetRenderDrawColor(_rendererPtr, color);

    private static void Scroll() {
        smallScrollRect.X += 1f * Scale;
        largeScrollRect.X += 1f * Scale;
        Sdl.Delay(30);
    }

    private static float CalculateJump() {
        if (!_isOnGround) {
            _velocityY += Gravity;
            _positionY += _velocityY;

            if (_positionY < FloorY - 250) {
                _positionY = FloorY - 250; // Limit the jump height
                _velocityY = 0f;
            }
        }

        return _positionY;
    }

    private static void UpdateWindowDimensions() {
        Sdl.GetWindowSize(_windowPtr, out int width, out int height);
        Width = width;
        Height = height;
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

    private static int _currentIndex = 0;
    private static void Draw() {
        
        SetColor(0, 0, 0, 255); // Clear to black
        Sdl.RenderClear(_rendererPtr);


        _tileset.RenderTile(_animFrames[_currentIndex], 0, (int)_playerRect.X, (int)_playerRect.Y, Scale);

        for(float i = 0; i < Width; i += 32 * Scale) {
            _tileset.RenderTile(3, 1, (int)i, (int)_floorRect.Y, Scale);
        }

        _tileset.RenderTile(0, 1, (int)_smallCactiRect.X, (int)_smallCactiRect.Y, Scale);
        _tileset.RenderTile(1, 1, (int)_largeCactiRect.X, (int)_largeCactiRect.Y, Scale);

        RenderText(5, 2, $"Is On Ground? {_isOnGround}");
        RenderText(5, 18, $"Position Y: {_positionY}");
        RenderText(5, 34, $"Velocity: {_velocityY}");
        
        Sdl.RenderPresent(_rendererPtr);
    }

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
                    case Scancode.Space:
                        _isOnGround = false;
                        _velocityY = -12f; // Set an initial upward velocity for the jump
                        break;
                }
                break;
            case EventType.KeyUp:
                switch (sdlEvent.Key.ScanCode) {
                    case Scancode.Space:
                        if(_velocityY < -6f) {
                            _velocityY = -6f; // Limit the jump speed when the key is released
                        }
                        break;
                }
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

        _playerRect = _playerRect with { X = 200, Y = CalculateJump(), W = 32 * Scale, H = 32 * Scale };
        _floorRect = _floorRect with { X = 0, Y = Height - (32 * Scale), W = Width / Scale, H = 32 };
        _smallCactiRect = _smallCactiRect with { X = 1000 - smallScrollRect.X, Y = FloorY, W = 32 * Scale, H = 32 * Scale };
        _largeCactiRect = _largeCactiRect with { X = 1500 - largeScrollRect.X, Y = FloorY, W = 64 * Scale, H = 64 * Scale };

        if (_smallCactiRect.X + _smallCactiRect.W < 0) {
            smallScrollRect.X = -_rand.Next(Width / 2, (Width / 2) + 64);
        }

        if (_largeCactiRect.X + _largeCactiRect.W < 0) {
            largeScrollRect.X = -_rand.Next(Width / 2, (Width / 2) + 90);
        }
        if (_currDelay > _animDelay) {
            _currDelay = 0;
            _currentIndex = (_currentIndex + 1) % _animFrames.Length;
            return; // Skip the frame if the delay is not met
        }
        _currDelay++;

        if (_currentIndex > _animFrames.Length) {
            _currentIndex = 0;
        }

        Scroll();
        if(_positionY < FloorY) {
            _isOnGround = false;
        } else {
            _isOnGround = true;
            _positionY = FloorY; // Reset position to floor level
            _velocityY = 0f; // Reset vertical velocity when on the ground
        }
    }

    private static void Cleanup() {
        if(_font.Handle != nint.Zero) {
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

    /*
            X
          0 | 1 | 2 | 3 | 4 | 5 | 6 | 7  | 8 | 9 | 10
        Y 1 | 1 | 1 | 1 | 1 | 1 | 1 | 1  | 1 | 1 | 1
          2 | 1 | 4 | 9 | 16| 25| 36| 49 | 64| 81| 100
          3 | 1 | 9 | 27
     */

    private static void Main(string[] args) {
        if(!Sdl.Init(InitFlags.Everything)) {
            Sdl.LogError(LogCategory.Error, $"Failed to initialize SDL3 and its dependencies. {Sdl.GetError()}");
            return;
        }

        _windowPtr = Sdl.CreateWindow("Dino Game", 1920, 1080, WindowFlags.Resizable | WindowFlags.MouseCapture);
        
        if (_windowPtr == nint.Zero) {
            Sdl.LogError(LogCategory.Error, $"Failed to create window. {Sdl.GetError()}");
            return;
        }

        UpdateWindowDimensions();

        _rendererPtr = Sdl.CreateRenderer(_windowPtr, "opengl");

        if (_rendererPtr == nint.Zero) {
            Sdl.LogError(LogCategory.Error, $"Failed to create renderer. {Sdl.GetError()}");
            Cleanup();
            return;
        }

        Sdl.LogInfo(LogCategory.Application, $"Using renderer: {Sdl.GetRendererName(_rendererPtr)}");

        if(!Ttf.Init()) {
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

        _tileset = new (_rendererPtr, "tileset.png");
        // SetupAnimation();
        Sdl.LogInfo(LogCategory.Application, "Starting game loop...");
        _running = true;
        while (_running) {
            if (!Sdl.PollEvent(out Event @event)) {
                Sdl.LogDebug(LogCategory.Application, "No events to process.");
            }
            Update(@event);
            Draw();
        }

        Sdl.LogInfo(LogCategory.Application, "Exiting game loop.");
        Cleanup();
    }
}