namespace Leap.Domain.Domain.ModelConfig.ModelParams
{
    public abstract class ModelParameters
    {
        public int Id { get; set; }
        public string TypeOfAlgorithm { get; set; }
        public ModelConfiguration ParentConfiguration { get; set; }
        public int ParentConfigurationId { get; set; }
    }
}
