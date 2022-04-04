using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cuture.Http.Test.Internal;

[TestClass]
public class SortedQueueTest
{
    #region Private 字段

    private static readonly Random s_random = new Random();
    private SortedQueue<CreatedTimeTagedObject<EmptyTestClass>> _queue;

    #endregion Private 字段

    #region Public 方法

    [TestCleanup]
    public void Cleanup()
    {
        _queue.Clear();
        _queue = null;
    }

    [TestMethod]
    [DataRow(100)]
    [DataRow(1_000)]
    [DataRow(1_00_000)]
    public void GetSetTest(int count)
    {
        FillQueue(count);
        int optCount = 0;
        while (_queue.Count > 0)
        {
            var bpCount = _queue.Count;
            var peeked = _queue.Peek();
            var apCount = _queue.Count;
            Assert.AreEqual(bpCount, apCount);

            var dequeue = _queue.Dequeue();
            var dqCount = _queue.Count;

            Assert.AreEqual(peeked, dequeue);
            Assert.AreEqual(apCount - 1, dqCount);

            optCount++;
        }

        Assert.AreEqual(count, optCount);
    }

    [TestInitialize]
    public void Init()
    {
        _queue = new SortedQueue<CreatedTimeTagedObject<EmptyTestClass>>();
    }

    [TestMethod]
    [DataRow(100)]
    [DataRow(1_000)]
    [DataRow(1_00_000)]
    public void SortTest(int count)
    {
        FillQueue(count);
        var last = _queue.Dequeue();
        while (_queue.Count > 0)
        {
            var current = _queue.Dequeue();
            Assert.IsTrue(current.CreatedTime >= last.CreatedTime);
        }
    }

    #endregion Public 方法

    #region Private 方法

    private void FillQueue(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var time = DateTime.UtcNow.AddSeconds(s_random.Next(-3600, 3600));
            //插入失败
            while (!_queue.Enqueue(new CreatedTimeTagedObject<EmptyTestClass>(new EmptyTestClass(), time)))
            {
            }
        }
        Assert.AreEqual(count, _queue.Count);
    }

    #endregion Private 方法

    #region Private 类

    private class EmptyTestClass
    {
    }

    #endregion Private 类
}
