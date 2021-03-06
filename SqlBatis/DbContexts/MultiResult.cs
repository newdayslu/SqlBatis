﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace SqlBatis
{
    public interface IMultiResult : IDisposable
    {
        /// <summary>
        /// 返回当前dynamic类型结果集
        /// </summary>
        /// <returns></returns>
        List<dynamic> GetList();
        /// <summary>
        /// 异步返回当前dynamic类型结果集
        /// </summary>
        /// <returns></returns>
        Task<List<dynamic>> GetListAsync();
        /// <summary>
        /// 返回当前T结果集
        /// </summary>
        /// <typeparam name="T">结果集类型</typeparam>
        /// <returns></returns>
        List<T> GetList<T>();
        /// <summary>
        ///  异步返回当前T类型结果集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<List<T>> GetListAsync<T>();
        /// <summary>
        /// 返回当前dynamic类型结果
        /// </summary>
        /// <returns></returns>
        dynamic Get();
        /// <summary>
        /// 异步返回当前dynamic类型结果
        /// </summary>
        /// <returns></returns>
        Task<dynamic> GetAsync();
        /// <summary>
        /// 返回当前T类型结果
        /// </summary>
        /// <typeparam name="T">结果集类型</typeparam>
        /// <returns></returns>
        T Get<T>();
        /// <summary>
        /// 异步返回当前T类型结果
        /// </summary>
        /// <typeparam name="T">结果集类型</typeparam>
        /// <returns></returns>
        Task<T> GetAsync<T>();
    }

    public class MultiResult : IMultiResult
    {
        private readonly IDataReader _reader = null;
        
        private readonly IDbCommand _command = null;

        private readonly ITypeMapper _mapper = null;

        internal MultiResult(IDbCommand command, ITypeMapper mapper)
        {
            _command = command;
            _reader = command.ExecuteReader();
            _mapper = mapper;
        }

        public void Dispose()
        {
            _reader?.Dispose();
            _command?.Dispose();
        }

        public T Get<T>()
        {
            return GetList<T>().FirstOrDefault();
        }

        public async Task<T> GetAsync<T>()
        {
            return (await GetListAsync<T>()).FirstOrDefault();
        }

        public dynamic Get()
        {
            return GetList().FirstOrDefault();
        }
       
        public async Task<dynamic> GetAsync()
        {
            return (await GetListAsync()).FirstOrDefault();
        }
      
        public async Task<List<dynamic>> GetListAsync()
        {
            var handler = TypeConvert.GetSerializer();
            var list = new List<dynamic>();
            while (await (_reader as DbDataReader).ReadAsync())
            {
                list.Add(handler(_reader));
            }
            _reader.NextResult();
            return list;
        }
      
        public List<dynamic> GetList()
        {
            var handler = TypeConvert.GetSerializer();
            var list = new List<dynamic>();
            while (_reader.Read())
            {
                list.Add(handler(_reader));
            }
            _reader.NextResult();
            return list;
        }
      
        public List<T> GetList<T>()
        {
            var handler = TypeConvert.GetSerializer<T>(_mapper, _reader);
            var list = new List<T>();
            while (_reader.Read())
            {
                list.Add(handler(_reader));
            }
            _reader.NextResult();
            return list;
        }

        public async Task<List<T>> GetListAsync<T>()
        {
            var handler = TypeConvert.GetSerializer<T>(_mapper, _reader);
            var list = new List<T>();
            while (await (_reader as DbDataReader).ReadAsync())
            {
                list.Add(handler(_reader));
            }
            _reader.NextResult();
            return list;
        }
    }
}
