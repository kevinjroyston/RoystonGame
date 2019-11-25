﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RoystonGame.TV.GameEngine.Rendering;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace RoystonGame.TV.GameEngine.Rendering
{
    public class UserDrawingObject : GameObject
    {
        public Texture2D Content { get; private set; }
        public Texture2D Background { get; private set; }
        public override Rectangle BoundingBox { get; set; } = new Rectangle(0, 0, 100, 100);
        private string UserDrawing { get; set; }
        public UserDrawingObject(string userDrawing) : base()
        {
            this.UserDrawing = userDrawing;

            // hacky fix.
            GameManager.RegisterGameObject(this);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (this.Content == null)
                return;
            spriteBatch.Draw(this.Background, this.BoundingBox, Color.White);
            spriteBatch.Draw(this.Content, this.BoundingBox, Color.White);
        }

        public override void UnloadContent()
        {
            // Empty
        }

        public override void Update(GameTime gameTime)
        {
            // Empty
        }

        public override void LoadContent(ContentManager content, GraphicsDevice graphics)
        {
            if ( this.UserDrawing != null)
            {
                var base64Data = Regex.Match(this.UserDrawing, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                var binData = Convert.FromBase64String(base64Data);
                MemoryStream stream = new MemoryStream(binData);
                this.Content = Texture2D.FromStream(graphics, stream);
            }
            this.Background = content.Load<Texture2D>("WhiteSquare");
        }
    }
}
