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
    public class TankTurret : AModel
    {
        public ThePlayer PlayerRef;
        AModel Barral;
        Shot TankShot;
        Timer ShotTimer;

        public TankTurret(Game game, ThePlayer player) : base(game)
        {
            PlayerRef = player;

            Barral = new AModel(game);
            ShotTimer = new Timer(game, 4.1f);
            LoadContent();
        }

        public override void Initialize()
        {
            base.Initialize();

            Barral.AddAsChildOf(this, true, false);
            Barral.Position.X = 8;
        }

        public void LoadContent()
        {
            LoadModel("Models/CLTankTurret");
            Barral.LoadModel("Models/CLTankBarral");
            TankShot = new Shot(Game, Load("Core/Cube"));
            BeginRun();
        }

        public override void BeginRun()
        {
            base.BeginRun();

            TankShot.Active = false;
            TankShot.Acceleration.Y = -50;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Vector3 target = new Vector3(PlayerRef.Position.X, 100, 0);
            Rotation.Y = AngleToTurret(WorldPosition, target);

            Barral.Rotation.Z = MathHelper.Clamp(AngleToBarral(WorldPosition, target),
                0, MathHelper.PiOver4);

            if (ShotTimer.Elapsed && Active)
            {
                ShotTimer.Reset();
                FireShot();
            }
        }

        void FireShot() //TODO: Fires shot into ground.
        {
            Vector2 pos = new Vector2(Barral.WorldPosition.X, Barral.WorldPosition.Y);
            Vector2 target = new Vector2(PlayerRef.Position.X, 100);
            Vector2 pvel = new Vector2(ParentPO.Velocity.X, ParentPO.Velocity.Y);

            TankShot.Spawn(WorldPosition, VelocityFromVectorsZ(Barral.Position, PlayerRef.Position, 100)
                + ParentPO.Velocity, 3.5f);
        }

        float AngleToBarral(Vector3 pos, Vector3 target)
        {
            Vector3 diference = Vector3.Zero;

            if (target.X > pos.X)
                diference = target - pos;
            else
                diference = pos - target;

            return Angle(new Vector3(1, 0, 0), diference);
        }

        float AngleToTurret(Vector3 pos, Vector3 target)
        {
            Vector3 diference = target - pos;

            float sign = (target.Y < pos.Y) ? -1.0f : 1.0f;
            return Angle(new Vector3(1, 0, 0), diference) * sign;
        }

        float Angle(Vector3 direction, Vector3 differnce)
        {
            float angle = AngleFromVectorsZ(direction, differnce);

            if (angle < 0)
                angle *= -1;

            return MathHelper.Clamp(angle, 0, MathHelper.Pi);
        }
    }
}
