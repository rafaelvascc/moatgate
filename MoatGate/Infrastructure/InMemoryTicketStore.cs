using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoatGate.Infrastructure
{
    public class InMemoryTicketStore : ITicketStore
    {
        private static Dictionary<string, AuthenticationTicket> _tickets { get; } = new Dictionary<string, AuthenticationTicket>();

        public Task RemoveAsync(string key)
        {
            _tickets.Remove(key);
            return Task.CompletedTask;
        }

        public Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            _tickets[key] = ticket;
            return Task.CompletedTask;
        }

        public Task<AuthenticationTicket> RetrieveAsync(string key)
        {
            return Task.FromResult(_tickets[key]);
        }

        public Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            var key = Guid.NewGuid().ToString();
            _tickets.Add(key, ticket);
            return Task.FromResult(key);
        }
    }
}
