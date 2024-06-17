using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiai_Helper.Models
{
    public class Tournament : ReactiveObject
    {
        private string name;
        public string Name 
        {
            get => name; 
            set => this.RaiseAndSetIfChanged(ref name, value); 
        }

        public IDictionary<int, Category> Categories { get; } = new Dictionary<int, Category>();

        public IDictionary<int, Competitor> Competitors { get; } = new Dictionary<int, Competitor>();
    }
}
