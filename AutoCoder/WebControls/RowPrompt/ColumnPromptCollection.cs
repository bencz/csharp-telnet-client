using System;
using System.Collections;

namespace AutoCoder.WebControls.RowPrompt
{
	// ----------------------- ColumnPromptCollection -------------------------
	public class ColumnPromptCollection : IList
	{
		//Private ArrayList to hold member objects
		private ArrayList arrList=new ArrayList();

		#region IList Members
		//Because IList interface type expects members of type object, to
		//ensure that we deal with ColumnPrompt objects only as collection members
		//we use "dual" method implementation.

		//Interface members that deal with object instances (type object), explicit interface implementation,
		//check passed objects for null references and that they are of type ColumnPrompt.
		//If passed object is of type ColumnPrompt, it is delegated to public instance member 
		//that corresponds to equivalent IList member but expects typed argument (of type ColumnPrompt)

		//This implementation ensures that if collection is accessed by casting it to IList interface type,
		//it works as expected and the intended functionality doesn't break. Also when collection is
		//accessed using "normal" instance members, it provides it's implementation as typed.
		//This will also give the collection designer support if it is used in VS.NET, for example as
		//property in a custom server control
		
		public bool IsReadOnly
		{
			get
			{
				return arrList.IsReadOnly; 
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return IsReadOnly; 		
			}
		}


		public ColumnPrompt this[int index]
		{
			get
			{
				return (ColumnPrompt)arrList[index];
			}
			set
			{
				if(value != null)
				{
					arrList[index]=value;
				}
				
				throw new ArgumentNullException("value","Collection does not accept null members"); 
				
			}
		}

		object IList.this[int index]
		{
			get
			{
				return (ColumnPrompt)arrList[index];		
			}
			set
			{
				if(value != null)
				{
					ColumnPrompt p=value as ColumnPrompt;
					if(p==null)
					{	
						throw new ArgumentException("Collection member must be of type ColumnPrompt","value"); 					}
					else
					{
						arrList[index]=p;
					}
				}
				else
				{
					throw new ArgumentNullException("value","Collection does not accept null members");
				}
				
			}
		}



		void IList.RemoveAt(int index)
		{
			RemoveAt(index);
		}

		public void RemoveAt(int index)
		{
			arrList.RemoveAt(index); 	
		}


		
		void IList.Insert(int index, object value)
		{
			if(value==null)
			{
				throw new ArgumentNullException("value","Collection does not accept null members");
			}

			ColumnPrompt p=value as ColumnPrompt;
			if(p==null)
			{
				throw new ArgumentException("Collection member must be of type ColumnPrompt","value"); 
			}
				
			Insert(index,p);
			
		}

		public void Insert(int index,ColumnPrompt value)
		{
			if(value==null)
			{
				throw new ArgumentNullException("value","Collection does not accept null members");
			}

			arrList.Insert(index,value); 		
		}


		 
		void IList.Remove(object value)
		{
			if(value==null)
			{
				throw new ArgumentNullException("value","You cannot remove collection member using null reference"); 
			}

			ColumnPrompt p=value as ColumnPrompt;
			if(p==null)
			{
				throw new ArgumentNullException("value","You can remove only an object of type ColumnPrompt"); 
			}

			Remove(p);
		}

		public void Remove(ColumnPrompt value)
		{
			if(value==null)
			{
				throw new ArgumentNullException("value","You cannot remove collection member using null reference"); 
			}
			arrList.Remove(value); 
		}


		bool IList.Contains(object value)
		{
			if(value==null)
			{
				return false;
			}

			ColumnPrompt p=value as ColumnPrompt;
			if(p==null)
			{
				return false;
			}

			return Contains(p);
		}

		public bool Contains(ColumnPrompt value)
		{
			if(value==null)
			{
				return false;
			}

			return arrList.Contains(value); 
		}


		void IList.Clear()
		{
			Clear();
		}

		public void Clear()
		{
			arrList.Clear(); 
		}


		int IList.IndexOf(object value)
		{
			if(value==null)
			{
				return -1;
			}

			ColumnPrompt p=value as ColumnPrompt;
			if(p==null)
			{
				return -1;
			}

			return IndexOf(p);

		}

		public int IndexOf(ColumnPrompt value)
		{
			if(value==null)
			{
				return -1;
			}

			return arrList.IndexOf(value); 
		}



		int IList.Add(object value)
		{
			if(value==null)
			{
				throw new ArgumentNullException("value","You cannot add an object with null reference to the collection"); 
			}

			ColumnPrompt p=value as ColumnPrompt;
			if(p==null)
			{
				throw new ArgumentException("value","You can add only objects of type ColumnPrompt");
			}

			return Add(p);
		}

		public int Add(ColumnPrompt value)
		{
			if(value==null)
			{
				throw new ArgumentNullException("value","You cannot add an object with null reference to the collection"); 
			}

			return arrList.Add(value);
		}



		bool IList.IsFixedSize
		{
			get
			{
				return IsFixedSize; 
			}
		}

		public bool IsFixedSize
		{
			get
			{
				return arrList.IsFixedSize; 
			}
		}

		#endregion

		#region ICollection Members

		//ICollection implementation is delegated straight to the pribate ArrayList member
		public bool IsSynchronized
		{
			get
			{
				return arrList.IsSynchronized; 
			}
		}

		public int Count
		{
			get
			{
				return arrList.Count;
			}
		}

		public void CopyTo(Array array, int index)
		{
			arrList.CopyTo(array,index); 
		}

		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		#endregion

		#region IEnumerable Members
		//IEnumerable implementation is delegated straight to the private ArrayList member
		public IEnumerator GetEnumerator()
		{
			return arrList.GetEnumerator(); 
		}

		#endregion
	}


}
