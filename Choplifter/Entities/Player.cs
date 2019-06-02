using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System;

namespace Choplifter
{
    class Player : ModelEntity
    {
        enum Direction
        {
            Right,
            ForwardFromRight,
            Left,
            ForwardFromLeft
        };

        enum CurrentState
        {
            Unload,
            Load,
            Flight
        };

        #region Fields
        GameLogic LogicRef;
        ModelEntity Blade;
        ModelEntity Rotor;
        Shot[] TheShots = new Shot[5];
        Timer FireTimer;
        Timer TurnTimer;
        Timer UnloadTimer;

        KeyboardState KeyState;
        KeyboardState KeyStateOld;

        float AccelerationAmount = 220;
        float MaxSpeed = 650;
        float Tilt = MathHelper.PiOver4 / 12f;
        float UnloadRate = 1.5f;
        float FireRate = 0.1f;
        float TurnRate = 1.1f;
        float RotateRate = MathHelper.PiOver2;
        float MoveHorizontal;
        float BoundLowY = -140;
        float BoundHighY = 176;
        float BoundLeftX = -3600 * 2;
        float BoundRightX = 145.5f;
        float UnloadXMin = 60.25f;
        float UnloadXMax = 133.5f;
        int ShotLimit;
        int NumberOfPassengers;
        int PassengerLimit = 4;
        bool FacingChanged;
        bool Coasting;

        Direction Facing;
        CurrentState State;
        #endregion
        #region Properties
        public Shot[] Shots { get => TheShots; }
        public float BoundLow { get => BoundLowY; }
        #endregion
        #region Base Methods
        public Player(Game game, Camera camera, GameLogic gameLogic) : base(game, camera)
        {
            LogicRef = gameLogic;
            Blade = new ModelEntity(game, camera);
            Rotor = new ModelEntity(game, camera);
            FireTimer = new Timer(game, FireRate);
            TurnTimer = new Timer(game, TurnRate);
            UnloadTimer = new Timer(game, UnloadRate);
            TheCamera.NeedLookUpResync = true;
        }

        public override void Initialize()
        {
            PO.Radius = 13;
            Facing = Direction.Right;
            State = CurrentState.Flight;
            NumberOfPassengers = 0; //TODO: For testing. Should be zero.
            ShotLimit = Shots.Length;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            LoadModel("ChopperBody");
            Blade.LoadModel("ChopperBlade");
            Rotor.LoadModel("ChopperRotor");
            base.LoadContent();
        }

        public override void BeginRun()
        {
            Blade.AddAsChildOf(this);
            Rotor.AddAsChildOf(this);
            Blade.Position = new Vector3(0, 6.5f, 0);
            Blade.RotationVelocity = new Vector3(0, 20, 0);
            Rotor.Position = new Vector3(-13, 4, -1);
            Rotor.RotationVelocity = new Vector3(0, 0, 24);

            for (int i = 0; i < Shots.Length; i++)
            {
                Shots[i] = new Shot(Game, TheCamera);
            }

            base.BeginRun();
        }

        public override void Update(GameTime gameTime)
        {
            TiltChopper();
            CheckToUnload();

            if (FacingChanged)
                UpdateFacing();

            if (Coasting)
                WindResistance();

            CheckPosition();
            GetInput();

            base.Update(gameTime);

            TheCamera.Position.X = Position.X;
        }
        #endregion
        #region Public Methods
        public bool PickUpPerson()
        {
            if (NumberOfPassengers == PassengerLimit)
                return false;

            NumberOfPassengers++;
                return true;
        }
        #endregion
        #region Private Methods
        void CheckToUnload()
        {
            if (Position.Y <= BoundLowY + 0.1f && Velocity == Vector3.Zero &&
                Position.X > UnloadXMin && Position.X < UnloadXMax)
            {
                if (State == CurrentState.Flight)
                    UnloadTimer.Reset();

                State = CurrentState.Unload;
            }
            else
            {
                State = CurrentState.Flight;
            }

            switch (State)
            {
                case CurrentState.Unload:
                    Unload();
                    break;
            }
        }

        void Unload()
        {
            if (UnloadTimer.Elapsed)
            {
                UnloadTimer.Reset();

                if (NumberOfPassengers > 0)
                {
                    NumberOfPassengers--;

                    LogicRef.HousesRef.SpawnPeople(new Vector3(Position.X,
                        BoundLow - 5, -10), true);
                }
            }
        }

