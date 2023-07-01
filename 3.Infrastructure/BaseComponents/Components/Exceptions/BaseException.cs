using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Infrastructure.BaseExtensions;

namespace Infrastructure.BaseComponents.Components.Exceptions
{
    /// <summary>
    /// Класс базового исключения.
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class BaseException : Exception
    {
        /// <summary>
        /// Создание объекта с помощью конструктора ограничено.
        /// </summary>
        protected BaseException(string message = null, Exception innerException = null) 
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Получить реальное сообщение, в зависимости от языкового стандарта.
        /// </summary>
        protected static string GetRealMessage(string message, string localLangName, string localMessage,
            CultureInfo culture = null)
        {
            var currCulture = culture ?? CultureInfo.CurrentUICulture;
            
            return currCulture.TwoLetterISOLanguageName == localLangName 
                   || currCulture.Name == localLangName
                ? localMessage
                : message;
        }
        
        /// <summary>
        /// Создает новый экземпляр исключения <see cref="BaseException" /> (фабричный метод).
        /// </summary>
        /// <remarks>
        /// Подставляется либо сообщение об ошибке <see cref="localMessage" />
        /// (если текущий языковой стандарт равен <see cref="localLangName" />),
        /// либо <see cref="message" /> (в противном случае).
        /// </remarks>
        /// <param name="message">Сообщение об ошибке на языке по умолчанию,
        /// указывающее причину создания исключения.</param>
        /// <param name="innerException">Исключение, вызвавшее текущее исключение, или null,
        /// если внутреннее исключение не задано.</param>
        /// <param name="localLangName">Имя языкового стандарта.</param>
        /// <param name="localMessage">Сообщение об ошибке на языке <paramref name="localLangName"/>,
        /// указывающее причину создания исключения.</param>
        public static BaseException CreateException(
            string message = null, Exception innerException = null,
            string localLangName = null, string localMessage = null)
        {
            var ex = localLangName.IsNullOrEmpty()
                ? new BaseException(message, innerException)
                : new BaseException(GetRealMessage(message, localLangName, localMessage),
                    innerException);

            return ex;
        }

        /// <inheritdoc cref="CreateException(string, Exception, string, string)"/>
        /// <summary>
        /// Создает новый экземпляр исключения <see cref="TEx" /> (фабричный метод).
        /// </summary>
        public static TEx CreateException<TEx>(
            string message = null, Exception innerException = null,
            string localLangName = null, string localMessage = null) where TEx: Exception
        {
            if (localLangName.IsNullOrEmpty())
            {
                var ex = (TEx)Activator.CreateInstance(typeof(TEx), 
                    message, innerException);

                return ex;
            }
            else
            {
                var ex = (TEx)Activator.CreateInstance(typeof(TEx), 
                    GetRealMessage(message, localLangName, localMessage), innerException);

                return ex;
            }
        }
    }
}
