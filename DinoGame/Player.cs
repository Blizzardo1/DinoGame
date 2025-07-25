using SharpSDL3;
using SharpSDL3.Enums;
using SharpSDL3.Structs;

namespace DinoGame;

public class Player : Entity {
    private const float Gravity = 0.25f;
    private const float CVelocityUp = -48f;
    private const float CVelocityDown = 24f;
    private float _velocity = 0f;
    private bool _isGrounded = true;
    private bool _isJumping = false;
    private bool _isMaxJump = false;
    private bool _lockedJump = false;
    private int _score = 0;

    private float MaxHeight => Program.GetFloorY(TileSet) - 400;

    public bool IsGrounded => _isGrounded;
    public bool IsJumping => _isJumping;
    public bool IsMaxJump => _isMaxJump;
    public bool IsLockedJump => _lockedJump;
    public bool IsDead => _isDead;

    public float Velocity => _velocity;

    public int Score => _score;


    public Player(nint renderer, float scale) : base(renderer,
            Path.Combine("Tilesets", "Heroes", "Hero 007", "Hero_007.png"),
            32, 32, scale: scale) {
        ResetEntity();
    }

    public void AddScore(int score) {
        _score += score;
    }

    public void Jump() {
        if(_isDead) return;

        Animation = 6;

        if(!_isMaxJump) {
            _lockedJump = true;
            _isGrounded = false; // Player is no longer grounded
            _isJumping = true;
        }

        if (_lockedJump && !_isMaxJump && Position.Y < MaxHeight) {
            _isMaxJump = true;
            _isJumping = false;
            _velocity = CVelocityDown;
        } else if(!_lockedJump) {
            _velocity = CVelocityUp - Gravity; // Reset velocity for jump
        }
        
    }

    public void EndJump() {
        _isJumping = false;
        _lockedJump = false;
        if (!_isGrounded) {
            _velocity = CVelocityDown + Gravity; // Stop upward movement
        }
    }

    public override void Draw() {
        if (TileSet is null) {
            return;
        }

        TileSet.RenderTile(Frame, Animation, (int)Position.X, (int)Position.Y, Scale);
#if DEBUG
        Program.SetColor(255, 255, 128);
        FRect fRect = Position;
        FRect fHitbox = HitBox;
        Sdl.RenderRect(rendererPtr, ref fRect);
        Program.SetColor(255, 0, 0);
        Sdl.RenderRect(rendererPtr, ref fHitbox);
        Program.SetColor(255,128,128);
        Sdl.RenderLine(rendererPtr, 0, MaxHeight, Program.Width, MaxHeight);
#endif
    }

    public override void Attack(Entity? entity) {
        
    }

    public override void Die() {
        Animation = 9;
        _isDead = true;
    }

    public override void ResetEntity() {
        Animation = 1;
        _isDead = false;
        _score = 0;
        UpdateDimensions();
    }

    public override void Update(Event sdlEvent) {
        if (TileSet is null) {
            return;
        }

        if (!_isGrounded && _isJumping && !_isMaxJump) {
            _velocity = CVelocityUp;
            Position = Position with { Y = Position.Y + _velocity };
        }

        if(!_isJumping && !_isGrounded || _isMaxJump) {
            _velocity = CVelocityDown;
            Position = Position with { Y = Position.Y + _velocity };
        }

        if (!_isJumping && Position.Y + Position.H >= Program.GetFloorY(TileSet)) {
            Position = Position with { Y = Program.GetFloorY(TileSet) }; // Prevent going below the ground
            _velocity = 0f;
            _isGrounded = true; // Player is grounded again
            _isMaxJump = false;
            if (!_isDead) {
                Animation = 1; // Reset to walking animation
            }
        }

        UpdateHitbox();

        Frame++;
        if(_isDead && Frame >= TileSet.Width / TileSet.TileWidth ) {
            Frame = TileSet.Width / TileSet.TileWidth;
        }
        if (Frame >= TileSet.Width / TileSet.TileWidth) {
            Frame = 0;
        }
    }
}
