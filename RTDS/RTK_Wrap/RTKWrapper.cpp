//Reference: https://gitlab.com/agravgaard/cbctrecon/tree/master/Examples

#include "pch.h"

#define ITK_WRAPPING_PARSER
#include <itkImage.h>

#include <itkImageFileWriter.h>

#include <rtkProjectionsReader.h>
#include <rtkConfiguration.h>
#include <rtkGgoFunctions.h>

#include <rtkDisplacedDetectorImageFilter.h>
#include <rtkFDKConeBeamReconstructionFilter.h>
#include <rtkParkerShortScanImageFilter.h>
#include <rtkProjectionsReader.h>
#include <rtkThreeDCircularProjectionGeometryXMLFile.h>
#ifdef USE_CUDA
#include <rtkCudaFDKConeBeamReconstructionFilter.h>
#endif
#ifdef RTK_USE_OPENCL
#include "rtkOpenCLFDKConeBeamReconstructionFilter.h"
#endif
#include "itkImageFileReader.h"
#include "itkMetaImageIOFactory.h"

#include "RTKWrapper.h"

struct args_info_onthefly_recon {
	bool verbose_flag = false;
	char* hardware_arg; // "cpu"
	std::vector<std::string> filenames;
	int n_filenames;
	double sdd = 3000; // mm
	double sid = 1500; // mm
	bool nodisplaced_flag = false;
	std::string output_arg;
	double hann_arg = 1.0;
	double pad_arg = 0.0;
	unsigned int dimension_given = 0;
	std::array<int, 3> dimension_arg = { {512, 512, 200} };
	std::array<int, 3> spacing_arg = { { 1, 1, 1 } };
	unsigned int spacing_given = 0;
	unsigned int origin_given = 0;
	std::array<int, 3> origin_arg = { {0, 0, 0} };
	bool direction_given = false;
	int* direction_arg;
	bool like_given = false;
	char* like_arg;
};

enum enDeviceType { CUDA_DEVT, CPU_DEVT, OPENCL_DEVT };

template <enDeviceType Tdev, typename pixel_type, unsigned int dim>
struct fdk_type {
	using ImageType = itk::Image<pixel_type, dim>;
	using type = rtk::FDKConeBeamReconstructionFilter<ImageType>;
};

#ifdef USE_CUDA
template <typename pixel_type, unsigned int dim>
struct fdk_type<CUDA_DEVT, pixel_type, dim> {
	using ImageType = itk::CudaImage<pixel_type, dim>;
	using type = rtk::FDKConeBeamReconstructionFilter<ImageType>;
};
#endif

#ifdef RTK_USE_OPENCL
template <> struct fdk_type<OPENCL_DEVT, float, 3U> {
	using ImageType = itk::Image<float, 3U>;
	using type = rtk::OpenCLFDKConeBeamReconstructionFilter;
};
#endif


// Pass projection name, projection parameters, last
struct ThreadInfoStruct {
	std::mutex mutex;
	args_info_onthefly_recon* args_info;
	bool stop;
	unsigned int nproj;
	double radius;
	double sid;
	double sdd;
	double gantryAngle;
	double projOffsetX;
	double projOffsetY;
	double outOfPlaneAngle;
	double inPlaneAngle;
	double sourceOffsetX;
	double sourceOffsetY;
	double collimationUInf;
	double collimationUSup;
	double collimationVInf;
	double collimationVSup;
	double minimumOffsetX; // Used for Wang weighting
	double maximumOffsetX;
	std::string fileName;
};

void computeOffsetsFromGeometry(
	rtk::ThreeDCircularProjectionGeometry::Pointer geometry, double* minOffset,
	double* maxOffset) {
	double min = std::numeric_limits<double>::max();
	double max = std::numeric_limits<double>::min();

	for (unsigned int i = 0; i < geometry->GetProjectionOffsetsX().size(); i++) {
		min = std::min(min, geometry->GetProjectionOffsetsX()[i]);
		max = std::max(max, geometry->GetProjectionOffsetsX()[i]);
	}
	*minOffset = min;
	*maxOffset = max;
}

