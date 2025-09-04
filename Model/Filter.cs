using System.Linq.Expressions;
using System.Reflection;

namespace Applet.Nat.Ux.Models
{
    public class Filter
    {
        public List<FilterItem> coFilterItems { get; set; }
    }
    public class FilterItem
    {
        public string ivstrPropName { get; set; }
        public string ivstrPropValue { get; set; }
        public string ivstrOper { get; set; }
    }
}