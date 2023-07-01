using System;
using System.Collections;
using System.Collections.Generic;

namespace Infrastructure.BaseComponents.Components.Exceptions
{
    /// <summary>
    /// Объект для работы со списком исключений.
    /// </summary>
    /// <typeparam name="TException">Тип исключений.</typeparam>
    public class ExceptionList<TException> : IEnumerable where TException : Exception
    {
        /// <summary>
        /// Список исключений.
        /// </summary>
        private List<TException> _exceptions;

        /// <summary>
        /// Признак того, что исключения отсутствуют.
        /// </summary>
        public bool IsNoExceptions => _exceptions.Count == 0;
        
        /// <summary>
        /// Конструктор.
        /// </summary>
        public ExceptionList()
        {
            _exceptions = new List<TException>();
        }

        /// <summary>
        /// Добавить исключение в список.
        /// </summary>
        public void Add(TException item)
        {
            _exceptions.Add(item);
        }
        
        /// <summary>
        /// Добавить в список исключения из <paramref name="addedExceptions"/>.
        /// </summary>
        public void AddRange(ExceptionList<TException> addedExceptions)
        {
            _exceptions.AddRange(addedExceptions.GetAll());
        }

        /// <summary>
        /// Получить все исключения списка.
        /// </summary>
        public IEnumerable<TException> GetAll()
        {
            return _exceptions;
        }
        
        /// <summary>
        /// Получить первое исключение списка.
        /// </summary>
        public TException GetFirst()
        {
            return _exceptions.Count > 0
                ? _exceptions[0]
                : null;
        }
        
        /// <summary>
        /// Получить последнее исключение списка.
        /// </summary>
        public TException GetLast()
        {
            return _exceptions.Count > 0
                ? _exceptions[^1]
                : null;
        }

        /// <summary>
        /// Удалить последнее исключение списка.
        /// </summary>
        public void RemoveLast()
        {
            if (_exceptions.Count > 0)
                _exceptions.RemoveAt(_exceptions.Count - 1);
        }
        
        /// <summary>
        /// Удалить все исключения списка.
        /// </summary>
        public void Clear()
        {
            _exceptions.Clear();
        }

        public IEnumerator GetEnumerator()
        {
            return _exceptions.GetEnumerator();
        }
    }
}