namespace Aeroverra.StreamDeck.Client.Actions
{

    [Serializable]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PluginAction : Attribute
    {

        public string Id { get; private set; }

        public PluginAction(string ActionId)
        {
            Id = ActionId;
        }
    }
}
