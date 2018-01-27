using Microsoft.Xna.Framework;
using XnaModel = Microsoft.Xna.Framework.Graphics.Model;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System;
using Engine;

namespace MGChoplifter.Entities
{
    public class TankTred : PositionedObject, ILoadContent
    {
        AModel[] TredAnimations = new AModel[2];
        Timer AnimationTimer;

        public bool Moving;

        public TankTred(Game game) : base(game)
        {
            for (int i = 0; i < 2; i++)
            {
                TredAnimations[i] = new AModel(game);
                TredAnimations[i].AddAsChildOf(this, true, false);
            }

            AnimationTimer = new Timer(game, 0.1f);

            LoadContent();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public void LoadContent()
        {
            TredAnimations[0].SetModel(Game.Content.Load<XnaModel>("Models/CLTankTred1"));
            TredAnimations[1].SetModel(Game.Content.Load<XnaModel>("Models/CLTankTred2"));
            BeginRun();
        }

        public override void BeginRun()
        {
            base.BeginRun();

            TredAnimations[1].Visable = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (AnimationTimer.Elapsed && Moving)
            {
                AnimationTimer.Reset();

                for(int i = 0; i < 2; i++)
                {
                    TredAnimations[i].Visable = !TredAnimations[i].Visable;
                }
            }
        }
    }
}
