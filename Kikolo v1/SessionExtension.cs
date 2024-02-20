//using Microsoft.AspNetCore.Http;
//using System.IO;
//using System.Runtime.Serialization.Formatters.Binary;

//public static class SessionExtensions
//{
//    public static void Set<T>(this ISession session, string key, T value)
//    {
//        using (MemoryStream stream = new MemoryStream())
//        {
//            new BinaryFormatter().Serialize(stream, value);
//            session.Set(key, stream.ToArray());
//        }
//    }

//    public static T Get<T>(this ISession session, string key)
//    {
//        byte[] data = session.Get(key);
//        if (data == null)
//        {
//            return default(T);
//        }

//        using (MemoryStream stream = new MemoryStream(data))
//        {
//            return (T)new BinaryFormatter().Deserialize(stream);
//        }
//    }
//}