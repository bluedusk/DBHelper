using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace DBH.Config
{
    public class DBHelperConfiguraionSectionHandler : IConfigurationSectionHandler
    {

        public DBHelperConfiguraionSectionHandler()
        { }
        #region IConfigurationSectionHandler 成员

        public object Create(object parent, object configContext, System.Xml.XmlNode section)
        {
            return section ;
        }

        #endregion
    }
}