        void GetInput()
        {
            KeyState = Keyboard.GetState();
            //Check keys after this. --------------
            if (KeyState.IsKeyDown(Keys.RightControl) && !KeyStateOld.IsKeyDown(Keys.RightControl))
            {
                if (TurnTimer.Elapsed)
                {
                    TurnTimer.Reset();

                    ChangeFacing();
                }
            }

            if (KeyState.IsKeyDown(Keys.LeftControl) && !KeyStateOld.IsKeyDown(Keys.LeftControl))
            {
                if (FireTimer.Elapsed)
                {
                    FireTimer.Reset();

                    FireShot();
                }
            }

            Acceleration = Vector3.Zero;
            MoveHorizontal = 0;
            Coasting = true;

            if (KeyState.IsKeyDown(Keys.Left))
            {

                if (Position.X > BoundLeftX)
                {
                    MoveHorizontal = 1;
                    MoveX(new Vector3(-AccelerationAmount, 0, 0));
                    Coasting = false;
                }
                else
                {
                    StopMovementX();
                }
            }
            else if (KeyState.IsKeyDown(Keys.Right))
            {

                if (Position.X < BoundRightX)
                {
                    MoveHorizontal = 1;
                    MoveX(new Vector3(AccelerationAmount, 0, 0));
                    Coasting = false;
                }
                else
                {
                    StopMovementX();
                }
            }

            if (KeyState.IsKeyDown(Keys.Up))
            {

                if (Position.Y < BoundHighY)
                {
                    MoveY(new Vector3(0, AccelerationAmount * 0.2f, 0));

                    Coasting = false;
                }
                else
                {
                    StopMovementY();
                }
            }
            else if (KeyState.IsKeyDown(Keys.Down))
            {

                if (Position.Y > BoundLowY)
                {
                    MoveY(new Vector3(0, -AccelerationAmount * 0.4f, 0));

                    Coasting = false;
                }
                else
                {
                    StopMovementY();
                }
            }

            //Check keys before this. ----------------
            KeyStateOld = KeyState;
        }

        void FireShot()
        {
            for (int i = 0; i < 5; i++)
            {
                if (!Shots[i].Enabled)
                {
                    Vector3 pos = Position;
                    Vector3 vel = Vector3.Zero;

                    switch (Facing)
                    {
                        case Direction.Right:
                            vel = PO.VelocityFromAngleZ(MathHelper.Clamp(Rotation.Z, -MathHelper.PiOver4,
                                MathHelper.PiOver4), Velocity.X + 400);
                            pos.X += 20;
                            break;

                        case Direction.Left:
                            vel = PO.VelocityFromAngleZ(MathHelper.Clamp(-Rotation.Z, -MathHelper.PiOver4,
                                MathHelper.PiOver4), Velocity.X - 400);
                            pos.X -= 20;
                            break;

                        case Direction.ForwardFromRight:
                        case Direction.ForwardFromLeft:
                            vel.Y = -200;
                            pos.Y -= 20;
                            break;
                    }

                    Shots[i].Spawn(pos, vel, 1.5f);
                    break;
                }
            }
        }

        void WindResistance()
        {
            float Deceration = 1.15f;
            Acceleration = -Velocity * Deceration;
        }

        void StopMovementX()
        {
            PO.Velocity.X = 0;
            PO.Acceleration.X = 0;
        }

        void StopMovementY()
        {
            PO.Velocity.Y = 0;
            PO.Acceleration.Y = 0;
        }

        void CheckPosition()
        {
            if (Position.Y < BoundLowY + 0.05f)
            {
                StopMovementX();
                StopMovementY();
            }

            if (Position.X < BoundLeftX || Position.X > BoundRightX)
            {
                StopMovementX();
            }

            if (Position.Y > BoundHighY)
            {
                StopMovementY();
            }

            Position = new Vector3(MathHelper.Clamp(Position.X, BoundLeftX, BoundRightX),
                MathHelper.Clamp(Position.Y, BoundLowY, BoundHighY), 0);
        }

        void MoveX(Vector3 direction)
        {
            float maxX = MaxSpeed;

            if (Facing == Direction.ForwardFromLeft || Facing == Direction.ForwardFromRight)
            {
                maxX *= 0.5f;
            }

            PO.Velocity.X = MathHelper.Clamp(Velocity.X, -maxX, maxX);
            Acceleration = direction;
            float Deceration = 1.1f;
            PO.Acceleration.Y =  -Velocity.Y * Deceration;
        }

