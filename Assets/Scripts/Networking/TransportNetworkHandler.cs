using Oasez.Extensions.Generics.Singleton;

namespace Oasez.Networking {

    public class TransportNetworkHandler : GenericSingleton<TransportNetworkHandler, TransportNetworkHandler> {

        public TransportClient Client { get; private set; }
        public TransportServer Server { get; private set; }

        protected override void Awake() {
            base.Awake();
            Client = gameObject.AddComponent<TransportClient>();
        }

        public void AddServer() {
            Server = gameObject.AddComponent<TransportServer>();
        }

        public void RemoveServer() {
            Destroy(Server);
        }

    } 

}