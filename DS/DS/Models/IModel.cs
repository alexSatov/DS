using DS.MathStructures.Points;

namespace DS.Models
{
    /// <summary>
    /// Интерфейс модели исследования
    /// </summary>
    public interface IModel
    {
        /// <summary>
        /// Модуль величины, которую можно считать выходом в бесконечность для модели
        /// </summary>
        public double AbsInf { get; }

        public double D12 { get; set; }
        public double D21 { get; set; }

        PointX GetNextPoint(PointX current);
        IModel Copy();
    }
}
