#region wbanie

// using Microsoft.Xna.Framework;
// using Microsoft.Xna.Framework.Graphics;
// using Microsoft.Xna.Framework.Input;
//
// namespace test3
// {
//     public class Game1 : Game
//     {
//         private GraphicsDeviceManager _graphics;
//         private SpriteBatch _spriteBatch;
//         private Vector2 _circlePosition;
//         private Vector2 _circleVelocity;
//
//         public Game1()
//         {
//             _graphics = new GraphicsDeviceManager(this);
//             Content.RootDirectory = "Content";
//             IsMouseVisible = true;
//         }
//
//         protected override void Initialize()
//         {
//             // Inicjalizacja pozycji i prędkości
//             _circlePosition = new Vector2(100, 100);
//             _circleVelocity = new Vector2(2, 2);
//             base.Initialize();
//         }
//
//         protected override void LoadContent()
//         {
//             _spriteBatch = new SpriteBatch(GraphicsDevice);
//         }
//
//         protected override void Update(GameTime gameTime)
//         {
//             if (Keyboard.GetState().IsKeyDown(Keys.Escape))
//                 Exit();
//
//             // Animacja kropki
//             _circlePosition += _circleVelocity;
//
//             // Odbicie od krawędzi okna
//             if (_circlePosition.X < 0 || _circlePosition.X > _graphics.PreferredBackBufferWidth)
//                 _circleVelocity.X *= -1;
//             if (_circlePosition.Y < 0 || _circlePosition.Y > _graphics.PreferredBackBufferHeight)
//                 _circleVelocity.Y *= -1;
//
//             base.Update(gameTime);
//         }
//
//         protected override void Draw(GameTime gameTime)
//         {
//             GraphicsDevice.Clear(Color.CornflowerBlue);
//
//             _spriteBatch.Begin();
//             _spriteBatch.DrawCircle(_circlePosition, 10, Color.Red); // Funkcja pomocnicza dla kropki
//             _spriteBatch.End();
//
//             base.Draw(gameTime);
//         }
//     }
//
//     public static class Extensions
//     {
//         public static void DrawCircle(this SpriteBatch spriteBatch, Vector2 position, float radius, Color color)
//         {
//             var texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
//             texture.SetData(new[] { color });
//             spriteBatch.Draw(texture, new Rectangle((int)(position.X - radius), (int)(position.Y - radius), (int)(radius * 2), (int)(radius * 2)), color);
//         }
//     }
// }


