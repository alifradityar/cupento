using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameStateManagementSample
{
    class Help : GameScreen
    {
        ContentManager content;

        Texture2D background, logo;
        SpriteFont font;
        List<ButtonSmall> buttons = new List<ButtonSmall>();
        string help_text;
        string developer_text;

        public Help()
        {
            help_text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. In ultricies risus quis lobortis molestie. Suspendisse sed tempor sapien, eget tempus libero. Sed quis bibendum leo, viverra lacinia diam. Nunc aliquet felis mauris. Phasellus cursus lectus sem, nec posuere sapien consequat quis. Quisque a eros eget ante tempor pulvinar sit amet vitae augue. Suspendisse at tincidunt nisi. Pellentesque varius quam eros, eget vulputate justo consequat vel. Sed nec dolor eu leo consequat aliquam a nec tellus. Aliquam vel arcu augue.";
            developer_text = "Developed by : Alif Raditya Rochman, Dikra Prasetya, Muhammad Ikhsan";
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");
                font = content.Load<SpriteFont>("descriptionfont");
                background = content.Load<Texture2D>("background");
                logo = content.Load<Texture2D>("logo");
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
                    LoadingScreen.Load(ScreenManager, false, PlayerIndex.One, new MainMenu());
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
            spriteBatch.DrawString(font, WrapText(font,help_text,600), new Vector2(100, 200), Color.White);
            spriteBatch.DrawString(font, developer_text, new Vector2(60, 450), Color.White);
            foreach (ButtonSmall button in buttons)
            {
                button.Draw(this);
            }
            spriteBatch.End();
        }


        public string WrapText(SpriteFont spriteFont, string text, float maxLineWidth)
        {
            string[] words = text.Split(' ');

            StringBuilder sb = new StringBuilder();

            float lineWidth = 0f;

            float spaceWidth = spriteFont.MeasureString(" ").X;

            foreach (string word in words)
            {
                Vector2 size = spriteFont.MeasureString(word);

                if (lineWidth + size.X < maxLineWidth)
                {
                    sb.Append(word + " ");
                    lineWidth += size.X + spaceWidth;
                }
                else
                {
                    sb.Append("\n" + word + " ");
                    lineWidth = size.X + spaceWidth;
                }
            }

            return sb.ToString();
        }
    }
}
