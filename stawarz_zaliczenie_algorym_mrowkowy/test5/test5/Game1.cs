using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using MonoGame.Extended;
using System.Collections.Generic;
using System.Linq;

namespace test5;

public class Game1 : Game {
    #region Fields

    // monogame variables
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    // graphical const
    private const int WindowHeight = 600;
    private const int WindowWidth = 600;
    private const int nodeTextureRadius = 10;

    // model constants
    private const int nodeCount = 30; // max 100 cuz slow
    private const int antCount = nodeCount;
    private const double alpha = 2.0; // distance power
    private const double beta = 5.0; // pheromone power
    private const double startingPheromone = 1.0;
    private const double pheromoneEvaporationRate = 0.1;
    private double Q; // will be calculated based on average distance

    // code variables
    private Random random = new Random();
    private List<Point> nodes;
    private Ant[] ants;
    private double elapsedTime;
    private const double antMovementFramerate = 30; // FPS
    private bool animationRunning;
    private Texture2D _lineTexture;
    private float lineWidth = 1f; // work with line width 
    private double[,] distanceMatrix = new double[nodeCount, nodeCount];
    private double[,] pheromoneMatrix = new double[nodeCount, nodeCount];
    private int iteration = 0;
    private int bestTourIteration = 0;

    // CUSTOM FLAGS
    private bool wannaDrawNextPath = true;
    private bool wannaDropTablesOnStart = false; // <--------------------------------------------------------------------

    // tracking best solution
    private double bestTourLength = double.MaxValue;
    private List<Point> bestTour = new List<Point>();

    // colors
    private Color defaultNodeColor = Color.White;
    private Color antNodeColor = Color.Red;
    private Color backgroundColor = Color.Black;
    private Color currentFillColor;
    private Color pathColor = Color.Gray;
    private Color bestPathColor = Color.LimeGreen;

    // db
    private DatabaseManager _dbManager;

    #endregion

    public Game1() {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _graphics.PreferredBackBufferWidth = WindowWidth;
        _graphics.PreferredBackBufferHeight = WindowHeight;
        _graphics.ApplyChanges();

        // Initialize database manager
        string dbPath = "aco_results.db";
        _dbManager = new DatabaseManager(dbPath);
    }

    // ---------------------------------INITIALIZE---------------------------------------------------------------------
    protected override void Initialize() {
        if (wannaDropTablesOnStart) _dbManager.DropAllTables();
        // create random-positioned nodes
        nodes = new List<Point>(nodeCount);
        for (int i = 0; i < nodeCount; i++) {
            Point point2add = new Point(
                random.Next(0, WindowWidth - nodeTextureRadius),
                random.Next(0, WindowHeight - nodeTextureRadius));
            nodes.Add(point2add);
        }

        // Calculate Q based on average distance
        double totalDistance = 0;
        int count = 0;
        for (int i = 0; i < nodeCount; i++) {
            for (int j = i + 1; j < nodeCount; j++) {
                double distance = Math.Sqrt(Math.Pow(nodes[i].X - nodes[j].X, 2) +
                                            Math.Pow(nodes[i].Y - nodes[j].Y, 2));
                totalDistance += distance;
                count++;
            }
        }

        Q = nodeCount * (totalDistance / count); // Q is proportional to average tour length

        // start db
        _dbManager.StartNewRun(nodeCount, antCount, alpha, beta, pheromoneEvaporationRate, startingPheromone, Q);

        // save node distribution to db
        _dbManager.SaveNodeDistribution(nodes);

        // initialize distance matrix with inverse distances
        for (int i = 0; i < nodeCount; i++) {
            for (int j = 0; j < nodeCount; j++) {
                if (i != j) {
                    double distance = Math.Sqrt(Math.Pow(nodes[i].X - nodes[j].X, 2) +
                                                Math.Pow(nodes[i].Y - nodes[j].Y, 2));
                    distanceMatrix[i, j] = 1.0 / Math.Pow(distance, alpha); // Inverse distance
                }
                else {
                    distanceMatrix[i, j] = 0.0;
                }

                pheromoneMatrix[i, j] = startingPheromone;
            }
        }

        // create ants on nodes
        ants = new Ant[antCount];
        for (int i = 0; i < antCount; i++) {
            ants[i] = new Ant(i, nodes[i]);
        }

        base.Initialize();
    }

