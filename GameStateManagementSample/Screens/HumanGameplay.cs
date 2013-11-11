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

namespace GameStateManagementSample
{
    class HumanGameplay : GameScreen
    {
        ContentManager content;
        Board board;
        Texture2D background, scrollbar, clock_icon;
        SoundEffect soundEffect;
        Vector2 scrollbarpos;
        int size, difficulty;
        double timer;
        SpriteFont titleFont, timeFont;
        int focus_piece = -1;
        bool flag = false;
        List<Piece> pieces = new List<Piece>();

        List<ButtonIcon> buttons = new List<ButtonIcon>();

        public HumanGameplay(int size, int difficulty)
        {
            this.size = size;
            this.difficulty = difficulty;

            EnabledGestures = GestureType.HorizontalDrag | GestureType.VerticalDrag | GestureType.Tap | GestureType.DoubleTap | GestureType.DragComplete | GestureType.FreeDrag | GestureType.Hold;
            board = new Board(difficulty * 4 + size);
        }

        private void LoadPieces()
        {
            for (int i = 0; i < 12; i++)
            {
                pieces.Add(new Piece(i, 120 + 250*i, 340));
            }
        }
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");
                background = content.Load<Texture2D>("bg_gameplay");
                clock_icon = content.Load<Texture2D>("clock_icon");
                scrollbar = content.Load <Texture2D>("scroll_bar");
                titleFont = content.Load<SpriteFont>("titleFont");
                timeFont = content.Load<SpriteFont>("timeFont");
                scrollbarpos = new Vector2(80, 460);
                LoadPieces();

                ButtonIcon leftButton = new ButtonIcon(new Vector2(10, 380), "arrow_left", "arrow_left_1", content);
                leftButton.Pressed += leftButton_Pressed;
                buttons.Add(leftButton);
                ButtonIcon rightButton = new ButtonIcon(new Vector2(740, 380), "arrow_right", "arrow_right_1", content);
                rightButton.Pressed += rightButton_Pressed;
                buttons.Add(rightButton);
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
                        foreach (ButtonIcon button in buttons)
                        {
                            flag = button.CheckForClicked(touchLocation.Position);
                        }
                    }
                    else
                    {
                        foreach (ButtonIcon button in buttons)
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
                foreach (ButtonIcon button in buttons)
                {
                    button.isPressed = false;
                }
            }
            foreach (GestureSample gesture in input.Gestures){
                if (gesture.GestureType == GestureType.DoubleTap && focus_piece == -1)
                {
                    if (gesture.Position.X < 100 || gesture.Position.X > 700)
                        continue;
                    foreach(Piece piece in pieces){
                        if (piece.IsInside(gesture.Position)){
                            piece.Rotate();
                        }
                    }
                }
                else if (gesture.GestureType == GestureType.HorizontalDrag && focus_piece == -1)
                {
                    if (gesture.Position.X < 100 || gesture.Position.X > 700)
                        continue;
                    foreach(Piece piece in pieces){
                        if (piece.IsInside(gesture.Position)){
                            piece.FlipHorizontal();
                        }
                    }
                }
                else if (gesture.GestureType == GestureType.VerticalDrag && focus_piece == -1)
                {
                    if (gesture.Position.X < 100 || gesture.Position.X > 700)
                        continue;
                    foreach(Piece piece in pieces){
                        if (piece.IsInside(gesture.Position)){
                            piece.FlipVertical();
                        }
                    }
                }
                else if (gesture.GestureType == GestureType.Hold && focus_piece == -1)
                {
                    if ((gesture.Position.X < 100 || gesture.Position.Y > 700) && (gesture.Position.Y > 380))
                        continue;
                    Debug.WriteLine("Hold");
                    bool flag2 = false;
                    foreach (Piece piece in pieces)
                    {
                        if (piece.IsInside(gesture.Position))
                        {
                            Debug.WriteLine("bingo");
                            flag2 = true;
                            focus_piece = pieces.IndexOf(piece);
                            break;
                        }
                    }
                    if (flag2)
                        break;
                    Piece p = board.RemovePiece(gesture.Position);
                    if (p != null)
                    {
                        pieces.Add(p);
                        focus_piece = pieces.IndexOf(p);
                    }
                        
                }
                else if (gesture.GestureType == GestureType.FreeDrag && focus_piece != -1)
                {
                    Debug.WriteLine("FreeDrag");
                    pieces[focus_piece].X = (int)(gesture.Position.X + gesture.Delta.X);
                    pieces[focus_piece].Y = (int)(gesture.Position.Y + gesture.Delta.Y);
                }
                else if (gesture.GestureType == GestureType.DragComplete && focus_piece != -1)
                {
                    if (board.IsCanPut(pieces[focus_piece]))
                    {
                        board.putPiece(pieces[focus_piece]);
                        pieces.RemoveAt(focus_piece);
                        if (board.IsComplete())
                        {
                            End();
                        }
                    }
                    Debug.WriteLine("Complete");
                    focus_piece = -1;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin();
            spriteBatch.Draw(background, new Vector2(0, 0), Color.White);
            spriteBatch.Draw(clock_icon, new Vector2(650,15), Color.White);
            switch (difficulty) 
            {
                case SelectDifficulty.DIFFICULTYEASY:
                    spriteBatch.DrawString(titleFont, "Human Mode | Easy", new Vector2(10, 10), Color.White);
                    break;
                case SelectDifficulty.DIFFICULTYNORMAL:
                    spriteBatch.DrawString(titleFont, "Human Mode | Normal", new Vector2(10, 10), Color.White);
                    break;
                case SelectDifficulty.DIFFICULTYHARD :
                    spriteBatch.DrawString(titleFont, "Human Mode | Hard", new Vector2(10, 10), Color.White);
                    break;
                case SelectDifficulty.DIFFICULTYIMPOSSIBLE:
                    spriteBatch.DrawString(titleFont, "Human Mode | Impossible", new Vector2(10, 10), Color.White);
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
            foreach(Piece piece in pieces){
                piece.Draw(this);
            }
            spriteBatch.Draw(scrollbar, scrollbarpos, Color.White);
            foreach (ButtonIcon button in buttons)
            {
                button.Draw(this);
            }

            spriteBatch.End();
        }


        void leftButton_Pressed(object sender, EventArgs e)
        {
            if (scrollbarpos.X > 80)
            {
                foreach (Piece piece in pieces)
                {
                    if (piece.Y > 335)
                        piece.X += 20;
                }
                scrollbarpos.X -= 4.5f;
            }
        }
        void rightButton_Pressed(object sender, EventArgs e)
        {
            if (scrollbarpos.X < 800 - 80 - scrollbar.Width)
            {
                foreach (Piece piece in pieces)
                {
                    if (piece.Y > 335)
                        piece.X -= 20;
                }
                scrollbarpos.X += 4.5f;
            }
        }
        void End()
        {
            LoadingScreen.Load(ScreenManager, false, PlayerIndex.One, new GameOver((int)timer));
        }

    }
}
