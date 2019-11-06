using dCForm.Client;
using System;
using System.Collections.Generic;
public class LCR_1005AFORFF_0415 : IDocModel {
    public LCR_1005AFORFF_0415() { }
    public string VendorName { get; set; }
    public string OfficeName { get; set; }
    public string AddressLineOne { get; set; }
    public string PrimaryPhone { get; set; }
    public System.DateTime DateOfInspection { get; set; }
    public System.DateTime TimeOfInspection { get; set; }
    public string InspectorName { get; set; }
    public bool CbInspectionPurposeLicensingRequirement { get; set; }
    public bool CbInspectionPurposeLifeSafetyRequirements { get; set; }
    public bool CbInspectionPurposeAuditRequirements { get; set; }
    public bool CbInspectionPurposeConductAnInquiry { get; set; }
    public bool CbInspectionPurposeOther { get; set; }
    public string InspectionPurposeOtherText { get; set; }
    public string QuestionsSuperVisorName { get; set; }
    public string QuestionsSuperVisorPhone { get; set; }
    public string AdminReviewSuperVisorName { get; set; }
    public string AdminReviewSuperVisorPhone { get; set; }

    //public string ProviderSignatureImage { get; set; }


    public string RbProviderNotPresentProviderRefusedToSign { get; set; }
    //public bool ProviderRefusedToSign { get; set; }

    //public string InspectorSignatureImage { get; set; }
    //public System.DateTime InspectorDateSigned { get; set; }


    //Signature Component Service Provider
    //public string ServiceProviderPrintName { get; set; }
    public string ProviderSignatureImage { get; set; }
    public System.DateTime ProviderDateSigned { get; set; }


    //Signature Component HCBS Auditor
    //public string HcbsAuditorPrintName { get; set; }
    public string InspectorSignatureImage { get; set; }
    public System.DateTime InspectorDateSigned { get; set; }
}
