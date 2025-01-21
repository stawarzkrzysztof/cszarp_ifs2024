// using Microsoft.Xna.Framework;
// using Microsoft.Xna.Framework.Graphics;
// using Microsoft.Xna.Framework.Input;
// using System;
// using MonoGame.Extended;
// using System.Collections.Generic;
//
// namespace test5;
//
// // https://docs.monogame.net/articles/getting_started/index.html
// // https://www.youtube.com/watch?v=u7bQomllcJw&t=1011s
//
// public class Game1 : Game
// {
//     #region Fields
//     // monogame variables
//     private GraphicsDeviceManager _graphics;
//     private SpriteBatch _spriteBatch;
//     
//     // graphical const
//     private const int WindowSize = 600;
//     private const int nodeTextureRadius = 10;
//     
//     // // model constants
//     // private const int nodeCount = 50;
//     // private const int antCount = nodeCount;
//     // private const int distancePower = 1; // alpha
//     // private const int pheromonePower = 4; // beta
//     // private const int Q = 300;
//     // private const double startingPheromone = .2;
//     // private const double pheromoneEvaporationRate = .5;
//     
//     // model constants
//     private const int nodeCount = 50;
//     private const int antCount = nodeCount;
//     private const int distancePower = 2; // alpha
//     private const int pheromonePower = 5; // beta
//     private const int Q = 300;
//     private const double startingPheromone = 1;
//     private const double pheromoneEvaporationRate = .1;
//     
//     // code variables
//     private Random random = new Random();
//     private List<Point> nodes;
//     private Ant[] ants;
//     private Dictionary<Point, double> distances;
//     private double elapsedTime;
//     private const double antMovementFramerate = 60; // FPS
//     private bool animationRunning;
//     private Texture2D _lineTexture;
//     private float lineWidth = 1f; // work with line width 
//     private double[,] distanceMatrix = new double[nodeCount, nodeCount];
//     private double[,] pheromoneMatrix = new double[nodeCount, nodeCount];
//     private int iteration = 0;
//     
//     // colors
//     private Color defaultNodeColor = Color.Black;
//     private Color antNodeColor = Color.Red;
//     private Color backgroundColor = Color.White;
//     private Color currentFillColor;
//     private Color pathColor = Color.Green;
//     
//     #endregion
//     
//     public Game1() {
//         _graphics = new GraphicsDeviceManager(this);
//         Content.RootDirectory = "Content";
//         IsMouseVisible = true;
//         
//         
//         _graphics.PreferredBackBufferWidth = WindowSize;
//         _graphics.PreferredBackBufferHeight = WindowSize;
//         _graphics.ApplyChanges();
//     }
//
//     protected override void Initialize() {
//         // TODO: Add your initialization logic here
//         
//         // create random-positioned nodes
//         nodes = new List<Point>(nodeCount);
//         for (int i = 0; i < nodeCount; i++) { 
//             Point point2add = new Point(
//                 random.Next(0, WindowSize-nodeTextureRadius), 
//                 random.Next(0, WindowSize-nodeTextureRadius));
//             nodes.Add(point2add);
//         }
//
//         double distance;
//         // create node and pheromone matrix
//         for (int i = 0; i < nodeCount; i++) {
//             for (int j = 0; j < nodeCount; j++) {
//                 distance = Math.Sqrt(Math.Pow(nodes[i].X-nodes[j].X, 2) + Math.Pow(nodes[i].Y-nodes[j].Y, 2));
//                 distanceMatrix[i, j] = 1.0 / Math.Pow(distance, distancePower);
//                 pheromoneMatrix[i, j] = startingPheromone;
//             }
//         }
//
//         // create ants on nodes
//         ants = new Ant[antCount];
//         for (int i = 0; i < antCount; i++) {
//             ants[i] = new Ant(i, nodes[i]);
//         }
//         
//         base.Initialize();
//     }
//
//     protected override void LoadContent() {
//         _spriteBatch = new SpriteBatch(GraphicsDevice);
//
//         // TODO: use this.Content to load your game content here
//         
//         _lineTexture = new Texture2D(GraphicsDevice, 1, 1);
//         _lineTexture.SetData(new[] { Color.White });
//     }
//
//     protected override void Update(GameTime gameTime) {
//         if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
//             Keyboard.GetState().IsKeyDown(Keys.Escape))
//             Exit();
//
//         // TODO: Add your update logic here
//         // start the animation with Enter or Space
//         if (Keyboard.GetState().IsKeyDown(Keys.Enter)) animationRunning = true;
//         if (Keyboard.GetState().IsKeyDown(Keys.Space)) animationRunning = false;
//         
//         // move ants with certain framerate
//         elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;
//         if (animationRunning && elapsedTime >= 1/antMovementFramerate) {
//             foreach (Ant ant in ants) {
//                 if (ant.visitedNodes.Count < nodes.Count) {
//                     int nodeIndex = nodes.IndexOf(new Point(ant.antPosition.X, ant.antPosition.Y));
//                     int indexOfNode2MoveTo = chooseNode2MoveTo(ant, nodeIndex);
//                     ant.move(nodes[indexOfNode2MoveTo]);
//                     
//                 }
//                 else {
//                     updatePheromones(ant);
//                     // go back to the starting node 
//                     Point endNode = new Point(ant.visitedNodes[0].X, ant.visitedNodes[0].Y);
//                     ant.move(endNode);
//                     ant.visitedNodes = new List<Point>();
//                     ant.visitedNodes.Add(endNode);
//                     // Console.WriteLine(endNode.ToString());
//                 }
//             }
//             elapsedTime = 0;
//         }
//         base.Update(gameTime);
//     }
//
//     protected override void Draw(GameTime gameTime) {
//         GraphicsDevice.Clear(backgroundColor);
//         _spriteBatch.Begin(SpriteSortMode.Immediate,
//             blendState: BlendState.AlphaBlend,
//             samplerState: SamplerState.PointClamp,
//             rasterizerState: new RasterizerState { ScissorTestEnable = true });
//         // TODO: Add your drawing code here
//         for (int i = 0; i < nodes.Count; i++) {
//             Point node = nodes[i];
//             var nodeTexture = new Rectangle(node.X, node.Y, nodeTextureRadius, nodeTextureRadius);
//             
//             // color ants and nodes
//             currentFillColor = defaultNodeColor;
//             for (int j = 0; j < ants.Length; j++)
//             {
//                 Ant ant = ants[j];
//                 if (node.X == ant.antPosition.X &&
//                     node.Y == ant.antPosition.Y) {
//                     currentFillColor = antNodeColor;
//                 }
//                 _spriteBatch.FillRectangle(nodeTexture, currentFillColor);
//                 
//                 // leave trail of the path
//                 if (ant.visitedNodes.Count >= 2) {
//                     for (int k = 1; k < ant.visitedNodes.Count; k++) {
//                         Vector2 point1 = new Vector2(
//                             ant.visitedNodes[k - 1].X, 
//                             ant.visitedNodes[k - 1].Y);
//                         Vector2 point2 = new Vector2(
//                             ant.visitedNodes[k].X,
//                             ant.visitedNodes[k].Y);
//                         DrawLine(_spriteBatch, point1, point2, pathColor, lineWidth);
//                     }
//                 }
//             }
//             
//         }
//         _spriteBatch.End();
//         base.Draw(gameTime);
//     }
//     private void DrawLine(SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float width)
//     {
//         // Oblicz odległość i kąt pomiędzy punktami
//         float distance = Vector2.Distance(point1, point2);
//         float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
//
//         // Rysuj linię jako przeskalowaną i obróconą białą teksturę z kolorem
//         spriteBatch.Draw(_lineTexture, 
//             point1, 
//             null, 
//             color,      // Dynamiczny kolor
//             angle, 
//             Vector2.Zero, 
//             new Vector2(distance, width), 
//             SpriteEffects.None, 
//             0);
//     }
//
//     
//     protected override void UnloadContent()
//     {
//         _lineTexture.Dispose();
//         base.UnloadContent();
//     }
//     
//     public int chooseNode2MoveTo(Ant ant, int antNodeNumber) {
//         List<double> probabilities = new List<double>(nodes.Count);
//         
//         // count mianownik
//         double mianownik = 0;
//         for (int col = 0; col < nodes.Count; col++) {
//             if (col == antNodeNumber || ant.visitedNodes.Contains(nodes[col])) continue;
//             mianownik += Math.Pow(distanceMatrix[antNodeNumber, col], distancePower) * Math.Pow(pheromoneMatrix[antNodeNumber, col], pheromonePower);
//         }
//
//         // count licznik and add probabilities to a list
//         for (int col = 0; col < nodes.Count; col++) {
//             double licznik = Math.Pow(distanceMatrix[antNodeNumber, col], distancePower) * Math.Pow(pheromoneMatrix[antNodeNumber, col], pheromonePower);
//             if (col == antNodeNumber || ant.visitedNodes.Contains(nodes[col])) probabilities.Add(0.0);
//             else probabilities.Add(licznik/mianownik);
//         }
//
//         // probabilities.ForEach(i => Console.Write("{0}\t", Math.Round(i, 2)));
//         // Console.Write(probabilities.Sum());
//         // Console.WriteLine();
//         int selectedIndex = PickRandomWeightedIndex(probabilities);
//         return selectedIndex;
//     }
//     
//     public int PickRandomWeightedIndex(List<double> probabilities) {
//         double randomValue = random.NextDouble(); // Random value between 0.0 and 1.0
//         double cumulativeSum = 0.0;
//         for (int i = 0; i < probabilities.Count; i++) {
//             cumulativeSum += probabilities[i];
//             if (randomValue < cumulativeSum) return i;
//         }
//
//         throw new InvalidOperationException("Probabilities do not sum to 1.0"); // Safety check
//     }
//
//     public void updatePheromones(Ant ant) {
//         double distance = .0;
//         for (int i = 1; i < nodeCount; i++) {
//             int tmp1 = nodes.IndexOf(new Point(ant.visitedNodes[i].X, ant.visitedNodes[i].Y));
//             int tmp2 = nodes.IndexOf(new Point(ant.visitedNodes[i-1].X, ant.visitedNodes[i-1].Y));
//             distance += distanceMatrix[tmp1, tmp2];
//         }
//         
//
//         if (ant.ID == 0) { // evaporate pheromones once
//             iteration++;
//             for (int i = 0; i < nodes.Count; i++) {
//                 for (int j = 0; j < nodes.Count; j++) {
//                     pheromoneMatrix[i, j] *= 1-pheromoneEvaporationRate;
//                     
//                 }
//             }
//             Console.WriteLine("Iteration: " + iteration);
//         }
//         
//         for (int i = 1; i < nodeCount; i++) {
//             int tmp1 = nodes.IndexOf(new Point(ant.visitedNodes[i].X, ant.visitedNodes[i].Y));
//             int tmp2 = nodes.IndexOf(new Point(ant.visitedNodes[i-1].X, ant.visitedNodes[i-1].Y));
//             pheromoneMatrix[tmp1, tmp2] += Q/distance;
//             pheromoneMatrix[tmp2, tmp1] += Q/distance;
//         }
//         
//         
//         
//         
//         
//         
//         // evaporate pheromones
//         
//         
//         
//     }
// }
//
//
//
// // #####################################################################################################################
// // #####################################################################################################################
// // #####################################################################################################################
//
// public class Ant
// {
//     public int ID;
//     public Point antPosition { get; set; }
//     public List<Point> visitedNodes = new List<Point>();
//
//     public Ant(int id, Point node)
//     {
//         ID = id;
//         antPosition = node;
//         visitedNodes.Add(antPosition);
//     }
//     
//     public void move(Point node2MoveTo) {
//         antPosition = node2MoveTo;
//         visitedNodes.Add(antPosition);
//   