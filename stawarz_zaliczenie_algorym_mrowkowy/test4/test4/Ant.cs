// using System;
// using System.Collections.Generic;
// using Microsoft.Xna.Framework;
//
// namespace test4
// {
//     public class Ant
//     {
//         public Point Position { get; private set; }
//         public bool HasFood { get; private set; }
//
//         private bool[,] visited; // Tracks visited cells
//         private int gridWidth;
//         private int gridHeight;
//
//         public Ant(Point startPosition, int gridWidth, int gridHeight)
//         {
//             Position = startPosition;
//             HasFood = false;
//             this.gridWidth = gridWidth;
//             this.gridHeight = gridHeight;
//             visited = new bool[gridWidth, gridHeight];
//             visited[Position.X, Position.Y] = true; // Mark the starting position as visited
//         }
//
//         public void Move(Random random, bool[,] path)
//         {
//             List<Point> possibles = GetPossibleMoves(Position, path);
//
//             // If there are no valid moves, the ant stays in its current position
//             if (possibles.Count == 0)
//                 return;
//
//             // Select a random valid move
//             int randomIndex = random.Next(possibles.Count);
//             Point new_Position = possibles[randomIndex];
//
//             // Update visited status and position
//             Position = new_Position;
//             visited[Position.X, Position.Y] = true;
//         }
//
//         private List<Point> GetPossibleMoves(Point position, bool[,] path)
//         {
//             var neighbors = new List<Point>();
//             var directions = new[]
//             {
//                 new Point(0, -1), new Point(0, 1), // Up and Down
//                 new Point(-1, 0), new Point(1, 0), // Left and Right
//                 
//             };
//
//             foreach (var offset in directions)
//             {
//                 var neighbor = new Point(position.X + offset.X, position.Y + offset.Y);
//
//                 // Check boundaries, path accessibility, and visited status
//                 if (neighbor.X >= 0 && neighbor.X < gridWidth &&
//                     neighbor.Y >= 0 && neighbor.Y < gridHeight &&
//                     path[neighbor.X, neighbor.Y] && // Cell must be traversable
//                     !visited[neighbor.X, neighbor.Y]) // Cell must not have been visited
//                 {
//                     neighbors.Add(neighbor);
//                 }
//             }
//
//             return neighbors;
//         }
//     }
// }
