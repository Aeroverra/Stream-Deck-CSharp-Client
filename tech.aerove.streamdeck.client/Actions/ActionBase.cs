using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tech.aerove.streamdeck.client.Actions
{
    public abstract class ActionBase 
    {
        public virtual void ApplicationDidLaunch()
        {

        }

        public virtual Task ApplicationDidLaunchAsync()
        {
            return Task.CompletedTask;
        }

        public virtual void ApplicationDidTerminate()
        {

        }

        public virtual Task ApplicationDidTerminateAsync()
        {
            return Task.CompletedTask;
        }

        public virtual void DeviceDidConnect()
        {

        }

        public virtual Task DeviceDidConnectAsync()
        {
            return Task.CompletedTask;
        }

        public virtual void DeviceDidDisconnect()
        {

        }

        public virtual Task DeviceDidDisconnectAsync()
        {
            return Task.CompletedTask;
        }

        public virtual void DidReceiveGlobalSettings()
        {

        }

        public virtual Task DidReceiveGlobalSettingsAsync()
        {
            return Task.CompletedTask;
        }

        public virtual void DidReceiveSettings()
        {

        }

        public virtual Task DidReceiveSettingsAsync()
        {
            return Task.CompletedTask;
        }

        public virtual void KeyDown()
        {

        }

        public virtual Task KeyDownAsync()
        {
            return Task.CompletedTask;
        }

        public virtual void KeyUp()
        {

        }

        public virtual Task KeyUpAsync()
        {
            return Task.CompletedTask;
        }

        public virtual void PropertyInspectorDidAppear()
        {

        }

        public virtual Task PropertyInspectorDidAppearAsync()
        {
            return Task.CompletedTask;
        }

        public virtual void PropertyInspectorDidDisappear()
        {

        }

        public virtual Task PropertyInspectorDidDisappearAsync()
        {
            return Task.CompletedTask;
        }

        public virtual void SendToPlugin()
        {

        }

        public virtual Task SendToPluginAsync()
        {
            return Task.CompletedTask;
        }

        public virtual void SystemDidWakeUp()
        {

        }

        public virtual Task SystemDidWakeUpAsync()
        {
            return Task.CompletedTask;
        }

        public virtual void TitleParametersDidChange()
        {

        }

        public virtual Task TitleParametersDidChangeAsync()
        {
            return Task.CompletedTask;
        }

        public virtual void WillAppear()
        {

        }

        public virtual Task WillAppearAsync()
        {
            return Task.CompletedTask;
        }

        public virtual void WillDisappear()
        {

        }

        public virtual Task WillDisappearAsync()
        {
            return Task.CompletedTask;
        }
    }
}
