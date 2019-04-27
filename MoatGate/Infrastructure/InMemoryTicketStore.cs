using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoatGate.Infrastructure
{
    public class InMemoryTicketStore : ITicketStore
    {
        private static Dictionary<string, AuthenticationTicket> Tickets { get; } = new Dictionary<string, AuthenticationTicket>();

        public Task RemoveAsync(string key)
        {
            Tickets.Remove(key);
            return Task.CompletedTask;
        }

        public Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            Tickets[key] = ticket;
            return Task.CompletedTask;
        }

        public Task<AuthenticationTicket> RetrieveAsync(string key)
        {
            if (Tickets.ContainsKey(key))
            {
                return Task.FromResult(Tickets[key]);
            }
            else
            {
                return Task.FromResult<AuthenticationTicket>(null);
            }
        }

        public Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            var key = Guid.NewGuid().ToString();
            Tickets.Add(key, ticket);
            return Task.FromResult(key);
        }
    }
}
