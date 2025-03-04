using System;
using System.Data;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace test5 {
    public class DatabaseManager {
        private readonly string _connectionString;
        private int _currentRunId;

        public DatabaseManager(string dbPath) {
            _connectionString = $"Data Source={dbPath}";
            InitializeDatabase();
        }

        private void InitializeDatabase() {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            // create tables
            using var command = connection.CreateCommand();
            
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS RunInfo (
                    RunID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Timestamp TEXT NOT NULL,
                    NodeCount INTEGER NOT NULL,
                    AntCount INTEGER NOT NULL,
                    Alpha REAL NOT NULL,
                    Beta REAL NOT NULL,
                    EvaporationRate REAL NOT NULL,
                    StartingPheromone REAL NOT NULL,
                    Q REAL NOT NULL
                )";
            command.ExecuteNonQuery();
            
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS NodeDistribution (
                    RunID INTEGER NOT NULL,
                    NodeIndex INTEGER NOT NULL,
                    X INTEGER NOT NULL,
                    Y INTEGER NOT NULL,
                    PRIMARY KEY (RunID, NodeIndex),
                    FOREIGN KEY (RunID) REFERENCES RunInfo(RunID)
                )";
            command.ExecuteNonQuery();
            
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS BestPaths (
                    PathID INTEGER PRIMARY KEY AUTOINCREMENT,
                    RunID INTEGER NOT NULL,
                    Iteration INTEGER NOT NULL,
                    TotalDistance REAL NOT NULL,
                    PathSequence TEXT NOT NULL,
                    FOREIGN KEY (RunID) REFERENCES RunInfo(RunID)
                )";
            command.ExecuteNonQuery();
        }
        
        public void DropAllTables() {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();

            // drop tables in correct order due to foreign key constraints
            command.CommandText = "DROP TABLE IF EXISTS BestPaths";
            command.ExecuteNonQuery();

            command.CommandText = "DROP TABLE IF EXISTS NodeDistribution";
            command.ExecuteNonQuery();

            command.CommandText = "DROP TABLE IF EXISTS RunInfo";
            command.ExecuteNonQuery();

            // reinitialize the database
            InitializeDatabase();
        }
        
        public int StartNewRun(
            int nodeCount, int antCount, double alpha, double beta,
            double evaporationRate, double startingPheromone, double q) {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();

            command.CommandText = @"
                INSERT INTO RunInfo (Timestamp, NodeCount, AntCount, Alpha, Beta, EvaporationRate, StartingPheromone, Q)
                VALUES (@timestamp, @nodeCount, @antCount, @alpha, @beta, @evaporationRate, @startingPheromone, @q);
                SELECT last_insert_rowid();";

            command.Parameters.AddWithValue("@timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.AddWithValue("@nodeCount", nodeCount);
            command.Parameters.AddWithValue("@antCount", antCount);
            command.Parameters.AddWithValue("@alpha", alpha);
            command.Parameters.AddWithValue("@beta", beta);
            command.Parameters.AddWithValue("@evaporationRate", evaporationRate);
            command.Parameters.AddWithValue("@startingPheromone", startingPheromone);
            command.Parameters.AddWithValue("@q", q);

            _currentRunId = Convert.ToInt32(command.ExecuteScalar());
            return _currentRunId;
        }

        public void SaveNodeDistribution(List<Point> nodes) {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try {
                using var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText = @"
                    INSERT INTO NodeDistribution (RunID, NodeIndex, X, Y)
                    VALUES (@runId, @nodeIndex, @x, @y)";

                for (int i = 0; i < nodes.Count; i++) {
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@runId", _currentRunId);
                    command.Parameters.AddWithValue("@nodeIndex", i);
                    command.Parameters.AddWithValue("@x", nodes[i].X);
                    command.Parameters.AddWithValue("@y", nodes[i].Y);
                    command.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch {
                transaction.Rollback();
                throw;
            }
        }

        public void SaveBestPath(int iteration, double totalDistance, List<Point> bestTour, List<Point> nodes) {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();

            // convert path to sequence of node indices
            var nodeIndices = new List<int>();
            foreach (var point in bestTour) {
                nodeIndices.Add(nodes.IndexOf(point));
            }
            string pathSequence = string.Join(",", nodeIndices);

            command.CommandText = @"
                INSERT INTO BestPaths (RunID, Iteration, TotalDistance, PathSequence)
                VALUES (@runId, @iteration, @totalDistance, @pathSequence)";

            command.Parameters.AddWithValue("@runId", _currentRunId);
            command.Parameters.AddWithValue("@iteration", iteration);
            command.Parameters.AddWithValue("@totalDistance", totalDistance);
            command.Parameters.AddWithValue("@pathSequence", pathSequence);
            command.ExecuteNonQuery();
        }
    }
}