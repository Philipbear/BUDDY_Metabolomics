﻿namespace BUDDY.MzmlDataHandler.Parser
{
    public enum MzMlDataFileContent
    {
        Undefined,
        MassSpectrum,
        ChargeInversionMassSpectrum,
        ConstantNeutralGainSpectrum,
        ConstantNeutralLossSpectrum,
        PrecursorIonSpectrum,
        ProductIonSpectrum,
        MS1Spectrum,
        MSnSpectrum,
        CRMSpectrum,
        SIMSpectrum,
        SRMSpectrum,
        PDASpectrum,
        EnhancedMultiplyChargedSpectrum,
        TimeDelayedFragmentationSpectrum,
        ElectromagneticRadiationSpectrum,
        EmissionSpectrum,
        AbsorptionSpectrum,
        TICchromatogram,
        BasepeakChromatogram,
        SICchromatogram,
        MassChromatogram,
        ElectromagneticRadiationChromatogram,
        AbsorptionChromatogram,
        EmissionChromatogram,
        SIMchromatogram,
        SRMchromatogram,
        CRMchromatogram,
    }
}