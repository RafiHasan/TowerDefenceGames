using Unity.Entities;
using Unity.Mathematics;

public struct RandomComponent : IComponentData
{
    public uint seed;
    public Random random;
    public Random deterministicrandom;

    //Initialization

    public void InitState(uint seed)
    {
        this.seed = seed;
        random.InitState(seed);
        deterministicrandom.InitState(seed);
    }

    //RandomIntegers

    public int NextInt()
    {
        return random.NextInt();
    }

    public int NextInt(int rangemin, int rangemax)
    {
        return random.NextInt(rangemin, rangemax);
    }

    public int NextInt(uint hashkey)
    {
        deterministicrandom.InitState(hashkey);
        deterministicrandom.NextInt();
        deterministicrandom.NextInt();
        return deterministicrandom.NextInt();
    }

    public int NextInt(uint hashkey,int rangemin,int rangemax)
    {
        deterministicrandom.InitState(hashkey);
        deterministicrandom.NextInt();
        deterministicrandom.NextInt();
        return deterministicrandom.NextInt(rangemin, rangemax);
    }
    //RandomFloats

    public float NextFloat()
    {
        return random.NextFloat();
    }
    public float NextFloat(float rangemin, float rangemax)
    {
        return random.NextFloat(rangemin, rangemax);
    }

    public float NextFloat(uint hashkey)
    {
        deterministicrandom.InitState(hashkey);
        deterministicrandom.NextFloat();
        deterministicrandom.NextFloat();
        return deterministicrandom.NextFloat();
    }

    public float NextFloat(uint hashkey, float rangemin, float rangemax)
    {
        deterministicrandom.InitState(hashkey);
        deterministicrandom.NextFloat();
        deterministicrandom.NextFloat();
        return deterministicrandom.NextFloat(rangemin, rangemax);
    }

    //RandomBooleans
    public bool NextBool()
    {
        return random.NextBool();
    }

    public bool NextBool(uint hashkey)
    {
        deterministicrandom.InitState(hashkey);
        deterministicrandom.NextBool();
        deterministicrandom.NextBool();
        return deterministicrandom.NextBool();
    }
}