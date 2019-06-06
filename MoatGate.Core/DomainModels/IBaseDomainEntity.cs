using System;

namespace MoatGate.Core.DomainModels
{
    public interface IBaseDomainEntity
    {
        Guid Id { get; set; }
    }
}
