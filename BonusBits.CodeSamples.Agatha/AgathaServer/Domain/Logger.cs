using System;

namespace BonusBits.CodeSamples.Agatha.Backend.Domain
{
	/// <summary>
	/// http://onestepback.org/index.cgi/Tech/Ruby/DependencyInjectionInRuby.rdoc
	/// </summary>
	public sealed class Logger : ILogger
	{
		/// <summary>
		/// Gets or sets the verbose.
		/// </summary>
		/// <value>The verbose.</value>
		public Boolean Verbose { get; set; }
	}
}
