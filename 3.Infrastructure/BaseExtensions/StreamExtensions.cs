using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Phrases;

namespace Infrastructure.BaseExtensions
{
    /// <summary>
    /// Методы расширения для классов <see cref="System.IO.Stream"/>.
    /// </summary>
    [SuppressMessage("ReSharper", "InvalidXmlDocComment")]
    public static class StreamExtensions
    {
        private const int DEFAULT_BUFFER_SIZE = 2048 * 10;  // 20 КБайт

        /// <summary>
        /// Передвинуть указатель в потоке.
        /// </summary>
        [SuppressMessage("ReSharper", "MethodHasAsyncOverload")]
        private static async void Seek(this Stream stream, byte[] buffer, int offset, 
            CancellationTokenSource tokenSource = null)
        {
            CommonPhrases.Culture = CultureInfo.CurrentUICulture;       // устанавливаем яз. стандарт для фраз

            if (stream.CanSeek)
            {
                stream.Seek(offset, SeekOrigin.Begin);
            }
            else
            {
                var count = offset / buffer.Length;
                for (var i = 0; i <= count; i++)
                {
                    int bytesTotal;
                    var bytesDesired = i != count
                        ? buffer.Length
                        : offset % buffer.Length;

                    if (tokenSource == null)
                    {
                        bytesTotal = stream.Read(buffer, 0, bytesDesired);
                    }
                    else
                    {
                        tokenSource.Token.ThrowIfCancellationRequested();
                        bytesTotal = await stream.ReadAsync(buffer, 0,bytesDesired,
                            tokenSource.Token);
                    }

                    if (bytesDesired != bytesTotal)
                        throw new IOException(
                            CommonPhrases.Exception_SeekError.Format(nameof(stream)));
                }
            }
        }
        
        /// <summary>
        /// Чтение из потока всех данных.
        /// </summary>
        /// <param name="stream">Поток, из которого производится чтение.</param>
        /// <param name="buffer">Буфер для чтения.</param>
        /// <returns>Массив со считанными из потока данными.</returns>
        public static byte[] ReadAllBytes(this Stream stream, byte[] buffer = null)
        {
            if (stream is MemoryStream memoryStream)
                return memoryStream.ToArray();

            buffer ??= new byte[DEFAULT_BUFFER_SIZE];
            var streamCapacity = stream.CanSeek 
                ? (int)Math.Min(int.MaxValue, stream.Length) 
                : 0;
            using var ms = new MemoryStream(streamCapacity);
            int count;
            while ((count = stream.Read(buffer, 0, buffer.Length)) > 0)
                ms.Write(buffer, 0, count);
            return ms.ToArray();
        }

        /// <summary>
        /// Асинхронное чтение из потока всех данных.
        /// </summary>
        /// <param name="tokenSource">Объект для отмены асинхронной операции.</param>
        /// <returns>Задача, результат которой - считанные из потока данные.</returns>
        public static async Task<byte[]> ReadAllBytesAsync(this Stream stream, 
            CancellationTokenSource tokenSource, byte[] buffer = null)
        {
            if (stream is MemoryStream memoryStream)
                return memoryStream.ToArray();

            buffer ??= new byte[DEFAULT_BUFFER_SIZE];
            var streamCapacity = stream.CanSeek 
                ? (int)Math.Min(int.MaxValue, stream.Length) 
                : 0;
            using var ms = new MemoryStream(streamCapacity);
            int count;
            while ((count = await stream.ReadAsync(buffer, 0, buffer.Length, tokenSource.Token)) > 0)
            {
                tokenSource.Token.ThrowIfCancellationRequested();
                await ms.WriteAsync(buffer, 0, count, tokenSource.Token);
            }

            return ms.ToArray();
        }

        /// <inheritdoc cref="ReadBytes(Stream, int)"/>
        /// <param name="offset">Смещение в байтах относительно начала потока.</param>
        public static byte[] ReadBytes(this Stream stream, int offset, int length)
        {
            if (length == 0)
                return Array.Empty<byte>();
            if (stream is MemoryStream memoryStream)
                return memoryStream.ToArray().SubArray(offset, length);

            var buffer = new byte[length];
            var bytesTotal = 0;

            Seek(stream, buffer, offset);       // передвигаем указатель в потоке
            
            for (var bytesRead = 0; bytesRead != -1 && bytesTotal < buffer.Length; bytesTotal += bytesRead)
                bytesRead = stream.Read(buffer, bytesTotal, buffer.Length - bytesTotal);

            if (bytesTotal != buffer.Length)
                Array.Resize(ref buffer, bytesTotal);

            return buffer;
        }

