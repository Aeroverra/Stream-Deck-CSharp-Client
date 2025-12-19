using Aeroverra.StreamDeck.Client.Events;
using Aeroverra.StreamDeck.Client.Events.SDKEvents;
using Microsoft.Extensions.Logging;

namespace Aeroverra.StreamDeck.Client.Actions
{
    internal class DefaultActionExecuter : IActionExecuter
    {
        private readonly ILogger<DefaultActionExecuter> _logger;
        public DefaultActionExecuter(ILogger<DefaultActionExecuter> logger)
        {
            _logger = logger;
        }

        public async Task ExecuteAsync(IElgatoEvent elgatoEvent, List<ActionBase> actions)
        {
            foreach (ActionBase action in actions)
            {
                _= Task.Run(() => ExecuteAsync(elgatoEvent, action));
            }
            await Task.Delay(0);
        }

        private async Task ExecuteAsync(IElgatoEvent elgatoEvent, ActionBase action)
        {
            try
            {
                var actionEvent = elgatoEvent;
                switch (elgatoEvent.Event)
                {
                    case ElgatoEventType.DidReceiveSettings:
                        {
                            var e = (DidReceiveSettingsEvent)actionEvent;
                            action.DidReceiveSettings(e.Payload.Settings);
                            await action.DidReceiveSettingsAsync(e.Payload.Settings);
                            break;
                        }
                    case ElgatoEventType.DidReceiveGlobalSettings:
                        {
                            var e = (DidReceiveGlobalSettingsEvent)actionEvent;
                            action.DidReceiveGlobalSettings(e.Payload.Settings);
                            await action.DidReceiveGlobalSettingsAsync(e.Payload.Settings);
                            break;
                        }
                    case ElgatoEventType.KeyDown:
                        {
                            var e = (KeyDownEvent)actionEvent;
                            action.KeyDown(e.Payload.UserDesiredState);
                            await action.KeyDownAsync(e.Payload.UserDesiredState);
                            break;
                        }
                    case ElgatoEventType.KeyUp:
                        {
                            var e = (KeyUpEvent)actionEvent;
                            action.KeyUp(e.Payload.UserDesiredState);
                            await action.KeyUpAsync(e.Payload.UserDesiredState);
                            break;
                        }
                    case ElgatoEventType.DialRotate:
                        {
                            var e = (DialRotateEvent)actionEvent;
                            action.DialRotate(e.Payload);
                            await action.DialRotateAsync(e.Payload);
                            break;
                        }
                    case ElgatoEventType.DialDown:
                        {
                            var e = (DialDownEvent)actionEvent;
                            action.DialDown(e.Payload);
                            await action.DialDownAsync(e.Payload);
                            break;
                        }
                    case ElgatoEventType.DialUp:
                        {
                            var e = (DialUpEvent)actionEvent;
                            action.DialUp(e.Payload);
                            await action.DialUpAsync(e.Payload);
                            break;
                        }
                    case ElgatoEventType.TouchTap:
                        {
                            var e = (TouchTapEvent)actionEvent;
                            action.TouchTap(e.Payload);
                            await action.TouchTapAsync(e.Payload);
                            break;
                        }
                    case ElgatoEventType.WillAppear:
                        {
                            action.WillAppear();
                            await action.WillAppearAsync();
                            break;
                        }
                    case ElgatoEventType.WillDisappear:
                        {
                            action.WillDisappear();
                            await action.WillDisappearAsync();
                            break;
                        }
                    case ElgatoEventType.TitleParametersDidChange:
                        {
                            action.TitleParametersDidChange();
                            await action.TitleParametersDidChangeAsync();
                            break;
                        }
                    case ElgatoEventType.DeviceDidConnect:
                        {
                            var e = (DeviceDidConnectEvent)actionEvent;
                            action.DeviceDidConnect(e.Device, e.DeviceInfo.Name, e.DeviceInfo.Size.Columns, e.DeviceInfo.Size.Rows);
                            await action.DeviceDidConnectAsync(e.Device, e.DeviceInfo.Name, e.DeviceInfo.Size.Columns, e.DeviceInfo.Size.Rows);
                            break;
                        }
                    case ElgatoEventType.DeviceDidDisconnect:
                        {
                            var e = (DeviceDidDisconnectEvent)actionEvent;
                            action.DeviceDidDisconnect(e.Device);
                            await action.DeviceDidDisconnectAsync(e.Device);
                            break;
                        }
                    case ElgatoEventType.ApplicationDidLaunch:
                        {
                            var e = (ApplicationDidLaunchEvent)actionEvent;
                            action.ApplicationDidLaunch(e.Payload.Application);
                            await action.ApplicationDidLaunchAsync(e.Payload.Application);
                            break;
                        }
                    case ElgatoEventType.ApplicationDidTerminate:
                        {
                            var e = (ApplicationDidTerminateEvent)actionEvent;
                            action.ApplicationDidTerminate(e.Payload.Application);
                            await action.ApplicationDidTerminateAsync(e.Payload.Application);
                            break;
                        }
                    case ElgatoEventType.SystemDidWakeUp:
                        {
                            var e = (SystemDidWakeUpEvent)actionEvent;
                            action.SystemDidWakeUp();
                            await action.SystemDidWakeUpAsync();
                            break;
                        }
                    case ElgatoEventType.PropertyInspectorDidAppear:
                        {
                            var e = (PropertyInspectorDidAppearEvent)actionEvent;
                            action.PropertyInspectorDidAppear();
                            await action.PropertyInspectorDidAppearAsync();
                            break;
                        }
                    case ElgatoEventType.PropertyInspectorDidDisappear:
                        {
                            var e = (PropertyInspectorDidDisappearEvent)actionEvent;
                            action.PropertyInspectorDidDisappear();
                            await action.PropertyInspectorDidDisappearAsync();
                            break;
                        }
                    case ElgatoEventType.SendToPlugin:
                        {
                            var e = (SendToPluginEvent)actionEvent;
                            action.SendToPlugin(e.payload);
                            await action.SendToPluginAsync(e.payload);
                            break;
                        }
					case ElgatoEventType.OnInitialized:
						{
							action.OnInitialized();
							await action.OnInitializedAsync();
							break;
						}
					case ElgatoEventType.DialStop:
						{
							if (actionEvent is DialStopEvent e)
							{
								switch (e.EventLength)
                                {
                                    case DialStopEventLength.Short:
                                        {
                                            action.DialStopShort(e.Payload);
                                            await action.DialStopShortAsync(e.Payload);
                                            break;
                                        }
                                    case DialStopEventLength.Medium:
                                        {
                                            action.DialStopMedium(e.Payload);
                                            await action.DialStopMediumAsync(e.Payload);
                                            break;
                                        }
                                    case DialStopEventLength.Long:
                                        {
                                            action.DialStopLong(e.Payload);
                                            await action.DialStopLongAsync(e.Payload);
                                            break;
                                        }
                                    case DialStopEventLength.ExtraLong:
                                        {
                                            action.DialStopExtraLong(e.Payload);
                                            await action.DialStopExtraLongAsync(e.Payload);
                                            break;
                                        }

                                }
                            }
                            break;
                        }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unhandled error in action {action}", action);
            }
        }

    }
}
