// Author: Wayne Gu
// Created: 2016-6-20 14:00
// Project: HttpLibrary.Net
// License: MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using HttpLibrary.Common;
using System.Threading;
using System.Diagnostics;
using HttpLibrary.Interop;

namespace HttpLibrary.Http
{
    class HttpProgressEventArgs : EventArgs
    {
        public long Offset { get; private set; }
        public long Total { get; private set; }
        public int LatestSize { get; private set; }
        public HttpProgressEventArgs(long offset, long total, int latestSize)
        {
            Offset = offset;
            Total = total;
            LatestSize = latestSize;
        }
    }

    class HttpReadyEventArgs : EventArgs
    {
        public ReadyState State { get; private set; }

        public HttpReadyEventArgs(ReadyState state)
        {
            State = state;
        }
    }


    /// <summary>
    /// Low level http request
    /// </summary>
    public class HttpRequest
    {
        private static Random r = new Random(DateTime.Now.Millisecond);

        internal int RequestId
        {
            get;
            private set;
        }

        private int chunkSize = 64 * 1024;
        internal int ChunkSize
        {
            get { return chunkSize; }
            set
            {
                if (value != chunkSize)
                {
                    chunkSize = value;
                }
            }
        }
        internal bool IsStopped { get; set; }
        internal HttpWebRequest OriginalRequest
        {
            get;
            private set;
        }

        private bool needDisposeWhenReset;
        private StreamCollection requestStreams;
        public StreamCollection RequestStreams
        {
            get
            {
                return requestStreams;
            }
            private set
            {
                Reset();

                requestStreams = value;
                RequestStreamEnumerator = new StreamBufferEnumerator(RequestId, requestStreams, requestStreams.Length, ChunkSize);
            }
        }

        public void SetRequestStream(Stream requestStream, string contentType, long contentLength = -1, bool disposeWhenReset = true)
        {
            RequestStreams = new StreamCollection(new Stream[] { requestStream });
            RequestStreams.Seek(0);
            needDisposeWhenReset = disposeWhenReset;

            //LogHelper.OnlineLogger.DebugFormat("Set ContentLength={0},ContentType={1}", requestStream.Length, contentType);
            Debug.WriteLine("Set ContentLength={0},ContentType={1}", requestStream.Length, contentType);

            httpLibraryPlatform.HttpSettings.SetHttpHeader(OriginalRequest, HttpRequestHeader.ContentLength, (contentLength == -1 ? RequestStreams.Length : contentLength).ToString());
            httpLibraryPlatform.HttpSettings.SetHttpHeader(OriginalRequest, HttpRequestHeader.ContentType, contentType);
            
            //OriginalRequest.AllowWriteStreamBuffering = false;
        }

        public void SetRequestStream(StreamCollection requestStreams, string contentType, long contentLength = -1, bool disposeWhenReset = true)
        {
            RequestStreams = requestStreams;

            RequestStreams.Seek(0);

            needDisposeWhenReset = disposeWhenReset;

            //LogHelper.OnlineLogger.DebugFormat("Set ContentLength={0},ContentType={1}", requestStreams.Length, contentType);
            Debug.WriteLine("Set ContentLength={0},ContentType={1}", requestStreams.Length, contentType);
            httpLibraryPlatform.HttpSettings.SetHttpHeader(OriginalRequest, HttpRequestHeader.ContentLength, (contentLength == -1 ? RequestStreams.Length : contentLength).ToString());
            httpLibraryPlatform.HttpSettings.SetHttpHeader(OriginalRequest, HttpRequestHeader.ContentType, contentType);

            //OriginalRequest.AllowWriteStreamBuffering = false;
        }

        private IHttpLibraryPlatform httpLibraryPlatform;
        internal HttpResponse Response
        {
            get;
            set;
        }

        public object Context
        {
            get;
            set;
        }

        internal Exception Exception
        {
            get;
            set;
        }

        internal StreamBufferEnumerator RequestStreamEnumerator
        {
            get;
            private set;
        }

        public List<KeyValuePair<string, string>> Headers
        {
            get;
            set;
        }

        internal bool IsTimeout
        {
            get;
            private set;
        }

        internal int ProxyId
        {
            get;
            set;
        }

        public HttpRequest(IHttpLibraryPlatform platform, string uri, string method = HttpConst.HttpMethod_Post,
                           bool allowReadBuffering = true, bool allowWriteBuffering = true,
                           int bufferSize = 64 * 1024)
        {
            httpLibraryPlatform = platform;

            RequestId = r.Next(1000000);

            try
            {
                OriginalRequest = HttpWebRequest.Create(uri) as HttpWebRequest;
            }
            catch (Exception ex)
            {
                //LogHelper.OnlineLogger.Error("Cannot create Http request " + ex.ToString());
                Debug.WriteLine("Cannot create Http request " + ex.ToString());
                OriginalRequest = HttpWebRequest.Create("http://localhost/biu/error/") as HttpWebRequest;
            }
            //OriginalRequest.AllowReadStreamBuffering = allowReadBuffering;
            //OriginalRequest.AllowWriteStreamBuffering = allowWriteBuffering;

            OriginalRequest.Method = method;
            ChunkSize = bufferSize;

        }

