
using System;
using System.Collections.Generic;
using dCForm;

//Checkboxes(Cb) are bool and radios(Rb) are string
public class LCR_1023AFORPD_0715 : IDocModel
{
    public string VendorSiteName { get; set; }
    public DateTime DateOfAssessment { get; set; }
    public DateTime TimeOfAssessment { get; set; }
    public string VendorSiteStreet { get; set; }
    public string VendorSiteCity { get; set; }
    public string VendorName { get; set; }
    public string VendorZip { get; set; }
    public string PhoneNumber { get; set; }
    public string RbNewApplicationRenewalAddressChangeSpecialRequest { get; set; }
    public string RbTypeOfSettingHcbsRespiteDayProgramTherapy { get; set; }

    //ASSESMENTS
    public string R618702RbInteriorExteriorInGoodRepairFreeOfDamagePosesAHazardYesNo { get; set; }
    public string R618702InteriorExteriorInGoodRepairFreeOfDamagePosesAHazardNotes { get; set; }
    public string R618702RbPlayAsTherapyEquipmentInGoodRepairYesNo { get; set; }
    public string R618702PlayAsTherapyEquipmentInGoodRepairNotes { get; set; }
    public string R618702RbSettingIsCleanToTheDegreeTheConditionDoesNotConstituteAHazardYesNo { get; set; }
    public string R618702SettingIsCleanToTheDegreeTheConditionDoesNotConstituteAHazardNotes { get; set; }
    public string R618702RbGarbageIsRemovedFromTheSettingPremisesAtLeastOnceEachWeekYesNo { get; set; }
    public string R618702GarbageIsRemovedFromTheSettingPremisesAtLeastOnceEachWeekNotes { get; set; }
    public string R618702RbTheSettingOutsidePlayAsFreeOfInsectRodentManifestationYesNo { get; set; }
    public string R618702TheSettingOutsidePlayAsFreeOfInsectRodentManifestationNotes { get; set; }

    public string R618703RbSettingHasASystemToLockHighlyToxicSubstancesYesNo { get; set; }
    public string R618703SettingHasASystemToLockHighlyToxicSubstancesNotes { get; set; }
    public string R618703RbSettingHasASystemToSafeguardCleaningSuppliesYesNo { get; set; }
    public string R618703SettingHasASystemToSafeguardCleaningSuppliesNotes { get; set; }
    public string R618703SpecifySystemForSafeGuardingNotes { get; set; }
    public string R618703RbWeaponsSafeguardedYesNoNoWeapons { get; set; }
    public string R618703WeaponsSafeguardedNotes { get; set; }
    public string R618703RbFirearmsLockedInAnUnbreakableContainerYesNoNoFireArms { get; set; }
    public string R618703FirearmsLockedInAnUnbreakableContainerNotes { get; set; }
    public string R618703RbFirearmsTriggerLockedOrRenderedInoperableYesNoNoFireArms { get; set; }
    public string R618703FirearmsTriggerLockedOrRenderedInoperableNotes { get; set; }
    public int? R618703NumberOfFirearms { get; set; }
    public string R618703RbAmmunitionIsLockedSeparateFromFirearmsYesNoNoAmmunition { get; set; }
    public string R618703AmmunitionIsLockedSeparateFromFirearmsNotes { get; set; }
    public string R618703RbBathtubsShowersRampsHaveSlipResistantSurfacesYesNo { get; set; }
    public string R618703BathtubsShowersRampsHaveSlipResistantSurfacesNotes { get; set; }
    public string R618703RbHrailsGrabBarsSecurelyAttachedStationaryYesNoNa { get; set; }
    public string R618703HrailsGrabBarsSecurelyAttachedStationaryNotes { get; set; }
    public string R618703RbSkirtingIsIntactAroundTheBaseOfTheSettingIfAMobileHomeYesNoNa { get; set; }
    public string R618703SkirtingIsIntactAroundTheBaseOfTheSettingIfAMobileHomeNotes { get; set; }
    public string R618703RbAnimalsDoNotPoseAHazardDueToBehaviorDiseaseEtcYesNoNa { get; set; }
    public string R618703AnimalsDoNotPoseAHazardDueToBehaviorDiseaseEtcNotes { get; set; }
    public DateTime? R618703AnimalsDoNotPoseAHazardDueToBehaviorDiseaseEtcDateCorrected { get; set; }
    public string R618703RbEvidenceIsAvailableInTheSettingForEachDogSCurrentRabiesYesNoNa { get; set; }
    public string R618703EvidenceIsAvailableInTheSettingForEachDogSCurrentRabiesNotes { get; set; }
    public DateTime? R618703EvidenceIsAvailableInTheSettingForEachDogSCurrentRabiesDateCorrected { get; set; }
    public string R618703VaccinationsTextBox1 { get; set; }
    public string R618703VaccinationsTextBox2 { get; set; }
    public string R618703VaccinationsTextBox3 { get; set; }
    public string R618703VaccinationsTextBox4 { get; set; }

