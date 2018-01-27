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
    public class Person : AModel
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
        Timer Attention;
        ThePlayer PlayerRef;
        public AModel[] Arms = new AModel[2];
        public AModel[] Legs = new AModel[2];
        float Seperation;
        float MaxSpeed;
        float RightBound;

        public Person(Game game, ThePlayer player, XnaModel model) : base(game, model)
        {
            PlayerRef = player;
            Attention = new Timer(game, Core.RandomMinMax(0.25f, 2));
        }

        public override void Initialize()
        {
            Radius = 10;

            Position.Z = Core.RandomMinMax(-49, -1);
            Seperation = Core.RandomNumber.Next(20, 200);
            MaxSpeed = Core.RandomNumber.Next(10, 25);
            RightBound = Core.RandomMinMax(-10000, -9000);

            for (int i = 0; i < 2; i++)
            {
                Arms[i] = new AModel(Game);
                Legs[i] = new AModel(Game);
            }

            for (int i = 0; i < 2; i++)
            {
                Arms[i].AddAsChildOf(this);
                Legs[i].AddAsChildOf(this);

                Arms[i].Position.Y = 7;
                Legs[i].Position.Y = 0;
            }

            Arms[0].Position.X = -2;
            Arms[1].Position.X = 2;

            Legs[0].Position.X = -1;
            Legs[1].Position.X = 1;

            Active = false;

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (Active)
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
            }

            base.Update(gameTime);
        }

        public void Spawn(Vector3 position, bool dropped)
        {
            base.Spawn(position);
            Position.Y = position.Y;

            if (dropped)
            {
                Mode = CurrentMode.DroppedOff;
                Position.X = position.X;
                SwitchToRunning();
            }
            else
            {
                Mode = CurrentMode.Waiting;
                Position.X = position.X + Core.RandomMinMax(-20, 20);
                SwitchToWaving();
            }
        }

        void Entering()
        {
            if (PlayerRef.Position.Y > PlayerRef.BoundLowY + 10)
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
                Velocity.X = -MaxSpeed;
            else
                Velocity.X = MaxSpeed;

            if (CirclesIntersect(PlayerRef))
            {
                if (PlayerRef.PickUpPerson())
                {
                    Active = false;
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

            Velocity.X = MaxSpeed;

            if (Position.X > 144.5f)
                Active = false;
        }

        void ChaseOrWave()
        {
            Velocity.X = 0;

            if (PlayerRef.Position.X > RightBound)
            {
                if (PlayerRef.Position.Y < PlayerRef.BoundLowY + 10)
                {
                    Mode = CurrentMode.Enter;
                    return;
                }

                float differnceX = PlayerRef.Position.X - Position.X;

                if (differnceX > Seperation)
                {
                    Velocity.X = MathHelper.Clamp(differnceX, -MaxSpeed, MaxSpeed);

                    if (State == CurrentState.Waving)
                    {
                        SwitchToRunning();
                    }
                }
                else if (differnceX < -Seperation)
                {
                    Velocity.X = MathHelper.Clamp(differnceX, -MaxSpeed, MaxSpeed);

                    if (State == CurrentState.Waving)
                    {
                        SwitchToRunning();
                    }
                }
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
            Arms[0].Rotation.X = -MathHelper.PiOver2;
            Arms[1].Rotation.X = MathHelper.PiOver2;
            Legs[0].RotationVelocity.X = MathHelper.Pi;
            Legs[1].RotationVelocity.X = -MathHelper.Pi;
            Rotation.Y = MathHelper.PiOver2;
            State = CurrentState.Running;
        }

        void SwitchToWaving()
        {
            Arms[0].Rotation.X = MathHelper.Pi - MathHelper.PiOver4;
            Arms[1].Rotation.X = MathHelper.Pi + MathHelper.PiOver4;
            Arms[0].RotationVelocity.X = MathHelper.Pi;
            Arms[1].RotationVelocity.X = -MathHelper.Pi;
            Rotation.Y = 0;
            State = CurrentState.Waving;
        }

        void Running()
        {
            foreach(AModel arm in Arms)
            {
                if (arm.Rotation.X < -MathHelper.PiOver4)
                {
                    arm.RotationVelocity.X = MathHelper.Pi;
                }

                if (arm.Rotation.X > MathHelper.PiOver4)
                {
                    arm.RotationVelocity.X = -MathHelper.Pi;
                }
            }

            foreach (AModel leg in Legs)
            {
                if (leg.Rotation.X < -MathHelper.PiOver4)
                {
                    leg.RotationVelocity.X = MathHelper.Pi;
                }

                if (leg.Rotation.X > MathHelper.PiOver4)
                {
                    leg.RotationVelocity.X = -MathHelper.Pi;
                }
            }
        }

        void Waving()
        {
            foreach (AModel leg in Legs)
            {
                leg.Rotation.X = 0;
                leg.RotationVelocity.X = 0;
            }

            foreach (AModel arm in Arms)
            {
                if (arm.Rotation.X < MathHelper.Pi - MathHelper.PiOver4)
                {
                    arm.RotationVelocity.X = MathHelper.Pi;
                }

                if (arm.Rotation.X > MathHelper.Pi + MathHelper.PiOver4)
                {
                    arm.RotationVelocity.X = -MathHelper.Pi;
                }
            }
        }
    }
}
