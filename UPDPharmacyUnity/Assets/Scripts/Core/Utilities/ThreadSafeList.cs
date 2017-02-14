using System.Collections.Generic;

public class ThreadSafeList<T> 
{

    private object _lock = new object();
    private List<T> _list = new List<T>();
	
	public int Count
	{
		get {	
			lock(_lock)
			{
				return _list.Count;	
			}
		}
	}
	
    public void Add(T item) 
    {
        lock(_lock) 
        {
            _list.Add(item);
        }
    }	

    public bool Remove(T item)
    {
        lock(_lock)
        {
            return _list.Remove(item);
        }
    }
	
	public void Enqueue(T item)
	{
		lock(_lock)
		{
			_list.Add(item);	
		}
	}
	
	public T Dequeue()
	{
		lock(_lock) 
		{
			if (_list.Count > 0) {
				T head = _list[0];
				_list.RemoveAt(0);
				return head;
			}
			else {
				return default(T);	
			}
		}
	}

    public List<T>.Enumerator GetEnumerator()
    {
        lock (_lock)
        {
            return _list.GetEnumerator();
        }
    }
}
