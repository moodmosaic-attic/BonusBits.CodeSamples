using System.Diagnostics;
using BonusBits.CodeSamples.WP7.Domain.Evans.Cargo;
using BonusBits.CodeSamples.WP7.Infrastructure.Factories;
using BonusBits.CodeSamples.WP7.Infrastructure.Threading;
using Microsoft.Phone.Controls;

namespace BonusBits.CodeSamples.WP7
{
    public partial class CodeOnlyPageView : PhoneApplicationPage
    {
        public CodeOnlyPageView()
        {
            InitializeComponent();

            // Singleton using the default constructor.
            var singleton = new SingleInstance<Cargo>();

            // Singleton using factory method.
            var singleton2 = new SingleInstance<Cargo>(FactoryMethod);

            Debug.Assert(singleton.Instance  != null);
            Debug.Assert(singleton2.Instance != null);
        }

        private static Cargo FactoryMethod()
        {
            return CargoFactory.CreateNew("Glyfada", "Perachora");
        }
    }
}