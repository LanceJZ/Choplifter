using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System;

namespace Choplifter
{
    class Person : ModelEntity
    {
        enum CurrentState
        {
            Running,
            Waving
        };

        enum CurrentMode
        {
            Waiting,
            DroppedOff,
            Enter
        };

        CurrentState State;
        CurrentMode Mode;
        Camera CameraRef;
        Timer Attention;
        Player PlayerRef;
        Background BackgroundRef;
        public ModelEntity[] Arms = new ModelEntity[2];
        public ModelEntity[] Legs = new ModelEntity[2];
        Model PersonManModel;
        Model PersonArmModel;
        Model PersonLegModel;
        float Seperation;
        float MaxSpeed;
        float LeftBound;

        public Person(Game game, Camera camera, GameLogic gameLogic) :
            base(game, camera)
        {
            CameraRef = camera;
            PlayerRef = gameLogic.PlayerRef;
            BackgroundRef = gameLogic.BackgroundRef;
            Attention = new Timer(game, Helper.RandomMinMax(0.25f, 2));
        }

        public override void Initialize()
        {
            PO.Radius = 10;

            PO.Position.Z = Helper.RandomMinMax(-49, -1);
            Seperation = Helper.RandomMinMax(20, 200);
            MaxSpeed = Helper.RandomMinMax(10, 25);
            LeftBound = Helper.RandomMinMax(-10000, -9000);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            PersonManModel = Helper.LoadModel("PersonMan");
            PersonArmModel = Helper.LoadModel("PersonArm");
            PersonLegModel = Helper.LoadModel("PersonLeg");
        }

        public override void BeginRun()
        {
            SetModel(PersonManModel);

            for (int i = 0; i < 2; i++)
            {
                Arms[i] = new ModelEntity(Game, CameraRef, PersonArmModel);
                Legs[i] = new ModelEntity(Game, CameraRef, PersonLegModel);
                Arms[i].AddAsChildOf(this);
                Legs[i].AddAsChildOf(this);
                Arms[i].PO.Position.Y = 3.5f;
                Legs[i].PO.Position.Y = 0;
            }

            Arms[0].PO.Position.X = -1.5f;
            Arms[1].PO.Position.X = 1.5f;

            Legs[0].PO.Position.X = -0.5f;
            Legs[1].PO.Position.X = 0.5f;

            Enabled = false;

            base.BeginRun();
        }

        public override void Update(GameTime gameTime)
        {
            switch (Mode)
            {
                case CurrentMode.Waiting:
                    if (Attention.Elapsed)
                    {
                        Attention.Reset();
                        ChaseOrWave();
                    }
                    break;

                case CurrentMode.DroppedOff:
                    RunToBase();
                    break;

                case CurrentMode.Enter:
                    Entering();
                    break;
            }

            switch (State)
            {
                case CurrentState.Running:
                    Running();
                    break;

                case CurrentState.Waving:
                    Waving();
                    break;
            }

            base.Update(gameTime);
        }

        public void Spawn(Vector3 position, bool dropped)
        {
            base.Spawn(position);
            PO.Position.Y = position.Y;

            if (dropped)
            {
                Mode = CurrentMode.DroppedOff;
                PO.Position.X = position.X;
                SwitchToRunning();
            }
            else
            {
                Mode = CurrentMode.Waiting;
                PO.Position.X = position.X + Helper.RandomMinMax(-20, 20);
                SwitchToWaving();
            }
        }

        void Entering()
        {
            if (PlayerRef.Position.Y > PlayerRef.BoundLow + 10)
            {
                Mode = CurrentMode.Waiting;
                State = CurrentState.Waving;
                return;
            }

            if(State == CurrentState.Waving)
            {
                SwitchToRunning();
            }

            if (Position.X > PlayerRef.Position.X)
                PO.Velocity.X = -MaxSpeed;
            else
                PO.Velocity.X = MaxSpeed;

            if (PO.CirclesIntersect(PlayerRef.PO))
            {
                if (PlayerRef.PickUpPerson())
                {
                    Enabled = false;
                }
                else
                {
                    Mode = CurrentMode.Waiting;
                    State = CurrentState.Waving;
                }
            }
        }

