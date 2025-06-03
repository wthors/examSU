namespace Breakout.Managers;

using System.Collections.Generic;
using Breakout.Balls;
using Breakout.Blocks;
using Breakout.Points;
using Breakout.States;
using DIKUArcade.Physics;

public class CollisionManager {
    private readonly List<Ball> _balls;
    private readonly List<IBlock> _blocks;
    private readonly Player _player;
    private readonly PointTracker _pointTracker;

    public CollisionManager(List<Ball> balls, List<IBlock> blocks, Player player, PointTracker pointTracker) {
        _balls = balls;
        _blocks = blocks;
        _player = player;
        _pointTracker = pointTracker;
    }

    public void Update() {
        foreach (var ball in _balls) {
            // Reflect horizontally if hitting left or right wall
            if (ball.Shape.Position.X <= 0.0f ||
                ball.Shape.Position.X + ball.Shape.Extent.X >= 1.0f) {
                ball.ReflectHorizontal();
            }
            // Reflect vertically if hitting the ceiling
            if (ball.Shape.Position.Y + ball.Shape.Extent.Y >= 1.0f) {
                ball.ReflectVertical();
            }
        }

        // Player and ball collisions
        foreach (var ball in _balls) {
            if (CollisionDetection.Aabb(ball.Shape.AsDynamicShape(),
                                        _player.Shape.AsDynamicShape()).Collision) {
                ball.ReflectVertical();
            }
        }

        // Ball and block collisions
        foreach (var ball in _balls) {
            var ballShape = ball.Shape.AsDynamicShape();
            foreach (var block in _blocks) {
                if (!block.IsAlive)
                    continue;  // skip destroyed blocks
                var blockShape = block.Shape.AsDynamicShape();
                var collision = CollisionDetection.Aabb(ballShape, blockShape);
                if (collision.Collision) {
                    // Reflect the ball based on which side of the block was hit
                    if (collision.CollisionDir == CollisionDirection.CollisionDirLeft ||
                        collision.CollisionDir == CollisionDirection.CollisionDirRight) {
                        ball.ReflectHorizontal();
                    } else {
                        ball.ReflectVertical();
                    }
                    // Apply damage to the block and award points if destroyed
                    block.DecreaseHealth();
                    if (!block.IsAlive) {
                        _pointTracker.AddPoints(block.GetValue());
                    }
                    // Only handle the first collision per ball per frame
                    break;
                }
            }
        }
    }
}
