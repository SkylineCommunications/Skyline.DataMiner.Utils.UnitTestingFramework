namespace Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Moq;

    using Skyline.DataMiner.Core.DataMinerSystem.Common;
    using Skyline.DataMiner.Core.DataMinerSystem.Common.Selectors;
    using Skyline.DataMiner.Core.DataMinerSystem.Common.Subscription.Monitors;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common;
    using Skyline.DataMiner.Core.DataMinerSystem.Common.Templates;
    using Skyline.DataMiner.Core.DataMinerSystem.Common.Properties;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Interfaces;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Table;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Standalone;
    using ParameterChangeEventMessage = Skyline.DataMiner.Net.Messages.ParameterChangeEventMessage;
    using ParameterTableUpdateEventMessage = Skyline.DataMiner.Net.Messages.ParameterTableUpdateEventMessage;
    using ParameterValue = Skyline.DataMiner.Net.Messages.ParameterValue;

    /// <summary>
    /// A pre-arranged mock of <see cref="IDmsElement"/>.
    /// </summary>
    /// <remarks>
    /// The mock uses a protocol.xml file to determine which standalone parameters and tables the element contains,
    /// and keeps track of the values that are set on and retrieved from them.
    /// </remarks>
    public class IDmsElementMock : Mock<IDmsElement>
    {
        private readonly Cache cache;
        private int activeAlarmCount;
        private int criticalAlarmCount;
        private int majorAlarmCount;
        private int minorAlarmCount;
        private int warningAlarmCount;
        private AlarmLevel alarmLevel = Skyline.DataMiner.Core.DataMinerSystem.Common.AlarmLevel.Undefined;
        private ElementState state = ElementState.Active;
        private readonly int agentId;
        private readonly int id;
        private readonly string pathToProtocolXml;
        private readonly Dictionary<string, Action<ElementAlarmlevelChange>> alarmLevelMonitors = new Dictionary<string, Action<ElementAlarmlevelChange>>();
        private readonly Dictionary<string, Action<ElementNameChange>> nameMonitors = new Dictionary<string, Action<ElementNameChange>>();
        private readonly Dictionary<string, Action<ElementStateChange>> stateMonitors = new Dictionary<string, Action<ElementStateChange>>();
        private readonly ParametersAndTables parametersAndTables;
        private readonly Dictionary<int, DmsTableMock> tableMocks = new Dictionary<int, DmsTableMock>();
        private readonly Dictionary<string, Mock> standaloneParameterMocks = new Dictionary<string, Mock>();
        private readonly string protocolName;
        private readonly string protocolVersion;
        /// Gets or sets the number of active alarms returned by the mock.
        public int ActiveAlarmCount
        {
            get
            {
                return activeAlarmCount;
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                activeAlarmCount = value;
            }
        }
        /// Gets or sets the number of active critical alarms returned by the mock.
        public int CriticalAlarmCount
        {
            get
            {
                return criticalAlarmCount;
            }

            set
            {
                if (value < 0 || value > ActiveAlarmCount)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "The critical alarm count must be between zero and the total active alarm count.");
                }

                criticalAlarmCount = value;
            }
        }

        /// Gets or sets the number of active major alarms returned by the mock.
        public int MajorAlarmCount
        {
            get => majorAlarmCount;
            set => majorAlarmCount = ValidateAlarmCount(value);
        }

        /// Gets or sets the number of active minor alarms returned by the mock.
        public int MinorAlarmCount
        {
            get => minorAlarmCount;
            set => minorAlarmCount = ValidateAlarmCount(value);
        }

        /// Gets or sets the number of active warning alarms returned by the mock.
        public int WarningAlarmCount
        {
            get => warningAlarmCount;
            set => warningAlarmCount = ValidateAlarmCount(value);
        }
        /// Gets or sets the alarm level returned by the mock.
        public AlarmLevel AlarmLevel
        {
            get
            {
                return alarmLevel;
            }

            set
            {
                if (!Enum.IsDefined(typeof(AlarmLevel), value))
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                alarmLevel = value;
                NotifyAlarmLevelChanged();
            }
        }
        /// Gets or sets the element state returned by the mock.
        public ElementState State
        {
            get
            {
                return state;
            }

            set
            {
                if (!Enum.IsDefined(typeof(ElementState), value))
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                state = value;
                NotifyStateChanged();
            }
        }

        /// Gets or sets the element description returned by the mock.
        public string Description { get; set; } = String.Empty;
        public string Type { get; set; } = String.Empty;

        /// Gets or sets the protocol returned by the mock.
        public IDmsProtocol Protocol => cache.GetProtocol(protocolName, protocolVersion)?.Object;
        /// Gets or sets the alarm template returned by the mock.
        public IDmsAlarmTemplate AlarmTemplate { get; set; }

        /// Gets or sets the trend template returned by the mock.
        public IDmsTrendTemplate TrendTemplate { get; set; }

        /// Gets or sets the DVE settings returned by the mock.
        public IDveSettings DveSettings { get; set; } = new Mock<IDveSettings>().Object;

        /// Gets or sets the redundancy settings returned by the mock.
        public IRedundancySettings RedundancySettings { get; set; } = new Mock<IRedundancySettings>().Object;

        /// Gets or sets a value indicating whether element startup is complete.
        public bool IsStartupComplete { get; set; } = true;

        /// Gets or sets the advanced settings returned by the mock.
        public IAdvancedSettings AdvancedSettings { get; set; } = new Mock<IAdvancedSettings>().Object;

        /// Gets or Sets the collection of IElementConnection objects.
        public IElementConnectionCollection Connections { get; set; } = CreateEmptyConnectionCollection();

        /// Gets the function settings of this element.
        public IFunctionSettings FunctionSettings { get; set; } = new Mock<IFunctionSettings>().Object;

        public IPropertyCollection<IDmsElementProperty, IDmsElementPropertyDefinition> Properties { get; set; } = CreateEmptyPropertyCollection<IDmsElementProperty, IDmsElementPropertyDefinition>();

        internal List<int> ViewIds { get; } = new List<int>();

        public IReplicationSettings ReplicationSettings { get; set; } = new Mock<IReplicationSettings>().Object;

        /// <summary>
        /// Gets or sets the spectrum analyzer component returned by the mock.
        /// </summary>
        public IDmsSpectrumAnalyzer SpectrumAnalyzer { get; set; }

        /// <summary>
        /// Gets a standalone parameter mock belonging to this element.
        /// </summary>
        /// <typeparam name="T">The parameter value type.</typeparam>
        /// <param name="parameterId">The parameter ID.</param>
        /// <returns>The standalone parameter mock.</returns>
        public DmsStandaloneParameterMock<T> GetStandaloneParameterMock<T>(int parameterId)
        {
            GetStandaloneParameterObject(typeof(T), parameterId);

            var cacheKey = $"{parameterId}|{typeof(T).AssemblyQualifiedName}";

            return (DmsStandaloneParameterMock<T>)standaloneParameterMocks[cacheKey];
        }

        /// <summary>
        /// Gets a table belonging to this element mock.
        /// </summary>
        /// <param name="tableId">The table ID.</param>
        /// <returns>The table.</returns>
        public DmsTableMock GetDmsTableMock(int tableId)
        {
            GetTableObject(tableId);

            return tableMocks[tableId];
        }

        /// <summary>
        /// Adds this element to the specified view.
        /// </summary>
        /// <param name="viewId">The view ID.</param>
        public void AddView(int viewId)
        {
            EnsureElementExists(agentId, id);

            var viewMock = cache.GetView(viewId);

            if (viewMock == null)
            {
                throw new ViewNotFoundException(viewId);
            }

            if (!ViewIds.Contains(viewId))
            {
                ViewIds.Add(viewId);
            }

            if (!viewMock.ElementIds.Any(elementId => elementId.AgentId == agentId && elementId.ElementId == id))
            {
                viewMock.ElementIds.Add(new DmsElementId(agentId, id));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IDmsElementMock"/> class.
        /// </summary>
        /// <param name="pathToProtocolXml">The path to the protocol.xml file.</param>
        /// <param name="id">The element ID.</param>
        /// <param name="agentId">The DataMiner Agent ID.</param>
        /// <param name="name">The element name.</param>
        internal IDmsElementMock(Cache cache, string pathToProtocolXml, int id = 0, int agentId = 0, string name = "Element", IProtocolModel protocolModel = null)
        {
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
            this.pathToProtocolXml = pathToProtocolXml;
            this.id = id;
            this.agentId = agentId;
            protocolModel = protocolModel ?? ProtocolModelBuilder.Build(pathToProtocolXml);
            parametersAndTables = ParametersAndTablesBuilder.Build(protocolModel); // TODO get ParameterAndTableDefinitions from IDmsProtocolMock in Cache and initialize ParametersAndTables from that 

            foreach (var parameterModel in parametersAndTables.GetParameters())
            {
                parameterModel.Changed += ParameterModel_Changed;
            }

            foreach (var tableModel in parametersAndTables.GetTables())
            {
                tableModel.RowChanged += TableModel_RowChanged;
            }

            var protocolMock = cache.GetProtocol(protocolModel.Protocol.Name.Value, protocolModel.Protocol.Version.Value);
            if (protocolMock == null)
            {
                protocolMock = new IDmsProtocolMock(protocolModel, pathToProtocolXml);
                cache.AddProtocol(protocolMock);
            }

            protocolName = protocolMock.Name;
            protocolVersion = protocolMock.ReferencedVersion;

            Setup(e => e.AdvancedSettings).Returns(() => AdvancedSettings);

            Setup(e => e.Id).Returns(id);

            Setup(e => e.AgentId).Returns(agentId);

            Setup(e => e.DmsElementId).Returns(new DmsElementId(agentId, id));
            Setup(e => e.Host).Returns(() => cache.GetDma(agentId)?.Object);

            ValidateName(name);

            var elementName = name;

            Setup(e => e.Name).Returns(() => elementName);

            SetupSet(e => e.Name = It.IsAny<string>()).Callback((string value) => { ValidateName(value); this.cache.UpdateElementName(agentId, id, elementName, value); elementName = value; NotifyNameChanged(value); });

            Setup(e => e.Description).Returns(() => Description);
            SetupSet(e => e.Description = It.IsAny<string>()).Callback((string value) => Description = value);

            Setup(e => e.Type).Returns(() => Type);

            Setup(e => e.State).Returns(() => State);

            Setup(e => e.Delete()).Callback(() => { EnsureElementExists(agentId, id); EnsureStateChangeIsSupported(); State = ElementState.Deleted; this.cache.RemoveElement(agentId, id); });
            Setup(e => e.Pause()).Callback(() => { EnsureElementExists(agentId, id); EnsureStateChangeIsSupported(); State = ElementState.Paused; });
            Setup(e => e.Restart()).Callback(() => { EnsureElementExists(agentId, id); EnsureStateChangeIsSupported(); State = ElementState.Restart; });
            Setup(e => e.Start()).Callback(() => { EnsureElementExists(agentId, id); EnsureStateChangeIsSupported(); State = ElementState.Active; });
            Setup(e => e.Stop()).Callback(() => { EnsureElementExists(agentId, id); EnsureStateChangeIsSupported(); State = ElementState.Stopped; });

            Setup(e => e.Protocol).Returns(() => Protocol);

            Setup(e => e.AlarmTemplate).Returns(() => AlarmTemplate);

            SetupSet(e => e.AlarmTemplate = It.IsAny<IDmsAlarmTemplate>()).Callback((IDmsAlarmTemplate value) => AlarmTemplate = value);

            Setup(e => e.TrendTemplate).Returns(() => TrendTemplate);

            SetupSet(e => e.TrendTemplate = It.IsAny<IDmsTrendTemplate>()).Callback((IDmsTrendTemplate value) => TrendTemplate = value);

            Setup(e => e.DveSettings).Returns(() => DveSettings);
            Setup(e => e.RedundancySettings).Returns(() => RedundancySettings);

            Setup(e => e.GetActiveAlarmCount()).Returns(() =>
            {
                EnsureAlarmInformationIsAvailable(agentId, id);
                return ActiveAlarmCount;
            });

            Setup(e => e.GetActiveCriticalAlarmCount()).Returns(() =>
            {
                EnsureAlarmInformationIsAvailable(agentId, id);
                return CriticalAlarmCount;
            });

            Setup(e => e.GetActiveMajorAlarmCount()).Returns(() =>
            {
                EnsureAlarmInformationIsAvailable(agentId, id);
                return MajorAlarmCount;
            });

            Setup(e => e.GetActiveMinorAlarmCount()).Returns(() =>
            {
                EnsureAlarmInformationIsAvailable(agentId, id);
                return MinorAlarmCount;
            });

            Setup(e => e.GetActiveWarningAlarmCount()).Returns(() =>
            {
                EnsureAlarmInformationIsAvailable(agentId, id);
                return WarningAlarmCount;
            });

            Setup(e => e.IsStartupComplete()).Returns(() => IsStartupComplete);

            Setup(e => e.GetAlarmLevel()).Returns(() => AlarmLevel);

            Setup(element => element.Connections).Returns(() => Connections);
            SetupSet(element => element.Connections = It.IsAny<IElementConnectionCollection>()).Callback((IElementConnectionCollection value) => Connections = value);
            Setup(element => element.FunctionSettings).Returns(() => FunctionSettings);
            Setup(element => element.Properties).Returns(() => Properties);
            Setup(element => element.ReplicationSettings).Returns(() => ReplicationSettings);
            Setup(element => element.SpectrumAnalyzer).Returns(() => SpectrumAnalyzer);
            Setup(element => element.Views).Returns(() => new HashSet<IDmsView>(ViewIds.Select(viewId => cache.GetView(viewId)).Where(viewMock => viewMock != null).Select(viewMock => viewMock.Object)));
            Setup(element => element.Exists()).Returns(() => cache.GetElement(agentId, id) != null && State != ElementState.Deleted);
            Setup(element => element.Update()).Callback(() => EnsureElementExists(agentId, id));

            Setup(element => element.Duplicate(It.IsAny<string>(), It.IsAny<IDma>())).Returns((string newElementName, IDma agent) => Duplicate(newElementName, agent));

            Setup(element => element.StartAlarmLevelMonitor(It.IsAny<string>(), It.IsAny<Action<ElementAlarmlevelChange>>())).Callback((string sourceId, Action<ElementAlarmlevelChange> onChange) => StartMonitor(alarmLevelMonitors, sourceId, onChange));
            Setup(element => element.StartAlarmLevelMonitor(It.IsAny<string>(), It.IsAny<Action<ElementAlarmlevelChange>>(), It.IsAny<TimeSpan>())).Callback((string sourceId, Action<ElementAlarmlevelChange> onChange, TimeSpan subscribeTimeout) => StartMonitor(alarmLevelMonitors, sourceId, onChange));
            Setup(element => element.StopAlarmLevelMonitor(It.IsAny<string>(), It.IsAny<bool>())).Callback((string sourceId, bool force) => StopMonitor(alarmLevelMonitors, sourceId));
            Setup(element => element.StopAlarmLevelMonitor(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<bool>())).Callback((string sourceId, TimeSpan subscribeTimeout, bool force) => StopMonitor(alarmLevelMonitors, sourceId));

            Setup(element => element.StartNameMonitor(It.IsAny<string>(), It.IsAny<Action<ElementNameChange>>())).Callback((string sourceId, Action<ElementNameChange> onChange) => StartMonitor(nameMonitors, sourceId, onChange));
            Setup(element => element.StartNameMonitor(It.IsAny<string>(), It.IsAny<Action<ElementNameChange>>(), It.IsAny<TimeSpan>())).Callback((string sourceId, Action<ElementNameChange> onChange, TimeSpan subscribeTimeout) => StartMonitor(nameMonitors, sourceId, onChange));
            Setup(element => element.StopNameMonitor(It.IsAny<string>(), It.IsAny<bool>())).Callback((string sourceId, bool force) => StopMonitor(nameMonitors, sourceId));
            Setup(element => element.StopNameMonitor(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<bool>())).Callback((string sourceId, TimeSpan subscribeTimeout, bool force) => StopMonitor(nameMonitors, sourceId));

            Setup(element => element.StartStateMonitor(It.IsAny<string>(), It.IsAny<Action<ElementStateChange>>())).Callback((string sourceId, Action<ElementStateChange> onChange) => StartMonitor(stateMonitors, sourceId, onChange));
            Setup(element => element.StartStateMonitor(It.IsAny<string>(), It.IsAny<Action<ElementStateChange>>(), It.IsAny<TimeSpan>())).Callback((string sourceId, Action<ElementStateChange> onChange, TimeSpan subscribeTimeout) => StartMonitor(stateMonitors, sourceId, onChange));
            Setup(element => element.StopStateMonitor(It.IsAny<string>(), It.IsAny<bool>())).Callback((string sourceId, bool force) => StopMonitor(stateMonitors, sourceId));
            Setup(element => element.StopStateMonitor(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<bool>())).Callback((string sourceId, TimeSpan subscribeTimeout, bool force) => StopMonitor(stateMonitors, sourceId));


            SetupStandaloneParameters();
            SetupTables();
        }

        private void ParameterModel_Changed(object sender, ParameterModelChangedEventArgs e)
        {
            var message = new ParameterChangeEventMessage(agentId, id, e.ParameterDefinition.Pid)
            {
                LastChange = e.NewTimestamp,
                NewValue = ToParameterValue(e.NewValue),
            };

            cache.GetConnection().NotifySubscriptions(message);
        }

        private void TableModel_RowChanged(object sender, RowChangedEventArgs e)
        {
            if (!(sender is ITableModel tableModel))
            {
                return;
            }

            bool isDeleted = e.ChangeType == RowChangeType.Deleted;
            var row = isDeleted ? null : tableModel.GetRow(e.PrimaryKey);

            var message = new ParameterTableUpdateEventMessage(agentId, id, tableModel.TableId)
            {
                IndexColumnID = tableModel.Schema.PrimaryKeyColumn.Pid,
                TableIndex = e.PrimaryKey,
                TableIndexPK = e.PrimaryKey,
                IsDeleted = isDeleted,
                LastChange = DateTime.Now,
                NewValue = ToParameterValue(isDeleted ? new object[0][] : new[] { row }),
                DeletedRows = isDeleted ? new[] { e.PrimaryKey } : new string[0],
            };

            cache.GetConnection().NotifySubscriptions(message);
        }

        private static ParameterValue ToParameterValue(object value)
        {
            if (value == null || value is DBNull)
            {
                return ParameterValue.Empty;
            }

            if (value is string stringValue)
            {
                return new ParameterValue(stringValue);
            }

            if (value is int intValue)
            {
                return new ParameterValue(intValue);
            }

            if (value is DateTime dateValue)
            {
                return new ParameterValue(dateValue);
            }

            if (value is Array arrayValue)
            {
                return new ParameterValue(arrayValue);
            }

            if (value is IConvertible)
            {
                return new ParameterValue(Convert.ToDouble(value, System.Globalization.CultureInfo.InvariantCulture));
            }

            return new ParameterValue(Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture));
        }

        private int ValidateAlarmCount(int value)
        {
            if (value < 0 || value > ActiveAlarmCount)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "The alarm count must be between zero and the total active alarm count.");
            }

            return value;
        }

        private IDmsElement Duplicate(string newElementName, IDma agent)
        {
            EnsureElementExists(agentId, id);
            EnsureStateChangeIsSupported();
            ValidateName(newElementName);

            if (agent == null)
            {
                throw new ArgumentNullException(nameof(agent));
            }

            var targetAgentMock = cache.GetDma(agent.Id);

            if (targetAgentMock == null || !ReferenceEquals(targetAgentMock.Object, agent))
            {
                throw new AgentNotFoundException(agent.Id);
            }

            var duplicate = targetAgentMock.CreateElement(pathToProtocolXml, targetAgentMock.GetNextElementId(), newElementName);
            duplicate.Description = Description;
            duplicate.Type = Type;
            duplicate.AlarmTemplate = AlarmTemplate;
            duplicate.TrendTemplate = TrendTemplate;
            duplicate.DveSettings = DveSettings;
            duplicate.RedundancySettings = RedundancySettings;
            duplicate.AdvancedSettings = AdvancedSettings;
            duplicate.Connections = Connections;
            duplicate.FunctionSettings = FunctionSettings;
            duplicate.Properties = Properties;
            duplicate.ReplicationSettings = ReplicationSettings;
            duplicate.SpectrumAnalyzer = SpectrumAnalyzer;

            return duplicate.Object;
        }

        private static void StartMonitor<TChange>(IDictionary<string, Action<TChange>> monitors, string sourceId, Action<TChange> onChange)
        {
            if (sourceId == null)
            {
                throw new ArgumentNullException(nameof(sourceId));
            }

            if (onChange == null)
            {
                throw new ArgumentNullException(nameof(onChange));
            }

            monitors[sourceId] = onChange;
        }

        private static void StopMonitor<TChange>(IDictionary<string, Action<TChange>> monitors, string sourceId)
        {
            if (sourceId == null)
            {
                throw new ArgumentNullException(nameof(sourceId));
            }

            monitors.Remove(sourceId);
        }

        private void NotifyAlarmLevelChanged()
        {
            foreach (var monitor in alarmLevelMonitors.ToList())
            {
                monitor.Value(new ElementAlarmlevelChange(new Element(agentId, id), monitor.Key, Object.Host?.Dms, AlarmLevel));
            }
        }

        private void NotifyNameChanged(string name)
        {
            foreach (var monitor in nameMonitors.ToList())
            {
                monitor.Value(new ElementNameChange(new Element(agentId, id), monitor.Key, Object.Host?.Dms, name));
            }
        }

        private void NotifyStateChanged()
        {
            foreach (var monitor in stateMonitors.ToList())
            {
                monitor.Value(new ElementStateChange(new Element(agentId, id), monitor.Key, Object.Host?.Dms, State));
            }
        }

        private static void ValidateName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("The element name cannot be empty or white space.", nameof(name));
            }

            if (name.Length > 200)
            {
                throw new ArgumentException("The element name cannot exceed 200 characters.", nameof(name));
            }

            if (name[0] == '.' || name[name.Length - 1] == '.' || name[0] == ' ' || name[name.Length - 1] == ' ')
            {
                throw new ArgumentException("The element name cannot start or end with a dot or space.", nameof(name));
            }

            if (name.IndexOfAny(new[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|', '°', ';' }) >= 0)
            {
                throw new ArgumentException("The element name contains a forbidden character.", nameof(name));
            }

            if (name.IndexOf('%') != name.LastIndexOf('%'))
            {
                throw new ArgumentException("The element name cannot contain more than one percentage character.", nameof(name));
            }
        }

        private void EnsureElementExists(int agentId, int id)
        {
            if (cache.GetElement(agentId, id) == null || state == ElementState.Deleted)
            {
                throw new ElementNotFoundException(agentId, id);
            }
        }
        private void EnsureStateChangeIsSupported()
        {
            if (Object.DveSettings.IsChild || Object.RedundancySettings.IsDerived)
            {
                throw new NotSupportedException( "The operation is not supported on a DVE child or derived element.");
            }
        }

        private void SetupStandaloneParameters()
        {
            Setup(e => e.GetStandaloneParameter<It.IsAnyType>(It.IsAny<int>()))
                .Returns(new InvocationFunc(invocation =>
                {
                    var parameterType = invocation.Method.GetGenericArguments()[0];
                    var parameterId = (int)invocation.Arguments[0];

                    return GetStandaloneParameterObject(parameterType, parameterId);
                }));
        }
        private void EnsureAlarmInformationIsAvailable(int agentId, int id)
        {
            EnsureElementExists(agentId, id);
        }
        private void SetupTables()
        {
            Setup(e => e.GetTable(It.IsAny<int>()))
                .Returns((int tableId) => GetTableObject(tableId));
        }

        private object GetStandaloneParameterObject(Type parameterType, int parameterId)
        {
            SupportedValueTypes.EnsureSupported(parameterType);

            var cacheKey = $"{parameterId}|{parameterType.AssemblyQualifiedName}";

            if (!standaloneParameterMocks.TryGetValue(cacheKey, out var parameterMock))
            {
                var parameterModel = parametersAndTables.GetParameter(parameterId);
                var parameterMockType = typeof(DmsStandaloneParameterMock<>).MakeGenericType(parameterType);

                parameterMock = (Mock)Activator.CreateInstance(parameterMockType, parameterModel, Object);
                standaloneParameterMocks.Add(cacheKey, parameterMock);
            }

            return parameterMock.Object;
        }

        private IDmsTable GetTableObject(int tableId)
        {
            if (!tableMocks.TryGetValue(tableId, out var tableMock))
            {
                var tableModel = parametersAndTables.GetTable(tableId);

                tableMock = new DmsTableMock(tableModel, Object);
                tableMocks.Add(tableId, tableMock);
            }

            return tableMock.Object;
        }

        private static IElementConnectionCollection CreateEmptyConnectionCollection()
        {
            var connectionsMock = new Mock<IElementConnectionCollection>();

            connectionsMock.Setup(connections => connections.Length).Returns(0);
            connectionsMock.Setup(connections => connections.Enumerator).Returns(new List<IElementConnection>());
            connectionsMock.Setup(connections => connections.GetEnumerator()).Returns(() => new List<IElementConnection>().GetEnumerator());
            connectionsMock.Setup(connections => connections.IsUpdateRequired()).Returns(false);

            return connectionsMock.Object;
        }
        private static IPropertyCollection<TProperty, TPropertyDefinition> CreateEmptyPropertyCollection<TProperty, TPropertyDefinition>()

            where TProperty : IDmsProperty<TPropertyDefinition>

            where TPropertyDefinition : IDmsPropertyDefinition
        {
            var propertiesMock = new Mock<IPropertyCollection<TProperty, TPropertyDefinition>>();

            propertiesMock.Setup(properties => properties.Count).Returns(0);
            propertiesMock.Setup(properties => properties.GetEnumerator()).Returns(() => new List<TProperty>().GetEnumerator());

            return propertiesMock.Object;
        }
    }
}
