using UnityEngine;

public class Connect4PositionMB : MonoBehaviour
{
    public const int WIDTH = 7;
    public const int HEIGHT = 6;
    public const int STRIDE = HEIGHT + 1;

    public const int MIN_SCORE = -(WIDTH * HEIGHT) / 2 + 3;
    public const int MAX_SCORE = (WIDTH * HEIGHT + 1) / 2 - 3;

    // current_position = stones of current player to move
    // mask = all stones
    [SerializeField] private ulong currentPosition = 0UL;
    [SerializeField] private ulong mask = 0UL;
    [SerializeField] private int moves = 0;

    public ulong CurrentPosition => currentPosition;
    public ulong Mask => mask;
    public int NbMoves => moves;

    public static readonly ulong BottomMask = ComputeBottomMask();
    public static readonly ulong BoardMask = BottomMask * ((1UL << HEIGHT) - 1UL);

    public void ResetPosition()
    {
        currentPosition = 0UL;
        mask = 0UL;
        moves = 0;
    }

    public void LoadFrom(ulong posToMove, ulong boardMask)
    {
        // IMPORTANT: expects posToMove = side-to-move stones, boardMask = all stones
        currentPosition = posToMove;
        mask = boardMask;
        moves = PopCount(boardMask);
    }

    public ulong Key()
    {
        return currentPosition + mask;
    }

    public bool CanPlay(int col)
    {
        return (mask & TopMaskCol(col)) == 0;
    }

    public ulong Possible()
    {
        return (mask + BottomMask) & BoardMask;
    }

    public bool CanWinNext()
    {
        return (WinningPosition() & Possible()) != 0;
    }

    public void Play(ulong moveBit)
    {
        // identical to Pascal Pons
        currentPosition ^= mask;
        mask |= moveBit;
        moves++;
    }

    public void PlayCol(int col)
    {
        // play((mask + bottom_mask_col(col)) & column_mask(col));
        ulong move = (mask + BottomMaskCol(col)) & ColumnMask(col);
        Play(move);
    }

    public bool IsWinningMove(int col)
    {
        return (WinningPosition() & Possible() & ColumnMask(col)) != 0;
    }

    public ulong PossibleNonLosingMoves()
    {
        // identical logic to Pascal Pons (assumes !CanWinNext)
        ulong possibleMask = Possible();
        ulong opponentWin = OpponentWinningPosition();
        ulong forced = possibleMask & opponentWin;

        if (forced != 0) {
            if ((forced & (forced - 1)) != 0) return 0; // more than one forced
            possibleMask = forced;
        }

        return possibleMask & ~(opponentWin >> 1);
    }

    public int MoveScore(ulong move)
    {
        return PopCount(ComputeWinningPosition(currentPosition | move, mask));
    }

    // winning squares for current player
    private ulong WinningPosition()
    {
        return ComputeWinningPosition(currentPosition, mask);
    }

    // winning squares for opponent
    private ulong OpponentWinningPosition()
    {
        return ComputeWinningPosition(currentPosition ^ mask, mask);
    }

    // ---- Static helpers ----

    public static ulong ColumnMask(int col)
    {
        return ((1UL << HEIGHT) - 1UL) << (col * STRIDE);
    }

    private static ulong TopMaskCol(int col)
    {
        return 1UL << ((HEIGHT - 1) + col * STRIDE);
    }

    private static ulong BottomMaskCol(int col)
    {
        return 1UL << (col * STRIDE);
    }

    private static ulong ComputeBottomMask()
    {
        ulong b = 0UL;
        for (int c = 0; c < WIDTH; c++)
            b |= 1UL << (c * STRIDE);
        return b;
    }

    public static int PopCount(ulong x)
    {
        int c = 0;
        while (x != 0) {
            x &= x - 1;
            c++;
        }
        return c;
    }

    public static ulong ComputeWinningPosition(ulong position, ulong mask)
    {
        // identical to Pascal Pons compute_winning_position

        // vertical
        ulong r = (position << 1) & (position << 2) & (position << 3);

        // horizontal
        ulong p = (position << (HEIGHT + 1)) & (position << 2 * (HEIGHT + 1));
        r |= p & (position << 3 * (HEIGHT + 1));
        r |= p & (position >> (HEIGHT + 1));

        p = (position >> (HEIGHT + 1)) & (position >> 2 * (HEIGHT + 1));
        r |= p & (position << (HEIGHT + 1));
        r |= p & (position >> 3 * (HEIGHT + 1));

        // diagonal 1
        p = (position << HEIGHT) & (position << 2 * HEIGHT);
        r |= p & (position << 3 * HEIGHT);
        r |= p & (position >> HEIGHT);

        p = (position >> HEIGHT) & (position >> 2 * HEIGHT);
        r |= p & (position << HEIGHT);
        r |= p & (position >> 3 * HEIGHT);

        // diagonal 2
        p = (position << (HEIGHT + 2)) & (position << 2 * (HEIGHT + 2));
        r |= p & (position << 3 * (HEIGHT + 2));
        r |= p & (position >> (HEIGHT + 2));

        p = (position >> (HEIGHT + 2)) & (position >> 2 * (HEIGHT + 2));
        r |= p & (position << (HEIGHT + 2));
        r |= p & (position >> 3 * (HEIGHT + 2));

        // keep only empty squares
        return r & (BoardMask ^ mask);
    }
}