// This thread reads in a geometry file and a sequence of projection file names
// and communicates them one by one to the other thread via a ThreadinfoStruct.
void AcquisitionCallback(ThreadInfoStruct& threadInfo, std::vector<std::string> &names) {
	double minOffset, maxOffset;

	threadInfo.mutex.lock();

		// rtk::GetProjectionsFileNamesFromGgo(*(threadInfo.args_info));
	// Create new RTK geometry object
	using GeometryType = rtk::ThreeDCircularProjectionGeometry;
	auto geometry = GeometryType::New();

	// Constants used to generate projection matrices
	const double sdd = threadInfo.args_info->sdd;
	const double sid = threadInfo.args_info->sid;
	
	// Projections reader (for angle)
	rtk::XimImageIOFactory::RegisterOneFactory();
	// Projection matrices
	for (const std::string& projectionsFileName : names)
	{
		using InputPixelType = unsigned int;
		using InputImageType = itk::Image< InputPixelType, 2 >;

		using ReaderType = itk::ImageFileReader< InputImageType >;
		ReaderType::Pointer reader = ReaderType::New();
		reader->SetFileName(projectionsFileName);
		reader->UpdateOutputInformation();

		using MetaDataDoubleType = itk::MetaDataObject< double >;
		const double angle =
			dynamic_cast<MetaDataDoubleType*>(reader->GetMetaDataDictionary()["dCTProjectionAngle"].GetPointer())->GetMetaDataObjectValue();
		if (angle != 6000)
		{
			/* Warning: The offsets in the test scans were very small,
			however this configuration improved reconstruction quality slightly.*/
			const double offsetx =
				dynamic_cast<MetaDataDoubleType*>(reader->GetMetaDataDictionary()["dDetectorOffsetX"].GetPointer())->GetMetaDataObjectValue();
			const double offsety =
				dynamic_cast<MetaDataDoubleType*>(reader->GetMetaDataDictionary()["dDetectorOffsetY"].GetPointer())->GetMetaDataObjectValue();
			/*The angle-direction of RTK is opposite of the Xim properties
			(There doesn't seem to be a flag for direction in neither the xml nor xim file) */
			geometry->AddProjection(sid, sdd, 180.0 - angle, offsetx, offsety);
		}
	}

	computeOffsetsFromGeometry(geometry, &minOffset,
		&maxOffset);

	threadInfo.mutex.unlock();


	// Mock an inline acquisition
	for (unsigned int i = 0; i < names.size(); i++) {
		threadInfo.mutex.lock();
		threadInfo.sdd = geometry->GetSourceToDetectorDistances()[i];
		threadInfo.sid = geometry->GetSourceToIsocenterDistances()[i];
		threadInfo.gantryAngle = geometry->GetGantryAngles()[i];
		threadInfo.sourceOffsetX = geometry->GetSourceOffsetsX()[i];
		threadInfo.sourceOffsetY = geometry->GetSourceOffsetsY()[i];
		threadInfo.inPlaneAngle = geometry->GetInPlaneAngles()[i];
		threadInfo.outOfPlaneAngle = geometry->GetOutOfPlaneAngles()[i];
		threadInfo.collimationUInf = geometry->GetCollimationUInf()[i];
		threadInfo.collimationUSup = geometry->GetCollimationUSup()[i];
		threadInfo.collimationVInf = geometry->GetCollimationVInf()[i];
		threadInfo.collimationVSup = geometry->GetCollimationVSup()[i];
		threadInfo.minimumOffsetX = minOffset;
		threadInfo.maximumOffsetX = maxOffset;
		threadInfo.fileName = names[std::min(i, (unsigned int)names.size() - 1)];
		threadInfo.nproj = i + 1;
		threadInfo.stop = (i == names.size() - 1);
		if (threadInfo.args_info->verbose_flag)
			std::cout
			<< std::endl
			<< "AcquisitionCallback has simulated the acquisition of projection #"
			<< i << std::endl;
		threadInfo.mutex.unlock();

		std::this_thread::sleep_for(std::chrono::milliseconds(200));
	}

	return;
}


