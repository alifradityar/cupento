using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using GameStateManagement;

namespace GameStateManagementSample
{
    class ButtonSmall
    {
        public const int ALLIGNNORMAL = 0;
        public const int ALLIGNHORIZONTALCENTERED = 1;
        public const int ALLIGNVERTICALCENTERED = 2;

        Texture2D state_on, state_off;
        Vector2 position;
        int width, height;
        public bool isPressed;
        string text;
        public event EventHandler<EventArgs> Clicked;

        public ButtonSmall(Vector2 position, string text, int allign, ContentManager content)
        {
            this.text = text;
            isPressed = false;
            state_on = content.Load<Texture2D>("button_1");
            state_off = content.Load<Texture2D>("button_0");
            width = state_on.Width;
            height = state_off.Height;
            if (allign == ALLIGNNORMAL)
            {
                this.position = position;
            }
            if (allign == ALLIGNVERTICALCENTERED)
            {
                position.X = 400 - state_on.Width / 2;
                this.position = position;
            }
            if (allign == ALLIGNHORIZONTALCENTERED)
            {
                position.Y = 240 - state_on.Height / 2;
                this.position = position;
            }
        }

        public bool CheckForPressed(Vector2 touchPosition)
        {
            if ((touchPosition.X >= position.X && touchPosition.X <= position.X + width) && (touchPosition.Y >= position.Y && touchPosition.Y <= position.Y + height))
            {
                isPressed = true;
                return true;
            }
            else
            {
                isPressed = false;
                return false;
            }
        }
        protected virtual void onClick()
        {
            if (Clicked != null)
                Clicked(this, EventArgs.Empty);
        }

        public bool CheckForClicked(Vector2 touchPosition)
        {
            if ((touchPosition.X >= position.X && touchPosition.X <= position.X + width) && (touchPosition.Y >= position.Y && touchPosition.Y <= position.Y + height))
            {
                onClick();
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Draw(GameScreen screen)
        {
            SpriteBatch spriteBatch = screen.ScreenManager.SpriteBatch;
            SpriteFont font = screen.ScreenManager.Font;
            if (isPressed)
                spriteBatch.Draw(state_on, position, Color.White);
            else
                spriteBatch.Draw(state_off, position, Color.White);

            Vector2 textSize = font.MeasureString(text);
            Vector2 textPosition = position + (new Vector2(width/2, height/2) - textSize / 2f);
            spriteBatch.DrawString(font, text, textPosition, Color.White);
        }
    }

    class ButtonBig
    {
        public const int ALLIGNNORMAL = 0;
        public const int ALLIGNTYPE1 = 1;
        public const int ALLIGNTYPE2 = 2;
        public const int ALLIGNTYPE3 = 3;
        public const int ALLIGNTYPE4 = 4;

        Texture2D state_on, state_off;
        Vector2 position;
        int width, height;
        public bool isPressed;
        string text;
        public event EventHandler<EventArgs> Clicked;

        public ButtonBig(Vector2 position, string text, int allign, ContentManager content)
        {
            this.text = text;
            isPressed = false;
            state_on = content.Load<Texture2D>("button_big_1");
            state_off = content.Load<Texture2D>("button_big_0");
            width = state_on.Width;
            height = state_off.Height;
            if (allign == ALLIGNNORMAL)
            {
                this.position = position;
            }
            if (allign == ALLIGNTYPE1)
            {
                position.X = 400 - state_on.Width - 10 ;
                position.Y = 240 - state_on.Height + 10;
                this.position = position;
            }
            if (allign == ALLIGNTYPE2)
            {
                position.X = 400 + 10;
                position.Y = 240 - state_on.Height + 10;
                this.position = position;
            }
            if (allign == ALLIGNTYPE3)
            {
                position.X = 400 - state_on.Width - 10;
                position.Y = 240 + 30;
                this.position = position;
            }
            if (allign == ALLIGNTYPE4)
            {
                position.X = 400 + 10;
                position.Y = 240 + 30;
                this.position = position;
            }
        }

        public bool CheckForPressed(Vector2 touchPosition){
            if ((touchPosition.X >= position.X && touchPosition.X <= position.X + width) && (touchPosition.Y >= position.Y && touchPosition.Y <= position.Y + height))
            {
                isPressed = true;
                return true;
            }
            else
            {
                isPressed = false;
                return false;
            }
        }
        protected virtual void onClick()
        {
            if (Clicked != null)
                Clicked(this, EventArgs.Empty);
        }

        public bool CheckForClicked(Vector2 touchPosition)
        {
            if ((touchPosition.X >= position.X && touchPosition.X <= position.X + width) && (touchPosition.Y >= position.Y && touchPosition.Y <= position.Y + height))
            {
                onClick();
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Draw(GameScreen screen)
        {
            SpriteBatch spriteBatch = screen.ScreenManager.SpriteBatch;

            SpriteFont font = screen.ScreenManager.BigFont;
            if (isPressed)
                spriteBatch.Draw(state_on, position, Color.White);
            else
                spriteBatch.Draw(state_off, position, Color.White);

            Vector2 textSize = font.MeasureString(text);
            Vector2 textPosition = position + (new Vector2(width/2, height/2) - textSize / 2f);
            spriteBatch.DrawString(font, text, textPosition, Color.White);
        }
    }

    class ButtonIcon
    {
        Texture2D state_on, state_off;
        Vector2 position;
        int width, height;
        public bool isPressed;
        string text;
        public event EventHandler<EventArgs> Clicked;
        public event EventHandler<EventArgs> Pressed;

        public ButtonIcon(Vector2 position,string icon_0,string icon_1, ContentManager content)
        {
            isPressed = false;
            state_on = content.Load<Texture2D>(icon_1);
            state_off = content.Load<Texture2D>(icon_0);
            width = state_on.Width;
            height = state_off.Height;
            this.position = position;
        }

        public bool CheckForPressed(Vector2 touchPosition)
        {
            if ((touchPosition.X >= position.X && touchPosition.X <= position.X + width) && (touchPosition.Y >= position.Y && touchPosition.Y <= position.Y + height))
            {
                onPressed();
                isPressed = true;
                return true;
            }
            else
            {
                isPressed = false;
                return false;
            }
        }

        protected virtual void onPressed()
        {
            if (Pressed != null)
            {
                Pressed(this, EventArgs.Empty);
            }
        }

        protected virtual void onClick()
        {
            if (Clicked != null)
                Clicked(this, EventArgs.Empty);
        }

        public bool CheckForClicked(Vector2 touchPosition)
        {
            if ((touchPosition.X >= position.X && touchPosition.X <= position.X + width) && (touchPosition.Y >= position.Y && touchPosition.Y <= position.Y + height))
            {
                onClick();
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Draw(GameScreen screen)
        {
            SpriteBatch spriteBatch = screen.ScreenManager.SpriteBatch;

            if (isPressed)
                spriteBatch.Draw(state_on, position, Color.White);
            else
                spriteBatch.Draw(state_off, position, Color.White);
        }
    }
}
