using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;

namespace System.Threading
{
    /// <summary>
    /// Represents a threaded List
    /// </summary>
    /// <typeparam name="T"> </typeparam>
    public class ListDoubleThreadSafe<T1,T2>
    {
        #region Variables

        /// <summary>
        /// The container list that holds the actual data
        /// </summary>
        //private readonly List<T> m_TList;
        private readonly List<T1> m_TList1;
        private readonly List<T2> m_TList2;

        /// <summary>
        /// The lock used when accessing the list
        /// </summary>
        private readonly ReaderWriterLockSlim LockList = new ReaderWriterLockSlim(); 

        // Variables
        #endregion

        #region Properties

        /// <summary>
        /// When true, we have already disposed of the object
        /// </summary>
        private int m_Disposed;

        /// <summary>
        /// When true, we have already disposed of the object
        /// </summary>
        private bool Disposed
        {
            get { return Thread.VolatileRead(ref m_Disposed) == 1; }
            set { Thread.VolatileWrite(ref m_Disposed, value ? 1 : 0); }
        }

        // Properties
        #endregion

        #region Init

        /// <summary>
        /// Creates an empty threaded list object
        /// </summary>
        public ListDoubleThreadSafe()
        {
            m_TList1 = new List<T1>();
            m_TList2 = new List<T2>();
        }

        /// <summary>
        /// Creates an empty threaded list object
        /// </summary>
        /// <param name="capacity">the number of elements the list can initially store</param>
        public ListDoubleThreadSafe(int capacity)
        {
            m_TList1 = new List<T1>(capacity);
            m_TList2 = new List<T2>(capacity);
        }

        /// <summary>
        /// Creates an empty threaded list object
        /// </summary>
        /// <param name="collection">a collection of objects which are copied into the threaded list</param>
        public ListDoubleThreadSafe(IEnumerable<T1> collection1, IEnumerable<T2> collection2)
        {
            m_TList1 = new List<T1>(collection1);
            m_TList2 = new List<T2>(collection2);
        }

        // Init
        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator to iterate through the collection
        /// </summary>
        public IEnumerator GetEnumerator()
        {
            // http://www.csharphelp.com/archives/archive181.html

            List<T1> localList1;
            List<T2> localList2;

            // init enumerator
            LockList.EnterReadLock();
            try
            {
                // create a copy of m_TList
                localList1 = new List<T1>(m_TList1);
                localList2 = new List<T2>(m_TList2);
            }
            finally
            {
                LockList.ExitReadLock();
            }

            // get the enumerator
            for (int i = 0; i < localList1.Count; i++) {
                yield return new Tuple<T1, T2>(localList1[i], localList2[i]);
            }
        }

        /// <summary>
        /// Returns an enumerator to iterate through the collection
        /// </summary>
        public IEnumerator GetEnumerator1()
        {
            // http://www.csharphelp.com/archives/archive181.html

            List<T1> localList1;

            // init enumerator
            LockList.EnterReadLock();
            try
            {
                // create a copy of m_TList
                localList1 = new List<T1>(m_TList1);
            }
            finally
            {
                LockList.ExitReadLock();
            }

            // get the enumerator
            foreach (T1 item in localList1)
                yield return item;
        }

        /// <summary>
        /// Returns an enumerator to iterate through the collection
        /// </summary>
        public IEnumerator GetEnumerator2()
        {
            // http://www.csharphelp.com/archives/archive181.html

            List<T2> localList2;

            // init enumerator
            LockList.EnterReadLock();
            try
            {
                // create a copy of m_TList
                localList2 = new List<T2>(m_TList2);
            }
            finally
            {
                LockList.ExitReadLock();
            }

            // get the enumerator
            foreach (T2 item in localList2)
                yield return item;
        }


        #endregion

        #region IEnumerable<T> Members

        /// <summary>
        /// Returns an enumerator to iterate through the collection
        /// </summary>
        IEnumerator<Tuple<T1,T2>> GetEnumeratorTuple()
        {
            // http://www.csharphelp.com/archives/archive181.html

            List<T1> localList1;
            List<T2> localList2;

            // init enumerator
            LockList.EnterReadLock();
            try
            {
                // create a copy of m_TList
                localList1 = new List<T1>(m_TList1);
                localList2 = new List<T2>(m_TList2);
            }
            finally
            {
                LockList.ExitReadLock();
            }

            // get the enumerator
            for(int i = 0; i < localList1.Count; i++)
                yield return new Tuple<T1, T2>(localList1[i], localList2[i]);
        }