// This thread receives information of each projection (one-by-one) and process
// directly the projections for which it has enough information. This thread
// currently assumes that the projections are sequentially sent with
// increasing gantry angles. Specific management with a queue must be
// implemented if the projections are not exactly sequential. Short
// scans has not been implemented yet because this filter currently
// require the full geometry of the acquisition. Management with a mock
// geometry file would be possible but it is still to be implemented.
template <enDeviceType device>
void InlineThreadCallback(ThreadInfoStruct& threadInfo) {

	threadInfo.mutex.lock();
	typedef float OutputPixelType;
	const unsigned int Dimension = 3;
	using CPUOutputImageType =
		fdk_type<CPU_DEVT, OutputPixelType, Dimension>::ImageType;

	using OutputImageType =
		typename fdk_type<device, OutputPixelType, Dimension>::ImageType;

	rtk::ThreeDCircularProjectionGeometry::Pointer geometry =
		rtk::ThreeDCircularProjectionGeometry::New();
	std::vector<std::string> fileNames;

	// Projections reader
	auto reader = rtk::ProjectionsReader<OutputImageType>::New();

	// Create reconstructed image
	auto constantImageSource = rtk::ConstantImageSource<OutputImageType>::New();
	rtk::SetConstantImageSourceFromGgo<rtk::ConstantImageSource<OutputImageType>,
		args_info_onthefly_recon>(
			constantImageSource, *(threadInfo.args_info));

	// Extract filter to process one projection at a time
	using ExtractFilterType =
		itk::ExtractImageFilter<OutputImageType, OutputImageType>;
	auto extract = ExtractFilterType::New();
	extract->SetInput(reader->GetOutput());
	typename ExtractFilterType::InputImageRegionType subsetRegion;

	// Displaced detector weighting
	auto ddf = rtk::DisplacedDetectorImageFilter<OutputImageType>::New();
	ddf->SetInput(extract->GetOutput());
	ddf->SetGeometry(geometry);
	ddf->SetDisable(threadInfo.args_info->nodisplaced_flag);

	// Short scan image filter
	//  typedef rtk::ParkerShortScanImageFilter< OutputImageType > PSSFType;
	//  PSSFType::Pointer pssf = PSSFType::New();
	//  pssf->SetInput( ddf->GetOutput() );
	//  pssf->SetGeometry( geometryReader->GetOutputObject() );
	//  pssf->InPlaceOff();

	auto fdk_input_reader = itk::ImageFileReader < itk::Image<float, 3>>::New();
	fdk_input_reader->SetFileName(threadInfo.args_info->output_arg);
	itk::Image<float, 3>::Pointer input_fdk_img;
	try {
		fdk_input_reader->Update();
		input_fdk_img = fdk_input_reader->GetOutput();
	}
	catch (std::exception & e) {
		input_fdk_img = constantImageSource->GetOutput();
	}

	// FDK reconstruction filtering
	using FDKType = typename fdk_type<device, OutputPixelType, Dimension>::type;
	typename FDKType::Pointer fdk = FDKType::New();
	fdk->SetInput(0, input_fdk_img);
	fdk->SetInput(1, ddf->GetOutput());
	fdk->SetGeometry(geometry);
	fdk->GetRampFilter()->SetTruncationCorrection(threadInfo.args_info->pad_arg);
	fdk->GetRampFilter()->SetHannCutFrequency(threadInfo.args_info->hann_arg);

	// Writer
	itk::MetaImageIOFactory::RegisterOneFactory();
	typedef itk::ImageFileWriter<CPUOutputImageType> WriterType;
	WriterType::Pointer writer = WriterType::New();
	writer->SetFileName(threadInfo.args_info->output_arg);

	// Set the cylindrical detector's radius
	geometry->SetRadiusCylindricalDetector(threadInfo.radius);

	threadInfo.mutex.unlock();

	// Inline loop
	std::cout << "Reconstruction thread has entered in the processing loop"
		<< std::endl;
	for (;;) {
		threadInfo.mutex.lock();

		if (geometry->GetMatrices().size() < threadInfo.nproj) {
			if (threadInfo.args_info->verbose_flag)
				std::cerr << "InlineThreadCallback has received projection #"
				<< threadInfo.nproj - 1 << std::endl;

			if (threadInfo.fileName != "" &&
				(fileNames.size() == 0 || fileNames.back() != threadInfo.fileName))
				fileNames.push_back(threadInfo.fileName);

			geometry->AddProjectionInRadians(
				threadInfo.sid, threadInfo.sdd, threadInfo.gantryAngle,
				threadInfo.projOffsetX, threadInfo.projOffsetY,
				threadInfo.outOfPlaneAngle, threadInfo.inPlaneAngle,
				threadInfo.sourceOffsetX, threadInfo.sourceOffsetY);
			geometry->SetCollimationOfLastProjection(
				threadInfo.collimationUInf, threadInfo.collimationUSup,
				threadInfo.collimationVInf, threadInfo.collimationVSup);

			std::cout << "Geometry size : " << geometry->GetMatrices().size()
				<< std::endl;

			if (geometry->GetMatrices().size() != threadInfo.nproj) {
				std::cerr << "Missed one projection in InlineThreadCallback"
					<< std::endl;
				exit(EXIT_FAILURE);
			}
			if (geometry->GetMatrices().size() < 3) {
				threadInfo.mutex.unlock();
				continue;
			}

			typename OutputImageType::RegionType region =
				reader->GetOutput()->GetLargestPossibleRegion();
			std::cout << "Reader size : " << region.GetSize()[0] << " "
				<< region.GetSize()[1] << " " << region.GetSize()[2]
				<< std::endl;
			std::cout << "Reader index : " << region.GetIndex()[0] << " "
				<< region.GetIndex()[1] << " " << region.GetIndex()[2]
				<< std::endl;

			reader->SetFileNames(fileNames);
			TRY_AND_EXIT_ON_ITK_EXCEPTION(reader->UpdateOutputInformation())
				subsetRegion = reader->GetOutput()->GetLargestPossibleRegion();
			subsetRegion.SetIndex(Dimension - 1, geometry->GetMatrices().size() - 2);
			subsetRegion.SetSize(Dimension - 1, 1);
			extract->SetExtractionRegion(subsetRegion);

			std::cout << "Region size : " << subsetRegion.GetSize()[0] << " "
				<< subsetRegion.GetSize()[1] << " " << subsetRegion.GetSize()[2]
				<< std::endl;
			std::cout << "Region index : " << subsetRegion.GetIndex()[0] << " "
				<< subsetRegion.GetIndex()[1] << " "
				<< subsetRegion.GetIndex()[2] << std::endl;

			typename ExtractFilterType::InputImageRegionType extractRegion =
				extract->GetOutput()->GetLargestPossibleRegion();

			std::cout << "Extract region size : " << extractRegion.GetSize()[0] << " "
				<< extractRegion.GetSize()[1] << " "
				<< extractRegion.GetSize()[2] << std::endl;
			std::cout << "Extract region index : " << extractRegion.GetIndex()[0]
				<< " " << extractRegion.GetIndex()[1] << " "
				<< extractRegion.GetIndex()[2] << std::endl;

			ddf->SetOffsets(threadInfo.minimumOffsetX, threadInfo.maximumOffsetX);

			TRY_AND_EXIT_ON_ITK_EXCEPTION(fdk->Update())
				typename OutputImageType::Pointer pimg = fdk->GetOutput();
			pimg->DisconnectPipeline();
			fdk->SetInput(pimg);
			TRY_AND_EXIT_ON_ITK_EXCEPTION(fdk->GetOutput()->UpdateOutputInformation())
				TRY_AND_EXIT_ON_ITK_EXCEPTION(
					fdk->GetOutput()->PropagateRequestedRegion());

			if (threadInfo.args_info->verbose_flag)
				std::cout << "Projection #" << subsetRegion.GetIndex(Dimension - 1)
				<< " has been processed in reconstruction." << std::endl;

			if (threadInfo.stop) {
				// Process first projection
				subsetRegion.SetIndex(Dimension - 1, 0);
				extract->SetExtractionRegion(subsetRegion);
				TRY_AND_EXIT_ON_ITK_EXCEPTION(fdk->Update())
					typename OutputImageType::Pointer pimg = fdk->GetOutput();
				pimg->DisconnectPipeline();
				fdk->SetInput(pimg);
				TRY_AND_EXIT_ON_ITK_EXCEPTION(
					fdk->GetOutput()->UpdateOutputInformation())
					TRY_AND_EXIT_ON_ITK_EXCEPTION(
						fdk->GetOutput()->PropagateRequestedRegion());
				if (threadInfo.args_info->verbose_flag)
					std::cout << "Projection #" << subsetRegion.GetIndex(Dimension - 1)
					<< " has been processed in reconstruction." << std::endl;

				// Process last projection
				subsetRegion.SetIndex(Dimension - 1,
					geometry->GetMatrices().size() - 1);
				extract->SetExtractionRegion(subsetRegion);
				TRY_AND_EXIT_ON_ITK_EXCEPTION(fdk->Update())
					writer->SetInput(fdk->GetOutput());

				if (threadInfo.args_info->verbose_flag)
					std::cout << "Projection #" << subsetRegion.GetIndex(Dimension - 1)
					<< " has been processed in reconstruction." << std::endl;

				// Write to disk and exit
				TRY_AND_EXIT_ON_ITK_EXCEPTION(writer->Update())
					return;
			}
		}

		threadInfo.mutex.unlock();
	}

	return;
}

