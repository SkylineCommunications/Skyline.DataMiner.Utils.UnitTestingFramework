namespace Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Table
{
    using System;

    public class CellModel : ParameterModelBase<ColumnDefinition>
    {
        public CellModel(ColumnDefinition columnDefinition, object value, DateTime? timestamp = null)
            : base(columnDefinition, value, timestamp)
        {

        }
    }
}