// using Microsoft.Xna.Framework;
// using Microsoft.Xna.Framework.Graphics;
// using Microsoft.Xna.Framework.Input;
// using System;
// using System.Collections.Generic;
// using MonoGame.Extended;
//
// namespace test3
// {
//     public class Game1 : Game
//     {
//         private GraphicsDeviceManager _graphics;
//         private SpriteBatch _spriteBatch;
//
//         private const int GridSize = 20;
//         private const int CellSize = 25;
//         private const int GridWidth = 20;
//         private const int GridHeight = 20;
//
//         private float[,] pheromones;
//         private bool[,] obstacles;
//         private List<Ant> ants;
//
//         private Point foodPosition = new Point(GridWidth - 2, GridHeight - 2);
//         private Random random = new Random();
//
//         public Game1()
//         {
//             _graphics = new GraphicsDeviceManager(this);
//             Content.RootDirectory = "Content";
//             IsMouseVisible = true;
//         }
//
//         protected override void Initialize()
//         {
//             pheromones = new float[GridWidth, GridHeight];
//             obstacles = new bool[GridWidth, GridHeight];
//             ants = new List<Ant>();
//
//             // Initialize ants
//             for (int i = 0; i < 10; i++)
//             {
//                 ants.Add(new Ant(new Point(1, 1)));
//             }
//
//             base.Initialize();
//         }
//
//         protected override void LoadContent()
//         {
//             _spriteBatch = new SpriteBatch(GraphicsDevice);
//         }
//
//         protected override void Update(GameTime gameTime)
//         {
//             if (Keyboard.GetState().IsKeyDown(Keys.Escape))
//                 Exit();
//
//             // Add obstacle on mouse click
//             if (Mouse.GetState().LeftButton == ButtonState.Pressed)
//             {
//                 var mouse = Mouse.GetState();
//                 int x = mouse.X / CellSize;
//                 int y = mouse.Y / CellSize;
//                 if (x >= 0 && x < GridWidth && y >= 0 && y < GridHeight && !(x == foodPosition.X && y == foodPosition.Y))
//                 {
//                     obstacles[x, y] = true;
//                 }
//             }
//
//             // Update ants
//             foreach (var ant in ants)
//             {
//                 ant.Move(pheromones, obstacles, foodPosition, random);
//
//                 // Drop pheromones
//                 pheromones[ant.Position.X, ant.Position.Y] += 0.5f;
//             }
//
//             // Evaporate pheromones
//             for (int x = 0; x < GridWidth; x++)
//             {
//                 for (int y = 0; y < GridHeight; y++)
//                 {
//                     pheromones[x, y] *= 0.99f; // Evaporation
//                 }
//             }
//
//             base.Update(gameTime);
//         }
//
//         protected override void Draw(GameTime gameTime)
//         {
//             GraphicsDevice.Clear(Color.CornflowerBlue);
//             _spriteBatch.Begin();
//
//             // Draw grid
//             for (int x = 0; x < GridWidth; x++)
//             {
//                 for (int y = 0; y < GridHeight; y++)
//                 {
//                     var rect = new Rectangle(x * CellSize, y * CellSize, CellSize, CellSize);
//
//                     // Draw pheromones
//                     float pheromoneLevel = MathHelper.Clamp(pheromones[x, y], 0, 1);
//                     Color pheromoneColor = new Color(1.0f, 1.0f - pheromoneLevel, 1.0f - pheromoneLevel);
//                     _spriteBatch.FillRectangle(rect, pheromoneColor);
//
//                     // Draw obstacles
//                     if (obstacles[x, y])
//                     {
//                         _spriteBatch.FillRectangle(rect, Color.Black);
//                     }
//
//                     // Draw grid lines
//                     _spriteBatch.DrawRectangle(rect, Color.Gray);
//                 }
//             }
//
//             // Draw food
//             _spriteBatch.FillRectangle(new Rectangle(foodPosition.X * CellSize, foodPosition.Y * CellSize, CellSize, CellSize), Color.Green);
//
//             // Draw ants
//             foreach (var ant in ants)
//             {
//                 var antRect = new Rectangle(ant.Position.X * CellSize + CellSize / 4, ant.Position.Y * CellSize + CellSize / 4, CellSize / 2, CellSize / 2);
//                 _spriteBatch.FillRectangle(antRect, Color.Red);
//             }
//
//             _spriteBatch.End();
//             base.Draw(gameTime);
//         }
//     }
//
//     public class Ant
//     {
//         public Point Position { get; private set; }
//         private List<Point> path;
//
//         public Ant(Point startPosition)
//         {
//             Position = startPosition;
//             path = new List<Point> { startPosition };
//         }
//
//         public void Move(float[,] pheromones, bool[,] obstacles, Point food, Random random)
//         {
//             List<Point> neighbors = GetNeighbors(Position, pheromones, obstacles);
//
//             if (neighbors.Count > 0)
//             {
//                 // Weighted random selection based on pheromone levels
//                 float[] weights = new float[neighbors.Count];
//                 for (int i = 0; i < neighbors.Count; i++)
//                 {
//                     weights[i] = pheromones[neighbors[i].X, neighbors[i].Y] + 0.1f; // Add small value to avoid zero weights
//                 }
//
//                 Position = SelectByWeight(neighbors, weights, random);
//
//                 path.Add(Position);
//
//                 if (Position == food)
//                 {
//                     // Reinforce path pheromones
//                     foreach (var step in path)
//                     {
//                         pheromones[step.X, step.Y] += 1.0f;
//                     }
//
//                     path.Clear();
//                     path.Add(Position);
//                 }
//             }
//         }
//
//         private List<Point> GetNeighbors(Point position, float[,] pheromones, bool[,] obstacles)
//         {
//             var neighbors = new List<Point>();
//
//             foreach (var offset in new[] { new Point(0, -1), new Point(0, 1), new Point(-1, 0), new Point(1, 0) })
//             {
//                 var neighbor = new Point(position.X + offset.X, position.Y + offset.Y);
//
//                 if (neighbor.X >= 0 && neighbor.X < pheromones.GetLength(0) &&
//                     neighbor.Y >= 0 && neighbor.Y < pheromones.GetLength(1) &&
//                     !obstacles[neighbor.X, neighbor.Y])
//                 {
//                     neighbors.Add(neighbor);
//                 }
//             }
//
//             return neighbors;
//         }
//
//         private Point SelectByWeight(List<Point> options, float[] weights, Random random)
//         {
//             float total = 0;
//             foreach (var weight in weights) total += weight;
//
//             float choice = (float)(random.NextDouble() * total);
//
//             for (int i = 0; i < options.Count; i++)
//             {
//                 if (choice < weights[i])
//                     return options[i];
//                 choice -= weights[i];
//             }
//
//             return options[0];
//         }
//     }
// }
#endregion

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using MonoGame.Extended;