        /// <summary>
        /// Чтение из потока указанного количества байт.
        /// </summary>
        /// <param name="stream">Поток, из которого производится чтение.</param>
        /// <param name="length">Количество байт для считывания.</param>
        /// <returns>Массив со считанными данными, содержащий не более <paramref name="length"/> байт.</returns>
        public static byte[] ReadBytes(this Stream stream, int length)
        {
            return ReadBytes(stream, 0, length);
        }

        /// <inheritdoc cref="ReadBytes(Stream, int, int)"/>
        /// <param name="tokenSource">Объект для отмены асинхронной операции.</param>
        /// <param name="buffer">Вспомогательный буфер для работы метода.</param>
        /// <returns>Задача, результат которой - массив со считанными из потока данными,
        /// содержащий не более <paramref name="length"/> байт.</returns>
        public static async Task<byte[]> ReadBytesAsync
        (this Stream stream,
            CancellationTokenSource tokenSource,
            int offset,
            long length,
            byte[] buffer = null)
        {
            if (length == 0)
                return Array.Empty<byte>();
            if (stream is MemoryStream memoryStream)
                return memoryStream.ToArray().SubArray(offset, length);

            buffer ??= new byte[DEFAULT_BUFFER_SIZE];
            var streamCapacity = stream.CanSeek 
                ? (int)Math.Min(int.MaxValue, stream.Length) 
                : 0;
            
            Seek(stream, buffer, offset, tokenSource);       // передвигаем указатель в потоке

            using var ms = new MemoryStream(streamCapacity);
            while (ms.Length < length)
            {
                tokenSource.Token.ThrowIfCancellationRequested();
                var count = length <= ms.Length + buffer.Length ? (int)(length % buffer.Length) : buffer.Length;
                count = await stream.ReadAsync(buffer, 0, count, tokenSource.Token);
                tokenSource.Token.ThrowIfCancellationRequested();
                await ms.WriteAsync(buffer, 0, count, tokenSource.Token);
            }

            return ms.ToArray();
        }

        /// <summary>
        /// Асинхронное чтение из потока указанного количества байт.
        /// </summary>
        /// <param name="tokenSource">Объект для отмены асинхронной операции.</param>
        /// <param name="buffer">Вспомогательный буфер для работы метода.</param>
        /// <returns>Задача, результат которой - массив со считанными из потока данными,
        /// содержащий не более <paramref name="length"/> байт.</returns>
        public static async Task<byte[]> ReadBytesAsync
        (this Stream stream,
            CancellationTokenSource tokenSource,
            long length,
            byte[] buffer = null)
        {
            return await stream.ReadBytesAsync(tokenSource, 0, length, buffer);
        }

        /// <inheritdoc cref="ReadToStream(Stream, Stream, long, byte[])"/>
        /// <param name="offset">Смещение в байтах относительно начала потока.</param>
        public static void ReadToStream
        (
            this Stream stream,
            Stream outStream,
            int offset,
            long length,
            byte[] buffer = null)
        {
            if (length == 0)
                return;

            buffer ??= new byte[DEFAULT_BUFFER_SIZE];

            Seek(stream, buffer, offset);       // передвигаем указатель в потоке
            
            while (outStream.Length < length)
            {
                var count = length <= outStream.Length + buffer.Length 
                    ? (int)(length % buffer.Length) 
                    : buffer.Length;
                count = stream.Read(buffer, 0, count);
                outStream.Write(buffer, 0, count);
            }
        }

        /// <inheritdoc cref="ReadBytes(Stream, int)"/>
        /// <summary>
        /// Чтение из текущего потока в поток <paramref name="outStream"/>.
        /// </summary>
        /// <param name="outStream">Выходной поток.</param>
        /// <param name="buffer">Вспомогательный буфер для работы метода.</param>
        /// <returns />
        public static void ReadToStream
        (
            this Stream stream,
            Stream outStream,
            long length,
            byte[] buffer = null)
        {
            stream.ReadToStream(outStream, 0, length, buffer);
        }

