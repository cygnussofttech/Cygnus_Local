using System;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

/// <summary>
/// AzureStorage Helper
/// </summary>
public class AzureStorageHelper
{
    public AzureStorageHelper()
    {
        // TODO: Add constructor logic here
    }
    /// <summary>
    /// Get Azure ContainerName
    /// </summary>
    public static string AzureContainerName
    {
        get { return ConfigurationManager.AppSettings["SOPContainerName"]; }
        //get { return SessionUtilities.Client.ToLower(); }
        //get { return "rcpllive"; }
    }

    /// <summary>
    /// Get Deployment Type
    /// </summary>
    private static string DeploymentType
    {
        get { return ConfigurationManager.AppSettings["DeploymentType"]; }
    }

    /// <summary>
    /// Get Azure ConnectionString From Configuration Manager
    /// </summary>
    public static string AzureConnectionString
    {
        get { return ConfigurationManager.ConnectionStrings[DeploymentType + "StorageConnectionString"].ConnectionString; }
    }

    /// <summary>
    /// Get Blob Container Detail
    /// </summary>
    /// <param name="containerName">ContainerName</param>
    /// <returns>CloudBlobContainer</returns>
    public static CloudBlobContainer GetBlobContainer(string containerName)
    {
        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(AzureConnectionString);
        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

        return blobClient.GetContainerReference(containerName);
    }

    /// <summary>
    /// Check Specific Blob Exists or not
    /// </summary>
    /// <param name="blob">CloudBlockBlob</param>
    /// <returns>bool</returns>
    public static bool IsBlobExists(CloudBlockBlob blob)
    {
        try
        {
            blob.FetchAttributes();
            return true;
        }
        catch (StorageClientException e)
        {
            if (e.ErrorCode != StorageErrorCode.ResourceNotFound) throw;
            return false;
        }
    }

    /// <summary>
    /// Check Specific Blob Exists or not
    /// </summary>
    /// <param name="ContainerName">ContainerName</param>
    /// <param name="BlobUri">BlobUri</param>
    /// <returns>bool</returns>
    public static bool IsBlobExists(string ContainerName, string BlobUri)
    {
        try
        {
            CloudBlobContainer container = GetBlobContainer(AzureContainerName);
            CloudBlob blockBlob = container.GetBlobReference(BlobUri);
            blockBlob.FetchAttributes();
            return true;
        }
        catch (StorageClientException e)
        {
            if (e.ErrorCode != StorageErrorCode.ResourceNotFound) throw;
            return false;
        }
    }

    /// <summary>
    /// Delete Specific Blob From Storage
    /// </summary>
    /// <param name="ContainerName">ContainerName</param>
    /// <param name="BlobUri">BlobUri</param>
    public static void DeleteBlob(string ContainerName, string BlobUri, string UserID)
    {
        try
        {
            CloudBlobContainer container = GetBlobContainer(AzureContainerName);
            CloudBlob blockBlob = container.GetBlobReference(BlobUri);
            blockBlob.DeleteIfExists();
        }
        catch (StorageClientException e)
        {
            ExceptionUtility.LogException(e, "DeleteBlob",   UserID, "AzureStorageHelper");
            throw;
        }
    }

    /// <summary>
    /// Create Client's Azure Container if not Exists
    /// </summary>
    public static void CreateContainer( string UserID)
    {
        try
        {
            var container = GetBlobContainer(AzureContainerName);
            container.CreateIfNotExist();
        }
        catch (Exception e)
        {
            ExceptionUtility.LogException(e, "CreateContainer",  UserID, "AzureStorageHelper");
        }
    }