        /// <summary>
        /// Returns an enumerator to iterate through the collection
        /// </summary>
        IEnumerator<T1> GetEnumeratorT1()
        {
            // http://www.csharphelp.com/archives/archive181.html

            List<T1> localList1;

            // init enumerator
            LockList.EnterReadLock();
            try
            {
                // create a copy of m_TList
                localList1 = new List<T1>(m_TList1);
            }
            finally
            {
                LockList.ExitReadLock();
            }

            // get the enumerator
            foreach (T1 item in localList1)
                yield return item;
        }

        /// <summary>
        /// Returns an enumerator to iterate through the collection
        /// </summary>
        IEnumerator<T2> GetEnumeratorT2()
        {
            // http://www.csharphelp.com/archives/archive181.html

            List<T2> localList2;

            // init enumerator
            LockList.EnterReadLock();
            try
            {
                // create a copy of m_TList
                localList2 = new List<T2>(m_TList2);
            }
            finally
            {
                LockList.ExitReadLock();
            }

            // get the enumerator
            foreach (T2 item in localList2)
                yield return item;
        }

        #endregion

        #region IDisposable Members

        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.

        /// <summary>
        /// Dispose of the object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.

        /// <summary>
        /// The disposer
        /// </summary>
        /// <param name="disposing">true if disposed</param>
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (this.Disposed) return;

            // If disposing equals true, dispose all managed
            // and unmanaged resources.
            //if(disposing)
            //{
            //    // NO managed resources to dispose for this object
            //}

            // Note disposing has been done.
            Disposed = true;
        }

        // Use C# destructor syntax for finalization code.
        // This destructor will run only if the Dispose method
        // does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide destructors in types derived from this class.

