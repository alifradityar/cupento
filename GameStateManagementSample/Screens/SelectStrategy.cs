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
    class SelectStrategy : GameScreen
    {
        public const int DIFFICULTYEASY = 0;
        public const int DIFFICULTYNORMAL = 1;
        public const int DIFFICULTYHARD = 2;
        public const int DIFFICULTYIMPOSSIBLE = 3;

        ContentManager content;

        Texture2D background, logo;
        List<ButtonSmall> buttons = new List<ButtonSmall>();
        int size, difficulty;

        public SelectStrategy(int size,int difficulty)
        {
            this.size = size;
            this.difficulty = difficulty;
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

                background = content.Load<Texture2D>("background");
                logo = content.Load<Texture2D>("logo");

                ButtonSmall dfsButton = new ButtonSmall(new Vector2(0, 240), "DFS", ButtonSmall.ALLIGNVERTICALCENTERED, content);
                dfsButton.Clicked += dfsButton_Clicked;
                buttons.Add(dfsButton);

                ButtonSmall bfsButton = new ButtonSmall(new Vector2(0, 300), "BFS", ButtonSmall.ALLIGNVERTICALCENTERED, content);
                //normalButton.Clicked += normalButton_Clicked;
                buttons.Add(bfsButton);
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
                    LoadingScreen.Load(ScreenManager, false, PlayerIndex.One, new SelectDifficulty(MainMenu.MODECOMPUTER,size));
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

        void dfsButton_Clicked(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, PlayerIndex.One, new DFSGameplay(size,difficulty));
        }

    }
}
