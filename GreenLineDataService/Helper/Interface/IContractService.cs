using GreenLineDataService.Models;
using GreenLineDataService.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenLineDataService.Helper.Interface
{
    public interface IContractService
    {
        #region General & Customer Contract Master Setup
        List<CYGNUS_LaneMaster> GetCustomerLaneId(string CustomerCode);
        List<CYGNUS_custcontract_hdr> GetCustContList(string CoustomerCode, string ContType);
        string getCodeContractID();
        DataTable CustomerChnageStatus(string SrNo, string BaseUserName);
        DataTable GetCustContName(string CoustomerCode, string ContarctID);
        DataTable GetCustomerContractDetails(string ContarctID, string QueryType);
        DataTable GetServiceType(string ContractID);
        DataTable AddNewContract(string custcode, string Paybas, string startdate, string enddate, string ContarctID, string BaseUserName, string BaseCompanyCode);
        DataTable FinalContractSubmit(string ContractID);
        #endregion

        #region Tab 1: Contract Information
        DataTable UpdateContractBasicInfo(string ContractId, string BaseUserName, string BaseCompanyCode, string xmlBasicInfo);
        #endregion

        #region Tab 2: Documents
        DataTable UpdateContractDocuments(string ContractId, string BaseUserName, string BaseCompanyCode, string xmlDocumentInfo);
        DataTable DeleteContractDocument(int docId, string BaseUserName);
        #endregion

        #region Tab 3: Additional Field Configurator
        DataTable UpdateContractAdditinalFieldConfigurator(string ContractId, string BaseUserName, string BaseCompanyCode, string xmlAdditionFieldInfo);
        #endregion

        #region Tab 4: Service Selection
        DataTable AddContractService(string Contractid, string Custcode, string BaseUserName, string BaseCompanyCode, string xmlServiceSelectionInfo);
        #endregion

        #region Tab 5: Mode Wise Selection
        DataTable AddContractModeViceService(string ContractId, string Custcode, string BaseUserName, string BaseCompanyCode, string xmlModeWiseSelectionInfo);
        #endregion

        #region Tab 6: Freight Charge - PTL
        List<CYGNUS_CustContract_FRTMatrix_SingleSlab> GetSinlgeslab(string ContractID, string ChargeType, string ChargeCode, string MatrixType, string TransMode, string rateType);
        DataTable AddFreightChargeChargeMatrix(string xmlFRIGHTCCM, string ContractID, string BaseUserName, string BaseCompanyCode, string ratetype);
        DataTable GetExcelDataFreightCharge(string contractID);
        DataTable UpdatePTLRateChange(string xmlData, string ContractId, string BaseUserName, string BaseCompanyCode);
        DataTable DeleteFreightCharges(int srno, string contractId, string BaseUserName);
        #endregion

        #region Tab 7: Freight Charge - FTL
        List<CYGNUS_CUSTCONTRACT_FRTMATRIX_FTLSLABHDR> ListSundryFTL(string chargecode, string TranMode, string MatrixType, string ftltype, string rate_type, string chargetype, string ConntractId);
        DataTable GetExcelDataFreightChargeFTL(string contractID);
        DataTable InsertSundryFTL_NEW(string XML, string ContractId, string BaseUserName, string BaseCompanyCode, string ftltype, string ratetype);
        DataTable UpdateFTLRateChange(string xmlData, string ContractId, string BaseUserName, string BaseCompanyCode);
        #endregion

        #region Tab 8: Charge Matrix
        List<CYGNUS_custcontract_charge_constraint> GetchargeMatrixList(string ContractId, string type);
        DataTable InsertChargeMatrix1(string ContractId, string BaseUserName, string BaseCompanyCode, string xmlChargeMatrixListInfo);
        #endregion

        #region Tab 9: Standard Charge
        List<CYGNUS_custcontract_charge_constraint> GetStandaradChargeList(string ContractId, string type);
        List<CYGNUS_CustContract_FRTMatrix_SingleSlab> GetSinlgeslabForStandardCharges(string ContractID, string chargecode, string serviceType);
        DataTable AddStandardChargeMatrix(string xmlFRIGHTCCM, string chargecode, string ContractID, string BaseUserName, string BaseCompanyCode, string serviceType);
        #endregion

        #region Tab 10: Billing Configuration
        DataTable AddBillConfigurationInfo(string ContractId, string CustCode, string BaseUserName, string BaseCompanyCode, string xmlBillConfigInfo);
        #endregion
    }
}
