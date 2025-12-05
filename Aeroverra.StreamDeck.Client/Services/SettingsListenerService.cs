using Aeroverra.StreamDeck.Client.Events;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace Aeroverra.StreamDeck.Client.Services
{
    internal sealed class SettingsListenerService(ILogger<SettingsListenerService> logger, EventManager eventsManager, IGlobalSettings settings) : IAsyncDisposable
    {
        public Task StartListeningAsync()
        {
            eventsManager.OnDidReceiveGlobalSettings += EventsManager_OnDidReceiveGlobalSettings;
            return Task.CompletedTask;
        }

        private void EventsManager_OnDidReceiveGlobalSettings(object? sender, DidReceiveGlobalSettingsEvent globalSettingsEvent)
        {
            JObject jObject = globalSettingsEvent.Payload.Settings;

            // Get the concrete type of the settings object
            Type settingsType = settings.GetType();

            // Loop through all public instance properties
            foreach (PropertyInfo prop in settingsType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                // 1. Skip if we can't write to this property
                if (!prop.CanWrite) continue;

                // 2. Check if the JObject contains this property name
                // StringComparison.OrdinalIgnoreCase allows "mySetting" in C# to match "MySetting" in JSON
                if (jObject.TryGetValue(prop.Name, StringComparison.OrdinalIgnoreCase, out JToken? token))
                {
                    try
                    {
                        // 3. Convert the token to the specific type of the property
                        // ToObject handles int, string, bool, List<>, Dictionary<,>, and Nullables automatically.
                        object? value = token.ToObject(prop.PropertyType);

                        // 4. Set the value on the settings instance
                        prop.SetValue(settings, value);
                    }
                    catch (Exception e)
                    {
                        logger.LogWarning(e, "Failed to set property {PropertyName} on settings", prop.Name);
                        // Optional: Handle conversion errors (e.g., JSON string provided for an int property)
                        // For now, we silently skip properties that fail to cast.
                    }
                }
            }

            settings.IsReady = true;
        }

        public ValueTask DisposeAsync()
        {
            eventsManager.OnDidReceiveGlobalSettings -= EventsManager_OnDidReceiveGlobalSettings;
            return ValueTask.CompletedTask;
        }
    }
}
