using StackExchange.Redis;

namespace FreeCourse.Services.Basket.Services
{
    public class RedisService
    {
        private readonly string _host;
        private readonly int _port;
        private ConnectionMultiplexer _connectionMultiplexer;

        public RedisService(string host, int port)
        {
            _host = host;
            _port = port;
        }

        public void Connect() => _connectionMultiplexer = ConnectionMultiplexer.Connect($"{_host}:{_port}");

        //Rediste 16 tane database gelir bunların farklı olmasının sebebi birinde test yap birinde gerçek veri tut bırınde stagging tut vs gibi.
        //Default 1 veri tabanı verdim.
        public IDatabase GetDatabase(int db = 1) => _connectionMultiplexer.GetDatabase(db);
    }
}
