/*namespace Common;

public class Singleton<T> where T:new ()
{
    private static T m_Instance;

    public static T Instance()
    {
        if (m_Instance==null)
        {
            m_Instance = new T();
        }

        return m_Instance;
    }
}
public class Doubleton<T> where T:new ()
{
    private static T m_Instance;

    public static T Instance()
    {
        if (m_Instance==null)
        {
            m_Instance = new T();
        }

        return m_Instance;
    }
}*/