using System.Collections;

namespace Netty;

class Deque<T>
{
	
	private const int _MinimumGrow = 4;
	private const int _ShrinkThreshold = 32;
	
	private T?[] _array;
	private int _capacity;
	private int _growFactor;
	private int _head;
	private int _tail;
	private int _size;
	
	public virtual int Count => _size;
	public virtual bool HasNext => Count > 0;

	public bool IsReadOnly => throw new NotImplementedException();

	public Deque()
			: this(64, 2.0f)
	{
	}
	
	public Deque(int capacity)
			: this(capacity, 2.0f)
	{
	}
	
	public Deque(int capacity, float growFactor)
	{
		if (capacity <= 0)
			throw new ArgumentOutOfRangeException("Queue capacity can't be negative or 0");
		_array = new T?[capacity];
		_capacity = capacity;
		_growFactor = (int) (growFactor * 100);
	}
	
	public virtual void Clear()
	{
		if (_head < _tail)
			Array.Clear(_array, _head, _size);
		else {
			Array.Clear(_array, _head, _array.Length - _head);
			Array.Clear(_array, 0, _tail);
		}
		_head = 0;
		_tail = 0;
		_size = 0;
	}
	
	/// <summary>
	/// Adds object obj to the beginning of the queue. Count modified.
	/// </summary>
	/// <param name="obj">Object to add</param>
	public virtual void EnqueueFirst(T? obj)
	{
		if (_size == _array.Length) {
			int newCap = (int) ((long) _array.Length * (long) _growFactor / 100);
			if (newCap < _array.Length + _MinimumGrow) {
				newCap = _array.Length + _MinimumGrow;
			}
			T?[] newArray = new T?[newCap];
			if (_size > 0) {
				if (_head < _tail) {
					Array.Copy(_array, _head, newArray, 1, _size);
				} else {
					Array.Copy(_array, _head, newArray, 1, _array.Length - _head);
					Array.Copy(_array, 0, newArray, _array.Length - _head + 1, _tail);
				}
			}
			_array = newArray;
			_head = 0;
			_tail = (_size == newCap) ? 0 : _size;
		}
		_array[_head] = obj;
		_head = (_head + 1) % _array.Length;
		_size++;
	}
	
	/// <summary>
	/// Adds object obj to the end of the queue. Count modified.
	/// </summary>
	/// <param name="obj">Object to add</param>
	public virtual void EnqueueLast(T? obj)
	{
		if (_size == _array.Length) {
			int newCap = (int) ((long) _array.Length * (long) _growFactor / 100);
			if (newCap < _array.Length + _MinimumGrow) {
				newCap = _array.Length + _MinimumGrow;
			}
			T?[] newArray = new T?[newCap];
			if (_size > 0) {
				if (_head < _tail) {
					Array.Copy(_array, _head, newArray, 0, _size);
				} else {
					Array.Copy(_array, _head, newArray, 0, _array.Length - _head);
					Array.Copy(_array, 0, newArray, _array.Length - _head, _tail);
				}
			}
			_array = newArray;
			_head = 0;
			_tail = (_size == newCap) ? 0 : _size;
		}
		
		_array[_tail] = obj;
		_tail = (_tail + 1) % _array.Length;
		_size++;
	}
	
	/// <summary>
	/// Removes an object at the beginning of the queue. Count modified.
	/// </summary>
	/// <returns>The object removed</returns>
	public virtual T? DequeueFirst()
	{
		if (Count == 0)
			throw new IndexOutOfRangeException("Deque is empty");
		T? removed = _array[_head];
		_array[_head] = default(T);
		_head = (_head + 1) % _array.Length;
		_size--;
		return removed;
	}
	
	public virtual T? DequeueLast()
	{
		if (Count == 0)
			throw new IndexOutOfRangeException("Deque is empty");
		T? removed = _array[_tail];
		_array[_tail] = default(T);
		_tail = (_tail + 1) % _array.Length;
		_size--;
		return removed;
	}
	
	public virtual T? Peek()
	{
		if (Count == 0)
			throw new IndexOutOfRangeException("Deque is empty");
		return _array[_head];
	}
	
	public virtual bool Contains(T obj)
	{
		int index = _head;
		int count = _size;
		while (count-- > 0) {
			if (obj == null) {
				if (_array[index] == null)
					return true;
			} else if (_array[index] != null && _array[index]!.Equals(obj)) {
				return true;
			}
			index = (index + 1) % _array.Length;
		}
		return false;
	}
	
	public virtual object[] ToArray()
	{
		object[] arr = new object[_size];
		if (_size == 0)
			return arr;
		
		if (_head < _tail) {
			Array.Copy(_array, _head, arr, 0, _size);
		} else {
			Array.Copy(_array, _head, arr, 0, _array.Length - _head);
			Array.Copy(_array, 0, arr, _array.Length - _head, _tail);
		}
		return arr;
	}

}