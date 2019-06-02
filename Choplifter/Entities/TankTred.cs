using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System;

namespace Choplifter
{
    class TankTred : PositionedObject
    {
        ModelEntity[] TredAnimations = new ModelEntity[2];
        Camera CameraRef;
        Timer AnimationTimer;

        public bool Moving;

        public TankTred(Game game, Camera camera) : base(game)
        {
            CameraRef = camera;
            AnimationTimer = new Timer(game, 0.1f);

            for (int i = 0; i < 2; i++)
            {
                TredAnimations[i] = new ModelEntity(game, camera);
                TredAnimations[i].PO.AddAsChildOf(this, false, true);
            }
        }

        public override void Initialize()
        {
            LoadContent();
            base.Initialize();
        }

        public void LoadContent()
        {
            TredAnimations[0].LoadModel("TankTred1");
            TredAnimations[1].LoadModel("TankTred2");
        }

        public override void BeginRun()
        {
            base.BeginRun();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (AnimationTimer.Elapsed && Moving)
            {
                AnimationTimer.Reset();

                for(int i = 0; i < 2; i++)
                {
                    TredAnimations[i].Enabled = !TredAnimations[i].Enabled;
                }
            }
        }

        public void Spawn()
        {
            TredAnimations[0].Enabled = false;
            TredAnimations[1].Enabled = true;
        }

        public void Disable()
        {
            for (int i = 0; i < 2; i++)
            {
                TredAnimations[i].Enabled = false;
            }
        }
    }
}
