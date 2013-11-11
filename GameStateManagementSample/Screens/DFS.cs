using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace GameStateManagementSample
{
    class Dfs
    {
        private Board board = null;
        private Board finalBoard = null;
        private Stopwatch timer = null;

        public Dfs()
        {
            timer = new Stopwatch();
        }

        public bool Solve(Board board)
        {
            this.board = board;

            timer.Start();
            return SolveRec(0, 0);
        }

        public bool SolveRec(int r, int c)
        {
            //System.Console.WriteLine(r + " " + c);
            //board.Print();

            if (finalBoard != null)
                return true;

            if (board.IsComplete())
            {
                finalBoard = board;
                timer.Stop();
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

        public Board GetFinalBoard()
        {
            return finalBoard;
        }

        public void PrintTimeElapsed()
        {
            System.Console.WriteLine(timer.Elapsed.ToString());
        }
    }
}
