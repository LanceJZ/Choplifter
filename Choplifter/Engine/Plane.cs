using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using XnaModel = Microsoft.Xna.Framework.Graphics.Model;
using System.Collections.Generic;
using System;

namespace Choplifter
{
    class Plane : DrawableGameComponent
    {
        #region Fields
        Camera TheCamera;
        protected PositionedObject ThePO;
        VertexPositionTexture[] Verts = new VertexPositionTexture[6];
        VertexBuffer PlaneVertexBuffer;
        Texture2D XNATexture;
        Matrix BaseWorld;
        BasicEffect PlaneBasicEffect;
        string TextureFileName;
        float TheWidth;
        float TheHeight;
        #endregion
        #region Properties
        public virtual Vector3 Position
        {
            get => ThePO.Position;
            set => ThePO.Position = value;
        }

        public virtual Vector3 Velocity
        {
            get => ThePO.Velocity;
            set => ThePO.Velocity = value;
        }

        public virtual Vector3 Acceleration
        {
            get => ThePO.Acceleration;
            set => ThePO.Acceleration = value;
        }

        public virtual Vector3 Rotation
        {
            get => ThePO.Rotation;
            set => ThePO.Rotation = value;
        }

        public virtual Vector3 RotationVelocity
        {
            get => ThePO.RotationVelocity;
            set => ThePO.RotationVelocity = value;
        }

        public virtual Vector3 RotationAcceleration
        {
            get => ThePO.RotationAcceleration;
            set => ThePO.RotationAcceleration = value;
        }

        public float Width { get; }
        public float Height { get; }

        public bool Moveable
        {
            get => ThePO.Moveable;
            set => ThePO.Moveable = value;
        }

        public new bool Enabled
        {
            get => base.Enabled;
            set { base.Enabled = value; ThePO.Enabled = value; }
        }

        public PositionedObject PO { get => ThePO; }
        #endregion
        public Plane(Game game, Camera camera) : base(game)
        {
            ThePO = new PositionedObject(game);
            TheCamera = camera;
            PlaneBasicEffect = new BasicEffect(game.GraphicsDevice);

            game.Components.Add(this);
        }

        public Plane(Game game, Camera camera, Texture2D texture) : base(game)
        {
            ThePO = new PositionedObject(game);
            TheCamera = camera;
            PlaneBasicEffect = new BasicEffect(game.GraphicsDevice);
            XNATexture = texture;

            game.Components.Add(this);
        }

        public Plane(Game game, Camera camera, string textureFileName) : base(game)
        {
            ThePO = new PositionedObject(game);
            TheCamera = camera;
            PlaneBasicEffect = new BasicEffect(game.GraphicsDevice);
            TextureFileName = textureFileName;

            game.Components.Add(this);
        }

        public override void Initialize()
        {
            base.Initialize();
            LoadContent();
            BeginRun();
        }

        protected override void LoadContent()
        {
            if (XNATexture == null)
                XNATexture = Helper.LoadTexture(TextureFileName);

            base.LoadContent();
        }

        public virtual void BeginRun()
        {
            Create(XNATexture);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            BaseWorld = Matrix.CreateScale(PO.Scale)
                * Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X,
                Rotation.Z + -MathHelper.PiOver2) *
                Matrix.CreateTranslation(Position);

            if (PO.Child)
            {
                BaseWorld *= Matrix.CreateTranslation(PO.ParentPO.Position)
                 * Matrix.CreateFromYawPitchRoll(PO.ParentPO.Rotation.Y,
                 PO.ParentPO.Rotation.X, PO.ParentPO.Rotation.Z)
                 * Matrix.CreateTranslation(Position);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            // Set object and camera info
            PlaneBasicEffect.World = BaseWorld;
            PlaneBasicEffect.View = TheCamera.View;
            PlaneBasicEffect.Projection = TheCamera.Projection;
            Game.GraphicsDevice.SetVertexBuffer(PlaneVertexBuffer);
            // Begin effect and draw for each frame
            foreach (EffectPass pass in PlaneBasicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Game.GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(
                    PrimitiveType.TriangleList, Verts, 0, 2);
            }
        }

        public void Create(Texture2D texture)
        {
            PlaneVertexBuffer = new VertexBuffer(Game.GraphicsDevice,
                typeof(VertexPositionTexture),
                Verts.Length, BufferUsage.None);

            ChangePlaneTexture(texture);
        }

        public void ChangePlaneTexture(Texture2D texture)
        {
            XNATexture = texture;
            PlaneBasicEffect.Texture = texture;
            PlaneBasicEffect.TextureEnabled = true;
            //PlaneBasicEffect.EnableDefaultLighting();

            if (texture != null)
                ChangePlaneSize(texture.Width, texture.Height);
        }

        public void ChangePlaneSize(float Width, float Height)
        {
            SetupVerts(Width, Height);
        }

        public Texture2D Load(string textureName)
        {
            return Helper.LoadTexture(textureName);
        }

        void SetupVerts(float width, float height)
        {
            // Setup plane
            TheWidth = width;
            TheHeight = height;

            Verts[0] = new VertexPositionTexture(new Vector3(-width / 2, -height / 2, 0), new Vector2(0, 0));
            Verts[1] = new VertexPositionTexture(new Vector3(-width / 2, height / 2, 0), new Vector2(0, 1));
            Verts[2] = new VertexPositionTexture(new Vector3(width / 2, -height / 2, 0), new Vector2(1, 0));
            Verts[3] = new VertexPositionTexture(new Vector3(-width / 2, height / 2, 0), new Vector2(0, 1));
            Verts[4] = new VertexPositionTexture(new Vector3(width / 2, height / 2, 0), new Vector2(1, 1));
            Verts[5] = new VertexPositionTexture(new Vector3(width / 2, -height / 2, 0), new Vector2(1, 0));

            PlaneVertexBuffer.SetData(Verts);
        }
    }
}
