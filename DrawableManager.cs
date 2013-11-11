using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace GameStateManagement
{
    class DrawableManager
    {
        private ContentManager content;
        private Dictionary<String, Texture2D> kamus;

        public DrawableManager()
        {
        }

        public void Initiate(){
            kamus = new Dictionary<String, Texture2D>();
            content = new ContentManager(ScreenManager.Game.Services, "Content");
        }

        public Texture2D GetTexture(String name)
        {
            if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

            if (kamus.ContainsKey(name))
            {
                if (kamus[name] == null)
                {
                    kamus[name] = content.Load<
                }
            }
        }

    }
}
