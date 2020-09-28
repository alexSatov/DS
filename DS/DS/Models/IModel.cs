using DS.MathStructures.Points;

namespace DS.Models
{
    /// <summary>
    /// Модель исследования
    /// </summary>
    public interface IModel
    {
        /// <summary>
        /// Модуль величины, которую можно считать выходом в бесконечность для модели
        /// </summary>
        public double AbsInf { get; }

        /// <summary>
        /// Итерация точки
        /// </summary>
        PointX GetNextPoint(PointX current);

        /// <summary>
        /// Создание копии модели (исп. для распараллеливания)
        /// </summary>
        IModel Copy();
    }
}
