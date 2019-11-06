using System;
using System.Collections.Generic;
using dCForm.Client;
public class LCR_1000AFORFF_0114 : IDocModel {
    public string AgencyNamePage1Page2 { get; set; }
    public string AgencyRepNamePage1Page2 { get; set; }
    public DateTime AuditDatePage1Page2 { get; set; }
    public bool Cb03RespiratoryTherapy { get; set; }
    public bool Cb05OccupationalTherapy { get; set; }
    public bool Cb06PhysicalTherapy { get; set; }
    public bool Cb07SPTHearingTherapy { get; set; }
    public bool Cb19ICFMR { get; set; }
    public bool Cb20Hospice { get; set; }
    public bool Cb23Homemaker { get; set; }
    public bool Cb26Respite { get; set; }
    public bool Cb28AttendantCare { get; set; }
    public bool Cb29HomeHealthAide { get; set; }
    public bool Cb30HomeHealthNurse { get; set; }
    public bool Cb31NonEmergencyTransportation { get; set; }
    public bool Cb32Habilitation { get; set; }
    public bool Cb39PersonalCareAttendant { get; set; }
    public bool Cb42FORMayCare { get; set; }
    public bool Cb46EnvironmentalModifications { get; set; }
    public bool cbOther { get; set; }
    public string OtherText { get; set; }
    public int TotalDirectCareStaff { get; set; }
    public int TotalFilesAudited { get; set; }
    public string RbAreServicesDeliveredYesNo { get; set; }
    public string AreServicesDeliveredYesAddress { get; set; }
    public DateTime AreServicesDeliveredYesDate { get; set; }
    public string AreServicesDeliveredYesInspectedBy { get; set; }
    public string RbHasAnOLCRInspectionYesNoNA { get; set; }
    public string RbIsAgencyMedicareCertiFiedYesNo { get; set; }
    public string RbTransportingYesNo { get; set; }
    public string RbInsuranceRegistrationCurrentYesNo { get; set; }
    public string RbIfYesAgencyStaffMatrixYesNo { get; set; }
    public string RbAreClassesForCPRYesNo { get; set; }
    public string RbDoesTheDirectorYesNo { get; set; }
    public string RbIfYesDoesTheDirectorYesNo { get; set; }
    public DateTime IfNoDoesTheDirectorExpDate { get; set; }
    public string RbReceivedCurrentAgencyStaffYesNo { get; set; }
    public string RbIsTheAgencyRequestingYesNo { get; set; }
    public string NotesFindingsPage1 { get; set; }
    public string RbAgencyWasInFullNotInFullButCorrectedNotInFullEvidence { get; set; }
    public string NotesFindingsPage2 { get; set; }
    
    //Reason for not including complex structure is because engine creates a separate table for each complex data structure.
    //hence keeping it here so columns gets created in the same table LCR_1000AFORFF_0114 for easy access.

    //Signature Component Service Provider
    public string ServiceProviderPrintName { get; set; }
    public string ServiceProviderSignatureImage { get; set; }
    public string ServiceProviderDateSigned { get; set; }
    

    //Signature Component HCBS Auditor
    public string HcbsAuditorPrintName { get; set; }
    public string HcbsAuditorSignatureImage { get; set; }
    public string HcbsAuditorDateSigned { get; set; }
    
}
