using Aeroverra.StreamDeck.Client.Pipeline.Middleware;
using Microsoft.Extensions.DependencyInjection;

namespace Aeroverra.StreamDeck.Client.Pipeline
{
	/// <summary>
	/// Tracks a middleware instance together with the scope that provided its dependencies,
	/// ensuring scoped services live as long as the middleware (singleton pipeline lifetime).
	/// </summary>
	internal sealed class MiddlewareLifetime : IAsyncDisposable, IDisposable
	{
		public MiddlewareLifetime(MiddlewareBase middleware, IServiceScope scope)
		{
			Middleware = middleware;
			Scope = scope;
		}

		public MiddlewareBase Middleware { get; }
		public IServiceScope Scope { get; }

		public async ValueTask DisposeAsync()
		{
			try
			{
				if (Middleware is IAsyncDisposable asyncDisposable)
				{
					await asyncDisposable.DisposeAsync();
				}
				else if (Middleware is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}
			finally
			{
				Scope.Dispose();
			}
		}

		public void Dispose()
		{
			DisposeAsync().AsTask().GetAwaiter().GetResult();
		}
	}
}