        void RunToBase()
        {
            if (State == CurrentState.Waving)
            {
                SwitchToRunning();
            }

            PO.Velocity.X = MaxSpeed;

            if (Position.X > 144.5f)
                Enabled = false;
        }

        void ChaseOrWave()
        {
            PO.Velocity.X = 0;

            if (PlayerRef.Position.X > LeftBound &&
                PlayerRef.Position.X < BackgroundRef.BarricadePositionX)
            {
                if (PlayerRef.Position.Y < PlayerRef.BoundLow + 10)
                {
                    Mode = CurrentMode.Enter;
                    return;
                }

                float differnceX = PlayerRef.Position.X - Position.X;

                if (differnceX > Seperation)
                {
                    PO.Velocity.X = MathHelper.Clamp(differnceX, -MaxSpeed, MaxSpeed);

                    if (State == CurrentState.Waving)
                    {
                        SwitchToRunning();
                        return;
                    }
                }
                else if (differnceX < -Seperation)
                {
                    PO.Velocity.X = MathHelper.Clamp(differnceX, -MaxSpeed, MaxSpeed);

                    if (State == CurrentState.Waving)
                    {
                        SwitchToRunning();
                        return;
                    }
                }

                SwitchToWaving();
            }
            else
            {
                if (State == CurrentState.Running)
                {
                    SwitchToWaving();
                }

            }
        }

        void SwitchToRunning()
        {
            Arms[0].PO.Rotation.X = MathHelper.TwoPi - MathHelper.PiOver2;
            Arms[1].PO.Rotation.X = MathHelper.PiOver2;
            Legs[0].PO.RotationVelocity.X = MathHelper.Pi;
            Legs[1].PO.RotationVelocity.X = -MathHelper.Pi;
            PO.Rotation.Y = MathHelper.PiOver2;
            State = CurrentState.Running;
        }

        void SwitchToWaving()
        {
            Arms[0].PO.Rotation.X = MathHelper.Pi - MathHelper.PiOver4;
            Arms[1].PO.Rotation.X = MathHelper.Pi + MathHelper.PiOver4;
            Arms[0].PO.RotationVelocity.X = MathHelper.Pi;
            Arms[1].PO.RotationVelocity.X = -MathHelper.Pi;
            PO.Rotation.Y = 0;
            State = CurrentState.Waving;
        }

        void Running()
        {
            foreach (ModelEntity arm in Arms)
            {
                arm.PO.RotationVelocity.X = SwingDown(arm.Rotation.X, arm.RotationVelocity.X);
            }

            foreach (ModelEntity leg in Legs)
            {
                leg.PO.RotationVelocity.X = SwingDown(leg.Rotation.X, leg.RotationVelocity.X);
            }
        }

        float SwingDown (float angle, float velocity)
        {
            if (angle < MathHelper.TwoPi - MathHelper.PiOver4 &&
                angle > MathHelper.Pi)
            {
                velocity = MathHelper.Pi;
            }

            if (angle > MathHelper.PiOver4 && angle < MathHelper.Pi)
            {
                velocity = -MathHelper.Pi;
            }

            return velocity;
        }

        void Waving()
        {
            foreach (ModelEntity leg in Legs)
            {
                leg.PO.Rotation.X = 0;
                leg.PO.RotationVelocity.X = 0;
            }

            foreach (ModelEntity arm in Arms)
            {
                if (arm.Rotation.X < MathHelper.Pi - MathHelper.PiOver4)
                {
                    arm.PO.RotationVelocity.X = MathHelper.Pi;
                }

                if (arm.Rotation.X > MathHelper.Pi + MathHelper.PiOver4)
                {
                    arm.PO.RotationVelocity.X = -MathHelper.Pi;
                }
            }
        }
    }
}
