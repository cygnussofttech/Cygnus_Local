using System;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;

// Utility for exceptions
// Author : Nilesh Makavana
// Date : 2014-01-22
public sealed class ExceptionUtility
{

    #region LogException
    /// <summary>
    /// Log an Exception
    /// </summary>
    /// <param name="exc">Exception</param>
    /// <param name="source">Exception source</param>
    public static string LogException(Exception exc, string source)
    {
        return Log(exc, source, "", "");
    }
    #endregion

    #region LogException
    /// <summary>
    /// Log an Exception
    /// </summary>
    /// <param name="exc">Exception</param>
    /// <param name="source">Exception source</param>
    public static string LogException(Exception exc, string source, string UserID)
    {
        return Log(exc, source, UserID, "");
    }
    #endregion

    #region LogException
    /// <summary>
    /// Log an Exception
    /// </summary>
    /// <param name="exc">Exception</param>
    /// <param name="source">Exception source</param>
    /// <param name="exc">Only Page Name</param>
    public static string LogException(Exception exc, string source, string UserID, string PageName)
    {
        return Log(exc, source, UserID, PageName);
    }
    #endregion

    private static string Log(Exception exc, string source, string UserID, string PageName)
    {
        logConnectionTimeOut(exc, source, PageName);

        // Include enterprise logic for logging exceptions
        // Get the absolute path to the log file

        // Open the log file for append and write the log
        //StreamWriter SW = new StreamWriter(logFile, true);
        string sql = "INSERT INTO [CYGNUS_error]([CreatedDate],[ExceptionType],[Exception],[ExceptionSource],[StackTrace],[InnerExceptionType],[InnerException],[InnerSource],[InnerStackTrace],[UserID]) \n";
        sql += " OUTPUT INSERTED.SrNo  VALUES (GETDATE(),@ExceptionType,@Exception,@ExceptionSource,@StackTrace,@InnerExceptionType, @InnerException, @InnerSource, @InnerStackTrace,@UserID)";

        string mExceptionType = "", mException = "", mExceptionSource = "", mStackTrace = "", mInnerExceptionType = "", mInnerException = "", mInnerSource = "", mInnerStackTrace = "";
        if (exc.InnerException != null)
        {
            mInnerExceptionType = exc.InnerException.GetType().ToString();
            mInnerException = exc.InnerException.Message;
            mInnerSource = exc.InnerException.Source;
            if (exc.InnerException.StackTrace != null)
            {
                mInnerStackTrace = exc.InnerException.StackTrace;
            }
        }

        mExceptionType = exc.GetType().ToString();
        mException = exc.Message;
        mExceptionSource = source;
        if (exc.StackTrace != null)
        {
            mStackTrace = exc.StackTrace;
        }
        String InsertID = Convert.ToString(SqlHelper.ExecuteScalar(System.Configuration.ConfigurationManager.AppSettings["dbConnection"].ToString().Trim()
                                , System.Data.CommandType.Text,
                                sql,
                                new SqlParameter("@ExceptionType", mExceptionType),
                                new SqlParameter("@Exception", mException),
                                new SqlParameter("@ExceptionSource", mExceptionSource),
                                new SqlParameter("@StackTrace", mStackTrace),
                                new SqlParameter("@InnerExceptionType", mInnerExceptionType),
                                new SqlParameter("@InnerException", mInnerException),
                                new SqlParameter("@InnerSource", mInnerSource),
                                new SqlParameter("@InnerStackTrace", mInnerStackTrace),
                                new SqlParameter("@UserID", UserID)));

        return InsertID;
    }

    #region Check connection time out exception
    public static bool logConnectionTimeOut(Exception ex, string PageSource, string PageURL)
    {
        if (isConnectionTimeOut(ex))
        {
            SqlHelper.ExecuteScalar(System.Configuration.ConfigurationManager.AppSettings["dbConnection"].ToString().Trim()
                                    , System.Data.CommandType.Text
                                    , "INSERT INTO CYGNUS_error_connectiontimeout(PageSource,PageUrl) VALUES (@PageSource,@PageUrl)"
                                    , new SqlParameter("@PageSource", PageSource.Trim())
                                    , new SqlParameter("@PageUrl", PageURL.Trim()));
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion

    #region Check connection time out exception
    public static bool isConnectionTimeOut(Exception ex)
    {
        var sqlException = ex as SqlException;
        if (sqlException == null)
        {
            sqlException = ex.InnerException as SqlException;
        }
        if (sqlException != null)
        {
            if (sqlException.Number == -2)            
               return true;            
            else            
                return false;            
        }
        else
        {
            return false;
        }
    }
    #endregion

    #region NotifySystemOps
    /// <summary>
    ///  Notify System Operators about an exception
    /// </summary>
    /// <param name="exc"></param>
    public static void NotifySystemOps(Exception exc)
    {
        // Include code for notifying IT system operators
    }
    #endregion
}
