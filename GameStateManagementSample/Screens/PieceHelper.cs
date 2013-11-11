using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace GameStateManagementSample
{   
    public static class PieceHelper
    {
        ////////// DEFAULT CONFIGURATIONS ///////////
        private static List<ulong>[] PIECE_CONFIGURATIONS = null;
                
        private static void initPieceConfigs()
        {
            string str;
            using (System.Xml.XmlReader reader = System.Xml.XmlReader.Create("pentomino.xml"))
            {
                reader.MoveToContent();
                reader.ReadToFollowing("pentomino");
                str = reader.ReadInnerXml();
            }
            char[] delimiters = new char[] { '\r', '\n' };
            string[] lines = str.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<ulong, bool> checkHash = new Dictionary<ulong,bool>();


            PIECE_CONFIGURATIONS = new List<ulong>[12];

            for (int id = 0, it_line = 0; id < 12; ++id)
            {
                PIECE_CONFIGURATIONS[id] = new List<ulong>();
                ulong mask = 0;
                for (int i = 0, it = 0; i < 5; ++i, ++it_line)
                    for (int j = 0; j < 5; ++j, ++it)
                    {
                        //Debug.WriteLine(it_line + " " + it + " -- " + i + " " + j);
                        mask += (lines[it_line][j] == '1') ? ((ulong)1 << it) : 0;
                        //Debug.WriteLine(it_line + " " + it + " -- " + i + " " + j);
                    }

                PIECE_CONFIGURATIONS[id].Add(mask);

                ulong temp = mask;
                for (int i = 0; i < 4; ++i)
                {
                    if (!checkHash.ContainsKey(mask))
                    {
                        PIECE_CONFIGURATIONS[id].Add(mask);
                        checkHash.Add(mask,true);
                    }

                    if (i < 3) mask = RotateBitPiece(mask);
                }

                mask = FlipBitPiece(temp);

                for (int i = 0; i < 4; ++i)
                {
                    if (!checkHash.ContainsKey(mask))
                    {
                        PIECE_CONFIGURATIONS[id].Add(mask);
                        checkHash.Add(mask,true);
                    }

                    if (i < 3) mask = RotateBitPiece(mask);
                } 
            }
        }

        public static List<Piece> GetAllPieceConfig(int id)
        {
            if (PIECE_CONFIGURATIONS == null) initPieceConfigs();

            List<Piece> ret = new List<Piece>();

            for (int i = 0; i < PIECE_CONFIGURATIONS[id].Count; ++i)
                ret.Add(new Piece(id, i,0,0));

            return ret;
        }

        private static ulong ShiftLeftBit(ulong mask)
        {
            ulong _mask = 0;

            int skip = -1, check = 0;
            do
            {
                skip++; if (skip == 4) break;

                for (int i = 0; i < 5; ++i)
                    if (CheckBit(mask, i, skip)) { check = 1; break; }

            } while (check == 0);

            for (int i = 0; i < 5; ++i)
                for (int j = 0; j < 5; ++j)
                    _mask += (j + skip < 5) ? (CheckBit(mask,i,j+skip)?((ulong)1 << (i*5 + j)):(ulong)0) : (ulong)0;

            return _mask;
        }

        private static ulong shiftTopBit(ulong mask)
        {
            ulong _mask = 0;

            int skip = -1, check = 0;
            do
            {
                skip++;
                if (skip == 4) break;

                for (int i = 0; i < 5; ++i)
                    if (CheckBit(mask, skip, i)) { check = 1; break; }

            } while (check == 0);

            for (int i = 0; i < 5; ++i)
                for (int j = 0; j < 5; ++j)
                    _mask += (i + skip < 5) ? ((CheckBit(mask, i+skip, j))? ((ulong)1 << (i*5 + j)):(ulong)0): (ulong)0;

            return _mask;
        }

        public static ulong RotateBitPiece(ulong mask)
        {
            ulong _mask = 0;

            for (int i = 0; i < 5; ++i)
                for (int j = 0; j < 5; ++j)
                    _mask += (CheckBit(mask, i*5 + j))? ((ulong)1 << (j*5 + (5-i-1))) : 0;

            _mask = ShiftLeftBit(_mask);
            _mask = shiftTopBit(_mask);
            return _mask;
        }

        public static ulong FlipBitPiece(ulong mask)
        {
            ulong _mask = 0;

            for (int i = 0; i < 5; ++i)
                for (int j = 0; j < 5; ++j)
                    _mask += (CheckBit(mask, i*5 + j))? ((ulong)1 << (i*5 + (5-j-1))):0;

            _mask = ShiftLeftBit(_mask);
            _mask = shiftTopBit(_mask);
            return _mask;
        }

        public static bool CheckBit(ulong mask, int it)
        {
            return (mask & ((ulong)1 << it)) > 0;
        }

        public static bool CheckBit(ulong mask, int r, int c)
        {
            return (mask & ((ulong)1 << (r*5 + c))) > 0;
        }  

        // Print piece configuration
        public static void PrintConfig(ulong mask)
        {
            for (int i = 0; i < 5; ++i)
            {
                for (int j = 0; j < 5; ++j)
                    System.Console.Write(((CheckBit(mask, i*5+j))?"1":"0") + " ");
                System.Console.WriteLine();
            }
        }

        public static ulong GetPieceConfig(int id)
        {
            int id_config = 0;
            if (PIECE_CONFIGURATIONS == null) initPieceConfigs();

            return PIECE_CONFIGURATIONS[id][id_config];
        }

        public static ulong GetPieceConfig(int id, int id_config)
        {
            if (PIECE_CONFIGURATIONS == null) initPieceConfigs();

            return PIECE_CONFIGURATIONS[id][id_config];
        }

        public static int[][] GetGridPieceConfig(Piece piece)
        {
            int[][] ret = new int[5][];
            for (int i = 0; i < 5; ++i)
            {
                ret[i] = new int[5];
                for (int j = 0; j < 5; ++j)
                {
                    ret[i][j] = (CheckBit(piece.GetMask(), i, j)) ? 1 : 0;
                }
            }

            return ret;
        }
    }
}
