﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HttpLibrary.Common;
using System.Net;
using System.Diagnostics;

namespace HttpLibrary.Http
{
    sealed class StreamBufferEnumerator
    {
        private const int BlockSize = 1024 * 64;

        private StreamCollection targetStreams;

        public long Offset { get; private set; }
        private int ChunkSize { get; set; }
        private long streamLength;

        private int logId;
        public StreamBufferEnumerator(int id, StreamCollection streams, long streamLen, int chunkSize = BlockSize)
        {
            this.logId = id;
            DiagnoseHelper.CheckArgument(streams, "target stream can not null");
            //LogHelper.OnlineLogger.DebugFormat("Create stream enumerator, length={0}, chunksize={1}, {2}", streamLen, chunkSize, HttpLayer.LogRequetId(logId));
            Debug.WriteLine("Create stream enumerator, length={0}, chunksize={1}, {2}", streamLen, chunkSize, HttpLayer.LogRequetId(logId));
            ChunkSize = chunkSize;
            targetStreams = streams;
            streamLength = streamLen;
            Reset();
        }

        public StreamBufferEnumerator(int id, Stream stream, long streamLen, int chunkSize = BlockSize)
        {
            this.logId = id;
            DiagnoseHelper.CheckArgument(stream, "target stream can not null");
            //LogHelper.OnlineLogger.DebugFormat("Create stream enumerator, length={0}, chunksize={1}, {2}", streamLen, chunkSize, HttpLayer.LogRequetId(logId));
            Debug.WriteLine("Create stream enumerator, length={0}, chunksize={1}, {2}", streamLen, chunkSize, HttpLayer.LogRequetId(logId));

            ChunkSize = chunkSize;
            targetStreams = new StreamCollection(stream);
            streamLength = streamLen;
            Reset();
        }

        public int CurrentLength { get; private set; }
        public byte[] Current { get; private set; }

        public bool MoveNext()
        {
            bool canMoveNext = false;
            try
            {
                ///Reduce new buffer times for background task
                if (Current.IsNullOrEmpty())
                {
                    Current = new byte[ChunkSize];
                }
                else
                {
                    Array.Clear(Current, 0, Current.Length);
                }

                CurrentLength = targetStreams.Read(Current, 0, ChunkSize);
                if (CurrentLength != 0)
                {
                    Offset += CurrentLength;
                    canMoveNext = true;
                }
                else
                {
                    if (streamLength != -1)
                    {
                        if (Offset < streamLength)
                        {
                            //LogHelper.OnlineLogger.ErrorFormat("Cannot read any data before EOF, id={0}. {1}", targetStreams.GetHashCode(), HttpLayer.LogRequetId(logId));
                            Debug.WriteLine("Cannot read any data before EOF, id={0}. {1}", targetStreams.GetHashCode(), HttpLayer.LogRequetId(logId));
                            throw new WebException("Cannot read any data before EOF", WebExceptionStatus.MessageLengthLimitExceeded);
                        }
                        else
                        {
                            //LogHelper.OnlineLogger.DebugFormat("Reach EOF, id={0}. {1}", targetStreams.GetHashCode(), HttpLayer.LogRequetId(logId));
                            Debug.WriteLine("Reach EOF, id={0}. {1}", targetStreams.GetHashCode(), HttpLayer.LogRequetId(logId));
                        }
                    }
                    else
                    {
                        //LogHelper.OnlineLogger.WarnFormat("Cannot read any data, Stream Length = -1, id={0}. {1}", targetStreams.GetHashCode(), HttpLayer.LogRequetId(logId));
                        Debug.WriteLine("Cannot read any data, Stream Length = -1, id={0}. {1}", targetStreams.GetHashCode(), HttpLayer.LogRequetId(logId));
                    }
                }
            }
            catch (EndOfStreamException ex)
            {
                //LogHelper.OnlineLogger.WarnFormat("Reach end of stream {0}. {1}", ex.ToString(), HttpLayer.LogRequetId(logId));
                Debug.WriteLine("Reach end of stream {0}. {1}", ex.ToString(), HttpLayer.LogRequetId(logId));
            }
            catch (ObjectDisposedException ex)
            {
                //LogHelper.OnlineLogger.WarnFormat("Can not read disposed stream {0}. {1}" + ex.ToString(), HttpLayer.LogRequetId(logId));
                Debug.WriteLine("Can not read disposed stream {0}. {1}" + ex.ToString(), HttpLayer.LogRequetId(logId));
            }

            return canMoveNext;
        }

        public void Skip(long steps)
        {
            Offset = steps;
            if (targetStreams.CanSeek)
                targetStreams.Seek(steps);
        }

        public void Reset()
        {
            CurrentLength = 0;
            Current = null;
            Offset = 0;
            if (targetStreams.CanSeek)
                targetStreams.Seek(0);
        }

        public void Dispose()
        {
            targetStreams = null;
        }
    }
}
