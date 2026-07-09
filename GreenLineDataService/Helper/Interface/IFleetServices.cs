using GreenLine.Classes;
using GreenLineDataService.Models;
using GreenLineDataService.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace GreenLineDataService.Helper.Interface
{
    public interface IFleetServices
    {
        #region PickUp Request
        List<CYGNUS_PickUp_Request> GetPickUpRequestDetails(string type = "");
        DataTable AddPickUpRequest(string XML_Main, string Flag, string CompanyCode, string BaseUserName);
        List<CYGNUS_CUSTHDR> GetCustomerOnCustomerGroup(string GRPCD);
        List<CYGNUS_Trip_Expense_Master> GetNatureofexp();
        List<CYGNUS_Trip_Expense_Master> GetTripExpenseMasterList();
        List<CYGNUS_Trip_Expense_Master> GetLrExpenseMasterList();
        List<CYGNUS_FLEET_ENROUTE_EXP> Get_TripExp_List(string PRNo);
        DataTable SaveTripExpense(string TripNo, string PRNo, string VehicleNo, string ExpenseCode, decimal Amount, string PaymentType, bool IsBilled, string DocPath, string BaseUserName);
        DataTable SaveDocketExpense(string xmlData, string companyCode, string userName);
        List<CYGNUS_FLEET_ENROUTE_EXP> GetTripExpenses(string TripNo);
        #endregion

        #region Vehicle Allocation
        List<CYGNUS_Vehicle_Allocation> GetVehicle_AllocationDetails();
        DataTable AddVehicle_Allocation(string XML_Main);
        List<CYGNUS_Vehicle_Master> GetUnallocatedVehicles();
        #endregion

        #region Trip
        List<CYGNUS_Trip_Master> GetTripList(string BaseUserName);
        List<Cygnus_Master_Event> GetEventList();
        List<CYGNUS_Master_Vehicle_Event> GetVehicleEventList();
        DataTable UpdateTripEventStatus(string PRNo, string VehicleNo, int Event_Id, string Username, string TripNo, string Remarks);
        DataTable UpdateTripEventLastStatus(string PRNo, string VehicleNo, int Event_Id, string Username, string TripNo, int StopId, string Remarks);
        //DataTable GetInvoiceOCRData(string filePath);
        List<CYGNUS_LaneMaster> Get_ContractLane(string ContractId);
        DataSet SaveLREntry(string xml, string BaseUserName, string BaseCompanyCode, string BaseFinYear);
        List<CYGNUS_TripStageActivity> GetTripStageByPRNo(string PRNo);
        List<CYGNUS_TripDocket> GetTripLRList(string TripNo, string PRNo, string StopId);
        DataTable SaveDoPodUpload(string xmlData, string companyCode, string userName);
        DataTable GetTripPODCounts(string tripNo, int stopId);
        #endregion

        #region Spot PR Approve/Reject
        List<CYGNUS_PickUp_Request> GetSpotPR_ForApproval();
        #endregion

        #region PaymentLink
        CYGNUS_CUSTHDR GetCustomerDetailsByPRNo(string PRNo);
        List<CYGNUS_Payment_Links> GetPaymentLinks(string PRNo);
        CYGNUS_Payment_Links CheckActivePaymentLink(string PRNo, decimal Amount);
        void SavePaymentLink(string PRNo, string merchantOrderId, decimal Amount, string paymentLink, string paymentToken, string status, string expiryIso, string createdBy);
        void UpdatePaymentLinkStatus(string merchantOrderId, string status, string transactionId = null, string paymentMode = null, string paymentDateTime = null, string updatedBy = null);
        #endregion

        #region
        DataTable SaveTripNotes(string xmlData, string baseUserName, string companyCode);
        List<CYGNUS_TripNotes> GetTripNotesList(string tripNo);
        #endregion

        #region TimeLine
        DataSet GetTripOperationCount(string PRNo, string TripNo, string VehicleNo);
        #endregion

        #region VehicleEvent
        DataTable SaveVehicleEvent(string xml, string TripNo, string CompanyCode);
        #endregion

        #region PODList
        List<CYGNUS_DO_POD_Upload> GetAllPODList();
        CYGNUS_DO_POD_Upload GetPODById(int Id);
        bool EditPODForVerify(int id, string userName);
        #endregion 
    }
}