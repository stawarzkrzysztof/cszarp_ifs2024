using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using MonoGame.Extended;

public class Game2 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    
    private const int CellSize = 30;
    private const int GridWidth = 25;
    private const int GridHeight = 25;
    
    private float[,] foodPheromones;
    private float[,] homePheromones;
    private bool[,] obstacles;
    private List<Ant> ants;

    private Point nestPosition = new Point(1, GridHeight - 2);
    private Point foodPosition = new Point(GridWidth - 2, 1);
    private Random random = new Random();

    private Color nestColor = Color.Red;
    private Color foodColor = Color.LightGreen;
    private Color antColor = Color.Black;

    private bool antsRunning = false;
    private int antNumber = 5;
    
    private double elapsedTime = 0;
    private const float PheromoneDegradationRate = 0.995f; // Współczynnik wyparowywania feromonów
    private const float PheromoneDepositAmount = 0.1f; // Ilość zostawianych feromonów

    public Game2()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        
        _graphics.PreferredBackBufferWidth = GridWidth * CellSize;
        _graphics.PreferredBackBufferHeight = GridHeight * CellSize;
        _graphics.ApplyChanges();
    }

    protected override void Initialize()
    {
        obstacles = new bool[GridWidth, GridHeight];
        foodPheromones = new float[GridWidth, GridHeight];
        homePheromones = new float[GridWidth, GridHeight];
        ants = new List<Ant>();
        
        for (int i = 0; i < antNumber; i++)
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
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        
        if (Keyboard.GetState().IsKeyDown(Keys.Enter)) antsRunning = true;
        
        // Obsługa myszki dla przeszkód
        HandleMouseInput();
        
        elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;
        
        if (antsRunning && elapsedTime >= 0.25)
        {
            UpdatePheromones();
            UpdateAnts();
            elapsedTime = 0;
        }

        base.Update(gameTime);
    }

    private void HandleMouseInput()
    {
        var mouseState = Mouse.GetState();
        if (!antsRunning)
        {
            int x = mouseState.X / CellSize;
            int y = mouseState.Y / CellSize;
            
            if (IsValidCell(x, y) && !IsSpecialCell(new Point(x, y)))
            {
                if (mouseState.LeftButton == ButtonState.Pressed)
                    obstacles[x, y] = true;
                else if (mouseState.RightButton == ButtonState.Pressed)
                    obstacles[x, y] = false;
            }
        }
    }

    private bool IsValidCell(int x, int y)
    {
        return x >= 0 && x < GridWidth && y >= 0 && y < GridHeight;
    }

    private bool IsSpecialCell(Point position)
    {
        return (position == foodPosition || position == nestPosition);
    }

    private void UpdatePheromones()
    {
        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < GridHeight; y++)
            {
                foodPheromones[x, y] *= PheromoneDegradationRate;
                homePheromones[x, y] *= PheromoneDegradationRate;
            }
        }
    }

    private void UpdateAnts()
    {
        foreach (var ant in ants)
        {
            Point oldPosition = ant.Position;
            ant.Move(random, obstacles, foodPheromones, homePheromones, foodPosition, nestPosition);
            
            // Zostawianie feromonów
            if (ant.HasFood)
            {
                foodPheromones[oldPosition.X, oldPosition.Y] += PheromoneDepositAmount;
            }
            else
            {
                homePheromones[oldPosition.X, oldPosition.Y] += PheromoneDepositAmount;
            }

            // Sprawdzanie czy mrówka dotarła do celu
            if (!ant.HasFood && ant.Position == foodPosition)
            {
                ant.HasFood = true;
            }
            else if (ant.HasFood && ant.Position == nestPosition)
            {
                ant.HasFood = false;
            }
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.White);
        _spriteBatch.Begin();
        
        // Rysowanie siatki i feromonów
        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < GridHeight; y++)
            {
                var rect = new Rectangle(x * CellSize, y * CellSize, CellSize, CellSize);
                
                // Rysowanie feromonów
                float foodPhLevel = MathHelper.Clamp(foodPheromones[x, y], 0, 1);
                float homePhLevel = MathHelper.Clamp(homePheromones[x, y], 0, 1);
                Color pheromoneColor = new Color(
                    1.0f - homePhLevel, 
                    1.0f, 
                    1.0f - foodPhLevel
                );
                _spriteBatch.FillRectangle(rect, pheromoneColor);

                // Rysowanie specjalnych komórek i przeszkód
                if (x == nestPosition.X && y == nestPosition.Y) 
                    _spriteBatch.FillRectangle(rect, nestColor);
                else if (x == foodPosition.X && y == foodPosition.Y) 
                    _spriteBatch.FillRectangle(rect, foodColor);
                else if (obstacles[x, y]) 
                    _spriteBatch.FillRectangle(rect, Color.Black);
                
                _spriteBatch.DrawRectangle(rect, Color.Gray * 0.5f);
            }
        }
        
        // Rysowanie mrówek
        foreach (var ant in ants)
        {
            var antRect = new Rectangle(
                ant.Position.X * CellSize + CellSize / 4, 
                ant.Position.Y * CellSize + CellSize / 4, 
                CellSize / 2, CellSize / 2);
            _spriteBatch.FillRectangle(antRect, ant.HasFood ? Color.Yellow : antColor);
        }
        
        _spriteBatch.End();
        base.Draw(gameTime);
    }
}

