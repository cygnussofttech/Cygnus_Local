using System;

namespace GreenLine.Security
{
    /// <summary>
    /// Private members have short names to preserve space using json serialization
    /// </summary>
    public class IdentityRepresentation
    {
        private bool ia;

        public bool IsAuthenticated
        {
            get { return ia; }
            set { ia = value; }
        }

        private string n;

        public string Name
        {
            get { return n; }
            set { n = value; }
        }

        private string r;
        public string Roles
        {
            get { return r; }
            set { r = value; }
        }

        private string locationCode;
        public string LocationCode
        {
            get { return locationCode; }
            set { locationCode = value; }
        }

        private string mainLocCode;
        public string MainLocCode
        {
            get { return mainLocCode; }
            set { mainLocCode = value; }
        }

        private string locationName;
        public string LocationName
        {
            get { return locationName; }
            set { locationName = value; }
        }

        private Nullable<decimal> locationLevel;
        public Nullable<decimal> LocationLevel
        {
            get { return locationLevel; }
            set { locationLevel = value; }
        }

        private string finYear;
        public string FinYear
        {
            get { return finYear; }
            set { finYear = value; }
        }

        private string yearVal;
        public string YearVal   
        {
            get { return yearVal; }
            set { yearVal = value; }
        }

        private string headOfficeCode;
        public string HeadOfficeCode
        {
            get { return headOfficeCode; }
            set { headOfficeCode = value; }
        }

        private string companyCode;
        public string CompanyCode
        {
            get { return companyCode; }
            set { companyCode = value; }
        }

        private string companyName;
        public string CompanyName
        {
            get { return companyName; }
            set { companyName = value; }
        }

        private string locationGroup;
        public string LocationGroup
        {
            get { return locationGroup; }
            set { locationGroup = value; }
        }

        private string UserImage;
        public string User_Image
        {
            get { return UserImage; }
            set { UserImage = value; }
        }

        private string CurrfinYear;
        public string CurrFinYear
        {
            get { return CurrfinYear; }
            set { CurrfinYear = value; }
        }

        private string CurryearVal;
        public string CurrYearVal
        {
            get { return CurryearVal; }
            set { CurryearVal = value; }
        }

        private string Usertype;
        public string UserType
        {
            get { return Usertype; }
            set { Usertype = value; }
        }

        private string Userreadwrite;
        public string UserReadWrite
        {
            get { return Userreadwrite; }
            set { Userreadwrite = value; }
        }

        private string Emptype;
        public string EmpType
        {
            get { return Emptype; }
            set { Emptype = value; }
        }

        private string gSTNToken;
        public string GSTNToken
        {
            get { return gSTNToken; }
            set { gSTNToken = value; }
        }

        private string _DomainName;
        public string DomainName
        {
            get { return _DomainName; }
            set { _DomainName = value; }
        }

        private string _Currency;
        public string Currency
        {
            get { return _Currency; }
            set { _Currency = value; }
        }
        private string _CurrencyName;
        public string CurrencyName
        {
            get { return _CurrencyName; }
            set { _CurrencyName = value; }
        }
    }
}