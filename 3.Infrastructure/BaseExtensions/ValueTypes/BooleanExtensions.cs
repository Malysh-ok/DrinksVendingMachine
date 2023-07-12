namespace Infrastructure.BaseExtensions.ValueTypes
{
    /// <summary>
    /// Методы расширения для <see cref="bool" />.
    /// </summary>
    public static class BooleanExtensions
    {
        /// <summary>
        /// Получить булево значение из обнуляемого типа.
        /// </summary>
        public static bool FromNullable(this bool? value, bool defaultValue = false)
        {
            return value ?? defaultValue;
        }
        
        /// <summary>
        /// Преобразовать в число.
        /// </summary>
        public static int ToInt(this bool value)
        {
            return value ? 1 : 0;
        }
    }
}
