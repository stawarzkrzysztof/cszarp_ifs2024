using System;
using System.Collections.Generic;

namespace test2;

class Ant
{
    public int CurrentCity { get; set; }
    public List<int> VisitedCities { get; set; }

    public Ant(int startCity)
    {
        CurrentCity = startCity;
        VisitedCities = new List<int> { startCity };
    }

    public int ChooseNextCity(double[,] pheromoneLevels, double[,] distanceMatrix)
    {
        double totalPheromone = 0;
        for (int i = 0; i < pheromoneLevels.GetLength(0); i++)
        {
            if (!VisitedCities.Contains(i))
            {
                totalPheromone += pheromoneLevels[CurrentCity, i] / distanceMatrix[CurrentCity, i];
            }
        }

        double randomValue = new Random().NextDouble() * totalPheromone;
        for (int i = 0; i < pheromoneLevels.GetLength(0); i++)
        {
            if (!VisitedCities.Contains(i))
            {
                randomValue -= pheromoneLevels[CurrentCity, i] / distanceMatrix[CurrentCity, i];
                if (randomValue <= 0)
                {
                    return i;
                }
            }
        }
        return -1; // No city to visit
    }
}

class Program
{
    static void Main()
    {
        int numberOfCities = 5;
        double[,] distanceMatrix = new double[,]
        {
            { 0, 2, 9, 10, 1 },
            { 1, 0, 6, 4, 3 },
            { 15, 7, 0, 8, 12 },
            { 6, 3, 12, 0, 5 },
            { 10, 4, 8, 2, 0 }
        };

        double[,] pheromoneLevels = new double[numberOfCities, numberOfCities];
        for (int i = 0; i < numberOfCities; i++)
            for (int j = 0; j < numberOfCities; j++)
                pheromoneLevels[i, j] = 1.0;

        int iterations = 10;
        for (int iter = 0; iter < iterations; iter++)
        {
            Ant ant = new Ant(0);
            while (ant.VisitedCities.Count < numberOfCities)
            {
                int nextCity = ant.ChooseNextCity(pheromoneLevels, distanceMatrix);
                if (nextCity != -1)
                {
                    ant.VisitedCities.Add(nextCity);
                    ant.CurrentCity = nextCity;
                    pheromoneLevels[ant.CurrentCity, nextCity] += 1.0 / distanceMatrix[ant.CurrentCity, nextCity]; // Update pheromones
                }
            }

            Console.WriteLine("Ant's path: " + string.Join(" -> ", ant.VisitedCities));
        }
    }
}
