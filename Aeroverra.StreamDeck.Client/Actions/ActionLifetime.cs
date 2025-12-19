using Microsoft.Extensions.DependencyInjection;

namespace Aeroverra.StreamDeck.Client.Actions
{
	/// <summary>
	/// Holds an action instance together with its owning DI scope so scoped services
	/// live for the action's entire lifetime and are disposed alongside the action.
	/// </summary>
	internal sealed class ActionLifetime : IAsyncDisposable, IDisposable
	{
		private readonly IServiceScope _scope;
		private bool _disposed;

		public ActionLifetime(ActionBase action, IServiceScope scope, string instanceId)
		{
			Action = action;
			_scope = scope;
			InstanceId = instanceId;
		}

		public ActionBase Action { get; }
		public string InstanceId { get; }

		public async ValueTask DisposeAsync()
		{
			if (_disposed) return;
			_disposed = true;

			try
			{
				if (Action is IAsyncDisposable asyncDisposable)
				{
					await asyncDisposable.DisposeAsync();
				}
				else if (Action is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}
			finally
			{
				_scope.Dispose();
			}
		}

		public void Dispose()
		{
			DisposeAsync().AsTask().GetAwaiter().GetResult();
		}
	}
}
