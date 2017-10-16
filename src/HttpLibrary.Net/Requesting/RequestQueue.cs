// Author: Wayne Gu
// Created: 2016-6-20 14:00
// Project: HttpLibrary.Net
// License: MIT license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using HttpLibrary.Common;
using HttpLibrary.Http;
using System.Security;
using System.Diagnostics;
using HttpLibrary.Interop;

namespace HttpLibrary.Requesting
{
    /// <summary>
    /// RequestQueue is core of HttpLibrary. It sends requests in the request queue.
    /// Request in request queue is prioritized, which means request with high priority will be sent first.
    /// RequestQueue is multithread safe, you can add requests from diffent threads. 
    /// </summary>
    public class RequestQueue : IDisposable
    {
        private Task contextTask;
        private ManualResetEvent stopNotifier = new ManualResetEvent(false);

        private const int QueueCount = (int)RequestPriority.Low + 1;
        
        private List<Request>[] msgQueues;

        /// <summary>
        /// Number of realtime requests can be sent parallelly.
        /// Default value is 2.
        /// </summary>
        public int ParallelRealTime
        {
            get;
            private set;
        }

        /// <summary>
        /// Number of non-realtime requests can be sent parallelly.
        /// Default value is 2.
        /// </summary>
        public int ParallelOther
        {
            get;
            private set;
        }

        private object cacheLocker = new object();

        private List<Request> addedCache = new List<Request>();
        private List<Request> cancelledCache = new List<Request>();
        private List<Request> responsedCache = new List<Request>();

        private AutoResetEvent cacheChangedNotifier = new AutoResetEvent(false);

        private HttpLayer httpLayer;

        /// <summary>
        /// Trigger when proxy credential is required 
        /// </summary>
        public event EventHandler<ProxyAuthorizationRequiredEventArgs> ProxyAuthRequired;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpLibPlatform">HttpLibraryPlatform interface</param>
        public RequestQueue(IHttpLibraryPlatform httpLibPlatform)
        {
            InitMsgQueues();
            httpLayer = new HttpLayer(httpLibPlatform);
            httpLayer.OnResponsed += OnHttpRequestResponsed;
            httpLayer.OnFailed += OnHttpRequestFailed;
            httpLayer.ProxyAuthorizationRequired += OnProxyAuthorizationRequired;


            contextTask = new Task(ReuqestQueueProcedure, TaskCreationOptions.LongRunning);
            contextTask.Start();
            //contextThread = new Thread(new ThreadStart(ReuqestQueueProcedure));
            //contextThread.Name = "http network thread";
            //contextThread.IsBackground = true;
            //contextThread.Start();
        }

        /// <summary>
        /// Add request into request queue
        /// </summary>
        /// <param name="request">request</param>
        public void AddRequest(Request request)
        {
            DiagnoseHelper.CheckArgument(request, "request");

            request.IsSending = false;
            request.PrepareLowLevelRequest();
            DiagnoseHelper.CheckReference(request.HttpRequest, "Can not convert http request from specific request");
            DiagnoseHelper.CheckReference(request.Response, "Can not create response from specific request");

            lock (cacheLocker)
            {
                addedCache.Add(request);
            }
            cacheChangedNotifier.Set();
        }

        /// <summary>
        /// Cancel request
        /// </summary>
        /// <param name="request"></param>
        public void CancelRequest(Request request)
        {
            DiagnoseHelper.CheckArgument(request, "request");

            lock (cacheLocker)
            {
                cancelledCache.Add(request);
            }
            cacheChangedNotifier.Set();
        }

        /// <summary>
        /// Send request in Async mode
        /// </summary>
        /// <param name="request">request</param>
        /// <returns>Task</returns>
        public Task SendRequestAsync(Request request)
        {
            AddRequest(request);
            TaskCompletionSource<Response> completeSource = new TaskCompletionSource<Response>();

            request.Response.OnReady += (sender, e) =>
            {
                Response resp = sender as Response;
                Task t = new Task(() =>
                {
                    completeSource.SetResult(resp);
                });
                t.Start();
            };

            return completeSource.Task;
            
        }

        /// <summary>
        /// Set proxy credential
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="password">Password</param>
        public void SetProxyAuth(string userName, string password)
        {
            httpLayer.SetProxy(userName, password);
        }

        /// <summary>
        /// Release related resource
        /// </summary>
        public void Dispose()
        {
            stopNotifier.Set();
            contextTask.Wait();
            //contextThread.Join();
        }

        private void InitMsgQueues()
        {
            ParallelRealTime = ParallelOther = 2;
            msgQueues = new List<Request>[QueueCount];
            for (int i = 0; i < QueueCount; i++)
            {
                msgQueues[i] = new List<Request>();
            }
        }

        private List<Request> GetMsgQueue(RequestPriority priority)
        {
            int index = (int)priority;
            return msgQueues[index];
        }

        private bool IsMsgQueuesEmpty()
        {
            bool isEmpty = true;
            foreach (var queue in msgQueues)
            {
                isEmpty &= !queue.Any();
                if (!isEmpty)
                    break;
            }

            return isEmpty;
        }

        private void ResponseRequest(Request request)
        {
            if (request == null)
            {
                //NLogger.LogHelper.OnlineLogger.Warn("Request is empty??");
                Debug.WriteLine("Request is empty??");
                return;
            }

            lock (cacheLocker)
            {
                responsedCache.Add(request);
            }
            cacheChangedNotifier.Set();
        }