    public string R618704RbPrescriptionOverTheCounterMedicationsInTheSettingLockedYesNo { get; set; }
    public string R618704PrescriptionOverTheCounterMedicationsInTheSettingLockedNotes { get; set; }
    public DateTime? R618704PrescriptionOverTheCounterMedicationsInTheSettingLockedDateCorrected { get; set; }
    //public DateTime? R618704nPrescriptionOverTheCounterMedicationsInTheSettingLockedDateCorrected { get; set; }
    
    public string R618704RbMedicationsMustBeReadilyAvailableOrMayBeAccessedPerAnIndividualSCasePlanSafeguardedYesNoNa { get; set; }
    public string R618704MedicationsMustBeReadilyAvailableOrMayBeAccessedPerAnIndividualSCasePlanSafeguardedNotes { get; set; }
    public DateTime? R618704MedicationsMustBeReadilyAvailableOrMayBeAccessedPerAnIndividualSCasePlanSafeguardedDateCorrected { get; set; }

    public string R618704MedicationsMustBeRefrigeratedLockedWithoutPreventingAccessToRefrigeratedFoodYesNoNa { get; set; }
    public string R618704MedicationsMustBeRefrigeratedLockedWithoutPreventingAccessToRefrigeratedFoodNotes { get; set; }
    public DateTime? R618704MedicationsMustBeRefrigeratedLockedWithoutPreventingAccessToRefrigeratedFoodDateCorrected { get; set; }


    public string R618705RbAppliancesForRefrigeratingCookingFoodFunctioningSafeYesNo { get; set; }
    public string R618705AppliancesForRefrigeratingCookingFoodFunctioningSafeNotes { get; set; }
    public int? R618705RefrigeratorTemperature { get; set; }
    public string R618705RbSettingHasSufficientLightingToPerformNormalActivitiesMBedroomsLivingProgramAsYesNo { get; set; }
    public string R618705SettingHasSufficientLightingToPerformNormalActivitiesMBedroomsLivingProgramAsNotes { get; set; }
    public string R618705RbSettingHasAdequateHeatingAndCoolingAsYesNo { get; set; }
    public string R618705SettingHasAdequateHeatingAndCoolingAsNotes { get; set; }
    public int? R618705InteriorTemperature { get; set; }
    public string R618705RbSettingHasAnOperableTelephoneYesNo { get; set; }
    public string R618705SettingHasAnOperableTelephoneNotes { get; set; }
    public string R618705RbTheClothesDryerIsSafelyVentedWithANonFlammableVentHoseYesNoNa { get; set; }
    public string R618705TheClothesDryerIsSafelyVentedWithANonFlammableVentHoseNotes { get; set; }
    public string R618705RbEachPortableHeaterMeetsTheFollowingStardsYesNoNa { get; set; }
    public string R618705EachPortableHeaterMeetsTheFollowingStardsNotes { get; set; }

