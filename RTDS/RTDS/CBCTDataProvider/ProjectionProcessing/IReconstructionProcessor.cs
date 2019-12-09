using System;
using System.Threading.Tasks;

namespace RTDS.CBCTDataProvider.ProjectionProcessing
{
    internal interface IReconstructionProcessor
    {
        Task StartConsumingProjections();

        event EventHandler<string> ImageReconstructed;
    }
}