        private void _AddRequest(Request req)
        {
            var queue = GetMsgQueue(req.Priority);
            if (req.Priority == RequestPriority.Realtime || req.Priority == RequestPriority.High)
            {
                queue.Insert(0, req);
            }
            else
            {
                queue.Add(req);
            }
        }

        private void _AddRequsts(IEnumerable<Request> reqs)
        {
            foreach (var req in reqs)
            {
                _AddRequest(req);
            }
        }

        private void _CancelRequest(Request req)
        {
            var queue = GetMsgQueue(req.Priority);

            queue.Remove(req);

            if(req.HttpRequest != null)
                httpLayer.CancelRequest(req.HttpRequest);
            else if (req.Response != null && req.Response.HttpResponse != null)
            {
                httpLayer.CancelRequest(req.Response.HttpResponse);
            }
        }

        private void _CancelRequests(IEnumerable<Request> reqs)
        {
            foreach (var req in reqs)
            {
                _CancelRequest(req);
            }
        }

        private void _ResponseRequest(Request req)
        {
            var queue = GetMsgQueue(req.Priority);

            queue.Remove(req);
        }

        private void _ResponseRequests(IEnumerable<Request> reqs)
        {
            foreach (var req in reqs)
            {
                _ResponseRequest(req);
            }
        }

        private void ReuqestQueueProcedure()
        {
            try
            {
                WaitHandle[] waiters = new WaitHandle[] { stopNotifier, cacheChangedNotifier };
                int waiterIndex = -1;
                while (waiterIndex != 0)
                {
                    lock (cacheLocker)
                    {
                        _AddRequsts(addedCache);
                        addedCache.Clear();

                        _CancelRequests(cancelledCache);
                        cancelledCache.Clear();

                        _ResponseRequests(responsedCache);
                        responsedCache.Clear();
                    }

                    if (!SendRealTimeRequests())
                    {
                        SendOtherRequest();
                    }

                    waiterIndex = WaitHandle.WaitAny(waiters);
                }
            }catch(Exception ex)
            {
                Debug.WriteLine("Fatal: ReuqestQueueProcedure task crashed!");
                Debug.WriteLine(ex.ToString());
            }
        }

        private int GetParallelCount(RequestPriority priority)
        {
            return priority == RequestPriority.Realtime ? ParallelRealTime : ParallelOther;
        }

        private int GetSendingRequestCount(RequestPriority priority)
        {
            var queue = GetMsgQueue(priority);
            return queue.Count(r => r.IsSending);
        }

        private bool SendRealTimeRequests()
        {
            int sendingCount = 0;

            var queue = GetMsgQueue(RequestPriority.Realtime);

            var pendingQueue = queue.Where(r => !r.IsSending);

            sendingCount += (queue.Count - pendingQueue.Count());
            
            foreach (var r in pendingQueue)
            {
                if (sendingCount >= ParallelRealTime)
                    break;

                sendingCount++;
                r.IsSending = true;
                httpLayer.SendRequest(r.HttpRequest);
            }

            return sendingCount != 0;
        }

        private void SendOtherRequest()
        {
            int sendingCount = 0;

            var queue = GetMsgQueue(RequestPriority.High);

            var pendingQueueHigh = queue.Where(r => !r.IsSending);

            sendingCount += (queue.Count - pendingQueueHigh.Count());

            queue = GetMsgQueue(RequestPriority.Normal);

            var pendingQueueNormal = queue.Where(r => !r.IsSending);

            sendingCount += (queue.Count - pendingQueueNormal.Count());

            queue = GetMsgQueue(RequestPriority.Low);

            var pendingQueueLow = queue.Where(r => !r.IsSending);

            sendingCount += (queue.Count - pendingQueueLow.Count());

            foreach (var r in pendingQueueHigh)
            {
                if (sendingCount >= ParallelOther)
                    return;

                sendingCount++;
                r.IsSending = true;
                httpLayer.SendRequest(r.HttpRequest);
            }

            foreach (var r in pendingQueueNormal)
            {
                if (sendingCount >= ParallelOther)
                    return;

                sendingCount++;
                r.IsSending = true;
                httpLayer.SendRequest(r.HttpRequest);
            }

            foreach (var r in pendingQueueLow)
            {
                if (sendingCount >= ParallelOther)
                    return;

                sendingCount++;
                r.IsSending = true;
                httpLayer.SendRequest(r.HttpRequest);
            }
        }

        private void OnHttpRequestResponsed(object sender, HttpEventArgs e)
        {
            Request req = e.Request.Context as Request;
            e.Request.Context = null;
            ResponseRequest(req);
        }

        private void OnHttpRequestFailed(object sender, HttpEventArgs e)
        {
            Request req = e.Request.Context as Request;
            e.Request.Context = null;
            ResponseRequest(req);
        }

        private void OnProxyAuthorizationRequired(object sender, ProxyAuthRequiredEventArgs e)
        {
            if (ProxyAuthRequired != null)
            {
                var eArgs = new ProxyAuthorizationRequiredEventArgs()
                {
                    ProxyUri = e.ProxyUri,
                    ProxyName = e.ProxyName
                };

                ProxyAuthRequired(sender, eArgs);

                e.ProxyUserName = eArgs.ProxyUserName;
                e.ProxyPassword = eArgs.ProxyPassword;
            }
        }
    }
}
