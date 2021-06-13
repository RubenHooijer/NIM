namespace Oasez.Networking {

    public enum ReceiverGroup : byte {

        Others  = 0,
        All     = 1,
        Master  = 2,

    }

    public enum ServerNetworkEventKeys : byte {

        ServerData      = 255,
        PlayerJoined    = 254,
        PlayerLeft      = 253,
        ChangeProperties= 252,
        Ping            = 251,

    }

}