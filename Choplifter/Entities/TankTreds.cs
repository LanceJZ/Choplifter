using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System;

namespace Choplifter
{
    class TankTreds : PositionedObject
    {
        ModelEntity[] TredAnimations = new ModelEntity[2];
        Camera CameraRef;
        Timer AnimationTimer;

        public bool Moving;

        public TankTreds(Game game, Camera camera) : base(game)
        {
            CameraRef = camera;
            AnimationTimer = new Timer(game, 0.1f);

            for (int i = 0; i < 2; i++)
            {
                TredAnimations[i] = new ModelEntity(game, camera);
                TredAnimations[i].PO.AddAsChildOf(this);
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            LoadContent();
        }

        public void LoadContent()
        {
            TredAnimations[0].SetModel(Game.Content.Load<Model>("Models/TankTred1"));
            TredAnimations[1].SetModel(Game.Content.Load<Model>("Models/TankTred2"));
        }

        public override void BeginRun()
        {
            base.BeginRun();

            TredAnimations[1].Enabled = false;
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
    }
}