    public string R618705RbElectricUlApprovedEquippedWithATipOverShutOffSwitchYesNo { get; set; }
    public string R618705ElectricUlApprovedEquippedWithATipOverShutOffSwitchNotes { get; set; }
    public string R618705RbHasAProtectiveCoveringForTheHeatingElementYesNo { get; set; }
    public string R618705HasAProtectiveCoveringForTheHeatingElementNotes { get; set; }
    public string R618705RbIsPlacedAtLeast3IiFromFlammableObjectWhenInUseYesNo { get; set; }
    public string R618705IsPlacedAtLeast3IiFromFlammableObjectWhenInUseNotes { get; set; }
    public string R618705RbIsNotUsedInBedroomsOrAsThePrimarySourceForHeatInTheSettingYesNo { get; set; }
    public string R618705RbIsNotUsedInBedroomsOrAsThePrimarySourceForHeatInTheSettingNotes { get; set; }
    public string R618705RbACarbonMonoxideDetectorIsInstalledOnEachLevelHasAFireBurningApplianceOrHeatingDeviceYesNoNa { get; set; }
    public string R618705ACarbonMonoxideDetectorIsInstalledOnEachLevelHasAFireBurningApplianceOrHeatingDeviceNotes { get; set; }
    
    public string R618706RbWiringAppearsSafeYesNo { get; set; }
    public string R618706WiringAppearsSafeNotes { get; set; }
    public string R618706RbLightSocketsHaveLightBulbsAreSafelyCoveredToPreventElectricalShockYesNo { get; set; }
    public string R618706LightSocketsHaveLightBulbsAreSafelyCoveredToPreventElectricalShockNotes { get; set; }
    public string R618706RbPanelsOutletsAreCoveredAndHaveNoExposedWiringYesNo { get; set; }
    public string R618706PanelsOutletsAreCoveredAndHaveNoExposedWiringNotes { get; set; }
    public string R618706RbOutletsAreNotOverloadedYesNo { get; set; }
    public string R618706OutletsAreNotOverloadedNotes { get; set; }
    public string R618706RbCordsAreInGoodConditionNoBrokenOrFrayedCordsAreInUseYesNo { get; set; }
    public string R618706CordsAreInGoodConditionNoBrokenOrFrayedCordsAreInUseNotes { get; set; }
    public string R618706RbExtensionCordsAreNotUsedOnAPermanentBasisYesNo { get; set; }
    public string R618706ExtensionCordsAreNotUsedOnAPermanentBasisNotes { get; set; }
    public string R618706RbMidSizedAppliancesArePluggedIntoGroundedOutletsPowerStripsYesNo { get; set; }
    public string R618706MidSizedAppliancesArePluggedIntoGroundedOutletsPowerStripsNotes { get; set; }
    public string R618706RbMajorOrAppliancesArePluggedDirectlyIntoGroundedOutletsYesNo { get; set; }
    public string R618706MajorOrAppliancesArePluggedDirectlyIntoGroundedOutletsNotes { get; set; }

    public string R618707RbTheSettingHasAContinuousSourceOfSafeDrinkingWaterYesNo { get; set; }
    public string R618707TheSettingHasAContinuousSourceOfSafeDrinkingWaterNotes { get; set; }
    public string R618707RbHotWaterTemperatureInAreasForBathingDoesNotExceed120FYesNo { get; set; }
    public string R618707HotWaterTemperatureInAreasForBathingDoesNotExceed120FNotes { get; set; }
    public int? R618707hotWaterTemperature { get; set; }
    public string R618707RbSewageDisposalIsFunctioningWithNoVisibleSignsOfLeakageYesNo { get; set; }
    public string R618707SewageDisposalIsFunctioningWithNoVisibleSignsOfLeakageNotes { get; set; }
    public int? R618707numberOfWorkingToilets { get; set; }
    public int? R618707showerAndTubs { get; set; }
    public int? R618707bathroomSinks { get; set; }
    public string R618707RbSettingHasAtLeast1WorkingToiletSinkAndTubShowerPer10ResidentsYesNo { get; set; }
    public string R618707SettingHasAtLeast1WorkingToiletSinkAndTubShowerPer10ResidentsNotes { get; set; }

