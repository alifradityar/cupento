using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameStateManagementSample
{
    class Pause : GameScreen
    {
        ContentManager content;

        Texture2D background;
        SpriteFont titleFont;
        List<ButtonSmall> buttons = new List<ButtonSmall>();

        public Pause()
        {

        }
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

                background = content.Load<Texture2D>("background");
                titleFont = content.Load<SpriteFont>("titleFont");
                ButtonSmall resumeButton = new ButtonSmall(new Vector2(0, 240), "Resume", ButtonSmall.ALLIGNVERTICALCENTERED, content);
                resumeButton.Clicked += resumeButton_Clicked;
                buttons.Add(resumeButton);
                ButtonSmall restartButton = new ButtonSmall(new Vector2(0, 300), "Restart", ButtonSmall.ALLIGNVERTICALCENTERED, content);
                restartButton.Clicked += restartButton_Clicked;
                buttons.Add(restartButton);
                ButtonSmall menuButton = new ButtonSmall(new Vector2(0, 360), "Main Menu", ButtonSmall.ALLIGNVERTICALCENTERED, content);
                menuButton.Clicked += menuButton_Clicked;
                buttons.Add(menuButton);
            }
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {

        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
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
            string text = "Game is paused";
            Vector2 textSize = titleFont.MeasureString(text);
            Vector2 textPosition = new Vector2(400, 100) - textSize/2;
            spriteBatch.DrawString(titleFont, text, textPosition, Color.White);
            foreach (ButtonSmall button in buttons)
            {
                button.Draw(this);
            }
            spriteBatch.End();
        }

        void resumeButton_Clicked(object sender, EventArgs e)
        {
            ExitScreen();
        }

        void restartButton_Clicked(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, PlayerIndex.One, new MainMenu());
        }

        void menuButton_Clicked(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, PlayerIndex.One, new MainMenu());
        }
    }
}
