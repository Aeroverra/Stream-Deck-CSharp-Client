using Newtonsoft.Json.Linq;

namespace Tech.Aerove.StreamDeck.Client
{
    public interface IElgatoDispatcher
    {
        /// <summary>
        /// Request the global persistent data.
        /// </summary>
        void GetGlobalSettings();

        /// <summary>
        /// Request the global persistent data.
        /// </summary>
        Task GetGlobalSettingsAsync();

        /// <summary>
        /// Request the persistent data for the action's instance.
        /// </summary>
        /// <param name="context">A value to Identify the instance's action or Property Inspector. 
        /// In the case of the Property Inspector, this value is received by the 
        /// Property Inspector as parameter of the connectElgatoStreamDeckSocket function.
        /// </param>
        void GetSettings(string context);

        /// <summary>
        /// Request the persistent data for the action's instance.
        /// </summary>
        /// <param name="context">A value to Identify the instance's action or Property Inspector. 
        /// In the case of the Property Inspector, this value is received by the 
        /// Property Inspector as parameter of the connectElgatoStreamDeckSocket function.
        /// </param>
        Task GetSettingsAsync(string context);

        /// <summary>
        /// Write a debug log to the logs file.
        /// </summary>
        /// <param name="message">A string to write to the logs file.</param>
        void LogMessage(string message);


        /// <summary>
        /// Write a debug log to the logs file.
        /// </summary>
        /// <param name="message">A string to write to the logs file.</param>
        Task LogMessageAsync(string message);

        /// <summary>
        /// Open an URL in the default browser.
        /// </summary>
        /// <param name="url">An URL to open in the default browser.</param>
        void OpenUrl(string url);

        /// <summary>
        /// Open an URL in the default browser.
        /// </summary>
        /// <param name="url">An URL to open in the default browser.</param>
        Task OpenUrlAsync(string url);

        void SendRegisterEvent();

        Task SendRegisterEventAsync();

        /// <summary>
        /// Send a payload to the Property Inspector.
        /// </summary>
        /// <param name="context">A value to identify the instance's action.</param>
        /// <param name="action">The action's unique identifier.</param>
        /// <param name="data">An anonymous object that will be received by the Property Inspector.</param>
        void SendToPropertyInspector(string context, string action, object data);

        /// <summary>
        /// Send a payload to the Property Inspector.
        /// </summary>
        /// <param name="context">A value to identify the instance's action.</param>
        /// <param name="action">The action's unique identifier.</param>
        /// <param name="data">An anonymous object that will be received by the Property Inspector.</param>
        Task SendToPropertyInspectorAsync(string context, string action, object data);

        /// <summary>
        /// Save data securely and globally for the plugin.
        /// </summary>
        /// <param name="settings">An anonymous object which is persistently saved globally.</param>
        void SetGlobalSettings(object settings);


        /// <summary>
        /// Save data securely and globally for the plugin.
        /// </summary>
        /// <param name="settings">An anonymous object which is persistently saved globally.</param>
        Task SetGlobalSettingsAsync(object settings);

        /// <summary>
        /// Dynamically change the image displayed by an instance of an action.
        /// </summary>
        /// <param name="context">A value to Identify the instance's action you want to modify.</param>
        /// <param name="image">The image to display encoded in base64 with the image format declared in the mime type (PNG, JPEG, BMP, ...). svg is also supported. If not provided, the image is reset to the default image from the manifest.</param>
        /// <param name="target">Specify if you want to display the title on the hardware and software (0), only on the hardware (1), or only on the software (2). Default is 0.</param>
        /// <param name="state">A 0-based integer value representing the state of an action with multiple states. If not specified, the image is set to all states.</param>
        void SetImage(string context, string image, int target = 0, int? state = null);

        /// <summary>
        /// Dynamically change the image displayed by an instance of an action.
        /// </summary>
        /// <param name="context">A value to Identify the instance's action you want to modify.</param>
        /// <param name="image">The image to display encoded in base64 with the image format declared in the mime type (PNG, JPEG, BMP, ...). svg is also supported. If not provided, the image is reset to the default image from the manifest.</param>
        /// <param name="target">Specify if you want to display the title on the hardware and software (0), only on the hardware (1), or only on the software (2). Default is 0.</param>
        /// <param name="state">A 0-based integer value representing the state of an action with multiple states. If not specified, the image is set to all states.</param>
        Task SetImageAsync(string context, string image, int target = 0, int? state = null);

