using JetBrains.Annotations;
using UnityEngine;

public interface IAiStrategy
{
    int ChooseMove(int[] scores);
}

public class EasyAi : IAiStrategy
{
    private Solver solver;
    private int weight = 10;

    public EasyAi(Solver solver)
    {
        this.solver = solver;
    }
    
    public int ChooseMove(int[] scores)
    {
        return solver.ChooseWeightedMove(scores, weight);
        
    }
}

public class MediumAi : IAiStrategy
{
    private Solver solver;
    private int weight = 4;

    public MediumAi(Solver solver)
    {
        this.solver = solver;
    }

    public int ChooseMove(int[] scores)
    {
        return solver.ChooseWeightedMove(scores, weight);
        
    }
}

public class HardAi : IAiStrategy
{
    private Solver solver;
    private int weight = 1;

    public HardAi(Solver solver)
    {
        this.solver = solver;
    }

    public int ChooseMove(int[] scores)
    {
        return solver.ChooseWeightedMove(scores, weight);
    }
}

public class ImpossibleAi : IAiStrategy
{
    private Solver solver;

    public ImpossibleAi(Solver solver)
    {
        this.solver = solver;
    }

    public int ChooseMove(int[] scores)
    {
        return solver.ChooseBestMove(scores);
    }
}