int ProcessFiles(char* file_in1, char* file_in2, char* file_in3, char* file_out) {

	args_info_onthefly_recon args_info;
	args_info.filenames.push_back(file_in1);
	args_info.filenames.push_back(file_in2);
	args_info.filenames.push_back(file_in3);
	args_info.output_arg = std::string(file_out);


	// Launch threads, one for acquisition, one for reconstruction with inline
	// processing
	ThreadInfoStruct threadInfo;
	threadInfo.args_info = &args_info;

	threadInfo.nproj = 0;
	threadInfo.minimumOffsetX = 0.0;
	threadInfo.maximumOffsetX = 0.0;

	std::thread thread_acq(AcquisitionCallback, std::ref(threadInfo), std::ref(args_info.filenames));

	std::thread thread_fdk;

	if (!strcmp(args_info.hardware_arg, "cuda")) {
#ifndef USE_CUDA
		std::cerr << "Program was not compiled with cuda option\n";
		return EXIT_FAILURE;
#else
		thread_fdk =
			std::thread(InlineThreadCallback<CUDA_DEVT>, std::ref(threadInfo));
#endif
	}
	else if (!strcmp(args_info.hardware_arg, "opencl")) {
#ifndef RTK_USE_OPENCL
		std::cerr << "Program was not compiled with OpenCL option\n";
		return EXIT_FAILURE;
#else
		thread_fdk =
			std::thread(InlineThreadCallback<OPENCL_DEVT>, std::ref(threadInfo));
#endif
	}
	else {
		thread_fdk =
			std::thread(InlineThreadCallback<CPU_DEVT>, std::ref(threadInfo));
	}

	thread_acq.join();
	thread_fdk.join();
	return 1;
}
