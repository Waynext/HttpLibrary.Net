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
using HttpLibrary.Http;
using HttpLibrary.Common;
using System.IO;
using HttpLibrary.Interop;

namespace HttpLibrary.Requesting
{
    /// <summary>
    /// Request or response progress event arguments
    /// </summary>
    public class ProgressEventArgs : EventArgs
    {
        public ProgressEventArgs(long sended, long total, int latestSize)
        {
            Sended = sended;
            Total = total;
            LatestSize = latestSize;
        }

        /// <summary>
        /// Sent bytes
        /// </summary>
        public long Sended { get; private set; }

        /// <summary>
        /// Total bytes 
        /// </summary>
        public long Total { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int LatestSize { get; private set; }
    }

    /// <summary>
    /// Request or response state event arguments
    /// </summary>
    public class ReadyEventArgs : EventArgs
    {
        /// <summary>
        /// Request or response state
        /// </summary>
        public ReadyState State { get; private set; }

        public ReadyEventArgs(ReadyState state)
        {
            State = state;
        }
    }

    /// <summary>
    /// Request priority, the higher priority request is, the faster it will be sent.
    /// </summary>
    public enum RequestPriority { Realtime = 0, High, Normal, Low }

    /// <summary>
    /// Request base class.
    /// </summary>
    public abstract class Request
    {
        protected IHttpLibraryPlatform HttpLibPlatform
        {
            get;
            private set;
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="priority">Request priority.</param>
        public Request(IHttpLibraryPlatform httpLibPlatform, RequestPriority priority)
        {
            HttpLibPlatform = httpLibPlatform;
            Id = GenerateId();
            Priority = priority;
        }

        /// <summary>
        /// Request Id.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Request url.
        /// </summary>
        public string Uri { get; protected set; }

        /// <summary>
        /// Request priority.
        /// </summary>
        public RequestPriority Priority { get; private set; }

        /// <summary>
        /// Response.
        /// </summary>
        public Response Response { get; protected set; }

        /// <summary>
        /// Request headers
        /// </summary>
        protected List<KeyValuePair<string, string>> requestHeaders;

        /// <summary>
        /// Request send progress event. It's used when request contains body.
        /// </summary>
        public event EventHandler<ProgressEventArgs> OnProgressing;

        /// <summary>
        /// Low level http request.
        /// </summary>
        protected internal HttpRequest HttpRequest { get; set; }
        internal void PrepareLowLevelRequest()
        {
            Encode();
            CreateResponse();

            //Link Request and Response with this Id;
            Response.Id = Id;

            HttpRequest.Context = this;

            HttpRequest.OnProgressing += OnHttpRequestPrgogressing;
            HttpRequest.OnReady += OnHttpRequestReady;
            HttpRequest.OnResponsed += OnHttpResponsed;
        }

        private void OnHttpRequestReady(object sender, HttpReadyEventArgs e)
        {
            OnProgressing = null;

            //Clear HttpRequest 
            HttpRequest.OnProgressing -= OnHttpRequestPrgogressing;
            HttpRequest.OnReady -= OnHttpRequestReady;

            if (e.State != ReadyState.Succeeded)
            {
                Response.SetException(HttpRequest.Exception);
                HttpRequest.OnResponsed -= OnHttpResponsed;
                HttpRequest = null;

                Response.HttpResponse_OnReady(sender, e);
            }
        }

        private void OnHttpRequestPrgogressing(object sender, HttpProgressEventArgs e)
        {
            if (OnProgressing != null)
            {
                OnProgressing(this, new ProgressEventArgs(e.Offset, e.Total, e.LatestSize));
            }
        }

        private void OnHttpResponsed(object sender, EventArgs e)
        {
            Response.SetHttpResponse(HttpRequest.Response);
            HttpRequest.OnResponsed -= OnHttpResponsed;
            HttpRequest = null;
        }

        /// <summary>
        /// Create low level request and set its properties.
        /// Override it if you want to use your own way to create HttpRequest.
        /// </summary>
        protected virtual void Encode()
        {
            HttpRequest = new HttpRequest(HttpLibPlatform, Uri);
            if (!requestHeaders.IsNullOrEmpty())
            {
                HttpRequest.Headers = requestHeaders;
            }
        }

        /// <summary>
        /// Create reponse.
        /// Override it since usually you have you own reponse type.
        /// </summary>
        protected virtual void CreateResponse()
        {
            Response = new Response();
        }

        //True if it's sending, 
        //false if it's waiting
        internal bool IsSending { get; set; }

        private static int idSource = 0;
        private static int GenerateId()
        {
            return Interlocked.Increment(ref idSource);
        }
    }

