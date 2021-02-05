using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PyLibSharp.Common
{
    /// <summary>
    /// 排序方式
    /// </summary>
    public enum OrderBy
    {
        /// <summary>
        /// 升序
        /// </summary>
        Asc = 0,

        /// <summary>
        /// 降序
        /// </summary>
        Desc = 1
    }

    public class StepEqualsZeroException : Exception
    {
        public  string    error = "步长不能为0";
        private Exception innerException;

        public StepEqualsZeroException()
        {
        }

        public StepEqualsZeroException(string msg) : base(msg)
        {
            this.error = msg;
        }

        public StepEqualsZeroException(string msg, Exception innerException) : base(msg, innerException)
        {
            this.innerException = innerException;
            error               = msg;
        }

        public string GetError()
        {
            return error;
        }
    }

    /// <summary>
    /// 可排序，不可重复的整数范围
    /// </summary>
    public class Range : IEnumerable<int>, IEquatable<Range>
    {
        private List<int> rangeData;

        /// <summary>
        /// 范围内的数据
        /// </summary>
        public List<int> data
        {
            get => rangeData;
            set => rangeData = value.Distinct().ToList();
        }

        /// <summary>
        /// 默认排序方式
        /// </summary>
        public OrderBy orderBy = OrderBy.Asc;

        /// <summary>
        /// 创建一个范围
        /// </summary>
        /// <param name="start">开始的数字</param>
        /// <param name="stop">终止的数字</param>
        /// <param name="step">步长</param>
        /// <returns></returns>
        public static Range range(int start, int stop, int step = 1)
            => new Range(start, stop, step);

        /// <summary>
        /// 用一个可迭代序列来初始化一个范围
        /// </summary>
        /// <param name="data"></param>
        public Range(IEnumerable<int> data)
        {
            rangeData = data.Distinct().ToList();
        }

        /// <summary>
        /// 用一个整数列表来初始化一个范围
        /// </summary>
        /// <param name="data"></param>
        public Range(List<int> data)
        {
            rangeData = data.Distinct().ToList();
        }

        public static implicit operator List<int>(Range range)
        {
            return range.rangeData;
        }

        public static implicit operator Range(List<int> intList)
        {
            return new Range(intList);
        }

        public static Range operator +(Range a, Range b)
            => a.AddRange(b);

        public static Range operator -(Range a, Range b)
            => a.RemoveRange(b);

        public static Range operator +(Range a, int b)
            => a.Add(b);

        public static Range operator -(Range a, int b)
            => a.Remove(b);


        /// <summary>
        /// 针对范围中每一个数都乘一个数
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Range operator *(Range a, int b)
        {
            for (var i = 0; i < a.rangeData.Count; i++)
            {
                if (!a.data.Contains(a[i] * b))
                    a[i] *= b;
            }

            return a;
        }

        /// <summary>
        /// 针对范围中每一个数都除一个数（若有小数将直接抹掉）
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Range operator /(Range a, int b)
        {
            for (var i = 0; i < a.rangeData.Count; i++)
            {
                if (!a.data.Contains(a[i] / b))
                    a[i] /= b;
            }

            return a;
        }

        /// <summary>
        /// 获取指定下标位置的数据
        /// </summary>
        /// <param name="index">下标号（从0开始）</param>
        /// <returns></returns>
        public int this[int index]
        {
            get => rangeData[index];
            set
            {
                //不能出现重复项
                if (rangeData.All(i => i != value))
                {
                    rangeData[index] = value;
                }
            }
        }

        /// <summary>
        /// 获取范围中，从startIndex下标到endIndex下标的数据，默认步长为1，但可调
        /// </summary>
        /// <param name="startIndex">开始的下标（从0开始）</param>
        /// <param name="endIndex">结束的下标（从0开始）</param>
        /// <param name="step">步长</param>
        /// <returns></returns>
        public Range this[int startIndex, int endIndex, int step = 1]
        {
            get
            {
                Range ret = new Range();
                for (var i = Math.Min(startIndex, endIndex); i <= Math.Max(startIndex, endIndex); i += Math.Abs(step))
                {
                    ret.Add(rangeData[i]);
                }

                return ret;
            }
        }

        public Range()
        {
            rangeData = new List<int>();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="count">从0或1开始生成多少个数字</param>
        /// <param name="startAtZero">
        /// 是否从0开始。
        /// <para>例如，假设count=7，且此参数为true：</para>
        /// <para>  范围是：0~6</para>
        /// <para>否则：</para>
        /// <para>  范围是：1~7</para>
        /// </param>
        /// <param name="orderBy">排序方式</param>
        public Range(int count, bool startAtZero = true, OrderBy orderBy = OrderBy.Asc)
        {
            var data = new List<int>();
            if (count <= 0)
            {
                rangeData = data;
                return;
            }

            for (int i = startAtZero ? 0 : 1; (startAtZero ? i < count : i <= count); i++)
            {
                data.Add(i);
            }

            rangeData = data;
        }

        /// <summary>
        /// 构造函数
        /// 构造一个[start,stop]范围的
        /// <see>
        ///     <cref>List{int}</cref>
        /// </see>
        /// </summary>
        /// <param name="start">范围开始的数字（包含这个数字）</param>
        /// <param name="stop">范围终止的数（包含这个数字）</param>
        /// <param name="step">范围增加步长（可为负数）</param>
        public Range(int start, int stop, int step = 1)
        {
            var data = new List<int>();
            if (step == 0) throw new StepEqualsZeroException();

            if (start > stop || step < 0)
            {
                orderBy = OrderBy.Desc;
            }

            for (int i = Math.Min(start, stop); i <= Math.Max(start, stop); i += Math.Abs(step))
            {
                data.Add(i);
            }

            rangeData = data;
            Sort();
        }

        public Range AddRange(Range toAdd)
        {
            foreach (int item in toAdd.ToList())
            {
                if (!rangeData.Contains(item))
                    rangeData.Add(item);
            }

            return (rangeData);
        }

        public Range RemoveRange(Range toRemove)
        {
            foreach (int item in toRemove.ToList())
            {
                if (rangeData.Contains(item))
                    rangeData.Remove(item);
            }

            return (rangeData);
        }

        public Range Add(int toAdd)
        {
            if (!rangeData.Contains(toAdd))
                rangeData.Add(toAdd);

            return (rangeData);
        }

        public Range Remove(int toRemove)
        {
            if (rangeData.Contains(toRemove))
                rangeData.Remove(toRemove);

            return (rangeData);
        }

        public Range Sort()
        {
            rangeData.Sort();
            if (orderBy == OrderBy.Desc)
            {
                rangeData.Reverse();
            }

            return (rangeData);
        }

        public Range SortBy(OrderBy order)
        {
            rangeData.Sort();
            if (order == OrderBy.Desc)
            {
                rangeData.Reverse();
            }

            return (rangeData);
        }

        /// <summary>
        /// 范围是否包含某一个数字
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public bool IsIn(int i)
            => rangeData.Contains(i);

        /// <summary>
        /// 某一个范围是否在该范围之内（交集是否为本身）
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public bool IsIn(Range range)
        {
            if (rangeData.Count < range.Count())
            {
                return false;
            }

            foreach (int item in range.ToList())
            {
                if (!rangeData.Contains(item))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 范围是否包含不某一个数字
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public bool IsOut(int i)
            => !((Range) rangeData).IsIn(i);

        /// <summary>
        /// 范围是否不包含某一个范围（交集是否为空）
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public bool IsOut(Range i)
            => !((Range) rangeData).IsIn(i);

        public List<int> AsList()
            => rangeData;

        public List<int> ToList()
            => rangeData;

        public Range CopyOne()
        {
            var ret = new Range();
            return ret.AddRange(this);
        }

        public IEnumerator<int> GetEnumerator()
        {
            return ((IEnumerable<int>) rangeData).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) rangeData).GetEnumerator();
        }

        public override bool Equals(object? value)
        {
            if (!(value is Range))
            {
                return false;
            }

            return (Range) value == this;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(rangeData);
        }

        public static bool operator ==(Range one, Range two)
            => one?.Equals(two) ?? false;

        public static bool operator !=(Range one, Range two)
            => !(one == two);

        public bool Equals(Range other)
        {
            if (this.Count() != other.Count()) return false;

            var a = CopyOne().SortBy(OrderBy.Asc);
            var b = other?.CopyOne().SortBy(OrderBy.Asc);
            for (int i = 0; i < a?.Count(); i++)
            {
                if (a[i] != b?[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}