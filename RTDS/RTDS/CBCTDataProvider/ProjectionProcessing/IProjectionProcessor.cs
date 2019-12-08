using System;

namespace RTDS.CBCTDataProvider.ProjectionProcessing
{
    internal interface IProjectionProcessor
    {
        void StartConsumingProjections();

        event EventHandler<string> ImageReconstructed;
    }
}