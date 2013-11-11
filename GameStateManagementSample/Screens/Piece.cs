using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GameStateManagement;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace GameStateManagementSample
{
    public class Piece
    {
        ////////// DEFAULT CONFIGURATIONS ///////////
        //Texture2D texture;
        private ulong mask;
        private int id;
        private int id_config;
        static List<Color> colors;
        static Color colorBorder;

        private static bool color_initiated = false;

        private static void initPieceConfigs()
        {
            colors = new List<Color>();
            colors.Add(Color.Magenta);
            colors.Add(Color.Orange);
            colors.Add(Color.LightGreen);
            colors.Add(Color.SpringGreen);
            colors.Add(new Color(175, 189, 119)); // Martian Green
            colors.Add(Color.OliveDrab);
            colors.Add(new Color(245, 135, 79)); // Autumn Orange
            colors.Add(new Color(164, 94, 77)); // Ruby Red
            colors.Add(new Color(237, 47, 89)); // Neon Red
            colors.Add(new Color(199, 94, 163)); // Neon Purple
            colors.Add(new Color(139, 79, 139)); // Deep River
            colors.Add(Color.LightYellow);

            colorBorder = new Color(230, 231, 232);
        }


        public int X, Y; // center X & Y position. Can be accessed publicly

        public Piece(int id, int id_config, int X, int Y)
        {
            // initialize only at first
            if (color_initiated == false)
            {
                color_initiated = true;
                initPieceConfigs();
            }

            this.X = X;
            this.Y = Y;
            this.id = id;
            this.id_config = id_config;
            this.mask = PieceHelper.GetPieceConfig(id, id_config);
            //texture = content.Load<Texture2D>("pentamino_color");
        }


        public Piece(int id, int X, int Y)
        {
            // initialize only at first
            if (color_initiated == false)
            {
                color_initiated = true;
                initPieceConfigs();
            }

            this.X = X;
            this.Y = Y;
            this.id = id;
            this.id_config = 0;
            this.mask = PieceHelper.GetPieceConfig(id, id_config);
            //texture = content.Load<Texture2D>("pentamino_color");
        }

        public Piece(Piece piece)
        {
            mask = piece.mask;
            id = piece.id;
            id_config = piece.id_config;
            X = piece.X;
            Y = piece.Y;
        }

        public int GetId()
        {
            return id;
        }

        public int GetIdConfig()
        {
            return id_config;
        }

        public void Rotate()
        {
            mask = PieceHelper.RotateBitPiece(mask);

            List<Piece> lp = PieceHelper.GetAllPieceConfig(id);
            for (int i = 0; i < lp.Count; ++i)
            {
                if (lp[i].GetMask() == mask)
                {
                    id_config = lp[i].GetIdConfig();
                    break;
                }
            }
        }

        public void Mirror()
        {
            mask = PieceHelper.FlipBitPiece(mask);

            List<Piece> lp = PieceHelper.GetAllPieceConfig(id);
            for (int i = 0; i < lp.Count; ++i)
            {
                if (lp[i].GetMask() == mask)
                {
                    id_config = lp[i].GetIdConfig();
                    break;
                }
            }
        }

        public void FlipHorizontal()
        {
            Mirror();
        }

        public void FlipVertical()
        {
            Mirror();
            Rotate();
            Rotate();
        }

        public void Print()
        {
            PieceHelper.PrintConfig(mask);
        }

        public ulong GetMask()
        {
            return mask;
        }

        public bool CheckBit(int it)
        {
            return PieceHelper.CheckBit(mask, it);
        }

        public bool CheckBit(int r, int c)
        {
            return PieceHelper.CheckBit(mask, r, c);
        }

        public int[][] GetGridConfig()
        {
            return PieceHelper.GetGridPieceConfig(this);
        }

        public int GetNRow()
        {
            int row = 0;
            int check = 0;
            int[][] config = GetGridConfig();
            do
            {
                for (int i = 0; i < 5; ++i)
                    if (config[row][i] == 1) { check = 1; break; }
                row++;
            } while (check == 1 && row < 4);

            return row;
        }

        public int GetNCol()
        {
            int col = 0;
            int check = 0;
            int[][] config = GetGridConfig();
            do
            {
                for (int i = 0; i < 5; ++i)
                    if (config[i][col] == 1) { check = 1; break; }
                col++;
            } while (check == 1 && col < 4);

            return col;
        }

        public void Draw(GameScreen screen)
        {
            SpriteBatch spriteBatch = screen.ScreenManager.SpriteBatch;
            Texture2D blank = screen.ScreenManager.BlankTexture;
            int[][] config = GetGridConfig();
            for (int i = 0; i < config.Length; i++)
            {
                for (int j = 0; j < config[i].Length; j++)
                {
                    if (config[i][j] == 1)
                    {
                        Rectangle rect1 = new Rectangle(X + j * 40 - j * 2, Y + i * 40 - i * 2, 40, 40);
                        spriteBatch.Draw(blank, rect1, colorBorder);
                        Rectangle rect2 = new Rectangle(X + j * 40 - j * 2 + 2, Y + i * 40 - i * 2 + 2, 36, 36);
                        spriteBatch.Draw(blank, rect2, colors[id]);
                    }
                }
            }

        }

        public bool IsInside(Vector2 position)
        {
            int[][] config = GetGridConfig();
            for (int i = 0; i < config.Length; i++)
            {
                for (int j = 0; j < config[i].Length; j++)
                {
                    if (config[i][j] == 0) continue;
                    int X1 = X + 38 * j;
                    int Y1 = Y + 38 * j;
                    if ((position.X >= X1 && position.X <= X1 + 40) && (position.Y >= Y1 && position.Y <= Y1 + 40))
                        return true;
                }
            }
            return false;
        }

    }
}
