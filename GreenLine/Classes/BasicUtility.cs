using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLine.Classes
{
    public class BasicUtility
    {
        public const string FMDocumentDirectory = @"FMDocuments";
        public static string GetFMDocketTypeSuffix(string DocTypeValue)
        {
            string strDocTypeSuffix = "";

            switch (DocTypeValue)
            {
                case "1":
                    strDocTypeSuffix = "P";
                    break;
                case "2":
                    strDocTypeSuffix = "B";
                    break;
                case "3":
                    strDocTypeSuffix = "O";
                    break;
                case "4":
                    strDocTypeSuffix = "C";
                    break;
                case "5":
                    strDocTypeSuffix = "PL";
                    break;
            }

            return strDocTypeSuffix;
        }
    
    }
}