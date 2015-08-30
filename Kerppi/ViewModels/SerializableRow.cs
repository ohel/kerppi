using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Kerppi.ViewModels
{
    public class SerializableRow : INotifyPropertyChanged
    {
        private decimal _price = 0.0m;
        private decimal _amount = 0.0m;
        private decimal _total = 0.0m;
        public string Code { get; set; }
        public string Description { get; set; }
        public string Price { get { return _price.ToString(); } set {
            _price = StringToDecimal(value);
            _total = _price * _amount;
            NotifyPropertyChanged(() => Price);
            NotifyPropertyChanged(() => Total);
        } }
        public string Amount { get { return _amount.ToString(); } set {
            _amount = StringToDecimal(value);
            _total = _price * _amount;
            NotifyPropertyChanged(() => Amount);
            NotifyPropertyChanged(() => Total);
        } }
        public string Total { get { return _total.ToString(); } }

        public SerializableRow()
        {
            Code = "";
            Description = "";
        }

        public SerializableRow(string code = "", string description = "", string price = "", string amount = "")
        {
            Code = code;
            Description = description;
            Price = price;
            Amount = amount;
        }

        private decimal StringToDecimal(string val)
        {
            decimal parsed = 0.0m;
            if (Decimal.TryParse(val, out parsed))
                return parsed;
            else
                return 0.0m;
        }

        public bool ShouldSerializeTotal()
        {
            return false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged<T>(System.Linq.Expressions.Expression<Func<T>> property)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(ViewModels.PropertyHelper.GetPropertyName(property)));
        }
    }
}
