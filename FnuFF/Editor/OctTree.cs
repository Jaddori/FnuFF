using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
	public class OctTree<T>
	{
		public class Node
		{
			public Node Parent;
			public Node[] Children;
			public List<T> Objects;
			public List<Triple> ObjectMins;
			public List<Triple> ObjectMaxs;
			public Triple Min, Max;
			public bool Empty;
		}

		private int _size;
		private int _minSize;
		private Node _root;

		public Node Root => _root;

		public OctTree( int size, int minSize = 16 )
		{
			_size = size;
			_minSize = minSize;

			_root = CreateNode( null, new Triple( -_size * 0.5f ), new Triple( _size * 0.5f ) );
		}

		public void Add( T obj, Triple min, Triple max )
		{
			AddToNode( _root, min, max, obj );
		}

		private void AddToNode( Node node, Triple min, Triple max, T obj )
		{
			node.Empty = false;

			if( node.Children == null )
			{
				node.Objects.Add( obj );
				node.ObjectMins.Add( min );
				node.ObjectMaxs.Add( max );
			}
			else
			{
				/*Node childNode = null;
				foreach( var child in node.Children )
				{
					if( InsideBox( child.Min, child.Max, min ) || InsideBox( child.Min, child.Max, max ) )
					{
						if( childNode != null )
						{
							childNode = null;
							break;
						}
						else
							childNode = child;
					}
				}

				if( childNode != null )
				{
					AddToNode( childNode, min, max, obj );
				}
				else
				{
					// no single child could contain the whole object, add to parent instead
					node.Objects.Add( obj );
					node.ObjectMins.Add( min );
					node.ObjectMaxs.Add( max );
				}*/

				foreach( var child in node.Children )
				{
					if( BoxesIntersect( child.Min, child.Max, min, max ) )
					{
						AddToNode( child, min, max, obj );
					}
				}
			}
		}

		private Node CreateNode( Node parent, Triple min, Triple max )
		{
			var result = new Node();
			result.Parent = parent;
			result.Min = min;
			result.Max = max;
			result.Objects = new List<T>();
			result.ObjectMins = new List<Triple>();
			result.ObjectMaxs = new List<Triple>();
			result.Empty = true;

			if( max.X - min.X > _minSize )
			{
				result.Children = new Node[8];

				var boxSize = max - min;
				var halfSize = boxSize * 0.5f;

				for( int x = 0; x < 2; x++ )
				{
					for( int y = 0; y < 2; y++ )
					{
						for( int z = 0; z < 2; z++ )
						{
							var index = x * 4 + y * 2 + z;

							var childMin = min + new Triple( halfSize.X * x, halfSize.Y * y, halfSize.Z * z );
							var childMax = childMin + halfSize;

							result.Children[index] = CreateNode( result, childMin, childMax );
						}
					}
				}
			}

			return result;
		}

		public void Clear()
		{
			ClearNode( _root );
		}

		private void ClearNode( Node node )
		{
			if( node.Children != null )
				foreach( var child in node.Children )
					ClearNode( child );

			node.Objects.Clear();
			node.Empty = true;
		}

		private bool InsideBox( Triple min, Triple max, Triple point )
		{
			return
			(
				point.X >= min.X && point.X <= max.X &&
				point.Y >= min.Y && point.Y <= max.Y &&
				point.Z >= min.Z && point.Z <= max.Z
			);
		}

		private bool BoxesIntersect( Triple amin, Triple amax, Triple bmin, Triple bmax )
		{
			var eps = Extensions.EPSILON;

			return
			(
				amax.X-eps > bmin.X &&
				amin.X+eps < bmax.X &&
				amax.Y-eps > bmin.Y &&
				amin.Y+eps < bmax.Y &&
				amax.Z-eps > bmin.Z &&
				amin.Z+eps < bmax.Z
			);
		}
	}
}
