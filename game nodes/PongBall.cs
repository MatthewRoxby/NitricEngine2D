using OpenTK.Mathematics;
using NitricEngine2D.nodes;
using System.Text.Json;

namespace NitricEngine2D.game_nodes
{
    public class PongBall : Sprite2D
    {
        public Vector2 direction;
        public float start_speed = 20f;
        public float speed_multiplier = 1.2f;
        public float speed;
        Random rand = new Random();

        int bound_x = 79, bound_y = 44;

        int radius = 3;

        PongPaddle paddle_l;
        PongPaddle paddle_r;

        PongScore score;
        
        public PongBall(JsonElement data) : base(data) { }

        public override void Begin()
        {
            
            paddle_l = GameManager.currentRoot.GetNode("PaddleL") as PongPaddle;
            paddle_r = GameManager.currentRoot.GetNode("PaddleR") as PongPaddle;
            score = GameManager.currentRoot.GetNode("Score") as PongScore;
            direction = new Vector2(rand.Next(-1, 2), rand.Next(-1, 2));
            if(direction.X == 0) direction.X = 1;
            if(direction.Y == 0) direction.Y = 1;
            speed = start_speed;

            base.Begin();
        }

        void paddleCollision(PongPaddle p, int dir)
        {
            Vector2 ppos = p.position;
            Vector2 ps = p.half_size + (Vector2.One * radius);

            if(position.X < ppos.X + ps.X && position.X > ppos.X - ps.X && position.Y < ppos.Y + ps.Y && position.Y > ppos.Y - ps.Y)
            {
                position.X = ppos.X + (ps.X * dir);
                direction.X = dir;
                speed *= speed_multiplier;
            }
        }

        public override void Update(float deltaTime)
        {
            position += direction * speed * deltaTime;

            if(position.X > bound_x - radius)
            {
                position.X = 0;
                position.Y = 0;
                direction = new Vector2(1, rand.Next(-1, 1));
                if(direction.Y == 0) direction.Y = 1;
                speed = start_speed;
                score.IncreaseScore(0, this);
            }

            if (position.X < -bound_x + radius)
            {
                position.X = 0;
                position.Y = 0;
                direction = new Vector2(-1, rand.Next(-1, 1));
                if (direction.Y == 0) direction.Y = 1;
                speed = start_speed;
                score.IncreaseScore(1, this);
            }

            if (position.Y > bound_y - radius)
            {
                position.Y = bound_y - radius;
                direction.Y *= -1;
            }

            if (position.Y < -bound_y + radius)
            {
                position.Y = -bound_y + radius;
                direction.Y *= -1;
            }

            //paddle collision
            paddleCollision(paddle_r, -1);
            paddleCollision(paddle_l, 1);
            

            base.Update(deltaTime);
        }
    }
}
