using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ITOrm.Core.Dapper
{
    /// <summary>
    /// The grid reader provides interfaces for reading multiple result sets from a Dapper query 
    /// </summary>
    public partial class GridReader : IDisposable
    {
        private IDataReader reader;
        private IDbCommand command;
        private Identity identity;

        internal GridReader(IDbCommand command, IDataReader reader, Identity identity, IParameterCallbacks callbacks)
        {
            this.command = command;
            this.reader = reader;
            this.identity = identity;
            this.callbacks = callbacks;
        }

        /// <summary>
        /// Read the next grid of results, returned as a dynamic object
        /// </summary>
        /// <remarks>Note: each row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
        public IEnumerable<dynamic> Read(bool buffered = true)
        {
            return ReadImpl<dynamic>(typeof(DapperRow), buffered);
        }

        /// <summary>
        /// Read the next grid of results
        /// </summary>
        public IEnumerable<T> Read<T>()
        {
            return Read<T>(true);
        }

        /// <summary>
        /// Read the next grid of results
        /// </summary>

        public IEnumerable<T> Read<T>(bool buffered)
        {
            return ReadImpl<T>(typeof(T), buffered);
        }

        /// <summary>
        /// Read the next grid of results
        /// </summary>
        public IEnumerable<object> Read(Type type, bool buffered)
        {
            if (type == null) throw new ArgumentNullException("type");
            return ReadImpl<object>(type, buffered);
        }

        private IEnumerable<T> ReadImpl<T>(Type type, bool buffered)
        {
            if (reader == null) throw new ObjectDisposedException(GetType().FullName, "The reader has been disposed; this can happen after all data has been consumed");
            if (consumed) throw new InvalidOperationException("Query results must be consumed in the correct order, and each result can only be consumed once");
            var typedIdentity = identity.ForGrid(type, gridIndex);
            CacheInfo cache = SqlMapper.GetCacheInfo(typedIdentity, null, true);
            var deserializer = cache.Deserializer;

            int hash = SqlMapper.GetColumnHash(reader);
            if (deserializer.Func == null || deserializer.Hash != hash)
            {
                deserializer = new DeserializerState(hash, SqlMapper.GetDeserializer(type, reader, 0, -1, false));
                cache.Deserializer = deserializer;
            }
            consumed = true;
            var result = ReadDeferred<T>(gridIndex, deserializer.Func, typedIdentity);
            return buffered ? result.ToList() : result;
        }

        private IEnumerable<TReturn> MultiReadInternal<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(Delegate func, string splitOn)
        {
            var identity = this.identity.ForGrid(typeof(TReturn), new Type[] { 
                    typeof(TFirst), 
                    typeof(TSecond),
                    typeof(TThird),
                    typeof(TFourth),
                    typeof(TFifth),
                    typeof(TSixth),
                    typeof(TSeventh)
                }, gridIndex);
            try
            {
                foreach (var r in SqlMapper.MultiMapImpl<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(null, default(CommandDefinition), func, splitOn, reader, identity, false))
                {
                    yield return r;
                }
            }
            finally
            {
                NextResult();
            }
        }

        /// <summary>
        /// Read multiple objects from a single record set on the grid
        /// </summary>
        public IEnumerable<TReturn> Read<TFirst, TSecond, TReturn>(Func<TFirst, TSecond, TReturn> func, string splitOn)
        {
            return Read<TFirst, TSecond, TReturn>(func, splitOn, true);
        }

        /// <summary>
        /// Read multiple objects from a single record set on the grid
        /// </summary>
        public IEnumerable<TReturn> Read<TFirst, TSecond, TReturn>(Func<TFirst, TSecond, TReturn> func, string splitOn, bool buffered)
        {
            var result = MultiReadInternal<TFirst, TSecond, DontMap, DontMap, DontMap, DontMap, DontMap, TReturn>(func, splitOn);
            return buffered ? result.ToList() : result;
        }

        /// <summary>
        /// Read multiple objects from a single record set on the grid
        /// </summary>
        public IEnumerable<TReturn> Read<TFirst, TSecond, TThird, TReturn>(Func<TFirst, TSecond, TThird, TReturn> func, string splitOn)
        {
            return Read<TFirst, TSecond, TThird, TReturn>(func, splitOn, true);
        }

        /// <summary>
        /// Read multiple objects from a single record set on the grid
        /// </summary>
        public IEnumerable<TReturn> Read<TFirst, TSecond, TThird, TReturn>(Func<TFirst, TSecond, TThird, TReturn> func, string splitOn, bool buffered)
        {
            var result = MultiReadInternal<TFirst, TSecond, TThird, DontMap, DontMap, DontMap, DontMap, TReturn>(func, splitOn);
            return buffered ? result.ToList() : result;
        }

        /// <summary>
        /// Read multiple objects from a single record set on the grid
        /// </summary>
        public IEnumerable<TReturn> Read<TFirst, TSecond, TThird, TFourth, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TReturn> func, string splitOn)
        {
            return Read<TFirst, TSecond, TThird, TFourth, TReturn>(func, splitOn, true);
        }

        /// <summary>
        /// Read multiple objects from a single record set on the grid
        /// </summary>
        public IEnumerable<TReturn> Read<TFirst, TSecond, TThird, TFourth, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TReturn> func, string splitOn, bool buffered)
        {
            var result = MultiReadInternal<TFirst, TSecond, TThird, TFourth, DontMap, DontMap, DontMap, TReturn>(func, splitOn);
            return buffered ? result.ToList() : result;
        }

        public IEnumerable<T> ReadDeferred<T>(int index, Func<IDataReader, object> deserializer, Identity typedIdentity)
        {
            try
            {
                while (index == gridIndex && reader.Read())
                {
                    yield return (T)deserializer(reader);
                }
            }
                finally // finally so that First etc progresses things even when multiple rows
            {
                if (index == gridIndex)
                {
                    NextResult();
                }
            }
        }
        private int gridIndex, readCount;
        private bool consumed;
        private IParameterCallbacks callbacks;

        /// <summary>
        /// Has the underlying reader been consumed?
        /// </summary>
        public bool IsConsumed
        {
            get
            {
                return consumed;
            }
        }

        private void NextResult()
        {
            if (reader.NextResult())
            {
                readCount++;
                gridIndex++;
                consumed = false;
            }
            else
            {
                // happy path; close the reader cleanly - no
                // need for "Cancel" etc
                reader.Dispose();
                reader = null;
                if (callbacks != null) callbacks.OnCompleted();
                Dispose();
            }
        }

        /// <summary>
        /// Dispose the grid, closing and disposing both the underlying reader and command.
        /// </summary>
        public void Dispose()
        {
            if (reader != null)
            {
                if (!reader.IsClosed && command != null) command.Cancel();
                reader.Dispose();
                reader = null;
            }
            if (command != null)
            {
                command.Dispose();
                command = null;
            }
        }

        CancellationToken cancel;
        internal GridReader(IDbCommand command, IDataReader reader, Identity identity, DynamicParameters dynamicParams, CancellationToken cancel)
            : this(command, reader, identity, dynamicParams)
        {
            this.cancel = cancel;
        }

        /// <summary>
        /// Read the next grid of results, returned as a dynamic object
        /// </summary>
        /// <remarks>Note: each row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
        public Task<IEnumerable<dynamic>> ReadAsync(bool buffered = true)
        {
            return ReadAsyncImpl<dynamic>(typeof(DapperRow), buffered);
        }

        /// <summary>
        /// Read the next grid of results
        /// </summary>
        public Task<IEnumerable<object>> ReadAsync(Type type, bool buffered = true)
        {
            if (type == null) throw new ArgumentNullException("type");
            return ReadAsyncImpl<object>(type, buffered);
        }

        /// <summary>
        /// Read the next grid of results
        /// </summary>
        public Task<IEnumerable<T>> ReadAsync<T>(bool buffered = true)
        {
            return ReadAsyncImpl<T>(typeof(T), buffered);
        }

        private async Task NextResultAsync()
        {
            if (await ((DbDataReader)reader).NextResultAsync(cancel).ConfigureAwait(false))
            {
                readCount++;
                gridIndex++;
                consumed = false;
            }
            else
            {
                // happy path; close the reader cleanly - no
                // need for "Cancel" etc
                reader.Dispose();
                reader = null;
                if (callbacks != null) callbacks.OnCompleted();
                Dispose();
            }
        }

        private Task<IEnumerable<T>> ReadAsyncImpl<T>(Type type, bool buffered)
        {
            if (reader == null) throw new ObjectDisposedException(GetType().FullName, "The reader has been disposed; this can happen after all data has been consumed");
            if (consumed) throw new InvalidOperationException("Query results must be consumed in the correct order, and each result can only be consumed once");
            var typedIdentity = identity.ForGrid(type, gridIndex);
            CacheInfo cache = SqlMapper.GetCacheInfo(typedIdentity, null, true);
            var deserializer = cache.Deserializer;

            int hash = SqlMapper.GetColumnHash(reader);
            if (deserializer.Func == null || deserializer.Hash != hash)
            {
                deserializer = new DeserializerState(hash, SqlMapper.GetDeserializer(type, reader, 0, -1, false));
                cache.Deserializer = deserializer;
            }
            consumed = true;
            if (buffered && this.reader is DbDataReader)
            {
                return ReadBufferedAsync<T>(gridIndex, deserializer.Func, typedIdentity);
            }
            else
            {
                var result = ReadDeferred<T>(gridIndex, deserializer.Func, typedIdentity);
                if (buffered) result = result.ToList(); // for the "not a DbDataReader" scenario
                return Task.FromResult(result);
            }
        }

        private async Task<IEnumerable<T>> ReadBufferedAsync<T>(int index, Func<IDataReader, object> deserializer, Identity typedIdentity)
        {
            //try
            //{
            var reader = (DbDataReader)this.reader;
            List<T> buffer = new List<T>();
            while (index == gridIndex && await reader.ReadAsync(cancel).ConfigureAwait(false))
            {
                buffer.Add((T)deserializer(reader));
            }
            if (index == gridIndex) // need to do this outside of the finally pre-C#6
            {
                await NextResultAsync().ConfigureAwait(false);
            }
            return buffer;
            //}
            //finally // finally so that First etc progresses things even when multiple rows
            //{
            //    if (index == gridIndex)
            //    {
            //        await NextResultAsync().ConfigureAwait(false);
            //    }
            //}
        }
    }
}