        /// <summary>
        /// Save data persistently for the action's instance.
        /// </summary>
        /// <param name="context">A value to Identify the instance's action or Property Inspector. This value is received by the Property Inspector as a parameter of the connectElgatoStreamDeckSocket function.</param>
        /// <param name="settings">An anonymous object which is persistently saved for the action's instance.</param>
        void SetSettings(string context, object settings);

        /// <summary>
        /// Save data persistently for the action's instance.
        /// </summary>
        /// <param name="context">A value to Identify the instance's action or Property Inspector. This value is received by the Property Inspector as a parameter of the connectElgatoStreamDeckSocket function.</param>
        /// <param name="settings">An anonymous object which is persistently saved for the action's instance.</param>
        Task SetSettingsAsync(string context, object settings);

        /// <summary>
        /// Change the state of the action's instance supporting multiple states.
        /// </summary>
        /// <param name="context">A value to identify the instance's action.</param>
        /// <param name="state">A 0-based integer value representing the state requested.</param>
        void SetState(string context, int state);

        /// <summary>
        /// Change the state of the action's instance supporting multiple states.
        /// </summary>
        /// <param name="context">A value to identify the instance's action.</param>
        /// <param name="state">A 0-based integer value representing the state requested.</param>
        Task SetStateAsync(string context, int state);

        /// <summary>
        /// Dynamically change the title of an instance of an action.
        /// </summary>
        /// <param name="context">A value to Identify the instance's action you want to modify.</param>
        /// <param name="title">The title to display. If there is no title parameter, the title is reset to the title set by the user.</param>
        /// <param name="target">Specify if you want to display the title on the hardware and software (0), only on the hardware (1), or only on the software (2). Default is 0.</param>
        /// <param name="state">A 0-based integer value representing the state of an action with multiple states. If not specified, the title is set to all states.</param>
        void SetTitle(string context, string title, int target = 0, int? state = null);

        /// <summary>
        /// Dynamically change the title of an instance of an action.
        /// </summary>
        /// <param name="context">A value to Identify the instance's action you want to modify.</param>
        /// <param name="title">The title to display. If there is no title parameter, the title is reset to the title set by the user.</param>
        /// <param name="target">Specify if you want to display the title on the hardware and software (0), only on the hardware (1), or only on the software (2). Default is 0.</param>
        /// <param name="state">A 0-based integer value representing the state of an action with multiple states. If not specified, the title is set to all states.</param>
        Task SetTitleAsync(string context, string title, int target = 0, int? state = null);

        /// <summary>
        /// Temporarily show an alert icon on the image displayed by an instance of an action.
        /// </summary>
        /// <param name="context">A value to identify the instance's action.</param>
        void ShowAlert(string context);


        /// <summary>
        /// Temporarily show an alert icon on the image displayed by an instance of an action.
        /// </summary>
        /// <param name="context">A value to identify the instance's action.</param>
        Task ShowAlertAsync(string context);


        /// <summary>
        /// Temporarily show an OK checkmark icon on the image displayed by an instance of an action.
        /// </summary>
        /// <param name="context">A value to identify the instance's action.</param>
        void ShowOk(string context);


        /// <summary>
        /// Temporarily show an OK checkmark icon on the image displayed by an instance of an action.
        /// </summary>
        /// <param name="context">A value to identify the instance's action.</param>
        Task ShowOkAsync(string context);

        /// <summary>
        /// Switch to one of the preconfigured read-only profiles.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="profile"></param>
        void SwitchToProfile(string device, string profile);

        /// <summary>
        /// Switch to one of the preconfigured read-only profiles.
        /// </summary>
        /// <param name="device">A value to identify the device.</param>
        /// <param name="profile">The name of the profile to switch to. The name should be identical to the name provided in the manifest.json file.</param>
        Task SwitchToProfileAsync(string device, string profile);
    }
}