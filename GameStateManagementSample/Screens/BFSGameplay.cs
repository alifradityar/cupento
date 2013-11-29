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
    class BFSGameplay : GameScreen
    {
        ContentManager content;
        Board finalBoard = null;
        Board board;
        Texture2D background, clock_icon;
        SoundEffect soundEffect;
        int size, difficulty;
        double timer,timer2 = 0;
        SpriteFont titleFont, timeFont;
        int focus_piece = -1;
        bool flag = false, flag3 = false;
        bool wait = true;
        double delay = 5;

        List<ButtonSmall> buttons = new List<ButtonSmall>();

        public void SolveRec()
        {
            bool bol = Solve();
        }

        public bool Solve()
        {
            Board problem = board;
            //timer.Start();
            queue.Enqueue(problem);

            System.Console.WriteLine("Solving starts now!");


            while (queue.Count > 0)
            {
                board = queue.Dequeue();

                //System.Console.WriteLine("row = " + r + ", col = " + c + ", mask = " + mask);

                Vector2 pos = board.GetCurrentRowCol();
                int r = (int)pos.X, c = (int)pos.Y;

                for (int id_piece = 0; id_piece < 12; ++id_piece) // try every available pentomino pieces 
                {
                    if (!board.IsPieceAvailable(id_piece)) continue;


                    List<Piece> p_list = PieceHelper.GetAllPieceConfig(id_piece);

                    foreach (Piece piece in p_list)
                    {
                        if (board.IsCanPut(piece, r, c))
                        {
                            Board _board = new Board(board);
                            _board.PutPiece(piece, r, c);

                            if (_board.IsImpossibru())
                                continue;

                            // _board.Print();
                            //System.Console.ReadKey();

                            if (_board.IsComplete()) // if all pieces have been put, then we found the solution
                            {
                                finalBoard = _board;
                                board = finalBoard;

                                //timer.Stop();

                                return true;
                            }

                            queue.Enqueue(_board);
                        }
                    }
                }
            }
            queue.Clear();
            
            return false;
        }

        Thread thread;
        private Queue<Board> queue;
        public BFSGameplay(int size, int difficulty)
        {
            this.size = size;
            this.difficulty = difficulty;

            EnabledGestures = GestureType.Tap;
            queue = new Queue<Board>();
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
                if (board.IsComplete())
                {
                    flag3 = true;
                    thread.Join();
                    //End();
                }
                if (flag3)
                {
                    timer2 += gameTime.ElapsedGameTime.TotalSeconds;
                    if (timer2 > delay)
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


    }
}
