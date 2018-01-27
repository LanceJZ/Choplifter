using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System;

namespace Choplifter
{
    class Background : GameComponent
    {
        GameLogic LogicRef;
        Player PlayerRef;
        Camera TheCamera;
        StarControl Stars;
        ModelEntity Base;

        Plane[] Grass = new Plane[51];
        ModelEntity[] Blockades = new ModelEntity[4];

        Model BlockadeModel;
        Model PlayerBaseModel;
        Texture2D GrassTexture;

        float spaceBetweenGrass = 85;
        float GrassEdge = 600;
        float[] GrassX;
        float[] BlocksX;

        public Background(Game game, Camera camera, GameLogic gameLogic) : base(game)
        {
            LogicRef = gameLogic;
            PlayerRef = gameLogic.PlayerRef;
            TheCamera = camera;

            GrassX = new float[Grass.Length];
            BlocksX = new float[Blockades.Length];

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
            BlockadeModel = Helper.LoadModel("Blockade");
            GrassTexture = Helper.LoadTexture("Grass");

            BeginRun();
        }

        public void BeginRun()
        {
            for (int i = 0; i < Grass.Length; i++)
            {
                Grass[i] = new Plane(Game, TheCamera, GrassTexture);
            }

            for (int i = 0; i < Blockades.Length; i++)
            {
                Blockades[i] = new ModelEntity(Game, TheCamera, BlockadeModel);
            }

            Base = new ModelEntity(Game, TheCamera, PlayerBaseModel, new Vector3(100, -150, -10));

            float spaceBetweenBlocks = -8;
            float startGrassX = 10 + -spaceBetweenGrass * (int)(Grass.Length / 6);
            float startGrassY = -267;
            float startBlockX = -600;
            float startBlockY = -250;

            for (int i = 0; i < (int)(Grass.Length / 3); i++)
            {
                Grass[i].Position.X = startGrassX + (i * spaceBetweenGrass);
                Grass[i].Position.Y = startGrassY;

                int leveltwo = i + (int)(Grass.Length / 3);
                int levelthree = i + (int)(Grass.Length / 3) * 2;

                Grass[leveltwo].Position.X = startGrassX + (i * spaceBetweenGrass);
                Grass[leveltwo].Position.Y = startGrassY - spaceBetweenGrass * 0.75f;
                Grass[levelthree].Position.X = startGrassX + (i * spaceBetweenGrass);
                Grass[levelthree].Position.Y = startGrassY - (spaceBetweenGrass * 1.7f);
            }

            for (int i = 0; i < Grass.Length; i++)
            {
                GrassX[i] = Grass[i].Position.X;
                Grass[i].Position.Z = -200;
            }

            for (int i = 0; i < Blockades.Length; i++)
            {
                Blockades[i].Position = new Vector3(startBlockX, startBlockY +
                    (i * (spaceBetweenBlocks + (i * spaceBetweenBlocks * 0.95f))), -150);
                BlocksX[i] = Blockades[i].Position.X;
            }
        }

        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < Grass.Length / 3; i++)
            {
                int leveltwo = i + (int)(Grass.Length / 3);
                int levelthree = i + (int)(Grass.Length / 3) * 2;

                Grass[leveltwo].Velocity.X = -PlayerRef.Velocity.X * 0.25f;
                Grass[levelthree].Velocity.X = -PlayerRef.Velocity.X * 0.5f;
            }

            for (int i = 0; i < Grass.Length; i++)
            {
                if (PlayerRef.Velocity.X < 0)
                {
                    if (Grass[i].Position.X - spaceBetweenGrass > TheCamera.Position.X + GrassEdge)
                    {
                        Grass[i].Position.X -= 1200 + spaceBetweenGrass * 2;
                    }
                }

                if (PlayerRef.Velocity.X > 0)
                {
                    if (Grass[i].Position.X + spaceBetweenGrass < TheCamera.Position.X - GrassEdge)
                    {
                        Grass[i].Position.X += 1200 + spaceBetweenGrass * 2;
                    }
                }
            }

            for (int i = 0; i < Blockades.Length; i++)
            {
                Blockades[i].PO.Position.X = BlocksX[i] - ((
                    TheCamera.Position.X - BlocksX[i]) * (0.2f * i));
            }

            base.Update(gameTime);
        }
    }
}
