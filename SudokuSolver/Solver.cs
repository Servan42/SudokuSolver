using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    // https://www.tutorialspoint.com/genetic_algorithms/genetic_algorithms_fundamentals.htm
    public class Solver
    {
        private readonly Random random;

        const int POPULATION_SIZE = 1000;
        const float percentageOfElitism = 0.1f;
        const float percentageOfParentsToKeep = 0.5f;
        const float percentageOfGeneMutationChange = 0.1f;

        // Cached sudoku data
        private string[] templateGrid;
        private int fillingStringSize;

        private List<Sudoku> population = [];

        public Solver(int? seed = null)
        {
            random = seed.HasValue ? new Random(seed.Value) : new Random();
        }

        public Sudoku Solve(Sudoku originalSudoku)
        {
            int generationCount = 0;
            this.templateGrid = originalSudoku.GetTemplate();
            this.fillingStringSize = originalSudoku.GetMissingNumbersCount();

            GenerateInitialPopulation();

            Sudoku? winner = null;
            do
            {
                generationCount++;
                var newGeneration = new List<Sudoku>();
                this.population = this.population.OrderByDescending(x => x.Score).ToList();
                newGeneration.AddRange(SelectElite());
                population = BreedAndMutate(newGeneration);
                winner = population.FirstOrDefault(x => x.Score == Sudoku.WINNING_SCORE);

                Console.WriteLine($"Generation {generationCount} | Score: {population.Average(x => x.Score)}");
            } while (winner == null);

            Console.WriteLine($"Found after {generationCount} generations");
            return winner;
        }

        private IEnumerable<Sudoku> SelectElite()
        {
            return this.population
                .Take((int)(POPULATION_SIZE * percentageOfElitism));
        }

        private IEnumerable<Sudoku> SelectParents()
        {
            return this.population
                .Take((int)(POPULATION_SIZE * percentageOfParentsToKeep));
        }

        private List<Sudoku> BreedAndMutate(List<Sudoku> newGeneration)
        {
            var parents = new List<Sudoku>(SelectParents());
            while(newGeneration.Count < POPULATION_SIZE)
            {
                var parent1 = population[random.Next(0, parents.Count)].FillingString;
                var parent2 = population[random.Next(0, parents.Count)].FillingString;
                var offspring = Breed(parent1, parent2);
                offspring = Mutate(offspring);
                var sudoku = new Sudoku(templateGrid);
                sudoku.Fill(offspring);
                sudoku.Evaluate();
                newGeneration.Add(sudoku);
            }

            return newGeneration;
        }

        private string Mutate(string offspring)
        {
            var mutatedOffspring = offspring.ToArray();
            for (int i = 0; i < mutatedOffspring.Length; i++)
            {
                if (random.NextSingle() > percentageOfGeneMutationChange)
                    continue;

                mutatedOffspring[i] = random.CharNum();
            }
            
            return new string(mutatedOffspring);
        }

        private string Breed(string parent1, string parent2)
        {
            var offspring = new StringBuilder();
            for (int i = 0; i  < parent1.Length; i++)
            {
                offspring.Append(random.NextBool() ? parent1[i] : parent2[i]);
            }
            return offspring.ToString();
        }

        private void GenerateInitialPopulation()
        {
            for (int i = 0; i < POPULATION_SIZE; i++)
            {
                var sudoku = new Sudoku(templateGrid);
                sudoku.Fill(random.CharNumString(this.fillingStringSize));
                sudoku.Evaluate();
                population.Add(sudoku);
            }
        }
    }
}
