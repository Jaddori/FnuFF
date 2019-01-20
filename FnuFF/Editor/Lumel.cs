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
		public Triple Normal;
		public List<Lumel> Transfers;
		public float Incidence;
		public float Excidence;
		public float Emission;
		public float Reflectiveness;

		public Lumel()
		{
			Position = new Triple();
			Normal = new Triple();
			Incidence = Excidence = Emission = Reflectiveness = 0.0f;
		}

		public Lumel( Triple position, Triple normal, float emission = 0.0f, float reflectiveness = 0.5f  )
		{
			Position = position;
			Normal = normal;
			Transfers = new List<Lumel>();

			Emission = emission;
			Reflectiveness = reflectiveness;
			Incidence = Excidence = 0.0f;
		}
	}
}
