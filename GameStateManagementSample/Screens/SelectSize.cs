using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameStateManagement;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;

namespace GameStateManagementSample
{
    class SelectSize : GameScreen
    {
        public const int SIZE6X10 = 0;
        public const int SIZE5X12 = 1;
        public const int SIZE4X15 = 2;
        public const int SIZE3X20 = 3;

        ContentManager content;

        Texture2D background;
        List<ButtonBig> buttons = new List<ButtonBig>();
        int mode;

        public SelectSize(int mode)
        {
            this.mode = mode;
        }
        
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

                background = content.Load<Texture2D>("background");
                ButtonBig size6x10 = new ButtonBig(Vector2.Zero, "6 x 10", ButtonBig.ALLIGNTYPE1, content);
                size6x10.Clicked += size6x10_Clicked;
                buttons.Add(size6x10);
                ButtonBig size5x12 = new ButtonBig(Vector2.Zero, "5 x 12", ButtonBig.ALLIGNTYPE2, content);
                size5x12.Clicked += size5x12_Clicked;
                buttons.Add(size5x12);
                ButtonBig size4x15 = new ButtonBig(Vector2.Zero, "4 x 15", ButtonBig.ALLIGNTYPE3, content);
                size4x15.Clicked += size4x15_Clicked;
                buttons.Add(size4x15);
                ButtonBig size3x20 = new ButtonBig(Vector2.Zero, "3 x 20", ButtonBig.ALLIGNTYPE4, content);
                size3x20.Clicked += size3x20_Clicked;
                buttons.Add(size3x20);

            }
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {

        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            foreach (GamePadState gamePadState in input.CurrentGamePadStates){
                if (gamePadState.Buttons.Back == ButtonState.Pressed)
                {
                    LoadingScreen.Load(ScreenManager, false, PlayerIndex.One, new MainMenu());
                }
            }
            TouchCollection touchCollection = input.TouchState;
            if (touchCollection.Count > 0)
            {
                foreach (TouchLocation touchLocation in touchCollection)
                {
                    foreach (ButtonBig button in buttons)
                    {
                        button.CheckForPressed(touchLocation.Position);
                    }
                    if (touchLocation.State == TouchLocationState.Released)
                    {
                        foreach (ButtonBig button in buttons)
                        {
                            button.CheckForClicked(touchLocation.Position);
                        }
                    }
                }
            }
            else
            {
                foreach (ButtonBig button in buttons)
                {
                    button.isPressed = false;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            spriteBatch.Draw(background, new Vector2(0, 0), Color.White);
            foreach (ButtonBig button in buttons)
            {
                button.Draw(this);
            }
            spriteBatch.End();
        }

        void size6x10_Clicked(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, PlayerIndex.One, new SelectDifficulty(mode,SIZE6X10));
        }

        void size5x12_Clicked(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, PlayerIndex.One, new SelectDifficulty(mode,SIZE5X12));
        }

        void size4x15_Clicked(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, PlayerIndex.One, new SelectDifficulty(mode,SIZE4X15));
        }

        void size3x20_Clicked(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, PlayerIndex.One, new SelectDifficulty(mode,SIZE3X20));
        }

    }
}