    public string R618708RbFlammablesCombustiblesStoredMoreThan3FeetFromTheHotWaterHeaterOtherHeatSourcesYesNo { get; set; }
    public string R618708FlammablesCombustiblesStoredMoreThan3FeetFromTheHotWaterHeaterOtherHeatSourcesNotes { get; set; }
    public string R618708RbWorkingFireplacesWoodStovesProtectedByFireScreensYesNoNa { get; set; }
    public string R618708WorkingFireplacesWoodStovesProtectedByFireScreensNotes { get; set; }
    public string R618708RbSettingHasAtLeastOneFunctioningFireExtinguisherWithAMinimumRatingOf2A1ObcOnEachLevelYesNo { get; set; }
    public string R618708SettingHasAtLeastOneFunctioningFireExtinguisherWithAMinimumRatingOf2A1ObcOnEachLevelNotes { get; set; }
    public string R618708RbSettingHasAtLeastOneWorkingSmokeDetectorOnEachLevelYesNo { get; set; }
    public string R618708SettingHasAtLeastOneWorkingSmokeDetectorOnEachLevelNotes { get; set; }
    public string R618708RbSettingHasAtLeastOneWorkingSmokeDetectorInEachBedroomYesNo { get; set; }
    public string R618708SettingHasAtLeastOneWorkingSmokeDetectorInEachBedroomNotes { get; set; }
    public string R618708RbSettingHasAnEmergencyEvacuationPlanWhichMeetsTheFollowingStardsYesNo { get; set; }
    public string R618708SettingHasAnEmergencyEvacuationPlanWhichMeetsTheFollowingStardsNotes { get; set; }
    public string R618708RbIdentifiesTwoRoutesToEvacuateFromBedroomsUsedForCYesNo { get; set; }
    public string R618708IdentifiesTwoRoutesToEvacuateFromBedroomsUsedForCNotes { get; set; }
    public string R618708RbIdentifiesTheLocationOfFireExtinguishersFireEvacuationEquipmentYesNo { get; set; }
    public string R618708IdentifiesTheLocationOfFireExtinguishersFireEvacuationEquipmentNotes { get; set; }
    public string R618708RbDesignatesASafeMeetingPlaceOutsideTheSettingYesNo { get; set; }
    public string R618708DesignatesASafeMeetingPlaceOutsideTheSettingNotes { get; set; }
    public string R618708RbIsMaintainedInTheSettingYesNo { get; set; }
    public string R618708IsMaintainedInTheSettingNotes { get; set; }
    public string R618708RbExitsFromTheSettingUnobstructedYesNo { get; set; }
    public string R618708ExitsFromTheSettingUnobstructedNotes { get; set; }
    public string R618708RbBedroomsUsedForCMustHaveAnExitOpensDirectlyToTheOutsideYesNo { get; set; }
    public string R618708BedroomsUsedForCMustHaveAnExitOpensDirectlyToTheOutsideNotes { get; set; }
    public string R618708RbLocksBarsOnWindowsInBedroomsUsedForCOnDoorsLeadingToTheOutsideHaveAQuickReleaseMechanismYesNo { get; set; }
    public string R618708LocksBarsOnWindowsInBedroomsUsedForCOnDoorsLeadingToTheOutsideHaveAQuickReleaseMechanismNotes { get; set; }
    public string R618708RbSettingsProvidingCTo6OrMoreIndividualsPracticeDocumentAnEvacuationDrillAtLeastOnceEvery3MonthsYesNoNa { get; set; }
    public string R618708SettingsProvidingCTo6OrMoreIndividualsPracticeDocumentAnEvacuationDrillAtLeastOnceEvery3MonthsNotes { get; set; }
    public string R618708RbTheAddressForTheSettingIsPostedVisibleFromTheStreetYesNoNa { get; set; }
    public string R618708TheAddressForTheSettingIsPostedVisibleFromTheStreetNotes { get; set; }

