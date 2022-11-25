using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StringFormatter.Core.Models;

namespace StringFormatter.Core.Interfaces
{
    public interface ICacheService
    {
        bool TryAdd(DictionaryKey key, Delegate del);
        
        bool TryGetValue(DictionaryKey key, out Delegate del);

        void Clear();
    }
}