public class Ant
{
    public Point Position { get; private set; }
    public bool HasFood { get; set; }
    
    private const float PheromoneWeight = 1.0f;
    private const float RandomWeight = 0.1f;
    
    public Ant(Point startPosition)
    {
        Position = startPosition;
        HasFood = false;
    }
    
    public void Move(Random random, bool[,] obstacles, float[,] foodPheromones, float[,] homePheromones, 
        Point foodPosition, Point nestPosition)
    {
        List<(Point position, float weight)> neighbors = GetWeightedNeighbors(
            Position, 
            obstacles, 
            foodPheromones, 
            homePheromones,
            HasFood ? nestPosition : foodPosition,
            HasFood
        );

        if (neighbors.Count == 0) return;

        // Wybór następnej pozycji na podstawie wag
        float totalWeight = 0;
        foreach (var neighbor in neighbors)
        {
            totalWeight += neighbor.weight;
        }

        float randomValue = (float)random.NextDouble() * totalWeight;
        float currentSum = 0;

        foreach (var neighbor in neighbors)
        {
            currentSum += neighbor.weight;
            if (currentSum >= randomValue)
            {
                Position = neighbor.position;
                break;
            }
        }
    }

    private List<(Point position, float weight)> GetWeightedNeighbors(
        Point position, 
        bool[,] obstacles, 
        float[,] foodPheromones, 
        float[,] homePheromones,
        Point target,
        bool hasFood)
    {
        var neighbors = new List<(Point position, float weight)>();
        var directions = new[] {
            new Point(0, -1), new Point(0, 1), new Point(-1, 0), new Point(1, 0),
            new Point(-1, -1), new Point(1, -1), new Point(-1, 1), new Point(1, 1)
        };

        foreach (var offset in directions)
        {
            var neighbor = new Point(position.X + offset.X, position.Y + offset.Y);
            if (IsValidMove(neighbor, obstacles))
            {
                float weight = CalculateWeight(neighbor, target, hasFood ? homePheromones : foodPheromones);
                neighbors.Add((neighbor, weight));
            }
        }

        return neighbors;
    }

    private bool IsValidMove(Point position, bool[,] obstacles)
    {
        return position.X >= 0 && position.X < obstacles.GetLength(0) &&
               position.Y >= 0 && position.Y < obstacles.GetLength(1) &&
               !obstacles[position.X, position.Y];
    }

    private float CalculateWeight(Point position, Point target, float[,] pheromones)
    {
        // Składnik feromonowy
        float pheromoneInfluence = pheromones[position.X, position.Y] * PheromoneWeight;
        
        // Składnik heurystyczny - odległość do celu
        float distance = Vector2.Distance(
            new Vector2(position.X, position.Y), 
            new Vector2(target.X, target.Y)
        );
        float heuristicInfluence = 1.0f / (distance + 1.0f);
        
        // Dodanie losowości
        float randomInfluence = RandomWeight;
        
        return pheromoneInfluence + heuristicInfluence + randomInfluence;
    }
}