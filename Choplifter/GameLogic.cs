using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System.Linq;
using System;

public enum GameState
{
    Over,
    InPlay,
    HighSHelper,
    MainMenu
};

namespace Choplifter
{
    class GameLogic : GameComponent
    {
        Camera TheCamera;
        Background TheBackground;
        HouseControl TheHouses;
        Tank TheTank;
        Player ThePlayer;

        Timer FPSTimer;
        float FPSFrames = 0;

        GameState GameMode = GameState.MainMenu;
        KeyboardState OldKeyState;

        public GameState CurrentMode { get => GameMode; }
        public Player PlayerRef { get => ThePlayer; }
        public Background BackgroundRef { get => TheBackground; }
        public HouseControl HousesRef { get => TheHouses; }

        public GameLogic(Game game, Camera camera) : base(game)
        {
            TheCamera = camera;

            ThePlayer = new Player(game, camera, this);
            TheBackground = new Background(game, camera, this);
            TheHouses = new HouseControl(game, camera, this);

            TheTank = new Tank(game, camera, this);

            FPSTimer = new Timer(game, 1);
            // Screen resolution is 1200 X 900.
            // Y positive is Up.
            // X positive is right of window when camera is at rotation zero.
            // Z positive is towards the camera when at rotation zero.
            // Positive rotation rotates CCW. Zero has front facing X positive. Pi/2 on Y faces Z negative.
            game.Components.Add(this);
        }

        public override void Initialize()
        {
            GameMode = GameState.InPlay;

            base.Initialize();
            LoadContent();
            BeginRun();
        }

        public void LoadContent()
        {
        }

        public void BeginRun()
        {
            Vector3 pos = new Vector3(TheBackground.BarricadePositionX - 100,
                TheBackground.BasePosition.Position.Y, 0);
            //TheTank.Spawn(pos);

        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState KBS = Keyboard.GetState();

            if (KBS != OldKeyState)
            {
                if (KBS.IsKeyDown(Keys.Space))
                {
                }
            }

            OldKeyState = Keyboard.GetState();

            FPSFrames++;

            if (FPSTimer.Elapsed)
            {
                FPSTimer.Reset();
                System.Diagnostics.Debug.WriteLine("FPS " + FPSFrames.ToString());
                FPSFrames = 0;
            }

            base.Update(gameTime);
        }
    }
}
