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
    public class EnemyControl : GameComponent, IBeginable, IUpdateableComponent
    {
        public ThePlayer PlayerRef;
        List<Tank> Tanks;

        public EnemyControl(Game game, ThePlayer player) : base(game)
        {
            PlayerRef = player;
            Tanks = new List<Tank>();

            game.Components.Add(this);
            BeginRun();
        }

        public override void Initialize()
        {
            base.Initialize();

        }

        public void BeginRun()
        {
            SpawnTanks(6);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            CheckOtherTanks();
            CheckTankHit();
        }

        void SpawnTanks(int amount)
        {
            for (int a = 0; a < amount; a++)
            {
                bool spawnNew = true;
                int spawnNumber = 0;

                for (int i = 0; i < Tanks.Count; i++)
                {
                    if (!Tanks[i].Active)
                    {
                        spawnNew = false;
                        spawnNumber = i;
                        break;
                    }
                }

                if (spawnNew)
                {
                    spawnNumber = Tanks.Count;
                    Tanks.Add(new Tank(Game, PlayerRef));
                }


                Tanks[spawnNumber].Spawn(new Vector3(Core.RandomMinMax(-3000, -8000), 10, 0));
            }
        }

        void CheckOtherTanks()
        {
            if (Tanks.Count < 2)
                return;

            foreach (Tank tank in Tanks)
            {
                tank.NotBumped();
            }

            foreach (Tank tanka in Tanks)
            {
                foreach (Tank tankb in Tanks)
                {
                    if (tanka != tankb)
                    {
                        if (tanka.Active && tankb.Active)
                        {
                            if (tanka.CirclesIntersect(tankb))
                            {
                                if (tanka.Position.X > tankb.Position.X)
                                {
                                    tanka.BumpedL();
                                    tankb.BumpedR();
                                }
                                else
                                {
                                    tanka.BumpedR();
                                    tankb.BumpedL();
                                }
                            }
                        }
                    }
                }
            }
        }

        void CheckTankHit()
        {
            foreach (Tank tank in Tanks)
            {
                if (tank.Active)
                {
                    foreach (Shot shot in PlayerRef.Shots)
                    {
                        if (shot != null)
                        {
                            if (shot.Active)
                            {
                                if (shot.CirclesIntersect(tank))
                                {
                                    shot.Active = false;
                                    tank.Active = false;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
