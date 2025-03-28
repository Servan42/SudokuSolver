using SudokuSolver;

internal class Program
{
    private static void Main(string[] args)
    {
        var input = File.ReadAllLines("sudoku_input.txt");
        var sudoku = new Sudoku(input);
        var solved = new Solver().Solve(sudoku);
        Console.WriteLine(solved);
        File.WriteAllText("sudoku_output.htm", solved.ToHTML());
    }
}