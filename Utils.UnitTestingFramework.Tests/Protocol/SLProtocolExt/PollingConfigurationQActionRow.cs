namespace Utils.UnitTestingFramework.Tests.Protocol.SLProtocolExt
{
    using Skyline.DataMiner.Scripting;

    // Created this class based on how DIS generates table row classes in real protocol solutions.
    public class PollingConfigurationQActionRow : QActionTableRow
    {
        public PollingConfigurationQActionRow() : base(0, 5)
        {
        }
        public PollingConfigurationQActionRow(object[] oRow) : base(0, 5, oRow)
        {
        }

        public object Pollingconfigurationinstance_901 { get { if (Columns.ContainsKey(0)) { return Columns[0]; } else { return null; } } set { if (Columns.ContainsKey(0)) { Columns[0] = value; } else { Columns.Add(0, value); } } }

        public object Pollingconfigurationdescription_902 { get { if (Columns.ContainsKey(1)) { return Columns[1]; } else { return null; } } set { if (Columns.ContainsKey(1)) { Columns[1] = value; } else { Columns.Add(1, value); } } }

        public object Pollingconfigurationperiod_903
        {
            get
            {
                if (Columns.ContainsKey(2))
                {
                    return Columns[2];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (Columns.ContainsKey(2))
                {
                    Columns[2] = value;
                }
                else
                { Columns.Add(2, value);
                }
            }
        }

        public object Pollingconfigurationlastpolled_904
        {
            get
            {
                if (Columns.ContainsKey(3))
                {
                    return Columns[3];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (Columns.ContainsKey(3))
                {
                    Columns[3] = value;
                }
                else
                {
                    Columns.Add(3, value);
                }
            }
        }

        public object Pollingconfigurationconnectionid_905
        {
            get
            {
                if (Columns.ContainsKey(4))
                {
                    return Columns[4];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (Columns.ContainsKey(4))
                {
                    Columns[4] = value;
                }
                else
                {
                    Columns.Add(4, value);
                }
            }
        }
    }
}