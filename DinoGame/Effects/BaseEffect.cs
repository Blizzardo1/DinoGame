using DinoGame.GameObjects;

namespace DinoGame.Effects; 
internal abstract class BaseEffect : GameObject {

    public BaseEffect(nint rendererPtr, float scale) : base(rendererPtr) {
        Scale = scale;
        
    }
}
