using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tech.aerove.streamdeck.client.Events;

namespace tech.aerove.streamdeck.client.Actions
{
    public abstract class ActionBase 
    {
        public string Context { get; set; }
        public virtual void ApplicationDidLaunch(ApplicationDidLaunchEvent e)
        {

        }

        public virtual Task ApplicationDidLaunchAsync(ApplicationDidLaunchEvent e)
        {
            return Task.CompletedTask;
        }

        public virtual void ApplicationDidTerminate(ApplicationDidTerminateEvent e)
        {

        }

        public virtual Task ApplicationDidTerminateAsync(ApplicationDidTerminateEvent e)
        {
            return Task.CompletedTask;
        }

        public virtual void DeviceDidConnect(DeviceDidConnectEvent e)
        {

        }

        public virtual Task DeviceDidConnectAsync(DeviceDidConnectEvent e)
        {
            return Task.CompletedTask;
        }

        public virtual void DeviceDidDisconnect(DeviceDidDisconnectEvent e)
        {

        }

        public virtual Task DeviceDidDisconnectAsync(DeviceDidDisconnectEvent e)
        {
            return Task.CompletedTask;
        }

        public virtual void DidReceiveGlobalSettings(DidReceiveGlobalSettingsEvent e)
        {

        }

        public virtual Task DidReceiveGlobalSettingsAsync(DidReceiveGlobalSettingsEvent e)
        {
            return Task.CompletedTask;
        }

        public virtual void DidReceiveSettings(DidReceiveSettingsEvent e)
        {

        }

        public virtual Task DidReceiveSettingsAsync(DidReceiveSettingsEvent e)
        {
            return Task.CompletedTask;
        }

        public virtual void KeyDown(KeyDownEvent e)
        {

        }

        public virtual Task KeyDownAsync(KeyDownEvent e)
        {
            return Task.CompletedTask;
        }

        public virtual void KeyUp(KeyUpEvent e)
        {

        }

        public virtual Task KeyUpAsync(KeyUpEvent e)
        {
            return Task.CompletedTask;
        }

        public virtual void PropertyInspectorDidAppear(PropertyInspectorDidAppearEvent e)
        {

        }

        public virtual Task PropertyInspectorDidAppearAsync(PropertyInspectorDidAppearEvent e)
        {
            return Task.CompletedTask;
        }

        public virtual void PropertyInspectorDidDisappear(PropertyInspectorDidDisappearEvent e)
        {

        }

        public virtual Task PropertyInspectorDidDisappearAsync(PropertyInspectorDidDisappearEvent e)
        {
            return Task.CompletedTask;
        }

        public virtual void SendToPlugin(SendToPluginEvent e)
        {

        }

        public virtual Task SendToPluginAsync(SendToPluginEvent e)
        {
            return Task.CompletedTask;
        }

        public virtual void SystemDidWakeUp(SystemDidWakeUpEvent e)
        {

        }

        public virtual Task SystemDidWakeUpAsync(SystemDidWakeUpEvent e)
        {
            return Task.CompletedTask;
        }

        public virtual void TitleParametersDidChange(TitleParametersDidChangeEvent e)
        {

        }

        public virtual Task TitleParametersDidChangeAsync(TitleParametersDidChangeEvent e)
        {
            return Task.CompletedTask;
        }

        public virtual void WillAppear(WillAppearEvent e)
        {

        }

        public virtual Task WillAppearAsync(WillAppearEvent e)
        {
            return Task.CompletedTask;
        }

        public virtual void WillDisappear(WillDisappearEvent e)
        {

        }

        public virtual Task WillDisappearAsync(WillDisappearEvent e)
        {
            return Task.CompletedTask;
        }
    }
}
