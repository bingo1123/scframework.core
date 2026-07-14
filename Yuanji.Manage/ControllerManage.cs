using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YS.Yuanji.Drive;

namespace Yuanji.Manage
{
    public class ControllerManage
    {
        private readonly ConcurrentDictionary<string, IDevice> _controllers = new ConcurrentDictionary<string, IDevice>();

        public ControllerManage()
        {
            
        }

        public void AddController(string name, IDevice controller)
        {
            if(!_controllers.ContainsKey(name))
            _controllers.TryAdd(name, controller);
        }

        public IDevice GetController(string name)
        {
            _controllers.TryGetValue(name, out var controller);
            return controller;
        }

        public IEnumerable<string> GetAllControllerNames()
        {
            return _controllers.Keys;
        }

        public IEnumerable<IDevice> GetAllControllers()
        {
            return _controllers.Values;
        }
    }
}
