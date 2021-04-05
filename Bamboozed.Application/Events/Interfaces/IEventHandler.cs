﻿using System.Threading.Tasks;

namespace Bamboozed.Application.Events.Interfaces
{
    public interface IEventHandler<in T>
        where T : IDomainEvent
    {
        Task HandleAsync(T domainEvent);
    }
}
