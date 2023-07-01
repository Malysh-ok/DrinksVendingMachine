using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Infrastructure.BaseExtensions;
using Infrastructure.BaseExtensions.ValueTypes;

namespace Infrastructure.BaseComponents.Components
{
    /// <summary>
    /// Класс, для работы с криптографией (шифрованием и т.п.).
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static class Crypto
    {
        /// <summary>
        /// Генератор криптографически защищённых случайных чисел.
        /// </summary>
        private static readonly RandomNumberGenerator Generator = RandomNumberGenerator.Create();

        /// <summary>
        /// Размер секретного ключа, байт.
        /// </summary>
        public const int SecretKeySize = 32;
        
        /// <summary>
        /// Размер вектора инициализации, байт.
        /// </summary>
        public const int IvSize = 16;

        /// <summary>
        /// Получить случайное 64-битное целое.
        /// </summary>
        public static long GetRandomLong()
        {
            var randomArr = new byte[8];
            Generator.GetBytes(randomArr);
            return BitConverter.ToInt64(randomArr, 0);
        }

        /// <summary>
        /// Получить случайный массив байт.
        /// </summary>
        public static byte[] GetRandomBytes(int count)
        {
            var randomArr = new byte[count];
            Generator.GetBytes(randomArr);
            return randomArr;
        }

        #region [---------- Хеши ---------]

        /// <summary>
        /// Вычислить хеш-функцию MD5 по потоку.
        /// </summary>
        public static byte[] GetMd5HashArray(Stream stream)
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));
            
            return MD5.Create().ComputeHash(stream);
        }
        
        /// <summary>
        /// Вычислить хеш-функцию MD5 по потоку.
        /// </summary>
        public static string GetMd5Hash(Stream stream)
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));

            return GetMd5HashArray(stream).BytesToHex();
        }

        /// <summary>
        /// Вычислить хеш-функцию MD5 по массиву байт.
        /// </summary>
        public static string GetMd5Hash(byte[] bytes)
        {
            if (bytes is null) throw new ArgumentNullException(nameof(bytes));
            
            return MD5.Create().ComputeHash(bytes).BytesToHex();
        }

        /// <summary>
        /// Вычислить хеш-функцию MD5 по строке.
        /// </summary>
        public static string GetMd5Hash(string str)
        {
            if (str is null) throw new ArgumentNullException(nameof(str));
            
            return GetMd5Hash(Encoding.UTF8.GetBytes(str));
        }
        
        /// <summary>
        /// Вычислить хеш-функцию SHA256 по потоку.
        /// </summary>
        public static byte[] GetSha256HashArray(Stream stream)
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));
            
            return SHA256.Create().ComputeHash(stream);
        }
        
        /// <summary>
        /// Вычислить хеш-функцию SHA256 по потоку.
        /// </summary>
        public static string GetSha256Hash(Stream stream)
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));
            
            return GetSha256HashArray(stream).BytesToHex();
        }
        
        /// <summary>
        /// Вычислить хеш-функцию SHA256 по массиву байт.
        /// </summary>
        public static string GetSha256Hash(byte[] bytes)
        {
            if (bytes is null) throw new ArgumentNullException(nameof(bytes));
            
            return SHA256.Create().ComputeHash(bytes).BytesToHex();
        }

        /// <summary>
        /// Вычислить хеш-функцию SHA256 по строке.
        /// </summary>
        public static string GetSha256Hash(string str)
        {
            if (str is null) throw new ArgumentNullException(nameof(str));
            
            return GetSha256Hash(Encoding.UTF8.GetBytes(str));
        }
        
        /// <summary>
        /// Вычислить хеш-функцию SHA512 по потоку.
        /// </summary>
        public static byte[] GetSha512HashArray(Stream stream)
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));
            
            return SHA512.Create().ComputeHash(stream);
        }
        
        /// <summary>
        /// Вычислить хеш-функцию SHA512 по потоку.
        /// </summary>
        public static string GetSha512Hash(Stream stream)
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));
            
            return GetSha512HashArray(stream).BytesToHex();
        }
        
        /// <summary>
        /// Вычислить хеш-функцию SHA512 по массиву байт.
        /// </summary>
        public static string GetSha512Hash(byte[] bytes)
        {
            if (bytes is null) throw new ArgumentNullException(nameof(bytes));
            
            return SHA512.Create().ComputeHash(bytes).BytesToHex();
        }

        /// <summary>
        /// Вычислить хеш-функцию SHA512 по строке.
        /// </summary>
        public static string GetSha512Hash(string str)
        {
            if (str is null) throw new ArgumentNullException(nameof(str));
            
            return GetSha512Hash(Encoding.UTF8.GetBytes(str));
        }
        #endregion

        #region [---------- Шифрация/дешифрация ---------]

        /// <summary>
        /// Зашифровать строку.
        /// </summary>
        /// <remarks>
        /// В случае неудачи возвращает null.
        /// </remarks>
        public static string Encrypt(string str, byte[] secretKey, byte[] iv)
        {
            if (str is null) throw new ArgumentNullException(nameof(str));
            if (secretKey is null) throw new ArgumentNullException(nameof(secretKey));
            if (iv is null) throw new ArgumentNullException(nameof(iv));
            
            var encryptBytes = EncryptBytes(Encoding.UTF8.GetBytes(str), secretKey, iv);
            
            return encryptBytes is null 
                ? null 
                : Convert.ToBase64String(encryptBytes);
        }

        /// <summary>
        /// Зашифровать массив байт.
        /// </summary>
        /// <remarks>
        /// В случае неудачи возвращает null.
        /// </remarks>
        public static byte[] EncryptBytes(byte[] bytes, byte[] secretKey, byte[] iv)
        {
            if (bytes is null) throw new ArgumentNullException(nameof(bytes));
            if (secretKey is null) throw new ArgumentNullException(nameof(secretKey));
            if (iv is null) throw new ArgumentNullException(nameof(iv));
            
            Aes alg = null;
            try
            {
                alg = Aes.Create();
                alg.Key = secretKey;
                alg.IV = iv;
                alg.Padding = PaddingMode.PKCS7;

                using var memStream = new MemoryStream();
                using var cryptoStream =
                    new CryptoStream(memStream, alg.CreateEncryptor(secretKey, iv), CryptoStreamMode.Write);
                cryptoStream.Write(bytes, 0, bytes.Length);
                cryptoStream.FlushFinalBlock();

                return memStream.ToArray();
            }
            catch
            {
                return null;
            }
            finally
            {
                alg?.Clear();
            }
        }
        
        /// <summary>
        /// Дешифровать строку.
        /// </summary>
        /// <remarks>
        /// В случае неудачи возвращает null.
        /// </remarks>
        public static string Decrypt(string str, byte[] secretKey, byte[] iv)
        {
            if (str is null) throw new ArgumentNullException(nameof(str));
            if (secretKey is null) throw new ArgumentNullException(nameof(secretKey));
            if (iv is null) throw new ArgumentNullException(nameof(iv));
            
            var bytes = new byte[str.Length];
                
            // Получаем массив байтов bytes из исходной строки str
            if (!Convert.TryFromBase64String(str, bytes, out var length))
                return null;

            Array.Resize(ref bytes, length);                             // укорачиваем массив под длину исх. строки
            var decryptBytes = DecryptBytes(bytes, secretKey, iv);  // дешифруем массив
            
            return decryptBytes is null 
                ? null 
                : Encoding.UTF8.GetString(decryptBytes);                 // массив байтов -> строка
        }

        /// <summary>
        /// Дешифровать массив байт.
        /// </summary>
        /// <remarks>
        /// В случае неудачи возвращает null.
        /// </remarks>
        public static byte[] DecryptBytes(byte[] bytes, byte[] secretKey, byte[] iv)
        {
            if (bytes is null) throw new ArgumentNullException(nameof(bytes));
            if (secretKey is null) throw new ArgumentNullException(nameof(secretKey));
            if (iv is null) throw new ArgumentNullException(nameof(iv));
            
            Aes alg = null;
            try
            {
                alg = Aes.Create();
                alg.Key = secretKey;
                alg.IV = iv;
                alg.Padding = PaddingMode.PKCS7;

                using var memStream = new MemoryStream(bytes);
                using var cryptoStream =
                    new CryptoStream(memStream, alg.CreateDecryptor(secretKey, iv), CryptoStreamMode.Read);

                return cryptoStream.ReadAllBytes();
            }
            catch
            {
                return null;
            }
            finally
            {
                alg?.Clear();
            }
        }
        #endregion

        #region [---------- Устарело ----------]

        /// <summary>
        /// Вычислить хеш-функцию MD5 по массиву байт.
        /// </summary>
        [Obsolete("Рекомендуется использовать GetMd5Hash(byte[] bytes).")]
        public static string ComputeHash(byte[] bytes)
        {
            return GetMd5Hash(bytes);
        }

        /// <summary>
        /// Вычислить хеш-функцию MD5 по строке.
        /// </summary>
        [Obsolete("Рекомендуется использовать GetMd5Hash(string str).")]
        public static string ComputeHash(string str)
        {
            return GetMd5Hash(str);
        }
        
        /// <summary>
        /// Вычислить хеш-функцию MD5 по потоку.
        /// </summary>
        [Obsolete("Рекомендуется использовать GetMd5Hash(Stream stream).")]
        public static string ComputeHash(Stream stream)
        {
            return GetMd5Hash(stream);
        }

        
        /// <summary>
        /// Вычислить хеш-функцию SHA256 по массиву байт.
        /// </summary>
        [Obsolete("Рекомендуется использовать GetSha256Hash(byte[] bytes).")]
        public static string GetSha256Checksum(byte[] bytes)
        {
            return GetSha256Hash(bytes);
        }

        /// <summary>
        /// Вычислить хеш-функцию SHA256 по строке.
        /// </summary>
        [Obsolete("Рекомендуется использовать GetSha256Hash(string str).")]
        public static string GetSha256Checksum(string str)
        {
            return GetSha256Hash(str);
        }

        /// <summary>
        /// Вычислить хеш-функцию SHA256 по потоку.
        /// </summary>
        [Obsolete("Рекомендуется использовать GetSha256Hash(Stream stream).")]
        public static string GetSha256Checksum(Stream stream)
        {
            return  GetSha256Hash(stream);
        }    

        #endregion
    }
}