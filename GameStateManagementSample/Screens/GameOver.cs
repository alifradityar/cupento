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
    class GameOver : GameScreen
    {
        ContentManager content;

        Texture2D background;
        SpriteFont titleFont, timeFont;
        List<ButtonSmall> buttons = new List<ButtonSmall>();
        int time;

        public GameOver(int time_to_complete)
        {
            time = time_to_complete;
        }
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

                background = content.Load<Texture2D>("background");
                titleFont = content.Load<SpriteFont>("titleFont");
                timeFont = content.Load<SpriteFont>("timeFont");
                ButtonSmall menuButton = new ButtonSmall(new Vector2(0, 350), "Main Menu", ButtonSmall.ALLIGNVERTICALCENTERED, content);
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
            string text = "Completed in";
            Vector2 textSize = titleFont.MeasureString(text);
            Vector2 textPosition = new Vector2(400, 100) - textSize/2;
            spriteBatch.DrawString(titleFont, text, textPosition, Color.White);
            int time_in_sec = (int)time;
            int time_in_min = time_in_sec / 60;
            time_in_sec %= 60;
            string strtime;
            if (time_in_min < 10)
                strtime = "0" + time_in_min.ToString();
            else
                strtime = time_in_min.ToString();
            strtime = strtime + ":";
            if (time_in_sec < 10)
                strtime = strtime + '0' + time_in_sec.ToString();
            else
                strtime = strtime + time_in_sec.ToString();
            text = strtime;
            textSize = titleFont.MeasureString(text);
            textPosition = new Vector2(400, 140) - textSize / 2;
            spriteBatch.DrawString(timeFont, strtime, textPosition, Color.White);
            foreach (ButtonSmall button in buttons)
            {
                button.Draw(this);
            }
            spriteBatch.End();
        }


        void menuButton_Clicked(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, PlayerIndex.One, new MainMenu());
        }
    }
}
