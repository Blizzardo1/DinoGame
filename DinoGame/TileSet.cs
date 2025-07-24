using SharpSDL3;
using SharpSDL3.Enums;
using SharpSDL3.Structs;
using System.IO;

namespace DinoGame;

internal class TileSet {
    private const int CTileWidth = 32;
    private const int CTileHeight = 32;

    /// <summary>
    /// Pointer to the image surface loaded from the tileset image file.
    /// </summary>
    private nint _imagePtr;
    private nint _texturePtr;
    private nint _rendererPtr;
    private int _xOffset;
    private int _yOffset;
    private int _xSpacing;
    private int _ySpacing;

    public int TileWidth { get; }
    public int TileHeight { get; }

    public int Width { get; }
    public int Height { get; }

    public TileSet(nint rendererPtr, string image,
        int tileWidth = CTileWidth, int tileHeight = CTileHeight,
        int xOffset = 0, int yOffset = 0,
        int xSpacing = 0, int ySpacing = 0,
        FlipMode flipMode = FlipMode.None) {
        _rendererPtr = rendererPtr;

        _imagePtr = Sdl.LoadImage(image);
        if (_imagePtr == nint.Zero) {
            Sdl.LogError(LogCategory.Error, $"Failed to load image: {image}. {Sdl.GetError()}");
            return;
        }

        Sdl.FlipSurface(_imagePtr, flipMode);
        _texturePtr = Sdl.CreateTextureFromSurface(rendererPtr, _imagePtr);
        if (_texturePtr == nint.Zero) {
            Sdl.LogError(LogCategory.Error, $"Failed to create texture from surface. {Sdl.GetError()}");
            return;
        }

        _xOffset = xOffset;
        _yOffset = yOffset;
        _xSpacing = xSpacing;
        _ySpacing = ySpacing;

        Surface surface = Sdl.PointerToStructure<Surface>(_imagePtr);
        Width = surface.W;
        Height = surface.H;

        TileWidth = tileWidth;
        TileHeight = tileHeight;

        Sdl.LogInfo(LogCategory.Application, $"Tileset \"{image}\" Width: {Width}; Height: {Height}, TileWidth: {TileWidth}; TileHeight: {TileHeight}");
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

    public void RenderTile(int tileX, int tileY, float x, float y, float scale = 1) {
        if (_imagePtr == nint.Zero) {
            Sdl.LogError(LogCategory.Error, "TileSet image not loaded.");
            return;
        }
        
        // Calculate the source rectangle for the tile
        FRect srcRect = new() {
            X = tileX * TileWidth + _xOffset + _xSpacing,
            Y = tileY * TileHeight + _yOffset + _ySpacing,
            W = Width,
            H = Height
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
