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
    public class Tank : AModel
    {
        public ThePlayer PlayerRef;
        TankTurret Turret;
        TankTred[] Treds = new TankTred[2];
        float MaxSpeed;
        float Seperation;
        float RightBound;
        bool CollidedR;
        bool CollidedL;

        public Tank(Game game, ThePlayer player) : base(game)
        {
            PlayerRef = player;
            Turret = new TankTurret(game, player);
            Active = false;

            for (int i = 0; i < 2; i++)
            {
                Treds[i] = new TankTred(game);
                Treds[i].AddAsChildOf(this, true, false);
            }

            LoadContent();
        }

        public override void Initialize()
        {
            base.Initialize();

            MaxSpeed = Core.RandomMinMax(50, 100);
            Seperation = Core.RandomMinMax(100, 200);
            RightBound = Core.RandomMinMax(-1000, -1100);

            Turret.AddAsChildOf(this, true, false);
            Radius = 24;
        }

        public void LoadContent()
        {
            LoadModel("Models/CLTankBody");
            BeginRun();
        }

        public override void BeginRun()
        {
            Treds[0].Position = new Vector3(0, -2, -16);
            Treds[1].Position = new Vector3(0, -2, 16);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            foreach (TankTred tred in Treds)
            {
                tred.Moving = false;
            }

            FollowPlayer();

        }

        public override void Spawn(Vector3 position)
        {
            base.Spawn(position);
        }

        public void BumpedR()
        {
            CollidedR = true;
        }

        public void BumpedL()
        {
            CollidedL = true;
        }

        public void NotBumped()
        {
            CollidedL = false;
            CollidedR = false;
        }

        void FollowPlayer()
        {
            Velocity.X = 0;

            float differnceX = PlayerRef.Position.X - Position.X;

            if (differnceX > Seperation && PlayerRef.Position.X < RightBound && !CollidedR)
            {
                Velocity.X = MathHelper.Clamp(differnceX * 0.1f, -MaxSpeed, MaxSpeed);

                foreach (TankTred tred in Treds)
                {
                    tred.Moving = true;
                }
            }

            if (differnceX < -Seperation && PlayerRef.Position.X > PlayerRef.BoundLeftX && !CollidedL)
            {
                Velocity.X = MathHelper.Clamp(differnceX * 0.1f, -MaxSpeed, MaxSpeed);
                foreach (TankTred tred in Treds)
                {
                    tred.Moving = true;
                }
            }
        }
    }
}
