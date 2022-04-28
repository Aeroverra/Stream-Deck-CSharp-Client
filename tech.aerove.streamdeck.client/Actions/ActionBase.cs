using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tech.Aerove.StreamDeck.Client.Events;

namespace Tech.Aerove.StreamDeck.Client.Actions
{
    public abstract class ActionBase
    {
        public IActionDispatcher Dispatcher { get; set; }
        public IActionContext Context { get; set; }

        /// <summary>
        /// When a monitored application is launched, the plugin will receive the applicationDidLaunch event.
        /// </summary>
        /// <param name="application">The identifier of the application that has been launched.</param>
        public virtual void ApplicationDidLaunch(string application)
        {

        }

        /// <summary>
        /// When a monitored application is launched, the plugin will receive the applicationDidLaunch event.
        /// </summary>
        /// <param name="application">The identifier of the application that has been launched.</param>
        public virtual Task ApplicationDidLaunchAsync(string application)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// When a monitored application is terminated, the plugin will receive the applicationDidTerminate event.
        /// </summary>
        /// <param name="application">The identifier of the application that has been launched.</param>
        public virtual void ApplicationDidTerminate(string application)
        {

        }

        /// <summary>
        /// When a monitored application is terminated, the plugin will receive the applicationDidTerminate event.
        /// </summary>
        /// <param name="application">The identifier of the application that has been launched.</param>
        public virtual Task ApplicationDidTerminateAsync(string application)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// When a device is plugged to the computer, the plugin will receive a deviceDidConnect event.
        /// </summary>
        /// <param name="id">A value to identify the device.</param>
        /// <param name="name">The name of the device set by the user.</param>
        /// <param name="columns">The number of columns and rows of keys that the device owns.</param>
        /// <param name="rows">The number of columns and rows of keys that the device owns.</param>
        public virtual void DeviceDidConnect(string id, string name, int columns, int rows)
        {

        }

        /// <summary>
        /// When a device is plugged to the computer, the plugin will receive a deviceDidConnect event.
        /// </summary>
        /// <param name="id">A value to identify the device.</param>
        /// <param name="name">The name of the device set by the user.</param>
        /// <param name="columns">The number of columns and rows of keys that the device owns.</param>
        /// <param name="rows">The number of columns and rows of keys that the device owns.</param>
        public virtual Task DeviceDidConnectAsync(string id, string name, int columns, int rows)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// When a device is unplugged from the computer, the plugin will receive a deviceDidDisconnect event.
        /// </summary>
        /// <param name="id">A value to identify the device.</param>
        public virtual void DeviceDidDisconnect(string id)
        {

        }

        /// <summary>
        /// When a device is unplugged from the computer, the plugin will receive a deviceDidDisconnect event.
        /// </summary>
        /// <param name="id">A value to identify the device.</param>
        public virtual Task DeviceDidDisconnectAsync(string id)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Event received after calling the getGlobalSettings API to retrieve the global persistent data.
        /// </summary>
        /// <param name="settings">This JSON object contains persistently stored data.</param>
        public virtual void DidReceiveGlobalSettings(JObject settings)
        {

        }

        /// <summary>
        /// Event received after calling the getGlobalSettings API to retrieve the global persistent data.
        /// </summary>
        /// <param name="settings">This JSON object contains persistently stored data.</param>
        public virtual Task DidReceiveGlobalSettingsAsync(JObject settings)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Event received after calling the getSettings API to retrieve the persistent data stored for the action.
        /// </summary>
        /// <param name="settings">This JSON object contains persistently stored data.</param>
        public virtual void DidReceiveSettings(JObject settings)
        {

        }

        /// <summary>
        /// Event received after calling the getSettings API to retrieve the persistent data stored for the action.
        /// </summary>
        /// <param name="settings">This JSON object contains persistently stored data.</param>
        public virtual Task DidReceiveSettingsAsync(JObject settings)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// When the user presses a key, the plugin will receive the keyDown event.
        /// </summary>
        /// <param name="userDesiredState">Only set when the action is triggered with a specific value from a Multi-Action. For example, if the user sets the Game Capture Record action to be disabled in a Multi-Action, you would see the value 1. 0 and 1 are valid.</param>
        public virtual void KeyDown(int userDesiredState)
        {

        }


