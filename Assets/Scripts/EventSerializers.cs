using Oasez.Networking;
using UnityEngine;
using ZeroFormatter.Formatters;

public static class EventSerializers {
 
    [RuntimeInitializeOnLoadMethod]
    private static void RegisterFormatters() {
        Formatter<DefaultResolver, Player>.Register(new PlayerFormatter<DefaultResolver>());

        TransportSerializer.RegisterType(typeof(Player), 0,
            TypeSerializers.ZeroFormatterSerialize<Player>,
            TypeSerializers.ZeroFormatterDeserialize<Player>);

        Debug.Log("Registered formatters");
    }

}