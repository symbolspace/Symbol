/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;
using System.Collections;
using System.Collections.Generic;

#pragma warning disable 1591

/// <summary>
/// 这是将.net 3.5开始的Enumerable类复制并改造过的，让.net 2.0下也可以类似的功能。
/// </summary>
public static class LinqHelper {

    #region Any
    /// <summary>
    /// 集合中至少有一个成员时返回true。
    /// </summary>
    /// <returns></returns>
    public static bool Any<T>(
#if !net20
        this 
#endif
        IEnumerable<T> source) {
        return Any<T>(source, null);
    }
    public static bool Any<T>(
#if !net20
        this 
#endif
        IEnumerable<T> source, Predicate<T> predicate) {
        foreach (T item in source) {
            if (predicate == null || predicate(item))
                return true;
        }
        return false;
    }
    #endregion

    #region Count
    public static int Count<T>(IEnumerable<T> source) {
        return Count<T>(source, null);
    }
    public static int Count<T>(IEnumerable<T> source, Predicate<T> predicate) {
        int result = 0;
        foreach (T item in source) {
            if (predicate == null || predicate(item))
                result++;
        }
        return result;
    }
    #endregion

    #region FirstOrDefault
    public static T FirstOrDefault<T>(IEnumerable<T> source) {
        return FirstOrDefault<T>(source, null);
    }
    public static T FirstOrDefault<T>(IEnumerable<T> source, Predicate<T> predicate) {
        foreach (T item in source) {
            if (predicate == null || predicate(item))
                return item;
        }
        return default(T);
    }
    #endregion

    #region Where
    public static IQueryable<TSource> Where<TSource>(IEnumerable<TSource> source, LinqHelperFunc<TSource, bool> predicate) {
        if (source == null) {
            throw new ArgumentNullException("source");
        }
        if (predicate == null) {
            throw new ArgumentNullException("predicate");
        }
        return new WhereEnumerable<TSource>() { _source = source, _predicate = predicate };
    }
    abstract class Queryable<TSource> : IQueryable<TSource> {
        public bool Any() {
            return LinqHelper.Any(this);
        }

        public bool Any(Predicate<TSource> predicate) {
            return LinqHelper.Any(this, predicate);
        }

        public int Count() {
            return LinqHelper.Count(this);
        }

        public int Count(Predicate<TSource> predicate) {
            return LinqHelper.Count(this, predicate);
        }

        public TSource FirstOrDefault() {
            return LinqHelper.FirstOrDefault(this);
        }

        public TSource FirstOrDefault(Predicate<TSource> predicate) {
            return LinqHelper.FirstOrDefault(this, predicate);
        }

        public abstract IEnumerator<TSource> GetEnumerator();

        public IOrderedEnumerable<TSource> OrderBy<TKey>(LinqHelperFunc<TSource, TKey> keySelector) {
            return LinqHelper.OrderBy(this, keySelector);
        }

        public IOrderedEnumerable<TSource> OrderBy<TKey>(LinqHelperFunc<TSource, TKey> keySelector, IComparer<TKey> comparer) {
            return LinqHelper.OrderBy(this, keySelector, comparer);
        }

        public IOrderedEnumerable<TSource> OrderByDescending<TKey>(LinqHelperFunc<TSource, TKey> keySelector) {
            return LinqHelper.OrderByDescending(this, keySelector);
        }

        public IOrderedEnumerable<TSource> OrderByDescending<TKey>(LinqHelperFunc<TSource, TKey> keySelector, IComparer<TKey> comparer) {
            return LinqHelper.OrderByDescending(this, keySelector, comparer);
        }

        public IQueryable<TResult> Select<TResult>(LinqHelperFunc<TSource, TResult> selector) {
            return LinqHelper.Select(this, selector);
        }

        public IQueryable<TSource> Skip(int count) {
            return LinqHelper.Skip(this, count);
        }

        public IQueryable<TSource> Take(int count) {
            return LinqHelper.Take(this, count);
        }

        public TSource[] ToArray() {
            return LinqHelper.ToArray(this);
        }

        public TSource[] ToArray(Predicate<TSource> predicate) {
            return LinqHelper.ToArray(this, predicate);
        }

        public List<TSource> ToList() {
            return LinqHelper.ToList(this);
        }

        public List<TSource> ToList(Predicate<TSource> predicate) {
            return LinqHelper.ToList(this, predicate);
        }