        /// <summary>
        /// When the user presses a key, the plugin will receive the keyDown event.
        /// </summary>
        /// <param name="userDesiredState">Only set when the action is triggered with a specific value from a Multi-Action. For example, if the user sets the Game Capture Record action to be disabled in a Multi-Action, you would see the value 1. 0 and 1 are valid.</param>
        public virtual Task KeyDownAsync(int userDesiredState)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// When the user releases a key, the plugin will receive the keyUp event.
        /// </summary>
        /// <param name="userDesiredState">Only set when the action is triggered with a specific value from a Multi-Action. For example, if the user sets the Game Capture Record action to be disabled in a Multi-Action, you would see the value 1. 0 and 1 are valid.</param>
        public virtual void KeyUp(int userDesiredState)
        {

        }

        /// <summary>
        /// When the user releases a key, the plugin will receive the keyUp event.
        /// </summary>
        /// <param name="userDesiredState">Only set when the action is triggered with a specific value from a Multi-Action. For example, if the user sets the Game Capture Record action to be disabled in a Multi-Action, you would see the value 1. 0 and 1 are valid.</param>
        public virtual Task KeyUpAsync(int userDesiredState)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Event received when the Property Inspector appears in the Stream Deck user interface, for example, when selecting a new instance.
        /// </summary>
        public virtual void PropertyInspectorDidAppear()
        {

        }

        /// <summary>
        /// Event received when the Property Inspector appears in the Stream Deck user interface, for example, when selecting a new instance.
        /// </summary>
        public virtual Task PropertyInspectorDidAppearAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Event received when the Property Inspector is removed from the Stream Deck user interface, for example, when selecting a different instance.
        /// </summary>
        public virtual void PropertyInspectorDidDisappear()
        {

        }

        /// <summary>
        /// Event received when the Property Inspector is removed from the Stream Deck user interface, for example, when selecting a different instance.
        /// </summary>
        public virtual Task PropertyInspectorDidDisappearAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Event received by the plugin when the Property Inspector uses the sendToPlugin event.
        /// </summary>
        /// <param name="data"></param>
        public virtual void SendToPlugin(JObject data)
        {

        }

        /// <summary>
        /// Event received by the plugin when the Property Inspector uses the sendToPlugin event.
        /// </summary>
        /// <param name="data"></param>
        public virtual Task SendToPluginAsync(JObject data)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// When the computer wakes up, the plugin will receive the systemDidWakeUp event.
        /// </summary>
        public virtual void SystemDidWakeUp()
        {

        }

        /// <summary>
        /// When the computer wakes up, the plugin will receive the systemDidWakeUp event.
        /// </summary>
        public virtual Task SystemDidWakeUpAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// When the user changes the title or title parameters, the plugin will receive a titleParametersDidChange event.
        /// </summary>
        public virtual void TitleParametersDidChange()
        {

        }

        /// <summary>
        /// When the user changes the title or title parameters, the plugin will receive a titleParametersDidChange event.
        /// </summary>
        public virtual Task TitleParametersDidChangeAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// When an instance of an action is displayed on Stream Deck, for example, when the hardware is first plugged in or when a folder containing that action is entered, the plugin will receive a willAppear event.
        /// </summary>
        public virtual void WillAppear()
        {

        }

        /// <summary>
        /// When an instance of an action is displayed on Stream Deck, for example, when the hardware is first plugged in or when a folder containing that action is entered, the plugin will receive a willAppear event.
        /// </summary>
        public virtual Task WillAppearAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// When an instance of an action ceases to be displayed on Stream Deck, for example, when switching profiles or folders, the plugin will receive a willDisappear event.
        /// </summary>
        public virtual void WillDisappear()
        {

        }

        /// <summary>
        /// When an instance of an action ceases to be displayed on Stream Deck, for example, when switching profiles or folders, the plugin will receive a willDisappear event.
        /// </summary>
        public virtual Task WillDisappearAsync()
        {
            return Task.CompletedTask;
        }
    }
}
