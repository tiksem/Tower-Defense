using System;

namespace AssemblyCSharp
{
	public interface SavingGameComponent
	{
		void OnRestore(object data);
		object OnSave();
	}
}

