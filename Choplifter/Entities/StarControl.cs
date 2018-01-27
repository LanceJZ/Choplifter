using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace Choplifter
{
    class StarControl : GameComponent
    {
        ModelEntity[] Stars = new ModelEntity[50];
        Model StarModel;
        Camera TheCamera;
        float[] StarsX;

        public StarControl(Game game, Camera camera) : base(game)
        {
            StarsX = new float[Stars.Length];
            TheCamera = camera;
            game.Components.Add(this);
            LoadContent();
        }

        public override void Initialize()
        {
            base.Initialize();

        }

        public void LoadContent()
        {
            StarModel = Helper.LoadModel("Core/Cube");
            BeginRun();
        }

        public void BeginRun()
        {
            float spinSpeed = 7.5f;

            for (int i = 0; i < Stars.Length; i++)
            {
                Stars[i] = new ModelEntity(Game, TheCamera, StarModel);
                Stars[i].Position = new Vector3(Helper.RandomMinMax(-600, 600),
                    Helper.RandomMinMax(-125, 225), -100);
                Stars[i].RotationVelocity = new Vector3(Helper.RandomMinMax(-spinSpeed, spinSpeed),
                    Helper.RandomMinMax(-spinSpeed, spinSpeed),
                    Helper.RandomMinMax(-spinSpeed, spinSpeed));
                Stars[i].Scale = Helper.RandomMinMax(0.5f, 1.5f);
                StarsX[i] = Stars[i].Position.X;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            for (int i = 0; i < Stars.Length; i++)
            {
                Stars[i].PO.Position.X = StarsX[i] + TheCamera.Position.X;
            }
        }
    }
}
