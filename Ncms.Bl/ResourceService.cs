using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using Ncms.Contracts;

namespace Ncms.Bl
{
    public class ResourceService : IResourceService
    {
        private readonly Assembly _resourceAssembly;

        public ResourceService(Assembly resourceAssembly)
        {
            _resourceAssembly = resourceAssembly;
        }

        public Dictionary<string, Dictionary<string, string>> List()
        {
            var dictionary = new Dictionary<string, Dictionary<string, string>>();

            foreach (var name in _resourceAssembly.GetManifestResourceNames())
            {
                var stream = _resourceAssembly.GetManifestResourceStream(name);
                var resourceReader = new ResourceReader(stream);

                var resources = resourceReader.OfType<DictionaryEntry>();
                var innerValues = resources.ToDictionary(r => Convert.ToString(r.Key), r => Convert.ToString(r.Value));

                dictionary.Add(name, innerValues);
            }

            return dictionary;
        }
    }
}
