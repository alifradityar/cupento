using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameStateManagement;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace GameStateManagementSample
{
    class SelectDifficulty : GameScreen
    {
        public const int DIFFICULTYEASY = 0;
        public const int DIFFICULTYNORMAL = 1;
        public const int DIFFICULTYHARD = 2;
        public const int DIFFICULTYIMPOSSIBLE = 3;

        ContentManager content;

        Texture2D background, logo;
        List<ButtonSmall> buttons = new List<ButtonSmall>();
        int mode, size;

        public SelectDifficulty(int mode,int size)
        {
            this.mode = mode;
            this.size = size;
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

                background = content.Load<Texture2D>("background");
                logo = content.Load<Texture2D>("logo");

                ButtonSmall easyButton = new ButtonSmall(new Vector2(0, 200), "Easy", ButtonSmall.ALLIGNVERTICALCENTERED, content);
                easyButton.Clicked += easyButton_Clicked;
                buttons.Add(easyButton);

                ButtonSmall normalButton = new ButtonSmall(new Vector2(0, 260), "Normal", ButtonSmall.ALLIGNVERTICALCENTERED, content);
                normalButton.Clicked += normalButton_Clicked;
                buttons.Add(normalButton);

                ButtonSmall hardButton = new ButtonSmall(new Vector2(0, 320), "Hard", ButtonSmall.ALLIGNVERTICALCENTERED, content);
                hardButton.Clicked += hardButton_Clicked;
                buttons.Add(hardButton);

                ButtonSmall impossibleButton = new ButtonSmall(new Vector2(0, 380), "Impossible", ButtonSmall.ALLIGNVERTICALCENTERED, content);
                impossibleButton.Clicked += impossibleButton_Clicked;
                buttons.Add(impossibleButton);
            }
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {

        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            foreach (GamePadState gamePadState in input.CurrentGamePadStates)
            {
                if (gamePadState.Buttons.Back == ButtonState.Pressed)
                {
                    LoadingScreen.Load(ScreenManager, false, PlayerIndex.One, new SelectSize(mode));
                }
            }
            TouchCollection touchCollection = input.TouchState;
            if (touchCollection.Count > 0)
            {
                foreach (TouchLocation touchLocation in touchCollection)
                {
                    foreach (ButtonSmall button in buttons)
                    {
                        button.CheckForPressed(touchLocation.Position);
                    }
                    if (touchLocation.State == TouchLocationState.Released)
                    {
                        foreach (ButtonSmall button in buttons)
                        {
                            button.CheckForClicked(touchLocation.Position);
                        }
                    }
                }
            }
            else
            {
                foreach (ButtonSmall button in buttons)
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
            spriteBatch.Draw(logo, new Vector2(background.Width / 2 - logo.Width / 2, 50), Color.White);
            foreach (ButtonSmall button in buttons)
            {
                button.Draw(this);
            }
            spriteBatch.End();
        }


        void easyButton_Clicked(object sender, EventArgs e)
        {
            if (mode == MainMenu.MODEHUMAN)
                LoadingScreen.Load(ScreenManager, false, PlayerIndex.One, new HumanGameplay(size, DIFFICULTYEASY));
            else
                LoadingScreen.Load(ScreenManager, false, PlayerIndex.One, new SelectStrategy(size, DIFFICULTYEASY));
        }

        void normalButton_Clicked(object sender, EventArgs e)
        {
            if (mode == MainMenu.MODEHUMAN)
                LoadingScreen.Load(ScreenManager, false, PlayerIndex.One, new HumanGameplay(size, DIFFICULTYNORMAL));
            else
                LoadingScreen.Load(ScreenManager, false, PlayerIndex.One, new SelectStrategy(size, DIFFICULTYEASY));
        }

        void hardButton_Clicked(object sender, EventArgs e)
        {
            if (mode == MainMenu.MODEHUMAN)
                LoadingScreen.Load(ScreenManager, false, PlayerIndex.One, new HumanGameplay(size, DIFFICULTYHARD));
            else
                LoadingScreen.Load(ScreenManager, false, PlayerIndex.One, new SelectStrategy(size, DIFFICULTYEASY));
        }

        void impossibleButton_Clicked(object sender, EventArgs e)
        {
            if (mode == MainMenu.MODEHUMAN)
                LoadingScreen.Load(ScreenManager, false, PlayerIndex.One, new HumanGameplay(size, DIFFICULTYIMPOSSIBLE));
            else
                LoadingScreen.Load(ScreenManager, false, PlayerIndex.One, new SelectStrategy(size, DIFFICULTYEASY));
        }
    }
}
