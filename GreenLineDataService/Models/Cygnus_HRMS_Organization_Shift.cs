using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace GreenLineDataService.Models
{
    [XmlRoot("Cygnus_HRMS_Organization_Shift"), XmlType("Cygnus_HRMS_Organization_Shift")]
    public class Cygnus_HRMS_Organization_Shift
    {

        #region Constructor
        public Cygnus_HRMS_Organization_Shift()
        {
        }
        #endregion

        #region Private Members

        #endregion

        #region Public Members

        public int ShiftId { get; set; }

        public string ShiftCode { get; set; }

        public string CompanyCode { get; set; }

        public string LocationCode { get; set; }

        public string ShiftName { get; set; }

        public string Description { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime UpdatedDate { get; set; }

        #endregion

        #region Common Members

        public string Action { get; set; }

        public bool IsActive { get; set; }

        public string EntryBy { get; set; }

        public DateTime EntryDate { get; set; }

        public string UpdateBy { get; set; }

        public DateTime UpdateDate { get; set; }

        #endregion

        #region Extra Members out of Table

        #region Drop Down Members

        public string id { get; set; }
        public string text { get; set; }

        #endregion

        #endregion

    }
}