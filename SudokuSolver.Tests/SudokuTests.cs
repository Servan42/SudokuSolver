namespace SudokuSolver.Tests
{
    public class SudokuTests
    {
        Sudoku sudoku_sut;

        [SetUp]
        public void Setup()
        {
            sudoku_sut = new Sudoku([
                ".....3.27",
                ".1..57..9",
                "6......3.",
                "45..7...2",
                "...4.9..5",
                "9.2..1374",
                "1.58.2496",
                ".6..4..18",
                ".4.1.6.53"
            ]);
        }

        [Test]
        public void Should_return_the_count_of_missing_numbers()
        {
            // WHEN
            int result = sudoku_sut.GetMissingNumbersCount();

            // THEN
            Assert.That(result, Is.EqualTo(43));
        }

        [Test]
        public void Should_fill_with_filling_string()
        {
            // GIVEN
            var fillingString = new string('1', sudoku_sut.GetMissingNumbersCount());

            // WHEN
            sudoku_sut.Fill(fillingString);

            // THEN
            string expected = "111113127\n111157119\n611111131\n451171112\n111419115\n912111374\n115812496\n161141118\n141116153";
            Assert.That(sudoku_sut.ToString(), Is.EqualTo(expected));
        }

        [Test]
        public void Should_get_score()
        {
            // GIVEN
            sudoku_sut.Fill(new string('1', sudoku_sut.GetMissingNumbersCount()));

            // WHEN
            int score = sudoku_sut.Evaluate();

            // THEN
            Assert.That(score, Is.EqualTo(
                4 + 4 + 3 + 3 + 4 + 6 + 3 + 6 + 9
                + 4 + 4 + 3 + 5 + 4 + 6 + 7 + 4 + 5
                + 2 + 4 + 5 + 5 + 4 + 6 + 4 + 5 + 7));
        }

        [Test]
        public void Should_get_winning_score()
        {
            // GIVEN
            sudoku_sut.Fill("5946188326427984511389673621885673397522897");

            // WHEN
            int score = sudoku_sut.Evaluate();

            // THEN
            Assert.That(score, Is.EqualTo(Sudoku.WINNING_SCORE));
        }
    }
}