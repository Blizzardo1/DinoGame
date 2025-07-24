using SharpSDL3.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoGame {
    internal class Biome(nint renderer, float scale) : Entity(renderer,
            Path.Combine("Tilesets", "Tilesets", "PS_Tileset_07.png"), 16, 16, 0, 0, 0, 0, scale) {

        public override void Draw() {
            // Theoretically, 120 tiles across the bottom
            for (int i = 0; i <= Program.Width / TileSet!.TileWidth / Scale; i++) {
                TileSet.RenderTile(17, 0, i * TileSet.TileWidth * Scale, Position.Y, Scale);
            }
        }

        public override void Attack(Entity? entity) {
        }

        public override void Die() {
        }

        public override void ResetEntity() {
        }

        public override void Update(Event sdlEvent) {
            Position = Position with {
                X = 0,
                Y = Program.Height - TileSet!.TileHeight * Scale,
                W = Program.Width,
                H = TileSet.TileHeight * Scale
            }; // Update the position to cover the bottom of the screen
        }
    }
}
