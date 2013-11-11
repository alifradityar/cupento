using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GameStateManagement;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace GameStateManagementSample
{
    class Board
    {
        private ulong empty; // bitmask dari kolom-kolom yang kosong
        private ulong need; // bitmask dari kolom-kolom yang harus diisi
        public byte[] piece_position; // posisi kolom dari pieces yang sudah ditaruh
        public byte[] piece_config; // konfigurasi dari pieces yang sudah ditaruh

        //public enum State : byte { Empty = 0, Need = 1, Filled = 2 };
        //private State[][] prob; // contains the Must-Filled columns
        public int N_ROWS, N_COLS; // number of rows & columns
        Color inner, border;
        public int X, Y; // top-left coordinate of board, used for drawing

        //List<Tuple<Piece, int, int>> pieces_on_board;

        public Board(int row,int col)
        {
            X = 400 - (40 * col / 2) + col;
            Y = 210 - (40 * row / 2) + row;
            N_ROWS = row;
            N_COLS = col;
            //prob = new State[N_ROWS][];
            empty = 0;
            need = ((ulong)1 << (N_ROWS * N_COLS)) - 1;
            //pieces_on_board = new List<Tuple<Piece, int, int>>();
            inner = new Color(136, 159, 172);
            border = new Color(230, 231, 232);
            piece_position = new byte[12];
            piece_config = new byte[12];
            for (int i = 0; i < 12; ++i)
                piece_position[i] = piece_config[i] = 255;
        }

        public Board(int id_soal)
        {
            string soal = "soal" + id_soal + ".xml";
            string str;
            Debug.WriteLine(soal);
            using (System.Xml.XmlReader reader = System.Xml.XmlReader.Create(soal))
            {
                reader.MoveToContent();
                reader.ReadToFollowing("pentomino");
                str = reader.ReadInnerXml();
            }
            char[] delimiters = new char[] { '\r', '\n' };
            string[] lines = str.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

            N_ROWS = lines.Length;
            N_COLS = lines[0].Length;
            Debug.WriteLine(N_ROWS + " -- " + N_COLS);
            X = 400 - (40 * N_COLS / 2) + N_COLS;
            Y = 210 - (40 * N_ROWS / 2) + N_ROWS;
            //prob = new State[N_ROWS][];
            need = empty = 0;
            for (int i = 0,it = 0; i < N_ROWS; ++i)
            {
                Debug.WriteLine(lines[i]);
                for (int j = 0; j < N_COLS; ++j, ++it)
                {
                    if (lines[i][j] == '1') need += ((ulong)1 << it);
                    else empty += ((ulong)1 << it);
                    //a[i][j] = -1;
                }
            }
            inner = new Color(136, 159, 172);
            border = new Color(230, 231, 232);
            piece_position = new byte[12];
            piece_config = new byte[12];
            for (int i = 0; i < 12; ++i)
                piece_position[i] = piece_config[i] = 255;
            //pieces_on_board = new List<Tuple<Piece, int, int>>();
        }

        public void Print()
        {
            Debug.WriteLine("Board : ");
            for (int i = 0; i < N_ROWS; ++i)
            {
                string str = "";
                for (int j = 0; j < N_COLS; ++j)
                {
                    str = str + ((GetNeedBitMask(i, j)) ? "1 " : "0 ");
                }
                Debug.WriteLine(str);
            }
            Debug.WriteLine("-------------------------");

            //PrintPento();
        }

        public bool IsComplete()
        {
            return (need == 0);
        }
        public bool IsPieceAvailable(int id)
        {
            return (piece_position[id] == 255) ? true : false;
        }
        public bool GetNeedBitMask(int r, int c)
        {
            return (need & ((ulong)1 << (r * N_COLS + c))) > 0;
        }

        public void SetNeedBitMask(int r, int c, int val)
        {
            if (val == 0) { need &= ~((ulong)1 << (r * N_COLS + c)); }
            else { need |= ((ulong)1 << (r * N_COLS + c)); }
        }

        // Check wether a piece can be put at [r][c] or not
        public bool IsCanPut(Piece piece)
        {
            int c = piece.X - X - (piece.X-X)/38*2;
            c = c / 38;
            int r = piece.Y - Y - (piece.Y - Y) / 38 * 2;
            r = r / 38;

            //int shft = 0;

            //while (!piece.CheckBit(0, shft)) shft++;

            //c += shft;
            Debug.WriteLine(piece.X + " " + piece.Y + " " + X + " " + Y);
            Debug.WriteLine("try to put in r = " + r + ", c = " + c);
            if (c < 0 || c >= N_COLS || r < 0 || r >= N_ROWS)
            {
                Debug.WriteLine("failed_1");
                return false;
            }
            return IsCanPut(piece, r, c);
        }

        // Check wether a piece can be put at [r][c] or not
        public bool IsCanPut(Piece piece, int r, int c)
        {
            int shft = 0;

            while (!piece.CheckBit(0, shft)) shft++;
            //System.Console.WriteLine(shft);

            int temp = 0;
            for (int i = 0; i < 5; ++i)
                for (int j = 0; j < 5; ++j)
                {
                    if (!piece.CheckBit(i, j)) continue;

                    int _i = i + r;
                    int _j = j + c - shft;
                    temp++;

                   //Debug.WriteLine("at " + _i + " and " + _j);

                    if (_i < 0 || _i >= N_ROWS || _j < 0 || _j >= N_COLS)
                    {
                        Debug.WriteLine("ERROR DI : " + i + ", " + j + "--" + _i + ", " + _j);
                        Print();
                        return false;
                    }

                    if (!GetNeedBitMask(_i, _j))
                    {
                        Debug.WriteLine("-ERROR DI : " + i + ", " + j + "--" + _i + ", " + _j);
                        Print();
                        return false;
                    }
                }

            if (temp != 5)
            {
                piece.Print();
                //System.Console.WriteLine(piece.GetId() + " ROTOPOL");
            }

            return true;
        }

        public void SetBoardState(Piece piece, int r, int c, int val)
        {
            int shft = 0;

            while (!piece.CheckBit(0, shft)) shft++;
            Debug.WriteLine(r + " " + c);
            Debug.WriteLine(shft);

            for (int i = 0; i < 5; ++i)
                for (int j = 0; j < 5; ++j)
                {
                    if (!piece.CheckBit(i, j)) continue;

                    int _i = i + r;
                    int _j = j + c - shft;
                    Debug.WriteLine(i + " " + j);
                    Debug.WriteLine("at " + _i + " and " + _j + " with value " + val);
                    if (_i < 0 || _i >= N_ROWS || _j < 0 || _j >= N_COLS)
                    {
                        //System.Console.WriteLine("at " + _i + " and " + _j);
                        //System.Console.ReadKey();
                    }

                    SetNeedBitMask(_i, _j, val);
                    //a[_i][_j] = piece.GetId();

                    //Print();
                }
        }

        public void DeletePiece(Piece piece, int r, int c)
        {
            SetBoardState(piece, r, c, 1);
            piece_position[piece.GetId()] = (byte)255;
            piece_config[piece.GetId()] = (byte)255;
        }

        public Piece RemovePiece(Vector2 position)
        {
            List<Tuple<Piece, int, int>> pieces_on_board = ParseSolution();
            foreach (Tuple<Piece, int, int> tuple in pieces_on_board)
            {
                Piece piece = tuple.First;
                int r = tuple.Second;
                int c = tuple.Third;
                Debug.WriteLine(r + " " + c);
                if (piece.IsInside(position))
                {
                    int shft = 0;
                    while (!piece.CheckBit(0, shft)) shft++;
                    c += shft;
                    SetBoardState(piece, r, c, 1);
                    piece_position[piece.GetId()] = (byte)(r * N_COLS + c);
                    piece_config[piece.GetId()] = (byte)piece.GetIdConfig();
                    DeletePiece(piece, r, c);
                    return piece;
                }
            }
            return null;
        }

        public void PutPiece(Piece piece, int r, int c)
        {
            SetBoardState(piece, r, c, 0);

            int shft = 0;

            while (!piece.CheckBit(0, shft)) shft++;

            c -= shft;

            piece_position[piece.GetId()] = (byte)(r * N_COLS + c);
            piece_config[piece.GetId()] = (byte)piece.GetIdConfig();
            Print();
        }

        public void putPiece(Piece piece)
        {

            int c = piece.X - X - (piece.X - X) / 38 * 2;
            c = c / 38;
            int r = piece.Y - Y - (piece.Y - Y) / 38 * 2;
            r = r / 38;

            //int shft = 0;

            //while (!piece.CheckBit(0, shft)) shft++;

            //c += shft;
            
            Debug.WriteLine("put in r = " + r + ", c = " + c);
            piece.X = X + c * 40 - c*2;
            piece.Y = Y + r * 40 - r*2;
            SetBoardState(piece, r, c, 0);
            PutPiece(piece, r, c);
        }

        public bool GetEmptyBitMask(int r, int c)
        {
            return (empty & ((ulong)1 << (r * N_COLS + c))) > 0;
        }

        public void Draw(GameScreen screen)
        {
            SpriteBatch spriteBatch = screen.ScreenManager.SpriteBatch;
            Texture2D blank = screen.ScreenManager.BlankTexture;
            for (int i = 0; i < N_ROWS;i++ )
            {
                for (int j = 0; j < N_COLS; j++)
                {
                    if (!GetEmptyBitMask(i,j)){
                        Rectangle rect1 = new Rectangle(X + j * 40 - j * 2, Y + i * 40 - i * 2, 40, 40);
                        spriteBatch.Draw(blank, rect1, border);
                        Rectangle rect2 = new Rectangle(X + j * 40 - j * 2 + 2, Y + i * 40 - i * 2 + 2, 36, 36);
                        spriteBatch.Draw(blank, rect2, inner);
                    }
                }
            }
            List<Tuple<Piece, int, int>> pieces_on_board = ParseSolution();
            foreach (Tuple<Piece, int, int> tuple_piece in pieces_on_board)
            {
                Piece piece = tuple_piece.First;
                piece.Draw(screen);
            }
        }

        public List<Tuple<Piece, int, int>> ParseSolution()
        {
            List<Tuple<Piece, int, int>> solution = new List<Tuple<Piece, int, int>>();

            for (int i = 0; i < 12; ++i)
            {
                int conf = piece_config[i];
                int pos = piece_position[i];
                if (pos < 255)
                {
                    int c = pos % N_COLS;
                    int r = pos / N_COLS;
                    Piece piece = new Piece(i, conf, 0, 0);
                    piece.X = X + c * 40 - c * 2;
                    piece.Y = Y + r * 40 - r * 2;

                    solution.Add(new Tuple<Piece, int, int>(piece, r,c));

                   // Debug.WriteLine("Solution added --> " +i + " -- " + r + ", " + c);
                }
            }

            return solution;
        }

    }
}