        /// <summary>
        /// Finalizes an instance of the <see cref="ListThreadSafe{T}"/> class. 
        /// </summary>
        ~ListDoubleThreadSafe()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }

        #endregion

        #region Add

        /// <summary>
        /// Adds an item to the threaded list
        /// </summary>
        /// <param name="item">the item to add to the end of the collection</param>
        public void Add(T1 item1, T2 item2)
        {
            LockList.EnterWriteLock();
            try
            {
                m_TList1.Add(item1);
                m_TList2.Add(item2);
            }

            finally
            {
                LockList.ExitWriteLock();
            }
        }

        // Add
        #endregion

        #region AddRange

        public bool AddRange(Tuple<T1,T2>[] items)
        {
            if (items == null || items.Length == 0) return false;
            LockList.EnterWriteLock();
            try
            {
                m_TList1.AddRange(items.Select(x=>x.Item1));
                m_TList2.AddRange(items.Select(x=>x.Item2));
            }
            finally
            {
                LockList.ExitWriteLock();
            }
            return true;
        }

        /// <summary>
        /// Adds the elements of collection to the end of the threaded list
        /// </summary>
        /// <param name="collection">the collection to add to the end of the list</param>
        public bool AddRange(IEnumerable<T1> collection1, IEnumerable<T2> collection2)
        {
            if (collection1.Count() != collection2.Count()) return false;

            LockList.EnterWriteLock();
            try
            {
                m_TList1.AddRange(collection1);
                m_TList2.AddRange(collection2);
            }

            finally
            {
                LockList.ExitWriteLock();
            }
            return true;
        }

        // AddRange
        #endregion

        #region AddIfNotExist

        /// <summary>
        /// Adds an item to the threaded list if it is not already in the list.
        /// Returns true if added to the list, false if the item already existed
        /// in the list
        /// </summary>
        /// <param name="item">the item to add to the end of the collection</param>
        public bool AddIfNotExistItem1AndItem2(T1 item1, T2 item2)
        {
            LockList.EnterWriteLock();
            try
            {
                // check if it exists already
                if (m_TList1.Contains(item1) && m_TList2.Contains(item2))
                    return false;

                // add the item and return true
                m_TList1.Add(item1);
                m_TList2.Add(item2);

                return true;
            }

            finally
            {
                LockList.ExitWriteLock();
            }
        }

        /// <summary>
        /// Adds an item to the threaded list if it is not already in the list.
        /// Returns true if added to the list, false if the item already existed
        /// in the list
        /// </summary>
        /// <param name="item">the item to add to the end of the collection</param>
        public bool AddIfNotExistItem1OrItem2(T1 item1, T2 item2)
        {
            LockList.EnterWriteLock();
            try
            {
                // check if it exists already
                if (m_TList1.Contains(item1) || m_TList2.Contains(item2))
                    return false;

                // add the item and return true
                m_TList1.Add(item1);
                m_TList2.Add(item2);

                return true;
            }

            finally
            {
                LockList.ExitWriteLock();
            }
        }

        /// <summary>
        /// Adds an item to the threaded list if it is not already in the list.
        /// Returns true if added to the list, false if the item already existed
        /// in the list
        /// </summary>
        /// <param name="item">the item to add to the end of the collection</param>
        public bool AddIfNotExistItem1(T1 item1, T2 item2)
        {
            LockList.EnterWriteLock();
            try
            {
                // check if it exists already
                if (m_TList1.Contains(item1))
                    return false;

                // add the item and return true
                m_TList1.Add(item1);
                m_TList2.Add(item2);

                return true;
            }

            finally
            {
                LockList.ExitWriteLock();
            }
        }

        /// <summary>
        /// Adds an item to the threaded list if it is not already in the list.
        /// Returns true if added to the list, false if the item already existed
        /// in the list
        /// </summary>
        /// <param name="item">the item to add to the end of the collection</param>
        public bool AddIfNotExistItem2(T1 item1, T2 item2)
        {
            LockList.EnterWriteLock();
            try
            {
                // check if it exists already
                if (m_TList2.Contains(item2))
                    return false;

                // add the item and return true
                m_TList1.Add(item1);
                m_TList2.Add(item2);

                return true;
            }

            finally
            {
                LockList.ExitWriteLock();
            }
        }

        // AddIfNotExist
        #endregion

        #region AsReadOnly

        /// <summary>
        /// Returns a read-only collection of the current threaded list
        /// </summary>
        public ReadOnlyCollection<T1> AsReadOnly1()
        {
            LockList.EnterReadLock();
            try
            {
                return m_TList1.AsReadOnly();
            }

            finally
            {
                LockList.ExitReadLock();
            }
        }

        /// <summary>
        /// Returns a read-only collection of the current threaded list
        /// </summary>
        public ReadOnlyCollection<T2> AsReadOnly2()
        {
            LockList.EnterReadLock();
            try
            {
                return m_TList2.AsReadOnly();
            }

            finally
            {
                LockList.ExitReadLock();
            }
        }

        // AsReadOnly
        #endregion

        #region BinarySearch

        /// <summary>
        /// Searches the collection using the default comparator and returns the zero-based index of the item found
        /// </summary>
        /// <param name="item">the item to search for</param>
        public Tuple<T1, T2> BinarySearch1(T1 item1)
        {
            LockList.EnterReadLock();
            try
            {
                int id = m_TList1.BinarySearch(item1);
                if (id != -1)
                    return new Tuple<T1, T2>(m_TList1[id], m_TList2[id]);
            }
            finally
            {
                LockList.ExitReadLock();
            }
            return null;
        }

        /// <summary>
        /// Searches the collection using the default comparator and returns the zero-based index of the item found
        /// </summary>
        /// <param name="item">the item to search for</param>
        public Tuple<T1, T2> BinarySearch2(T2 item2)
        {
            LockList.EnterReadLock();
            try
            {
                int id = m_TList2.BinarySearch(item2);
                if (id != -1)
                    return new Tuple<T1, T2>(m_TList1[id], m_TList2[id]);
            }
            finally
            {
                LockList.ExitReadLock();
            }
            return null;
        }

        /// <summary>
        /// Searches the collection using the default comparator and returns the zero-based index of the item found
        /// </summary>
        /// <param name="item">the item to search for</param>
        /// <param name="comparer">the IComparer to use when searching, or null to use the default</param>
        public Tuple<T1, T2> BinarySearch1(T1 item1, IComparer<T1> comparer1)
        {
            LockList.EnterReadLock();
            try
            {
                int id = m_TList1.BinarySearch(item1, comparer1);
                if (id != -1)
                    return new Tuple<T1, T2>(m_TList1[id], m_TList2[id]);
            }
            finally
            {
                LockList.ExitReadLock();
            }
            return null;
        }

        /// <summary>
        /// Searches the collection using the default comparator and returns the zero-based index of the item found
        /// </summary>
        /// <param name="item">the item to search for</param>
        /// <param name="comparer">the IComparer to use when searching, or null to use the default</param>
        public Tuple<T1, T2> BinarySearch2(T2 item2, IComparer<T2> comparer2)
        {
            LockList.EnterReadLock();
            try
            {
                int id = m_TList2.BinarySearch(item2, comparer2);
                if (id != -1)
                    return new Tuple<T1, T2>(m_TList1[id], m_TList2[id]);
            }
            finally
            {
                LockList.ExitReadLock();
            }
            return null;
        }

        /// <summary>
        /// Searches the collection using the default comparator and returns the zero-based index of the item found
        /// </summary>
        /// <param name="index">the zero-based index to start searching from</param>
        /// <param name="count">the number of records to search</param>
        /// <param name="item">the item to search for</param>
        /// <param name="comparer">the IComparer to use when searching, or null to use the default</param>
        public Tuple<T1, T2> BinarySearch1(int index, int count, T1 item1, IComparer<T1> comparer1)
        {
            LockList.EnterReadLock();
            try
            {
                int id = m_TList1.BinarySearch(index, count, item1, comparer1);
                if (id != -1)
                    return new Tuple<T1, T2>(m_TList1[id], m_TList2[id]);
            }
            finally
            {
                LockList.ExitReadLock();
            }
            return null;
        }

        // BinarySearch
        #endregion

        #region Capacity

        /// <summary>
        /// Gets or sets the initial capacity of the list
        /// </summary>
        public int Capacity
        {
            get
            {
                LockList.EnterReadLock();
                try
                {
                    return m_TList1.Capacity;
                }

                finally
                {
                    LockList.ExitReadLock();
                }
            }
            set
            {
                LockList.EnterWriteLock();
                try
                {
                    m_TList1.Capacity = value;
                }

                finally
                {
                    LockList.ExitWriteLock();
                }
            }
        }

        // Capacity
        #endregion

        #region Clear

        /// <summary>
        /// Removes all items from the threaded list
        /// </summary>
        public void Clear()
        {
            LockList.EnterReadLock();
            try
            {
                m_TList1.Clear();
                m_TList2.Clear();
            }

            finally
            {
                LockList.ExitReadLock();
            }
        }

        // Clear
        #endregion

        #region Contains

        /// <summary>
        /// Returns true if the collection contains this item
        /// </summary>
        /// <param name="item">the item to find in the collection</param>
        public bool Contains1(T1 item1)
        {
            LockList.EnterReadLock();
            try
            {
                return m_TList1.Contains(item1);
            }

            finally
            {
                LockList.ExitReadLock();
            }
        }

        /// <summary>
        /// Returns true if the collection contains this item
        /// </summary>
        /// <param name="item">the item to find in the collection</param>
        public bool Contains2(T2 item2)
        {
            LockList.EnterReadLock();
            try
            {
                return m_TList2.Contains(item2);
            }

            finally
            {
                LockList.ExitReadLock();
            }
        }

        /// <summary>
        /// Returns true if the collection contains this item
        /// </summary>
        /// <param name="item">the item to find in the collection</param>
        public bool ContainsItem1AndItem2(T1 item1, T2 item2)
        {
            LockList.EnterReadLock();
            try
            {
                return m_TList1.Contains(item1) && m_TList2.Contains(item2);
            }
            finally
            {
                LockList.ExitReadLock();
            }
        }

        /// <summary>
        /// Returns true if the collection contains this item
        /// </summary>
        /// <param name="item">the item to find in the collection</param>
        public bool ContainsItem1OrItem2(T1 item1, T2 item2)
        {
            LockList.EnterReadLock();
            try
            {
                return m_TList1.Contains(item1) || m_TList2.Contains(item2);
            }
            finally
            {
                LockList.ExitReadLock();
            }
        }

        // Contains
        #endregion

        #region ConvertAll

        /// <summary>
        /// Converts the elements of the threaded list to another type, and returns a list of the new type
        /// </summary>
        /// <typeparam name="TOutput">the destination type</typeparam>
        /// <param name="converter">delegate to convert the items to a new type</param>
        public List<TOutput> ConvertAll1<TOutput>(Converter<T1, TOutput> converter1)
        {
            LockList.EnterReadLock();
            try
            {
                return m_TList1.ConvertAll(converter1);
            }

            finally
            {
                LockList.ExitReadLock();
            }
        }

        /// <summary>
        /// Converts the elements of the threaded list to another type, and returns a list of the new type
        /// </summary>
        /// <typeparam name="TOutput">the destination type</typeparam>
        /// <param name="converter">delegate to convert the items to a new type</param>
        public List<TOutput> ConvertAll2<TOutput>(Converter<T2, TOutput> converter2)
        {
            LockList.EnterReadLock();
            try
            {
                return m_TList2.ConvertAll(converter2);
            }

            finally
            {
                LockList.ExitReadLock();
            }
        }
        // ConvertAll
        #endregion

        #region CopyTo

        /// <summary>
        /// Copies the elements of this threaded list to a one-dimension array of the same type
        /// </summary>
        /// <param name="array">the destination array</param>
        /// <param name="arrayIndex">index at which copying begins</param>
        public void CopyTo1(T1[] array1, int arrayIndex1)
        {
            LockList.EnterReadLock();
            try
            {
                m_TList1.CopyTo(array1, arrayIndex1);
            }

            finally
            {
                LockList.ExitReadLock();
            }
        }

        /// <summary>
        /// Copies the elements of this threaded list to a one-dimension array of the same type
        /// </summary>
        /// <param name="array">the destination array</param>
        /// <param name="arrayIndex">index at which copying begins</param>
        public void CopyTo2(T2[] array2, int arrayIndex2)
        {
            LockList.EnterReadLock();
            try
            {
                m_TList2.CopyTo(array2, arrayIndex2);
            }

            finally
            {
                LockList.ExitReadLock();
            }
        }

        // CopyTo
        #endregion

        #region Count

        /// <summary>
        /// Returns a count of the number of elements in this collection
        /// </summary>
        public int Count
        {
            get
            {
                LockList.EnterReadLock();
                try
                {
                    return m_TList1.Count;
                }

                finally
                {
                    LockList.ExitReadLock();
                }
            }
        }

        // Count
        #endregion

        #region Exists

        /// <summary>
        /// Determines whether an item exists which meets the match criteria
        /// </summary>
        /// <param name="match">delegate that defines the conditions to search for</param>
        public bool Exists1(Predicate<T1> match1)
        {
            LockList.EnterReadLock();
            try
            {
                return m_TList1.Exists(match1);
            }
            finally
            {
                LockList.ExitReadLock();
            }
        }

        /// <summary>
        /// Determines whether an item exists which meets the match criteria
        /// </summary>
        /// <param name="match">delegate that defines the conditions to search for</param>
        public bool Exists2(Predicate<T2> match2)
        {
            LockList.EnterReadLock();
            try
            {
                return m_TList2.Exists(match2);
            }
            finally
            {
                LockList.ExitReadLock();
            }
        }

        /// <summary>
        /// Determines whether an item exists which meets the match criteria
        /// </summary>
        /// <param name="match">delegate that defines the conditions to search for</param>
        public bool ExistsItem1AndItem2(Predicate<T1> match1, Predicate<T2> match2)
        {
            LockList.EnterReadLock();
            try
            {
                return m_TList1.Exists(match1) && m_TList2.Exists(match2);
            }
            finally
            {
                LockList.ExitReadLock();
            }
        }

        /// <summary>
        /// Determines whether an item exists which meets the match criteria
        /// </summary>
        /// <param name="match">delegate that defines the conditions to search for</param>
        public bool ExistsItem1OrItem2(Predicate<T1> match1, Predicate<T2> match2)
        {
            LockList.EnterReadLock();
            try
            {
                return m_TList1.Exists(match1) || m_TList2.Exists(match2);
            }
            finally
            {
                LockList.ExitReadLock();
            }
        }

        // Exists
        #endregion

        #region Find

        /// <summary>
        /// Searches for an element that matches the criteria
        /// </summary>
        /// <param name="match">delegate that defines the conditions to search for</param>
        public T1 Find1(Predicate<T1> match1)
        {
            LockList.EnterReadLock();
            try
            {
                return m_TList1.Find(match1);
            }
            finally
            {
                LockList.ExitReadLock();
            }
        }

        /// <summary>
        /// Searches for an element that matches the criteria
        /// </summary>
        /// <param name="match">delegate that defines the conditions to search for</param>
        public T2 Find2(Predicate<T2> match2)
        {
            LockList.EnterReadLock();
            try
            {
                return m_TList2.Find(match2);
            }
            finally
            {
                LockList.ExitReadLock();
            }
        }

        /// <summary>
        /// Searches for an element that matches the criteria
        /// </summary>
        /// <param name="match">delegate that defines the conditions to search for</param>
        public Tuple<T1,T2> Find1Tuple(Predicate<T1> match1)
        {
            LockList.EnterReadLock();
            try
            {
                T1 v1 = m_TList1.Find(match1);
                int id = m_TList1.IndexOf(v1);
                if (id != -1)
                    return new Tuple<T1, T2>(v1, m_TList2[id]);
            }
            finally
            {
                LockList.ExitReadLock();
            }
            return null;
        }

        /// <summary>
        /// Searches for an element that matches the criteria
        /// </summary>
        /// <param name="match">delegate that defines the conditions to search for</param>
        public Tuple<T1, T2> Find2Tuple(Predicate<T2> match2)
        {
            LockList.EnterReadLock();
            try
            {
                T2 v2 = m_TList2.Find(match2);
                int id = m_TList2.IndexOf(v2);
                if (id != -1)
                    return new Tuple<T1, T2>(m_TList1[id], v2);
            }
            finally
            {
                LockList.ExitReadLock();
            }
            return null;
        }

        public T2[] FindItem1LessThanAndRemove(Func<List<T1>, T1, int[]> func, T1 valueCompareLessThan)
        {
            LockList.EnterWriteLock();
            try
            {
                int[] ids = func(m_TList1, valueCompareLessThan);
                if (ids.Length > 0) {
                    List<T2> ls = new List<T2>();
                    for (int i = 0; i < ids.Length; i++)
                        ls.Add(m_TList2[ids[i]]);

                    ids = ids.OrderByDescending(x => x).ToArray();
                    for (int i = 0; i < ids.Length; i++)
                    {
                        m_TList1.RemoveAt(ids[i]);
                        m_TList2.RemoveAt(ids[i]);
                    }
                    return ls.ToArray();
                }
            }
            finally
            {
                LockList.ExitWriteLock();
            }
            return new T2[] { };
        }
        // Find
        #endregion

        #region FindAll

        /// <summary>
        /// Searches for elements that match the criteria
        /// </summary>
        /// <param name="match">delegate that defines the conditions to search for</param>
        public List<T1> FindAll1(Predicate<T1> match1)
        {
            LockList.EnterReadLock();
            try
            {
                return m_TList1.FindAll(match1);
            }
            finally
            {
                LockList.ExitReadLock();
            }
        }

        /// <summary>
        /// Searches for elements that match the criteria
        /// </summary>
        /// <param name="match">delegate that defines the conditions to search for</param>
        public List<T2> FindAll2(Predicate<T2> match2)
        {
            LockList.EnterReadLock();
            try
            {
                return m_TList2.FindAll(match2);
            }
            finally
            {
                LockList.ExitReadLock();
            }
        }
         

        // FindAll
        #endregion

        #region FindIndex

        /// <summary>
        /// Returns the index of the element which matches the criteria
        /// </summary>
        /// <param name="match">delegate that defines the conditions to search for</param>
        public int FindIndex1(Predicate<T1> match1)
        {
            LockList.EnterReadLock();
            try
            {
                return m_TList1.FindIndex(match1);
            }
            finally
            {
                LockList.ExitReadLock();
            }
        }

        /// <summary>
        /// Returns the index of the element which matches the criteria
        /// </summary>
        /// <param name="startIndex">the zero-based index starting the search</param>
        /// <param name="match">delegate that defines the conditions to search for</param>
        public int FindIndex1(int startIndex, Predicate<T1> match1)
        {
            LockList.EnterReadLock();
            try
            {
                return m_TList1.FindIndex(startIndex, match1);
            }
            finally
            {
                LockList.ExitReadLock();
            }
        }

        /// <summary>
        /// Returns the index of the element which matches the criteria
        /// </summary>
        /// <param name="startIndex">the zero-based index starting the search</param>
        /// <param name="count">the number of elements to search</param>
        /// <param name="match">delegate that defines the conditions to search for</param>
        public int FindIndex1(int startIndex, int count, Predicate<T1> match1)
        {
            LockList.EnterReadLock();
            try
            {
                return m_TList1.FindIndex(startIndex, count, match1);
            }
            finally
            {
                LockList.ExitReadLock();
            }
        }

        // FindIndex
        #endregion

        #region FindLast

        /// <summary>
        /// Searches for the last element in the collection that matches the criteria
        /// </summary>
        /// <param name="match">delegate that defines the conditions to search for</param>
        public T1 FindLast(Predicate<T1> match1)
        {
            LockList.EnterReadLock();
            try
            {
                return m_TList1.FindLast(match1);
            }
            finally
            {
                LockList.ExitReadLock();
            }
        }

        // FindLast
        #endregion

        #region FindLastIndex

        /// <summary>
        /// Returns the last index of the element which matches the criteria
        /// </summary>
        /// <param name="match">delegate that defines the conditions to search for</param>
        public int FindLastIndex1(Predicate<T1> match1)
        {
            LockList.EnterReadLock();
            try
            {
                return m_TList1.FindLastIndex(match1);
            }
            finally
            {
                LockList.ExitReadLock();
            }
        }

        /// <summary>
        /// Returns the last index of the element which matches the criteria
        /// </summary>
        /// <param name="startIndex">the zero-based index starting the search</param>
        /// <param name="match">delegate that defines the conditions to search for</param>
        public int FindLastIndex1(int startIndex, Predicate<T1> match1)
        {
            LockList.EnterReadLock();
            try
            {
                return m_TList1.FindLastIndex(startIndex, match1);
            }
            finally
            {
                LockList.ExitReadLock();
            }
        }

        /// <summary>
        /// Returns the last index of the element which matches the criteria
        /// </summary>
        /// <param name="startIndex">the zero-based index starting the search</param>
        /// <param name="count">the number of elements to search</param>
        /// <param name="match">delegate that defines the conditions to search for</param>
        public int FindLastIndex1(int startIndex, int count, Predicate<T1> match1)
        {
            LockList.EnterReadLock();
            try
            {
                return m_TList1.FindLastIndex(startIndex, count, match1);
            }
            finally
            {
                LockList.ExitReadLock();
            }
        }

        // FindLastIndex
        #endregion

        #region ForEach

        /// <summary>
        /// Peforms the action on each element of the list
        /// </summary>
        /// <param name="action">the action to perfom</param>
        public void ForEach1(Action<T1> action1)
        {
            LockList.EnterWriteLock();
            try
            {
                m_TList1.ForEach(action1);
            }
            finally
            {
                LockList.ExitWriteLock();
            }
        }

        // ForEach
        #endregion
            
        /// <summary>
        /// Creates a shallow copy of the range of elements in the source
        /// </summary>
        /// <param name="index">index to start from</param>
        /// <param name="count">number of elements to return</param>
        /// <returns></returns>
        public List<T1> GetRange(int index, int count)
        {
            LockList.EnterReadLock();
            try
            {
                return m_TList1.GetRange(index, count);
            }
            finally
            {
                LockList.ExitReadLock();
            }
        }

        /// <summary>
        /// Searches the list and returns the index of the item found in the list
        /// </summary>
        /// <param name="item">the item to find</param>
        public int IndexOf1(T1 item1)
        {
            LockList.EnterReadLock();
            try
            {
                return m_TList1.IndexOf(item1);
            }
            finally
            {
                LockList.ExitReadLock();
            }
        }

        /// <summary>
        /// Searches the list and returns the index of the item found in the list
        /// </summary>
        /// <param name="item">the item to find</param>
        /// <param name="index">the zero-based index to begin searching from</param>
        public int IndexOf1(T1 item1, int index)
        {
            LockList.EnterReadLock();
            try
            {
                return m_TList1.IndexOf(item1, index);
            }
            finally
            {
                LockList.ExitReadLock();
            }
        }

        /// <summary>
        /// Searches the list and returns the index of the item found in the list
        /// </summary>
        /// <param name="item">the item to find</param>
        /// <param name="index">the zero-based index to begin searching from</param>
        /// <param name="count">the number of elements to search</param>
        public int IndexOf1(T1 item1, int index, int count)
        {
            LockList.EnterReadLock();
            try
            {
                return m_TList1.IndexOf(item1, index, count);
            }
            finally
            {
                LockList.ExitReadLock();
            }
        }

        /// <summary>
        /// Inserts the item into the list
        /// </summary>
        /// <param name="index">the index at which to insert the item</param>
        /// <param name="item">the item to insert</param>
        public void Insert1(int index, T1 item1)
        {
            LockList.ExitWriteLock();
            try
            {
                m_TList1.Insert(index, item1);
            }
            finally
            {
                LockList.ExitWriteLock();
            }
        }

        /// <summary>
        /// Insert a range of objects into the list
        /// </summary>
        /// <param name="index">index to insert at</param>
        /// <param name="range">range of values to insert</param>
        public void InsertRange1(int index, IEnumerable<T1> range1)
        {
            LockList.EnterWriteLock();
            try
            {
                m_TList1.InsertRange(index, range1);
            }
            finally
            {
                LockList.ExitWriteLock();
            }
        }

        /// <summary>
        /// Always false
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Returns the last index of the item in the list
        /// </summary>
        /// <param name="item">the item to find</param>
        public int LastIndexOf1(T1 item1)
        {
            LockList.EnterReadLock();
            try
            {
                return m_TList1.LastIndexOf(item1);
            }
            finally
            {
                LockList.ExitReadLock();
            }
        }

        /// <summary>
        /// Returns the last index of the item in the list
        /// </summary>
        /// <param name="item">the item to find</param>
        /// <param name="index">the index at which to start searching</param>
        public int LastIndexOf1(T1 item1, int index)
        {
            LockList.EnterReadLock();
            try
            {
                return m_TList1.LastIndexOf(item1, index);
            }
            finally
            {
                LockList.ExitReadLock();
            }
        }

        /// <summary>
        /// Returns the last index of the item in the list
        /// </summary>
        /// <param name="item">the item to find</param>
        /// <param name="index">the index at which to start searching</param>
        /// <param name="count">number of elements to search</param>
        public int LastIndexOf1(T1 item1, int index, int count)
        {
            LockList.EnterReadLock();
            try
            {
                return m_TList1.LastIndexOf(item1, index, count);
            }
            finally
            {
                LockList.ExitReadLock();
            }
        }

        public bool RemoveItem1AndItem2_byItem2(T2 item2)
        {
            LockList.EnterWriteLock();
            try
            {
                int id = m_TList2.IndexOf(item2);
                if (id != -1) {
                    m_TList2.RemoveAt(id);
                    m_TList1.RemoveAt(id);
                    return true;
                }
            }
            finally
            {
                LockList.ExitWriteLock();
            }
            return false;
        }

        /// <summary>
        /// Removes this item from the list
        /// </summary>
        /// <param name="item">the item to remove</param>
        public bool Remove1(T1 item1)
        {
            LockList.EnterWriteLock();
            try
            {
                return m_TList1.Remove(item1);
            }
            finally
            {
                LockList.ExitWriteLock();
            }
        }

        /// <summary>
        /// Removes all the matching items from the list
        /// </summary>
        /// <param name="match">the pattern to search on</param>
        public int RemoveAll1(Predicate<T1> match1)
        {
            LockList.EnterWriteLock();
            try
            {
                return m_TList1.RemoveAll(match1);
            }
            finally
            {
                LockList.ExitWriteLock();
            }
        }

        /// <summary>
        /// Removes the item at the specified index
        /// </summary>
        /// <param name="index">the index of the item to remove</param>
        public void RemoveAt1(int index)
        {
            LockList.EnterWriteLock();
            try
            {
                m_TList1.RemoveAt(index);
            }
            finally
            {
                LockList.ExitWriteLock();
            }
        }

        /// <summary>
        /// Removes the items from the list
        /// </summary>
        /// <param name="index">the index of the item to begin removing</param>
        /// <param name="count">the number of items to remove</param>
        public void RemoveRange1(int index, int count)
        {
            LockList.EnterWriteLock();
            try
            {
                m_TList1.RemoveRange(index, count);
            }
            finally
            {
                LockList.ExitWriteLock();
            }
        }

        /// <summary>
        /// Reverses the order of elements in the list
        /// </summary>
        public void Reverse1()
        {
            LockList.EnterWriteLock();
            try
            {
                m_TList1.Reverse();
            }
            finally
            {
                LockList.ExitWriteLock();
            }
        }

        /// <summary>
        /// Reverses the order of elements in the list
        /// </summary>
        /// <param name="index">the index to begin reversing at</param>
        /// <param name="count">the number of elements to reverse</param>
        public void Reverse1(int index, int count)
        {
            LockList.EnterWriteLock();
            try
            {
                m_TList1.Reverse(index, count);
            }
            finally
            {
                LockList.ExitWriteLock();
            }
        }

        /// <summary>
        /// Sorts the items in the list
        /// </summary>
        public void Sort1()
        {
            LockList.EnterWriteLock();
            try
            {
                m_TList1.Sort();
            }
            finally
            {
                LockList.ExitWriteLock();
            }
        }

        /// <summary>
        /// Sorts the items in the list
        /// </summary>
        /// <param name="comparison">the comparison to use when comparing elements</param>
        public void Sort1(Comparison<T1> comparison1)
        {
            LockList.EnterWriteLock();
            try
            {
                m_TList1.Sort(comparison1);
            }
            finally
            {
                LockList.ExitWriteLock();
            }
        }

        /// <summary>
        /// Sorts the items in the list
        /// </summary>
        /// <param name="comparer">the comparer to use when comparing elements</param>
        public void Sort1(IComparer<T1> comparer1)
        {
            LockList.EnterWriteLock();
            try
            {
                m_TList1.Sort(comparer1);
            }
            finally
            {
                LockList.ExitWriteLock();
            }
        }

        /// <summary>
        /// Sorts the items in the list
        /// </summary>
        /// <param name="index">the index to begin sorting at</param>
        /// <param name="count">the number of elements to sort</param>
        /// <param name="comparer">the comparer to use when sorting</param>
        public void Sort1(int index, int count, IComparer<T1> comparer1)
        {
            LockList.EnterWriteLock();
            try
            {
                m_TList1.Sort(index, count, comparer1);
            }
            finally
            {
                LockList.ExitWriteLock();
            }
        }

        /// <summary>
        /// Copies the elements of the list to an array
        /// </summary>
        public T1[] ToArray(bool clearAfterThat = false)
        {
            if (clearAfterThat)
                LockList.EnterWriteLock();
            else
                LockList.EnterReadLock();
            try
            {
                T1[] a = m_TList1.ToArray();
                if (clearAfterThat) m_TList1.Clear();
                return a;
            }
            finally
            {
                if (clearAfterThat)
                    LockList.ExitWriteLock();
                else
                    LockList.ExitReadLock();
            }
        }

        /// <summary>
        /// Sets the capacity to the actual number of elements in the list, if that 
        /// number is less than the threshold
        /// </summary>
        public void TrimExcess1()
        {
            LockList.EnterWriteLock();
            try
            {
                m_TList1.TrimExcess();
            }
            finally
            {
                LockList.ExitWriteLock();
            }
        }

        /// <summary>
        /// Determines whether all members of the list matches the conditions in the predicate
        /// </summary>
        /// <param name="match">the delegate which defines the conditions for the search</param>
        public bool TrueForAll(Predicate<T1> match1)
        {
            LockList.EnterWriteLock();
            try
            {
                return m_TList1.TrueForAll(match1);
            }
            finally
            {
                LockList.ExitWriteLock();
            }
        }

        #region IList<T> Members

        /// <summary>
        /// An item in the list
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T1 this[int index]
        {
            get
            {
                LockList.EnterReadLock();
                try
                {
                    return m_TList1[index];
                }

                finally
                {
                    LockList.ExitReadLock();
                }
            }
            set
            {
                LockList.EnterWriteLock();
                try
                {
                    m_TList1[index] = value;
                }

                finally
                {
                    LockList.ExitWriteLock();
                }
            }
        }

        #endregion
    }
}
