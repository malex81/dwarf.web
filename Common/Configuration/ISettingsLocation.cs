using System;
using System.Collections.Generic;
using System.Text;

namespace Dwarf.Web.Configuration
{
	public interface ISettingsLocation
	{
		string ConfigurationBasePath { get; }
	}
}
