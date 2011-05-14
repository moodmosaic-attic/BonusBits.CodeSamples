using System.Reflection;
using Agatha.Common;

namespace BonusBits.CodeSamples.Agatha.Client
{
    public static class ComponentRegistration
    {
        public static void Register()
        {
            Assembly messages = Assembly.Load("BonusBits.CodeSamples.Agatha.Messages");
            new ClientConfiguration(messages, typeof(global::Agatha.Castle.Container)).Initialize();
        }
    }
}