        /// <inheritdoc cref="ReadToStreamAsync(Stream, Stream, CancellationTokenSource, long, byte[])"/>
        /// <param name="offset">Смещение в байтах относительно начала потока.</param>
        public static async Task ReadToStreamAsync
        (
            this Stream stream,
            Stream outStream,
            CancellationTokenSource tokenSource,
            int offset,
            long length,
            byte[] buffer = null)
        {
            if (length == 0)
            {
                return;
            }

            buffer ??= new byte[DEFAULT_BUFFER_SIZE];
            
            Seek(stream, buffer, offset, tokenSource);       // передвигаем указатель в потоке
            
            while (outStream.Length < length)
            {
                tokenSource.Token.ThrowIfCancellationRequested();
                var count = length <= outStream.Length + buffer.Length 
                    ? (int)(length % buffer.Length) 
                    : buffer.Length;
                count = await stream.ReadAsync(buffer, 0, count, tokenSource.Token);
                tokenSource.Token.ThrowIfCancellationRequested();
                await outStream.WriteAsync(buffer, 0, count, tokenSource.Token);
            }
        }

        /// <inheritdoc cref="ReadToStream(Stream, Stream, long, byte[])"/>
        /// <summary>
        /// Асинхронное чтение из текущего потока в поток <paramref name="outStream"/>.
        /// </summary>
        /// <param name="tokenSource">Объект для отмены асинхронной операции.</param>
        /// <returns><see cref="Task"/></returns>
        public static async Task ReadToStreamAsync
        (
            this Stream stream,
            Stream outStream,
            CancellationTokenSource tokenSource,
            long length,
            byte[] buffer = null)
        {
            await stream.ReadToStreamAsync(outStream, tokenSource, 0, length, buffer);
        }
        
        /// <summary>
        /// Получить количество найденных в потоке повторений строки.
        /// </summary>
        /// <param name="stream">Поток, из которого производится чтение.</param>
        /// <param name="searchString">Искомая строка.</param>
        /// <param name="encoding">Кодировка символов.</param>
        public static int GetSubstringCount(this Stream stream, string searchString, Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            int charCount;                                      // количество считанных символов
            var substringCount = 0;                             // количество найденых повторений строки
            var chBuffer = new char[DEFAULT_BUFFER_SIZE];       // буфер для работы
            var sr = new StreamReader(stream, encoding, true);

            do
            {
                // Читаем кусок данных из sr в буфер 
                charCount = sr.ReadBlock(chBuffer, 0, DEFAULT_BUFFER_SIZE);

                // Подсчитываем повторения строки
                var str = new string(chBuffer);
                substringCount += str.GetSubstringCount(searchString);
            } 
            while(charCount >= DEFAULT_BUFFER_SIZE);    
            
            sr.Close();
            return substringCount;
        }

        
        /// <summary>
        /// Подсчет символов перевода строки в потоке.
        /// </summary>
        /// <param name="stream">Поток, из которого производится чтение.</param>
        /// <param name="encoding">Кодировка символов.</param>
        public static int GetNewlineCharCount(this Stream stream, Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            int charCount;                                      // количество считанных символов
            var crLfCount = 0;                                  // количество переводов строк
            var isCr = false;                                   // признак возврата каретки
            var isCrLf = false;                                 // признак возврата каретки + перевода строки 
            var chBuffer = new char[DEFAULT_BUFFER_SIZE];       // буфер для работы
            var sr = new StreamReader(stream, encoding, true);
            
            do
            {
                // Читаем кусок данных из sr в буфер 
                charCount = sr.ReadBlock(chBuffer, 0, DEFAULT_BUFFER_SIZE);

                // Подсчитываем переводы строк
                for (var i = 0; i < charCount; i++)
                {
                    isCr = chBuffer[i] == '\r';
                    isCrLf = isCr && chBuffer[i + 1] == '\n';

                    if (isCrLf)
                    {
                        isCr = false;
                        i++;
                    }
                    else
                        isCr |= chBuffer[i] == '\n';

                    if (isCr || isCrLf)
                        crLfCount++;
                }
            } 
            while(charCount >= DEFAULT_BUFFER_SIZE);

            // Если есть символы между последним переводом строки и EOF
            if (!isCr && !isCrLf)
                crLfCount++;
            
            sr.Close();
            return crLfCount;
        }
    }
}
