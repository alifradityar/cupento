using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameStateManagement;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Microsoft.Xna.Framework.Audio;
using System.Threading;

namespace GameStateManagementSample
{
    class DFSGameplay : GameScreen
    {
        ContentManager content;
        Board finalBoard = null;
        Board board;
        Texture2D background, clock_icon;
        SoundEffect soundEffect;
        int size, difficulty;
        double timer,timer_for_animation;
        SpriteFont titleFont, timeFont;
        int focus_piece = -1;
        bool flag = false;
        bool wait = true;
        double delay = 0.5;

        List<ButtonSmall> buttons = new List<ButtonSmall>();

        public void SolveRec()
        {
            bool bol = SolveRec(0, 0);
        }

        public bool SolveRec(int r, int c)
        {
            //System.Console.WriteLine(r + " " + c);
            //board.Print();
            wait = true;
            while (wait)
            {
                // Wait
            }
            if (finalBoard != null)
                return true;

            if (board.IsComplete())
            {
                finalBoard = board;
                return true;
            }

            if (c >= board.N_COLS)
            {
                return SolveRec(r + 1, 0);
            }

            if (!board.GetNeedBitMask(r, c))
            {
                return SolveRec(r, c + 1);
            }

            //board.Print();

            for (int id_piece = 0; id_piece < 12; ++id_piece) // try every available pentomino pieces 
            {
                if (!board.IsPieceAvailable(id_piece)) continue;

                List<Piece> p_list = PieceHelper.GetAllPieceConfig(id_piece);

                foreach (Piece piece in p_list)
                {
                    if (board.IsCanPut(piece, r, c))
                    {
                        board.PutPiece(piece, r, c);

                        bool flag = SolveRec(r, c + 1);

                        if (flag) return true;

                        board.DeletePiece(piece, r, c);
                    }
                }
            }

            return false;
        }

        Thread thread;
        public DFSGameplay(int size, int difficulty)
        {
            this.size = size;
            this.difficulty = difficulty;

            EnabledGestures = GestureType.Tap;
            board = new Board(difficulty * 4 + size);
            thread = new Thread(SolveRec);
            thread.Start();
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");
                background = content.Load<Texture2D>("bg_gameplay");
                clock_icon = content.Load<Texture2D>("clock_icon");
                titleFont = content.Load<SpriteFont>("titleFont");
                timeFont = content.Load<SpriteFont>("timeFont");
                soundEffect = content.Load<SoundEffect>("POL-final-act-short");
                ButtonSmall slowButton = new ButtonSmall(new Vector2(5, 365), "Slow", ButtonSmall.ALLIGNNORMAL, content);
                slowButton.Clicked += slowButton_Clicked;
                buttons.Add(slowButton);

                ButtonSmall normalButton = new ButtonSmall(new Vector2(5 + 260 + 5, 365), "Normal", ButtonSmall.ALLIGNNORMAL, content);
                normalButton.Clicked += normalButton_Clicked;
                buttons.Add(normalButton);

                ButtonSmall fastButton = new ButtonSmall(new Vector2(2 + 260 + 5 + 260 + 5, 365), "Fast", ButtonSmall.ALLIGNNORMAL, content);
                fastButton.Clicked += fastButton_Clicked;
                buttons.Add(fastButton);
            }
        }
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (IsActive)
            {
                if (flag)
                {
                    SoundEffectInstance instance = soundEffect.CreateInstance();
                    instance.IsLooped = true;
                    instance.Play();
                    flag = false;

                }
                timer += gameTime.ElapsedGameTime.TotalSeconds;
                if (wait)
                {
                    timer_for_animation += gameTime.ElapsedGameTime.TotalSeconds;
                    if (timer_for_animation > delay)
                    {
                        timer_for_animation = 0;
                        wait = false;
                    }
                }
                if (board.IsComplete())
                {
                    thread.Join();
                    End();
                }
            }
            else
            {
                flag = true;
            }
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            bool flag = false;
            foreach (GamePadState gamePadState in input.CurrentGamePadStates)
            {
                if (gamePadState.Buttons.Back == ButtonState.Pressed)
                {
                    ScreenManager.AddScreen(new Pause(), ControllingPlayer);
                }
            }
            TouchCollection touchCollection = input.TouchState;
            if (touchCollection.Count > 0)
            {
                foreach (TouchLocation touchLocation in touchCollection)
                {
                    if (touchLocation.State == TouchLocationState.Released)
                    {
                        foreach (ButtonSmall button in buttons)
                        {
                            flag = button.CheckForClicked(touchLocation.Position);
                        }
                    }
                    else
                    {
                        foreach (ButtonSmall button in buttons)
                        {
                            flag = button.CheckForPressed(touchLocation.Position);
                        }
                    }
                    if (flag)
                        break;
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
            spriteBatch.Draw(clock_icon, new Vector2(650, 15), Color.White);
            switch (difficulty)
            {
                case SelectDifficulty.DIFFICULTYEASY:
                    spriteBatch.DrawString(titleFont, "Computer Mode | Easy", new Vector2(10, 10), Color.White);
                    break;
                case SelectDifficulty.DIFFICULTYNORMAL:
                    spriteBatch.DrawString(titleFont, "Computer Mode | Normal", new Vector2(10, 10), Color.White);
                    break;
                case SelectDifficulty.DIFFICULTYHARD:
                    spriteBatch.DrawString(titleFont, "Computer Mode | Hard", new Vector2(10, 10), Color.White);
                    break;
                case SelectDifficulty.DIFFICULTYIMPOSSIBLE:
                    spriteBatch.DrawString(titleFont, "Computer Mode | Impossible", new Vector2(10, 10), Color.White);
                    break;
                default:
                    break;
            }
            int time_in_sec = (int)timer;

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
            spriteBatch.DrawString(timeFont, strtime, new Vector2(700, 10), Color.White);
            board.Draw(this);
            foreach (ButtonSmall button in buttons)
            {
                button.Draw(this);
            }

            spriteBatch.End();
        }

        void End()
        {
            LoadingScreen.Load(ScreenManager, false, PlayerIndex.One, new GameOver((int)timer));
        }

        void slowButton_Clicked(object sender, EventArgs e)
        {
            delay = 0.1;
        }

        void normalButton_Clicked(object sender, EventArgs e)
        {
            delay = 0.03;
        }

        void fastButton_Clicked(object sender, EventArgs e)
        {
            delay = 0;
        }

    }
}
