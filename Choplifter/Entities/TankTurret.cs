using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System;

namespace Choplifter
{
    class TankTurret : ModelEntity
    {
        Player PlayerRef;
        ModelEntity Barral;
        Shot TankShot;
        Timer ShotTimer;

        public TankTurret(Game game, Camera camera, GameLogic gameLogic) : base(game, camera)
        {
            PlayerRef = gameLogic.PlayerRef;
            Barral = new ModelEntity(game, camera);
            ShotTimer = new Timer(game, 4.1f);
            TankShot = new Shot(game, camera);
        }

        public override void Initialize()
        {

            base.Initialize();
        }

        protected override void LoadContent()
        {
            LoadModel("TankTurret");
            Barral.LoadModel("TankBarral");
        }

        public override void BeginRun()
        {
            Barral.AddAsChildOf(this);
            Barral.PO.Position.X = 8;
            TankShot.PO.Acceleration.Y = -50;

            base.BeginRun();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Vector3 target = new Vector3(PlayerRef.Position.X, 100, 0);
            PO.Rotation.Y = AngleToTurret(PO.WorldPosition, target);

            Barral.PO.Rotation.Z = MathHelper.Clamp(AngleToBarral(PO.WorldPosition, target),
                0, MathHelper.PiOver4);

            if (ShotTimer.Elapsed)
            {
                ShotTimer.Reset();
                FireShot();
            }
        }

        void FireShot() //TODO: Fires shot into ground.
        {
            Vector2 pos = new Vector2(Barral.PO.WorldPosition.X, Barral.PO.WorldPosition.Y);
            Vector2 target = new Vector2(PlayerRef.PO.Position.X, 100);
            Vector2 pvel = new Vector2(PO.ParentPO.Velocity.X, PO.ParentPO.Velocity.Y);

            TankShot.Spawn(PO.WorldPosition, PO.VelocityFromVectorsZ(
                Barral.Position, PlayerRef.Position, 100) + PO.ParentPO.Velocity, 3.5f);
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
            float angle = PO.AngleFromVectorsZ(direction, differnce);

            if (angle < 0)
                angle *= -1;

            return MathHelper.Clamp(angle, 0, MathHelper.Pi);
        }
    }
}
