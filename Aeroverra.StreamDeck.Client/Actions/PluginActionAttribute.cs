namespace Aeroverra.StreamDeck.Client.Actions
{

    [Serializable]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class PluginActionAttribute : Attribute
    {

        public string Id { get; private set; }

        public PluginActionAttribute(string ActionId)
        {
            Id = ActionId;
        }
    }
}
