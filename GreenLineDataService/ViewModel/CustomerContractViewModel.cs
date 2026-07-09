using GreenLineDataService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLineDataService.ViewModel
{
    public class CustomerContractViewModel
    {
        public CYGNUS_custcontract_hdr CustomerContract { get; set; }
        public List<CYGNUS_custcontract_hdr> ListCustContract { get; set; }
        public CustomerContractViewModel()
        {
            CustomerContract = new CYGNUS_custcontract_hdr();
            ListCustContract = new List<CYGNUS_custcontract_hdr>();
        }
    }

    public class ContractDocumentsViewModel
    {
        public string ContractId { get; set; }
        public List<DocumentModel> Documents { get; set; }
        public List<DocumentModel> ExistingDocuments { get; set; }
    }

    public class DocumentModel
    {
        public int Id { get; set; }
        public long SrNo { get; set; }
        public string ContractId { get; set; }
        public string Doc_Type { get; set; }
        public string Doc_Type_Desc { get; set; }
        public string Doc_File { get; set; }
        public HttpPostedFileBase fileUpload { get; set; }
    }

    public class ContarctPTLViewModel
    {
        public CYGNUS_CustContract_FRTMatrix_SingleSlab CustsingleSlab { get; set; }
        public List<CYGNUS_CustContract_FRTMatrix_SingleSlab> ListsingleSlab { get; set; }
        public List<RateUpdateModelForFTLPTL> ListForUpdateSlab { get; set; }
    }

    public class ContarctFTLViewModel
    {
        public CYGNUS_CUSTCONTRACT_FRTMATRIX_FTLSLABHDR FTLfreightCharge { get; set; }
        public List<CYGNUS_CUSTCONTRACT_FRTMATRIX_FTLSLABHDR> FTLfreightChargeList { get; set; }
        public List<RateUpdateModelForFTLPTL> ListForUpdateSlab { get; set; }

    }
    public class ContractChargeConstraintViewModel
    {
        public CYGNUS_custcontract_charge_constraint chargeContraint { get; set; }
        public List<CYGNUS_custcontract_charge_constraint> ListContraint { get; set; }

        public ContractChargeConstraintViewModel()
        {
            chargeContraint = new CYGNUS_custcontract_charge_constraint();
            ListContraint = new List<CYGNUS_custcontract_charge_constraint>();
        }
    }

    public class ContractStandardChargeViewModel
    {
        public CYGNUS_custcontract_charge_constraint chargeContraint { get; set; }
        public List<CYGNUS_CustContract_FRTMatrix_SingleSlab> ListsingleSlab { get; set; }
    }

    public class RateUpdateModelForFTLPTL
    {
        public int srno { get; set; }
        public decimal old_rate { get; set; }
        public decimal rate_differnce { get; set; }
        public decimal rate { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string FromLane { get; set; }
        public string ToLane { get; set; }
        public string ProductGroup { get; set; }
        public int FromQty { get; set; }
        public int ToQty { get; set; }
        public int IsEffective { get; set; }
    }
}