    /// <summary>
    /// Upload Blob item To Storage
    /// </summary>
    /// <param name="DocumentType">v</param>
    /// <param name="upLoadFile">upLoadFile</param>
    /// <param name="fileName">fileName</param>
    /// <returns>string(Uploaded BlobUri)</returns>
    public static string UploadBlobFile(string DocumentType, HttpPostedFileBase upLoadFile, string fileName, string UserID)
    {
        DeleteOldFiles(DocumentType, UserID);
        string sopFileLocation = string.Empty;
        try
        {
            sopFileLocation = DocumentType + "/" + System.DateTime.Now.ToString("yyyy/MMM") + "/" + fileName;

            if (!string.IsNullOrEmpty(upLoadFile.FileName))
            {
                if (IsBlobExists(AzureContainerName, sopFileLocation))
                    DeleteBlob(AzureContainerName, sopFileLocation, UserID);
                CloudBlobContainer container = GetBlobContainer(AzureContainerName);
                CloudBlob blockBlob = container.GetBlobReference(sopFileLocation);

                string extn, name;
                Match m = Regex.Match(upLoadFile.FileName, @"(?'Name'[^\\]+)\.(?'Ext'.*)");
                extn = m.Groups["Ext"].Value;
                name = m.Groups["Name"].Value;
                string path = HttpContext.Current.Server.MapPath("~/UploadedFiles/" + DocumentType + "/" + name + "." + extn);
                upLoadFile.SaveAs(path);
                blockBlob.Properties.ContentType = IOHelper.GetContentType(Path.GetExtension(upLoadFile.FileName));
                blockBlob.UploadFile(path);
            }
        }
        catch (Exception ex)
        {
            ExceptionUtility.LogException(ex, "UploadBlob - " + fileName, UserID, "AzureStorageHelper");
            sopFileLocation = string.Empty;
        }
        return sopFileLocation;
    }

    /// <summary>
    /// Upload Blob item To Storage
    /// </summary>
    /// <param name="DocumentType">v</param>
    /// <param name="upLoadFile">upLoadFile</param>
    /// <param name="fileName">fileName</param>
    /// <returns>string(Uploaded BlobUri)</returns>
    public static string UploadBlob(string DocumentType, FileUpload upLoadFile, string fileName,string UserID)
    {
        DeleteOldFiles(DocumentType, UserID);
        string sopFileLocation = string.Empty;
        try
        {
            sopFileLocation = DocumentType + "/" + System.DateTime.Now.ToString("yyyy/MMM") + "/" + fileName;

            if (!string.IsNullOrEmpty(upLoadFile.PostedFile.FileName))
            {
                if (IsBlobExists(AzureContainerName, sopFileLocation))
                    DeleteBlob(AzureContainerName, sopFileLocation, UserID);
                CloudBlobContainer container = GetBlobContainer(AzureContainerName);
                CloudBlob blockBlob = container.GetBlobReference(sopFileLocation);

                string extn, name;
                Match m = Regex.Match(upLoadFile.PostedFile.FileName, @"(?'Name'[^\\]+)\.(?'Ext'.*)");
                extn = m.Groups["Ext"].Value;
                name = m.Groups["Name"].Value;
                string path = HttpContext.Current.Server.MapPath("~/UploadedFiles/" + DocumentType + "/" + name + "." + extn);
                upLoadFile.PostedFile.SaveAs(path);
                blockBlob.Properties.ContentType = IOHelper.GetContentType(Path.GetExtension(upLoadFile.PostedFile.FileName));
                blockBlob.UploadFile(path);
            }
        }
        catch (Exception ex)
        {
            ExceptionUtility.LogException(ex, "UploadBlob - " + fileName, UserID, "AzureStorageHelper");
            sopFileLocation = string.Empty;
        }
        return sopFileLocation;
    }

    /// <summary>
    /// Download Specific Blob item From Storage at Specific Path
    /// </summary>
    /// <param name="BlobUri">BlobUri</param>
    /// <param name="FileLocation">FileLocation</param>
    public static void DownloadBlob(string BlobUri, string FileLocation)
    {
        CloudBlobContainer container = GetBlobContainer(AzureContainerName);
        CloudBlockBlob blockBlob = container.GetBlockBlobReference(container.Uri + "/" + BlobUri);
        if (IsBlobExists(blockBlob))
        {
            blockBlob.DownloadToFile(FileLocation);
        }
    }

