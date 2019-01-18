using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
	public class Lumel
	{
		public Triple Position;
		public float Radiance;
		public Triple Normal;
		public List<Lumel> Transfers;

		public Lumel()
			: this( new Triple( 0.0f ) )
		{
		}

		public Lumel( Triple position, float radiance = 0.0f )
		{
			Position = position;
			Radiance = radiance;
			Transfers = new List<Lumel>();
		}

		public Lumel( Triple position, Triple normal, float radiance = 0.0f  )
		{
			Position = position;
			Radiance = radiance;
			Normal = normal;
			Transfers = new List<Lumel>();
		}
	}
}
