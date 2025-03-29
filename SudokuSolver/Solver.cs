using System.Text;

namespace SudokuSolver
{
    // https://www.tutorialspoint.com/genetic_algorithms/genetic_algorithms_fundamentals.htm
    public class Solver
    {
        private readonly Random random;

        const int POPULATION_SIZE = 2000;
        const float PRECENTAGE_OF_ELITISM = 0.01f;
        const float PERCENTAGE_OF_PARENTS_TO_KEEP = 1.0f;
        const int CANDIDATES_NB_IN_TOURNAME = 3;
        const float PERCENTAGE_OF_OFFSPRING_MUTATION_CHANCE = 0.5f;
        float PERCENTAGE_OF_GENE_MUTATION_CHANCE = 0.1f;

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
            (int lastAverage, int occurences) localMaxBreaker = (0, 0);
            do
            {
                generationCount++;
                this.population = this.population.OrderByDescending(x => x.Score).ToList();

                int avg = (int) Math.Floor(population.Average(x => x.Score));
                if(avg == localMaxBreaker.lastAverage)
                    localMaxBreaker.occurences++;
                else
                    localMaxBreaker = (avg, 0);

                if (localMaxBreaker.occurences > 20)
                {
                    Console.WriteLine($"Population reset at Gen: {generationCount,4} | Avg: {avg,3} | Max: {population[0].Score,3}");
                    population.Clear();
                    GenerateInitialPopulation();
                    generationCount = 0;
                }

                population = BreedAndMutate();
                winner = population.FirstOrDefault(x => x.Score == Sudoku.WINNING_SCORE);

            } while (winner == null);

            return winner;
        }

        private IEnumerable<Sudoku> SelectElite()
        {
            return this.population
                .Take((int)(POPULATION_SIZE * PRECENTAGE_OF_ELITISM));
                //.DistinctBy(x => x.FillingString);
        }

        private IEnumerable<Sudoku> SelectParents()
        {
            return this.population
                .Take((int)(POPULATION_SIZE * PERCENTAGE_OF_PARENTS_TO_KEEP));
        }

        private List<Sudoku> BreedAndMutate()
        {
            var newGeneration = new List<Sudoku>();

            newGeneration.AddRange(SelectElite());

            var eligibleParents = new List<Sudoku>(SelectParents());
            while (newGeneration.Count < POPULATION_SIZE)
            {
                var parent1 = TournamentSelection(eligibleParents);
                var parent2 = TournamentSelection(eligibleParents);
                var offspring = Breed(parent1, parent2);
                offspring = Mutate(offspring);
                var sudoku = new Sudoku(templateGrid);
                sudoku.Fill(offspring);
                //sudoku.Evaluate();
                newGeneration.Add(sudoku);
            }

            Parallel.ForEach(newGeneration, sudoku => { sudoku.Evaluate(); });
            return newGeneration;
        }

        private string TournamentSelection(List<Sudoku> eligibleParents)
        {
            var tournament = new List<Sudoku>();
            for (int i = 0; i < Math.Min(CANDIDATES_NB_IN_TOURNAME, eligibleParents.Count); i++)
            {
                tournament.Add(eligibleParents[random.Next(0, eligibleParents.Count)]);
            }
            return tournament.OrderByDescending(x => x.Score).First().FillingString;
        }

        private string Mutate(string offspring)
        {
            if(random.NextSingle() > PERCENTAGE_OF_OFFSPRING_MUTATION_CHANCE)
                return offspring;

            var mutatedOffspring = offspring.ToArray();
            for (int i = 0; i < mutatedOffspring.Length; i++)
            {
                if (random.NextSingle() > PERCENTAGE_OF_GENE_MUTATION_CHANCE)
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
