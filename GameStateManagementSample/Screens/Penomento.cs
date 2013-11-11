using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Cupento.Screens
{
    class Pentomino
    {
        static List<List<List<int>>> mat_tipe;

        Texture2D texture;
        int tipe;
        int rotation;
        List<List<int>> mat;
        public Vector2 scr_position;

        private void LoadPentomino()
        {
            mat_tipe = new List<List<List<int>>>();
            List<List<int>> m = new List<List<int>> { 
                new List<int> {1},
                new List<int> {1},
                new List<int> {1},
                new List<int> {1},
                new List<int> {1}
            };
            mat_tipe.Add(m);
            m = new List<List<int>> { 
                new List<int> {0,1,1},
                new List<int> {1,1,0},
                new List<int> {0,1,0},
            };
            mat_tipe.Add(m);
            m = new List<List<int>> { 
                new List<int> {1,1,0},
                new List<int> {0,1,1},
                new List<int> {0,1,0},
            };
            mat_tipe.Add(m);
            m = new List<List<int>> { 
                new List<int> {0,1},
                new List<int> {0,1},
                new List<int> {0,1},
                new List<int> {1,1},
            };
            mat_tipe.Add(m);
            m = new List<List<int>> { 
                new List<int> {1,0},
                new List<int> {1,0},
                new List<int> {1,0},
                new List<int> {1,1},
            };
            mat_tipe.Add(m);
        }

        public Pentomino(int tipe, Vector2 position, ContentManager content)
        {
            if (mat_tipe == null){
                LoadPentomino();
            }
            mat = mat_tipe[tipe];
            this.tipe = tipe;
            scr_position = position;
        }

        public void RotateToRight()
        {
        }

    }
}