namespace test3
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private const int GridSize = 20;
        private const int CellSize = 25;
        private const int GridWidth = 20;
        private const int GridHeight = 20;

        private float[,] homePheromones; // Feromony prowadzące do gniazda
        private float[,] foodPheromones; // Feromony prowadzące do jedzenia
        private bool[,] obstacles;
        private List<Ant> ants;

        private Point nestPosition = new Point(1, 1);
        private Point foodPosition = new Point(GridWidth - 2, GridHeight - 2);
        private Random random = new Random();

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
            // Ustaw rozmiar okna
            _graphics.PreferredBackBufferWidth = GridWidth * CellSize;
            _graphics.PreferredBackBufferHeight = GridHeight * CellSize;
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            homePheromones = new float[GridWidth, GridHeight];
            foodPheromones = new float[GridWidth, GridHeight];
            obstacles = new bool[GridWidth, GridHeight];
            ants = new List<Ant>();

            // Initialize ants
            for (int i = 0; i < 20; i++)
            {
                ants.Add(new Ant(nestPosition));
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Add obstacle on mouse click
            var mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                int x = mouseState.X / CellSize;
                int y = mouseState.Y / CellSize;
                if (x >= 0 && x < GridWidth && y >= 0 && y < GridHeight && 
                    !(x == foodPosition.X && y == foodPosition.Y) && 
                    !(x == nestPosition.X && y == nestPosition.Y))
                {
                    obstacles[x, y] = true;
                }
            }

            // Update ants
            foreach (var ant in ants)
            {
                ant.Move(foodPheromones, homePheromones, obstacles, foodPosition, nestPosition, random);
            }

            // Evaporate pheromones
            for (int x = 0; x < GridWidth; x++)
            {
                for (int y = 0; y < GridHeight; y++)
                {
                    foodPheromones[x, y] *= 0.995f; // Wolniejsze parowanie
                    homePheromones[x, y] *= 0.995f;
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();

            // Draw grid
            for (int x = 0; x < GridWidth; x++)
            {
                for (int y = 0; y < GridHeight; y++)
                {
                    var rect = new Rectangle(x * CellSize, y * CellSize, CellSize, CellSize);

                    // Draw pheromones
                    float foodPhLevel = MathHelper.Clamp(foodPheromones[x, y], 0, 1);
                    float homePhLevel = MathHelper.Clamp(homePheromones[x, y], 0, 1);
                    Color pheromoneColor = new Color(
                        1.0f - homePhLevel, 
                        1.0f, 
                        1.0f - foodPhLevel
                    );
                    _spriteBatch.FillRectangle(rect, pheromoneColor);

                    // Draw obstacles
                    if (obstacles[x, y])
                    {
                        _spriteBatch.FillRectangle(rect, Color.Black);
                    }

                    // Draw grid lines
                    _spriteBatch.DrawRectangle(rect, Color.Gray * 0.5f);
                }
            }

            // Draw nest
            _spriteBatch.FillRectangle(
                new Rectangle(nestPosition.X * CellSize, nestPosition.Y * CellSize, CellSize, CellSize), 
                Color.Brown);

            // Draw food
            _spriteBatch.FillRectangle(
                new Rectangle(foodPosition.X * CellSize, foodPosition.Y * CellSize, CellSize, CellSize), 
                Color.Green);

            // Draw ants
            foreach (var ant in ants)
            {
                var antRect = new Rectangle(
                    ant.Position.X * CellSize + CellSize / 4, 
                    ant.Position.Y * CellSize + CellSize / 4, 
                    CellSize / 2, CellSize / 2);
                _spriteBatch.FillRectangle(antRect, ant.HasFood ? Color.Yellow : Color.Red);
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }

    public class Ant
    {
        public Point Position { get; private set; }
        public bool HasFood { get; private set; }
        private const float PheromoneStrength = 1.0f;
        private const float ExplorationRate = 0.1f;

        public Ant(Point startPosition)
        {
            Position = startPosition;
            HasFood = false;
        }

        public void Move(float[,] foodPheromones, float[,] homePheromones, bool[,] obstacles, 
                        Point food, Point nest, Random random)
        {
            if (Position == food && !HasFood)
            {
                HasFood = true;
                return;
            }
            
            if (Position == nest && HasFood)
            {
                HasFood = false;
                return;
            }

            List<Point> neighbors = GetNeighbors(Position, obstacles);
            
            if (neighbors.Count > 0)
            {
                Point target = HasFood ? nest : food;
                float[] weights = new float[neighbors.Count];

                for (int i = 0; i < neighbors.Count; i++)
                {
                    Point neighbor = neighbors[i];
                    float distance = Vector2.Distance(
                        new Vector2(neighbor.X, neighbor.Y), 
                        new Vector2(target.X, target.Y));
                    
                    // Używamy odpowiednich feromonów w zależności od stanu mrówki
                    float pheromoneLevel = HasFood ? 
                        homePheromones[neighbor.X, neighbor.Y] : 
                        foodPheromones[neighbor.X, neighbor.Y];

                    // Waga oparta na feromonach i odległości do celu
                    weights[i] = (pheromoneLevel + 0.1f) * (1.0f / (distance + 1));
                    
                    // Dodaj losowość dla eksploracji
                    if (random.NextDouble() < ExplorationRate)
                    {
                        weights[i] += (float)random.NextDouble();
                    }
                }

                Point oldPosition = Position;
                Position = SelectByWeight(neighbors, weights, random);

                // Zostawiamy ślad feromonów
                if (HasFood)
                {
                    foodPheromones[oldPosition.X, oldPosition.Y] += PheromoneStrength;
                }
                else
                {
                    homePheromones[oldPosition.X, oldPosition.Y] += PheromoneStrength;
                }
            }
        }

        private List<Point> GetNeighbors(Point position, bool[,] obstacles)
        {
            var neighbors = new List<Point>();
            var directions = new[] 
            { 
                new Point(0, -1), new Point(0, 1), new Point(-1, 0), Point.Zero, new Point(1, 0),
                new Point(-1, -1), new Point(1, -1), new Point(-1, 1), new Point(1, 1)
            };

            foreach (var offset in directions)
            {
                var neighbor = new Point(position.X + offset.X, position.Y + offset.Y);

                if (neighbor.X >= 0 && neighbor.X < obstacles.GetLength(0) &&
                    neighbor.Y >= 0 && neighbor.Y < obstacles.GetLength(1) &&
                    !obstacles[neighbor.X, neighbor.Y])
                {
                    neighbors.Add(neighbor);
                }
            }

            return neighbors;
        }

        private Point SelectByWeight(List<Point> options, float[] weights, Random random)
        {
            float total = 0;
            foreach (var weight in weights) total += weight;

            float choice = (float)(random.NextDouble() * total);
            float sum = 0;

            for (int i = 0; i < options.Count; i++)
            {
                sum += weights[i];
                if (choice <= sum)
                    return options[i];
            }

            return options[0];
        }
    }
}


#region sranie
// using Microsoft.Xna.Framework;
// using Microsoft.Xna.Framework.Graphics;
// using Microsoft.Xna.Framework.Input;
// using System;
// using System.Collections.Generic;
// using MonoGame.Extended;
//
// namespace test3
// {
//     public enum GameState
//     {
//         Drawing,
//         Simulation
//     }
//
//     public class Game1 : Game
//     {
//         private GraphicsDeviceManager _graphics;
//         private SpriteBatch _spriteBatch;
//         private SpriteFont _font; // Zmieniamy na pole instancji
//
//         private const int GridSize = 20;
//         private const int CellSize = 25;
//         private const int GridWidth = 20;
//         private const int GridHeight = 20;
//
//         private float[,] homePheromones;
//         private float[,] foodPheromones;
//         private bool[,] obstacles;
//         private List<Ant> ants;
//
//         private Point nestPosition = new Point(1, 1);
//         private Point foodPosition = new Point(GridWidth - 2, GridHeight - 2);
//         private Random random = new Random();
//         
//         private GameState currentState = GameState.Drawing;
//         private KeyboardState previousKeyboardState;
//
//         public Game1()
//         {
//             _graphics = new GraphicsDeviceManager(this);
//             Content.RootDirectory = "Content";
//             IsMouseVisible = true;
//             
//             _graphics.PreferredBackBufferWidth = GridWidth * CellSize;
//             _graphics.PreferredBackBufferHeight = GridHeight * CellSize;
//             _graphics.ApplyChanges();
//         }
//
//         private void ResetSimulation()
//         {
//             homePheromones = new float[GridWidth, GridHeight];
//             foodPheromones = new float[GridWidth, GridHeight];
//             obstacles = new bool[GridWidth, GridHeight];
//             ants = new List<Ant>();
//
//             // Initialize ants (but they won't move until simulation starts)
//             for (int i = 0; i < 20; i++)
//             {
//                 ants.Add(new Ant(nestPosition));
//             }
//
//             currentState = GameState.Drawing;
//         }
//
//         protected override void Initialize()
//         {
//             ResetSimulation();
//             
//             // Tworzymy podstawową czcionkę
//             // var texture = new Texture2D(GraphicsDevice, 1, 1);
//             // texture.SetData(new[] { Color.White });
//             // _font = SpriteFont.CreateFromBitmap(
//             //     GraphicsDevice,
//             //     texture,
//             //     32, // First character in the font sheet
//             //     8,  // Font size
//             //     14, // Line spacing
//             //     14  // Character spacing
//             // );
//             
//             base.Initialize();
//         }
//
//         protected override void LoadContent()
//         {
//             _spriteBatch = new SpriteBatch(GraphicsDevice);
//         }
//
//         protected override void Update(GameTime gameTime)
//         {
//             KeyboardState currentKeyboardState = Keyboard.GetState();
//
//             if (currentKeyboardState.IsKeyDown(Keys.Escape))
//                 Exit();
//
//             // Handle Reset (R key)
//             if (currentKeyboardState.IsKeyDown(Keys.R) && !previousKeyboardState.IsKeyDown(Keys.R))
//             {
//                 ResetSimulation();
//             }
//
//             // Handle Enter to start simulation
//             if (currentState == GameState.Drawing && 
//                 currentKeyboardState.IsKeyDown(Keys.Enter) && 
//                 !previousKeyboardState.IsKeyDown(Keys.Enter))
//             {
//                 currentState = GameState.Simulation;
//             }
//
//             // Handle mouse input only in Drawing state
//             if (currentState == GameState.Drawing)
//             {
//                 var mouseState = Mouse.GetState();
//                 if (mouseState.LeftButton == ButtonState.Pressed)
//                 {
//                     int x = mouseState.X / CellSize;
//                     int y = mouseState.Y / CellSize;
//                     if (x >= 0 && x < GridWidth && y >= 0 && y < GridHeight && 
//                         !(x == foodPosition.X && y == foodPosition.Y) && 
//                         !(x == nestPosition.X && y == nestPosition.Y))
//                     {
//                         obstacles[x, y] = true;
//                     }
//                 }
//                 // Right click to remove obstacles
//                 else if (mouseState.RightButton == ButtonState.Pressed)
//                 {
//                     int x = mouseState.X / CellSize;
//                     int y = mouseState.Y / CellSize;
//                     if (x >= 0 && x < GridWidth && y >= 0 && y < GridHeight)
//                     {
//                         obstacles[x, y] = false;
//                     }
//                 }
//             }
//             // Update ants only in Simulation state
//             else if (currentState == GameState.Simulation)
//             {
//                 foreach (var ant in ants)
//                 {
//                     ant.Move(foodPheromones, homePheromones, obstacles, foodPosition, nestPosition, random);
//                 }
//
//                 // Evaporate pheromones
//                 for (int x = 0; x < GridWidth; x++)
//                 {
//                     for (int y = 0; y < GridHeight; y++)
//                     {
//                         foodPheromones[x, y] *= 0.995f;
//                         homePheromones[x, y] *= 0.995f;
//                     }
//                 }
//             }
//
//             previousKeyboardState = currentKeyboardState;
//             base.Update(gameTime);
//         }
//
//         protected override void Draw(GameTime gameTime)
//         {
//             GraphicsDevice.Clear(Color.CornflowerBlue);
//             _spriteBatch.Begin();
//
//             // Draw grid
//             for (int x = 0; x < GridWidth; x++)
//             {
//                 for (int y = 0; y < GridHeight; y++)
//                 {
//                     var rect = new Rectangle(x * CellSize, y * CellSize, CellSize, CellSize);
//
//                     // Draw pheromones (only in Simulation state)
//                     if (currentState == GameState.Simulation)
//                     {
//                         float foodPhLevel = MathHelper.Clamp(foodPheromones[x, y], 0, 1);
//                         float homePhLevel = MathHelper.Clamp(homePheromones[x, y], 0, 1);
//                         Color pheromoneColor = new Color(
//                             1.0f - homePhLevel,
//                             1.0f,
//                             1.0f - foodPhLevel
//                         );
//                         _spriteBatch.FillRectangle(rect, pheromoneColor);
//                     }
//                     else
//                     {
//                         // In Drawing state, use a light background
//                         _spriteBatch.FillRectangle(rect, Color.LightGray);
//                     }
//
//                     // Draw obstacles
//                     if (obstacles[x, y])
//                     {
//                         _spriteBatch.FillRectangle(rect, Color.Black);
//                     }
//
//                     // Draw grid lines
//                     _spriteBatch.DrawRectangle(rect, Color.Gray * 0.5f);
//                 }
//             }
//
//             // Draw nest
//             _spriteBatch.FillRectangle(
//                 new Rectangle(nestPosition.X * CellSize, nestPosition.Y * CellSize, CellSize, CellSize),
//                 Color.Brown);
//
//             // Draw food
//             _spriteBatch.FillRectangle(
//                 new Rectangle(foodPosition.X * CellSize, foodPosition.Y * CellSize, CellSize, CellSize),
//                 Color.Green);
//
//             // Draw ants (only in Simulation state)
//             if (currentState == GameState.Simulation)
//             {
//                 foreach (var ant in ants)
//                 {
//                     var antRect = new Rectangle(
//                         ant.Position.X * CellSize + CellSize / 4,
//                         ant.Position.Y * CellSize + CellSize / 4,
//                         CellSize / 2, CellSize / 2);
//                     _spriteBatch.FillRectangle(antRect, ant.HasFood ? Color.Yellow : Color.Red);
//                 }
//             }
//
//             // Draw state information
//             string stateText = currentState == GameState.Drawing ? 
//                 "Drawing Mode (Press Enter to start)" : 
//                 "Simulation Mode (Press R to reset)";
//             
//             Vector2 textPosition = new Vector2(5, GraphicsDevice.Viewport.Height - 25);
//             Color backgroundColor = Color.White * 0.7f;
//             Vector2 textSize = _font.MeasureString(stateText);
//             
//             // Draw text background
//             _spriteBatch.FillRectangle(
//                 new Rectangle((int)textPosition.X, (int)textPosition.Y, 
//                             (int)textSize.X, (int)textSize.Y),
//                 backgroundColor);
//             
//             // Draw text
//             _spriteBatch.DrawString(
//                 _font,
//                 stateText,
//                 textPosition,
//                 Color.Black);
//
//             _spriteBatch.End();
//             base.Draw(gameTime);
//         }
//     }
//
//     // Klasa Ant pozostaje bez zmian
//     public class Ant
//     {
//         public Point Position { get; private set; }
//         public bool HasFood { get; private set; }
//         private const float PheromoneStrength = 1.0f;
//         private const float ExplorationRate = 0.1f;
//
//         public Ant(Point startPosition)
//         {
//             Position = startPosition;
//             HasFood = false;
//         }
//
//         public void Move(float[,] foodPheromones, float[,] homePheromones, bool[,] obstacles,
//                         Point food, Point nest, Random random)
//         {
//             if (Position == food && !HasFood)
//             {
//                 HasFood = true;
//                 return;
//             }
//
//             if (Position == nest && HasFood)
//             {
//                 HasFood = false;
//                 return;
//             }
//
//             List<Point> neighbors = GetNeighbors(Position, obstacles);
//
//             if (neighbors.Count > 0)
//             {
//                 Point target = HasFood ? nest : food;
//                 float[] weights = new float[neighbors.Count];
//
//                 for (int i = 0; i < neighbors.Count; i++)
//                 {
//                     Point neighbor = neighbors[i];
//                     float distance = Vector2.Distance(
//                         new Vector2(neighbor.X, neighbor.Y),
//                         new Vector2(target.X, target.Y));
//
//                     float pheromoneLevel = HasFood ?
//                         homePheromones[neighbor.X, neighbor.Y] :
//                         foodPheromones[neighbor.X, neighbor.Y];
//
//                     weights[i] = (pheromoneLevel + 0.1f) * (1.0f / (distance + 1));
//
//                     if (random.NextDouble() < ExplorationRate)
//                     {
//                         weights[i] += (float)random.NextDouble();
//                     }
//                 }
//
//                 Point oldPosition = Position;
//                 Position = SelectByWeight(neighbors, weights, random);
//
//                 if (HasFood)
//                 {
//                     foodPheromones[oldPosition.X, oldPosition.Y] += PheromoneStrength;
//                 }
//                 else
//                 {
//                     homePheromones[oldPosition.X, oldPosition.Y] += PheromoneStrength;
//                 }
//             }
//         }
//
//         private List<Point> GetNeighbors(Point position, bool[,] obstacles)
//         {
//             var neighbors = new List<Point>();
//             var directions = new[]
//             {
//                 new Point(0, -1), new Point(0, 1), new Point(-1, 0), Point.Zero, new Point(1, 0),
//                 new Point(-1, -1), new Point(1, -1), new Point(-1, 1), new Point(1, 1)
//             };
//
//             foreach (var offset in directions)
//             {
//                 var neighbor = new Point(position.X + offset.X, position.Y + offset.Y);
//
//                 if (neighbor.X >= 0 && neighbor.X < obstacles.GetLength(0) &&
//                     neighbor.Y >= 0 && neighbor.Y < obstacles.GetLength(1) &&
//                     !obstacles[neighbor.X, neighbor.Y])
//                 {
//                     neighbors.Add(neighbor);
//                 }
//             }
//
//             return neighbors;
//         }
//
//         private Point SelectByWeight(List<Point> options, float[] weights, Random random)
//         {
//             float total = 0;
//             foreach (var weight in weights) total += weight;
//
//             float choice = (float)(random.NextDouble() * total);
//             float sum = 0;
//
//             for (int i = 0; i < options.Count; i++)
//             {
//                 sum += weights[i];
//                 if (choice <= sum)
//                     return options[i];
//             }
//
//             return options[0];
//         }
//     }
// }

#endregion