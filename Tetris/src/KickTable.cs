
class WallKickTable
{
    public List<(int, int)> row0R = [];
    public List<(int, int)> rowR0 = [];
    public List<(int, int)> rowR2 = [];
    public List<(int, int)> row2R = [];
    public List<(int, int)> row2L = [];
    public List<(int, int)> rowL2 = [];
    public List<(int, int)> rowL0 = [];
    public List<(int, int)> row0L = [];

    public (int, int) GetOffset(Orientation fromOrientation, Orientation toOrientation, int testIndex)
    {
        if (fromOrientation == Orientation.Zero && toOrientation == Orientation.Right)
            return row0R[testIndex];
        else if (fromOrientation == Orientation.Right && toOrientation == Orientation.Zero)
            return rowR0[testIndex];
        else if (fromOrientation == Orientation.Right && toOrientation == Orientation.Two)
            return rowR2[testIndex];
        else if (fromOrientation == Orientation.Two && toOrientation == Orientation.Right)
            return row2R[testIndex];
        else if (fromOrientation == Orientation.Two && toOrientation == Orientation.Left)
            return row2L[testIndex];
        else if (fromOrientation == Orientation.Left && toOrientation == Orientation.Two)
            return rowL2[testIndex];
        else if (fromOrientation == Orientation.Left && toOrientation == Orientation.Zero)
            return rowL0[testIndex];
        else if (fromOrientation == Orientation.Zero && toOrientation == Orientation.Left)
            return row0L[testIndex];
        else
            throw new ArgumentException("Invalid orientation transition");
    }

    public static WallKickTable Make_JLSTZ_Table()
    {
        // https://tetris.wiki/Super_Rotation_System#Wall_Kicks
        return new WallKickTable()
        {
            row0R = [(0, 0), (-1, 0), (-1, +1), (0, -2), (-1, -2)],
            rowR0 = [(0, 0), (+1, 0), (+1, -1), (0, +2), (+1, +2)],
            rowR2 = [(0, 0), (+1, 0), (+1, -1), (0, +2), (+1, +2)],
            row2R = [(0, 0), (-1, 0), (-1, +1), (0, -2), (-1, -2)],
            row2L = [(0, 0), (+1, 0), (+1, +1), (0, -2), (+1, -2)],
            rowL2 = [(0, 0), (-1, 0), (-1, -1), (0, +2), (-1, +2)],
            rowL0 = [(0, 0), (-1, 0), (-1, -1), (0, +2), (-1, +2)],
            row0L = [(0, 0), (+1, 0), (+1, +1), (0, -2), (+1, -2)],
        };
    }

    public static WallKickTable Make_I_Table()
    {
        // https://tetris.wiki/Super_Rotation_System#Wall_Kicks
        return new WallKickTable()
        {
            row0R = [(0, 0), (-2, 0), (+1, 0), (-2, -1), (+1, +2)],
            rowR0 = [(0, 0), (+2, 0), (-1, 0), (+2, +1), (-1, -2)],
            rowR2 = [(0, 0), (-1, 0), (+2, 0), (-1, +2), (+2, -1)],
            row2R = [(0, 0), (+1, 0), (-2, 0), (+1, -2), (-2, +1)],
            row2L = [(0, 0), (+2, 0), (-1, 0), (+2, +1), (-1, -2)],
            rowL2 = [(0, 0), (-2, 0), (+1, 0), (-2, -1), (+1, +2)],
            rowL0 = [(0, 0), (+1, 0), (-2, 0), (+1, -2), (-2, +1)],
            row0L = [(0, 0), (-1, 0), (+2, 0), (-1, +2), (+2, -1)],
        };
    }

    public static WallKickTable Make_ThiccI_Table()
    {
        // https://tetris.wiki/Super_Rotation_System#Wall_Kicks
        return new WallKickTable()
        {
            row0R = [(0, 0), (-2, 0), (+1, 0), (-2, -1), (+1, +2)],
            rowR0 = [(0, 0), (+1, 0), (-1, 0), (+2, +1), (-1, -2)],
            rowR2 = [(0, 0), (-1, 0), (+1, 0), (-1, +2), (+2, -1)],
            row2R = [(0, 0), (+1, 0), (-1, 0), (+1, -2), (-2, +1)],
            row2L = [(0, 0), (+1, 0), (-1, 0), (+2, +1), (-1, -2)],
            rowL2 = [(0, 0), (-2, 0), (+1, 0), (-2, -1), (+1, +2)],
            rowL0 = [(0, 0), (+1, 0), (-1, 0), (+1, -2), (-2, +1)],
            row0L = [(0, 0), (-1, 0), (+2, 0), (-1, +2), (+2, -1)],
        };
    }

    public static WallKickTable Make_III_Table()
    {
        // Aggressive wall kicks for big bois
        return new WallKickTable()
        {
            row0R = [(0, 0), (-4, 0), (+2, 0), (-4, -2), (+2, +4)],
            rowR0 = [(0, 0), (+4, 0), (-2, 0), (+4, +2), (-2, -4)],
            rowR2 = [(0, 0), (-4, 0), (+3, 0), (-2, +4), (+4, -2)],
            row2R = [(0, 0), (+2, 0), (-4, 0), (+2, -4), (-4, +2)],
            row2L = [(0, 0), (+4, 0), (-2, 0), (+4, +2), (-2, -4)],
            rowL2 = [(0, 0), (-4, 0), (+2, 0), (-4, -2), (+2, +4)],
            rowL0 = [(0, 0), (+2, 0), (-5, 0), (+2, -4), (-4, +2)],
            row0L = [(0, 0), (-2, 0), (+4, 0), (-2, +4), (+4, -2)],
        };
    }
}
