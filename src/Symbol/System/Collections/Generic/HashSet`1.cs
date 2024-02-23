/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
#if net20
using System;
using System.Collections;
using System.Collections.Generic;

#pragma warning disable 1591

namespace System.Collections.Generic;

/// <summary>
/// HashSet类。
/// </summary>
/// <typeparam name="T">任意类型。</typeparam>
[System.Serializable]
[System.Diagnostics.DebuggerDisplay("Count = {Count}")]
[System.Diagnostics.DebuggerTypeProxy(typeof(HashSetDebugView<>))]
#if !netcore
[System.Security.Permissions.HostProtection(System.Security.Permissions.SecurityAction.LinkDemand, MayLeakOnAbort = true)]
#endif
public class HashSet<T> :
    System.Runtime.Serialization.ISerializable,
    System.Runtime.Serialization.IDeserializationCallback,
    ICollection<T> {

    #region fields

    private const string CapacityName = "Capacity";
    private const string ComparerName = "Comparer";
    private const string ElementsName = "Elements";
    private const int GrowthFactor = 2;
    private const int Lower31BitMask = 2147483647;
    private int[] m_buckets;
    private IEqualityComparer<T> m_comparer;
    private int m_count;
    private int m_freeList;
    private int m_lastIndex;
    private System.Runtime.Serialization.SerializationInfo m_siInfo;
    private Slot[] m_slots;
    private int m_version;
    private const int ShrinkThreshold = 3;
    private const int StackAllocThreshold = 100;
    private const string VersionName = "Version";

    #endregion

    #region properties
    public IEqualityComparer<T> Comparer {
        get {
            return this.m_comparer;
        }
    }

    public int Count {
        get {
            return this.m_count;
        }
    }

    bool ICollection<T>.IsReadOnly {
        get {
            return false;
        }
    }
    #endregion

    #region ctor
    public HashSet()
        : this(EqualityComparer<T>.Default) {
    }

    public HashSet(IEnumerable<T> collection)
        : this(collection, EqualityComparer<T>.Default) {
    }

    public HashSet(IEqualityComparer<T> comparer) {
        if (comparer == null) {
            comparer = EqualityComparer<T>.Default;
        }
        this.m_comparer = comparer;
        this.m_lastIndex = 0;
        this.m_count = 0;
        this.m_freeList = -1;
        this.m_version = 0;
    }

    public HashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer)
        : this(comparer) {
        if (collection == null) {
            throw new ArgumentNullException("collection");
        }
        int capacity = 0;
        ICollection<T> is2 = collection as ICollection<T>;
        if (is2 != null) {
            capacity = is2.Count;
        }
        this.Initialize(capacity);
        this.UnionWith(collection);
        if (((this.m_count == 0) && (this.m_slots.Length > HashHelpers.GetMinPrime())) || ((this.m_count > 0) && ((this.m_slots.Length / this.m_count) > 3))) {
            this.TrimExcess();
        }
    }

    protected HashSet(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) {
        this.m_siInfo = info;
    }
    #endregion

    #region methods

    public bool Add(T item) {
        return this.AddIfNotPresent(item);
    }

    private bool AddIfNotPresent(T value) {
        int freeList;
        if (this.m_buckets == null) {
            this.Initialize(0);
        }
        int hashCode = this.InternalGetHashCode(value);
        int index = hashCode % this.m_buckets.Length;
        for (int i = this.m_buckets[hashCode % this.m_buckets.Length] - 1; i >= 0; i = this.m_slots[i].next) {
            if ((this.m_slots[i].hashCode == hashCode) && this.m_comparer.Equals(this.m_slots[i].value, value)) {
                return false;
            }
        }
        if (this.m_freeList >= 0) {
            freeList = this.m_freeList;
            this.m_freeList = this.m_slots[freeList].next;
        } else {
            if (this.m_lastIndex == this.m_slots.Length) {
                this.IncreaseCapacity();
                index = hashCode % this.m_buckets.Length;
            }
            freeList = this.m_lastIndex;
            this.m_lastIndex++;
        }
        this.m_slots[freeList].hashCode = hashCode;
        this.m_slots[freeList].value = value;
        this.m_slots[freeList].next = this.m_buckets[index] - 1;
        this.m_buckets[index] = freeList + 1;
        this.m_count++;
        this.m_version++;
        return true;
    }

    private bool AddOrGetLocation(T value, out int location) {
        int freeList;
        int hashCode = this.InternalGetHashCode(value);
        int index = hashCode % this.m_buckets.Length;
        for (int i = this.m_buckets[hashCode % this.m_buckets.Length] - 1; i >= 0; i = this.m_slots[i].next) {
            if ((this.m_slots[i].hashCode == hashCode) && this.m_comparer.Equals(this.m_slots[i].value, value)) {
                location = i;
                return false;
            }
        }
        if (this.m_freeList >= 0) {
            freeList = this.m_freeList;
            this.m_freeList = this.m_slots[freeList].next;
        } else {
            if (this.m_lastIndex == this.m_slots.Length) {
                this.IncreaseCapacity();
                index = hashCode % this.m_buckets.Length;
            }
            freeList = this.m_lastIndex;
            this.m_lastIndex++;
        }
        this.m_slots[freeList].hashCode = hashCode;
        this.m_slots[freeList].value = value;
        this.m_slots[freeList].next = this.m_buckets[index] - 1;
        this.m_buckets[index] = freeList + 1;
        this.m_count++;
        this.m_version++;
        location = freeList;
        return true;
    }

    private static bool AreEqualityComparersEqual(HashSet<T> set1, HashSet<T> set2) {
        return set1.Comparer.Equals(set2.Comparer);
    }

    [System.Security.SecurityCritical]
    private unsafe ElementCount CheckUniqueAndUnfoundElements(IEnumerable<T> other, bool returnIfUnfound) {
        ElementCount count;
        if (this.m_count != 0) {
            BitHelper helper;
            int length = BitHelper.ToIntArrayLength(this.m_lastIndex);
            if (length <= 100) {
                int* bitArrayPtr = stackalloc int[length];
                helper = new BitHelper(bitArrayPtr, length);
            } else {
                int[] bitArray = new int[length];
                helper = new BitHelper(bitArray, length);
            }
            int num4 = 0;
            int num5 = 0;
            foreach (T local in other) {
                int bitPosition = this.InternalIndexOf(local);
                if (bitPosition >= 0) {
                    if (!helper.IsMarked(bitPosition)) {
                        helper.MarkBit(bitPosition);
                        num5++;
                    }
                } else {
                    num4++;
                    if (returnIfUnfound) {
                        break;
                    }
                }
            }
            count.uniqueCount = num5;
            count.unfoundCount = num4;
            return count;
        }
        int num = 0;
        using (IEnumerator<T> enumerator = other.GetEnumerator()) {
            while (enumerator.MoveNext()) {
                T current = enumerator.Current;
                num++;
                goto Label_0039;
            }
        }
    Label_0039:
        count.uniqueCount = 0;
        count.unfoundCount = num;
        return count;
    }

    public void Clear() {
        if (this.m_lastIndex > 0) {
            Array.Clear(this.m_slots, 0, this.m_lastIndex);
            Array.Clear(this.m_buckets, 0, this.m_buckets.Length);
            this.m_lastIndex = 0;
            this.m_count = 0;
            this.m_freeList = -1;
        }
        this.m_version++;
    }

    public bool Contains(T item) {
        if (this.m_buckets != null) {
            int hashCode = this.InternalGetHashCode(item);
            for (int i = this.m_buckets[hashCode % this.m_buckets.Length] - 1; i >= 0; i = this.m_slots[i].next) {
                if ((this.m_slots[i].hashCode == hashCode) && this.m_comparer.Equals(this.m_slots[i].value, item)) {
                    return true;
                }
            }
        }
        return false;
    }

    private bool ContainsAllElements(IEnumerable<T> other) {
        foreach (T local in other) {
            if (!this.Contains(local)) {
                return false;
            }
        }
        return true;
    }

    public void CopyTo(T[] array) {
        this.CopyTo(array, 0, this.m_count);
    }

    public void CopyTo(T[] array, int arrayIndex) {
        this.CopyTo(array, arrayIndex, this.m_count);
    }

    public void CopyTo(T[] array, int arrayIndex, int count) {
        if (array == null) {
            throw new ArgumentNullException("array");
        }
        if (arrayIndex < 0) {
            throw new ArgumentOutOfRangeException("arrayIndex");// SR.GetString("ArgumentOutOfRange_NeedNonNegNum"));
        }
        if (count < 0) {
            throw new ArgumentOutOfRangeException("count");//, SR.GetString("ArgumentOutOfRange_NeedNonNegNum"));
        }
        if ((arrayIndex > array.Length) || (count > (array.Length - arrayIndex))) {
            throw new ArgumentException("数量不匹配");//SR.GetString("Arg_ArrayPlusOffTooSmall"));
        }
        int num = 0;
        for (int i = 0; (i < this.m_lastIndex) && (num < count); i++) {
            if (this.m_slots[i].hashCode >= 0) {
                array[arrayIndex + num] = this.m_slots[i].value;
                num++;
            }
        }
    }

    public static IEqualityComparer<HashSet<T>> CreateSetComparer() {
        return new HashSetEqualityComparer<T>();
    }

    public void ExceptWith(IEnumerable<T> other) {
        if (other == null) {
            throw new ArgumentNullException("other");
        }
        if (this.m_count != 0) {
            if (other == this) {
                this.Clear();
            } else {
                foreach (T local in other) {
                    this.Remove(local);
                }
            }
        }
    }

    public Enumerator GetEnumerator() {
        return new Enumerator((HashSet<T>)this);
    }
#if !netcore
    [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.LinkDemand, Flags = System.Security.Permissions.SecurityPermissionFlag.SerializationFormatter)]
#endif
    public virtual void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) {
        if (info == null) {
            throw new ArgumentNullException("info");
        }
        info.AddValue("Version", this.m_version);
        info.AddValue("Comparer", this.m_comparer, typeof(IEqualityComparer<T>));
        info.AddValue("Capacity", (this.m_buckets == null) ? 0 : this.m_buckets.Length);
        if (this.m_buckets != null) {
            T[] array = new T[this.m_count];
            this.CopyTo(array);
            info.AddValue("Elements", array, typeof(T[]));
        }
    }

    internal static bool HashSetEquals(HashSet<T> set1, HashSet<T> set2, IEqualityComparer<T> comparer) {
        if (set1 == null) {
            return (set2 == null);
        }
        if (set2 == null) {
            return false;
        }
        if (HashSet<T>.AreEqualityComparersEqual(set1, set2)) {
            if (set1.Count != set2.Count) {
                return false;
            }
            foreach (T local in set2) {
                if (!set1.Contains(local)) {
                    return false;
                }
            }
            return true;
        }
        foreach (T local2 in set2) {
            bool flag = false;
            foreach (T local3 in set1) {
                if (comparer.Equals(local2, local3)) {
                    flag = true;
                    break;
                }
            }
            if (!flag) {
                return false;
            }
        }
        return true;
    }

    private void IncreaseCapacity() {
        int min = this.m_count * 2;
        if (min < 0) {
            min = this.m_count;
        }
        int prime = HashHelpers.GetPrime(min);
        if (prime <= this.m_count) {
            throw new ArgumentException("缓冲区溢出");
        }
        Slot[] destinationArray = new Slot[prime];
        if (this.m_slots != null) {
            Array.Copy(this.m_slots, 0, destinationArray, 0, this.m_lastIndex);
        }
        int[] numArray = new int[prime];
        for (int i = 0; i < this.m_lastIndex; i++) {
            int index = destinationArray[i].hashCode % prime;
            destinationArray[i].next = numArray[index] - 1;
            numArray[index] = i + 1;
        }
        this.m_slots = destinationArray;
        this.m_buckets = numArray;
    }

    private void Initialize(int capacity) {
        int prime = HashHelpers.GetPrime(capacity);
        this.m_buckets = new int[prime];
        this.m_slots = new Slot[prime];
    }

    private int InternalGetHashCode(T item) {
        if (item == null) {
            return 0;
        }
        return (this.m_comparer.GetHashCode(item) & 2147483647);
    }

    private int InternalIndexOf(T item) {
        int hashCode = this.InternalGetHashCode(item);
        for (int i = this.m_buckets[hashCode % this.m_buckets.Length] - 1; i >= 0; i = this.m_slots[i].next) {
            if ((this.m_slots[i].hashCode == hashCode) && this.m_comparer.Equals(this.m_slots[i].value, item)) {
                return i;
            }
        }
        return -1;
    }

    [System.Security.SecurityCritical]
    public void IntersectWith(IEnumerable<T> other) {
        if (other == null) {
            throw new ArgumentNullException("other");
        }
        if (this.m_count != 0) {
            ICollection<T> is2 = other as ICollection<T>;
            if (is2 != null) {
                if (is2.Count == 0) {
                    this.Clear();
                    return;
                }
                HashSet<T> set = other as HashSet<T>;
                if ((set != null) && HashSet<T>.AreEqualityComparersEqual((HashSet<T>)this, set)) {
                    this.IntersectWithHashSetWithSameEC(set);
                    return;
                }
            }
            this.IntersectWithEnumerable(other);
        }
    }

    [System.Security.SecurityCritical]
    private unsafe void IntersectWithEnumerable(IEnumerable<T> other) {
        BitHelper helper;
        int lastIndex = this.m_lastIndex;
        int length = BitHelper.ToIntArrayLength(lastIndex);
        if (length <= 100) {
            int* bitArrayPtr = stackalloc int[length];
            helper = new BitHelper(bitArrayPtr, length);
        } else {
            int[] bitArray = new int[length];
            helper = new BitHelper(bitArray, length);
        }
        foreach (T local in other) {
            int bitPosition = this.InternalIndexOf(local);
            if (bitPosition >= 0) {
                helper.MarkBit(bitPosition);
            }
        }
        for (int i = 0; i < lastIndex; i++) {
            if ((this.m_slots[i].hashCode >= 0) && !helper.IsMarked(i)) {
                this.Remove(this.m_slots[i].value);
            }
        }
    }

    private void IntersectWithHashSetWithSameEC(HashSet<T> other) {
        for (int i = 0; i < this.m_lastIndex; i++) {
            if (this.m_slots[i].hashCode >= 0) {
                T item = this.m_slots[i].value;
                if (!other.Contains(item)) {
                    this.Remove(item);
                }
            }
        }
    }

    [System.Security.SecurityCritical]
    public bool IsProperSubsetOf(IEnumerable<T> other) {
        if (other == null) {
            throw new ArgumentNullException("other");
        }
        ICollection<T> is2 = other as ICollection<T>;
        if (is2 != null) {
            if (this.m_count == 0) {
                return (is2.Count > 0);
            }
            HashSet<T> set = other as HashSet<T>;
            if ((set != null) && HashSet<T>.AreEqualityComparersEqual((HashSet<T>)this, set)) {
                if (this.m_count >= set.Count) {
                    return false;
                }
                return this.IsSubsetOfHashSetWithSameEC(set);
            }
        }
        ElementCount count = this.CheckUniqueAndUnfoundElements(other, false);
        return ((count.uniqueCount == this.m_count) && (count.unfoundCount > 0));
    }

    [System.Security.SecurityCritical]
    public bool IsProperSupersetOf(IEnumerable<T> other) {
        if (other == null) {
            throw new ArgumentNullException("other");
        }
        if (this.m_count == 0) {
            return false;
        }
        ICollection<T> is2 = other as ICollection<T>;
        if (is2 != null) {
            if (is2.Count == 0) {
                return true;
            }
            HashSet<T> set = other as HashSet<T>;
            if ((set != null) && HashSet<T>.AreEqualityComparersEqual((HashSet<T>)this, set)) {
                if (set.Count >= this.m_count) {
                    return false;
                }
                return this.ContainsAllElements(set);
            }
        }
        ElementCount count = this.CheckUniqueAndUnfoundElements(other, true);
        return ((count.uniqueCount < this.m_count) && (count.unfoundCount == 0));
    }

    [System.Security.SecurityCritical]
    public bool IsSubsetOf(IEnumerable<T> other) {
        if (other == null) {
            throw new ArgumentNullException("other");
        }
        if (this.m_count == 0) {
            return true;
        }
        HashSet<T> set = other as HashSet<T>;
        if ((set != null) && HashSet<T>.AreEqualityComparersEqual((HashSet<T>)this, set)) {
            if (this.m_count > set.Count) {
                return false;
            }
            return this.IsSubsetOfHashSetWithSameEC(set);
        }
        ElementCount count = this.CheckUniqueAndUnfoundElements(other, false);
        return ((count.uniqueCount == this.m_count) && (count.unfoundCount >= 0));
    }

    private bool IsSubsetOfHashSetWithSameEC(HashSet<T> other) {
        foreach (T local in this) {
            if (!other.Contains(local)) {
                return false;
            }
        }
        return true;
    }

    public bool IsSupersetOf(IEnumerable<T> other) {
        if (other == null) {
            throw new ArgumentNullException("other");
        }
        ICollection<T> is2 = other as ICollection<T>;
        if (is2 != null) {
            if (is2.Count == 0) {
                return true;
            }
            HashSet<T> set = other as HashSet<T>;
            if (((set != null) && HashSet<T>.AreEqualityComparersEqual((HashSet<T>)this, set)) && (set.Count > this.m_count)) {
                return false;
            }
        }
        return this.ContainsAllElements(other);
    }

    public virtual void OnDeserialization(object sender) {
        if (this.m_siInfo != null) {
            int num = this.m_siInfo.GetInt32("Capacity");
            this.m_comparer = (IEqualityComparer<T>)this.m_siInfo.GetValue("Comparer", typeof(IEqualityComparer<T>));
            this.m_freeList = -1;
            if (num != 0) {
                this.m_buckets = new int[num];
                this.m_slots = new Slot[num];
                T[] localArray = (T[])this.m_siInfo.GetValue("Elements", typeof(T[]));
                if (localArray == null) {
                    throw new System.Runtime.Serialization.SerializationException("序列化数据错误");
                }
                for (int i = 0; i < localArray.Length; i++) {
                    this.AddIfNotPresent(localArray[i]);
                }
            } else {
                this.m_buckets = null;
            }
            this.m_version = this.m_siInfo.GetInt32("Version");
            this.m_siInfo = null;
        }
    }

    public bool Overlaps(IEnumerable<T> other) {
        if (other == null) {
            throw new ArgumentNullException("other");
        }
        if (this.m_count != 0) {
            foreach (T local in other) {
                if (this.Contains(local)) {
                    return true;
                }
            }
        }
        return false;
    }

    public bool Remove(T item) {
        if (this.m_buckets != null) {
            int hashCode = this.InternalGetHashCode(item);
            int index = hashCode % this.m_buckets.Length;
            int num3 = -1;
            for (int i = this.m_buckets[index] - 1; i >= 0; i = this.m_slots[i].next) {
                if ((this.m_slots[i].hashCode == hashCode) && this.m_comparer.Equals(this.m_slots[i].value, item)) {
                    if (num3 < 0) {
                        this.m_buckets[index] = this.m_slots[i].next + 1;
                    } else {
                        this.m_slots[num3].next = this.m_slots[i].next;
                    }
                    this.m_slots[i].hashCode = -1;
                    this.m_slots[i].value = default(T);
                    this.m_slots[i].next = this.m_freeList;
                    this.m_freeList = i;
                    this.m_count--;
                    this.m_version++;
                    return true;
                }
                num3 = i;
            }
        }
        return false;
    }

    public int RemoveWhere(Predicate<T> match) {
        if (match == null) {
            throw new ArgumentNullException("match");
        }
        int num = 0;
        for (int i = 0; i < this.m_lastIndex; i++) {
            if (this.m_slots[i].hashCode >= 0) {
                T local = this.m_slots[i].value;
                if (match(local) && this.Remove(local)) {
                    num++;
                }
            }
        }
        return num;
    }
    public bool Exists(Predicate<T> match) {
        if (match == null) {
            throw new ArgumentNullException("match");
        }
        for (int i = 0; i < this.m_lastIndex; i++) {
            if (this.m_slots[i].hashCode >= 0) {
                T local = this.m_slots[i].value;
                if (match(local))
                    return true;
            }
        }
        return false;
    }
    [System.Security.SecurityCritical]
    public bool SetEquals(IEnumerable<T> other) {
        if (other == null) {
            throw new ArgumentNullException("other");
        }
        HashSet<T> set = other as HashSet<T>;
        if ((set != null) && HashSet<T>.AreEqualityComparersEqual((HashSet<T>)this, set)) {
            if (this.m_count != set.Count) {
                return false;
            }
            return this.ContainsAllElements(set);
        }
        ICollection<T> is2 = other as ICollection<T>;
        if (((is2 != null) && (this.m_count == 0)) && (is2.Count > 0)) {
            return false;
        }
        ElementCount count = this.CheckUniqueAndUnfoundElements(other, true);
        return ((count.uniqueCount == this.m_count) && (count.unfoundCount == 0));
    }

    [System.Security.SecurityCritical]
    public void SymmetricExceptWith(IEnumerable<T> other) {
        if (other == null) {
            throw new ArgumentNullException("other");
        }
        if (this.m_count == 0) {
            this.UnionWith(other);
        } else if (other == this) {
            this.Clear();
        } else {
            HashSet<T> set = other as HashSet<T>;
            if ((set != null) && HashSet<T>.AreEqualityComparersEqual((HashSet<T>)this, set)) {
                this.SymmetricExceptWithUniqueHashSet(set);
            } else {
                this.SymmetricExceptWithEnumerable(other);
            }
        }
    }

    [System.Security.SecurityCritical]
    private unsafe void SymmetricExceptWithEnumerable(IEnumerable<T> other) {
        BitHelper helper;
        BitHelper helper2;
        int lastIndex = this.m_lastIndex;
        int length = BitHelper.ToIntArrayLength(lastIndex);
        if (length <= 50) {
            int* bitArrayPtr = stackalloc int[length];
            helper = new BitHelper(bitArrayPtr, length);
            int* numPtr2 = stackalloc int[length];
            helper2 = new BitHelper(numPtr2, length);
        } else {
            int[] bitArray = new int[length];
            helper = new BitHelper(bitArray, length);
            int[] numArray2 = new int[length];
            helper2 = new BitHelper(numArray2, length);
        }
        foreach (T local in other) {
            int location = 0;
            if (this.AddOrGetLocation(local, out location)) {
                helper2.MarkBit(location);
            } else if ((location < lastIndex) && !helper2.IsMarked(location)) {
                helper.MarkBit(location);
            }
        }
        for (int i = 0; i < lastIndex; i++) {
            if (helper.IsMarked(i)) {
                this.Remove(this.m_slots[i].value);
            }
        }
    }

    private void SymmetricExceptWithUniqueHashSet(HashSet<T> other) {
        foreach (T local in other) {
            if (!this.Remove(local)) {
                this.AddIfNotPresent(local);
            }
        }
    }

    void ICollection<T>.Add(T item) {
        this.AddIfNotPresent(item);
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator() {
        return new Enumerator((HashSet<T>)this);
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return new Enumerator((HashSet<T>)this);
    }

    internal T[] ToArray() {
        T[] array = new T[this.Count];
        this.CopyTo(array);
        return array;
    }

    public void TrimExcess() {
        if (this.m_count == 0) {
            this.m_buckets = null;
            this.m_slots = null;
            this.m_version++;
        } else {
            int prime = HashHelpers.GetPrime(this.m_count);
            Slot[] slotArray = new Slot[prime];
            int[] numArray = new int[prime];
            int index = 0;
            for (int i = 0; i < this.m_lastIndex; i++) {
                if (this.m_slots[i].hashCode >= 0) {
                    slotArray[index] = this.m_slots[i];
                    int num4 = slotArray[index].hashCode % prime;
                    slotArray[index].next = numArray[num4] - 1;
                    numArray[num4] = index + 1;
                    index++;
                }
            }
            this.m_lastIndex = index;
            this.m_slots = slotArray;
            this.m_buckets = numArray;
            this.m_freeList = -1;
        }
    }

    public void UnionWith(IEnumerable<T> other) {
        if (other == null) {
            throw new ArgumentNullException("other");
        }
        foreach (T local in other) {
            this.AddIfNotPresent(local);
        }
    }
    #endregion

    #region classes

    // Nested Types
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    internal struct ElementCount {
        internal int uniqueCount;
        internal int unfoundCount;
    }

    [System.Serializable]
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
#if !netcore
    [System.Security.Permissions.HostProtection(System.Security.Permissions.SecurityAction.LinkDemand, MayLeakOnAbort = true)]
#endif
    public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator {
        private HashSet<T> set;
        private int index;
        private int version;
        private T current;
        internal Enumerator(HashSet<T> set) {
            this.set = set;
            this.index = 0;
            this.version = set.m_version;
            this.current = default(T);
        }

        public void Dispose() {
        }

        public bool MoveNext() {
            if (this.version != this.set.m_version) {
                throw new InvalidOperationException("已失效的版本");
            }
            while (this.index < this.set.m_lastIndex) {
                if (this.set.m_slots[this.index].hashCode >= 0) {
                    this.current = this.set.m_slots[this.index].value;
                    this.index++;
                    return true;
                }
                this.index++;
            }
            this.index = this.set.m_lastIndex + 1;
            this.current = default(T);
            return false;
        }

        public T Current {
            get {
                return this.current;
            }
        }
        object IEnumerator.Current {
            get {
                if ((this.index == 0) || (this.index == (this.set.m_lastIndex + 1))) {
                    throw new InvalidOperationException("索引无效");
                }
                return this.Current;
            }
        }
        void IEnumerator.Reset() {
            if (this.version != this.set.m_version) {
                throw new InvalidOperationException("已失效的版本");
            }
            this.index = 0;
            this.current = default(T);
        }
    }
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    internal struct Slot {
        internal int hashCode;
        internal T value;
        internal int next;
    }
    [System.Serializable]
    internal static class HashHelpers {
        // Fields
        internal static readonly int[] primes = new int[] { 
    3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 
    293, 353, 431, 521, 631, 761, 919, 1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 
    5839, 7013, 8419, 10103, 12143, 14591, 17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 
    108631, 130363, 156437, 187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263, 1674319, 
    2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369
 };

        // Methods
        internal static int GetMinPrime() {
            return primes[0];
        }

#if !netcore
        [System.Runtime.ConstrainedExecution.ReliabilityContract(System.Runtime.ConstrainedExecution.Consistency.WillNotCorruptState, System.Runtime.ConstrainedExecution.Cer.Success)]
#endif
        internal static int GetPrime(int min) {
            for (int i = 0; i < primes.Length; i++) {
                int num2 = primes[i];
                if (num2 >= min) {
                    return num2;
                }
            }
            for (int j = min | 1; j < 2147483647; j += 2) {
                if (IsPrime(j)) {
                    return j;
                }
            }
            return min;
        }
#if !netcore
        [System.Runtime.ConstrainedExecution.ReliabilityContract(System.Runtime.ConstrainedExecution.Consistency.WillNotCorruptState, System.Runtime.ConstrainedExecution.Cer.Success)]
#endif
        internal static bool IsPrime(int candidate) {
            if ((candidate & 1) == 0) {
                return (candidate == 2);
            }
            int num = (int)Math.Sqrt((double)candidate);
            for (int i = 3; i <= num; i += 2) {
                if ((candidate % i) == 0) {
                    return false;
                }
            }
            return true;
        }
    }
    internal class BitHelper {
        // Fields
        private const byte IntSize = 32;
        private int[] m_array;
        private unsafe int* m_arrayPtr;
        private int m_length;
        private const byte MarkedBitFlag = 1;
        private bool useStackAlloc;

        // Methods
        [System.Security.SecurityCritical]
        internal unsafe BitHelper(int* bitArrayPtr, int length) {
            this.m_arrayPtr = bitArrayPtr;
            this.m_length = length;
            this.useStackAlloc = true;
        }

        internal BitHelper(int[] bitArray, int length) {
            this.m_array = bitArray;
            this.m_length = length;
        }

        [System.Security.SecurityCritical]
        internal unsafe bool IsMarked(int bitPosition) {
            if (this.useStackAlloc) {
                int num = bitPosition / 32;
                return (((num < this.m_length) && (num >= 0)) && ((this.m_arrayPtr[num] & (((int)1) << (bitPosition % 32))) != 0));
            }
            int index = bitPosition / 32;
            return (((index < this.m_length) && (index >= 0)) && ((this.m_array[index] & (((int)1) << (bitPosition % 32))) != 0));
        }

        [System.Security.SecurityCritical]
        internal unsafe void MarkBit(int bitPosition) {
            if (this.useStackAlloc) {
                int num = bitPosition / 32;
                if ((num < this.m_length) && (num >= 0)) {
                    int* numPtr1 = this.m_arrayPtr + num;
                    numPtr1[0] |= ((int)1) << (bitPosition % 32);
                }
            } else {
                int index = bitPosition / 32;
                if ((index < this.m_length) && (index >= 0)) {
                    this.m_array[index] |= ((int)1) << (bitPosition % 32);
                }
            }
        }

        internal static int ToIntArrayLength(int n) {
            if (n <= 0) {
                return 0;
            }
            return (((n - 1) / 32) + 1);
        }
    }

    #endregion
}
internal class HashSetDebugView<T> {
    private HashSet<T> set;

    [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.RootHidden)]
    public T[] Items {
        get {
            return this.set.ToArray();
        }
    }

    public HashSetDebugView(HashSet<T> set) {
        if (set == null) {
            throw new ArgumentNullException("set");
        }
        this.set = set;
    }

   
}
internal class HashSetEqualityComparer<T> : IEqualityComparer<HashSet<T>> {
    // Fields
    private IEqualityComparer<T> m_comparer;

    // Methods
    public HashSetEqualityComparer() {
        this.m_comparer = EqualityComparer<T>.Default;
    }

    public HashSetEqualityComparer(IEqualityComparer<T> comparer) {
        if (this.m_comparer == null) {
            this.m_comparer = EqualityComparer<T>.Default;
        } else {
            this.m_comparer = comparer;
        }
    }

    public override bool Equals(object obj) {
        HashSetEqualityComparer<T> comparer = obj as HashSetEqualityComparer<T>;
        if (comparer == null) {
            return false;
        }
        return (this.m_comparer == comparer.m_comparer);
    }

    public bool Equals(HashSet<T> x, HashSet<T> y) {
        return HashSet<T>.HashSetEquals(x, y, this.m_comparer);
    }

    public override int GetHashCode() {
        return this.m_comparer.GetHashCode();
    }

    public int GetHashCode(HashSet<T> obj) {
        int num = 0;
        if (obj != null) {
            foreach (T local in obj) {
                num ^= this.m_comparer.GetHashCode(local) & 2147483647;
            }
        }
        return num;
    }
}
#pragma warning restore 1591

#endif