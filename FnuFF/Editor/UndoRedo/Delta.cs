using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.UndoRedo
{
	public class Delta<T>
	{
		public T Old { get; set; }
		public T New { get; set; }

		public Delta()
		{
		}

		public Delta( T old, T @new )
		{
			Old = old;
			New = @new;
		}

		public bool Same()
		{
			return Old.Equals( New );
		}
	}
}
