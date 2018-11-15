using Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sprites
{
    class RandomEnemy : Enemy
    {
        public RandomEnemy(Game g, Texture2D texture, Vector2 userPosition, int framecount ) : 
            base(g,texture,userPosition,framecount)
        {
            TargetPosition = new Vector2(Helper.staticRandom.Next(0,g.GraphicsDevice.Viewport.Bounds.Size.X),
                                         Helper.staticRandom.Next(0, g.GraphicsDevice.Viewport.Bounds.Size.Y));

        }

        public override void Update(GameTime gametime)
        {
            // If we are close enoough to the target
            if(Math.Abs(Vector2.Distance(position,TargetPosition)) < 2)
            {
                // Slide the last bit of the way in
                position = TargetPosition;
                // Generate a new Random Target Position
                TargetPosition = new Vector2(Helper.staticRandom.Next(0, myGame.GraphicsDevice.Viewport.Bounds.Size.X),
                                 Helper.staticRandom.Next(0, myGame.GraphicsDevice.Viewport.Bounds.Size.Y));
            }
            // Continue towards the Target
            else position = Vector2.Lerp(position,TargetPosition,0.1f);

            base.Update(gametime);
        }
    }
}
