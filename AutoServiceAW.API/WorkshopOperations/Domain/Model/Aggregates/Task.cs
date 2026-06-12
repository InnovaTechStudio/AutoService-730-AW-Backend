namespace AutoServiceAW.API.WorkshopOperations.Domain.Model.Aggregates;

public class Task
{
     #region Properties

    /// <summary>
    /// Gets the unique structural identifier for the operational task.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// Gets the unique identifier of the parent work order aggregate this task belongs to.
    /// </summary>
    public int WorkOrderId { get; private set; }

    /// <summary>
    /// Gets the unique identifier of the assigned mechanic technician, or <see langword="null"/> if unassigned.
    /// </summary>
    public int? MechanicId { get; private set; }

    /// <summary>
    /// Gets the structural instruction description of the operation to perform.
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// Gets the current execution lifecycle or workflow state progress indicator string.
    /// </summary>
    public string Status { get; private set; }

    /// <summary>
    /// Gets the triage urgency designation scale index level for the task (e.g., Low, Medium, High).
    /// </summary>
    public string Priority { get; private set; }

    /// <summary>
    /// Gets the total estimated time metrics measured in minutes or arbitrary units required to finish.
    /// </summary>
    public int EstimatedTime { get; private set; }

    /// <summary>
    /// Gets the specific cost valuation assigned exclusively for the technical labor work.
    /// </summary>
    public decimal LaborPrice { get; private set; }

    /// <summary>
    /// Gets the deep technical diagnostic analysis findings logs captured directly by the mechanic actor.
    /// </summary>
    public string TechnicalDiagnosis { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the clear customer-facing translated text version explaining the automotive failure root cause.
    /// </summary>
    public string CustomerExplanation { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the internal-only technical observations or secret notes shared among the workshop crew members.
    /// </summary>
    public string InternalObservation { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the media reference logs storage addresses pointing to visual proof or inspection diagnostic evidence.
    /// </summary>
    public string EvidenceRegistered { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the supervisory administration review authorization assessment state indicator.
    /// </summary>
    public string AdminReviewStatus { get; private set; } = string.Empty;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="Task"/> class setting structural operational parameters.
    /// </summary>
    /// <param name="workOrderId">The parent tracking work order identification key.</param>
    /// <param name="mechanicId">The targeted technical mechanic staff identifier reference.</param>
    /// <param name="description">The action instructions script description text.</param>
    /// <param name="status">The initial lifecycle entry tracking status string.</param>
    /// <param name="priority">The urgency classification severity boundary level.</param>
    /// <param name="estimatedTime">The baseline expected duration window value.</param>
    /// <param name="laborPrice">The standalone financial cost fee for structural labor work.</param>
    public Task(int workOrderId, int? mechanicId, string description, string status, string priority, int estimatedTime, decimal laborPrice)
    {
        WorkOrderId = workOrderId;
        MechanicId = mechanicId;
        Description = description;
        Status = status;
        Priority = priority;
        EstimatedTime = estimatedTime;
        LaborPrice = laborPrice;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Task"/> class with default values.
    /// Required by ORMs such as Entity Framework Core for data mapping.
    /// </summary>
    protected Task() 
    {
        Description = string.Empty;
        Status = string.Empty;
        Priority = string.Empty;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Updates structural metadata definitions, scheduling scopes, financial fees, and operator allocations.
    /// </summary>
    /// <param name="description">The revised technical description task statement.</param>
    /// <param name="status">The updated progress state designation label criteria.</param>
    /// <param name="priority">The modified workflow execution urgency priority.</param>
    /// <param name="estimatedTime">The adjusted total predicted time duration index.</param>
    /// <param name="laborPrice">The updated billing cost evaluation fee parameters.</param>
    /// <param name="mechanicId">The updated or reassigned technician identification profile reference.</param>
    public void Update(string description, string status, string priority, int estimatedTime, decimal laborPrice, int? mechanicId)
    {
        Description = description;
        Status = status;
        Priority = priority;
        EstimatedTime = estimatedTime;
        LaborPrice = laborPrice;
        MechanicId = mechanicId;
    }

    /// <summary>
    /// Patches ongoing flow parameters, diagnostic logging histories, transparency explanations, and oversight flags.
    /// </summary>
    /// <param name="status">The optional progression status override code.</param>
    /// <param name="diagnosis">The mechanic's diagnostic analysis documentation string.</param>
    /// <param name="explanation">The customer-centric transparency documentation narrative.</param>
    /// <param name="observation">The internal service operational auditing remarks.</param>
    /// <param name="evidence">The media uniform path references package data log.</param>
    /// <param name="reviewStatus">The validation approval stage state tracker indicator.</param>
    public void PatchTechnicalData(string status, string diagnosis, string explanation, string observation, string evidence, string reviewStatus)
    {
        Status = string.IsNullOrEmpty(status) ? Status : status;
        TechnicalDiagnosis = diagnosis ?? string.Empty;
        CustomerExplanation = explanation ?? string.Empty;
        InternalObservation = observation ?? string.Empty;
        EvidenceRegistered = evidence ?? string.Empty;
        AdminReviewStatus = reviewStatus ?? string.Empty;
    }
    public decimal MaterialsCost { get; private set; }

    public decimal TotalCost =>
        LaborPrice + MaterialsCost;
    
    public void UpdateMaterialsCost(decimal materialsCost)
    {
        MaterialsCost = materialsCost;
    }
    public ICollection<TaskPart> Parts { get; private set; }
        = new List<TaskPart>();
    public void AddPart(TaskPart part)
    {
        Parts.Add(part);
    }
    
    
    #endregion
}