    protected override void LoadContent() {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _lineTexture = new Texture2D(GraphicsDevice, 1, 1);
        _lineTexture.SetData(new[] { Color.White });
    }

    // -------------------------------------UPDATE---------------------------------------------------------------------
    protected override void Update(GameTime gameTime) {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape)) {
            _dbManager.SaveBestPath(bestTourIteration, bestTourLength, bestTour, nodes);
            Exit();
        }

        // start the animation with Enter or Space
        if (Keyboard.GetState().IsKeyDown(Keys.Enter)) animationRunning = true;
        if (Keyboard.GetState().IsKeyDown(Keys.Space)) animationRunning = false;

        // move ants with certain framerate
        elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;
        if (animationRunning && elapsedTime >= 1 / antMovementFramerate) {
            foreach (Ant ant in ants) {
                if (ant.visitedNodes.Count < nodes.Count) {
                    int nodeIndex = nodes.IndexOf(ant.antPosition);
                    int indexOfNode2MoveTo = chooseNode2MoveTo(ant, nodeIndex);
                    ant.move(nodes[indexOfNode2MoveTo]);
                }
                else {
                    updatePheromones(ant);
                    // go back to the starting node 
                    Point endNode = ant.visitedNodes[0];
                    ant.move(endNode);
                    ant.visitedNodes = new List<Point>();
                    ant.visitedNodes.Add(endNode);
                }
            }

            elapsedTime = 0;
        }

        base.Update(gameTime);
    }

    // ----------------------------------------DRAW---------------------------------------------------------------------
    protected override void Draw(GameTime gameTime) {
        GraphicsDevice.Clear(backgroundColor);
        _spriteBatch.Begin(SpriteSortMode.Immediate,
            blendState: BlendState.AlphaBlend,
            samplerState: SamplerState.PointClamp,
            rasterizerState: new RasterizerState { ScissorTestEnable = true });

        // draw best tour if available
        if (bestTour.Count > 0 && wannaDrawNextPath) {
            for (int i = 1; i < bestTour.Count; i++) {
                Vector2 point1 = new Vector2(bestTour[i - 1].X, bestTour[i - 1].Y);
                Vector2 point2 = new Vector2(bestTour[i].X, bestTour[i].Y);
                DrawLine(_spriteBatch, point1, point2, bestPathColor, lineWidth * 6);
            }

            // draw line back to start
            if (bestTour.Count == nodeCount && wannaDrawNextPath) {
                Vector2 last = new Vector2(bestTour.Last().X, bestTour.Last().Y);
                Vector2 first = new Vector2(bestTour.First().X, bestTour.First().Y);
                DrawLine(_spriteBatch, last, first, bestPathColor, lineWidth * 6);
            }
        }

        // draw current ant paths and nodes
        for (int i = 0; i < nodes.Count; i++) {
            Point node = nodes[i];
            var nodeTexture = new Rectangle(node.X, node.Y, nodeTextureRadius, nodeTextureRadius);

            // color ants and nodes
            currentFillColor = defaultNodeColor;
            for (int j = 0; j < ants.Length; j++) {
                Ant ant = ants[j];
                if (node.X == ant.antPosition.X &&
                    node.Y == ant.antPosition.Y) {
                    currentFillColor = antNodeColor;
                    // currentFillColor = new Color(random.Next(50, 255), random.Next(50, 255), random.Next(50, 255));
                }

                _spriteBatch.FillRectangle(nodeTexture, currentFillColor);

                // draw current ant paths
                if (ant.visitedNodes.Count >= 2) {
                    for (int k = 1; k < ant.visitedNodes.Count; k++) {
                        Vector2 point1 = new Vector2(
                            ant.visitedNodes[k - 1].X,
                            ant.visitedNodes[k - 1].Y);
                        Vector2 point2 = new Vector2(
                            ant.visitedNodes[k].X,
                            ant.visitedNodes[k].Y);
                        DrawLine(_spriteBatch, point1, point2, pathColor, lineWidth);
                    }
                }
            }
        }

        _spriteBatch.End();
        base.Draw(gameTime);
    }

    private void DrawLine(SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float width) {
        float distance = Vector2.Distance(point1, point2);
        float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
        spriteBatch.Draw(_lineTexture,
            point1,
            null,
            color,
            angle,
            Vector2.Zero,
            new Vector2(distance, width),
            SpriteEffects.None,
            0);
    }

    protected override void UnloadContent() {
        _lineTexture.Dispose();
        base.UnloadContent();
    }

    // -----------------------------------DECISION---------------------------------------------------------------------
    public int chooseNode2MoveTo(Ant ant, int antNodeNumber){
        List<double> probabilities = new List<double>(nodes.Count);
        double denominator = 0;

        // calculate denominator
        for (int col = 0; col < nodes.Count; col++) {
            if (col == antNodeNumber || ant.visitedNodes.Contains(nodes[col])) continue;
            denominator += distanceMatrix[antNodeNumber, col] *
                           Math.Pow(pheromoneMatrix[antNodeNumber, col], beta);
        }

        // calculate probabilities
        for (int col = 0; col < nodes.Count; col++) {
            if (col == antNodeNumber || ant.visitedNodes.Contains(nodes[col])) {
                probabilities.Add(0.0);
            }
            else {
                double probability = (distanceMatrix[antNodeNumber, col] *
                                      Math.Pow(pheromoneMatrix[antNodeNumber, col], beta)) / denominator;
                probabilities.Add(probability);
            }
        }

        return PickRandomWeightedIndex(probabilities);
    }

    public int PickRandomWeightedIndex(List<double> probabilities)
    {
        double randomValue = random.NextDouble();
        double cumulativeSum = 0.0;
        for (int i = 0; i < probabilities.Count; i++) {
            cumulativeSum += probabilities[i];
            if (randomValue < cumulativeSum) return i;
        }

        return probabilities.Count - 1; // Fallback to last index if nothing selected
    }

    // ----------------------------------PHEROMONES---------------------------------------------------------------------
    public void updatePheromones(Ant ant) {
        // Calculate tour length
        double tourLength = 0;
        for (int i = 1; i < ant.visitedNodes.Count; i++) {
            int from = nodes.IndexOf(ant.visitedNodes[i - 1]);
            int to = nodes.IndexOf(ant.visitedNodes[i]);
            tourLength += 1.0 / distanceMatrix[from, to]; // convert back to real distance
        }

        // add distance back to start
        int lastIndex = nodes.IndexOf(ant.visitedNodes.Last());
        int firstIndex = nodes.IndexOf(ant.visitedNodes.First());
        tourLength += 1.0 / distanceMatrix[lastIndex, firstIndex];

        // update best tour if current tour is better
        if (tourLength < bestTourLength) {
            bestTourLength = tourLength;
            bestTour = new List<Point>(ant.visitedNodes);
            bestTourIteration = iteration;
            Console.WriteLine($"--Found new best tour! (length: {Math.Round(tourLength, 2)})--");
        }

        // evaporate pheromones once per iteration
        if (ant.ID == 0) {
            iteration++;
            for (int i = 0; i < nodes.Count; i++) {
                for (int j = 0; j < nodes.Count; j++) {
                    pheromoneMatrix[i, j] *= (1 - pheromoneEvaporationRate);
                }
            }

            Console.WriteLine($"Iteration: {iteration}");
        }

        // add new pheromones based on tour quality
        double pheromoneDeposit = Q / tourLength;
        for (int i = 1; i < ant.visitedNodes.Count; i++) {
            int from = nodes.IndexOf(ant.visitedNodes[i - 1]);
            int to = nodes.IndexOf(ant.visitedNodes[i]);
            pheromoneMatrix[from, to] += pheromoneDeposit;
            pheromoneMatrix[to, from] += pheromoneDeposit;
        }

        // add pheromone for return to start
        pheromoneMatrix[lastIndex, firstIndex] += pheromoneDeposit;
        pheromoneMatrix[firstIndex, lastIndex] += pheromoneDeposit;
    }
}
// --------------------------------------------------------------------------------------------------------------------

public class Ant
{
    public int ID;
    public Point antPosition { get; set; }
    public List<Point> visitedNodes = new List<Point>();

    public Ant(int id, Point node)
    {
        ID = id;
        antPosition = node;
        visitedNodes.Add(antPosition);
    }

    public void move(Point node2MoveTo)
    {
        antPosition = node2MoveTo;
        visitedNodes.Add(antPosition);
    }
}