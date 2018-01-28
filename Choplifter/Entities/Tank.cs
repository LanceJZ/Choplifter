using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System;

namespace Choplifter
{
    class Tank : ModelEntity
    {
        Player PlayerRef;
        GameLogic GameLogicRef;
        Camera CameraRef;
        TankTurret Turret;
        TankTreds[] Treds = new TankTreds[2];
        float MaxSpeed;
        float Seperation;
        float RightBound;
        bool CollidedR;
        bool CollidedL;

        public Tank(Game game, Camera camera, GameLogic gameLogic) : base(game, camera)
        {
            GameLogicRef = gameLogic;
            PlayerRef = gameLogic.PlayerRef;
            Turret = new TankTurret(game, camera, gameLogic);
            Turret.AddAsChildOf(this);

            for (int i = 0; i < 2; i++)
            {
                Treds[i] = new TankTreds(game, camera);
                Treds[i].AddAsChildOf(PO);
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            MaxSpeed = Helper.RandomMinMax(50, 100);
            Seperation = Helper.RandomMinMax(100, 200);
            RightBound = Helper.RandomMinMax(-1000, -1100);

            PO.Radius = 24;
        }

        protected override void LoadContent()
        {
            LoadModel("Models/TankBody");
        }

        public override void BeginRun()
        {
            Treds[0].Position = new Vector3(0, -2, -16);
            Treds[1].Position = new Vector3(0, -2, 16);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            foreach (TankTreds tred in Treds)
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
            PO.Velocity.X = 0;

            float differnceX = PlayerRef.Position.X - Position.X;

            if (differnceX > Seperation && PlayerRef.Position.X < RightBound && !CollidedR)
            {
                PO.Velocity.X = MathHelper.Clamp(differnceX * 0.1f, -MaxSpeed, MaxSpeed);

                foreach (TankTreds tred in Treds)
                {
                    tred.Moving = true;
                }
            }

            if (differnceX < -Seperation && PlayerRef.Position.X >
                GameLogicRef.BackgroundRef.BarricadePositionX && !CollidedL)
            {
                PO.Velocity.X = MathHelper.Clamp(differnceX * 0.1f, -MaxSpeed, MaxSpeed);

                foreach (TankTreds tred in Treds)
                {
                    tred.Moving = true;
                }
            }
        }
    }
}
