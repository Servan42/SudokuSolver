using System.Collections.ObjectModel;
using System.Text;

namespace SudokuSolver
{
    public class Sudoku
    {
        public const int WINNING_SCORE = 3 * 9 * 9;

        private static readonly ReadOnlyCollection<(int line, int col)> SQUARE_CENTERS = new([
            (1, 1), (1, 4), (1, 7),
            (4, 1), (4, 4), (4, 7),
            (7, 1), (7, 4), (7, 7),
        ]);
        private static readonly ReadOnlyCollection<(int x, int y)> SQUARE_DIRECTIONS = new([
            (-1, -1), (-1, 0), (-1, 1),
            (0, -1), (0, 0), (0, 1),
            (1, -1), (1, 0), (1, 1),
        ]);

        private string[] templateGrig;
        private List<string> filledGrid;

        public int Score { get; private set; }
        public string FillingString { get; private set; }

        public Sudoku(string[] input)
        {
            this.templateGrig = input;
            this.filledGrid = new List<string>();
        }

        public string[] GetTemplate()
        {
            return new List<string>(templateGrig).ToArray();
        }

        public int Evaluate()
        {
            var score = 0;
            foreach (var line in filledGrid)
                score += GetScore(line);

            for (int col = 0; col < 9; col++)
            {
                var colNumbers = new char[9];
                for (int line = 0; line < 9; line++)
                {
                    colNumbers[line] = filledGrid[line][col];
                }
                score += GetScore(colNumbers);
            }

            foreach(var squareCenter in SQUARE_CENTERS)
            {
                score += GetScore(FlattenSquare(squareCenter));
            }

            this.Score = score;
            return score;
        }

        private char[] FlattenSquare((int line, int col) squareCenter)
        {
            var result = new char[9];
            var (line, col) = squareCenter;
            for(int i = 0; i < 9; i++)
            {
                result[i] = filledGrid[line + SQUARE_DIRECTIONS[i].x][col + SQUARE_DIRECTIONS[i].y];
            }
            return result;
        }

        private int GetScore(IEnumerable<char> line)
        {
            var uniqueChars = new HashSet<char>();
            foreach (var c in line)
            {
                if (c == '.')
                    continue;

               uniqueChars.Add(c);
            }
            return uniqueChars.Count;
        }

        public void Fill(string fillingString)
        {
            this.FillingString = fillingString;
            var queue = new Queue<char>(fillingString);
            var gridLine = new StringBuilder();
            for (int line = 0; line < templateGrig.Length; line++)
            {
                for (int col = 0; col < templateGrig.Length; col++)
                {
                    if (templateGrig[line][col] != '.')
                        gridLine.Append(templateGrig[line][col]);
                    else
                        gridLine.Append(queue.Dequeue());
                }
                filledGrid.Add(gridLine.ToString());
                gridLine.Clear();
            }
        }

        public int GetMissingNumbersCount()
        {
            return templateGrig.SelectMany(x => x).Count(x => x == '.');
        }

        public override string? ToString()
        {
            return string.Join('\n', filledGrid);
        }

        public string ToHTML()
        {
            var sb = new StringBuilder();
            sb.Append("<html>");
            sb.Append("<style>span { padding: 15px } table, tr, td { border:1px solid gray; border-collapse: collapse; font-size: larger; font-family: Consolas } html { background-color: black; color: white; }</style>");
            sb.Append("<table>");
            for(int line = 0; line < 9; line++) 
            {
                sb.Append("<tr>");
                for (int col = 0; col < 9; col++)
                {
                    sb.Append("<td>");
                    if (templateGrig[line][col] == '.')
                        sb.Append("<span style=\"color: lightblue\">");
                    else
                        sb.Append("<span>");
                    sb.Append(filledGrid[line][col]);
                    sb.Append("</span></td>");
                }
                sb.Append("</tr>");
            }
            sb.Append("</table></html>");
            return sb.ToString();
        }
    }
}