        void MoveY(Vector3 direction)
        {
            PO.Velocity.Y = MathHelper.Clamp(Velocity.Y, -MaxSpeed * 0.5f, MaxSpeed * 0.33f);
            Acceleration = direction;
            float Deceration = 1.1f;
            PO.Acceleration.X = -Velocity.X * Deceration;
        }

        void TiltChopper()
        {
            float comp = 0.001f;

            switch (Facing)
            {
                case Direction.Left:
                    ChangeXTilt();

                    //PO.Rotation.Z = MathHelper.TwoPi - Tilt;

                    PO.Rotation.Z = (MoveHorizontal * (MathHelper.TwoPi - Tilt))
                        + (Velocity.X * comp);

                    break;

                case Direction.Right:
                    ChangeXTilt();

                    PO.Rotation.Z = (MoveHorizontal * Tilt) - (Velocity.X * comp);

                    break;

                case Direction.ForwardFromRight:
                case Direction.ForwardFromLeft:

                    PO.Rotation.X = (MoveHorizontal * Tilt) - (Velocity.X * 0.5f * comp);

                    PO.RotationVelocity.Z = 0;

                    if (Rotation.Z < MathHelper.TwoPi && Rotation.Z > MathHelper.Pi)
                    {
                        PO.RotationVelocity.Z = RotateRate * 0.25f;
                    }
                    else if (Rotation.Z > 0.05f && Rotation.Z < MathHelper.PiOver4)
                    {
                        PO.RotationVelocity.Z = -RotateRate * 0.25f;
                    }

                    break;
            }
        }

        void ChangeXTilt()
        {
            PO.RotationVelocity.X = 0;

            if (Rotation.X > MathHelper.Pi && Rotation.X < MathHelper.TwoPi - MathHelper.PiOver4)
            {
                PO.RotationVelocity.X = RotateRate * 0.25f;
            }
            else if (Rotation.X > MathHelper.TwoPi)
            {
                PO.RotationVelocity.X = -RotateRate * 0.25f;
            }
        }

        void ChangeFacing()
        {
            FacingChanged = true;

            switch (Facing)
            {
                case Direction.Right:
                    Facing = Direction.ForwardFromRight;
                    PO.RotationVelocity.Y = -RotateRate;
                    break;

                case Direction.ForwardFromRight:
                    Facing = Direction.Left;
                    PO.RotationVelocity.Y = -RotateRate;
                    break;

                case Direction.Left:
                    Facing = Direction.ForwardFromLeft;
                    PO.RotationVelocity.Y = RotateRate;
                    break;

                case Direction.ForwardFromLeft:
                    Facing = Direction.Right;
                    PO.RotationVelocity.Y = RotateRate;
                    break;
            }
        }

        void UpdateFacing()
        {
            switch (Facing)
            {
                case Direction.ForwardFromRight:
                    if (Rotation.Y > MathHelper.TwoPi - MathHelper.PiOver2)
                    {
                        PO.RotationVelocity.Y = -RotateRate;
                    }
                    else
                    {
                        PO.RotationVelocity.Y = 0;
                        FacingChanged = false;
                    }
                    break;

                case Direction.ForwardFromLeft:
                    if (Rotation.Y < MathHelper.TwoPi - MathHelper.PiOver2)
                    {
                        PO.RotationVelocity.Y = RotateRate;
                    }
                    else
                    {
                        PO.RotationVelocity.Y = 0;
                        FacingChanged = false;
                    }
                    break;

                case Direction.Right:
                    if (Rotation.Y < MathHelper.TwoPi)
                    {
                        PO.RotationVelocity.Y = RotateRate;
                    }
                    else
                    {
                        PO.Rotation.Y = 0;
                        PO.RotationVelocity.Y = 0;
                        FacingChanged = false;
                    }
                    break;

                case Direction.Left:
                    if (Rotation.Y > MathHelper.Pi)
                    {
                        PO.RotationVelocity.Y = -RotateRate;
                    }
                    else
                    {
                        PO.RotationVelocity.Y = 0;
                        FacingChanged = false;
                    }
                    break;
            }
        }
        #endregion
    }
}
