using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace MoatGate.Infrastructure
{
    public class RedisTicketStore : ITicketStore
    {
        private readonly string _redisServer;
        private readonly ConnectionMultiplexer _connectionMultiplexer;

        public RedisTicketStore(string redisServer)
        {
            _redisServer = redisServer;
            _connectionMultiplexer = ConnectionMultiplexer.Connect(_redisServer);
        }

        public async Task RemoveAsync(string key)
        {
            var db = _connectionMultiplexer.GetDatabase(0);
            await db.KeyDeleteAsync(key);
        }

        public async Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            var db = _connectionMultiplexer.GetDatabase(0);
            await db.KeyDeleteAsync(key);
            var ticketBytes = TicketSerializer.Default.Serialize(ticket);
            await db.StringSetAsync(key, ticketBytes);
        }

        public async Task<AuthenticationTicket> RetrieveAsync(string key)
        {
            var db = _connectionMultiplexer.GetDatabase(0);
            if (db.KeyExists(key))
            {
                var ticketBytes = await db.StringGetAsync(key);
                return TicketSerializer.Default.Deserialize(ticketBytes);
            }
            return null;
        }

        public async Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            var key = Guid.NewGuid().ToString();
            var db = _connectionMultiplexer.GetDatabase(0);
            var ticketBytes = TicketSerializer.Default.Serialize(ticket);
            await db.StringSetAsync(key, ticketBytes);
            return key;
        }
    }
}
