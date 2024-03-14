using OpenTK.Mathematics;
using NitricEngine2D.nodes;
using System.Text.Json;

namespace NitricEngine2D.game_nodes
{
    /// <summary>
    /// a game class that defines a score counter for pong
    /// </summary>
    internal class PongScore : Node2D
    {
        Sprite2D sprite_l, sprite_r;

        int score_l = 0, score_r = 0;

        public PongScore(JsonElement data) : base(data) { }

        public override void Begin()
        {
            base.Begin();

            sprite_l = GetNode("ScoreL") as Sprite2D;
            sprite_r = GetNode("ScoreR") as Sprite2D;
        }

        public void IncreaseScore(int player, PongBall ballRef)
        {
            if(player == 0)
            {
                score_l++;
                if (score_l == 10)
                {
                    sprite_l.frame = 10;
                    sprite_r.frame = 13;
                    ballRef.position = Vector2.Zero;
                    ballRef.speed = 0;
                }
                else
                {
                    sprite_l.frame = score_l;
                }
                
            }
            else
            {
                score_r++;
                if (score_r == 10)
                {
                    sprite_r.frame = 12;
                    sprite_l.frame = 11;
                    ballRef.position = Vector2.Zero;
                    ballRef.speed = 0;
                }
                else
                {
                    sprite_r.frame = score_r;
                }
            }
        }
    }
}
