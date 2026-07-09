using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace GreenLineDataService.Models
{
    [XmlRoot("Cygnus_HRMS_Leave_LeaveGroup"), XmlType("Cygnus_HRMS_Leave_LeaveGroup")]
    public class Cygnus_HRMS_Leave_LeaveGroup
    {

        #region Constructor
        public Cygnus_HRMS_Leave_LeaveGroup()
        {
        }
        #endregion

        #region Private Members

        #endregion

        #region Public Members

        public int LeaveGroupId { get; set; }

        public string LeaveGroupName { get; set; }

        public string Description { get; set; }

        public string CompanyCode { get; set; }

        public string LocationCode { get; set; }

        #endregion

        #region Common Members

        public string Action { get; set; }

        public bool IsActive { get; set; }

        public string EntryBy { get; set; }

        public DateTime EntryDate { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime UpdatedDate { get; set; }

        #endregion

        #region Extra Members out of Table

        #endregion

    }

}