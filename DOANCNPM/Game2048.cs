using System;
using System.Linq;

public class Game2048
{
    public int[,] Board { get; private set; } // Trạng thái của bảng
    public int Score { get; private set; } // Điểm số hiện tại
    private readonly Random rand = new Random();

    public Game2048()
    {
        Board = new int[4, 4];
        ResetGame();
    }

    // Khởi tạo lại trò chơi
    public void ResetGame()
    {
        Array.Clear(Board, 0, Board.Length);
        Score = 0;
        AddRandomTile();
        AddRandomTile();
    }

    // Thêm một ô ngẫu nhiên vào bảng
    public void AddRandomTile()
    {
        var emptyTiles = (from row in Enumerable.Range(0, 4)
                          from col in Enumerable.Range(0, 4)
                          where Board[row, col] == 0
                          select (row, col)).ToList();

        if (emptyTiles.Count > 0)
        {
            var (r, c) = emptyTiles[rand.Next(emptyTiles.Count)];
            Board[r, c] = rand.Next(10) == 0 ? 4 : 2;
        }
    }

    // Kiểm tra trò chơi có thắng hay không
    public bool IsGameWon() => Board.Cast<int>().Any(value => value == 2048);

    // Kiểm tra trò chơi có kết thúc hay không
    public bool IsGameOver()
    {
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                if (Board[row, col] == 0 ||
                    (row > 0 && Board[row, col] == Board[row - 1, col]) ||
                    (col > 0 && Board[row, col] == Board[row, col - 1]))
                {
                    return false;
                }
            }
        }
        return true;
    }

    // Các phương thức di chuyển
    public bool MoveLeft() => SlideTiles((r, c) => (r, c), (r, c) => (r, c - 1));
    public bool MoveRight() => SlideTiles((r, c) => (r, 3 - c), (r, c) => (r, c + 1));
    public bool MoveUp() => SlideTiles((r, c) => (c, r), (r, c) => (r - 1, c));
    public bool MoveDown() => SlideTiles((r, c) => (3 - c, r), (r, c) => (r + 1, c));

    // Xử lý logic trượt ô
    private bool SlideTiles(Func<int, int, (int, int)> order, Func<int, int, (int, int)> next)
    {
        bool moved = false;
        for (int row = 0; row < 4; row++)
        {
            int[] line = new int[4];
            int index = 0;

            for (int col = 0; col < 4; col++)
            {
                var (r, c) = order(row, col);
                if (Board[r, c] != 0)
                {
                    if (index > 0 && line[index - 1] == Board[r, c])
                    {
                        line[index - 1] *= 2;
                        Score += line[index - 1];
                        moved = true;
                    }
                    else
                    {
                        line[index++] = Board[r, c];
                    }
                }
            }

            for (int col = 0; col < 4; col++)
            {
                var (r, c) = order(row, col);
                if (Board[r, c] != line[col])
                {
                    moved = true;
                }
                Board[r, c] = line[col];
            }
        }

        return moved;
    }
}
    