    /// <summary>
    /// Download Specific Blob item in MemoryStream
    /// </summary>
    /// <param name="BlobUri">BlobUri</param>
    /// <param name="response">response</param>
    public static void DownloadBlob(string BlobUri, HttpResponse response)
    {
        CloudBlobContainer container = GetBlobContainer(AzureContainerName);
        CloudBlockBlob blockBlob = container.GetBlockBlobReference(container.Uri + "/" + BlobUri);
        {
            using (var memStream = new MemoryStream())
            {
                blockBlob.DownloadToStream(memStream);
                response.Clear();
                response.ClearContent();
                response.ClearHeaders();
                response.Buffer = true;
                response.Expires = -1;
                response.ContentType = blockBlob.Properties.ContentType;
                response.AddHeader("Content-Disposition", "Attachment; filename=" + blockBlob.Name);
                response.AddHeader("Content-Length", blockBlob.Properties.Length.ToString());
                response.BinaryWrite(memStream.ToArray());
                response.Flush();
                response.Close();
                response.End();

                //blockBlob.DownloadToStream(memStream);
                //response.ContentType = blockBlob.Properties.ContentType;
                //response.AddHeader("Content-Disposition", "Attachment; filename=" + blockBlob.Name);
                //response.AddHeader("Content-Length", blockBlob.Properties.Length.ToString());
                //response.BinaryWrite(memStream.ToArray());
                //response.Flush();
            }
        }
    }

