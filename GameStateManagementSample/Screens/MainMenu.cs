using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameStateManagement;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;

namespace GameStateManagementSample
{
    class MainMenu : GameScreen
    {
        public const int MODEHUMAN = 0;
        public const int MODECOMPUTER = 1;

        ContentManager content;

        Texture2D background, logo;
        List<ButtonSmall> buttons = new List<ButtonSmall>();


        public MainMenu()
        {
            
        }
        
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

                background = content.Load<Texture2D>("background");
                logo = content.Load<Texture2D>("logo");

                ButtonSmall humanModeButton = new ButtonSmall(new Vector2(0, 200), "Human Mode", ButtonSmall.ALLIGNVERTICALCENTERED, content);
                humanModeButton.Clicked += humanModeButton_Clicked;
                buttons.Add(humanModeButton);

                ButtonSmall computerModeButton = new ButtonSmall(new Vector2(0, 260), "Computer Mode", ButtonSmall.ALLIGNVERTICALCENTERED, content);
                computerModeButton.Clicked += computerModeButton_Clicked;
                buttons.Add(computerModeButton);

                ButtonSmall helpButton = new ButtonSmall(new Vector2(0, 320), "Help", ButtonSmall.ALLIGNVERTICALCENTERED, content);
                helpButton.Clicked += helpButton_Clicked;
                buttons.Add(helpButton);

                ButtonSmall exitButton = new ButtonSmall(new Vector2(0, 380), "Exit", ButtonSmall.ALLIGNVERTICALCENTERED, content);
                exitButton.Clicked += exitButton_Clicked;
                buttons.Add(exitButton);
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
                    ScreenManager.Game.Exit();
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
            foreach (ButtonSmall button in buttons){
                button.Draw(this);
            }
            spriteBatch.End();
        }

        void humanModeButton_Clicked(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, PlayerIndex.One, new SelectSize(MODEHUMAN));
        }

        void computerModeButton_Clicked(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, PlayerIndex.One, new SelectSize(MODECOMPUTER));
        }

        void helpButton_Clicked(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, PlayerIndex.One, new Help());
        }

        void exitButton_Clicked(object sender, EventArgs e)
        {
            ScreenManager.Game.Exit();
        }
    }
}
 