    public string R618709RbPoolsMaintainedNotStagnantClearEnoughToSeeThroughTheWaterToTheBottomSurfaceOfThePoolYesNo { get; set; }
    public string R618709PoolsMaintainedNotStagnantClearEnoughToSeeThroughTheWaterToTheBottomSurfaceOfThePoolNotes { get; set; }
    public string R618709RbIfWaterIsDeeperThan4FtAShepherdsCrookRingBuoyWithAttachedRopeAvailableInThePoolAYesNoNa { get; set; }
    public string R618709IfWaterIsDeeperThan4FtAShepherdsCrookRingBuoyWithAttachedRopeAvailableInThePoolANotes { get; set; }
    public string R618709RbTheEnclosureFenceMeetsTheFollowingStardsYesNoNa { get; set; }
    public string R618709TheEnclosureFenceMeetsTheFollowingStardsNotes { get; set; }
    public string R618709RbTheExteriorSideOfTheFenceIsAtLeast5FtHighWithNoFootHholdsYesNo { get; set; }
    public string R618709TheExteriorSideOfTheFenceIsAtLeast5FtHighWithNoFootHholdsNotes { get; set; }
    public string R618709RbIfChainLinkTheMeshMeasuresLessThan1HorizontallyYesNoNa { get; set; }
    public string R618709IfChainLinkTheMeshMeasuresLessThan1HorizontallyNotes { get; set; }
    public string R618709RbOpeningsMeasureLessThan4InchesYesNo { get; set; }
    public string R618709OpeningsMeasureLessThan4InchesNotes { get; set; }
    public string R618709RbGatesSelfClosingSelfLatchingOpenAwayFromThePoolYesNo { get; set; }
    public string R618709GatesSelfClosingSelfLatchingOpenAwayFromThePoolNotes { get; set; }
    public string R618709RbTheGateLatchIsAtLeast54AboveTheGroundYesNo { get; set; }
    public string R618709TheGateLatchIsAtLeast54AboveTheGroundNotes { get; set; }
    public string R618709RbTheGateToTheEnclosureIsLockedYesNo { get; set; }
    public string R618709TheGateToTheEnclosureIsLockedNotes { get; set; }
    public string R618709RbIfTheSettingConstitutesPartOfTheEnclosureTheFollowingStardsMetYesNoNa { get; set; }
    public string R618709IfTheSettingConstitutesPartOfTheEnclosureTheFollowingStardsMetNotes { get; set; }
    public string R618709RbTheFenceDoesNotInterfereWithSafeEgressFromTheSettingYesNo { get; set; }
    public string R618709TheFenceDoesNotInterfereWithSafeEgressFromTheSettingNotes { get; set; }
    public string R618709RbADoorFromTheSettingDoesNotOpenWithinThePoolEnclosureYesNo { get; set; }
    public string R618709ADoorFromTheSettingDoesNotOpenWithinThePoolEnclosureNotes { get; set; }
    public string R618709RbAWindowInABedroomDesignatedForAnIndividualReceivingCIsNotPositionedWithinThePoolEnclosureYesNo { get; set; }
    public string R618709AWindowInABedroomDesignatedForAnIndividualReceivingCIsNotPositionedWithinThePoolEnclosureNotes { get; set; }
    public string R618709RbOtherWindowsWithinThePoolEnclosurePermanentlySecuredToOpenNoMoreThan4InchesYesNo { get; set; }
    public string R618709OtherWindowsWithinThePoolEnclosurePermanentlySecuredToOpenNoMoreThan4InchesNotes { get; set; }

    public string RbConditionOfSettingFullCompliannceNotFullCompliance { get; set; }
    public string RbConditionOfSettingNotInFullCompliannceLicensingVerifyCorrectionOlcrVerifyCorrection { get; set; }
    public DateTime? DateFullComplianceVerifiedByOlcrDateCorrected { get; set; }
    public string InspectorComments { get; set; }
    //AssessmentCorrections

    public DateTime? R618702InteriorExteriorInGoodRepairFreeOfDamagePosesAHazardDateCorrected { get; set; }
    public DateTime? R618702PlayAsTherapyEquipmentInGoodRepairDateCorrected { get; set; }
    public DateTime? R618702SettingIsCleanToTheDegreeTheConditionDoesNotConstituteAHazardDateCorrected { get; set; }
    public DateTime? R618702GarbageIsRemovedFromTheSettingPremisesAtLeastOnceEachWeekDateCorrected { get; set; }
    public DateTime? R618702TheSettingOutsidePlayAsFreeOfInsectRodentManifestationDateCorrected { get; set; }
    public DateTime? R618703SettingHasASystemToLockHighlyToxicSubstancesDateCorrected { get; set; }
    public DateTime? R618703SettingHasASystemToSafeguardCleaningSuppliesDateCorrected { get; set; }
    public DateTime? R618703WeaponsSafeguardedDateCorrected { get; set; }
    public DateTime? R618703FirearmsLockedInAnUnbreakableContainerDateCorrected { get; set; }
    public DateTime? R618703FirearmsTriggerLockedOrRenderedInoperableDateCorrected { get; set; }
    public DateTime? R618703AmmunitionIsLockedSeparateFromFirearmsDateCorrected { get; set; }
    public DateTime? R618703BathtubsShowersRampsHaveSlipResistantSurfacesDateCorrected { get; set; }
    public DateTime? R618703HrailsGrabBarsSecurelyAttachedStationaryDateCorrected { get; set; }
    public DateTime? R618703SkirtingIsIntactAroundTheBaseOfTheSettingIfAMobileHomeDateCorrected { get; set; }