    /// <summary>
    /// Delete Old Files Less than 1 hour From Download Directory of Specific Document Type
    /// </summary>
    /// <param name="DocumentType">DocumentType</param>
    private static void DeleteOldFiles(string DocumentType, string UserID)
    {
        try
        {
            string BasePath = HttpContext.Current.Server.MapPath("~/UploadedFiles");
            if (Directory.Exists(BasePath + @"\" + DocumentType))
            {
                DirectoryInfo dir = new DirectoryInfo(BasePath + @"\" + DocumentType);
                FileInfo[] fiList = dir.GetFiles();
                foreach (FileInfo fi in fiList)
                {
                    if (fi.CreationTime <= System.DateTime.Now.AddHours(-1))
                        fi.Delete();
                }

                DirectoryInfo[] dirList = dir.GetDirectories();
                foreach (DirectoryInfo di in dirList)
                {
                    if (di.CreationTime <= System.DateTime.Now.AddHours(-1))
                        di.Delete(true);
                }
            }
            else
                Directory.CreateDirectory(BasePath + @"\" + DocumentType);
        }
        catch (Exception e)
        {
            ExceptionUtility.LogException(e, "DeleteOldFiles", UserID, "AzureStorageHelper");
            throw;
        }
    }

    public static string UploadBlobFileForUser(HttpPostedFileBase upLoadFile, string fileName, string UserID,string type)
    {
        string sopFileLocation = string.Empty;
        try
        {
            sopFileLocation = fileName;

            if (!string.IsNullOrEmpty(upLoadFile.FileName))
            {
                //if (IsBlobExists(AzureContainerName, sopFileLocation))
                //    DeleteBlob(AzureContainerName, sopFileLocation, UserID);
                //CloudBlobContainer container = GetBlobContainer(AzureContainerName);
                //CloudBlob blockBlob = container.GetBlobReference(sopFileLocation);

                string extn, name;
                Match m = Regex.Match(upLoadFile.FileName, @"(?'Name'[^\\]+)\.(?'Ext'.*)");
                extn = m.Groups["Ext"].Value;
                name = m.Groups["Name"].Value;
                if(type=="user")
                {
                    string path = HttpContext.Current.Server.MapPath("~/Images/UserImage/" + upLoadFile.FileName);
                    upLoadFile.SaveAs(path);
                    //blockBlob.Properties.ContentType = IOHelper.GetContentType(Path.GetExtension(upLoadFile.FileName));
                    //blockBlob.UploadFile(path);
                }
                else if (type == "userimg")
                {
                    string path = HttpContext.Current.Server.MapPath("~/Images/UserImage/" + fileName);
                    upLoadFile.SaveAs(path);
                }
                else
                {
                    string path = HttpContext.Current.Server.MapPath("~/Images/UplaodContractFile/" + UserID + "." + extn);
                    upLoadFile.SaveAs(path);
                    //blockBlob.Properties.ContentType = IOHelper.GetContentType(Path.GetExtension(upLoadFile.FileName));
                    //blockBlob.UploadFile(path);
                }
                
            }
        }
        catch (Exception ex)
        {
            ExceptionUtility.LogException(ex, "UploadBlob - " + fileName, UserID, "AzureStorageHelper");
            sopFileLocation = string.Empty;
        }
        return sopFileLocation;
    }

    public static string UploadBlobFileForSOP(HttpPostedFileBase upLoadFile, string fileName)
    {
        string sopFileLocation = string.Empty;
        try
        {
            sopFileLocation = fileName;

            if (!string.IsNullOrEmpty(upLoadFile.FileName))
            {
                string extn, name;
                Match m = Regex.Match(upLoadFile.FileName, @"(?'Name'[^\\]+)\.(?'Ext'.*)");
                extn = m.Groups["Ext"].Value;
                name = m.Groups["Name"].Value;

                string FolderPath = HttpContext.Current.Server.MapPath("~/UploadedDocuments/UplaodSOPFile/");

                // If directory does not exist, create it
                if (!Directory.Exists(FolderPath))
                {
                    Directory.CreateDirectory(FolderPath);
                }
                string path = HttpContext.Current.Server.MapPath("~/UploadedDocuments/UplaodSOPFile/" + fileName);
                upLoadFile.SaveAs(path);
            }
        }
        catch (Exception ex)
        {
            ExceptionUtility.LogException(ex, "UploadBlob - " + fileName, "AzureStorageHelper");
            sopFileLocation = string.Empty;
        }
        return sopFileLocation;
    }
    public static string UploadBlobFileFordeps(HttpPostedFileBase upLoadFile, string fileName)
    {
        string DepsFileLocation = string.Empty;
        try
        {
            DepsFileLocation = fileName;

            if (!string.IsNullOrEmpty(upLoadFile.FileName))
            {
                string extn, name;
                Match m = Regex.Match(upLoadFile.FileName, @"(?'Name'[^\\]+)\.(?'Ext'.*)");
                extn = m.Groups["Ext"].Value;
                name = m.Groups["Name"].Value;

                string FolderPath = HttpContext.Current.Server.MapPath("~/UploadedDocuments/UplaodDepsFile/");

                // If directory does not exist, create it
                if (!Directory.Exists(FolderPath))
                {
                    Directory.CreateDirectory(FolderPath);
                }
                string path = HttpContext.Current.Server.MapPath("~/UploadedDocuments/UplaodDepsFile/" + fileName);
                upLoadFile.SaveAs(path);
            }
        }
        catch (Exception ex)
        {
            ExceptionUtility.LogException(ex, "UploadBlob - " + fileName, "AzureStorageHelper");
            DepsFileLocation = string.Empty;
        }
        return DepsFileLocation;
    }

    public static string UploadBlobFileForims(HttpPostedFileBase upLoadFile, string fileName)
    {
        string ImsFileLocation = string.Empty;
        try
        {
            ImsFileLocation = fileName;

            if (!string.IsNullOrEmpty(upLoadFile.FileName))
            {
                string extn, name;
                Match m = Regex.Match(upLoadFile.FileName, @"(?'Name'[^\\]+)\.(?'Ext'.*)");
                extn = m.Groups["Ext"].Value;
                name = m.Groups["Name"].Value;

                string FolderPath = HttpContext.Current.Server.MapPath("~/UploadedDocuments/UplaodIMSFile/");

                // If directory does not exist, create it
                if (!Directory.Exists(FolderPath))
                {
                    Directory.CreateDirectory(FolderPath);
                }
                string path = HttpContext.Current.Server.MapPath("~/UploadedDocuments/UplaodIMSFile/" + fileName);
                upLoadFile.SaveAs(path);
            }
        }
        catch (Exception ex)
        {
            ExceptionUtility.LogException(ex, "UploadBlob - " + fileName, "AzureStorageHelper");
            ImsFileLocation = string.Empty;
        }
        return ImsFileLocation;
    }
}
