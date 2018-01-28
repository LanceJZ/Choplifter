using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System;

namespace Choplifter
{
    class Background : GameComponent
    {
        #region Fields
        GameLogic LogicRef;
        Player PlayerRef;
        Camera TheCamera;
        StarControl Stars;
        ModelEntity Base;

        Plane[] Grass = new Plane[51];
        ModelEntity[] Barricades = new ModelEntity[4];

        Model BarricadeModel;
        Model PlayerBaseModel;
        Texture2D GrassTexture;

        float spaceBetweenGrass = 85;
        float GrassEdge = 600;
        float[] GrassX;
        #endregion
        #region Properties
        public PositionedObject BasePosition { get=> Base.PO; }
        public float BarricadePositionX { get => Barricades[0].Position.X; }
        #endregion
        public Background(Game game, Camera camera, GameLogic gameLogic) : base(game)
        {
            LogicRef = gameLogic;
            PlayerRef = gameLogic.PlayerRef;
            TheCamera = camera;

            GrassX = new float[Grass.Length];

            Stars = new StarControl(game, camera);

            game.Components.Add(this);
        }

        public override void Initialize()
        {
            base.Initialize();

            LoadContent();
        }

        public void LoadContent()
        {
            PlayerBaseModel = Helper.LoadModel("Base");
            BarricadeModel = Helper.LoadModel("Barricade");
            GrassTexture = Helper.LoadTexture("Grass");

            BeginRun();
        }

        public void BeginRun()
        {
            for (int i = 0; i < Grass.Length; i++)
            {
                Grass[i] = new Plane(Game, TheCamera, GrassTexture);
            }

            for (int i = 0; i < Barricades.Length; i++)
            {
                Barricades[i] = new ModelEntity(Game, TheCamera, BarricadeModel);
            }

            Base = new ModelEntity(Game, TheCamera, PlayerBaseModel, new Vector3(100, -150, -10));

            float spaceBetweenBlocks = -50;
            float startGrassX = 10 + -spaceBetweenGrass * (Grass.Length / 6);
            float startGrassZ = 128;
            float startBlockX = -600;
            float startBlockY = -150;

            for (int i = 0; i < (Grass.Length / 3); i++)
            {
                Grass[i].PO.Position.X = startGrassX + (i * spaceBetweenGrass);
                Grass[i].PO.Position.Z = startGrassZ;

                int leveltwo = i + (Grass.Length / 3);
                int levelthree = i + (Grass.Length / 3) * 2;

                Grass[leveltwo].PO.Position.X = startGrassX + (i * spaceBetweenGrass);
                Grass[leveltwo].PO.Position.Z = startGrassZ - spaceBetweenGrass * 0.75f;
                Grass[levelthree].PO.Position.X = startGrassX + (i * spaceBetweenGrass);
                Grass[levelthree].PO.Position.Z = startGrassZ - (spaceBetweenGrass * 1.7f);
            }

            for (int i = 0; i < Grass.Length; i++)
            {
                GrassX[i] = Grass[i].Position.X;
                Grass[i].PO.Position.Y = Base.Position.Y;
                Grass[i].PO.Rotation.X = -MathHelper.PiOver2;
            }

            for (int i = 0; i < Barricades.Length; i++)
            {
                Barricades[i].Position = new Vector3(startBlockX, startBlockY,
                    (i * spaceBetweenBlocks) + 100);
            }
        }

        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < Grass.Length / 3; i++)
            {
                int leveltwo = i + (Grass.Length / 3);
                int levelthree = i + (Grass.Length / 3) * 2;
            }

            for (int i = 0; i < Grass.Length; i++)
            {
                if (PlayerRef.Velocity.X < 0)
                {
                    if (Grass[i].Position.X - spaceBetweenGrass > TheCamera.Position.X + GrassEdge)
                    {
                        Grass[i].PO.Position.X -= 1200 + spaceBetweenGrass * 2;
                    }
                }

                if (PlayerRef.Velocity.X > 0)
                {
                    if (Grass[i].Position.X + spaceBetweenGrass < TheCamera.Position.X - GrassEdge)
                    {
                        Grass[i].PO.Position.X += 1200 + spaceBetweenGrass * 2;
                    }
                }
            }

            //for (int i = 0; i < Blockades.Length; i++)
            //{
            //    Blockades[i].PO.Position.X = BlocksX[i] - ((
            //        TheCamera.Position.X - BlocksX[i]) * (0.2f * i));
            //}

            base.Update(gameTime);
        }
    }
}