        public IQueryable<TSource> Where(LinqHelperFunc<TSource, bool> predicate) {
            return LinqHelper.Where(this, predicate);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
    class WhereEnumerable<TSource> : Queryable<TSource> {
        public IEnumerable<TSource> _source;
        public LinqHelperFunc<TSource, bool> _predicate;

      

        #region IEnumerable<T> 成员

        public override IEnumerator<TSource> GetEnumerator() {
            return new Enumerator() { _source = _source.GetEnumerator(), _predicate = _predicate, };
        }

        #endregion

        class Enumerator : IEnumerator<TSource> {
            public IEnumerator<TSource> _source;
            public LinqHelperFunc<TSource, bool> _predicate;
            private TSource _current;
            #region IEnumerator<T> 成员

            public TSource Current {
                get { return _current; }
            }

            #endregion

            #region IDisposable 成员

            public void Dispose() {
                _source = null;
            }

            #endregion

            #region IEnumerator 成员

            object System.Collections.IEnumerator.Current {
                get { return _current; }
            }

            public bool MoveNext() {
                bool result = false;
            lb_ReNext:
                result = _source.MoveNext();
                TSource item = default(TSource);
                if (result) {
                    item = _source.Current;
                    if (!_predicate(_source.Current)) {
                        goto lb_ReNext;
                    }
                }
                _current = item;
                return result;
            }

            public void Reset() {
                throw new NotImplementedException();
            }

            #endregion
        }
    }

    #endregion

    #region ToList
    public static List<T> ToList<T>(IEnumerable<T> source) {
        return ToList<T>(source, null);
    }
    public static List<T> ToList<T>(IEnumerable<T> source, Predicate<T> predicate) {
        List<T> result = new List<T>();
        foreach (T item in source) {
            if (predicate == null || predicate(item))
                result.Add(item);
        }
        return result;
    }
    #endregion

    #region ToArray
   
    public static T[] ToArray<T>(IEnumerable<T> source) {
        return ToArray<T>(source, null);
    }
    public static T[] ToArray<T>(IEnumerable<T> source, Predicate<T> predicate) {
        List<T> result = new List<T>();
        foreach (T item in source) {
            if (predicate == null || predicate(item))
                result.Add(item);
        }
        return result.ToArray();
    }
    #endregion

    #region Skip
    public static IQueryable<T> Skip<T>(IEnumerable<T> source, int count) {
        return new SkipEnumerable<T>() { _source = source, _count = count };
    }
    class SkipEnumerable<T> : Queryable<T> {
        public IEnumerable<T> _source;
        public int _count;
        #region IEnumerable<T> 成员

        public override IEnumerator<T> GetEnumerator() {
            return new Enumerator() { _source = _source.GetEnumerator(), _count = _count };
        }

        #endregion

        class Enumerator : IEnumerator<T> {
            public IEnumerator<T> _source;
            public int _count;
            private T _current;
            private bool _finded;
            #region IEnumerator<T> 成员

            public T Current {
                get { return _current; }
            }

            #endregion

            #region IDisposable 成员

            public void Dispose() {
                _source = null;
            }

            #endregion

            #region IEnumerator 成员

            object System.Collections.IEnumerator.Current {
                get { return _current; }
            }

            public bool MoveNext() {
                if (!_finded) {
                    if (_count > 0) {
                        int index = 0;
                        bool b = false;

                        while ((b = _source.MoveNext())) {
                            index++;
                            if (index == _count) {
                                _finded = true;
                                break;
                            }
                        }
                    } else {
                        _finded = true;
                    }
                }
                bool result = _source.MoveNext();
                if (result) {
                    _current = _source.Current;
                }
                return result;
            }

            public void Reset() {
                throw new NotImplementedException();
            }

            #endregion
        }
    }
    #endregion

    #region Take
    public static IQueryable<T> Take<T>(IEnumerable<T> source, int count) {
        return new TakeEnumerable<T>() { _source = source, _count = count };
    }
    class TakeEnumerable<T> : Queryable<T> {
        public IEnumerable<T> _source;
        public int _count;
        #region IEnumerable<T> 成员

        public override IEnumerator<T> GetEnumerator() {
            return new Enumerator() { _source = _source.GetEnumerator(), _count = _count };
        }

        #endregion

        class Enumerator : IEnumerator<T> {
            public IEnumerator<T> _source;
            public int _count;
            private int _index = 0;
            private T _current;
            #region IEnumerator<T> 成员

            public T Current {
                get { return _current; }
            }

            #endregion

            #region IDisposable 成员

            public void Dispose() {
                _source = null;
            }

            #endregion

            #region IEnumerator 成员

            object System.Collections.IEnumerator.Current {
                get { return _current; }
            }

            public bool MoveNext() {

                bool result = _index == _count ? false : _source.MoveNext();
                if (result) {
                    _index++;
                    _current = _source.Current;
                }
                return result;
            }

            public void Reset() {
                throw new NotImplementedException();
            }

            #endregion
        }
    }
    #endregion

    #region Select
    public static IQueryable<TResult> Select<TSource, TResult>(IEnumerable<TSource> source, LinqHelperFunc<TSource, TResult> selector) {
        if (source == null) {
            throw new ArgumentNullException("source");
        }
        if (selector == null) {
            throw new ArgumentNullException("selector");
        }
        SelectEnumerable<TSource, TResult> result = new SelectEnumerable<TSource, TResult>() {
            _source = source,
            _selector = selector
        };
        return result;
    }
    class SelectEnumerable<TSource, TResult> : Queryable<TResult> {
        public IEnumerable<TSource> _source;
        public LinqHelperFunc<TSource, TResult> _selector;
        #region IEnumerable<T> 成员

        public override IEnumerator<TResult> GetEnumerator() {
            return new Enumerator() { _source = _source.GetEnumerator(), _selector = _selector, };
        }

        #endregion

       

        class Enumerator : IEnumerator<TResult> {
            public IEnumerator<TSource> _source;
            public LinqHelperFunc<TSource, TResult> _selector;
            private TResult _current;
            #region IEnumerator<T> 成员

            public TResult Current {
                get { return _current; }
            }

            #endregion

            #region IDisposable 成员

            public void Dispose() {
                _source = null;
            }

            #endregion

            #region IEnumerator 成员

            object System.Collections.IEnumerator.Current {
                get { return _current; }
            }

            public bool MoveNext() {
                bool result = _source.MoveNext();
                if (result) {
                    _current = _selector(_source.Current);
                }
                return result;
            }

            public void Reset() {
                throw new NotImplementedException();
            }

            #endregion
        }
    }
    public static IQueryable<TResult> Select<TResult>(IEnumerable source, LinqHelperFunc<object, TResult> selector) {
        if (source == null) {
            throw new ArgumentNullException("source");
        }
        if (selector == null) {
            throw new ArgumentNullException("selector");
        }
        SelectEnumerable<TResult> result = new SelectEnumerable<TResult>() {
            _source = source,
            _selector = selector
        };
        return result;
    }
    class SelectEnumerable<TResult> : Queryable<TResult> {
        public IEnumerable _source;
        public LinqHelperFunc<object, TResult> _selector;
        #region IEnumerable<T> 成员

        public override IEnumerator<TResult> GetEnumerator() {
            return new Enumerator() { _source = _source.GetEnumerator(), _selector = _selector, };
        }

        #endregion

       

        class Enumerator : IEnumerator<TResult> {
            public IEnumerator _source;
            public LinqHelperFunc<object, TResult> _selector;
            private TResult _current;
            #region IEnumerator<T> 成员

            public TResult Current {
                get { return _current; }
            }

            #endregion

            #region IDisposable 成员

            public void Dispose() {
                _source = null;
            }

            #endregion

            #region IEnumerator 成员

            object System.Collections.IEnumerator.Current {
                get { return _current; }
            }

            public bool MoveNext() {
                bool result = _source.MoveNext();
                if (result) {
                    _current = _selector(_source.Current);
                }
                return result;
            }

            public void Reset() {
                throw new NotImplementedException();
            }

            #endregion
        }
    }
    #endregion

    #region Max
    public static decimal Max(IEnumerable<decimal> source) {
        if (source == null) {
            throw new ArgumentNullException("source");
        }
        decimal num = 0M;
        bool flag = false;
        foreach (decimal num2 in source) {
            if (flag) {
                if (num2 > num) {
                    num = num2;
                }
            } else {
                num = num2;
                flag = true;
            }
        }
        if (!flag) {
            throw new Symbol.NotFoundException("source");
        }
        return num;
    }
    public static double Max(IEnumerable<double> source) {
        if (source == null) {
            throw new ArgumentNullException("source");
        }
        double d = 0.0;
        bool flag = false;
        foreach (double num2 in source) {
            if (flag) {
                if ((num2 > d) || double.IsNaN(d)) {
                    d = num2;
                }
                continue;
            }
            d = num2;
            flag = true;
        }
        if (!flag) {
            throw new Symbol.NotFoundException("source");
        }
        return d;
    }
    public static int Max(IEnumerable<int> source) {
        if (source == null) {
            throw new ArgumentNullException("source");
        }
        int num = 0;
        bool flag = false;
        foreach (int num2 in source) {
            if (flag) {
                if (num2 > num) {
                    num = num2;
                }
            } else {
                num = num2;
                flag = true;
            }
        }
        if (!flag) {
            throw new Symbol.NotFoundException("source");
        }
        return num;
    }
    public static long Max(IEnumerable<long> source) {
        if (source == null) {
            throw new ArgumentNullException("source");
        }
        long num = 0;
        bool flag = false;
        foreach (long num2 in source) {
            if (flag) {
                if (num2 > num) {
                    num = num2;
                }
            } else {
                num = num2;
                flag = true;
            }
        }
        if (!flag) {
            throw new Symbol.NotFoundException("source");
        }
        return num;
    }
    public static double? Max(IEnumerable<double?> source) {
        if (source == null) {
            throw new ArgumentNullException("source");
        }
        double? nullable = null;
        foreach (double? nullable2 in source) {
            if (nullable2.HasValue) {
                if (nullable.HasValue) {
                    double? nullable3 = nullable2;
                    double? nullable4 = nullable;
                    if (((nullable3.GetValueOrDefault() <= nullable4.GetValueOrDefault()) || !(nullable3.HasValue & nullable4.HasValue)) && !double.IsNaN(nullable.Value)) {
                        continue;
                    }
                }
                nullable = nullable2;
            }
        }
        return nullable;
    }
    public static int? Max(IEnumerable<int?> source) {
        if (source == null) {
            throw new ArgumentNullException("source");
        }
        int? nullable = null;
        foreach (int? nullable2 in source) {
            if (nullable.HasValue) {
                int? nullable3 = nullable2;
                int? nullable4 = nullable;
                if ((nullable3.GetValueOrDefault() <= nullable4.GetValueOrDefault()) || !(nullable3.HasValue & nullable4.HasValue)) {
                    continue;
                }
            }
            nullable = nullable2;
        }
        return nullable;
    }
    public static long? Max(IEnumerable<long?> source) {
        if (source == null) {
            throw new ArgumentNullException("source");
        }
        long? nullable = null;
        foreach (long? nullable2 in source) {
            if (nullable.HasValue) {
                long? nullable3 = nullable2;
                long? nullable4 = nullable;
                if ((nullable3.GetValueOrDefault() <= nullable4.GetValueOrDefault()) || !(nullable3.HasValue & nullable4.HasValue)) {
                    continue;
                }
            }
            nullable = nullable2;
        }
        return nullable;
    }
    public static TSource Max<TSource>(IEnumerable<TSource> source) {
        if (source == null) {
            throw new ArgumentNullException("source");
        }
        Comparer<TSource> comparer = Comparer<TSource>.Default;
        TSource y = default(TSource);
        if (y == null) {
            foreach (TSource local2 in source) {
                if ((local2 != null) && ((y == null) || (comparer.Compare(local2, y) > 0))) {
                    y = local2;
                }
            }
            return y;
        }
        bool flag = false;
        foreach (TSource local3 in source) {
            if (flag) {
                if (comparer.Compare(local3, y) > 0) {
                    y = local3;
                }
            } else {
                y = local3;
                flag = true;
            }
        }
        if (!flag) {
            throw new Symbol.NotFoundException("source");
        }
        return y;
    }
    public static decimal? Max(IEnumerable<decimal?> source) {
        if (source == null) {
            throw new ArgumentNullException("source");
        }
        decimal? nullable = null;
        foreach (decimal? nullable2 in source) {
            if (nullable.HasValue) {
                decimal? nullable3 = nullable2;
                decimal? nullable4 = nullable;
                if ((nullable3.GetValueOrDefault() <= nullable4.GetValueOrDefault()) || !(nullable3.HasValue & nullable4.HasValue)) {
                    continue;
                }
            }
            nullable = nullable2;
        }
        return nullable;
    }
    public static float? Max(IEnumerable<float?> source) {
        if (source == null) {
            throw new ArgumentNullException("source");
        }
        float? nullable = null;
        foreach (float? nullable2 in source) {
            if (nullable2.HasValue) {
                if (nullable.HasValue) {
                    float? nullable3 = nullable2;
                    float? nullable4 = nullable;
                    if (((nullable3.GetValueOrDefault() <= nullable4.GetValueOrDefault()) || !(nullable3.HasValue & nullable4.HasValue)) && !float.IsNaN(nullable.Value)) {
                        continue;
                    }
                }
                nullable = nullable2;
            }
        }
        return nullable;
    }
    public static float Max(IEnumerable<float> source) {
        if (source == null) {
            throw new ArgumentNullException("source");
        }
        float num = 0f;
        bool flag = false;
        foreach (float num2 in source) {
            if (flag) {
                if ((num2 > num) || double.IsNaN((double)num)) {
                    num = num2;
                }
                continue;
            }
            num = num2;
            flag = true;
        }
        if (!flag) {
            throw new Symbol.NotFoundException("source");
        }
        return num;
    }

    public static decimal Max<TSource>(IEnumerable<TSource> source, LinqHelperFunc<TSource, decimal> selector) {
        return Max(Select<TSource, decimal>(source, selector));
    }
    public static double Max<TSource>(IEnumerable<TSource> source, LinqHelperFunc<TSource, double> selector) {
        return Max(Select<TSource, double>(source, selector));
    }
    public static int Max<TSource>(IEnumerable<TSource> source, LinqHelperFunc<TSource, int> selector) {
        return Max(Select<TSource, int>(source, selector));
    }
    public static double? Max<TSource>(IEnumerable<TSource> source, LinqHelperFunc<TSource, double?> selector) {
        return Max(Select<TSource, double?>(source, selector));
    }
    public static long Max<TSource>(IEnumerable<TSource> source, LinqHelperFunc<TSource, long> selector) {
        return Max(Select<TSource, long>(source, selector));
    }
    public static decimal? Max<TSource>(IEnumerable<TSource> source, LinqHelperFunc<TSource, decimal?> selector) {
        return Max(Select<TSource, decimal?>(source, selector));
    }
    public static TResult Max<TSource, TResult>(IEnumerable<TSource> source, LinqHelperFunc<TSource, TResult> selector) {
        return Max(Select<TSource, TResult>(source, selector));
    }
    public static int? Max<TSource>(IEnumerable<TSource> source, LinqHelperFunc<TSource, int?> selector) {
        return Max(Select<TSource, int?>(source, selector));
    }
    public static long? Max<TSource>(IEnumerable<TSource> source, LinqHelperFunc<TSource, long?> selector) {
        return Max(Select<TSource, long?>(source, selector));
    }
    public static float? Max<TSource>(IEnumerable<TSource> source, LinqHelperFunc<TSource, float?> selector) {
        return Max(Select<TSource, float?>(source, selector));
    }
    public static float Max<TSource>(IEnumerable<TSource> source, LinqHelperFunc<TSource, float> selector) {
        return Max(Select<TSource, float>(source, selector));
    }
    #endregion

    #region Min
    public static double Min(IEnumerable<double> source) {
        if (source == null) {
            throw new ArgumentNullException("source");
        }
        double num = 0.0;
        bool flag = false;
        foreach (double num2 in source) {
            if (flag) {
                if ((num2 < num) || double.IsNaN(num2)) {
                    num = num2;
                }
                continue;
            }
            num = num2;
            flag = true;
        }
        if (!flag) {
            throw new Symbol.NotFoundException("source");
        }
        return num;
    }
    public static decimal? Min(IEnumerable<decimal?> source) {
        if (source == null) {
            throw new ArgumentNullException("source");
        }
        decimal? nullable = null;
        foreach (decimal? nullable2 in source) {
            if (nullable.HasValue) {
                decimal? nullable3 = nullable2;
                decimal? nullable4 = nullable;
                if ((nullable3.GetValueOrDefault() >= nullable4.GetValueOrDefault()) || !(nullable3.HasValue & nullable4.HasValue)) {
                    continue;
                }
            }
            nullable = nullable2;
        }
        return nullable;
    }
    public static double? Min(IEnumerable<double?> source) {
        if (source == null) {
            throw new ArgumentNullException("source");
        }
        double? nullable = null;
        foreach (double? nullable2 in source) {
            if (nullable2.HasValue) {
                if (nullable.HasValue) {
                    double? nullable3 = nullable2;
                    double? nullable4 = nullable;
                    if (((nullable3.GetValueOrDefault() >= nullable4.GetValueOrDefault()) || !(nullable3.HasValue & nullable4.HasValue)) && !double.IsNaN(nullable2.Value)) {
                        continue;
                    }
                }
                nullable = nullable2;
            }
        }
        return nullable;
    }
    public static int Min(IEnumerable<int> source) {
        if (source == null) {
            throw new ArgumentNullException("source");
        }
        int num = 0;
        bool flag = false;
        foreach (int num2 in source) {
            if (flag) {
                if (num2 < num) {
                    num = num2;
                }
            } else {
                num = num2;
                flag = true;
            }
        }
        if (!flag) {
            throw new Symbol.NotFoundException("source");
        }
        return num;
    }
    public static int? Min(IEnumerable<int?> source) {
        if (source == null) {
            throw new ArgumentNullException("source");
        }
        int? nullable = null;
        foreach (int? nullable2 in source) {
            if (nullable.HasValue) {
                int? nullable3 = nullable2;
                int? nullable4 = nullable;
                if ((nullable3.GetValueOrDefault() >= nullable4.GetValueOrDefault()) || !(nullable3.HasValue & nullable4.HasValue)) {
                    continue;
                }
            }
            nullable = nullable2;
        }
        return nullable;
    }
    public static long? Min(IEnumerable<long?> source) {
        if (source == null) {
            throw new ArgumentNullException("source");
        }
        long? nullable = null;
        foreach (long? nullable2 in source) {
            if (nullable.HasValue) {
                long? nullable3 = nullable2;
                long? nullable4 = nullable;
                if ((nullable3.GetValueOrDefault() >= nullable4.GetValueOrDefault()) || !(nullable3.HasValue & nullable4.HasValue)) {
                    continue;
                }
            }
            nullable = nullable2;
        }
        return nullable;
    }
    public static long Min(IEnumerable<long> source) {
        if (source == null) {
            throw new ArgumentNullException("source");
        }
        long num = 0;
        bool flag = false;
        foreach (long num2 in source) {
            if (flag) {
                if (num2 < num) {
                    num = num2;
                }
            } else {
                num = num2;
                flag = true;
            }
        }
        if (!flag) {
            throw new Symbol.NotFoundException("source");
        }
        return num;
    }
    public static float? Min(IEnumerable<float?> source) {
        if (source == null) {
            throw new ArgumentNullException("source");
        }
        float? nullable = null;
        foreach (float? nullable2 in source) {
            if (nullable2.HasValue) {
                if (nullable.HasValue) {
                    float? nullable3 = nullable2;
                    float? nullable4 = nullable;
                    if (((nullable3.GetValueOrDefault() >= nullable4.GetValueOrDefault()) || !(nullable3.HasValue & nullable4.HasValue)) && !float.IsNaN(nullable2.Value)) {
                        continue;
                    }
                }
                nullable = nullable2;
            }
        }
        return nullable;
    }
    public static TSource Min<TSource>(IEnumerable<TSource> source) {
        if (source == null) {
            throw new ArgumentNullException("source");
        }
        Comparer<TSource> comparer = Comparer<TSource>.Default;
        TSource y = default(TSource);
        if (y == null) {
            foreach (TSource local2 in source) {
                if ((local2 != null) && ((y == null) || (comparer.Compare(local2, y) < 0))) {
                    y = local2;
                }
            }
            return y;
        }
        bool flag = false;
        foreach (TSource local3 in source) {
            if (flag) {
                if (comparer.Compare(local3, y) < 0) {
                    y = local3;
                }
            } else {
                y = local3;
                flag = true;
            }
        }
        if (!flag) {
            throw new Symbol.NotFoundException("source");
        }
        return y;
    }
    public static decimal Min(IEnumerable<decimal> source) {
        if (source == null) {
            throw new ArgumentNullException("source");
        }
        decimal num = 0M;
        bool flag = false;
        foreach (decimal num2 in source) {
            if (flag) {
                if (num2 < num) {
                    num = num2;
                }
            } else {
                num = num2;
                flag = true;
            }
        }
        if (!flag) {
            throw new Symbol.NotFoundException("source");
        }
        return num;
    }
    public static float Min(IEnumerable<float> source) {
        if (source == null) {
            throw new ArgumentNullException("source");
        }
        float num = 0f;
        bool flag = false;
        foreach (float num2 in source) {
            if (flag) {
                if ((num2 < num) || float.IsNaN(num2)) {
                    num = num2;
                }
                continue;
            }
            num = num2;
            flag = true;
        }
        if (!flag) {
            throw new Symbol.NotFoundException("source");
        }
        return num;
    }

    public static decimal Min<TSource>(IEnumerable<TSource> source, LinqHelperFunc<TSource, decimal> selector) {
        return Min(Select<TSource, decimal>(source, selector));
    }
    public static int Min<TSource>(IEnumerable<TSource> source, LinqHelperFunc<TSource, int> selector) {
        return Min(Select<TSource, int>(source, selector));
    }
    public static double Min<TSource>(IEnumerable<TSource> source, LinqHelperFunc<TSource, double> selector) {
        return Min(Select<TSource, double>(source, selector));
    }
    public static decimal? Min<TSource>(IEnumerable<TSource> source, LinqHelperFunc<TSource, decimal?> selector) {
        return Min(Select<TSource, decimal?>(source, selector));
    }
    public static long Min<TSource>(IEnumerable<TSource> source, LinqHelperFunc<TSource, long> selector) {
        return Min(Select<TSource, long>(source, selector));
    }
    public static double? Min<TSource>(IEnumerable<TSource> source, LinqHelperFunc<TSource, double?> selector) {
        return Min(Select<TSource, double?>(source, selector));
    }
    public static int? Min<TSource>(IEnumerable<TSource> source, LinqHelperFunc<TSource, int?> selector) {
        return Min(Select<TSource, int?>(source, selector));
    }
    public static long? Min<TSource>(IEnumerable<TSource> source, LinqHelperFunc<TSource, long?> selector) {
        return Min(Select<TSource, long?>(source, selector));
    }
    public static TResult Min<TSource, TResult>(IEnumerable<TSource> source, LinqHelperFunc<TSource, TResult> selector) {
        return Min(Select<TSource, TResult>(source, selector));
    }
    public static float Min<TSource>(IEnumerable<TSource> source, LinqHelperFunc<TSource, float> selector) {
        return Min(Select<TSource, float>(source, selector));
    }
    public static float? Min<TSource>(IEnumerable<TSource> source, LinqHelperFunc<TSource, float?> selector) {
        return Min(Select<TSource, float?>(source, selector));
    }

    #endregion

    #region Sum
    public static decimal Sum(
#if !net20
        this 
#endif
        IEnumerable<decimal> source) {
        if (source == null) {
            throw new ArgumentNullException("source");
        }
        decimal num = 0M;
        foreach (decimal num2 in source) {
            num += num2;
        }
        return num;
    }

    public static double Sum(
#if !net20
        this 
#endif
        IEnumerable<double> source) {
        if (source == null) {
            throw new ArgumentNullException("source");
        }
        double num = 0.0;
        foreach (double num2 in source) {
            num += num2;
        }
        return num;
    }

    public static int Sum(
#if !net20
        this 
#endif
        IEnumerable<int> source) {
        if (source == null) {
            throw new ArgumentNullException("source");
        }
        int num = 0;
        foreach (int num2 in source) {
            num += num2;
        }
        return num;
    }

    public static long Sum(
#if !net20
        this 
#endif
        IEnumerable<long> source) {
        if (source == null) {
            throw new ArgumentNullException("source");
        }
        long num = 0;
        foreach (long num2 in source) {
            num += num2;
        }
        return num;
    }

    public static decimal? Sum(
#if !net20
        this 
#endif
        IEnumerable<decimal?> source) {
        if (source == null) {
            throw new ArgumentNullException("source");
        }
        decimal num = 0M;
        foreach (decimal? nullable in source) {
            if (nullable.HasValue) {
                num += nullable.GetValueOrDefault();
            }
        }
        return new decimal?(num);
    }

    public static double? Sum(
#if !net20
        this 
#endif
        IEnumerable<double?> source) {
        if (source == null) {
            throw new ArgumentNullException("source");
        }
        double num = 0.0;
        foreach (double? nullable in source) {
            if (nullable.HasValue) {
                num += nullable.GetValueOrDefault();
            }
        }
        return new double?(num);
    }

    public static int? Sum(
#if !net20
        this 
#endif
        IEnumerable<int?> source) {
        if (source == null) {
            throw new ArgumentNullException("source");
        }
        int num = 0;
        foreach (int? nullable in source) {
            if (nullable.HasValue) {
                num += nullable.GetValueOrDefault();
            }
        }
        return new int?(num);
    }

    public static long? Sum(
#if !net20
        this 
#endif
        IEnumerable<long?> source) {
        if (source == null) {
            throw new ArgumentNullException("source");
        }
        long num = 0;
        foreach (long? nullable in source) {
            if (nullable.HasValue) {
                num += nullable.GetValueOrDefault();
            }
        }
        return new long?(num);
    }

    public static float? Sum(
#if !net20
        this 
#endif
        IEnumerable<float?> source) {
        if (source == null) {
            throw new ArgumentNullException("source");
        }
        double num = 0.0;
        foreach (float? nullable in source) {
            if (nullable.HasValue) {
                num += (double)nullable.GetValueOrDefault();
            }
        }
        return new float?((float)num);
    }

    public static float Sum(
#if !net20
        this 
#endif
        IEnumerable<float> source) {
        if (source == null) {
            throw new ArgumentNullException("source");
        }
        double num = 0.0;
        foreach (float num2 in source) {
            num += num2;
        }
        return (float)num;
    }

    public static decimal? Sum<TSource>(
#if !net20
        this 
#endif
        IEnumerable<TSource> source, LinqHelperFunc<TSource, decimal?> selector) {
        return Sum(Select<TSource, decimal?>(source, selector));
    }

    public static decimal Sum<TSource>(
#if !net20
        this 
#endif
        IEnumerable<TSource> source, LinqHelperFunc<TSource, decimal> selector) {
        return Sum(Select<TSource, decimal>(source, selector));
    }

    public static double? Sum<TSource>(
#if !net20
        this 
#endif
        IEnumerable<TSource> source, LinqHelperFunc<TSource, double?> selector) {
        return Sum(Select<TSource, double?>(source, selector));
    }

    public static double Sum<TSource>(
#if !net20
        this 
#endif
        IEnumerable<TSource> source, LinqHelperFunc<TSource, double> selector) {
        return Sum(Select<TSource, double>(source, selector));
    }

    public static int? Sum<TSource>(
#if !net20
        this 
#endif
        IEnumerable<TSource> source, LinqHelperFunc<TSource, int?> selector) {
        return Sum(Select<TSource, int?>(source, selector));
    }

    public static int Sum<TSource>(
#if !net20
        this 
#endif
        IEnumerable<TSource> source, LinqHelperFunc<TSource, int> selector) {
        return Sum(Select<TSource, int>(source, selector));
    }

    public static long Sum<TSource>(
#if !net20
        this 
#endif
        IEnumerable<TSource> source, LinqHelperFunc<TSource, long> selector) {
        return Sum(Select<TSource, long>(source, selector));
    }

    public static long? Sum<TSource>(
#if !net20
        this 
#endif
        IEnumerable<TSource> source, LinqHelperFunc<TSource, long?> selector) {
        return Sum(Select<TSource, long?>(source, selector));
    }

    public static float? Sum<TSource>(
#if !net20
        this 
#endif
        IEnumerable<TSource> source, LinqHelperFunc<TSource, float?> selector) {
        return Sum(Select<TSource, float?>(source, selector));
    }

    public static float Sum<TSource>(
#if !net20
        this 
#endif
        IEnumerable<TSource> source, LinqHelperFunc<TSource, float> selector) {
        return Sum(Select<TSource, float>(source, selector));
    }
    #endregion

    #region OrderBy
    public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(IEnumerable<TSource> source, LinqHelperFunc<TSource, TKey> keySelector) {
        return new OrderedEnumerable<TSource, TKey>(source, keySelector, null, false);
    }
    public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(IEnumerable<TSource> source, LinqHelperFunc<TSource, TKey> keySelector, IComparer<TKey> comparer) {
        return new OrderedEnumerable<TSource, TKey>(source, keySelector, comparer, false);
    }
    #endregion

    #region OrderByDescending
    public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(IEnumerable<TSource> source, LinqHelperFunc<TSource, TKey> keySelector) {
        return new OrderedEnumerable<TSource, TKey>(source, keySelector, null, true);
    }
    public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(IEnumerable<TSource> source, LinqHelperFunc<TSource, TKey> keySelector, IComparer<TKey> comparer) {
        return new OrderedEnumerable<TSource, TKey>(source, keySelector, comparer, true);
    }
    #endregion

    #region ThenBy
    public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(IOrderedEnumerable<TSource> source, LinqHelperFunc<TSource, TKey> keySelector) {
        if (source == null) {
            throw new ArgumentNullException("source");
        }
        return source.CreateOrderedEnumerable<TKey>(keySelector, null, false);
    }

    public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(IOrderedEnumerable<TSource> source, LinqHelperFunc<TSource, TKey> keySelector, IComparer<TKey> comparer) {
        if (source == null) {
            throw new ArgumentNullException("source");
        }
        return source.CreateOrderedEnumerable<TKey>(keySelector, comparer, false);
    }
    #endregion

    #region ThenByDescending
    public static IOrderedEnumerable<TSource> ThenByDescending<TSource, TKey>(IOrderedEnumerable<TSource> source, LinqHelperFunc<TSource, TKey> keySelector) {
        if (source == null) {
            throw new ArgumentNullException("source");
        }
        return source.CreateOrderedEnumerable<TKey>(keySelector, null, true);
    }

    public static IOrderedEnumerable<TSource> ThenByDescending<TSource, TKey>(IOrderedEnumerable<TSource> source, LinqHelperFunc<TSource, TKey> keySelector, IComparer<TKey> comparer) {
        if (source == null) {
            throw new ArgumentNullException("source");
        }
        return source.CreateOrderedEnumerable<TKey>(keySelector, comparer, true);
    }
    #endregion

    #region types
    /// <summary>
    /// 通用委托（一个参数+返回类型）。
    /// </summary>
    /// <typeparam name="T">参数1的类型。</typeparam>
    /// <typeparam name="TResult">返回的类型。</typeparam>
    /// <param name="arg">参数1。</param>
    /// <returns>返回值。</returns>
    public delegate TResult LinqHelperFunc<in T, out TResult>(T arg);

    #region IQueryable<TSource>
    /// <summary>
    /// 可查询接口（用于兼容无Linq的平台）。
    /// </summary>
    /// <typeparam name="TSource">任意类型。</typeparam>
    public interface IQueryable<TSource> : IEnumerable<TSource> {
        /// <summary>
        /// 集合中至少有一个成员时返回true。
        /// </summary>
        /// <returns></returns>
        bool Any();
        /// <summary>
        /// 集合中至少有一个成员满足条件时返回true。
        /// </summary>
        /// <param name="predicate">匹配规则。</param>
        /// <returns></returns>
        bool Any(Predicate<TSource> predicate);
        /// <summary>
        /// 集合中的成员数。
        /// </summary>
        /// <returns></returns>
        int Count();
        /// <summary>
        /// 集合中成员满足条件的成员数。
        /// </summary>
        /// <param name="predicate">匹配规则。</param>
        /// <returns></returns>
        int Count(Predicate<TSource> predicate);
        /// <summary>
        /// 集合中第一个成员。
        /// </summary>
        /// <returns></returns>
        TSource FirstOrDefault();
        /// <summary>
        /// 集合中第一个满足条件的成员。
        /// </summary>
        /// <param name="predicate">匹配规则。</param>
        /// <returns></returns>
        TSource FirstOrDefault(Predicate<TSource> predicate);
        /// <summary>
        /// 将集合中的成员输出为System.Collections.Generic.List&lt;TSource&gt;。
        /// </summary>
        /// <returns></returns>
        List<TSource> ToList();
        /// <summary>
        /// 将集合中满足条件的成员输出为System.Collections.Generic.List&lt;TSource&gt;。
        /// </summary>
        /// <param name="predicate">匹配规则。</param>
        /// <returns></returns>
        List<TSource> ToList(Predicate<TSource> predicate);
        /// <summary>
        /// 将集合中的成员输出数TSource[]数组。
        /// </summary>
        /// <returns></returns>
        TSource[] ToArray();
        /// <summary>
        /// 将集合中满足条件的成员输出数TSource[]数组。
        /// </summary>
        /// <param name="predicate">匹配规则。</param>
        /// <returns></returns>
        TSource[] ToArray(Predicate<TSource> predicate);
        /// <summary>
        /// 过滤集合中的成员。
        /// </summary>
        /// <param name="predicate">匹配规则。</param>
        /// <returns></returns>
        IQueryable<TSource> Where(LinqHelperFunc<TSource, bool> predicate);
        /// <summary>
        /// 跳过集合中的指定成员数（常用于翻页）。
        /// </summary>
        /// <param name="count">跳过数量，为0时不跳过。</param>
        /// <returns></returns>
        IQueryable<TSource> Skip(int count);
        /// <summary>
        /// 从集合中输出指定成员数（常用于翻页）。
        /// </summary>
        /// <param name="count">输出数量。</param>
        /// <returns></returns>
        IQueryable<TSource> Take(int count);
        /// <summary>
        /// 重新定义成员输出结构。
        /// </summary>
        /// <typeparam name="TResult">任意类型。</typeparam>
        /// <param name="selector">成员输出结构定义规则。</param>
        /// <returns></returns>
        IQueryable<TResult> Select<TResult>(LinqHelperFunc<TSource, TResult> selector);
        /// <summary>
        /// 排序（升序/顺序）。
        /// </summary>
        /// <typeparam name="TKey">任意类型。</typeparam>
        /// <param name="keySelector">用于排序的对象。</param>
        /// <returns></returns>
        IOrderedEnumerable<TSource> OrderBy<TKey>(LinqHelperFunc<TSource, TKey> keySelector);
        /// <summary>
        /// 排序（升序/顺序）。
        /// </summary>
        /// <typeparam name="TKey">任意类型。</typeparam>
        /// <param name="keySelector">用于排序的对象。</param>
        /// <param name="comparer">对象比较规则。</param>
        /// <returns></returns>
        IOrderedEnumerable<TSource> OrderBy<TKey>(LinqHelperFunc<TSource, TKey> keySelector, IComparer<TKey> comparer);
        /// <summary>
        /// 排序（降序/逆序）。
        /// </summary>
        /// <typeparam name="TKey">任意类型。</typeparam>
        /// <param name="keySelector">用于排序的对象。</param>
        /// <returns></returns>
        IOrderedEnumerable<TSource> OrderByDescending<TKey>(LinqHelperFunc<TSource, TKey> keySelector);
        /// <summary>
        /// 排序（降序/逆序）。
        /// </summary>
        /// <typeparam name="TKey">任意类型。</typeparam>
        /// <param name="keySelector">用于排序的对象。</param>
        /// <param name="comparer">对象比较规则。</param>
        /// <returns></returns>
        IOrderedEnumerable<TSource> OrderByDescending<TKey>(LinqHelperFunc<TSource, TKey> keySelector, IComparer<TKey> comparer);
    }
    #endregion
    #region IOrderedEnumerable<TElement>
    /// <summary>
    /// 已排序的集合接口。
    /// </summary>
    /// <typeparam name="TElement">任意类型。</typeparam>
    public interface IOrderedEnumerable<TElement> : IQueryable<TElement> {
        /// <summary>
        /// 创建排序集合。
        /// </summary>
        /// <typeparam name="TKey">任意类型。</typeparam>
        /// <param name="keySelector">用于排序的对象。</param>
        /// <param name="comparer">对象比较规则。</param>
        /// <param name="descending">是否为降序</param>
        /// <returns></returns>
        IOrderedEnumerable<TElement> CreateOrderedEnumerable<TKey>(LinqHelperFunc<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending);

        /// <summary>
        /// 次级排序（升序/顺序）。
        /// </summary>
        /// <typeparam name="TKey">任意类型。</typeparam>
        /// <param name="keySelector">用于排序的对象。</param>
        /// <returns></returns>
        IOrderedEnumerable<TElement> ThenBy<TKey>(LinqHelperFunc<TElement, TKey> keySelector);
        /// <summary>
        /// 次级排序（升序/顺序）。
        /// </summary>
        /// <typeparam name="TKey">任意类型。</typeparam>
        /// <param name="keySelector">用于排序的对象。</param>
        /// <param name="comparer">对象比较规则。</param>
        /// <returns></returns>
        IOrderedEnumerable<TElement> ThenBy<TKey>(LinqHelperFunc<TElement, TKey> keySelector, IComparer<TKey> comparer);
        /// <summary>
        /// 次级排序（降序/逆序）。
        /// </summary>
        /// <typeparam name="TKey">任意类型。</typeparam>
        /// <param name="keySelector">用于排序的对象。</param>
        /// <returns></returns>
        IOrderedEnumerable<TElement> ThenByDescending<TKey>(LinqHelperFunc<TElement, TKey> keySelector);
        /// <summary>
        /// 次级排序（降序/逆序）。
        /// </summary>
        /// <typeparam name="TKey">任意类型。</typeparam>
        /// <param name="keySelector">用于排序的对象。</param>
        /// <param name="comparer">对象比较规则。</param>
        /// <returns></returns>
        IOrderedEnumerable<TElement> ThenByDescending<TKey>(LinqHelperFunc<TElement, TKey> keySelector, IComparer<TKey> comparer);
    }
    #endregion
    #region Ordered classes
    abstract class OrderedEnumerable<TElement> : Queryable<TElement>, IOrderedEnumerable<TElement> {
        // Fields
        internal IEnumerable<TElement> source;

        // Methods
        protected OrderedEnumerable() {
        }

        internal abstract EnumerableSorter<TElement> GetEnumerableSorter(EnumerableSorter<TElement> next);
        public override IEnumerator<TElement> GetEnumerator() {
            Buffer<TElement> iteratorVariable0 = new Buffer<TElement>(this.source);
            if (iteratorVariable0.count <= 0) {
                goto Label_00EA;
            }
            int[] iteratorVariable2 = this.GetEnumerableSorter(null).Sort(iteratorVariable0.items, iteratorVariable0.count);
            int index = 0;
        Label_PostSwitchInIterator: ;
            if (index < iteratorVariable0.count) {
                yield return iteratorVariable0.items[iteratorVariable2[index]];
                index++;
                goto Label_PostSwitchInIterator;
            }
        Label_00EA: ;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        public IOrderedEnumerable<TElement> CreateOrderedEnumerable<TKey>(LinqHelperFunc<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending) {
            return new OrderedEnumerable<TElement, TKey>(this.source, keySelector, comparer, descending) { parent = (OrderedEnumerable<TElement>)this };
        }
        #region ThenBy
        public IOrderedEnumerable<TElement> ThenBy<TKey>(LinqHelperFunc<TElement, TKey> keySelector) {
            return CreateOrderedEnumerable<TKey>(keySelector, null, false);
        }

        public IOrderedEnumerable<TElement> ThenBy<TKey>(LinqHelperFunc<TElement, TKey> keySelector, IComparer<TKey> comparer) {
            return CreateOrderedEnumerable<TKey>(keySelector, comparer, false);
        }
        #endregion
        #region ThenByDescending
        public IOrderedEnumerable<TElement> ThenByDescending<TKey>(LinqHelperFunc<TElement, TKey> keySelector) {
            return CreateOrderedEnumerable<TKey>(keySelector, null, true);
        }

        public IOrderedEnumerable<TElement> ThenByDescending<TKey>(LinqHelperFunc<TElement, TKey> keySelector, IComparer<TKey> comparer) {
            return CreateOrderedEnumerable<TKey>(keySelector, comparer, true);
        }
        #endregion

        private sealed class _GetEnumerator_d__0 : IEnumerator<TElement> {
            // Fields
            private int __1__state;
            private TElement __2__current;
            public OrderedEnumerable<TElement> __4__this=null;
            public Buffer<TElement> _buffer_5__1;
            public int _i_5__4;
            public int[] _map_5__3;
            public EnumerableSorter<TElement> _sorter_5__2;

            public _GetEnumerator_d__0(int __1__state) {
                this.__1__state = __1__state;
            }

            public bool MoveNext() {
                switch (this.__1__state) {
                    case 0:
                        this.__1__state = -1;
                        this._buffer_5__1 = new Buffer<TElement>(this.__4__this.source);
                        if (this._buffer_5__1.count <= 0) {
                            goto Label_00EA;
                        }
                        this._sorter_5__2 = this.__4__this.GetEnumerableSorter(null);
                        this._map_5__3 = this._sorter_5__2.Sort(this._buffer_5__1.items, this._buffer_5__1.count);
                        this._sorter_5__2 = null;
                        this._i_5__4 = 0;
                        break;

                    case 1:
                        this.__1__state = -1;
                        this._i_5__4++;
                        break;

                    default:
                        goto Label_00EA;
                }
                if (this._i_5__4 < this._buffer_5__1.count) {
                    this.__2__current = this._buffer_5__1.items[this._map_5__3[this._i_5__4]];
                    this.__1__state = 1;
                    return true;
                }
            Label_00EA:
                return false;
            }

            void System.Collections.IEnumerator.Reset() {
                throw new NotSupportedException();
            }

            void IDisposable.Dispose() {
            }

            // Properties
            TElement IEnumerator<TElement>.Current {
                get {
                    return this.__2__current;
                }
            }

            object System.Collections.IEnumerator.Current {
                get {
                    return this.__2__current;
                }
            }
        }
    }
    class OrderedEnumerable<TElement, TKey> : OrderedEnumerable<TElement> {
        // Fields
        internal IComparer<TKey> comparer;
        internal bool descending;
        internal LinqHelperFunc<TElement, TKey> keySelector;
        internal OrderedEnumerable<TElement> parent;

        // Methods
        internal OrderedEnumerable(IEnumerable<TElement> source, LinqHelperFunc<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending) {
            if (source == null) {
                throw new ArgumentNullException("source");
            }
            if (keySelector == null) {
                throw new ArgumentNullException("keySelector");
            }
            base.source = source;
            this.parent = null;
            this.keySelector = keySelector;
            this.comparer = (comparer != null) ? comparer : ((IComparer<TKey>)Comparer<TKey>.Default);
            this.descending = descending;
        }

        internal override EnumerableSorter<TElement> GetEnumerableSorter(EnumerableSorter<TElement> next) {
            EnumerableSorter<TElement> enumerableSorter = new EnumerableSorter<TElement, TKey>(this.keySelector, this.comparer, this.descending, next);
            if (this.parent != null) {
                enumerableSorter = this.parent.GetEnumerableSorter(enumerableSorter);
            }
            return enumerableSorter;
        }
    }
    internal abstract class EnumerableSorter<TElement> {
        // Methods
        protected EnumerableSorter() {
        }

        internal abstract int CompareKeys(int index1, int index2);
        internal abstract void ComputeKeys(TElement[] elements, int count);
        private void QuickSort(int[] map, int left, int right) {
            do {
                int index = left;
                int num2 = right;
                int num3 = map[index + ((num2 - index) >> 1)];
                do {
                    while ((index < map.Length) && (this.CompareKeys(num3, map[index]) > 0)) {
                        index++;
                    }
                    while ((num2 >= 0) && (this.CompareKeys(num3, map[num2]) < 0)) {
                        num2--;
                    }
                    if (index > num2) {
                        break;
                    }
                    if (index < num2) {
                        int num4 = map[index];
                        map[index] = map[num2];
                        map[num2] = num4;
                    }
                    index++;
                    num2--;
                }
                while (index <= num2);
                if ((num2 - left) <= (right - index)) {
                    if (left < num2) {
                        this.QuickSort(map, left, num2);
                    }
                    left = index;
                } else {
                    if (index < right) {
                        this.QuickSort(map, index, right);
                    }
                    right = num2;
                }
            }
            while (left < right);
        }

        internal int[] Sort(TElement[] elements, int count) {
            this.ComputeKeys(elements, count);
            int[] map = new int[count];
            for (int i = 0; i < count; i++) {
                map[i] = i;
            }
            this.QuickSort(map, 0, count - 1);
            return map;
        }
    }
    internal class EnumerableSorter<TElement, TKey> : EnumerableSorter<TElement> {
        // Fields
        internal IComparer<TKey> comparer;
        internal bool descending;
        internal TKey[] keys;
        internal LinqHelperFunc<TElement, TKey> keySelector;
        internal EnumerableSorter<TElement> next;

        // Methods
        internal EnumerableSorter(LinqHelperFunc<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending, EnumerableSorter<TElement> next) {
            this.keySelector = keySelector;
            this.comparer = comparer;
            this.descending = descending;
            this.next = next;
        }

        internal override int CompareKeys(int index1, int index2) {
            int num = this.comparer.Compare(this.keys[index1], this.keys[index2]);
            if (num == 0) {
                if (this.next == null) {
                    return (index1 - index2);
                }
                return this.next.CompareKeys(index1, index2);
            }
            if (!this.descending) {
                return num;
            }
            return -num;
        }

        internal override void ComputeKeys(TElement[] elements, int count) {
            this.keys = new TKey[count];
            for (int i = 0; i < count; i++) {
                this.keys[i] = this.keySelector(elements[i]);
            }
            if (this.next != null) {
                this.next.ComputeKeys(elements, count);
            }
        }
    }
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    internal struct Buffer<TElement> {
        internal TElement[] items;
        internal int count;
        internal Buffer(IEnumerable<TElement> source) {
            TElement[] array = null;
            int length = 0;
            ICollection<TElement> is2 = source as ICollection<TElement>;
            if (is2 != null) {
                length = is2.Count;
                if (length > 0) {
                    array = new TElement[length];
                    is2.CopyTo(array, 0);
                }
            } else {
                foreach (TElement local in source) {
                    if (array == null) {
                        array = new TElement[4];
                    } else if (array.Length == length) {
                        TElement[] destinationArray = new TElement[length * 2];
                        Array.Copy(array, 0, destinationArray, 0, length);
                        array = destinationArray;
                    }
                    array[length] = local;
                    length++;
                }
            }
            this.items = array;
            this.count = length;
        }

        internal TElement[] ToArray() {
            if (this.count == 0) {
                return new TElement[0];
            }
            if (this.items.Length == this.count) {
                return this.items;
            }
            TElement[] destinationArray = new TElement[this.count];
            Array.Copy(this.items, 0, destinationArray, 0, this.count);
            return destinationArray;
        }
    }
    #endregion


    #endregion
}

#pragma warning restore 1591
