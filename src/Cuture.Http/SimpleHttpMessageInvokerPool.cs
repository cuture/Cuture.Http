using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;

namespace Cuture.Http;

/// <summary>
/// 简单实现的 <see cref="IHttpMessageInvokerPool"/>
/// </summary>
public sealed class SimpleHttpMessageInvokerPool : IHttpMessageInvokerPool
{
    #region Public 字段

    /// <summary>
    /// 存活时间
    /// </summary>
    public static readonly TimeSpan DefaultAliveTime = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Dispose延时
    /// </summary>
    public static readonly TimeSpan DefaultDisposeDelay = TimeSpan.FromSeconds(30);

    #endregion Public 字段

    #region 字段

    /// <summary>
    /// 存活时间
    /// </summary>
    private readonly TimeSpan _aliveTime;

    /// <summary>
    /// Dispose延时
    /// </summary>
    private readonly TimeSpan _disposeDelay;

    private readonly ConcurrentQueue<DisposeQueueItem> _disposeQueue = new();

    private readonly CancellationTokenSource _runningTokenSource = new();

    /// <summary>
    /// 是否已释放
    /// </summary>
    private volatile bool _isDisposed;

    #region Client

    /// <summary>
    /// 默认的 <see cref="StrictDisposeOwnedHttpMessageInvoker"/>
    /// </summary>
    private readonly PlaceHolder<StrictDisposeOwnedHttpMessageInvoker> _defaultInvoker = new();

    /// <summary>
    /// 禁用Proxy的 <see cref="StrictDisposeOwnedHttpMessageInvoker"/>
    /// </summary>
    private readonly PlaceHolder<StrictDisposeOwnedHttpMessageInvoker> _noneProxyInvoker = new();

    #endregion Client

    #region Client Dictionary

    /// <summary>
    /// 有代理信息的 <see cref="StrictDisposeOwnedHttpMessageInvoker"/>
    /// </summary>
    private readonly ConcurrentDictionary<int, PlaceHolder<StrictDisposeOwnedHttpMessageInvoker>> _proxyedInvokers = new();

    #endregion Client Dictionary

    #endregion 字段

    #region Public 构造函数

    /// <inheritdoc cref="SimpleHttpMessageInvokerPool"/>
    public SimpleHttpMessageInvokerPool() : this(DefaultAliveTime, DefaultDisposeDelay)
    {
    }

    /// <inheritdoc cref="SimpleHttpMessageInvokerPool"/>
    public SimpleHttpMessageInvokerPool(TimeSpan aliveTime, TimeSpan disposeDelay)
    {
        _aliveTime = aliveTime > TimeSpan.Zero ? aliveTime : throw new ArgumentOutOfRangeException(nameof(aliveTime));
        _disposeDelay = disposeDelay > TimeSpan.Zero ? disposeDelay : throw new ArgumentOutOfRangeException(nameof(disposeDelay));

        StartDisposeLoopTask();
    }

    #endregion Public 构造函数

    #region Private 析构函数

    /// <summary>
    ///
    /// </summary>
    ~SimpleHttpMessageInvokerPool()
    {
        Dispose();
    }

    #endregion Private 析构函数

    #region 方法

    /// <summary>
    /// 销毁相关资源
    /// </summary>
    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;

        try
        {
            _runningTokenSource.Cancel();
        }
        catch { }

        _runningTokenSource.Dispose();

        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public IOwner<HttpMessageInvoker> Rent(IHttpRequest request)
    {
        CheckDisposed();

        return request.DisableProxy
               ? InternalRent(_noneProxyInvoker, () => new(this, HttpClientUtil.CreateNoProxyClientHandler()))
               : request.Proxy is null
                    ? InternalRentDefaultInvoker()
                    : InternalRentProxyedInvoker(request.Proxy, request.RequestUri);
    }

    #endregion 方法

    #region Private 方法

