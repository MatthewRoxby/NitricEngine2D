using OpenTK.Mathematics;
using System.Text.Json;
using OpenTK.Windowing.GraphicsLibraryFramework;
using NitricEngine2D.nodes;

namespace NitricEngine2D.game_nodes
{
    /// <summary>
    /// a game class that defines a pong paddle
    /// </summary>
    public class PongPaddle : Sprite2D
    {

        public float max_speed = 50f;

        public float acceleration = 10f;

        public float min_y = -36f;

        public float max_y = 36f;

        int player = -1;

        float speed = 0f;

        public Vector2 half_size = new Vector2(1, 8);

        public PongPaddle(JsonElement data) : base(data) 
        {
            this.player = Helper.JSONGetPropertyInt(data, "player", -1);
        }

        public override void Update(float deltaTime)
        {
            int d = 0;

            if(player == 0)
            {
                
                if (Input.IsKeyDown(Keys.W))
                {
                    d++;
                }

                if (Input.IsKeyDown(Keys.S))
                {
                    d--;
                }
            }
            else if(player == 1)
            {
                if (Input.IsKeyDown(Keys.Up))
                {
                    d++;
                }

                if (Input.IsKeyDown(Keys.Down))
                {
                    d--;
                }
            }

            speed = MathHelper.Lerp(speed, max_speed * d, acceleration * deltaTime);

            position.Y += speed * deltaTime;

            position.Y = MathHelper.Clamp(position.Y, min_y, max_y);

            base.Update(deltaTime);
        }
    }
}