    /// <summary>
    /// Reponse base class.
    /// </summary>
    public class Response
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Response()
        {
            IsSucceeded = false;
        }

        /// <summary>
        /// Reponse Id, equals to request Id.
        /// </summary>
        public int Id { get; internal set; }

        /// <summary>
        /// Reponse exception
        /// </summary>
        public Exception Exception
        {
            get;
            protected set;
        }

        /// <summary>
        /// Indicate if reponse is succeeded.
        /// </summary>
        public bool IsSucceeded { get; protected set; }

        /// <summary>
        /// Repoonse stream.
        /// </summary>
        protected Stream ResponseStream
        {
            get
            {
                return HttpResponse != null ? HttpResponse.ResponseStream : null;
            }
        }

        /// <summary>
        /// Object context
        /// </summary>
        public object Context
        {
            get;
            set;
        }

        /// <summary>
        /// Clean reponse sream
        /// </summary>
        protected void CleanResponseStream()
        {
            if (HttpResponse != null)
            {
                HttpResponse.CleanReponseStream();
            }
        }

        /// <summary>
        /// Low level http reponse
        /// </summary>
        protected internal HttpResponse HttpResponse { get; private set; }

        /// <summary>
        /// Trigger when reponse is ready.
        /// </summary>
        public event EventHandler<ReadyEventArgs> OnReady;
        /// <summary>
        /// Trigger when response is in progress.
        /// It's used when response contains body.
        /// </summary>
        public event EventHandler<ProgressEventArgs> OnProgressing;

        internal void SetException(Exception ex)
        {
            Exception = ex;
            IsSucceeded = false;
        }

        internal void SetHttpResponse(HttpResponse response)
        {
            HttpResponse = response;

            HttpResponse.OnProgressing += HttpResponse_OnProgressing;
            HttpResponse.OnReady += HttpResponse_OnReady;

            IsSucceeded = true;
        }

        private void HttpResponse_OnProgressing(object sender, HttpProgressEventArgs e)
        {
            ReportProgressing(e.Offset, e.Total, e.LatestSize);
        }

        internal void HttpResponse_OnReady(object sender, HttpReadyEventArgs e)
        {
            ReadyState state = e.State;
            if (HttpResponse != null)
            {
                if (HttpResponse.Exception != null)
                {
                    SetException(HttpResponse.Exception);
                }
                
                Decode();

                if (!IsSucceeded && state != ReadyState.Cancelled && state != ReadyState.Failed)
                {
                    state = ReadyState.Failed;
                }
            }
            else if(state == ReadyState.Failed)
            {
                ExplainException();
            }

            if (OnReady != null)
                OnReady(this, new ReadyEventArgs(state));
            if (HttpResponse != null)
            {
                HttpResponse.OnProgressing -= HttpResponse_OnProgressing;
                HttpResponse.OnReady -= HttpResponse_OnReady;
                HttpResponse.Dispose();
                HttpResponse = null;
            }

            OnReady = null;
            OnProgressing = null;
        }

        protected virtual void ReportProgressing(long sended, long total, int latestSize)
        {
            if (OnProgressing != null)
            {
                ProgressEventArgs e = new ProgressEventArgs(sended, total, latestSize);
                OnProgressing(this, e);
            }
        }

        /// <summary>
        /// Decode low level http response.
        /// Override it to decode http response and assign to your response object. 
        /// </summary>
        protected virtual void Decode() { }

        /// <summary>
        /// Decode low level http response when it's failed.
        /// Override it to decode http error response and assign to your response object.
        /// </summary>
        protected virtual void ExplainException() { }

        /// <summary>
        /// Get response header
        /// </summary>
        /// <param name="header">header name</param>
        /// <returns></returns>
        protected string GetResponseHeader(string header)
        {
            return HttpResponse.OriginalResponse.Headers[header];
        }
    }
}