        //Close request stream
        internal void Close()
        {
            Reset();
        }

        internal void TimeoutAbort()
        {
            IsTimeout = true;
            //We can abort request here, no deadlock would happen
            OriginalRequest.Abort();
            Stop();
        }

        internal void Stop()
        {
            IsStopped = true;
        }

        private void Reset()
        {
            if (RequestStreamEnumerator != null)
            {
                RequestStreamEnumerator.Dispose();
                RequestStreamEnumerator = null;
            }
            if (needDisposeWhenReset && requestStreams != null)
            {
                requestStreams.Dispose();
                requestStreams = null;
            }
        }

        internal void RecreateHttpRequest()
        {
            var tempReq = OriginalRequest;
            OriginalRequest = HttpWebRequest.Create(tempReq.RequestUri) as HttpWebRequest;

            //OriginalRequest.AllowReadStreamBuffering = tempReq.AllowReadStreamBuffering;

            //OriginalRequest.AllowWriteStreamBuffering = tempReq.AllowWriteStreamBuffering;

            OriginalRequest.Method = tempReq.Method;
        }
        #region Events

        internal event EventHandler<HttpProgressEventArgs> OnProgressing;
        internal event EventHandler<HttpReadyEventArgs> OnReady;
        internal event EventHandler<EventArgs> OnResponsed;

        internal void ReportProgress(long offset, long total, int latestSize)
        {
            if (OnProgressing != null)
                OnProgressing(this, new HttpProgressEventArgs(offset, total, latestSize));
        }

        internal void ReportReady(ReadyState state)
        {
            if (OnReady != null)
                OnReady(this, new HttpReadyEventArgs(state));
        }

        internal void ReportResponsed()
        {
            if (OnResponsed != null)
                OnResponsed(this, null);
        }
        #endregion
    }

    /// <summary>
    /// Low level http response
    /// </summary>
    public class HttpResponse : IDisposable
    {
        internal int ResponseId
        {
            get;
            private set;
        }

        //protected static StreamAccessor streamCache = new StreamAccessor("HttpRequest", true);
        //private const int CacheValveSize = 1024 * 100;
        internal bool IsStopped { get; set; }

        internal HttpWebResponse OriginalResponse
        {
            get;
            private set;
        }

        /// <summary>
        /// Response stream
        /// </summary>
        public Stream ResponseStream
        {
            get;
            protected set;
        }

        internal Exception Exception
        {
            get;
            set;
        }

        /// <summary>
        /// Http status code
        /// </summary>
        public HttpStatusCode StatusCode
        {
            get;
            internal set;
        }

        public string StatusDescription
        {
            get;
            internal set;
        }


        internal HttpResponse(HttpWebResponse response, int responseId)
        {
            ResponseId = responseId;
            OriginalResponse = response;
            StatusCode = response.StatusCode;
            StatusDescription = response.StatusDescription;

            CreateResponseStream();
        }

        internal virtual void DeseralizeFromStream()
        {
        }

        /// <summary>
        /// Release reponse.
        /// </summary>
        public void Dispose()
        {
            if (OriginalResponse != null)
            {
                OriginalResponse.Dispose();
                OriginalResponse = null;
            }

            if (ResponseStream != null)
            {
                ResponseStream.Dispose();
                ResponseStream = null;
                /*if (streamKey != null)
                {
                    streamCache.ClearAsync(streamKey, StreamType.File).Wait();
                }*/
            }
        }

        private void CreateResponseStream()
        {
            ResponseStream = new MemoryStream();
            /*if (OriginalResponse.ContentLength > CacheValveSize)
            {
                streamCache.CreateStream(GetResponseKey(), StreamType.File, (byte[])null, (int)OriginalResponse.ContentLength).Wait();
                var loadStreamTask = streamCache.LoadStreamAsync(streamKey, StreamType.File);
                loadStreamTask.Wait();
                ResponseStream = loadStreamTask.Result;
            }
            else
            {
                ResponseStream = new MemoryStream((int)OriginalResponse.ContentLength);
            }*/
        }

        /*private string GetResponseKey()
        {
            int hash = OriginalResponse.ResponseUri.AbsoluteUri.GetHashCode();
            string name = Path.GetFileName(OriginalResponse.ResponseUri.AbsolutePath);
            string key = name + hash.ToString("X");
            return key;
        }*/

        internal void CleanReponseStream()
        {
            if (ResponseStream != null)
                ResponseStream.Dispose();

            CreateResponseStream();
        }

        #region Events
        internal event EventHandler<HttpProgressEventArgs> OnProgressing;
        internal event EventHandler<HttpReadyEventArgs> OnReady;

        internal void ReportProgress(long offset, long total, int latestSize)
        {
            if (OnProgressing != null)
                OnProgressing(this, new HttpProgressEventArgs(offset, total, latestSize));
        }

        internal void ReportReady(ReadyState state)
        {
            if (OnReady != null)
                OnReady(this, new HttpReadyEventArgs(state));
        }
        #endregion
    }
}
