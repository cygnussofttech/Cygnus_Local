using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;


public class ReportServiceUtilities
{
    public static Uri ReportServerURL
    {
        get { return new Uri(ConfigurationManager.AppSettings["ReportServerURL"]); }
    }

    public static string ReportPathPrefix
    {
        get { return ConfigurationManager.AppSettings["ReportPathPrefix"]; }
    }

    public static string ReportServerUser
    {
        get { return ConfigurationManager.AppSettings["ReportServerUser"]; }
    }

    public static string ReportServerPass
    {
        get { return ConfigurationManager.AppSettings["ReportServerPass"]; }
    }

    public static ReportCredentials BaseGetReportCredentials
    {
        get { return new ReportCredentials(ConfigurationManager.AppSettings["ReportServerUser"].ToString(), ConfigurationManager.AppSettings["ReportServerPass"].ToString(), ConfigurationManager.AppSettings["ReportServerURL"].ToString()); }
    }
    public static NetworkCredential ReportNetworkCredential
    {
        get { return new NetworkCredential(ConfigurationManager.AppSettings["ReportServerUser"].ToString(), ConfigurationManager.AppSettings["ReportServerPass"].ToString(), ConfigurationManager.AppSettings["ReportServerURL"].ToString()); }
    }
}

public class ReportCredentials : Microsoft.Reporting.WebForms.IReportServerCredentials
{
    string _userName, _password, _domain;

    public ReportCredentials(string userName, string password, string domain)
    {
        _userName = userName;
        _password = password;
        _domain = domain;
    }

    public System.Security.Principal.WindowsIdentity ImpersonationUser
    {
        get { return null; }
    }

    public System.Net.ICredentials NetworkCredentials
    {
        get
        { return new System.Net.NetworkCredential(_userName, _password, _domain); }
    }

    public bool GetFormsCredentials(out System.Net.Cookie authCoki, out string userName, out string password, out string authority)
    {
        userName = _userName;
        password = _password;
        authority = _domain;

        authCoki = new System.Net.Cookie(".ASPXAUTH", ".ASPXAUTH", "/", "Domain");
        return true;
    }
}