    public DateTime? R618705AppliancesForRefrigeratingCookingFoodFunctioningSafeDateCorrected { get; set; }
    public DateTime? R618705SettingHasSufficientLightingToPerformNormalActivitiesMBedroomsLivingProgramAsDateCorrected { get; set; }
    public DateTime? R618705SettingHasAdequateHeatingAndCoolingAsDateCorrected { get; set; }
    //public DateTime? R618705InteriorTemperatureDateCorrected { get; set; }//Not Used in HTML right now and causing empty column
    public DateTime? R618705SettingHasAnOperableTelephoneDateCorrected { get; set; }

    public DateTime? R618705TheClothesDryerIsSafelyVentedWithANonFlammableVentHoseDateCorrected { get; set; }
    public DateTime? R618705EachPortableHeaterMeetsTheFollowingStardsDateCorrected { get; set; }
    public DateTime? R618705ElectricUlApprovedEquippedWithATipOverShutOffSwitchDateCorrected { get; set; }
    public DateTime? R618705HasAProtectiveCoveringForTheHeatingElementDateCorrected  { get; set; }
    public DateTime? R618705IsPlacedAtLeast3IiFromFlammableObjectWhenInUseDateCorrected { get; set; }
    public DateTime? R618705IsNotUsedInBedroomsOrAsThePrimarySourceForHeatInTheSettingDateCorrected { get; set; }
    public DateTime? R618705ACarbonMonoxideDetectorIsInstalledOnEachLevelHasAFireBurningApplianceOrHeatingDeviceDateCorrected { get; set; }

    public DateTime? R618706WiringAppearsSafeDateCorrected { get; set; }
    public DateTime? R618706LightSocketsHaveLightBulbsAreSafelyCoveredToPreventElectricalShockDateCorrected { get; set; }
    public DateTime? R618706PanelsOutletsAreCoveredAndHaveNoExposedWiringDateCorrected { get; set; }
    public DateTime? R618706OutletsAreNotOverloadedDateCorrected { get; set; }
    public DateTime? R618706CordsAreInGoodConditionNoBrokenOrFrayedCordsAreInUseDateCorrected { get; set; }
    public DateTime? R618706ExtensionCordsAreNotUsedOnAPermanentBasisDateCorrected { get; set; }
    public DateTime? R618706MidSizedAppliancesArePluggedIntoGroundedOutletsPowerStripsDateCorrected { get; set; }
    public DateTime? R618706MajorOrAppliancesArePluggedDirectlyIntoGroundedOutletsDateCorrected { get; set; }

    public DateTime? R618707TheSettingHasAContinuousSourceOfSafeDrinkingWaterDateCorrected { get; set; }
    public DateTime? R618707HotWaterTemperatureInAreasForBathingDoesNotExceed120FDateCorrected { get; set; }
    public DateTime? R618707SewageDisposalIsFunctioningWithNoVisibleSignsOfLeakageDateCorrected { get; set; }
    public DateTime? R618707SettingHasAtLeast1WorkingToiletSinkAndTubShowerPer10ResidentsDateCorrected { get; set; }

