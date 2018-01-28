using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Choplifter
{
    class HouseControl : GameComponent
    {
        ModelEntity[] Houses = new ModelEntity[4];
        ModelEntity[] OpenHouses = new ModelEntity[4];

        List<Person> People = new List<Person>();

        Player PlayerRef;
        GameLogic LogicRef;
        Camera CameraRef;

        Model HouseModel;
        Model HouseOpenModel;

        float Height = -230;
        float StartX = -6000;
        float DistanceBetween = 600;

        public HouseControl(Game game, Camera camera, GameLogic gameLogic) : base(game)
        {
            PlayerRef = gameLogic.PlayerRef;
            CameraRef = camera;
            LogicRef = gameLogic;

            game.Components.Add(this);
        }

        public override void Initialize()
        {
            base.Initialize();
            LoadContent();
            BeginRun();
        }

        public void LoadContent()
        {
            HouseModel = Helper.LoadModel("CLHouse");
            HouseOpenModel = Helper.LoadModel("CLHouseOpen");
        }

        public void BeginRun()
        {
            for (int i = 0; i < Houses.Length; i++)
            {
                Houses[i] = new ModelEntity(Game, CameraRef, HouseModel);
                OpenHouses[i] = new ModelEntity(Game, CameraRef, HouseOpenModel);
                Houses[i].Position = new Vector3(StartX - (i * DistanceBetween), Height, -100);
                Houses[i].PO.Radius = 32;
                OpenHouses[i].Position = Houses[i].Position;
                OpenHouses[i].Enabled = false;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            for (int i = 0; i < Houses.Length; i++)
            {
                if (Houses[i].Enabled)
                {
                    foreach (Shot shot in PlayerRef.Shots)
                    {
                        if (shot.Enabled)
                        {
                            if (Houses[i].PO.CirclesIntersect(shot.PO))
                            {
                                Houses[i].Enabled = false;
                                OpenHouses[i].Enabled = true;
                                shot.Enabled = false;
                                SpawnPeople(Houses[i].Position);
                                break;
                            }
                        }
                    }
                }
            }
        }

        void SpawnPeople(Vector3 position)
        {
            for (int p = 0; p < 4; p++)
            {
                bool spawnNew = true;

                foreach (Person man in People)
                {
                    if (!man.Enabled)
                    {
                        spawnNew = false;
                        man.Spawn(position, false);
                        break;
                    }
                }

                if (spawnNew)
                {
                    People.Add(new Person(Game, CameraRef, LogicRef));
                    People.Last().Spawn(position, false);
                }
            }
        }
    }
}
