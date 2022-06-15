using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AssembleIVM.T_reduct {
    class Number {
        bool isInt;
        public dynamic value;

        public Number(string stringValue) {
            if(int.TryParse(stringValue, out _)) {
                isInt = true;
                value = int.Parse(stringValue);
            } else {
                isInt = false;
                value = decimal.Parse(stringValue, CultureInfo.InvariantCulture);
            }
        }

        public Number(dynamic value) {
            this.value = value;
            if (value is int) {
                isInt = true;
            } else {
                isInt = false;
            }
        }
    }
}
