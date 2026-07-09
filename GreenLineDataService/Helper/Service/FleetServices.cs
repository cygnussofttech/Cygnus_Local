using GreenLine.Classes;
using GreenLineDataService.Helper.Interface;
using GreenLineDataService.Models;
using GreenLineDataService.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace GreenLineDataService.Helper
{
    public class FleetServices : IFleetServices
    {
        private string QueryString = string.Empty;
        readonly GeneralFunctions GF = new GeneralFunctions();

        #region Pickup Request
        public List<CYGNUS_PickUp_Request> GetPickUpRequestDetails(string type = "")
        {
            QueryString = "EXEC USP_GetPickUpRequestDetails '" + type + "'";
            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_PickUp_Request> List = DataRowToObject.CreateListFromTable<CYGNUS_PickUp_Request>(Dt);
            return List;
        }

        public List<CYGNUS_CUSTHDR> GetCustomerOnCustomerGroup(string GRPCD)
        {
            QueryString = "EXEC USP_Get_CustomerOn_CustomerGroup '" + GRPCD + "'";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_CUSTHDR> CYGNUS_CUSTHDRList = DataRowToObject.CreateListFromTable<CYGNUS_CUSTHDR>(dataTable);
            return CYGNUS_CUSTHDRList;
        }

        public DataTable AddPickUpRequest(string XML_Main, string Flag, string CompanyCode, string BaseUserName)
        {
            QueryString = "EXEC USP_PickUp_Request_InsertUpdate '" + XML_Main + "' ,'" + Flag + "','" + CompanyCode + "','" + BaseUserName + "'";
            int id = GF.SaveRequestServices(QueryString.Replace("'", "''"), "AddPickUpRequest", "", "");
            DataSet DS = GF.GetDataSetFromSP(QueryString);
            return DS.Tables[0];
        }

        public List<CYGNUS_Trip_Expense_Master> GetNatureofexp()
        {
            //string SQRY = "EXEC usp_fulExp";
            QueryString = "EXEC usp_fulExp_New_Portal";
            DataTable Dt1 = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_Trip_Expense_Master> GeneralList = DataRowToObject.CreateListFromTable<CYGNUS_Trip_Expense_Master>(Dt1);
            return GeneralList;
        }

        public List<CYGNUS_Trip_Expense_Master> GetTripExpenseMasterList()
        {
            QueryString = "EXEC usp_Get_Trip_Expense_Master";
            DataTable Dt1 = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_Trip_Expense_Master> GeneralList = DataRowToObject.CreateListFromTable<CYGNUS_Trip_Expense_Master>(Dt1);
            return GeneralList;
        }

        public List<CYGNUS_Trip_Expense_Master> GetLrExpenseMasterList()
        {
            QueryString = "EXEC usp_Get_Lr_Expense_Master";
            DataTable Dt1 = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_Trip_Expense_Master> GeneralList = DataRowToObject.CreateListFromTable<CYGNUS_Trip_Expense_Master>(Dt1);
            return GeneralList;
        }

        public List<CYGNUS_FLEET_ENROUTE_EXP> Get_TripExp_List(string PRNo)
        {
            QueryString = "EXEC USP_Get_Expense_ListOn_PR '" + PRNo + "' ";
            DataTable Dt1 = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_FLEET_ENROUTE_EXP> List = DataRowToObject.CreateListFromTable<CYGNUS_FLEET_ENROUTE_EXP>(Dt1);
            return List;
        }

        public DataTable SaveTripExpense(string TripNo, string PRNo, string VehicleNo, string ExpenseCode, decimal Amount, string PaymentType, bool IsBilled, string DocPath, string BaseUserName)
        {
            QueryString = "EXEC USP_Insert_Trip_Expenses" + " '" + TripNo + "'" + ",'" + PRNo + "'" + ",'" + VehicleNo + "'" + ",'" + ExpenseCode + "'" + "," + Amount + ",'" + PaymentType + "'" + "," + (IsBilled ? 1 : 0) + ",'" + DocPath.Replace("'", "''") + "','" + BaseUserName + "'";
            int id = GF.SaveRequestServices(QueryString.Replace("'", "''"), "AddTripExpence", "", "");
            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            return Dt;
        }

        public DataTable SaveDocketExpense(string xmlData, string companyCode, string userName)
        {
            QueryString = "EXEC USP_AddEdit_Docket_Expense '" + xmlData.Replace("'", "''") + "','" + companyCode + "','" + userName + "'";
            GF.SaveRequestServices(QueryString.Replace("'", "''"), "AddDocketExpense", "", "");
            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            return Dt;
        }

        public List<CYGNUS_FLEET_ENROUTE_EXP> GetTripExpenses(string TripNo)
        {
            string SQRY = "EXEC USP_GetTripExpenses '" + TripNo + "'";
            DataTable Dt = GF.GetDataTableFromSP(SQRY);
            return DataRowToObject.CreateListFromTable<CYGNUS_FLEET_ENROUTE_EXP>(Dt);
        }

        public List<CYGNUS_LaneMaster> Get_ContractLane(string ContractId)
        {
            QueryString = "EXEC USP_Get_ContractLane '" + ContractId + "'";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            return DataRowToObject.CreateListFromTable<CYGNUS_LaneMaster>(dataTable);
        }
        #endregion

        #region Vehicle Allocation
        public List<CYGNUS_Vehicle_Allocation> GetVehicle_AllocationDetails()
        {
            QueryString = "EXEC USP_GetPickUpRequestDetails ";
            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_Vehicle_Allocation> List = DataRowToObject.CreateListFromTable<CYGNUS_Vehicle_Allocation>(Dt);
            return List;
        }

        public DataTable AddVehicle_Allocation(string XML_Main)
        {
            QueryString = "EXEC USP_Vehicle_Allocation_Insert '" + XML_Main + "'";
            int id = GF.SaveRequestServices(QueryString.Replace("'", "''"), "AddVehicle_Allocation", "", "");
            DataSet DS = GF.GetDataSetFromSP(QueryString);
            return DS.Tables[0];
        }

        public List<CYGNUS_Vehicle_Master> GetUnallocatedVehicles()
        {
            QueryString = "EXEC usp_Get_Unallocated_Vehicle_List";
            DataTable dt = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_Vehicle_Master> list = DataRowToObject.CreateListFromTable<CYGNUS_Vehicle_Master>(dt);
            return list;
        }
        #endregion

        #region Trip
        public List<CYGNUS_Trip_Master> GetTripList(string BaseUserName)
        {
            QueryString = "EXEC USP_Get_TripList_New'" + BaseUserName + "'";
            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_Trip_Master> List = DataRowToObject.CreateListFromTable<CYGNUS_Trip_Master>(Dt);
            return List;
        }

        public List<Cygnus_Master_Event> GetEventList()
        {
            QueryString = "EXEC USP_Get_EventMaster";
            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            List<Cygnus_Master_Event> List = DataRowToObject.CreateListFromTable<Cygnus_Master_Event>(Dt);
            return List;
        }

        public List<CYGNUS_Master_Vehicle_Event> GetVehicleEventList()
        {
            QueryString = "EXEC USP_GetAll_VehicleEvent";
            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_Master_Vehicle_Event> List = DataRowToObject.CreateListFromTable<CYGNUS_Master_Vehicle_Event>(Dt);
            return List;
        }

        public DataTable UpdateTripEventStatus(string PRNo, string VehicleNo, int Event_Id, string Username, string TripNo, string Remarks)
        {
            QueryString = "EXEC USP_UpdateTripEventStatus '" + PRNo + "','" + VehicleNo + "','" + Event_Id + "','" + Username + "','" + TripNo + "','" + (Remarks ?? "").Replace("'", "''") + "'";
            int id = GF.SaveRequestServices(QueryString.Replace("'", "''"), "UpdateTripEventStatus", "", "");
            DataSet DS = GF.GetDataSetFromSP(QueryString);
            return DS.Tables[0];
        }

        public DataTable UpdateTripEventLastStatus(string PRNo, string VehicleNo, int Event_Id, string Username, string TripNo, int StopId, string Remarks)
        {
            QueryString = "EXEC Usp_updatetripevent_ForLastStatus '" + PRNo + "','" + VehicleNo + "','" + Event_Id + "','" + Username + "','" + TripNo + "','" + StopId + "','" + (Remarks ?? "").Replace("'", "''") + "'";
            int id = GF.SaveRequestServices(QueryString.Replace("'", "''"), "UpdateTripEventLastStatus", "", "");
            DataSet DS = GF.GetDataSetFromSP(QueryString);
            return DS.Tables[0];
        }

        //public DataTable GetInvoiceOCRData(string filePath)
        //{
        //    QueryString = "EXEC USP_Process_Invoice_OCR_Data '" + filePath.Replace("'", "''") + "'";
        //    return GF.GetDataTableFromSP(QueryString);
        //}

        public DataSet SaveLREntry(string xml, string BaseCompanyCode, string BaseUserName, string BaseFinYear)
        {
            //string ewbDate    = m.eway_bill_date.HasValue    ? m.eway_bill_date.Value.ToString("yyyy-MM-dd")    : "";  
            //string ewbValidity = m.eway_bill_validity.HasValue ? m.eway_bill_validity.Value.ToString("yyyy-MM-dd") : "";
            //string invDate    = m.invoice_date.HasValue       ? m.invoice_date.Value.ToString("yyyy-MM-dd")       : "";

            QueryString = "EXEC USP_Insert_LR_Entry '" + xml + "','" + BaseCompanyCode + "','" + BaseUserName + "','" + BaseFinYear + "'";
            int id = GF.SaveRequestServices(QueryString.Replace("'", "''"), "SaveLREntry", "", "");
            DataSet DS = GF.GetDataSetFromSP(QueryString);
            //return DS.Tables.Count > 0 ? DS.Tables[0] : new System.Data.DataTable();
            return DS;
        }

        public List<CYGNUS_TripStageActivity> GetTripStageByPRNo(string PRNo)
        {
            QueryString = "EXEC USP_GetTripStageByPRNo '" + PRNo + "'";
            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_TripStageActivity> List = DataRowToObject.CreateListFromTable<CYGNUS_TripStageActivity>(Dt);
            return List;
        }

        public List<CYGNUS_TripDocket> GetTripLRList(string TripNo, string PRNo, string StopId)
        {
            QueryString = "EXEC USP_GetLRListByTrip '" + (TripNo ?? "") + "','" + (PRNo ?? "") + "','" + (StopId ?? "") + "'";
            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_TripDocket> List = DataRowToObject.CreateListFromTable<CYGNUS_TripDocket>(Dt);
            return List;
        }

        public DataTable SaveDoPodUpload(string xmlData, string companyCode, string userName)
        {
            QueryString = "EXEC USP_AddEdit_DO_POD_Upload '" + xmlData.Replace("'", "''") + "','" + companyCode + "','" + userName + "'";
            int id = GF.SaveRequestServices(QueryString.Replace("'", "''"), "SaveDoPodUpload", "", "");
            DataTable dt = GF.GetDataTableFromSP(QueryString);
            return dt;
        }

        public DataTable GetTripPODCounts(string tripNo, int stopId)
        {
            QueryString = "EXEC USP_GetTripPODCounts '" + tripNo + "'," + stopId;
            return GF.GetDataTableFromSP(QueryString);
        }
        #endregion

        #region  Spot PR Approve/Reject
        public List<CYGNUS_PickUp_Request> GetSpotPR_ForApproval()
        {
            QueryString = "EXEC [USP_GetSpotPR_ForApproval] ";
            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_PickUp_Request> List = DataRowToObject.CreateListFromTable<CYGNUS_PickUp_Request>(Dt);
            return List;
        }
        #endregion

        #region PaymentLink
        public CYGNUS_CUSTHDR GetCustomerDetailsByPRNo(string PRNo)
        {
            QueryString = "EXEC USP_GetCustomerDetailsByPRNo '" + PRNo.Replace("'", "''") + "'";
            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_CUSTHDR> list = DataRowToObject.CreateListFromTable<CYGNUS_CUSTHDR>(Dt);
            return list != null && list.Count > 0 ? list[0] : null;
        }

        public List<CYGNUS_Payment_Links> GetPaymentLinks(string PRNo)
        {
            QueryString = "EXEC USP_GetPaymentLinks '" + PRNo.Replace("'", "''") + "'";
            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            return DataRowToObject.CreateListFromTable<CYGNUS_Payment_Links>(Dt);
        }

        public CYGNUS_Payment_Links CheckActivePaymentLink(string PRNo, decimal Amount)
        {
            QueryString = "EXEC USP_CheckActivePaymentLink '" + PRNo.Replace("'", "''") + "', " + Amount;
            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_Payment_Links> list = DataRowToObject.CreateListFromTable<CYGNUS_Payment_Links>(Dt);
            return list != null && list.Count > 0 ? list[0] : null;
        }

        public void SavePaymentLink(string PRNo, string merchantOrderId, decimal Amount, string paymentLink, string paymentToken, string status, string expiryIso, string createdBy)
        {
            QueryString = "EXEC USP_SavePaymentLink '" + PRNo+ "', '" + merchantOrderId + "', " + Amount + ", '" + paymentLink.Replace("'", "''") + "', '" + paymentToken.Replace("'", "''") + "', '" + status.Replace("'", "''") + "', '" + expiryIso.Replace("'", "''") + "', '" + createdBy + "'";
            GF.SaveRequestServices(QueryString.Replace("'", "''"), "SavePaymentLink", "", "");
            GF.ExecuteNonQuery(QueryString);
        }

        public void UpdatePaymentLinkStatus(string merchantOrderId, string status, string transactionId = null, string paymentMode = null, string paymentDateTime = null, string updatedBy = null)
        {
            QueryString = "EXEC USP_UpdatePaymentLinkStatus '" + merchantOrderId + "', '" + status.Replace("'", "''") + "', " +
                          (transactionId != null ? "'" + transactionId.Replace("'", "''") + "'" : "NULL") + ", " +
                          (paymentMode != null ? "'" + paymentMode.Replace("'", "''") + "'" : "NULL") + ", " +
                          (paymentDateTime != null ? "'" + paymentDateTime.Replace("'", "''") + "'" : "NULL") + ", " +
                          (updatedBy != null ? "'" + updatedBy + "'" : "NULL");
            GF.SaveRequestServices(QueryString.Replace("'", "''"), "UpdatePaymentLinkStatus", "", "");
            GF.ExecuteNonQuery(QueryString);
        }
        #endregion

        #region
        public DataTable SaveTripNotes(string xmlData, string baseUserName, string companyCode)
        {
            string QueryString = "EXEC USP_SaveTripNote '" + xmlData + "','" + baseUserName + "','" + companyCode + "'";
            int id = GF.SaveRequestServices(QueryString.Replace("'", "''"), "SaveTripNotes", "", "");
            DataTable dt = GF.GetDataTableFromSP(QueryString);
            return dt;
        }

        public List<CYGNUS_TripNotes> GetTripNotesList(string tripNo)
        {
            string QueryString = "EXEC USP_GetTripNotesList '" + tripNo + "'";
            DataTable dt = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_TripNotes> list = DataRowToObject.CreateListFromTable<CYGNUS_TripNotes>(dt);
            return list;
        }
        #endregion

        public DataSet GetTripOperationCount(string PRNo, string TripNo, string VehicleNo)
        {
            QueryString = "USP_TripOperationCount '" + PRNo + "','" + TripNo + "','" + VehicleNo + "'";
            DataSet ds = GF.GetDataSetFromSP(QueryString);
            return ds;
        }

        #region VehicleEvent
        public DataTable SaveVehicleEvent(string xml, string BaseUserName, string CompanyCode)
        {
            QueryString = "USP_SaveVehicleEvent '" + xml + "','" + BaseUserName + "','" + CompanyCode + "'";
            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            return Dt;
        }
        #endregion

        #region PODList      
        public List<CYGNUS_DO_POD_Upload> GetAllPODList()
        {
            QueryString = "EXEC USP_GetAll_POD_Upload";
            DataTable dt = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_DO_POD_Upload> PODList = DataRowToObject.CreateListFromTable<CYGNUS_DO_POD_Upload>(dt);
            return PODList;
        }

        public CYGNUS_DO_POD_Upload GetPODById(int Id)
        {
            QueryString = "EXEC USP_Get_POD_Upload_ById '" + Id + "'";
            DataTable Dt1 = GF.GetDataTableFromSP(QueryString);
            CYGNUS_DO_POD_Upload data = DataRowToObject.CreateListFromTable<CYGNUS_DO_POD_Upload>(Dt1).FirstOrDefault();
            return data;
        }

        public bool EditPODForVerify(int id, string userName)
        {
            QueryString = "EXEC USP_Edit_POD_Upload_For_Verify '" + id + "','" + userName + "'";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            return Convert.ToBoolean(dataTable.Rows[0]["Status"]);
        }
        #endregion
    }
}