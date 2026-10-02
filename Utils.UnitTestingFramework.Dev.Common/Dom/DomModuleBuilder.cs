namespace Skyline.DataMiner.Utils.UnitTestingFramework.Dev.Common.Dom
{
    using System;
    using System.Collections.Generic;
    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Sections;

    /// <summary>
    /// Builder class for constructing a <see cref="DomModule"/> instance.
    /// </summary>
    public class DomModuleBuilder
    {
        private readonly string moduleId;

        private readonly List<Func<DomInstance>> domInstances = new List<Func<DomInstance>>();
        private readonly List<Func<DomDefinition>> domDefinitions = new List<Func<DomDefinition>>();
        private readonly List<Func<SectionDefinition>> sectionDefinitions = new List<Func<SectionDefinition>>();
        private readonly List<Func<DomBehaviorDefinition>> behaviorDefinitions = new List<Func<DomBehaviorDefinition>>();

        internal DomModuleBuilder(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("The DOM module ID must be defined.", nameof(id));
            }

            moduleId = id;
        }

        /// <summary>
        /// Adds a DOM instance to the module.
        /// </summary>
        /// <param name="createInstance">A function to create the DOM instance.</param>
        /// <returns>The current <see cref="DomModuleBuilder"/> instance.</returns>
        public DomModuleBuilder WithDomInstance(Func<DomInstance> createInstance)
        {
            AddDomObject(domInstances, moduleId, createInstance, nameof(createInstance));
            return this;
        }

        /// <summary>
        /// Adds a DOM definition to the module.
        /// </summary>
        /// <param name="createDefinition">A function to create the DOM definition.</param>
        /// <returns>The current <see cref="DomModuleBuilder"/> instance.</returns>
        public DomModuleBuilder WithDomDefinition(Func<DomDefinition> createDefinition)
        {
            AddDomObject(domDefinitions, moduleId, createDefinition, nameof(createDefinition));
            return this;
        }

        /// <summary>
        /// Adds a section definition to the module.
        /// </summary>
        /// <param name="createDefinition">A function to create the section definition.</param>
        /// <returns>The current <see cref="DomModuleBuilder"/> instance.</returns>
        public DomModuleBuilder WithSectionDefinition(Func<SectionDefinition> createDefinition)
        {
            AddDomObject(sectionDefinitions, moduleId, createDefinition, nameof(createDefinition));
            return this;
        }

        /// <summary>
        /// Adds a DOM behavior definition to the module.
        /// </summary>
        /// <param name="createDefinition">A function to create the DOM behavior definition.</param>
        /// <returns>The current <see cref="DomModuleBuilder"/> instance.</returns>
        public DomModuleBuilder WithDomBehaviorDefinition(Func<DomBehaviorDefinition> createDefinition)
        {
            AddDomObject(behaviorDefinitions, moduleId, createDefinition, nameof(createDefinition));
            return this;
        }

        internal DomModule Build()
        {
            if (String.IsNullOrWhiteSpace(moduleId))
            {
                throw new InvalidCastException("The DOM module ID must be defined.");
            }

            var module = new DomModule(moduleId);

            module.SetDefinitions(CreateDomObjects(moduleId, domDefinitions));
            module.SetSectionDefinitions(CreateDomObjects(moduleId, sectionDefinitions));
            module.SetInstances(CreateDomObjects(moduleId, domInstances));
            module.SetBehaviorDefinitions(CreateDomObjects(moduleId, behaviorDefinitions));

            return module;
        }

        private static void AddDomObject<T>(List<Func<T>> objects, string moduleId, Func<T> createObject, string parameterName)
        {
            if (String.IsNullOrWhiteSpace(moduleId))
            {
                throw new ArgumentException("The DOM module ID cannot be empty or white space.", nameof(moduleId));
            }

            if (createObject == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            objects.Add(createObject);
        }

        private static IEnumerable<T> CreateDomObjects<T>(string moduleId, IEnumerable<Func<T>> factories) where T : class
        {
            foreach (var factory in factories)
            {
                var value = factory() ?? throw new InvalidOperationException($"A DOM object factory for module '{moduleId}' returned null.");

                yield return value;
            }
        }
    }
}
