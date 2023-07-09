using System;

namespace NetCoreEventBus.Infra.EventBus.Events
{
	public record Event
	{
		public Event()
		{
			this.Id = Guid.NewGuid();
			this.CreatedAt = DateTime.UtcNow;
		}
		public Guid Id { get;}
		public DateTime CreatedAt { get;}
	}
}
