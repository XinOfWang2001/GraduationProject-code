using Leap.ApplicationServices.DTO;
using Leap.ApplicationServices.DTO.DataConfig;
using Leap.ApplicationServices.DTO.DataResult;
using Leap.ApplicationServices.DTO.ModelDTO;
using Leap.ApplicationServices.DTO.Workspace;
using Leap.ApplicationServices.Interfaces.ClientServerProxy;
using LeapDataScienceTool.Common.Services;

namespace LeapDataScienceTool.PageManagers;


public class WorkspaceManager : IWorkspaceManager
{
    private readonly IWorkspaceService workspaceService;
    private readonly IPreviewDataService dataProxyService;
    private readonly ResponseService responseService;

    public IEnumerable<DataColumnDTO> DataColumns { get; set; } = [];
    public WorkspaceConfigDTO WorkspaceConfig { get; set; }
    protected DataExtractConfigDTO? DataExtractConfigDTO { get; set; }
    protected ModelConfigDTO? ModelConfigDTO { get; set; }

    public event EventHandler<DataSourceEventArgs>? DataSourceChanged;

    public WorkspaceManager(IWorkspaceService workspaceService, IPreviewDataService dataProxyService, ResponseService responseService)
    {
        this.workspaceService = workspaceService;
        this.dataProxyService = dataProxyService;
        this.responseService = responseService;
    }

    public IEnumerable<DataColumnDTO> GetColumns()
    {
        return DataColumns.Where((col) => col.ColumnName != "timestamp").OrderBy((col) => col.ColumnName);
    }

    public async Task LoadAllAssets(Guid WorkspaceGuid)
    {
        // Load assets.
        WorkspaceConfig = await workspaceService.GetWorkspace(WorkspaceGuid);
        DataExtractConfigDTO = WorkspaceConfig?.DataSourceConfig;
        // Will be derived from workspace dto, but for now will be instantiated seperately
        ModelConfigDTO = WorkspaceConfig?.ModelConfigDTO;
        responseService.ShowInfoResponse("Alle gegevens zijn ingeladen.");
        await UpdateDataColumns();
    }

    public async Task UpdateDataColumns()
    {
        // Always reset data columns to empty. 
        // This is needed because this service is created once and functions as a singleton.
        // Reseting this variable, will prevent this data from one workspace to another.
        DataColumns = [];
        try
        {            // Call services if data extract config is not null.
            if (DataExtractConfigDTO != null)
            {

                PreviewDataDTO? previewData = await dataProxyService.GetPreviewData(WorkspaceConfig.WorkspaceGuid);
                // Set results to DataColumns
                if (previewData != null)
                {
                    DataColumns = previewData.DataColumns;

                }
                else
                {
                    Console.WriteLine("Chosen sensors did not provide with data. Select another set.");
                    responseService.ShowErrorResponse("Voor deze databron instellingen zijn geen gegevens beschikbaar. Voer alstublieft andere sensoren in.");
                }
            }
        }
        catch
        {
            DataColumns = [];
            Console.Error.WriteLine("Chosen sensors did not provide with data. Select another set.");
            responseService.ShowErrorResponse("Voor deze databron instellingen zijn geen gegevens beschikbaar. Voer alstublieft andere sensoren in.");
        }
    }

    public async Task UpdateDataSourceConfig(DataExtractConfigDTO config)
    {
        WorkspaceConfig.DataSourceConfig = config;
        DataExtractConfigDTO = config;
        responseService.ShowSuccessfullResponse("Databron is succesvol gewijzigd.");
        // Call update data columns
        await UpdateDataColumns();
        // Filter out invalid features and 
        if (ModelConfigDTO != null)
        {
            var datasourceEventArgs = FilterOutInvalidColumns();
            OnChangeDataSourceChange(datasourceEventArgs);
        }
    }

    private void OnChangeDataSourceChange(DataSourceEventArgs args)
    {
        DataSourceChanged?.Invoke(this, args);
    }

    public void UpdateModelConfig(ModelConfigDTO modelConfig)
    {
        ModelConfigDTO = modelConfig;
        responseService.ShowSuccessfullResponse("Modelparameters zijn succesvol gewijzigd.");
    }

    public ModelConfigDTO? GetModelConfig()
    {
        return ModelConfigDTO;
    }

    public DataExtractConfigDTO? GetDataExtractConfigDTO()
    {
        return DataExtractConfigDTO;
    }
    public WorkspaceConfigDTO GetWorkspaceConfigDTO()
    {
        return WorkspaceConfig;
    }

    private DataSourceEventArgs FilterOutInvalidColumns()
    {
        HashSet<DataColumnDTO> CurrentColumnsIds = [.. DataColumns.Select(dc => dc)];
        int FeatureCountBefore = ModelConfigDTO.Features.Count();
        int TargetCountBefore = ModelConfigDTO.Targets.Count();
        ModelConfigDTO.Features = ModelConfigDTO.Features.Where(feature => CurrentColumnsIds.Contains(feature));
        ModelConfigDTO.Targets = ModelConfigDTO.Targets.Where(target => CurrentColumnsIds.Contains(target));
        // You may want to return something meaningful here
        return new DataSourceEventArgs()
        {
            FeatureChanges = FeatureCountBefore - ModelConfigDTO.Features.Count(),
            TargetChanges = TargetCountBefore - ModelConfigDTO.Targets.Count()
        };
    }
}
