using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System;

namespace Choplifter
{
    class Shot : ModelEntity
    {
        Timer LifeTimer;

        public Shot(Game game, Camera camera, Model model) : base(game, camera, model)
        {
            LifeTimer = new Timer(game);
        }

        public override void Initialize()
        {
            base.Initialize();

            Enabled = false;
            //Radius = 2;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (LifeTimer.Elapsed)
                Enabled = false;
        }

        public void Spawn(Vector3 postion, Vector3 direction, float timer)
        {
            base.Spawn(postion, Vector3.Zero, direction);
            Vector3 acc = Acceleration;
            LifeTimer.Reset(timer);
        }
    }
}
