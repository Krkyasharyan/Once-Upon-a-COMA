using System.Text.Json;
using System.Text.Json.Serialization;

namespace Common;

public class RandomNumber
{
    private static Random random=new Random();

    public static int GetRandom()
    {
        return random.Next();
    }
}