    public DateTime? R618708FlammablesCombustiblesStoredMoreThan3FeetFromTheHotWaterHeaterOtherHeatSourcesDateCorrected { get; set; }
    public DateTime? R618708WorkingFireplacesWoodStovesProtectedByFireScreensDateCorrected { get; set; }
    public DateTime? R618708SettingHasAtLeastOneFunctioningFireExtinguisherWithAMinimumRatingOf2A1ObcOnEachLevelDateCorrected { get; set; }
    public DateTime? R618708SettingHasAtLeastOneWorkingSmokeDetectorOnEachLevelDateCorrected { get; set; }
    public DateTime? R618708SettingHasAtLeastOneWorkingSmokeDetectorInEachBedroomDateCorrected { get; set; }
    public DateTime? R618708SettingHasAnEmergencyEvacuationPlanWhichMeetsTheFollowingStardsDateCorrected { get; set; }
    public DateTime? R618708IdentifiesTwoRoutesToEvacuateFromBedroomsUsedForCDateCorrected { get; set; }
    public DateTime? R618708IdentifiesTheLocationOfFireExtinguishersFireEvacuationEquipmentDateCorrected { get; set; }
    public DateTime? R618708DesignatesASafeMeetingPlaceOutsideTheSettingDateCorrected { get; set; }
    public DateTime? R618708IsMaintainedInTheSettingDateCorrected { get; set; }
    public DateTime? R618708ExitsFromTheSettingUnobstructedDateCorrected { get; set; }
    public DateTime? R618708BedroomsUsedForCMustHaveAnExitOpensDirectlyToTheOutsideDateCorrected { get; set; }
    public DateTime? R618708LocksBarsOnWindowsInBedroomsUsedForCOnDoorsLeadingToTheOutsideHaveAQuickReleaseMechanismDateCorrected { get; set; }
    public DateTime? R618708SettingsProvidingCTo6OrMoreIndividualsPracticeDocumentAnEvacuationDrillAtLeastOnceEvery3MonthsDateCorrected { get; set; }
    public DateTime? R618708TheAddressForTheSettingIsPostedVisibleFromTheStreetDateCorrected { get; set; }
    public DateTime? R618709PoolsMaintainedNotStagnantClearEnoughToSeeThroughTheWaterToTheBottomSurfaceOfThePoolDateCorrected { get; set; }
    public DateTime? R618709IfWaterIsDeeperThan4FtAShepherdsCrookRingBuoyWithAttachedRopeAvailableInThePoolADateCorrected { get; set; }
    public DateTime? R618709TheEnclosureFenceMeetsTheFollowingStardsDateCorrected { get; set; }
    public DateTime? R618709TheExteriorSideOfTheFenceIsAtLeast5FtHighWithNoFootHholdsDateCorrected { get; set; }
    public DateTime? R618709IfChainLinkTheMeshMeasuresLessThan1HorizontallyDateCorrected { get; set; }
    public DateTime? R618709OpeningsMeasureLessThan4InchesDateCorrected { get; set; }
    public DateTime? R618709GatesSelfClosingSelfLatchingOpenAwayFromThePoolDateCorrected { get; set; }
    public DateTime? R618709TheGateLatchIsAtLeast54AboveTheGroundDateCorrected { get; set; }
    public DateTime? R618709TheGateToTheEnclosureIsLockedDateCorrected { get; set; }
    public DateTime? R618709IfTheSettingConstitutesPartOfTheEnclosureTheFollowingStardsMetDateCorrected { get; set; }
    public DateTime? R618709TheFenceDoesNotInterfereWithSafeEgressFromTheSettingDateCorrected { get; set; }
    public DateTime? R618709ADoorFromTheSettingDoesNotOpenWithinThePoolEnclosureDateCorrected { get; set; }
    public DateTime? R618709AWindowInABedroomDesignatedForAnIndividualReceivingCIsNotPositionedWithinThePoolEnclosureDateCorrected { get; set; }
    public DateTime? R618709OtherWindowsWithinThePoolEnclosurePermanentlySecuredToOpenNoMoreThan4InchesDateCorrected { get; set; }
    

    //Signatures
    //Signature Component Inspector
    public string InspectorPrintName { get; set; }
    public string InspectorSignatureImage { get; set; }
    public string InspectorDateSigned { get; set; }


    //Signature Component Provider
    public string ProviderPrintName { get; set; }
    public string ProviderSignatureImage { get; set; }
    public string ProviderDateSigned { get; set; }
}









