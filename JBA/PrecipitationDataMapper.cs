using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JBA
{
    public class PrecipitationDataMapper : ClassMap<PrecipitationData>
    {
        public PrecipitationDataMapper()
        {
            AutoMap(CultureInfo.InvariantCulture);
            Map(m => m.Date).TypeConverterOption.Format("d/M/yyyy");
        }
    }
}