    private void CheckDisposed()
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException(nameof(SimpleHttpMessageInvokerPool));
        }
    }

    private StrictDisposeOwnedHttpMessageInvoker InternalRent(PlaceHolder<StrictDisposeOwnedHttpMessageInvoker> placeHolder, Func<StrictDisposeOwnedHttpMessageInvoker> factory)
    {
        if (placeHolder.Value is StrictDisposeOwnedHttpMessageInvoker invoker)
        {
            invoker.Reference();
            return invoker;
        }

        invoker = factory();
        invoker.Reference();

        QueueToDispose(placeHolder.Exchange(invoker));

        return invoker;
    }

    private StrictDisposeOwnedHttpMessageInvoker InternalRentDefaultInvoker()
    {
        return InternalRent(_defaultInvoker, () => new(this, HttpClientUtil.CreateDefaultClientHandler()));
    }

    private StrictDisposeOwnedHttpMessageInvoker InternalRentProxyedInvoker(IWebProxy proxy, Uri requestUri)
    {
        if (proxy.IsBypassed(requestUri))
        {
            return InternalRentDefaultInvoker();
        }

        var proxyHash = new HashCode();
        if (proxy is WebProxy webProxy)
        {
            proxyHash.Add(webProxy.Address);
        }
        else
        {
            var proxyUri = proxy.GetProxy(requestUri);
            if (proxyUri is null)
            {
                return InternalRentDefaultInvoker();
            }
            proxyHash.Add(proxyUri);
        }

        if (proxy.Credentials?.GetCredential(requestUri, string.Empty) is NetworkCredential credential)
        {
            proxyHash.Add(credential.UserName);
            proxyHash.Add(credential.Password);
            proxyHash.Add(credential.Domain);
        }

        var placeHolder = _proxyedInvokers.GetOrAdd(proxyHash.ToHashCode(), static _ => new());

        return InternalRent(placeHolder, () => new(this, HttpClientUtil.CreateProxyedClientHandler(proxy)));
    }

    private void QueueToDispose(StrictDisposeOwnedHttpMessageInvoker? invoker)
    {
        if (invoker is not null)
        {
            _disposeQueue.Enqueue(new(DateTimeOffset.UtcNow, invoker));
        }
    }

    private void StartDisposeLoopTask()
    {
        var runningToken = _runningTokenSource.Token;

        Task.Run(async () =>
        {
            try
            {
                object? turnFirstItem = null;
                while (!runningToken.IsCancellationRequested)
                {
                    if (_disposeQueue.TryDequeue(out var disposeQueueItem))
                    {
                        var interval = DateTimeOffset.UtcNow - disposeQueueItem.QueueTime;

                        if (ReferenceEquals(turnFirstItem, disposeQueueItem)
                            && interval < _disposeDelay)
                        {
                            await Task.Delay(_disposeDelay - interval, runningToken).ConfigureAwait(false);
                            continue;
                        }

                        if (interval < _disposeDelay
                            || disposeQueueItem.Invoker.ReferenceCount > 0)
                        {
                            _disposeQueue.Enqueue(disposeQueueItem);
                            turnFirstItem ??= disposeQueueItem;
                        }
                        else
                        {
                            Dispose(disposeQueueItem.Invoker);
                        }
                    }
                    else
                    {
                        turnFirstItem = null;
                        await Task.Delay(_disposeDelay, runningToken).ConfigureAwait(false);
                    }
                }
            }
            catch { }

            Dispose(_defaultInvoker);
            Dispose(_noneProxyInvoker);

            foreach (var (_, placeHolder) in _proxyedInvokers)
            {
                Dispose(placeHolder);
            }

            static void Dispose(PlaceHolder<StrictDisposeOwnedHttpMessageInvoker> placeHolder) => placeHolder.Value?.RealDispose();
        });

        Task.Run(async () =>
        {
            while (!runningToken.IsCancellationRequested)
            {
                await Task.Delay(_aliveTime, runningToken).ConfigureAwait(false);

                try
                {
                    Check(_defaultInvoker);
                    Check(_noneProxyInvoker);

                    foreach (var (_, placeHolder) in _proxyedInvokers)
                    {
                        Check(placeHolder);
                    }
                }
                catch (Exception ex)
                {
                    Debug.Fail("Check Fail", ex.Message);
                }
            }

            void Check(PlaceHolder<StrictDisposeOwnedHttpMessageInvoker> placeHolder)
            {
                var invoker = placeHolder.Value;
                if (invoker is not null
                    && invoker.IsOutOfAliveTime())
                {
                    placeHolder.Exchange(null);
                    QueueToDispose(invoker);
                }
            };
        }, runningToken);
    }

    #endregion Private 方法

    #region Private 类

    private class DisposeQueueItem
    {
        #region Public 属性

        public StrictDisposeOwnedHttpMessageInvoker Invoker { get; }
        public DateTimeOffset QueueTime { get; }

        #endregion Public 属性

        #region Public 构造函数

        public DisposeQueueItem(DateTimeOffset queueTime, StrictDisposeOwnedHttpMessageInvoker invoker)
        {
            QueueTime = queueTime;
            Invoker = invoker;
        }

        #endregion Public 构造函数
    }

    private class PlaceHolder<T> where T : class
    {
        #region Private 字段

        private T? _value;

        #endregion Private 字段

        #region Public 属性

        public T? Value => _value;

        #endregion Public 属性

        #region Public 构造函数

        public PlaceHolder()
        {
        }

        public PlaceHolder(T? value)
        {
            _value = value;
        }

        #endregion Public 构造函数

        #region Public 方法

        public static implicit operator PlaceHolder<T>(T? value) => new(value);

        public static implicit operator T?(PlaceHolder<T> value) => value.Value;

        public T? Exchange(T? value) => Interlocked.Exchange(ref _value, value);

        #endregion Public 方法
    }

    private class StrictDisposeOwnedHttpMessageInvoker
        : HttpMessageInvoker
        , IOwner<HttpMessageInvoker>
    {
        #region Private 字段

        private readonly HttpMessageHandler _handler;
        private readonly SimpleHttpMessageInvokerPool _pool;
        private int _referenceCount = 0;

        #endregion Private 字段

        #region Public 属性

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTimeOffset CreateTime { get; } = DateTimeOffset.UtcNow;

        public HttpMessageInvoker Value => this;

        #endregion Public 属性

        #region Internal 属性

        internal int ReferenceCount => _referenceCount;

        #endregion Internal 属性

        #region Public 构造函数

        public StrictDisposeOwnedHttpMessageInvoker(SimpleHttpMessageInvokerPool pool, HttpMessageHandler handler) : base(handler, false)
        {
            _pool = pool;
            _handler = handler;
        }

        #endregion Public 构造函数

        #region Private 析构函数

        /// <summary>
        ///
        /// </summary>
        ~StrictDisposeOwnedHttpMessageInvoker()
        {
            Dispose(false);
        }

        #endregion Private 析构函数

        #region Internal 方法

        internal bool IsOutOfAliveTime()
        {
            return DateTimeOffset.UtcNow - CreateTime > _pool._aliveTime;
        }

        internal void RealDispose()
        {
            _handler.Dispose();
            base.Dispose(true);
        }

        internal void Reference() => Interlocked.Increment(ref _referenceCount);

        #endregion Internal 方法

        #region Protected 方法

        protected override void Dispose(bool disposing)
        {
            var currentReferenceCount = Interlocked.Decrement(ref _referenceCount);
            if (currentReferenceCount == 0
                && IsOutOfAliveTime())
            {
                _pool.QueueToDispose(this);
                return;
            }
            if (disposing
                && currentReferenceCount < 0)
            {
                throw new InvalidOperationException("Wrong number of dispose calls");
            }
        }

        #endregion Protected 方法
    }

    #endregion Private 类
}
