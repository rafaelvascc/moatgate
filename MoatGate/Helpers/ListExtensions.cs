using AutoMapper;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using MoatGate.Models.AspNetIIdentityCore.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoatGate.Helpers
{
    public static class ListExtensions
    {
        public static void ReflectEntityFrameworkState<T>(this IList<T> currentState, IList<T> newState, DbContext context) where T : class
        {
            if (currentState == null)
            {
                currentState = new List<T>();
            }

            if (newState == null)
            {
                newState = new List<T>();
            }

            var existingIds = currentState.Select(e => (int)typeof(T).GetProperty("Id").GetValue(e)).ToList<int>();
            var newStateIds = newState.Select(e => (int)typeof(T).GetProperty("Id").GetValue(e)).ToList<int>();
            var commonIds = existingIds.Intersect(newStateIds);
            var toDeleteIds = existingIds.Except(commonIds);

            var toUpdate = newState.Where(e => existingIds.Contains((int)typeof(T).GetProperty("Id").GetValue(e)));
            var toDelete = currentState.Where(e => toDeleteIds.Contains((int)typeof(T).GetProperty("Id").GetValue(e)));
            var toAdd = newState.Where(e => (int)typeof(T).GetProperty("Id").GetValue(e) <= 0);

            foreach (var entry in toDelete)
            {
                context.Entry(entry).State = EntityState.Deleted;
            }

            foreach (var entry in toUpdate)
            {
                var existingEntity = currentState.Where(e => (int)typeof(T).GetProperty("Id").GetValue(e) == (int)typeof(T).GetProperty("Id").GetValue(entry)).Single();
                if (GetHashCodeFromPropertyValues<T>(entry) != GetHashCodeFromPropertyValues<T>(existingEntity))
                {
                    Mapper.Map(entry, existingEntity);
                }
            }

            foreach (var entry in toAdd)
            {
                if (typeof(T).IsSubclassOf(typeof(IdentityServer4.EntityFramework.Entities.Secret)) || typeof(T).IsSubclassOf(typeof(IdentityServer4.Models.Secret)))
                {
                    typeof(T).GetProperty("Value").SetValue(entry, ((string)typeof(T).GetProperty("Value").GetValue(entry)).Sha256());
                }

                currentState.Add(entry);
                context.Entry(entry).State = EntityState.Added;
            }
        }

        private static int GetHashCodeFromPropertyValues<T>(T o)
        {
            return String.Concat(typeof(T).GetProperties().Where(p => IsPrimitive(p.PropertyType)).Select(p => p.GetValue(o) != null ? $"{p.Name}:{p.GetValue(o).ToString()}|" : $"{p.Name}::null:|")).GetHashCode();
        }

        private static bool IsPrimitive(Type t)
        {
            return new[] {
            typeof(string),
            typeof(char),
            typeof(byte),
            typeof(sbyte),
            typeof(ushort),
            typeof(short),
            typeof(uint),
            typeof(int),
            typeof(ulong),
            typeof(long),
            typeof(float),
            typeof(double),
            typeof(decimal),
            typeof(bool),
            typeof(DateTime),
            typeof(Guid),
            }.Contains(t);
        }
    }
}
